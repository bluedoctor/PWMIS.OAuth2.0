using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PWMIS.OAuth2.Tools
{
    /// <summary>
    /// 系统日志消息类，可以设置 WriteFile 委托方法以便使用你自己的日志写入对象
    /// </summary>
    public class Logger
    {
        private static object lock_obj = new object();
        private static Logger _instance=null;
        private BlockingCollection<string> bc = null;
        private int bufferCount = 0;
        private int checkSizeCount = 0;

        private Logger() {
            bc = new BlockingCollection<string>(100);
            WriteFile = WriteLogFile;
            StartConsuming();
        }

        private void StartConsuming()
        {
            Task.Run(() =>
            {
                foreach (var str in bc.GetConsumingEnumerable())
                {
                    WriteFile(str);
                    System.Threading.Interlocked.Decrement(ref bufferCount);
                }
                //上面代码会一直等待有日志消息，参考 https://blog.csdn.net/tigerzx/article/details/61917194
                WriteFile("完成日志信息写入");
            });
        }

        private void WriteLogFile(string logTxt)
        {
            string fileName = string.Format("ProxyLog_{0}.txt", DateTime.Now.ToString("yyyy-MM-dd"));
            string filePath = System.IO.Path.Combine(this.LogFilePath, fileName);

            try
            {
                //超过5M 文件改名备份
                if (checkSizeCount > 10)
                {
                    var fileInfo = new FileInfo(filePath);
                    if (fileInfo.Length > 1 * 1024 * 1024)
                    {
                        string bakFile = string.Format("{0}_{1}.{2}",
                           System.IO.Path.GetFileNameWithoutExtension(filePath),
                           DateTime.Now.ToString("HHmmss"),
                           System.IO.Path.GetExtension(filePath));
                        string bakFilePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(filePath), bakFile);
                        System.IO.File.Move(filePath, bakFilePath);
                    }
                    checkSizeCount=0;
                }
                checkSizeCount++;

                string text = string.Format("[Buff{0}] {1}", bufferCount, logTxt);
                System.IO.File.AppendAllText(filePath, text);
            }
            catch
            {

            }
        }

        private string _LogFilePath;

        /// <summary>
        /// 获取或者设置日志文件路径
        /// </summary>
        public string LogFilePath {
            get { return _LogFilePath; }
            set {
                _LogFilePath = value;
                if (!System.IO.Directory.Exists(LogFilePath))
                    System.IO.Directory.CreateDirectory(LogFilePath);
            }
        }
        /// <summary>
        /// 获取或者设置写日志的委托方法，默认直接调用系统的 File对象写文本文件
        /// </summary>
        public Action<string> WriteFile { get; set; }
        /// <summary>
        /// 获取唯一实例
        /// </summary>
        public static Logger Instance
        {
            get {
                if (_instance == null)
                {
                    lock (lock_obj)
                    {
                        if (_instance == null)
                        {
                            _instance = new Logger();
                        }
                    }
                }
                return _instance;
            }
        }
        /// <summary>
        /// 写入日志到缓冲区
        /// </summary>
        /// <param name="logText"></param>
        public void Write(string logText)
        {
            bc.Add(logText);
            System.Threading.Interlocked.Increment(ref bufferCount);
        }
        /// <summary>
        /// 写入一行日志到缓冲区
        /// </summary>
        /// <param name="logText"></param>
        public void WriteLine(string logText)
        {
            bc.Add(logText + "\r\n");
            System.Threading.Interlocked.Increment(ref bufferCount);
        }


    }
}
