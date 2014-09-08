/*
 * Ref: http://blogs.msdn.com/b/morgan/archive/2010/06/24/simplifying-commands-in-mvvm-and-wpf.aspx 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EkushApp.ShellService.Commands
{

    /// <summary>
    /// A map that exposes commands in a WPF binding friendly manner
    /// </summary>
    [TypeDescriptionProvider(typeof(CommandMapDescriptionProvider))]
    public class CommandMap : IDisposable
    {
        internal CommandMap() { }
        ~CommandMap()
        {
            Dispose(false);
        }
        /// <summary>
        /// Add a named command to the command map
        /// </summary>
        /// <param name="commandName">The name of the command</param>
        /// <param name="executeMethod">The method to execute</param>
        internal void AddCommand(string commandName, Action<object> executeMethod, bool isVisible)
        {
            if (RelayCommands.ContainsKey(commandName)) RelayCommands[commandName].Dispose();
            RelayCommands[commandName] = new RelayCommand(executeMethod, isVisible);
        }

        /// <summary>
        /// Add a named command to the command map
        /// </summary>
        /// <param name="commandName">The name of the command</param>
        /// <param name="executeMethod">The method to execute</param>
        /// <param name="canExecuteMethod">The method to execute to check if the command can be executed</param>
        internal void AddCommand(string commandName, Action<object> executeMethod, Predicate<object> canExecuteMethod, bool isVisible)
        {
            if (RelayCommands.ContainsKey(commandName)) RelayCommands[commandName].Dispose();
            RelayCommands[commandName] = new RelayCommand(executeMethod, canExecuteMethod, isVisible);
        }

        /// <summary>
        /// Remove a command from the command map
        /// </summary>
        /// <param name="commandName">The name of the command</param>
        internal void RemoveCommand(string commandName)
        {
            if (RelayCommands.ContainsKey(commandName))
            {
                RelayCommands[commandName].Dispose();
                RelayCommands.Remove(commandName);
            }
        }

        /// <summary>
        /// Store the commands
        /// </summary>
        private Dictionary<string, IRelayCommand> _relayCommands;
        /// <summary>
        /// Expose the dictionary of commands
        /// </summary>
        protected Dictionary<string, IRelayCommand> RelayCommands
        {
            get
            {
                if (null == _relayCommands) _relayCommands = new Dictionary<string, IRelayCommand>();

                return _relayCommands;
            }
        }
        #region IDisposeable
        public void Dispose()
        {
            if (RelayCommands != null && RelayCommands.Count > 0)
            {
                foreach (string commandName in RelayCommands.Keys)
                {
                    RelayCommands[commandName].Dispose();
                }
            }
            RelayCommands.Clear();
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
        }
        #endregion

        /// <summary>
        /// Implements ICommand in a delegate friendly way
        /// </summary>
        ///         
        private class RelayCommand : IRelayCommand
        {
            private List<EventHandler> _canExecuteSubscribers = new List<EventHandler>();
            private readonly Action<object> _executeMethod;
            private readonly Predicate<object> _canExecuteMethod;
            private bool _isVisible;
            public bool IsVisible
            {
                get { return _isVisible; }
                set
                {
                    _isVisible = value;
                }
            }

            /// <summary>
            /// Create a command that can always be executed
            /// </summary>
            /// <param name="executeMethod">The method to execute when the command is called</param>
            public RelayCommand(Action<object> executeMethod, bool isVisible) : this(executeMethod, null, isVisible) { }

            /// <summary>
            /// Create a delegate command which executes the canExecuteMethod before executing the executeMethod
            /// </summary>
            /// <param name="executeMethod"></param>
            /// <param name="canExecuteMethod"></param>
            public RelayCommand(Action<object> executeMethod, Predicate<object> canExecuteMethod, bool isVisible)
            {
                this._executeMethod = executeMethod;
                this._canExecuteMethod = canExecuteMethod;
                this._isVisible = isVisible;
            }

            ~RelayCommand()
            {
                Dispose(false);
            }

            public bool CanExecute(object parameter)
            {
                return (null == _canExecuteMethod) ? true : _canExecuteMethod(parameter);
            }

            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            public void Execute(object parameter)
            {
                if (null != _executeMethod)
                {
                    _executeMethod(parameter);
                }
            }

            #region IDisposeable
            public void Dispose()
            {
                _canExecuteSubscribers.ForEach(h => CanExecuteChanged -= h);
                _canExecuteSubscribers.Clear();
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            protected virtual void Dispose(bool disposing)
            {
                if (!disposing) return;
            }
            #endregion
        }


        /// <summary>
        /// Expose the dictionary entries of a CommandMap as properties
        /// </summary>
        private class CommandMapDescriptionProvider : TypeDescriptionProvider
        {
            /// <summary>
            /// Standard constructor
            /// </summary>
            public CommandMapDescriptionProvider()
                : this(TypeDescriptor.GetProvider(typeof(CommandMap)))
            {
            }

            /// <summary>
            /// Construct the provider based on a parent provider
            /// </summary>
            /// <param name="parent"></param>
            public CommandMapDescriptionProvider(TypeDescriptionProvider parent)
                : base(parent)
            {
            }

            /// <summary>
            /// Get the type descriptor for a given object instance
            /// </summary>
            /// <param name="objectType">The type of object for which a type descriptor is requested</param>
            /// <param name="instance">The instance of the object</param>
            /// <returns>A custom type descriptor</returns>
            public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
            {
                return new CommandMapDescriptor(base.GetTypeDescriptor(objectType, instance), instance as CommandMap);
            }
        }

        /// <summary>
        /// This class is responsible for providing custom properties to WPF - in this instance
        /// allowing you to bind to commands by name
        /// </summary>
        private class CommandMapDescriptor : CustomTypeDescriptor
        {
            /// <summary>
            /// Store the command map for later
            /// </summary>
            /// <param name="descriptor"></param>
            /// <param name="map"></param>
            public CommandMapDescriptor(ICustomTypeDescriptor descriptor, CommandMap map)
                : base(descriptor)
            {
                _map = map;
            }

            /// <summary>
            /// Get the properties for this command map
            /// </summary>
            /// <returns>A collection of synthesized property descriptors</returns>
            public override PropertyDescriptorCollection GetProperties()
            {
                //TODO: See about caching these properties (need the _map to be observable so can respond to add/remove)
                PropertyDescriptor[] props = new PropertyDescriptor[_map.RelayCommands.Count];

                int pos = 0;

                foreach (KeyValuePair<string, IRelayCommand> command in _map.RelayCommands)
                    props[pos++] = new CommandPropertyDescriptor(command);

                return new PropertyDescriptorCollection(props);
            }

            private CommandMap _map;
        }

        /// <summary>
        /// A property descriptor which exposes an ICommand instance
        /// </summary>
        private class CommandPropertyDescriptor : PropertyDescriptor
        {
            /// <summary>
            /// Construct the descriptor
            /// </summary>
            /// <param name="command"></param>
            public CommandPropertyDescriptor(KeyValuePair<string, IRelayCommand> command)
                : base(command.Key, null)
            {
                _command = command.Value;
            }

            /// <summary>
            /// Always read only in this case
            /// </summary>
            public override bool IsReadOnly
            {
                get { return true; }
            }

            /// <summary>
            /// Nope, it's read only
            /// </summary>
            /// <param name="component"></param>
            /// <returns></returns>
            public override bool CanResetValue(object component)
            {
                return false;
            }

            /// <summary>
            /// Not needed
            /// </summary>
            public override Type ComponentType
            {
                get { throw new NotImplementedException(); }
            }

            /// <summary>
            /// Get the ICommand from the parent command map
            /// </summary>
            /// <param name="component"></param>
            /// <returns></returns>
            public override object GetValue(object component)
            {
                CommandMap map = component as CommandMap;

                if (null == map)
                    throw new ArgumentException("component is not a CommandMap instance", "component");

                return map.RelayCommands[this.Name];
            }

            /// <summary>
            /// Get the type of the property
            /// </summary>
            public override Type PropertyType
            {
                get { return typeof(ICommand); }
            }

            /// <summary>
            /// Not needed
            /// </summary>
            /// <param name="component"></param>
            public override void ResetValue(object component)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Not needed
            /// </summary>
            /// <param name="component"></param>
            /// <param name="value"></param>
            public override void SetValue(object component, object value)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Not needed
            /// </summary>
            /// <param name="component"></param>
            /// <returns></returns>
            public override bool ShouldSerializeValue(object component)
            {
                return false;
            }

            /// <summary>
            /// Store the command which will be executed
            /// </summary>
            private ICommand _command;
        }
    }
}
