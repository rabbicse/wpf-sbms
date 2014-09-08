using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EkushApp.ShellService.Commands
{
    public class EventToCommandArgs
    {
        public Object Sender { get; private set; }
        public ICommand CommandRan { get; private set; }
        public Object CommandParameter { get; private set; }
        public EventArgs EventArgs { get; private set; }


        public EventToCommandArgs(Object sender, ICommand commandRan, Object commandParameter, EventArgs eventArgs)
        {
            Sender = sender;
            CommandRan = commandRan;
            CommandParameter = commandParameter;
            EventArgs = eventArgs;
        }
    }
}
