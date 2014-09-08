using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace EkushApp.ShellService.MVVM
{
    public class WindowBase : Window, IView, IDisposable
    {
        #region Constructor(s)
        public WindowBase()
        {
            this.Loaded += OnWindowBaseLoaded;
            this.Closing += OnWindowBaseClosing;
        }
        ~WindowBase()
        {
            Dispose(false);
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

        #region Event(s)
        void OnWindowBaseLoaded(object sender, RoutedEventArgs e)
        {
            if (null != ViewModel)
            {
                ViewModel.OnLoad();
                ViewModel.OnRegisterCommands();
            }
        }
        void OnWindowBaseClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Loaded -= OnWindowBaseLoaded;
            this.Closing -= OnWindowBaseClosing;
            if (null != ViewModel)
            {
                ViewModel.OnUnRegisterCommands();
                ViewModel.OnClosing();
            }
            this.Dispose();
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
