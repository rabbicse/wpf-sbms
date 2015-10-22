using EkushApp.EmbededDB;
using EkushApp.Model;
using EkushApp.ShellService.Commands;
using EkushApp.ShellService.MVVM;
using EkushApp.Utility.Extensions;
using SBMS.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SBMS.ViewModel
{
    public class GenericViewModel<T1, T2> : ViewModelBase
    {
        #region Command(s)
        public CommandHandler<object, object> NewCommand { get; private set; }
        public CommandHandler<object, object> EditCommand { get; private set; }
        public CommandHandler<object, object> DeleteCommand { get; private set; }
        #endregion

        #region Property(s)
        private string _tag;
        public string Tag
        {
            get
            {
                return _tag;
            }
            set
            {
                _tag = value;
            }
        }
        private ColumnConfig _columnConfig;
        public ColumnConfig ColumnConfiguration
        {
            get { return _columnConfig; }
            set
            {
                _columnConfig = value;
                OnPropertyChanged(() => ColumnConfiguration);
            }
        }
        private Lazy<OptimizedObservableCollection<T1>> _collection;
        public OptimizedObservableCollection<T1> Collection
        {
            get { return _collection.Value; }
        }
        private T1 _selectedItem;
        public T1 SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged(() => SelectedItem);
            }
        }
        #endregion

        #region ViewModel(s)
        protected T2 _operationVM;
        public T2 OperationVM
        {
            get
            {
                if (_operationVM == null)
                {
                    _operationVM = ShellContainer.GetExportedInstance<T2>();
                }
                return _operationVM;
            }
        }
        #endregion

        #region Constructor(s)
        public GenericViewModel()
        {
            _collection = new Lazy<OptimizedObservableCollection<T1>>();
            NewCommand = new CommandHandler<object, object>(NewCommandAction);
            EditCommand = new CommandHandler<object, object>(EditCommandAction);
            DeleteCommand = new CommandHandler<object, object>(DeleteCommandAction);
        }
        #endregion

        #region Command Manager(s)
        public virtual void NewCommandAction(object obj)
        {            
        }
        public void EditCommandAction(object obj)
        {

        }
        private void DeleteCommandAction(object obj)
        {

        }
        #endregion

        #region Method(s)
        public void OnCloseOperation() 
        {
            PopupContent = null;
            IsShowPopup = false;
        }
        #endregion

        #region ViewModelBase
        public override void OnLoad()
        {
            List<Column> columns = new List<Column>();
            PropertyInfo[] props = typeof(T1).GetProperties();
            foreach (PropertyInfo prop in props)
            {
                object[] attrs = prop.GetCustomAttributes(true);
                foreach (object attr in attrs)
                {
                    if (attr != null && attr is Header)
                    {
                        Header header = (Header)attr;
                        columns.Add(new Column { Header = header.Name, DataField = prop.Name });
                    }
                }
            }
            ColumnConfiguration = new ColumnConfig { Columns = columns };
            LoadCollection();
        }

        private async void LoadCollection() 
        {
            List<T1> collection = await DbHandler.Instance.GetAllData<T1>();
            Collection.Clear();
            Collection.AddRange(collection);
        }

        public override void OnClosing()
        {
            Collection.Clear();
        }
        #endregion
    }
}
