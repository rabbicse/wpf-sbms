using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EkushApp.ShellService.WeakEvents
{
    public class WeakEventAction
    {
        #region Data

        readonly MethodInfo _method;
        readonly Type _delegateType;
        #endregion

        #region Public Properties
        public WeakReference TargetObject { get; private set; }
        #endregion

        #region Internal Methods

        /// <summary>
        /// Constructs a new WeakAction
        /// </summary>
        /// <param name="target">The sender</param>
        /// <param name="method">The _method to call on sender</param>
        /// <param name="parameterType">The parameter type if using generics</param>
        public WeakEventAction(object target, MethodInfo method, Type parameterType)
        {
            this.TargetObject = new WeakReference(target);
            this._method = method;
            this._delegateType = parameterType == null
                                 ? typeof(Action)
                                 : typeof(Action<>).MakeGenericType(parameterType);
        }

        /// <summary>
        /// Creates callback delegate
        /// </summary>
        /// <returns>Callback delegate</returns>
        internal Delegate CreateAction()
        {
            var target = TargetObject.Target;
            if (target != null)
            {
                // Rehydrate into a real Action
                // object, so that the _method
                // can be invoked on the target.
                return Delegate.CreateDelegate(this._delegateType, TargetObject.Target, this._method);
            }

            return null;
        }
        #endregion
    }

    public class WeakActionEvent<T>
    {
        public static WeakActionEvent<T> operator +(WeakActionEvent<T> wre, Action<T> handler)
        {
            wre.Add(handler);
            return wre;
        }

        private void Add(Action<T> handler)
        {
            var parameters = handler.Method.GetParameters();

            if (parameters != null && parameters.Length > 1)
                throw new InvalidOperationException("Action should have only 0 or 1 parameter");

            if (_delegates.Any(del => del.TargetObject.Target == handler.Target))
            {
                return;
            }

            var parameterType = (parameters == null || parameters.Length == 0)
                                ? null
                                : parameters[0].ParameterType;

            _delegates.Add(new WeakEventAction(handler.Target, handler.Method, parameterType));
        }

        public static WeakActionEvent<T> operator -(WeakActionEvent<T> wre, Action<T> handler)
        {
            wre.Remove(handler);
            return wre;
        }
        private void Remove(Action<T> handler)
        {
            for (int index = 0; index < _delegates.Count; index++)
            {
                var del = _delegates[index];
                if (del.TargetObject.Target == handler.Target)
                {
                    _delegates.Remove(del);
                    return;
                }
            }
        }

        readonly List<WeakEventAction> _delegates = new List<WeakEventAction>();

        internal void Invoke(T arg)
        {
            for (var i = _delegates.Count - 1; i > -1; --i)
            {
                var weakAction = _delegates[i];
                if (!weakAction.TargetObject.IsAlive)
                    _delegates.RemoveAt(i);
                else
                {
                    var action = weakAction.CreateAction();
                    action.DynamicInvoke(arg);
                }
            }
        }
    }
}
