using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ExamTester
{
      /// <summary>
      /// IO helper
      /// </summary>
    public static class IoHelper
    {
        /// <summary>
        /// Read byte stream from file
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <returns>byte stream</returns>
        public static byte[] GetBytesByFilePath(string filePath)
        {
            byte[] rtn = null;

            try
            {
                rtn = File.ReadAllBytes(filePath);
            }
            catch (System.Exception ex)
            {
                // Fail
                LogHelper.Error(ex);

                return null;
            }

            return rtn;
        }

        /// <summary>
        /// Save byte stream to file
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <param name="content">File content</param>
        /// <returns>Whether success</returns>
        public static bool SaveBytesToFilePath(string filePath, byte[] content)
        {
            try
            {
                CreateDirectoryByFilePath(filePath);
                File.WriteAllBytes(filePath, content);
            }
            catch (System.Exception ex)
            {
                // Fail
                LogHelper.Error(ex);

                return false;
            }

            return true;
        }

        /// <summary>
        /// Save string to file
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <param name="content">File content</param>
        /// <returns>Whether success</returns>
        public static bool SaveStringToFilePath(string filePath, string content)
        {
            return SaveStringToFilePath(filePath, content, Encoding.Default);
        }

        /// <summary>
        /// Save string to file
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <param name="content">File content</param>
        /// <param name="encoding">Encoding</param>
        /// <returns>Whether success</returns>
        public static bool SaveStringToFilePath(string filePath, string content, Encoding encoding)
        {
            try
            {
                CreateDirectoryByFilePath(filePath);
                File.WriteAllText(filePath, content);
            }
            catch (System.Exception ex)
            {
                // Fail
                LogHelper.Error(ex);

                return false;
            }

            return true;
        }

        /// <summary>
        /// Append content to file
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <param name="content">File content (one line)</param>
        /// <returns>Whether success</returns>
        public static bool AppendStringToFilePath(string filePath, string content)
        {
            return AppendStringToFilePath(filePath, content, Encoding.Default);
        }

        /// <summary>
        /// Append content to file
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <param name="content">File content (one line)</param>
        /// <param name="encoding">Encoding</param>
        /// <returns>Whether success</returns>
        public static bool AppendStringToFilePath(string filePath, string content, Encoding encoding)
        {
            try
            {
                CreateDirectoryByFilePath(filePath);
                File.AppendAllText(filePath, content, encoding);
            }
            catch (System.Exception ex)
            {
                // Fail
                LogHelper.Error(ex);

                return false;
            }

            return true;
        }

        /// <summary>
        /// Append content line to file
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <param name="content">File content</param>
        /// <returns>Whether success</returns>
        public static bool AppendStringLineToFilePath(string filePath, string content)
        {
            return AppendStringLineToFilePath(filePath, content, Encoding.Default);
        }
        /// <summary>
        /// Append content line to file
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <param name="content">File content</param>
        /// <param name="encoding">Encoding</param>
        /// <returns>Whether success</returns>
        public static bool AppendStringLineToFilePath(string filePath, string content, Encoding encoding)
        {
            try
            {
                CreateDirectoryByFilePath(filePath);
                File.AppendAllLines(filePath, new List<string>() { content }, encoding);
            }
            catch (System.Exception ex)
            {
                // Fail
                LogHelper.Error(ex);

                return false;
            }

            return true;
        }

        /// <summary>
        /// Copy the file
        /// </summary>
        /// <param name="fromFilePath">Original path</param>
        /// <param name="toFilePath">Destination path</param>
        /// <returns>Whether success</returns>
        public static bool CopyFile(string fromFilePath, string toFilePath)
        {
            try
            {
                CreateDirectoryByFilePath(toFilePath);
                if (File.Exists(toFilePath))
                {
                    File.Delete(toFilePath);
                }

                // 判断文件是否已经被占用
                if (IoHelper.WaitReady(fromFilePath))
                {
                    File.Copy(fromFilePath, toFilePath);
                }
            }
            catch (System.Exception ex)
            {
                LogHelper.Error(ex);

                return false;
            }

            return true;
        }

        /// <summary>
        /// Move the file
        /// </summary>
        /// <param name="fromFilePath">Original path</param>
        /// <param name="toFilePath">Destination path</param>
        /// <returns>Whether success</returns>
        public static bool MoveFile(string fromFilePath, string toFilePath)
        {
            try
            {
                CreateDirectoryByFilePath(toFilePath);
                if (File.Exists(toFilePath))
                {
                    File.Delete(toFilePath);
                }
                File.Move(fromFilePath, toFilePath);
            }
            catch (System.Exception ex)
            {
                LogHelper.Error(ex);

                return false;
            }

            return true;
        }

        /// <summary>
        /// Delete folder
        /// </summary>
        /// <param name="filePath">File path</param>
        public static bool DeleteFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (System.Exception ex)
            {
                LogHelper.Error(ex);

                return false;
            }

            return true;
        }

        /// <summary>
        /// Create the folder when not exist
        /// </summary>
        /// <param name="directoryPath">Folder path</param>
        /// <returns></returns>
        public static bool CreateDirectory(string directoryPath)
        {
            try
            {
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
            }
            catch (System.Exception ex)
            {
                LogHelper.Error(ex);

                return false;
            }

            return true;
        }

        /// <summary>
        /// Create the folder when not exist
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <returns></returns>
        public static bool CreateDirectoryByFilePath(string filePath)
        {
            return CreateDirectory(Path.GetDirectoryName(filePath));
        }

        /// <summary>
        /// Get the folder name (not include the path)
        /// </summary>
        /// <param name="directoryPath">Folder path</param>
        /// <returns></returns>
        public static string GetDirectoryName(string directoryPath)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(directoryPath);

                return di.Name.ToSafeString();
            }
            catch (System.Exception ex)
            {
                LogHelper.Error(ex);
            }

            return string.Empty;
        }

        /// <summary>
        /// Get the folder name (not include the path)
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <returns></returns>
        public static string GetDirectoryNameByFilePath(string filePath)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(filePath));

                return di.Name.ToSafeString();
            }
            catch (System.Exception ex)
            {
                LogHelper.Error(ex);
            }

            return string.Empty;
        }

        /// <summary>
        /// Delete the folder
        /// </summary>
        /// <param name="directoryPath">Folder path</param>
        public static bool DeleteDirectory(string directoryPath)
        {
            try
            {
                if (Directory.Exists(directoryPath))
                {
                    Directory.Delete(directoryPath, true);
                }
            }
            catch (System.Exception ex)
            {
                LogHelper.Error(ex);

                return false;
            }

            return true;
        }
        /// <summary>
        /// 有限时间内等待其他进程释放文件
        /// </summary>
        public static bool WaitReady(string fileName)
        {
            while (true)
            {
                try
                {
                    using (Stream stream = System.IO.File.Open(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        if (stream != null)
                        {
                            LogHelper.Info(string.Format("文件 {0} 准备OK.", fileName));
                            //System.Diagnostics.Trace.WriteLine(string.Format("Output file {0} ready.", fileName));
                            return true;
                        }
                    }
                }
                catch (FileNotFoundException ex)
                {
                    LogHelper.Info("文件 {0} 找不到 ({1})", fileName, ex.Message);
                    //System.Diagnostics.Trace.WriteLine(string.Format("Output file {0} not yet ready ({1})", fileName, ex.Message));
                }
                catch (IOException ex)
                {
                    LogHelper.Info("文件 {0} 被占用 ({1})", fileName, ex.Message);
                    //System.Diagnostics.Trace.WriteLine(string.Format("Output file {0} not yet ready ({1})", fileName, ex.Message));
                }
                catch (System.UnauthorizedAccessException ex)
                {
                    LogHelper.Info("文件 {0} 无权限 ({1})", fileName, ex.Message);
                    FileInfo file = new FileInfo(fileName);
                    if (file.IsReadOnly)
                    {
                        LogHelper.Info("文件{0}是只读文件.", fileName);
                        LogHelper.Info("正在尝试取消文件只读设置....");
                        System.IO.File.SetAttributes(fileName, System.IO.FileAttributes.Normal);
                        LogHelper.Info("取消文件只读设置.完成.");
                    }
                }
                System.Threading.Thread.Sleep(500);
            }
        }
        /// <summary>
        /// 有限时间内等待其他进程释放文件
        /// </summary>
        public static bool WaitReady(string fileName, int iCount)
        {
            while (iCount > 0)
            {
                try
                {
                    using (Stream stream = System.IO.File.Open(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        if (stream != null)
                        {
                            LogHelper.Info(string.Format("Output file {0} ready.", fileName));
                            //System.Diagnostics.Trace.WriteLine(string.Format("Output file {0} ready.", fileName));
                            return true;
                        }
                    }
                }
                catch (FileNotFoundException ex)
                {
                    LogHelper.Info("Output file {0} not yet ready ({1})", fileName, ex.Message);
                    //System.Diagnostics.Trace.WriteLine(string.Format("Output file {0} not yet ready ({1})", fileName, ex.Message));
                }
                catch (IOException ex)
                {
                    LogHelper.Info("Output file {0} not yet ready ({1})", fileName, ex.Message);
                    //System.Diagnostics.Trace.WriteLine(string.Format("Output file {0} not yet ready ({1})", fileName, ex.Message));
                }
                catch (System.UnauthorizedAccessException ex)
                {
                    LogHelper.Info("Output file {0} not yet ready ({1})", fileName, ex.Message);
                    FileInfo file = new FileInfo(fileName);
                    if (file.IsReadOnly)
                    {
                        LogHelper.Info("文件{0}是只读文件.", fileName);
                        LogHelper.Info("正在尝试取消文件只读设置....");
                        System.IO.File.SetAttributes(fileName, System.IO.FileAttributes.Normal);
                        LogHelper.Info("取消文件只读设置.完成.");
                    }
                }
                System.Threading.Thread.Sleep(1000);
                iCount--;
            }
            return false;
        }
    }
}
