using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace EkushApp.ShellService.Commands
{
    public class EventToCommandTrigger : TriggerAction<FrameworkElement>
    {
        #region Overrides
        /// <param name="parameter">The EventArgs of the fired event.</param>
        protected override void Invoke(object parameter)
        {
            if (IsAssociatedElementDisabled())
            {
                return;
            }

            ICommand command = Command;
            if (command != null && command.CanExecute(CommandParameter))
            {
                command.Execute(new EventToCommandArgs(this.AssociatedObject, command,
                    CommandParameter, (EventArgs)parameter));
            }
        }

        /// <summary>
        /// Called when this trigger is attached to a FrameworkElement.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            EnableDisableElement();
        }
        #endregion

        #region Private Methods
        private static void OnCommandChanged(EventToCommandTrigger thisTrigger, DependencyPropertyChangedEventArgs e)
        {
            if (thisTrigger == null)
            {
                return;
            }

            if (e.OldValue != null)
            {
                ((ICommand)e.OldValue).CanExecuteChanged -= thisTrigger.OnCommandCanExecuteChanged;
            }

            ICommand command = (ICommand)e.NewValue;

            if (command != null)
            {
                command.CanExecuteChanged += thisTrigger.OnCommandCanExecuteChanged;
            }

            thisTrigger.EnableDisableElement();
        }

        private bool IsAssociatedElementDisabled()
        {
#if !SILVERLIGHT
            return AssociatedObject != null && !AssociatedObject.IsEnabled;
#else
            return false;
#endif
        }

        private void EnableDisableElement()
        {
            if (AssociatedObject == null || Command == null)
            {
                return;
            }
#if !SILVERLIGHT
            AssociatedObject.IsEnabled = Command.CanExecute(this.CommandParameter);
#endif

        }

        private void OnCommandCanExecuteChanged(object sender, EventArgs e)
        {
            EnableDisableElement();
        }
        #endregion

        #region CommandParameter
        /// <summary>
        /// Identifies the <see cref="CommandParameter" /> dependency property
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
            "CommandParameter", typeof(object), typeof(EventToCommandTrigger),
            new PropertyMetadata(null,
                (s, e) =>
                {
                    EventToCommandTrigger sender = s as EventToCommandTrigger;
                    if (sender == null)
                    {
                        return;
                    }

                    if (sender.AssociatedObject == null)
                    {
                        return;
                    }

                    sender.EnableDisableElement();
                }));


        /// <summary>
        /// Gets or sets an object that will be passed to the <see cref="Command" />
        /// attached to this trigger. This is a DependencyProperty.
        /// </summary>
        public object CommandParameter
        {
            get { return (Object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }
        #endregion

        #region Command
        /// <summary
        /// >
        /// Identifies the <see cref="Command" /> dependency property
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command", typeof(ICommand), typeof(EventToCommandTrigger),
            new PropertyMetadata(null,
                (s, e) => OnCommandChanged(s as EventToCommandTrigger, e)));


        /// <summary>
        /// Gets or sets the ICommand that this trigger is bound to. This
        /// is a DependencyProperty.
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }
        #endregion
    }
}
