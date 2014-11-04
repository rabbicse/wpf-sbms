using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EkushApp.EmbededDB;
using EkushApp.ShellService.MVVM;
using EkushApp.Utility.Threads;
using EkushApp.Utility.Extensions;
using Microsoft.Practices.Prism.Events;
using System.Windows.Data;
using EkushApp.Utility.Tasks;
using EkushApp.Utility.WinRegistry;
using SBMS.View;
using EkushApp.ShellService.Commands;
using SBMS.Infrastructure;
using EkushApp.Model;

namespace SBMS.ViewModel
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ShellViewModel : ViewModelBase
    {
        #region Declaration(s)
        private AppUser _appUser;
        #endregion

        #region Command(s)
        public CommandHandler<object, object> LogoutCommand { get; private set; }
        #endregion

        #region Event(s)
        public event EventHandler OnShutDown;
        public event EventHandler OnLogout;
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
        private Lazy<OptimizedObservableCollection<IViewModel>> _tabCollection;
        public OptimizedObservableCollection<IViewModel> TabCollection
        {
            get { return _tabCollection.Value; }
        }
        private IViewModel _selectedTab;
        public IViewModel SelectedTab
        {
            get { return _selectedTab; }
            set
            {
                _selectedTab = value;
                OnPropertyChanged(() => SelectedTab);
            }
        }
        #endregion

        #region ViewModel(s)
        private HardwareViewModel _hardwareVM;
        public HardwareViewModel HardwareVM
        {
            get
            {
                return _hardwareVM ?? ShellContainer.GetExportedInstance<HardwareViewModel>();
            }
        }
        private UserViewModel _userVM;
        public UserViewModel UserVM
        {
            get
            {
                return _userVM ?? ShellContainer.GetExportedInstance<UserViewModel>();
            }
        }
        private SupplierViewModel _supplierVM;
        public SupplierViewModel SupplierVM
        {
            get
            {
                return _supplierVM ?? ShellContainer.GetExportedInstance<SupplierViewModel>();
            }
        }
        private AppUserViewModel _appuserVM;
        public AppUserViewModel AppuserVM
        {
            get
            {
                return _appuserVM ?? ShellContainer.GetExportedInstance<AppUserViewModel>();
            }
        }
        private ReportViewModel _reportVM;
        public ReportViewModel ReportVM
        {
            get
            {
                return _reportVM ?? ShellContainer.GetExportedInstance<ReportViewModel>();
            }
        }
        #endregion

        #region Constructor(s)
        [ImportingConstructor]
        public ShellViewModel(IShellView view, CompositionContainer compositionContainer, IEventAggregator eventAggregator, Lazy<IShellService> shellService)
        {
            View = view;
            View.ViewModel = this;
            ShellContainer = compositionContainer;
            EventAggregator = eventAggregator;
            ShellService = shellService;
            LogoutCommand = new CommandHandler<object, object>(LogoutCommandAction);
            _tabCollection = new Lazy<OptimizedObservableCollection<IViewModel>>();
        }
        public void PrepareView(AppUser appUser)
        {
            _appUser = appUser;
        }
        #endregion

        #region CommandHandler(s)
        private void LogoutCommandAction(object obj)
        {
            if (null != OnLogout)
            {
                OnLogout(this, new EventArgs { });
            }
        }
        #endregion

        #region EventAggregator

        #endregion

        #region ViewModelBase
        public override void OnLoad()
        {
            Version = Globals.Assembly.EXE_VERSION;
            if (_appUser != null && _appUser.RoleId == Role.ADMIN)
            {
                TabCollection.Add(AppuserVM);
            }
            TabCollection.Add(HardwareVM);
            TabCollection.Add(UserVM);
            TabCollection.Add(SupplierVM);
            TabCollection.Add(ReportVM);
            SelectedTab = TabCollection.FirstOrDefault();
        }

        public override void OnClosing()
        {
            this.Dispose();
        }
        #endregion
    }
}
