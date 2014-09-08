using EkushApp.ShellService.MVVM;
using SBMS.View;
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
    public class UserViewModel : ViewModelBase
    {
        public string Tag { get { return "User"; } }
        #region Constructor(s)
        [ImportingConstructor]
        public UserViewModel(IUserView view)
        {
            View = view;
            View.ViewModel = this;
        }
        #endregion
        #region ViewModelBase
        public override void OnLoad()
        {
        }

        public override void OnClosing()
        {
            this.Dispose();
        }
        #endregion
    }
}
