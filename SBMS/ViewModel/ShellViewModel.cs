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

namespace SBMS.ViewModel
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ShellViewModel : ViewModelBase
    {
        #region Command(s)
        public CommandHandler<object, object> LogoutCommand { get; private set; }
        #endregion

        #region Event(s)
        public event EventHandler OnShutDown;
        public event EventHandler OnLogout;
        #endregion

        #region Property(s)
        private Lazy<OptimizedObservableCollection<IViewModel>> _tabCollection;
        public OptimizedObservableCollection<IViewModel> TabCollection
        {
            get { return _tabCollection.Value; }
        }
        #endregion
        #region ViewModel(s)
        private HardwareViewModel _hardwareVM;
        public HardwareViewModel HardwareVM
        {
            get
            {
                if (_hardwareVM == null)
                {
                    _hardwareVM = ShellContainer.GetExportedInstance<HardwareViewModel>();
                }
                return _hardwareVM;
            }
        }
        private UserViewModel _userVM;
        public UserViewModel UserVM
        {
            get
            {
                if (_userVM == null)
                {
                    _userVM = ShellContainer.GetExportedInstance<UserViewModel>();
                }
                return _userVM;
            }
        }
        private SupplierViewModel _supplierVM;
        public SupplierViewModel SupplierVM
        {
            get
            {
                if (_supplierVM == null)
                {
                    _supplierVM = ShellContainer.GetExportedInstance<SupplierViewModel>();
                }
                return _supplierVM;
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
            TabCollection.Add(HardwareVM);
            TabCollection.Add(UserVM);
            TabCollection.Add(SupplierVM);
        }

        public override void OnClosing()
        {
            this.Dispose();
        }
        #endregion
    }
}
