using EkushApp.Model;
using EkushApp.ShellService.MVVM;
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
        #region Constructor(s)
        [ImportingConstructor]
        public BbSearchViewModel(IBbSearchView view, CompositionContainer compositionContainer)
        {
            View = view;
            View.ViewModel = this;
            ShellContainer = compositionContainer;
            Tag = "BB Search Category";            
        }

        public void OperationVM_OnClosed(object sender, EventArgs e)
        {
            OnCloseOperation();
            OperationVM.OnClosed -= OperationVM_OnClosed;
            _operationVM = null;
            base.OnLoad();
        }
        #endregion

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
        }
        public override void OnClosing()
        {
            base.OnClosing();
            this.Dispose();
        }
        #endregion
    }
}
