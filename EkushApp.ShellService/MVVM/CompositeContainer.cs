using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkushApp.ShellService.MVVM
{
    public static class CompositeContainer
    {
        public static T GetExportedInstance<T>(this CompositionContainer container)
        {
            try
            {
                var export = container.GetExport<T>();
                if (export != null)
                {
                    T instance = export.Value;
                    container.ReleaseExport(export);
                    return instance;
                }
                return default(T);
            }
            catch (Exception x)
            {
                Console.WriteLine(x.ToString());
                throw;
            }
        }
    }
}
