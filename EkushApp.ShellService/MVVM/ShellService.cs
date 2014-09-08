using Microsoft.Practices.Prism.Regions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkushApp.ShellService.MVVM
{
    [Export(typeof(IShellService))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ShellService : IShellService
    {
        [Import]
        private IRegionManager _regionManager;

        public void AddViewToContentPane(IViewModel viewModel, string regionname)
        {
            _regionManager.RegisterViewWithRegion(regionname, () => viewModel.View);
        }

        public bool RemoveViewFromContentPane(IViewModel viewModel, string regionname)
        {
            try
            {
                IRegion region = _regionManager.Regions[regionname];
                region.Remove(viewModel.View);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
    }
}
