using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkushApp.Utility.WinRegistry
{
    public static class RegistryConstants
    {
        public static readonly string PATH = @"Software\";
        public static readonly string PATH_64 = @"Software\Wow6432Node\";
        public static readonly string KEY_BASE = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths";
    }
}
