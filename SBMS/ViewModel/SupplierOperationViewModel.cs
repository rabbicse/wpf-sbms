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
    public class SupplierOperationViewModel : ViewModelBase
    {
        #region Event(s)
        public event EventHandler OnClosed;
        public event EventHandler<Supplier> OnSaved;
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
        private string _address;
        public string Address
        {
            get { return _address; }
            set
            {
                _address = value;
                OnPropertyChanged(() => Address);
            }
        }
        private string _contactMobileNo;
        public string ContactMobileNo
        {
            get { return _contactMobileNo; }
            set
            {
                _contactMobileNo = value;
                OnPropertyChanged(() => ContactMobileNo);
            }
        }
        private string _contactPersonName;
        public string ContactPersonName
        {
            get { return _contactPersonName; }
            set
            {
                _contactPersonName = value;
                OnPropertyChanged(() => ContactPersonName);
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
        public SupplierOperationViewModel(ISupplierOperationView view)
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
        public async void PrepareView(Supplier supplier)
        {
            await LoadHardwares();
            Id = supplier.Id;
            Name = supplier.Name;
            Address = supplier.Address;
            ContactMobileNo = supplier.ContactMobileNo;
            ContactPersonName = supplier.ContactPersonName;
            CurrentHardware = Hardwares.Where(h => h.SerialNo == supplier.SupplyHardware.SerialNo).FirstOrDefault();
            Comments = supplier.Comments;
        }
        #endregion

        private void CloseCommandAction(object obj)
        {
            if (null != OnClosed)
            {
                OnClosed(this, new EventArgs { });
            }
        }

        private async void SaveCommandAction(object obj)
        {
            var supplier = new Supplier
            {
                Id = Id,
                Name = Name,
                Address = Address,
                ContactMobileNo = ContactMobileNo,
                ContactPersonName = ContactPersonName,
                SupplyHardware = CurrentHardware,
                Comments = Comments
            };
            await DbHandler.Instance.SaveSupplier(supplier);
            if (null != OnSaved)
            {
                OnSaved(this, supplier);
            }
        }

        private async Task LoadHardwares()
        {
            Hardwares.Clear();
            var collection = await DbHandler.Instance.GetHardwareCollection();
            Hardwares.AddRange(collection);
        }

        #region ViewModelBase
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
