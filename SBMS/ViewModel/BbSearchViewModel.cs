using EkushApp.EmbededDB;
using EkushApp.Model;
using EkushApp.ShellService.MVVM;
using EkushApp.Utility.Extensions;
using SBMS.Generic;
using SBMS.View;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SBMS.ViewModel
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class BbSearchViewModel : GenericViewModel<BbCircularSearch, BbSearchOperationViewModel>
    {
        #region Property(s)
        private ColumnConfig _deptColumnConfig;
        public ColumnConfig DeptColumnConfiguration
        {
            get { return _deptColumnConfig; }
            set
            {
                _deptColumnConfig = value;
                OnPropertyChanged(() => DeptColumnConfiguration);
            }
        }
        private ColumnConfig _categoryColumnConfig;
        public ColumnConfig CategoryColumnConfiguration
        {
            get { return _categoryColumnConfig; }
            set
            {
                _categoryColumnConfig = value;
                OnPropertyChanged(() => CategoryColumnConfiguration);
            }
        }
        private OptimizedObservableCollection<BbDepartment> _departmentCollection;
        public OptimizedObservableCollection<BbDepartment> DepartmentCollection
        {
            get { return _departmentCollection; }
        }
        private OptimizedObservableCollection<BbCategory> _categoryCollection;
        public OptimizedObservableCollection<BbCategory> CategoryCollection
        {
            get { return _categoryCollection; }
        }
        #endregion

        #region Constructor(s)
        [ImportingConstructor]
        public BbSearchViewModel(IBbSearchView view, CompositionContainer compositionContainer)
        {
            View = view;
            View.ViewModel = this;
            ShellContainer = compositionContainer;
            Tag = "BB Search Category";
            _departmentCollection = new OptimizedObservableCollection<BbDepartment>();
            _categoryCollection = new OptimizedObservableCollection<BbCategory>();
        }

        public void OperationVM_OnClosed(object sender, EventArgs e)
        {
            OnCloseOperation();
            OperationVM.OnClosed -= OperationVM_OnClosed;
            _operationVM = null;
            LoadData();
        }
        #endregion

        private async void LoadData()
        {
            DepartmentCollection.Clear();
            CategoryCollection.Clear();
            var dCollection = await DbHandler.Instance.GetAllData<BbDepartment>();
            DepartmentCollection.AddRange(dCollection);
            var cCollection = await DbHandler.Instance.GetAllData<BbCategory>();
            CategoryCollection.AddRange(cCollection);
        }

        #region Command Handler(s)
        public override void NewCommandAction(object obj)
        {
            base.NewCommandAction(obj);
            OperationVM.OnClosed += OperationVM_OnClosed;
            PopupContent = ((ViewModelBase)OperationVM).View;
            IsShowPopup = true;
        }
        #endregion

        #region ViewModelBase
        public override void OnLoad()
        {
            base.OnLoad();

            DeptColumnConfiguration = GenerateColumnConfig<BbDepartment>();
            CategoryColumnConfiguration = GenerateColumnConfig<BbCategory>();


            LoadData();
        }
        
        public override void OnClosing()
        {
            base.OnClosing();
            this.Dispose();
        }
        #endregion
    }
}
