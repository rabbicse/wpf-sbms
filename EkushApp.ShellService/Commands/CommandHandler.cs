using EkushApp.ShellService.WeakEvents;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EkushApp.ShellService.Commands
{
    /// <summary>
    /// Interface that is used for ICommands that notify when they are
    /// completed
    /// </summary>
    public interface ICompletionAwareCommand
    {
        /// <summary>
        /// Notifies that the command has completed
        /// </summary>
        //event Action<Object> CommandCompleted;

        WeakActionEvent<object> CommandCompleted { get; set; }
    }


    /// <summary>
    /// Simple delegating command, based largely on DelegateCommand from PRISM/CAL
    /// </summary>
    /// <typeparam name="T">The type for the </typeparam>
    public class CommandHandler<T1, T2> : ICommand, ICompletionAwareCommand
    {
        private Func<T1, bool> canExecuteMethod;
        private Action<T2> executeMethod;
        public WeakActionEvent<object> CommandCompleted { get; set; }

        public CommandHandler(Func<T1, bool> canExecuteMethod, Action<T2> executeMethod)
        {
            this.executeMethod = executeMethod;
            this.canExecuteMethod = canExecuteMethod;
            this.CommandCompleted = new WeakActionEvent<object>();
        }

        public CommandHandler(Action<T2> executeMethod)
        {
            this.executeMethod = executeMethod;
            this.canExecuteMethod = (x) => { return true; };
            this.CommandCompleted = new WeakActionEvent<object>();
        }        

        public bool CanExecute(T1 parameter)
        {
            if (canExecuteMethod == null) return true;
            return canExecuteMethod(parameter);
        }

        public void Execute(T2 parameter)
        {
            if (executeMethod != null)
            {
                executeMethod(parameter);
            }

            //now raise CommandCompleted for this ICommand
            WeakActionEvent<object> completedHandler = CommandCompleted;
            if (completedHandler != null)
            {

                completedHandler.Invoke(parameter);
            }
        }

        public bool CanExecute(object parameter)
        {
            return CanExecute((T1)parameter);
        }

        public void Execute(object parameter)
        {
            Execute((T2)parameter);
        }

#if SILVERLIGHT
        /// <summary>
        /// Occurs when changes occur that affect whether the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;
#else
        /// <summary>
        /// Occurs when changes occur that affect whether the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (canExecuteMethod != null)
                {
                    CommandManager.RequerySuggested += value;
                }
            }

            remove
            {
                if (canExecuteMethod != null)
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }
#endif



        /// <summary>
        /// Raises the <see cref="CanExecuteChanged" /> event.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
            Justification = "The this keyword is used in the Silverlight version")]
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
            Justification = "This cannot be an event")]
        public void RaiseCanExecuteChanged()
        {
#if SILVERLIGHT
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
#else
            CommandManager.InvalidateRequerySuggested();
#endif
        }
    }
}
