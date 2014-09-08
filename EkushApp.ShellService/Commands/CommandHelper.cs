using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EkushApp.ShellService.Commands
{
    public class CommandHelper : INotifyPropertyChanged, IDisposable
    {
        #region Property(s)
        private static CommandHelper _instance;
        public static CommandHelper Instance
        {
            get
            {
                if (_instance == null) _instance = new CommandHelper();
                return _instance;
            }
        }
        private CommandMap _commands;
        public CommandMap Commands
        {
            get
            {
                if (null == _commands) _commands = new CommandMap();
                return _commands;
            }
        }
        #endregion

        #region Constructor(s)
        private CommandHelper() { }
        ~CommandHelper()
        {
            Dispose(false);
        }
        #endregion

        #region Method(s)
        public void RegisterCommand(string commandName, Action<object> executeMethod, bool isVisible = false)
        {
            Commands.AddCommand(commandName, executeMethod, isVisible);
            OnPropertyChanged(() => Commands);
        }
        public void RegisterCommand(string commandName, Action<object> executeMethod, Predicate<object> canExecuteMethod, bool isVisible = false)
        {
            Commands.AddCommand(commandName, executeMethod, canExecuteMethod, isVisible);
            OnPropertyChanged(() => Commands);
        }
        public void UnRegisterCommand(string commandName)
        {
            Commands.AddCommand(commandName, null, x => false, false);
            OnPropertyChanged(() => Commands);
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
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

        public static void Destroy()
        {
            if (null != _instance)
            {
                _instance.Commands.Dispose();
                _instance._commands = null;
                _instance.Dispose();
            }
        }

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
