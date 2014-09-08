using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkushApp.Logging
{
    public sealed class LogHandler
    {        
        public static void SetupLogging(string filename, LogLevel level)
        {
            try
            {
                string archivePath = LogConstants.CommonAppPath + Path.GetDirectoryName(filename) + LogConstants.ARCHIVE_FILE_DIRECTORY;

                SetupLogging(filename, level, LogConstants.CommonAppPath, archivePath);
            }
            catch (Exception x)
            {
                Console.WriteLine(x);
            }
        }

        private static void SetupLogging(string filename, LogLevel level, string path, string archivePath)
        {
            try
            {
                LoggingConfiguration config = new LoggingConfiguration();

                // Configuration for application
                FileTarget fileTarget = new FileTarget();
                fileTarget.FileName = path + @"\" + filename;
                fileTarget.Layout = LogConstants.LOG_FORMAT;
                fileTarget.Encoding = Encoding.UTF8;
                fileTarget.ArchiveFileName = archivePath + LogConstants.ARCHIVE_FILE_NAME + new DirectoryInfo(filename).Name;
                fileTarget.ArchiveEvery = FileArchivePeriod.Day;
                fileTarget.ArchiveNumbering = ArchiveNumberingMode.Date;
                fileTarget.ArchiveDateFormat = LogConstants.ARCHIVE_DATE_FORMAT;
                fileTarget.ArchiveAboveSize = LogConstants.MAX_LOG_FILE_SIZE;
                fileTarget.MaxArchiveFiles = LogConstants.MAX_NUM_ARCHIVE_FILE;
                fileTarget.ConcurrentWrites = true;
                fileTarget.ConcurrentWriteAttempts = 5;
                fileTarget.ConcurrentWriteAttemptDelay = 5;
                fileTarget.CreateDirs = true;
                fileTarget.EnableFileDelete = true;
                fileTarget.DeleteOldFileOnStartup = true;
                config.AddTarget("file", fileTarget);

                // Configuration for ravendb
                FileTarget fileTargetRaven = new FileTarget();
                fileTargetRaven.FileName = path + Path.GetDirectoryName(filename) + @"\" + "raven_" + new DirectoryInfo(filename).Name;
                fileTargetRaven.Layout = LogConstants.LOG_FORMAT;
                fileTargetRaven.ArchiveFileName = archivePath + LogConstants.ARCHIVE_FILE_NAME + "raven_" + new DirectoryInfo(filename).Name;
                fileTargetRaven.ArchiveEvery = FileArchivePeriod.Day;
                fileTargetRaven.ArchiveNumbering = ArchiveNumberingMode.Date;
                fileTargetRaven.ArchiveDateFormat = LogConstants.ARCHIVE_DATE_FORMAT;
                fileTargetRaven.ArchiveAboveSize = LogConstants.MAX_LOG_FILE_SIZE;
                fileTargetRaven.MaxArchiveFiles = LogConstants.MAX_NUM_ARCHIVE_FILE;
                fileTargetRaven.ConcurrentWrites = true;
                fileTargetRaven.ConcurrentWriteAttempts = 5;
                fileTargetRaven.ConcurrentWriteAttemptDelay = 5;
                fileTargetRaven.CreateDirs = true;
                fileTargetRaven.EnableFileDelete = true;
                fileTargetRaven.DeleteOldFileOnStartup = true;
                config.AddTarget("fileRaven", fileTargetRaven);

                // Logging rule for ravendb
                LoggingRule ruleRaven = new LoggingRule("Raven.*", NLogLevel(LogLevel.Debug), fileTargetRaven);
                ruleRaven.Final = true;

                // Logging rule for application
                LoggingRule rule = new LoggingRule("*", NLogLevel(level), fileTarget);
                config.LoggingRules.Add(ruleRaven);
                config.LoggingRules.Add(rule);

                LogManager.Configuration = config;
            }
            catch (Exception x)
            {
                throw x;
            }
        }
        private static NLog.LogLevel NLogLevel(LogLevel logLevel = LogLevel.Debug)
        {
            switch (logLevel)
            {
                case LogLevel.Debug:
                    return NLog.LogLevel.Debug;
                case LogLevel.Info:
                    return NLog.LogLevel.Info;
                case LogLevel.Warn:
                    return NLog.LogLevel.Warn;
                case LogLevel.Error:
                    return NLog.LogLevel.Error;
                case LogLevel.Fatal:
                    return NLog.LogLevel.Fatal;
                case LogLevel.Trace:
                default:
                    return NLog.LogLevel.Trace;
            }
        }
    }
}
