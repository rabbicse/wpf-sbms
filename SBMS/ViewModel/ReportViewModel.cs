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
        #endregion

        #region Property(s)
        public string Tag
        {
            get
            {
                return "Report";
            }
        }
        private OptimizedObservableCollection<HardwareReport> _hardwareCollection;
        public OptimizedObservableCollection<HardwareReport> HardwareCollection
        {
            get { return _hardwareCollection; }
        }
        private OptimizedObservableCollection<Hardware> _activeHardwareCollection;
        public OptimizedObservableCollection<Hardware> ActiveHardwareCollection
        {
            get { return _activeHardwareCollection; }
        }
        private OptimizedObservableCollection<Hardware> _inActiveHardwareCollection;
        public OptimizedObservableCollection<Hardware> InActiveHardwareCollection
        {
            get { return _inActiveHardwareCollection; }
        }
        private OptimizedObservableCollection<Hardware> _inRepairHardwareCollection;
        public OptimizedObservableCollection<Hardware> InRepairHardwareCollection
        {
            get { return _inRepairHardwareCollection; }
        }
        private OptimizedObservableCollection<UserReport> _userReportCollection;
        public OptimizedObservableCollection<UserReport> UserReportCollection
        {
            get { return _userReportCollection; }
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
            _hardwareCollection = new OptimizedObservableCollection<HardwareReport>();
            _activeHardwareCollection = new OptimizedObservableCollection<Hardware>();
            _inActiveHardwareCollection = new OptimizedObservableCollection<Hardware>();
            _inRepairHardwareCollection = new OptimizedObservableCollection<Hardware>();
            _userReportCollection = new OptimizedObservableCollection<UserReport>();
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
                ReportGenerator.CreateUserStatusReport(saveFileDialog.FileName, UserReportCollection);
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
                var report = from hardware in hardwares
                             group hardware by hardware.Model into g
                             select new HardwareReport
                             {
                                 Category = g.First().Category.ToString(),
                                 Model = g.Key,
                                 Count = g.Count().ToString()
                             };
                HardwareCollection.Clear();
                HardwareCollection.AddRange(report);
                ActiveHardwareCollection.Clear();
                ActiveHardwareCollection.AddRange(hardwares.Where(h => h.Status == HardwareStatus.RUNNING));
                InActiveHardwareCollection.Clear();
                InActiveHardwareCollection.AddRange(hardwares.Where(h => h.Status == HardwareStatus.UN_USED));
                InRepairHardwareCollection.Clear();
                InRepairHardwareCollection.AddRange(hardwares.Where(h => h.Status == HardwareStatus.REPAIR_TO_TLD));

                var users = await DbHandler.Instance.GetUserCollection();
                if (users != null && users.Count > 0)
                {
                    var reports = from user in users
                                  group user by user.Name into g
                                  select new UserReport
                                  {
                                      UserName = g.Key,
                                      Designation = g.First().Designation,
                                      Department = g.First().Department,
                                      Hardwares = g.Select(u => hardwares.FirstOrDefault(h => u.HardwareSerial == h.SerialNo)).ToList()
                                  };
                    UserReportCollection.AddRange(reports);
                }
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
