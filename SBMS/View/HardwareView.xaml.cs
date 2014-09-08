using EkushApp.ShellService.MVVM;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SBMS.View
{
    /// <summary>
    /// Interaction logic for HardwareView.xaml
    /// </summary>
    /// 
    [Export(typeof(IHardwareView))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class HardwareView : UserControlBase, IHardwareView
    {
        public HardwareView()
        {
            InitializeComponent();
        }
    }
    public interface IHardwareView : IView { }
}
