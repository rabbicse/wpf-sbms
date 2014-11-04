using SBMS.View;
using EkushApp.Localization;
using Microsoft.Practices.Prism.Logging;
using EkushApp.ShellService.Commands;
using EkushApp.ShellService.MVVM;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using EkushApp.Logging;
using EkushApp.Localization.Contracts;
using EkushApp.EmbededDB;
using EkushApp.Utility.Crypto;
using SBMS.Infrastructure;
using EkushApp.Utility.WinRegistry;
using EkushApp.Model;


namespace SBMS.ViewModel
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class LoginViewModel : ViewModelBase
    {
        #region Command(s)
        public CommandHandler<object, object> LoginCommand { get; private set; }
        public CommandHandler<object, object> CloseCommand { get; private set; }
        public CommandHandler<object, object> CheckedLanguageCommand { get; private set; }
        #endregion

        #region Event(s)
        public event EventHandler OnShutdown;
        public event Action<AppUser> OnLoggedIn;
        #endregion

        #region Property(s)
        private string _version;
        public string Version
        {
            get { return _version; }
            set
            {
                _version = value;
                OnPropertyChanged(() => Version);
            }
        }
        private string _username;
        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                OnPropertyChanged(() => Username);
            }
        }
        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged(() => Password);
            }
        }
        private List<CultureBean> _cultureCollection;
        public List<CultureBean> CultureCollection
        {
            get { return _cultureCollection; }
            set
            {
                _cultureCollection = value;
                OnPropertyChanged(() => CultureCollection);
            }
        }
        private bool _isShowPopup;
        public bool IsShowPopup
        {
            get { return _isShowPopup; }
            set
            {
                _isShowPopup = value;
                OnPropertyChanged(() => IsShowPopup);
            }
        }
        private object _popupContent;
        public object PopupContent
        {
            get { return _popupContent; }
            set
            {
                _popupContent = value;
                OnPropertyChanged(() => PopupContent);
            }
        }
        #endregion

        #region Constructor(s)
        [ImportingConstructor]
        public LoginViewModel(ILoginView view)
        {
            View = view;
            View.ViewModel = this;
            LoginCommand = new CommandHandler<object, object>(LoginCommandAction);
            CloseCommand = new CommandHandler<object, object>(CloseCommandAction);
            CheckedLanguageCommand = new CommandHandler<object, object>(CheckedLanguageCommandExecute);
            var _cultureCollection = LocaleManager.DefaultCultureCollection;
            _cultureCollection.Where(c => c.CultureCode.Equals(Thread.CurrentThread.CurrentUICulture.Name)).ToList().ForEach(p => p.IsChecked = true);
            CultureCollection = _cultureCollection;
        }



        private void CheckedLanguageCommandExecute(object e)
        {
            var commandParam = (string)((e as EkushApp.ShellService.Commands.EventToCommandArgs).CommandParameter);
            LocaleManager.SetCurrentLanguage(commandParam);
            RegistryUtility.WriteRegistryKey(Globals.RegistryConstants.KEY_PATH, Globals.RegistryConstants.LOCALE_KEY_NAME, commandParam);
        }
        #endregion

        #region Command Manager(s)
        private async void LoginCommandAction(object obj)
        {
            ShowBusyIndicator("Please wait...\nAuthenticating user");
            await Task.Run(async () =>
            {
                await OnLoginAsync();
            });
            HideBusyIndicator();
        }
        private async void CloseCommandAction(object obj)
        {
            ShowBusyIndicator("Shutting down application.");
            await Task.Run(() =>
            {
                OnClose();
            });
            HideBusyIndicator();
        }
        #endregion

        #region Method(s)
        private async Task OnLoginAsync()
        {
            AppUser user = null;
            if (await DbHandler.Instance.AuthenticateUser(Username, Password, x => user = x))
            {
                Log.Info("Successfully logged in.");
                ViewDispatcher.Invoke((Action)(() =>
                {
                    if (null != OnLoggedIn)
                    {
                        OnLoggedIn(user);
                    }
                }));
            }
            else
            {
                System.Windows.MessageBox.Show("Login failed!");
            }
        }
        private void OnClose()
        {
            ViewDispatcher.Invoke((Action)(() =>
            {
                if (null != OnShutdown)
                {
                    OnShutdown(this, new EventArgs { });
                }
            }));
        }
        #endregion

        #region ViewModelBase
        public override void OnLoad()
        {
            Version = Globals.Assembly.EXE_VERSION;
        }

        public override void OnClosing()
        {
            this.Dispose();
        }
        #endregion
    }
}
