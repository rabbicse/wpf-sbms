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

        #region Pagination Command(s)
        public CommandHandler<object, object> FirstCommand { get; set; }
        public CommandHandler<object, object> PrevCommand { get; set; }
        public CommandHandler<object, object> NextCommand { get; set; }
        public CommandHandler<object, object> LastCommand { get; set; }
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
        private int _start = 0;
        public int Start
        {
            get { return _start; }
            set
            {
                _start = value;
                OnPropertyChanged(() => Start);
            }
        }
        private int _max = 10;
        public int Max
        {
            get { return _max; }
            set
            {
                _max = value;
                OnPropertyChanged(() => Max);
            }
        }
        private int _total;
        public int Total
        {
            get { return _total; }
            set
            {
                _total = value;
                OnPropertyChanged(() => Total);
            }
        }
        private bool _isShowNext = false;
        public bool IsShowNext
        {
            get { return _isShowNext; }
            set
            {
                _isShowNext = value;
                OnPropertyChanged(() => IsShowNext);
            }
        }
        private bool _isShowPrev;
        public bool IsShowPrev
        {
            get { return _isShowPrev; }
            set
            {
                _isShowPrev = value;
                OnPropertyChanged(() => IsShowPrev);
            }
        }
        private bool _isShowFirst;
        public bool IsShowFirst
        {
            get { return _isShowFirst; }
            set
            {
                _isShowFirst = value;
                OnPropertyChanged(() => IsShowFirst);
            }
        }
        private bool _isShowLast;
        public bool IsShowLast
        {
            get { return _isShowLast; }
            set
            {
                _isShowLast = value;
                OnPropertyChanged(() => IsShowLast);
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

            FirstCommand = new CommandHandler<object, object>(FirstCommandAction);
            LastCommand = new CommandHandler<object, object>(LastCommandAction);
            NextCommand = new CommandHandler<object, object>(NextCommandAction);
            PrevCommand = new CommandHandler<object, object>(PrevCommandAction);
        }
        #endregion

        #region Command Manager(s)
        public virtual void NewCommandAction(object obj)
        {
        }
        public virtual void EditCommandAction(object obj)
        {

        }
        public virtual void DeleteCommandAction(object obj)
        {

        }
        #endregion

        #region Pagination CommandManager(s)
        protected virtual void FirstCommandAction(object obj)
        {
            Start = 0;
        }
        protected virtual void PrevCommandAction(object obj)
        {
            Start--;
        }
        protected virtual void NextCommandAction(object obj)
        {
            Start++;
        }
        protected virtual void LastCommandAction(object obj)
        {
            Start = ((int)Math.Ceiling((float)Total / (float)Max)) - 1;
        }
        #endregion

        #region Method(s)
        protected ColumnConfig GenerateColumnConfig<T>()
        {
            List<Column> columns = new List<Column>();
            PropertyInfo[] props = typeof(T).GetProperties();
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
            return new ColumnConfig { Columns = columns };
        }
        public void OnCloseOperation()
        {
            PopupContent = null;
            IsShowPopup = false;
        }
        #endregion

        #region ViewModelBase
        public override async void OnLoad()
        {
            ColumnConfiguration = GenerateColumnConfig<T1>();
            await LoadCollection();
            OnCalculatePagination();
        }
        protected void OnCalculatePagination()
        {
            IsShowFirst = Total > Max && Start > 0;
            IsShowPrev = Total > Max && Start > 0;
            IsShowNext = Total > Max && ((Start + 1) * Max) < Total;
            IsShowLast = Total > Max && ((Start + 1) * Max) < Total;
        }

        private async Task LoadCollection()
        {
            List<T1> collection = await DbHandler.Instance.GetAllData<T1>(t => Total = t, Start, Max);
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
