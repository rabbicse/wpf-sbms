using EkushApp.EmbededDB;
using EkushApp.Model;
using EkushApp.ShellService.Commands;
using EkushApp.ShellService.MVVM;
using EkushApp.Utility.Extensions;
using SBMS.View;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SBMS.ViewModel
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class AppUserOperatorViewModel : ViewModelBase
    {
        #region Command(s)
        public CommandHandler<object, object> SaveCommand { get; private set; }
        public CommandHandler<object, object> CloseCommand { get; private set; }
        #endregion

        #region Event(s)
        public event EventHandler OnClosed;
        #endregion

        #region Property(s)
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
        private string _passwordReEnter;
        public string PasswordReEnter
        {
            get { return _passwordReEnter; }
            set
            {
                _passwordReEnter = value;
                OnPropertyChanged(() => PasswordReEnter);
            }
        }
        private string _fullName;
        public string FullName
        {
            get { return _fullName; }
            set
            {
                _fullName = value;
                OnPropertyChanged(() => FullName);
            }
        }
        private string _email;
        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                OnPropertyChanged(() => Email);
            }
        }
        private string _mobileNo;
        public string MobileNo
        {
            get { return _mobileNo; }
            set
            {
                _mobileNo = value;
                OnPropertyChanged(() => MobileNo);
            }
        }
        private UserRole _selectedRole;
        public UserRole SelectedRole
        {
            get { return _selectedRole; }
            set
            {
                _selectedRole = value;
                OnPropertyChanged(() => SelectedRole);
            }
        }
        private OptimizedObservableCollection<UserRole> _userRoles;
        public OptimizedObservableCollection<UserRole> UserRoles
        {
            get { return _userRoles; }
        }
        #endregion

        #region Constructor(s)
        [ImportingConstructor]
        public AppUserOperatorViewModel(IAppUserOperatorView view)
        {
            View = view;
            View.ViewModel = this;
            SaveCommand = new CommandHandler<object, object>(SaveCommandAction);
            CloseCommand = new CommandHandler<object, object>(CloseCommandAction);
            _userRoles = new OptimizedObservableCollection<UserRole>();
            _userRoles.Add(new UserRole { RoleId = Role.ADMIN, RoleName = "ADMIN"});
            _userRoles.Add(new UserRole { RoleId = Role.MANAGER, RoleName = "MANAGER" });
            _userRoles.Add(new UserRole { RoleId = Role.OPERATOR, RoleName = "OPERATOR" });
        }

        public void PrepareView(AppUser user)
        {
            Username = user.Username;
            FullName = user.FullName;
            Email = user.Email;
            MobileNo = user.MobileNo;
            SelectedRole = _userRoles.FirstOrDefault(role => role.RoleId == user.RoleId);
        }

        private void CloseCommandAction(object obj)
        {
            if (null != OnClosed)
            {
                OnClosed(this, new EventArgs { });
            }
        }

        private async void SaveCommandAction(object obj)
        {
            if (string.IsNullOrEmpty(Username))
            {
                MessageBox.Show("Please enter username.");
                return;
            }
            if (string.IsNullOrEmpty(Password))
            {
                MessageBox.Show("Please enter password.");
                return;
            }
            if (string.IsNullOrEmpty(PasswordReEnter))
            {
                MessageBox.Show("Please re-enter password.");
                return;
            }
            if (!Password.Equals(PasswordReEnter))
            {
                MessageBox.Show("Password not matched!");
                return;
            }
            await DbHandler.Instance.SaveAppUserData(new AppUser
            {
                Username = Username,
                Password = Password,
                FullName = FullName,
                Email = Email,
                MobileNo = MobileNo,
                RoleId = SelectedRole.RoleId
            });
        }
        #endregion

        #region ViewModelBase
        public override void OnLoad()
        {

        }

        public override void OnClosing()
        {
            Dispose();
        }
        #endregion
    }

    public class UserRole
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }
}
