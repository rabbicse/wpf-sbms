using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkushApp.ShellService.MVVM
{
    public interface IViewModel
    {
        IView View { get; set; }
        void OnLoad();
        void OnClosing();
        void OnRegisterCommands();
        void OnUnRegisterCommands();
    }
}
