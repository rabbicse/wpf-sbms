using EkushApp.ShellService.MVVM;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBMS.ViewModel
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class BbCircularViewModel : ViewModelBase
    {
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
