using EkushApp.Model;
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
    public class HardwareViewModel : ViewModelBase
    {
        #region Property(s)
        public string Tag
        {
            get
            {
                return "Hardware";
            }
        }
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
        private Lazy<OptimizedObservableCollection<Hardware>> _hardwareCollection;
        public OptimizedObservableCollection<Hardware> HardwareCollection
        {
            get { return _hardwareCollection.Value; }
        }
        private Hardware _selectedHardware;
        public Hardware SelectedHardware
        {
            get { return _selectedHardware; }
            set
            {
                _selectedHardware = value;
                OnPropertyChanged(() => SelectedHardware);
            }
        }
        #endregion

        #region Constructor(s)
        [ImportingConstructor]
        public HardwareViewModel(IHardwareView view)
        {
            View = view;
            View.ViewModel = this;
            _hardwareCollection = new Lazy<OptimizedObservableCollection<Hardware>>();
        }
        #endregion

        #region Method(s)

        #endregion

        #region ViewModelBase
        public override void OnLoad()
        {
            HardwareCollection.Add(new Hardware { BrandName = "asdasd", Category = HardwareCategory.MODEM, HardwareTagNo = "adasdas", Model = "sdfsdf", Status = HardwareStatus.RUNNING });
        }

        public override void OnClosing()
        {
            this.Dispose();
        }
        #endregion
    }
}
