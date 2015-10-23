using EkushApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBMS.Infrastructure
{
    public static class Globals
    {
        public static class Assembly
        {
            public static string EXE_VERSION = "0.1.0";
        }
        public static class App
        {
            public static readonly string APPLICATION_EXE = @"\SBMS.exe";
        }
        public static class EmbededDB
        {
            public static readonly string DB_PATH = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\SBMS\sbms_db";
            public static readonly string DB_FILE_PATH = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\SBMS\sbms_fs";
        }
        public static class RegistryConstants
        {
            public static readonly string KEY_PATH = @"MOTO-ID\SBMS";
            public static readonly string LOCALE_KEY_NAME = "LOCALE";
            public static readonly string MONITOR_X = @"MONITOR_X";
            public static readonly string MONITOR_Y = @"MONITOR_Y";
        }
        public static class SearchKey
        {
            [Header("Department wise")]
            public const string DEPARTMENT_WISE = "DEPARTMENT_WISE";
            [Header("Category wise")]
            public const string CATEGORY_WISE = "CATEGORY_WISE";
            [Header("Title wise")]
            public const string TITLE_WISE = "TITLE_WISE";
            [Header("Date wise")]
            public const string DATE_WISE = "DATE_WISE";
            [Header("Recent All")]
            public const string RECENT_ALL = "RECENT_ALL";                        
        }
    }
}
