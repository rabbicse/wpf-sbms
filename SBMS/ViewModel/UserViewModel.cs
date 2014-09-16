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
    public class UserViewModel : ViewModelBase
    {
        #region Command(s)
        public CommandHandler<object, object> NewHardwareCommand { get; private set; }
        public CommandHandler<object, object> EditHardwareCommand { get; private set; }
        public CommandHandler<object, object> DeleteHardwareCommand { get; private set; }
        #endregion

        #region Property(s)
        public string Tag { get { return "User"; } }
        private Lazy<OptimizedObservableCollection<User>> _userCollection;
        public OptimizedObservableCollection<User> UserCollection
        {
            get { return _userCollection.Value; }
        }
        private User _selectedUser;
        public User SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                _selectedUser = value;
                OnPropertyChanged(() => SelectedUser);
            }
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
        private UserOperationViewModel _userOperationVM;
        public UserOperationViewModel UserOperationVM
        {
            get
            {
                if (_userOperationVM == null)
                {
                    _userOperationVM = ShellContainer.GetExportedInstance<UserOperationViewModel>();
                    _userOperationVM.OnClosed += _hardwareOperationVM_OnClosed;
                    _userOperationVM.OnSaved += _userOperationVM_OnSaved;
                }
                return _userOperationVM;
            }
        }

        void _userOperationVM_OnSaved(object sender, User e)
        {
            UserCollection.Remove(e);
            UserCollection.Add(e);
            _hardwareOperationVM_OnClosed(sender, new EventArgs { });
        }

        void _hardwareOperationVM_OnClosed(object sender, EventArgs e)
        {
            _userOperationVM.OnClosed -= _hardwareOperationVM_OnClosed;
            _userOperationVM.OnSaved -= _userOperationVM_OnSaved;
            PopupContent = null;
            IsShowPopup = false;
            _userOperationVM = null;
        }
        #endregion

        #region Constructor(s)
        [ImportingConstructor]
        public UserViewModel(IUserView view, CompositionContainer compositionContainer)
        {
            View = view;
            View.ViewModel = this;
            ShellContainer = compositionContainer;
            NewHardwareCommand = new CommandHandler<object, object>(NewHardwareCommandAction);
            EditHardwareCommand = new CommandHandler<object, object>(EditHardwareCommandAction);
            DeleteHardwareCommand = new CommandHandler<object, object>(DeleteHardwareCommandAction);
            _userCollection = new Lazy<OptimizedObservableCollection<User>>();
        }
        #endregion

        #region CommandHandler
        private void NewHardwareCommandAction(object obj)
        {
            UserOperationVM.PrepareView();
            PopupContent = UserOperationVM.View;
            IsShowPopup = true;
        }
        private async void DeleteHardwareCommandAction(object obj)
        {
            await DbHandler.Instance.DeleteUser(obj as User);
            UserCollection.Remove(obj as User);
        }
        private void EditHardwareCommandAction(object obj)
        {
            UserOperationVM.PrepareView(obj as User);
            PopupContent = UserOperationVM.View;
            IsShowPopup = true;
        }
        #endregion

        private async void LoadUsers()
        {
            UserCollection.Clear();
            var collection = await DbHandler.Instance.GetUserCollection();
            UserCollection.AddRange(collection);
        }

        #region ViewModelBase
        public override void OnLoad()
        {
            LoadUsers();
        }

        public override void OnClosing()
        {
            this.Dispose();
        }
        #endregion
    }
}
