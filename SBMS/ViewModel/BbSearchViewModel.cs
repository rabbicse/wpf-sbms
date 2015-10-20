using EkushApp.Model;
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
    public class BbSearchViewModel : GenericViewModel<BbCircularSearch>
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
        #endregion

        public override void OnLoad()
        {
            base.OnLoad();
            Collection.Add(new BbCircularSearch { Title = "Test", PublishDate = DateTime.Now });
        }
    }
}
