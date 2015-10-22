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
    /// Interaction logic for BbSearchOperationView.xaml
    /// </summary>
    /// 
    [Export(typeof(IBbSearchOperationView))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class BbSearchOperationView : UserControlBase, IBbSearchOperationView
    {
        public BbSearchOperationView()
        {
            InitializeComponent();
        }
    }
    public interface IBbSearchOperationView : IView { }
}
