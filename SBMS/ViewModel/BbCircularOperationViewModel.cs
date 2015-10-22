using EkushApp.EmbededDB;
using EkushApp.Model;
using EkushApp.ShellService.Commands;
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
    public class BbCircularOperationViewModel : GenericOperationViewModel<BbCircular>
    {
        #region Command(s)
        public CommandHandler<object, object> BrowseFileCommand { get; private set; }
        #endregion

        #region Property(s)
        private BbCircularSearch _selectedSearchTerm;
        public BbCircularSearch SelectedSearchTerm
        {
            get { return _selectedSearchTerm; }
            set
            {
                _selectedSearchTerm = value;
                OnPropertyChanged(() => SelectedSearchTerm);
            }
        }
        private OptimizedObservableCollection<BbCircularSearch> _searchTermCollection;
        public OptimizedObservableCollection<BbCircularSearch> SearchTermCollection
        {
            get { return _searchTermCollection; }
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
        private string _selectedFile;
        public string SelectedFile
        {
            get { return _selectedFile; }
            set
            {
                _selectedFile = value;
                OnPropertyChanged(() => SelectedFile);
            }
        }
        private string _originalFileName = string.Empty;
        #endregion

        #region Constructor(s)
        [ImportingConstructor]
        public BbCircularOperationViewModel(IBbCircularOperationView view, CompositionContainer compositionContainer)
        {
            View = view;
            View.ViewModel = this;
            ShellContainer = compositionContainer;
            _searchTermCollection = new OptimizedObservableCollection<BbCircularSearch>();
            BrowseFileCommand = new CommandHandler<object, object>(BrowseFileCommandAction);
        }
        #endregion

        #region CommandHandler(s)
        private void BrowseFileCommandAction(object obj)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();



            // Set filter for file extension and default file extension 
            //dlg.DefaultExt = ".pdf";
            //dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";
            //dlg.Filter = "All Files (*.pdf)";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                _originalFileName = dlg.SafeFileName;
                SelectedFile = filename;
            }
        }
        public override async void SaveCommandAction(object obj)
        {
            await DbHandler.Instance.SaveBbCircularData(new BbCircular
            {
                SearchTerm = SelectedSearchTerm.SearchTerm,
                SearchTermKey = SelectedSearchTerm.SearchTermKey,
                Title = Title,
                FileName = _originalFileName,
                PublishDate = DateTime.Now
            }, SelectedFile);
            base.SaveCommandAction(obj);
        }
        #endregion

        #region ViewModelBase
        public override async void OnLoad()
        {
            base.OnLoad();
            List<BbCircularSearch> searchByCollection = await DbHandler.Instance.GetAllData<BbCircularSearch>();
            SearchTermCollection.Clear();
            SearchTermCollection.AddRange(searchByCollection);
        }
        #endregion
    }
}
