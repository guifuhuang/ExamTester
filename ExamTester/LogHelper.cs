using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace ExamTester
{
    /// <summary>
    /// Log helper
    /// </summary>
    public static class LogHelper
    {
        #region Property
        /// <summary>
        /// For lock use
        /// </summary>
        private static readonly string onlyForLockUse = string.Empty;
        /// <summary>
        /// Name of log file
        /// </summary>
        private static string logFileName
        {
            get
            {
                return "{0}.log";
            }
        }
        /// <summary>
        /// Line content template
        /// {0}：Log time (replaced later)
        /// {1}：Log content (replaced later)
        /// </summary>
        private static string contentFormat
        {
            get
            {
                return "{0} {1}";
            }
        }
        /// <summary>
        /// Time format
        /// </summary>
        private static string timeStampFormat
        {
            get
            {
                return "yyyy-MM-dd HH:mm:ss.fff";
            }
        }
        #endregion

        #region Method
        /// <summary>
        /// Write info log
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="args">arguments</param>
        public static void Info(string content, params object[] args)
        {
            DateTime dt = DateTime.Now;

            lock (onlyForLockUse)
            {
                string appendContent = content.FormatString(args);

                Debug.WriteLine(GetContent(dt, appendContent));

                IoHelper.AppendStringLineToFilePath(GetFilePath(dt), GetContent(dt, appendContent));
            }
        }

        /// <summary>
        /// Write error log
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="args">arguments</param>
        public static void Error(string content, params object[] args)
        {
            DateTime dt = DateTime.Now;

            lock (onlyForLockUse)
            {
                string appendContent = content.FormatString(args);

                Debug.WriteLine(GetContent(dt, appendContent));

                IoHelper.AppendStringLineToFilePath(GetFilePath(dt), GetContent(dt, appendContent));
            }
        }

        /// <summary>
        /// Write error log
        /// </summary>
        /// <param name="content">content</param>
        public static void Error(System.Exception content)
        {
            DateTime dt = DateTime.Now;

            lock (onlyForLockUse)
            {
                string appendContent = content.ToSafeString();

                Debug.WriteLine(GetContent(dt, appendContent));

                IoHelper.AppendStringLineToFilePath(GetFilePath(dt), GetContent(dt, appendContent));
            }
        }

        /// <summary>
        /// Get the log file path
        /// </summary>
        /// <param name="dt">Log time</param>
        /// <returns></returns>
        private static string GetFilePath(DateTime dt)
        {
            string folder = string.Empty;
            try
            {
                folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");
            }
            catch
            {
                folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");
            }

            return Path.Combine(folder, dt.ToString("yyyy"), dt.ToString("MM"), logFileName.FormatString(dt.ToString("yyyy-MM-dd")));
        }

        /// <summary>
        /// Generate log content (splited by "\r\n")
        /// </summary>
        /// <param name="dt">Log time</param>
        /// <param name="content">content</param>
        /// <returns></returns>
        private static string GetContent(DateTime dt, string content)
        {
            return contentFormat.FormatString(dt.ToString(timeStampFormat), content);
        }
        #endregion

        #region Debug
        /// <summary>
        /// Write debug log
        /// </summary>
        /// <param name="content">content</param>
        /// <param name="args">arguments</param>
        public static void DebugInfo(string content, params object[] args)
        {
            Info(content, args);
        }
        #endregion Debug
    }
}
