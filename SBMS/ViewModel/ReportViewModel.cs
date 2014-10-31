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
            set
            {
                _hardwareCollection = value;
                OnPropertyChanged(() => HardwareCollection);
            }
        }
        private OptimizedObservableCollection<Hardware> _activeHardwareCollection;
        public OptimizedObservableCollection<Hardware> ActiveHardwareCollection
        {
            get { return _activeHardwareCollection; }
            set
            {
                _activeHardwareCollection = value;
                OnPropertyChanged(() => ActiveHardwareCollection);
            }
        }
        #endregion

        [ImportingConstructor]
        public ReportViewModel(IReportView view)
        {
            View = view;
            View.ViewModel = this;
            PrintHWReportCommand = new CommandHandler<object, object>(PrintHWReportCommandAction);
            _hardwareCollection = new OptimizedObservableCollection<HardwareReport>();
            _activeHardwareCollection = new OptimizedObservableCollection<Hardware>();
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

                HardwareCollection.AddRange(report);
            }
            catch (Exception x)
            {
                Log.Error("Error: ", x);
            }

            //HardwareCollection.AddRange();
        }

        public override void OnClosing()
        {
            this.Dispose();
        }
    }
}
