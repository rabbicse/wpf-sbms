using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EkushApp.Utility.Tasks
{
    public interface ITaskable
    {
        string Name { get; }
        int SleepTime { get; }
        //CancellationTokenSource AbortTokenSource { get; }
        //CancellationTokenSource WaitTokenSource { get; }
        AutoResetEvent WaitHandle { get; }
        AutoResetEvent AbortHandle { get; }
        void Operation();
    }
}
