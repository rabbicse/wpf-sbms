using EkushApp.EmbededDB;
using EkushApp.Logging;
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
        private BbDepartment _selectedDept;
        public BbDepartment SelectedDept
        {
            get { return _selectedDept; }
            set
            {
                _selectedDept = value;
                OnPropertyChanged(() => SelectedDept);
            }
        }
        private BbCategory _selectedCategory;
        public BbCategory SelectedCategory
        {
            get { return _selectedCategory; }
            set
            {
                _selectedCategory = value;
                OnPropertyChanged(() => SelectedCategory);
            }
        }
        private OptimizedObservableCollection<BbDepartment> _deptCollection;
        public OptimizedObservableCollection<BbDepartment> DeptCollection
        {
            get { return _deptCollection; }
        }
        private OptimizedObservableCollection<BbCategory> _categoryCollection;
        public OptimizedObservableCollection<BbCategory> CategoryCollection
        {
            get { return _categoryCollection; }
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
        private DateTime _pubDate = DateTime.Now;
        public DateTime PubDate
        {
            get { return _pubDate; }
            set
            {
                _pubDate = value;
                OnPropertyChanged(() => PubDate);
            }
        }
        private string _originalFileName = string.Empty;
        private BbDepartment _prevDepartment;
        private BbCategory _prevCategory;
        private bool _editMode = false;
        private BbCircular _bbCircular;
        #endregion

        #region Constructor(s)
        [ImportingConstructor]
        public BbCircularOperationViewModel(IBbCircularOperationView view, CompositionContainer compositionContainer)
        {
            View = view;
            View.ViewModel = this;
            ShellContainer = compositionContainer;
            _deptCollection = new OptimizedObservableCollection<BbDepartment>();
            _categoryCollection = new OptimizedObservableCollection<BbCategory>();
            BrowseFileCommand = new CommandHandler<object, object>(BrowseFileCommandAction);
        }
        public void PrepareView(BbCircular bbCircular)
        {
            _bbCircular = bbCircular;
            _prevDepartment = new BbDepartment { Name = bbCircular.Department, Key = bbCircular.DepartmentKey };
            Title = bbCircular.Title;
            SelectedFile = bbCircular.FileWithFullPath;
            _originalFileName = bbCircular.FileName;
            _editMode = true;
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
            try
            {
                if (_editMode)
                {
                    await DbHandler.Instance.DeleteBbCircular(_bbCircular);
                }
                await DbHandler.Instance.SaveBbCircularData(new BbCircular
                {
                    Department = SelectedDept != null ? SelectedDept.Name : string.Empty,
                    DepartmentKey = SelectedDept != null ? SelectedDept.Key : string.Empty,
                    Category = SelectedCategory != null ? SelectedCategory.Name : string.Empty,
                    CategoryKey = SelectedCategory != null ? SelectedCategory.Key : string.Empty,
                    Title = Title,
                    FileName = _originalFileName,
                    FileWithFullPath = SelectedFile,
                    PublishDate = PubDate
                });
                base.SaveCommandAction(obj);
            }
            catch (Exception x)
            {
                Log.Error("Error save.", x);
            }
        }
        #endregion

        #region ViewModelBase
        public override async void OnLoad()
        {
            base.OnLoad();
            List<BbDepartment> deptCollection = await DbHandler.Instance.GetAllData<BbDepartment>();
            List<BbCategory> categoryCollection = await DbHandler.Instance.GetAllData<BbCategory>();
            DeptCollection.Clear();
            CategoryCollection.Clear();
            DeptCollection.AddRange(deptCollection);
            CategoryCollection.AddRange(categoryCollection);
        }
        #endregion
    }
}
