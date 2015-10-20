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
    public class GenericViewModel<T> : ViewModelBase
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
        private Lazy<OptimizedObservableCollection<T>> _collection;
        public OptimizedObservableCollection<T> Collection
        {
            get { return _collection.Value; }
        }
        private T _selectedItem;
        public T SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged(() => SelectedItem);
            }
        }
        #endregion

        #region Constructor(s)
        public GenericViewModel()
        {
            _collection = new Lazy<OptimizedObservableCollection<T>>();
            NewCommand = new CommandHandler<object, object>(NewCommandAction);
            EditCommand = new CommandHandler<object, object>(EditCommandAction);
            DeleteCommand = new CommandHandler<object, object>(DeleteCommandAction);
        }
        #endregion

        #region Command Manager(s)
        public virtual void NewCommandAction(object obj)
        {
            Console.WriteLine("test...");
        }
        public void EditCommandAction(object obj)
        {

        }
        private void DeleteCommandAction(object obj)
        {

        }
        #endregion

        #region ViewModelBase
        public override void OnLoad()
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
            ColumnConfiguration = new ColumnConfig { Columns = columns };
        }

        public override void OnClosing()
        {
        }
        #endregion
    }
}
