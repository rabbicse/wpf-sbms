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
    public class SupplierViewModel : ViewModelBase
    {
        #region Command(s)
        public CommandHandler<object, object> NewHardwareCommand { get; private set; }
        public CommandHandler<object, object> EditHardwareCommand { get; private set; }
        public CommandHandler<object, object> DeleteHardwareCommand { get; private set; }
        #endregion

        #region Property(s)
        public string Tag { get { return "Supplier"; } }
        private Lazy<OptimizedObservableCollection<Supplier>> _supplierCollection;
        public OptimizedObservableCollection<Supplier> SupplierCollection
        {
            get { return _supplierCollection.Value; }
        }
        private Supplier _selectedSupplier;
        public Supplier SelectedSupplier
        {
            get { return _selectedSupplier; }
            set
            {
                _selectedSupplier = value;
                OnPropertyChanged(() => SelectedSupplier);
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
        private SupplierOperationViewModel _supplierOperationVM;
        public SupplierOperationViewModel SupplierOperationVM
        {
            get
            {
                if (_supplierOperationVM == null)
                {
                    _supplierOperationVM = ShellContainer.GetExportedInstance<SupplierOperationViewModel>();
                    _supplierOperationVM.OnSaved += _supplierOperationVM_OnSaved;
                    _supplierOperationVM.OnClosed += _hardwareOperationVM_OnClosed;
                }
                return _supplierOperationVM;
            }
        }

        void _supplierOperationVM_OnSaved(object sender, Supplier e)
        {
            SupplierCollection.Remove(e);
            SupplierCollection.Add(e);
            _hardwareOperationVM_OnClosed(sender, new EventArgs { });
        }

        void _hardwareOperationVM_OnClosed(object sender, EventArgs e)
        {
            PopupContent = null;
            IsShowPopup = false;
            _supplierOperationVM = null;
            LoadSuppliers();
        }
        #endregion

        #region Constructor(s)
        [ImportingConstructor]
        public SupplierViewModel(ISupplierView view, CompositionContainer compositionContainer)
        {
            View = view;
            View.ViewModel = this;
            ShellContainer = compositionContainer;
            NewHardwareCommand = new CommandHandler<object, object>(NewHardwareCommandAction);
            EditHardwareCommand = new CommandHandler<object, object>(EditHardwareCommandAction);
            DeleteHardwareCommand = new CommandHandler<object, object>(DeleteHardwareCommandAction);
            _supplierCollection = new Lazy<OptimizedObservableCollection<Supplier>>();
        }
        #endregion

        #region CommandHandler
        private void NewHardwareCommandAction(object obj)
        {
            SupplierOperationVM.PrepareView();
            PopupContent = SupplierOperationVM.View;
            IsShowPopup = true;
        }
        private async void DeleteHardwareCommandAction(object obj)
        {
            await DbHandler.Instance.DeleteSupplier(obj as Supplier);
            SupplierCollection.Remove(obj as Supplier);
        }
        private void EditHardwareCommandAction(object obj)
        {
            SupplierOperationVM.PrepareView(obj as Supplier);
            PopupContent = SupplierOperationVM.View;
            IsShowPopup = true;
        }
        #endregion

        private async void LoadSuppliers()
        {
            SupplierCollection.Clear();
            var collection = await DbHandler.Instance.GetSupplierCollection();
            SupplierCollection.AddRange(collection);
        }

        #region ViewModelBase
        public override void OnLoad()
        {
            LoadSuppliers();
        }

        public override void OnClosing()
        {
            this.Dispose();
        }
        #endregion
    }
}
