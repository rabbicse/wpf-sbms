using EkushApp.EmbededDB;
using EkushApp.Logging;
using EkushApp.Model;
using EkushApp.ShellService.Commands;
using EkushApp.ShellService.MVVM;
using EkushApp.Utility.Extensions;
using Microsoft.Win32;
using SBMS.Reports;
using SBMS.View;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBMS.ViewModel
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ReportViewModel : ViewModelBase
    {
        #region CommandHandler(s)
        public CommandHandler<object, object> PrintHWReportCommand { get; private set; }
        public CommandHandler<object, object> PrintRunningHWCommand { get; private set; }
        public CommandHandler<object, object> PrintInactiveHWCommand { get; private set; }
        public CommandHandler<object, object> PrintRepairHWCommand { get; private set; }
        public CommandHandler<object, object> PrintUserReportCommand { get; private set; }
        public CommandHandler<object, object> SearchByUserCommand { get; set; }
        #endregion

        #region Property(s)
        public string Tag
        {
            get
            {
                return "Report";
            }
        }
        private OptimizedObservableCollection<HardwareCountReport> _hardwareCollection;
        public OptimizedObservableCollection<HardwareCountReport> HardwareCollection
        {
            get { return _hardwareCollection; }
        }
        private OptimizedObservableCollection<HardwareReport> _activeHardwareCollection;
        public OptimizedObservableCollection<HardwareReport> ActiveHardwareCollection
        {
            get { return _activeHardwareCollection; }
        }
        private OptimizedObservableCollection<HardwareReport> _inActiveHardwareCollection;
        public OptimizedObservableCollection<HardwareReport> InActiveHardwareCollection
        {
            get { return _inActiveHardwareCollection; }
        }
        private OptimizedObservableCollection<HardwareReport> _inRepairHardwareCollection;
        public OptimizedObservableCollection<HardwareReport> InRepairHardwareCollection
        {
            get { return _inRepairHardwareCollection; }
        }
        private OptimizedObservableCollection<HardwareReport> _userReportCollection;
        public OptimizedObservableCollection<HardwareReport> UserReportCollection
        {
            get { return _userReportCollection; }
        }
        private OptimizedObservableCollection<User> _userCollection;
        public OptimizedObservableCollection<User> UserCollection
        {
            get { return _userCollection; }
        }
        private OptimizedObservableCollection<Hardware> _allHardwares;
        public OptimizedObservableCollection<Hardware> AllHardwares
        {
            get { return _allHardwares; }
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

        [ImportingConstructor]
        public ReportViewModel(IReportView view)
        {
            View = view;
            View.ViewModel = this;
            PrintHWReportCommand = new CommandHandler<object, object>(PrintHWReportCommandAction);
            PrintRunningHWCommand = new CommandHandler<object, object>(PrintRunningHWCommandAction);
            PrintInactiveHWCommand = new CommandHandler<object, object>(PrintInactiveHWCommandAction);
            PrintRepairHWCommand = new CommandHandler<object, object>(PrintRepairHWCommandAction);
            PrintUserReportCommand = new CommandHandler<object, object>(PrintUserReportCommandAction);
            SearchByUserCommand = new CommandHandler<object, object>(SearchByUserCommandAction);
            _userCollection = new OptimizedObservableCollection<User>();
            _allHardwares = new OptimizedObservableCollection<Hardware>();
            _hardwareCollection = new OptimizedObservableCollection<HardwareCountReport>();
            _activeHardwareCollection = new OptimizedObservableCollection<HardwareReport>();
            _inActiveHardwareCollection = new OptimizedObservableCollection<HardwareReport>();
            _inRepairHardwareCollection = new OptimizedObservableCollection<HardwareReport>();
            _userReportCollection = new OptimizedObservableCollection<HardwareReport>();
        }

        private void SearchByUserCommandAction(object obj)
        {

            var user = UserCollection.FirstOrDefault(u => u.Name.Equals(SelectedUser.Name));
            if (user != null)
            {
                var hardwares = AllHardwares.Where(hw => hw.ComputerUserId == user.Id).ToList();
                UserReportCollection.Clear();
                UserReportCollection.AddRange(hardwares.Select(hw => new HardwareReport
                {
                    Category = hw.Category.ToString(),
                    SerialNo = hw.SerialNo,
                    HardwareTagNo = hw.HardwareTagNo,
                    BrandName = hw.BrandName,
                    Model = hw.Model,
                    HardwareSerialNo = hw.HardwareSerialNo,
                    ReceiveDate = hw.ReceiveDate.HasValue ? hw.ReceiveDate.Value.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) : string.Empty,
                    ComputerUserName = user.Name,
                    Comments = hw.Comments
                }));
            }
        }

        private void PrintRepairHWCommandAction(object obj)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Pdf File (.pdf)|*.pdf";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (saveFileDialog.ShowDialog() == true)
            {
                ReportGenerator.CreateHardwareStatusReport(saveFileDialog.FileName, InRepairHardwareCollection);
            }
        }

        private void PrintInactiveHWCommandAction(object obj)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Pdf File (.pdf)|*.pdf";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (saveFileDialog.ShowDialog() == true)
            {
                ReportGenerator.CreateHardwareStatusReport(saveFileDialog.FileName, InActiveHardwareCollection);
            }
        }

        private void PrintRunningHWCommandAction(object obj)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Pdf File (.pdf)|*.pdf";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (saveFileDialog.ShowDialog() == true)
            {
                ReportGenerator.CreateHardwareStatusReport(saveFileDialog.FileName, ActiveHardwareCollection);
            }
        }

        private void PrintHWReportCommandAction(object obj)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Pdf File (.pdf)|*.pdf";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (saveFileDialog.ShowDialog() == true)
            {
                ReportGenerator.CreateHardwareReport(saveFileDialog.FileName, HardwareCollection);
            }
        }
        private void PrintUserReportCommandAction(object obj)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Pdf File (.pdf)|*.pdf";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (saveFileDialog.ShowDialog() == true)
            {
                ReportGenerator.CreateHardwareStatusReport(saveFileDialog.FileName, UserReportCollection);
            }
        }

        public override void OnLoad()
        {
            UpdateReport();
        }
        private async void UpdateReport()
        {
            try
            {
                var hardwares = await DbHandler.Instance.GetHardwareCollection();
                if (hardwares != null && hardwares.Count > 0)
                {
                    AllHardwares.Clear();
                    AllHardwares.AddRange(hardwares);
                }
                var users = await DbHandler.Instance.GetUserCollection();
                if (users != null && users.Count > 0)
                {
                    //var reports = from user in users
                    //              group user by user.Name into g
                    //              select new UserReport
                    //              {
                    //                  UserName = g.Key,
                    //                  Designation = g.First().Designation,
                    //                  Department = g.First().Department,
                    //                  Hardwares = hardwares.Where(h => h.ComputerUserId.HasValue && h.ComputerUserId.Value == g.First())
                    //              };
                    UserCollection.Clear();
                    UserCollection.AddRange(users);
                }

                var report = from hardware in hardwares
                             group hardware by hardware.Model into g
                             select new HardwareCountReport
                             {
                                 Category = g.First().Category.ToString(),
                                 Model = g.Key,
                                 Count = g.Count().ToString()
                             };
                HardwareCollection.Clear();
                HardwareCollection.AddRange(report);
                ActiveHardwareCollection.Clear();
                ActiveHardwareCollection.AddRange(hardwares.Where(h => h.Status == HardwareStatus.RUNNING)
                                                 .Select(hw => new HardwareReport
                                                 {
                                                     Category = hw.Category.ToString(),
                                                     SerialNo = hw.SerialNo,
                                                     HardwareTagNo = hw.HardwareTagNo,
                                                     BrandName = hw.BrandName,
                                                     Model = hw.Model,
                                                     HardwareSerialNo = hw.HardwareSerialNo,
                                                     ReceiveDate = hw.ReceiveDate.HasValue ? hw.ReceiveDate.Value.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) : string.Empty,
                                                     ComputerUserName = users != null && users.Any(u => hw.ComputerUserId.HasValue && hw.ComputerUserId.Value == u.Id)
                                                        ? users.FirstOrDefault(u => hw.ComputerUserId.Value == u.Id).Name : string.Empty,
                                                     Comments = hw.Comments
                                                 }));
                InActiveHardwareCollection.Clear();
                InActiveHardwareCollection.AddRange(hardwares.Where(h => h.Status == HardwareStatus.UN_USED)
                                                    .Select(hw => new HardwareReport
                                                    {
                                                        Category = hw.Category.ToString(),
                                                        SerialNo = hw.SerialNo,
                                                        HardwareTagNo = hw.HardwareTagNo,
                                                        BrandName = hw.BrandName,
                                                        Model = hw.Model,
                                                        HardwareSerialNo = hw.HardwareSerialNo,
                                                        ReceiveDate = hw.ReceiveDate.HasValue ? hw.ReceiveDate.Value.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) : string.Empty,
                                                        ComputerUserName = users != null && users.Any(u => hw.ComputerUserId.HasValue && hw.ComputerUserId.Value == u.Id)
                                                        ? users.FirstOrDefault(u => hw.ComputerUserId.Value == u.Id).Name : string.Empty,
                                                        Comments = hw.Comments
                                                    }));
                InRepairHardwareCollection.Clear();
                InRepairHardwareCollection.AddRange(hardwares.Where(h => h.Status == HardwareStatus.REPAIR_TO_ITD)
                                                    .Select(hw => new HardwareReport
                                                    {
                                                        Category = hw.Category.ToString(),
                                                        SerialNo = hw.SerialNo,
                                                        HardwareTagNo = hw.HardwareTagNo,
                                                        BrandName = hw.BrandName,
                                                        Model = hw.Model,
                                                        HardwareSerialNo = hw.HardwareSerialNo,
                                                        ReceiveDate = hw.ReceiveDate.HasValue ? hw.ReceiveDate.Value.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) : string.Empty,
                                                        ComputerUserName = users != null && users.Any(u => hw.ComputerUserId.HasValue && hw.ComputerUserId.Value == u.Id)
                                                        ? users.FirstOrDefault(u => hw.ComputerUserId.Value == u.Id).Name : string.Empty,
                                                        Comments = hw.Comments
                                                    }));
                UserReportCollection.Clear();
                UserReportCollection.AddRange(hardwares.Select(hw => new HardwareReport
                {
                    Category = hw.Category.ToString(),
                    SerialNo = hw.SerialNo,
                    HardwareTagNo = hw.HardwareTagNo,
                    BrandName = hw.BrandName,
                    Model = hw.Model,
                    HardwareSerialNo = hw.HardwareSerialNo,
                    ReceiveDate = hw.ReceiveDate.HasValue ? hw.ReceiveDate.Value.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) : string.Empty,
                    ComputerUserName = users != null && users.Any(u => hw.ComputerUserId.HasValue && hw.ComputerUserId.Value == u.Id)
                ? users.FirstOrDefault(u => hw.ComputerUserId.Value == u.Id).Name : string.Empty,
                    Comments = hw.Comments
                }));
            }
            catch (Exception x)
            {
                Log.Error("Error: ", x);
            }
        }

        public override void OnClosing()
        {
            this.Dispose();
        }
    }
}
