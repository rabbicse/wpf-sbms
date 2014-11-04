using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkushApp.Logging
{
    public class LogConstants
    {
        public static string LOG_FORMAT = "${longdate}|${level:uppercase=true}|${message}${onexception:${newline}${exception:format=ToString}}";
        public static string ARCHIVE_FILE_DIRECTORY = @"\log";
        public static string ARCHIVE_FILE_NAME = @"\archive.{#}.";
        public static string ARCHIVE_DATE_FORMAT = "yyyy-MM-dd.HH.mm";
        public static long MAX_LOG_FILE_SIZE = 5 * 1024 * 1024; // 5 MB
        public static int MAX_NUM_ARCHIVE_FILE = 20; // 20 archive files inside archive folder

        private static string _commonAppPath;
        public static string CommonAppPath
        {
            get
            {
                if (!string.IsNullOrEmpty(_commonAppPath)) return _commonAppPath;

                return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
            set
            {
                _commonAppPath = value;
            }
        }
    }
}
