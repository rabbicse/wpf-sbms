using EkushApp.EmbededDB;
using EkushApp.Model;
using EkushApp.ShellService.Commands;
using EkushApp.ShellService.MVVM;
using EkushApp.Utility.Extensions;
using SBMS.Generic;
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
        private DateTime _dateFrom = DateTime.Now;
        public DateTime DateFrom
        {
            get { return _dateFrom; }
            set
            {
                _dateFrom = value;
                OnPropertyChanged(() => DateFrom);
            }
        }
        private DateTime _dateTo = DateTime.Now;
        public DateTime DateTo
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
        public void OperationVM_OnClosed(object sender, EventArgs e)
        {
            OnCloseOperation();
            OperationVM.OnClosed -= OperationVM_OnClosed;
            _operationVM = null;
            LoadSearches();
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
            Collection.Clear();
            if (SelectedSearchBy.SearchKey.Equals("DATE_WISE"))
            {
                List<BbCircular> list = await DbHandler.Instance.SearchCircularByPubDate(DateFrom, DateTo);
                Collection.AddRange(list);
            }
            else if (SelectedSearchBy.SearchKey.Equals("TITLE_WISE"))
            {
                List<BbCircular> list = await DbHandler.Instance.SearchCircularByTitle(Title);
                Collection.AddRange(list);

            }
            else if (SelectedSearchBy.SearchKey.Equals("RECENT_ALL"))
            {
                List<BbCircular> list = await DbHandler.Instance.GetRecentCircular();
                Collection.AddRange(list);
            }
            else
            {
                List<BbCircular> list = await DbHandler.Instance.SearchCircularBySearchKey(BbSelectedSearchTerm.SearchTermKey);
                Collection.AddRange(list);
            }
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

        #region Method(s)
        private async void LoadSearches()
        {
            IsShowPubDate = false;
            IsShowSearchTerm = false;
            IsShowTitle = false;
            BbSearchTermCollection.Clear();
            List<BbCircularSearch> collection = await DbHandler.Instance.GetSearchTermCollection(SelectedSearchBy.SearchKey);
            BbSearchTermCollection.AddRange(collection);

            if (SelectedSearchBy.SearchKey.Equals("DATE_WISE"))
            {
                IsShowPubDate = true;
            }
            else if (SelectedSearchBy.SearchKey.Equals("RECENT_ALL"))
            {
                IsShowPubDate = true;
            }
            else if (SelectedSearchBy.SearchKey.Equals("TITLE_WISE"))
            {
                IsShowTitle = true;
            }
            else
            {
                IsShowSearchTerm = true;
            }
        }
        #endregion

        #region ViewModelBase
        public override async void OnLoad()
        {
            List<BbSearchBy> searchByCollection = await DbHandler.Instance.GetAllData<BbSearchBy>();
            BbSearchByCollection.Clear();
            BbSearchByCollection.AddRange(searchByCollection);
            base.OnLoad();
        }

        public override void OnClosing()
        {
        }
        #endregion
    }
}
