using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace EkushApp.ShellService.MVVM
{
    public class UserControlBase : UserControl, IView, IDisposable
    {
        #region Constructor(s)
        public UserControlBase()
        {
            this.Loaded += OnUserControlBaseLoaded;
            this.Unloaded += OnUserControlBaseUnLoaded;
        }
        ~UserControlBase()
        {
            Dispose(false);
        }
        #endregion

        #region Event(s)
        private void OnUserControlBaseLoaded(object sender, RoutedEventArgs e)
        {
            if (null != ViewModel)
            {                
                ViewModel.OnLoad();
                ViewModel.OnRegisterCommands();
            }
        }

        private void OnUserControlBaseUnLoaded(object sender, RoutedEventArgs e)
        {
            if (null != ViewModel)
            {
                ViewModel.OnUnRegisterCommands();
                ViewModel.OnClosing();
            }
            this.Dispose();
        }
        #endregion

        #region IView(s)
        public IViewModel ViewModel
        {
            get
            {
                return this.DataContext as IViewModel;
            }
            set
            {
                this.DataContext = value;
            }
        }
        public Dispatcher ViewDispatcher
        {
            get { return this.Dispatcher; }
        }
        #endregion

        #region IDisposeable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
        }
        #endregion
    }
}
