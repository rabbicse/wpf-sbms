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

namespace SBMS.ViewModel
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class UserOperationViewModel : ViewModelBase
    {
        #region Event(s)
        public event EventHandler OnClosed;
        public event EventHandler<User> OnSaved;
        #endregion

        #region Command(s)
        public CommandHandler<object, object> SaveCommand { get; private set; }
        public CommandHandler<object, object> CloseCommand { get; private set; }
        #endregion

        #region Property(s)
        private OptimizedObservableCollection<Hardware> _hardwares;
        public OptimizedObservableCollection<Hardware> Hardwares
        {
            get { return _hardwares; }
        }
        private Hardware _currentHardware;
        public Hardware CurrentHardware
        {
            get { return _currentHardware; }
            set
            {
                _currentHardware = value;                
                OnPropertyChanged(() => CurrentHardware);
            }
        }
        
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(() => Name);
            }
        }
        private string _designation;
        public string Designation
        {
            get { return _designation; }
            set
            {
                _designation = value;
                OnPropertyChanged(() => Designation);
            }
        }
        private string _department;
        public string Department
        {
            get { return _department; }
            set
            {
                _department = value;
                OnPropertyChanged(() => Department);
            }
        }
        public long Id { get; set; }
        private string _comments;
        public string Comments
        {
            get { return _comments; }
            set
            {
                _comments = value;
                OnPropertyChanged(() => Comments);
            }
        }
        #endregion

        #region Constructor(s)
        [ImportingConstructor]
        public UserOperationViewModel(IUserOperationView view)
        {
            View = view;
            View.ViewModel = this;
            SaveCommand = new CommandHandler<object, object>(SaveCommandAction);
            CloseCommand = new CommandHandler<object, object>(CloseCommandAction);
            _hardwares = new OptimizedObservableCollection<Hardware>();
        }
        public async void PrepareView()
        {
            await LoadHardwares();
        }
        public async void PrepareView(User user)
        {
            await LoadHardwares();
            Id = user.Id;
            Name = user.Name;
            Designation = user.Designation;
            Department = user.Department;
            CurrentHardware = Hardwares.Where(h => h.SerialNo == user.HardwareSerial).FirstOrDefault();
            Comments = user.Comments;
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
            var user = new User
            {
                Id = Id,
                Name = Name,
                Designation = Designation,
                Department = Department,
                HardwareSerial = CurrentHardware.SerialNo,
                Comments = Comments
            };
            await DbHandler.Instance.SaveUser(user);
            if (null != OnSaved)
            {
                OnSaved(this, user);
            }
        }
        #endregion

        private async Task LoadHardwares()
        {
            Hardwares.Clear();
            var collection = await DbHandler.Instance.GetHardwareCollection();
            Hardwares.AddRange(collection);
        }

        #region viewModelBase
        public override void OnLoad()
        {
        }

        public override void OnClosing()
        {
            this.Dispose();
        }
        #endregion
    }
}
