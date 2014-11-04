using EkushApp.EmbededDB;
using EkushApp.Model;
using EkushApp.ShellService.Commands;
using EkushApp.ShellService.MVVM;
using EkushApp.Utility.Extensions;
using SBMS.View;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBMS.ViewModel
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class AppUserViewModel : ViewModelBase
    {
        #region Command(s)
        public CommandHandler<object, object> NewAppUserCommand { get; private set; }
        public CommandHandler<object, object> EditAppUserCommand { get; private set; }
        public CommandHandler<object, object> DeleteAppUserCommand { get; private set; }
        #endregion

        #region Property(s)
        public string Tag
        {
            get
            {
                return "User Management";
            }
        }
        private Lazy<OptimizedObservableCollection<AppUser>> _appUserCollection;
        public OptimizedObservableCollection<AppUser> AppUserCollection
        {
            get { return _appUserCollection.Value; }
        }
        #endregion

        #region Popup
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

        #region ViewModel(s)
        private AppUserOperatorViewModel _appUserOperationVM;
        public AppUserOperatorViewModel AppUserOperationVM
        {
            get
            {
                if (_appUserOperationVM == null)
                {
                    _appUserOperationVM = ShellContainer.GetExportedInstance<AppUserOperatorViewModel>();
                    _appUserOperationVM.OnClosed += AppUserOperationVMOnClosed;
                }
                return _appUserOperationVM;
            }
        }

        void AppUserOperationVMOnClosed(object sender, EventArgs e)
        {
            PopupContent = null;
            IsShowPopup = false;
            _appUserOperationVM = null;
            LoadAppUsers();
        }
        #endregion

        #region Constructor(s)
        [ImportingConstructor]
        public AppUserViewModel(IAppUserView view, CompositionContainer compositionContainer)
        {
            View = view;
            View.ViewModel = this;
            ShellContainer = compositionContainer;
            NewAppUserCommand = new CommandHandler<object, object>(NewAppUserCommandAction);
            EditAppUserCommand = new CommandHandler<object, object>(EditAppUserCommandAction);
            DeleteAppUserCommand = new CommandHandler<object, object>(DeleteAppUserCommandAction);
            _appUserCollection = new Lazy<OptimizedObservableCollection<AppUser>>();
        }

        private async void DeleteAppUserCommandAction(object obj)
        {
            await DbHandler.Instance.DeleteAppUser(obj as AppUser);
        }

        private void EditAppUserCommandAction(object obj)
        {
            AppUserOperationVM.PrepareView(obj as AppUser);
            PopupContent = AppUserOperationVM.View;
            IsShowPopup = true;
        }

        private void NewAppUserCommandAction(object obj)
        {
            PopupContent = AppUserOperationVM.View;
            IsShowPopup = true;
        }
        #endregion

        #region ViewModelBase
        public override void OnLoad()
        {
            LoadAppUsers();
        }
        private async void LoadAppUsers()
        {
            AppUserCollection.Clear();
            var appUsers = await DbHandler.Instance.GetUsers();
            AppUserCollection.AddRange(appUsers);
        }

        public override void OnClosing()
        {
            Dispose();
        }
        #endregion
    }
}
