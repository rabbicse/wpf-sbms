using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EkushApp.Utility.Tasks
{
    public sealed class TaskHandler
    {
        #region Declaration(s)
        private static ConcurrentDictionary<string, TaskHandler> _tasks = new ConcurrentDictionary<string, TaskHandler>();
        #endregion

        #region Property(s)
        private AutoResetEvent _waitHandle;
        private AutoResetEvent _abortHandle;
        private CancellationTokenSource _cancelTaskToken;
        private Action _action;
        private Task _task;
        #endregion

        #region Constructor(s)
        public static TaskHandler Instance(ITaskable taskableObj)
        {
            var componentName = taskableObj.Name;
            if (!_tasks.ContainsKey(taskableObj.Name))
            {
                var handler = new TaskHandler();
                handler._action = taskableObj.Operation;
                handler._waitHandle = taskableObj.WaitHandle;
                handler._abortHandle = taskableObj.AbortHandle;
                _tasks[componentName] = handler;
            }
            return _tasks[componentName];
        }
        private TaskHandler() { }
        ~TaskHandler() { }
        #endregion

        #region Method(s)
        public void Start()
        {
            if (_task == null)
            {
                _task = Task.Factory.StartNew(_action, CancellationToken.None,
                    TaskCreationOptions.LongRunning, TaskScheduler.Default);

            }
            else if (_task != null
                    && (_task.Status == TaskStatus.RanToCompletion || _task.Status == TaskStatus.Running)
                    && _waitHandle != null)
            {
                _waitHandle.Set();
            }
        }
        public void Stop()
        {
            if (_task != null)
            {
                _waitHandle.Set();
                _abortHandle.Set();                
                _task.Wait();
                _task = null;
            }
        }
        public static void Shutdown() 
        {
            _tasks.AsParallel().ForAll(t =>
                {
                    t.Value._waitHandle.Set();
                    t.Value._abortHandle.Set();
                });
            Task.WaitAll(_tasks.Select(t => t.Value._task).ToArray(), TimeSpan.FromSeconds(20));
            _tasks.Clear();
        }
        #endregion
    }
}
