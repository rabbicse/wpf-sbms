using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace EkushApp.ShellService.MVVM
{
    public interface IView
    {
        IViewModel ViewModel { get; set; }
        Dispatcher ViewDispatcher { get; }
    }
}
