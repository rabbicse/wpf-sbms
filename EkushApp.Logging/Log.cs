using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EkushApp.Logging
{
    public sealed class Log
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        ~Log() { }

        #region Method(s)
        public static void Debug(string message, [CallerMemberName] string callerName = "")
        {
            var stackFrame = new StackFrame(1, false);
            var msg = stackFrame.GetMethod().DeclaringType + ".<" + callerName + ">|" + message;
            logger.Debug(msg);
        }
        public static void Debug(string message, Exception x, [CallerMemberName] string callerName = "")
        {
            var stackFrame = new StackFrame(1, false);
            var msg = stackFrame.GetMethod().DeclaringType + ".<" + callerName + ">|" + message;
            logger.DebugException(msg, x);
        }
        public static void Info(string message, [CallerMemberName] string callerName = "")
        {
            var stackFrame = new StackFrame(1, false);
            var msg = stackFrame.GetMethod().DeclaringType + ".<" + callerName + ">|" + message;
            logger.Info(msg);
        }
        public static void Info(string message, Exception x, [CallerMemberName] string callerName = "")
        {
            var stackFrame = new StackFrame(1, false);
            var msg = stackFrame.GetMethod().DeclaringType + ".<" + callerName + ">|" + message;
            logger.InfoException(msg, x);
        }
        public static void Warn(string message, [CallerMemberName] string callerName = "")
        {
            var stackFrame = new StackFrame(1, false);
            var msg = stackFrame.GetMethod().DeclaringType + ".<" + callerName + ">|" + message;
            logger.Warn(msg);
        }
        public static void Warn(string message, Exception x, [CallerMemberName] string callerName = "")
        {
            var stackFrame = new StackFrame(1, false);
            var msg = stackFrame.GetMethod().DeclaringType + ".<" + callerName + ">|" + message;
            logger.WarnException(msg, x);
        }
        public static void Error(string message, [CallerMemberName] string callerName = "")
        {
            var stackFrame = new StackFrame(1, false);
            var msg = stackFrame.GetMethod().DeclaringType + ".<" + callerName + ">|" + message;
            logger.Error(msg);
        }
        public static void Error(string message, Exception x, [CallerMemberName] string callerName = "")
        {
            var stackFrame = new StackFrame(1, false);
            var msg = stackFrame.GetMethod().DeclaringType + ".<" + callerName + ">|" + message;
            logger.ErrorException(msg, x);
        }
        public static void Fatal(string message, [CallerMemberName] string callerName = "")
        {
            var stackFrame = new StackFrame(1, false);
            var msg = stackFrame.GetMethod().DeclaringType + ".<" + callerName + ">|" + message;
            logger.Fatal(msg);
        }
        public static void Fatal(string message, Exception x, [CallerMemberName] string callerName = "")
        {
            var stackFrame = new StackFrame(1, false);
            var msg = stackFrame.GetMethod().DeclaringType + ".<" + callerName + ">|" + message;
            logger.FatalException(msg, x);
        }
        public static void Trace(string message, [CallerMemberName] string callerName = "")
        {
            var stackFrame = new StackFrame(1, false);
            var msg = stackFrame.GetMethod().DeclaringType + ".<" + callerName + ">|" + message;
            logger.Trace(msg);
        }
        public static void Trace(string message, Exception x, [CallerMemberName] string callerName = "")
        {
            var stackFrame = new StackFrame(1, false);
            var msg = stackFrame.GetMethod().DeclaringType + ".<" + callerName + ">|" + message;
            logger.TraceException(msg, x);
        }
        #endregion
    }
}
