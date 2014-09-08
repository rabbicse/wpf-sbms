using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkushApp.ShellService.MVVM
{
    public interface IBusyIndicator
    {
        bool IsBusy { get; set; }
        string BusyContent { get; set; }
        void ShowBusyIndicator(string message);
        void HideBusyIndicator();
    }
}
