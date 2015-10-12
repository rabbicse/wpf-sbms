using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkushApp.ShellService.MVVM
{
    public interface IPopup
    {
        bool IsShowPopup { get; set; }
        object PopupContent { get; set; }
    }
}
