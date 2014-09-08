using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EkushApp.EmbededDB;
using EkushApp.ShellService.MVVM;
using EkushApp.Utility.Threads;
using EkushApp.Utility.Extensions;
using Microsoft.Practices.Prism.Events;
using System.Windows.Data;
using EkushApp.Utility.Tasks;
using EkushApp.Utility.WinRegistry;
using SBMS.View;
using EkushApp.ShellService.Commands;

namespace NID.ViewModel
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ShellViewModel : ViewModelBase
    {
        #region Command(s)
        public CommandHandler<object, object> LogoutCommand { get; private set; }
        #endregion

        #region Event(s)
        public event EventHandler OnShutDown;
        public event EventHandler OnLogout;
        #endregion

        #region Property(s)
        #endregion

        #region Constructor(s)
        [ImportingConstructor]
        public ShellViewModel(IShellView view, CompositionContainer compositionContainer, IEventAggregator eventAggregator, Lazy<IShellService> shellService)
        {
            View = view;
            View.ViewModel = this;
            ShellContainer = compositionContainer;
            EventAggregator = eventAggregator;
            ShellService = shellService;
            LogoutCommand = new CommandHandler<object, object>(LogoutCommandAction);
        }
        #endregion

        #region CommandHandler(s)
        private void LogoutCommandAction(object obj)
        {
            if (null != OnLogout)
            {
                OnLogout(this, new EventArgs { });
            }
        }
        #endregion

        #region EventAggregator

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
