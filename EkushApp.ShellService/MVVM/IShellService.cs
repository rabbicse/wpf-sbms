using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EkushApp.ShellService.MVVM
{
    public interface IShellService
    {
        void AddViewToContentPane(IViewModel viewModel, string regionname);
        bool RemoveViewFromContentPane(IViewModel viewModel, string regionname);
    }
}
