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
    public class HardwareViewModel : ViewModelBase
    {
        #region Command(s)
        public CommandHandler<object, object> NewHardwareCommand { get; private set; }
        public CommandHandler<object, object> EditHardwareCommand { get; private set; }
        public CommandHandler<object, object> DeleteHardwareCommand { get; private set; }
        #endregion

        #region Property(s)
        public string Tag
        {
            get
            {
                return "Hardware";
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
        private HardwareOperationViewModel _hardwareOperationVM;
        public HardwareOperationViewModel HardwareOperationVM
        {
            get
            {
                if (_hardwareOperationVM == null)
                {
                    _hardwareOperationVM = ShellContainer.GetExportedInstance<HardwareOperationViewModel>();
                    _hardwareOperationVM.OnClosed += _hardwareOperationVM_OnClosed;
                }
                return _hardwareOperationVM;
            }
        }

        void _hardwareOperationVM_OnClosed(object sender, EventArgs e)
        {
            PopupContent = null;
            IsShowPopup = false;
            _hardwareOperationVM = null;
            LoadHardwares();
        }
        #endregion

        #region Constructor(s)
        [ImportingConstructor]
        public HardwareViewModel(IHardwareView view, CompositionContainer compositionContainer)
        {
            View = view;
            View.ViewModel = this;
            ShellContainer = compositionContainer;
            NewHardwareCommand = new CommandHandler<object, object>(NewHardwareCommandAction);
            EditHardwareCommand = new CommandHandler<object, object>(EditHardwareCommandAction);
            DeleteHardwareCommand = new CommandHandler<object, object>(DeleteHardwareCommandAction);
            _hardwareCollection = new Lazy<OptimizedObservableCollection<Hardware>>();
        }
        #endregion

        #region CommandHandler
        private void NewHardwareCommandAction(object obj)
        {
            PopupContent = HardwareOperationVM.View;
            IsShowPopup = true;
        }
        private async void DeleteHardwareCommandAction(object obj)
        {
            await DbHandler.Instance.DeleteHardware(obj as Hardware);
            HardwareCollection.Remove(obj as Hardware);
        }
        private void EditHardwareCommandAction(object obj)
        {
            HardwareOperationVM.PrepareView(obj as Hardware);
            PopupContent = HardwareOperationVM.View;
            IsShowPopup = true;
        }
        #endregion

        #region Method(s)

        #endregion

        #region ViewModelBase
        public override void OnLoad()
        {
            LoadHardwares();
        }

        private async void LoadHardwares()
        {
            HardwareCollection.Clear();
            var collection = await DbHandler.Instance.GetHardwareCollection();
            HardwareCollection.AddRange(collection);
        }

        public override void OnClosing()
        {
            this.Dispose();
        }
        #endregion
    }
}
