using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace TradeView
{
    public class Logger
    {
        private readonly string logFilePath;
        private readonly object lockObj = new object();

        public Logger(string logFilePath)
        {
            this.logFilePath = logFilePath;
            EnsureLogFileExists();
        }
        private void EnsureLogFileExists()
        {
            // 检查日志文件是否存在，如果不存在则创建
            if (!File.Exists(logFilePath))
            {
                // 使用空内容创建文件，或者您可以添加初始化的日志头等
                File.Create(logFilePath).Close();
            }
        }

        public void LogError(Exception ex)
        {
            Log("ERROR", ex.ToString());
        }

        public void LogInfo(string message)
        {
            Log("INFO", message);
        }

        private void Log(string level, string message)
        {
            lock (lockObj) // 确保线程安全
            {
                string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}";
                File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
            }
        }
    }
}
