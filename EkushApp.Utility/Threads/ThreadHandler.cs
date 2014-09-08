using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EkushApp.Utility.Threads
{
    public class ThreadHandler
    {
        #region Declaration(s)
        private static IDictionary<string, ThreadHandler> _threads = new Dictionary<string, ThreadHandler>();
        private static Object _handlerListLock = new Object();
        private static volatile bool _shutdown = false;
        private Thread _thread = null;
        private Object _handlerLock = null;
        private AutoResetEvent _waitHandle = null;
        private AutoResetEvent _abortHandle = null;
        private CancellationTokenSource _cancelTaskToken = null;
        private ThreadStart _task = null;
        private int MAX_WAIT_TIME = 5 * 1000; // 5 seconds
        #endregion

        #region Constructor(s)
        public static ThreadHandler Instance(IThreadable threadableObject)
        {
            lock (_handlerListLock)
            {
                // It's better not to return null if shutdown already initiated
                // because most callers may not handle null, and that can result in an Exception.
                // The strategy is to return a new handler to the caller, but not let them start
                // a new thread

                string componentName = threadableObject.Name;
                if (!_threads.ContainsKey(componentName))
                {
                    var handler = new ThreadHandler();
                    handler._task = threadableObject.Operation;
                    handler._waitHandle = threadableObject.WaitHandle;
                    handler._abortHandle = threadableObject.AbortHandle;
                    handler._cancelTaskToken = threadableObject.CancelTaskToken;
                    _threads.Add(componentName, handler);
                }
                return _threads[componentName];
            }
        }

        public ThreadHandler()
        {
            _handlerLock = new Object();
        }

        ~ThreadHandler() { }
        #endregion

        #region Method(s)
        /// <summary>
        /// Start/wake thread
        /// </summary>
        public void Start()
        {
            if (_shutdown) return;

            lock (_handlerLock)
            {
                if (_thread == null || !_thread.IsAlive)
                {
                    _thread = new Thread(_task);
                    _thread.Start();
                }
                else if (_thread.IsAlive && _waitHandle != null)
                {
                    _waitHandle.Set();
                }
            }
        }


        /// <summary>
        /// Tries to stop the thread when it reaches a sleep state.
        /// </summary>
        public void Stop()
        {
            if (_shutdown) return; // Since shutdown initiated, that will stop all threads

            lock (_handlerLock)
            {
                if (_thread != null && _thread.IsAlive)
                {
                    _abortHandle.Set();
                    _waitHandle.Set();
                    if (!_cancelTaskToken.IsCancellationRequested) _cancelTaskToken.Cancel();
                    _thread.Join();
                }
            }
        }

        /// <summary>
        /// Call only when shutting down application.
        /// Tries to gracefully stop all threads, but if times out, then
        /// force stops those threads.
        /// </summary>
        public static void Shutdown()
        {
            lock (_handlerListLock)
            {
                if (_shutdown) return;
                _shutdown = true;

                // Attempt to stop all running threads
                foreach (var pair in _threads)
                {
                    var handler = pair.Value;
                    lock (handler._handlerLock)
                    {
                        if (handler._thread != null && handler._thread.IsAlive)
                        {
                            handler._abortHandle.Set();
                            handler._waitHandle.Set();
                        }
                    }
                }

                // Wait for threads to stop
                foreach (var pair in _threads)
                {
                    var handler = pair.Value;
                    lock (handler._handlerLock)
                    {
                        if (handler._thread != null && !handler._thread.Join(handler.MAX_WAIT_TIME))
                        {
                            // Graceful stop failed. So do force stop.
                            handler._thread.Abort();
                            handler._thread.Join();
                        }
                    }
                }
            }
        }
        #endregion
    }
}
