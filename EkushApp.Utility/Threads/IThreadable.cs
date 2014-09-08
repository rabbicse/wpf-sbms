using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EkushApp.Utility.Threads
{
    public interface IThreadable
    {
        string Name { get; }
        int SleepTime { get; }
        AutoResetEvent WaitHandle { get; }
        AutoResetEvent AbortHandle { get; }
        CancellationTokenSource CancelTaskToken { get; }
        void Operation();
    }
}
