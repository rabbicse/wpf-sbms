using EkushApp.EmbededDB;
using EkushApp.ShellService.Commands;
using EkushApp.ShellService.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBMS.ViewModel
{
    public class GenericOperationViewModel<T> : ViewModelBase
    {
        #region Event(s)
        public event EventHandler OnClosed;
        #endregion

        #region Property(s)
        public T Data { get; set; }
        #endregion

        #region Command(s)
        public CommandHandler<object, object> SaveCommand { get; private set; }
        public CommandHandler<object, object> CloseCommand { get; private set; }
        #endregion

        #region Constructor(s)
        public GenericOperationViewModel()
        {
            SaveCommand = new CommandHandler<object, object>(SaveCommandAction);
            CloseCommand = new CommandHandler<object, object>(CloseCommandAction);
        }        
        #endregion

        #region CommandHandler(s)
        public virtual void SaveCommandAction(object obj)
        {
            if (null != OnClosed)
            {
                OnClosed(this, new EventArgs { });
            }
        }
        private void CloseCommandAction(object obj)
        {
            if (null != OnClosed)
            {
                OnClosed(this, new EventArgs { });
            }
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
