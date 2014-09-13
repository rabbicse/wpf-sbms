using Microsoft.Practices.Prism.MefExtensions;
using SBMS.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using EkushApp.Localization;
using EkushApp.Localization.Contracts;
using EkushApp.Logging;
using EkushApp.EmbededDB;
using EkushApp.ShellService.MVVM;
using SBMS.Infrastructure;
using EkushApp.WpfControls.Helper;
using SBMS.Splash;
using EkushApp.Model;
using EkushApp.ShellService.Commands;
using EkushApp.Utility.Threads;
using EkushApp.Utility.Tasks;


namespace SBMS
{
    public class Bootstrapper : MefBootstrapper
    {
        #region MefBootstrapper
        protected override System.Windows.DependencyObject CreateShell()
        {
            return null;
        }
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            Container.ComposeExportedValue(Container);
        }
        protected override void ConfigureAggregateCatalog()
        {
            base.ConfigureAggregateCatalog();
            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(Bootstrapper).Assembly));
            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(IShellService).Assembly));
        }

        public override void Run(bool runWithDefaultConfiguration)
        {
            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            base.Run(runWithDefaultConfiguration);
            InitializingApp();
        }


        #endregion

        #region StartupApplication
        private void InitializingApp()
        {
            // Setting up logging
            LogHandler.SetupLogging(@"\SBMS\SBMS.log", LogLevel.Debug);

            // Setting up culture            
            LocaleManager.SetLocaleAssembly("SBMS.Infrastructure.Properties.Resources", Assembly.Load("SBMS.Infrastructure"));
            LocaleManager.SetApplicationCultures(CultureNames.BANGLA, CultureNames.ENGLISH);
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("bn-BD");

            // Start splash screen at startup
            Splasher.SplashWindow = new SplashWindow();
            Splasher.ShowSplash();

            MessageListener.Instance.ReceiveMessage("Initializing database...");
            MessageListener.Instance.ReceiveProgress(10);

            DbHandler.DatabasePath = Globals.EmbededDB.DB_PATH;

            // TODO TEST, Here we'll delete dirty records etc. before start
            Task.Run(async () =>
                {
                    await DbHandler.Instance.SaveAppUserData(new AppUser
                    {
                        UserId = 1,
                        Username = "admin",
                        Password = "admin",
                        FullName = "Sonali Bank Administrator",
                        Address = "Dhaka",
                        DateOfBirth = new DateTime(1990, 1, 1),
                        Email = "admin@admin.com"
                    });
                });

            MessageListener.Instance.ReceiveProgress(100);
            (LoginViewModel.View as Window).Show();
            Splasher.CloseSplash();
        }
        #endregion

        #region ViewModel(s)
        private ShellViewModel _shellViewModel;
        public ShellViewModel ShellViewModel
        {
            get
            {
                if (_shellViewModel == null)
                {
                    _shellViewModel = Container.GetExportedInstance<ShellViewModel>();
                    _shellViewModel.OnLogout += OnLogout;
                    _shellViewModel.OnShutDown += Application_OnShutdown;
                }
                return _shellViewModel;
            }
        }
        private LoginViewModel _loginViewModel;
        public LoginViewModel LoginViewModel
        {
            get
            {
                if (_loginViewModel == null)
                {
                    _loginViewModel = Container.GetExportedInstance<LoginViewModel>();
                    _loginViewModel.OnLoggedIn += LoginViewModel_OnLoggedIn;
                    _loginViewModel.OnShutdown += Application_OnShutdown;
                }
                return _loginViewModel;
            }
        }

        private void LoginViewModel_OnLoggedIn(object obj)
        {
            OnCloseLoginViewModel();
            (ShellViewModel.View as Window).Show();
        }
        private void OnCloseLoginViewModel()
        {

            if (_loginViewModel != null)
            {
                (_loginViewModel.View as Window).Close();
                _loginViewModel.OnLoggedIn -= LoginViewModel_OnLoggedIn;
                _loginViewModel.OnShutdown -= Application_OnShutdown;
                _loginViewModel = null;
            }
        }

        private void OnLogout(object sender, EventArgs e)
        {
            OnCloseShellViewModel();
            (LoginViewModel.View as Window).Show();
        }

        private void OnCloseShellViewModel()
        {
            if (_shellViewModel != null)
            {
                (_shellViewModel.View as Window).Close();
                _shellViewModel.OnLogout -= OnLogout;
                _shellViewModel.OnShutDown -= Application_OnShutdown;
                _shellViewModel = null;
            }
        }

        private void Application_OnShutdown(object sender, EventArgs e)
        {
            TaskHandler.Shutdown();
            ThreadHandler.Shutdown();
            DbHandler.ShutDownDatabase();
            CommandHelper.Destroy();
            Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                OnCloseShellViewModel();
                OnCloseLoginViewModel();
                Application.Current.Shutdown();
            }));
        }
        #endregion
    }
}
