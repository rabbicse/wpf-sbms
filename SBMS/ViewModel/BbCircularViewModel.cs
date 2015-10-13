using EkushApp.Model;
using EkushApp.ShellService.MVVM;
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
    public class BbCircularViewModel : GenericViewModel<BbCircular>
    {
        #region Constructor(s)
        [ImportingConstructor]
        public BbCircularViewModel(IBbCircularView view, CompositionContainer compositionContainer) 
        {
            View = view;
            View.ViewModel = this;
            ShellContainer = compositionContainer;
            Tag = "Bangladesh Bank Circular";
        }
        #endregion

        #region Command Manager(s)
        public override void NewCommandAction(object obj)
        {
            Console.WriteLine("test...");
        }
        #endregion

        #region ViewModelBase
        public override void OnLoad()
        {
        }

        public override void OnClosing()
        {
        }
        #endregion
    }
}
