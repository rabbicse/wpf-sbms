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
    public class HardwareOperationViewModel : ViewModelBase
    {
        #region Event(s)
        public event EventHandler OnClosed;
        #endregion

        #region Command(s)
        public CommandHandler<object, object> SaveCommand { get; private set; }
        public CommandHandler<object, object> CloseCommand { get; private set; }
        #endregion

        #region Property(s)
        private OptimizedObservableCollection<HardwareCategory> _categories;
        public OptimizedObservableCollection<HardwareCategory> Categories
        {
            get { return _categories; }
        }
        private HardwareCategory _category;
        public HardwareCategory Category
        {
            get { return _category; }
            set
            {
                _category = value;
                OnPropertyChanged(() => Category);
            }
        }
        private string _hardwareTagNo;
        public string HardwareTagNo
        {
            get { return _hardwareTagNo; }
            set
            {
                _hardwareTagNo = value;
                OnPropertyChanged(() => HardwareTagNo);
            }
        }
        private string _brandName;
        public string BrandName
        {
            get { return _brandName; }
            set
            {
                _brandName = value;
                OnPropertyChanged(() => BrandName);
            }
        }
        private string _model;
        public string Model
        {
            get { return _model; }
            set
            {
                _model = value;
                OnPropertyChanged(() => Model);
            }
        }
        public long SerialNo { get; set; }
        private DateTime? _receiveDate;
        public DateTime? ReceiveDate
        {
            get { return _receiveDate; }
            set
            {
                _receiveDate = value;
                OnPropertyChanged(() => ReceiveDate);
            }
        }
        private OptimizedObservableCollection<HardwareStatus> _statuses;
        public OptimizedObservableCollection<HardwareStatus> Statuses
        {
            get { return _statuses; }
        }
        private HardwareStatus _status;
        public HardwareStatus Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged(() => Status);
            }
        }
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
        public HardwareOperationViewModel(IHardwareOperationView view)
        {
            View = view;
            View.ViewModel = this;
            SaveCommand = new CommandHandler<object, object>(SaveCommandAction);
            CloseCommand = new CommandHandler<object, object>(CloseCommandAction);
            _categories = new OptimizedObservableCollection<HardwareCategory>();
            _statuses = new OptimizedObservableCollection<HardwareStatus>();
            _categories.AddRange(Enum.GetValues(typeof(HardwareCategory)).Cast<HardwareCategory>().ToList());
            _statuses.AddRange(Enum.GetValues(typeof(HardwareStatus)).Cast<HardwareStatus>().ToList());
        }        

        public void PrepareView(Hardware hardware)
        {
            Category = hardware.Category;
            HardwareTagNo = hardware.HardwareTagNo;
            BrandName = hardware.BrandName;
            Model = hardware.Model;
            ReceiveDate = hardware.ReceiveDate;
            Status = hardware.Status;
            Comments = hardware.Comments;
            SerialNo = hardware.SerialNo;
        }
        #endregion

        #region CommandHandler
        private async void SaveCommandAction(object obj)
        {
            await DbHandler.Instance.SaveHardware(new Hardware
            {
                Category = Category,
                SerialNo = SerialNo,
                HardwareTagNo = HardwareTagNo,
                BrandName = BrandName,
                Model = Model,
                ReceiveDate = ReceiveDate,
                Status = Status,
                Comments = Comments
            });
            if (null != OnClosed)
            {
                OnClosed(this, new EventArgs { });
            }
        }
        private void CloseCommandAction(object obj)
        {
            if (null != OnClosed)
            {
                OnClosed(this, new EventArgs { });
            }
        }
        #endregion

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
