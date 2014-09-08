using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EkushApp.ShellService.Commands
{
    public interface IRelayCommand : ICommand, IDisposable
    {
        bool IsVisible { get; set; }
    }
}
