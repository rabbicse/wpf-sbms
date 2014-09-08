using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace EkushApp.ShellService.MVVM
{
    [Serializable]
    public abstract class ViewModelBase : IViewModel, IBusyIndicator, INotifyPropertyChanged, IDisposable
    {
        #region Constructor(s)
        ~ViewModelBase()
        {
            Dispose(false);
        }
        #endregion

        #region Property(s)
        [Import]
        protected CompositionContainer ShellContainer;

        [Import]
        protected IEventAggregator EventAggregator;

        [Import]
        protected ILoggerFacade Logger;

        [Import]
        protected Lazy<IShellService> ShellService { get; set; }

        protected Dispatcher ViewDispatcher
        {
            get { return this.View.ViewDispatcher; }
        }
        #endregion

        #region Abstract Method(s)
        public abstract void OnLoad();
        public abstract void OnClosing();
        public virtual void OnRegisterCommands() { }        
        public virtual void OnUnRegisterCommands() { }
        #endregion

        #region IViewModel
        private IView _view;
        public IView View
        {
            get { return _view; }
            set { _view = value; }
        }
        #endregion

        #region IBusyIndicator
        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                OnPropertyChanged(() => IsBusy);
            }
        }
        private string _busyContent;
        public string BusyContent
        {
            get { return _busyContent; }
            set
            {
                _busyContent = value;
                OnPropertyChanged(() => BusyContent);
            }
        }
        public void ShowBusyIndicator(string message)
        {
            BusyContent = message;
            IsBusy = true;
        }
        public void HideBusyIndicator()
        {
            BusyContent = string.Empty;
            IsBusy = false;
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Notify using pre-made PropertyChangedEventArgs
        /// </summary>
        /// <param name="args"></param>
        protected void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Notify using String property name
        /// </summary>
        protected void OnPropertyChanged(String propertyName)
        {
            this.VerifyPropertyName(propertyName);
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        protected void OnPropertyChanged(Expression<Func<Object>> propertyExpression)
        {
            string propertyName = GetPropertyName(propertyExpression);

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public static string GetPropertyName(Expression<Func<Object>> propertyExpression)
        {
            var lambda = propertyExpression as LambdaExpression;
            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = lambda.Body as UnaryExpression;
                memberExpression = unaryExpression.Operand as MemberExpression;
            }
            else
            {
                memberExpression = lambda.Body as MemberExpression;
            }
            var constantExpression = memberExpression.Expression as ConstantExpression;
            var propertyInfo = memberExpression.Member as PropertyInfo;

            return propertyInfo.Name;
        }
        #endregion

        #region Debugging Aides

        /// <summary>
        /// Warns the developer if this object does not have
        /// a public property with the specified name. This 
        /// method does not exist in a Release build.
        /// </summary>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
#if !SILVERLIGHT
            // Verify that the property name matches a real,  
            // public, instance property on this object.
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string msg = "Invalid property name: " + propertyName;

                if (this.ThrowOnInvalidPropertyName)
                    throw new Exception(msg);
                else
                    Debug.Fail(msg);
            }
#endif
        }

        /// <summary>
        /// Returns whether an exception is thrown, or if a Debug.Fail() is used
        /// when an invalid property name is passed to the VerifyPropertyName method.
        /// The default value is false, but subclasses used by unit tests might 
        /// override this property's getter to return true.
        /// </summary>
        protected virtual bool ThrowOnInvalidPropertyName { get; private set; }

        #endregion // Debugging Aides

        #region IParentablePropertyExposer
        /// <summary>
        /// Returns the list of delegates that are currently subscribed for the
        /// <see cref="System.ComponentModel.INotifyPropertyChanged">INotifyPropertyChanged</see>
        /// PropertyChanged event
        /// </summary>
        public Delegate[] GetINPCSubscribers()
        {
            return PropertyChanged == null ? null : PropertyChanged.GetInvocationList();
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
