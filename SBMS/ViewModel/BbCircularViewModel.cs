using EkushApp.Model;
using EkushApp.ShellService.MVVM;
using SBMS.Generic;
using SBMS.View;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SBMS.ViewModel
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class BbCircularViewModel : GenericViewModel<BbCircular>
    {
        #region Constructor(s)
        [ImportingConstructor]
        public BbCircularViewModel(IBbCircularView view, CompositionContainer compositionContainer)
        {
            View = view;
            View.ViewModel = this;
            ShellContainer = compositionContainer;
            Tag = "Bangladesh Bank Circular";
        }
        #endregion

        #region Command Manager(s)
        public override void NewCommandAction(object obj)
        {
            Console.WriteLine("test...");
        }
        #endregion

        #region ViewModelBase
        public override void OnLoad()
        {
            List<Column> columns = new List<Column>();
            PropertyInfo[] props = typeof(BbCircular).GetProperties();
            foreach (PropertyInfo prop in props)
            {
                object[] attrs = prop.GetCustomAttributes(true);
                foreach (object attr in attrs)
                {
                    if (attr != null && attr is Header)
                    {
                        Header header = (Header)attr;
                        columns.Add(new Column { Header = header.Name, DataField = prop.Name });
                    }
                }
            }
            ColumnConfiguration = new ColumnConfig { Columns = columns };
        }

        public override void OnClosing()
        {
        }
        #endregion
    }
}
