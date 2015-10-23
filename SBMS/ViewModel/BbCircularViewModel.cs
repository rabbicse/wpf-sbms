using EkushApp.EmbededDB;
using EkushApp.Model;
using EkushApp.ShellService.Commands;
using EkushApp.ShellService.MVVM;
using EkushApp.Utility.Extensions;
using SBMS.Generic;
using SBMS.Infrastructure;
using SBMS.View;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SBMS.ViewModel
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class BbCircularViewModel : GenericViewModel<BbCircular, BbCircularOperationViewModel>
    {
        #region Command(s)
        public CommandHandler<object, object> SearchCommand { get; set; }
        public CommandHandler<object, object> DownloadCommand { get; set; }
        #endregion

        #region Property(s)
        private OptimizedObservableCollection<BbSearchBy> _bbSearchByCollection;
        public OptimizedObservableCollection<BbSearchBy> BbSearchByCollection
        {
            get { return _bbSearchByCollection; }
        }
        private BbSearchBy _selectedSearchBy;
        public BbSearchBy SelectedSearchBy
        {
            get { return _selectedSearchBy; }
            set
            {
                _selectedSearchBy = value;
                OnPropertyChanged(() => SelectedSearchBy);
                LoadSearches();
            }
        }
        private OptimizedObservableCollection<BbCircularSearch> _bbSearchtermCollection;
        public OptimizedObservableCollection<BbCircularSearch> BbSearchTermCollection
        {
            get { return _bbSearchtermCollection; }
        }
        private BbCircularSearch _bbSelectedSearchTerm;
        public BbCircularSearch BbSelectedSearchTerm
        {
            get { return _bbSelectedSearchTerm; }
            set
            {
                _bbSelectedSearchTerm = value;
                OnPropertyChanged(() => BbSelectedSearchTerm);
            }
        }
        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged(() => Title);
            }
        }
        private DateTime? _dateFrom;
        public DateTime? DateFrom
        {
            get { return _dateFrom; }
            set
            {
                _dateFrom = value;
                OnPropertyChanged(() => DateFrom);
            }
        }
        private DateTime? _dateTo;
        public DateTime? DateTo
        {
            get { return _dateTo; }
            set
            {
                _dateTo = value;
                OnPropertyChanged(() => DateTo);
            }
        }
        private bool _isShowSearchTerm;
        public bool IsShowSearchTerm
        {
            get { return _isShowSearchTerm; }
            set
            {
                _isShowSearchTerm = value;
                OnPropertyChanged(() => IsShowSearchTerm);
            }
        }
        private bool _isShowTitle;
        public bool IsShowTitle
        {
            get { return _isShowTitle; }
            set
            {
                _isShowTitle = value;
                OnPropertyChanged(() => IsShowTitle);
            }
        }
        private bool _isShowPubDate;
        public bool IsShowPubDate
        {
            get { return _isShowPubDate; }
            set
            {
                _isShowPubDate = value;
                OnPropertyChanged(() => IsShowPubDate);
            }
        }
        #endregion

        #region Constructor(s)
        [ImportingConstructor]
        public BbCircularViewModel(IBbCircularView view, CompositionContainer compositionContainer)
        {
            View = view;
            View.ViewModel = this;
            ShellContainer = compositionContainer;
            Tag = "Bangladesh Bank Circular";
            SearchCommand = new CommandHandler<object, object>(SearchCommandAction);
            DownloadCommand = new CommandHandler<object, object>(DownloadCommandAction);
            _bbSearchByCollection = new OptimizedObservableCollection<BbSearchBy>();
            _bbSearchtermCollection = new OptimizedObservableCollection<BbCircularSearch>();
        }
        public async void OperationVM_OnClosed(object sender, EventArgs e)
        {
            OnCloseOperation();
            OperationVM.OnClosed -= OperationVM_OnClosed;
            _operationVM = null;
            LoadSearches();
            await LoadSearchResults();
        }
        #endregion

        #region Command Manager(s)
        public override void NewCommandAction(object obj)
        {
            base.NewCommandAction(obj);
            OperationVM.OnClosed += OperationVM_OnClosed;
            PopupContent = ((ViewModelBase)OperationVM).View;
            IsShowPopup = true;
        }
        public override void EditCommandAction(object obj)
        {
            base.EditCommandAction(obj);
            OperationVM.PrepareView(obj as BbCircular);
            OperationVM.OnClosed += OperationVM_OnClosed;
            PopupContent = ((ViewModelBase)OperationVM).View;
            IsShowPopup = true;
        }
        public override async void DeleteCommandAction(object obj)
        {
            base.DeleteCommandAction(obj);
            await DbHandler.Instance.DeleteBbCircular(obj as BbCircular);
            Collection.Remove(obj as BbCircular);
        }
        private async void SearchCommandAction(object obj)
        {
            Start = 0;
            await LoadSearchResults();
        }
        private async void DownloadCommandAction(object obj)
        {
            try
            {
                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                var selectedItem = obj as BbCircular;
                saveFileDialog.FileName = selectedItem.FileName;
                // Display SaveFileDialog by calling ShowDialog method 
                Nullable<bool> result = saveFileDialog.ShowDialog();
                if (result == true)
                {
                    string savePath = Path.GetDirectoryName(saveFileDialog.FileName);
                    bool downloaded = await DbHandler.Instance.DownloadFile(selectedItem.FileName, savePath);
                    if (downloaded)
                    {
                        MessageBox.Show("Successfully downloaded file.");
                    }
                }
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message);
            }
        }
        #endregion

        #region Pagination Command(s)
        protected override async void FirstCommandAction(object obj)
        {
            base.FirstCommandAction(obj);
            await LoadSearchResults();
        }
        protected override async void LastCommandAction(object obj)
        {
            base.LastCommandAction(obj);
            await LoadSearchResults();
        }
        protected override async void NextCommandAction(object obj)
        {
            base.NextCommandAction(obj);
            await LoadSearchResults();
        }
        protected override async void PrevCommandAction(object obj)
        {
            base.PrevCommandAction(obj);
            await LoadSearchResults();
        }
        #endregion

        #region Method(s)
        private async void LoadSearches()
        {
            IsShowPubDate = false;
            IsShowSearchTerm = false;
            IsShowTitle = false;
            DateFrom = (DateTime?)null;
            DateTo = (DateTime?)null;
            BbSearchTermCollection.Clear();
            if (SelectedSearchBy == null)
            {
                return;
            }
            switch (SelectedSearchBy.SearchKey)
            {
                case Globals.SearchKey.DEPARTMENT_WISE:
                    var depCollection = await DbHandler.Instance.GetAllData<BbDepartment>();
                    BbSearchTermCollection.AddRange(depCollection.Select(c => new BbCircularSearch { SearchKey = c.Key, SearchName = c.Name }));
                    IsShowSearchTerm = true;
                    IsShowPubDate = true;
                    break;
                case Globals.SearchKey.CATEGORY_WISE:
                    var catCollection = await DbHandler.Instance.GetAllData<BbCategory>();
                    BbSearchTermCollection.AddRange(catCollection.Select(c => new BbCircularSearch { SearchKey = c.Key, SearchName = c.Name }));
                    IsShowSearchTerm = true;
                    IsShowPubDate = true;
                    break;
                case Globals.SearchKey.TITLE_WISE:
                    IsShowTitle = true;
                    IsShowPubDate = true;
                    break;
                case Globals.SearchKey.DATE_WISE:
                    IsShowPubDate = true;
                    break;
                case Globals.SearchKey.RECENT_ALL:
                    break;
            }
        }
        private async Task LoadSearchResults()
        {
            Collection.Clear();
            List<BbCircular> list = new List<BbCircular>();
            if (SelectedSearchBy == null)
            {
                SelectedSearchBy = new BbSearchBy { SearchKey = Globals.SearchKey.RECENT_ALL };
            }
            switch (SelectedSearchBy.SearchKey)
            {
                case Globals.SearchKey.DEPARTMENT_WISE:
                    list = await DbHandler.Instance.SearchCircularByDept(BbSelectedSearchTerm.SearchKey, DateFrom, DateTo, t => Total = t, Start, Max);
                    Collection.AddRange(list);
                    break;
                case Globals.SearchKey.CATEGORY_WISE:
                    list = await DbHandler.Instance.SearchCircularByCategory(BbSelectedSearchTerm.SearchKey, DateFrom, DateTo, t => Total = t, Start, Max);
                    Collection.AddRange(list);
                    break;
                case Globals.SearchKey.TITLE_WISE:
                    list = await DbHandler.Instance.SearchCircularByTitle(Title, DateFrom, DateTo, t => Total = t, Start, Max);
                    Collection.AddRange(list);
                    break;
                case Globals.SearchKey.DATE_WISE:
                    list = await DbHandler.Instance.SearchCircularByPubDate(DateFrom.Value, DateTo.Value, t => Total = t, Start, Max);
                    Collection.AddRange(list);
                    break;
                case Globals.SearchKey.RECENT_ALL:
                default:
                    list = await DbHandler.Instance.GetRecentCircular(t => Total = t, Start, Max);
                    Collection.AddRange(list);
                    break;
            }

            OnCalculatePagination();
        }
        #endregion

        #region ViewModelBase
        public override void OnLoad()
        {
            BbSearchByCollection.Clear();
            FieldInfo[] fields = typeof(Globals.SearchKey).GetFields();
            foreach (FieldInfo field in fields)
            {
                var header = field.GetCustomAttributes(true).FirstOrDefault(f => f is Header);
                if (header != null)
                {
                    BbSearchByCollection.Add(new BbSearchBy { SearchName = ((Header)header).Name, SearchKey = (string)field.GetValue(field) });
                }
            }
            base.OnLoad();
        }

        public override void OnClosing()
        {
        }
        #endregion
    }
}
