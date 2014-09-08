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
    /// Interaction logic for UserView.xaml
    /// </summary>
    /// 
    [Export(typeof(IUserView))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class UserView : UserControlBase, IUserView
    {
        public UserView()
        {
            InitializeComponent();
        }
    }
    public interface IUserView : IView { }
}
