using Jing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace One
{
    /// <summary>
    /// 日志文件输出，在指定目录下，按照日期来输出日志
    /// </summary>
    class LogFile
    {
        /// <summary>
        /// 日志文件路径
        /// </summary>
        public string outputDir { get; private set; }

        /// <summary>
        /// 日志队列
        /// </summary>
        Queue<string> _logQueue;

        int _keepDays;

        public LogFile(string outputDir, int keepDays)
        {
            this.outputDir = outputDir;
            this._keepDays = keepDays;
            _logQueue = new Queue<string>();
            new Thread(OutputLogThread).Start();
        }

        public void OutputLog(string log)
        {
            lock (_logQueue)
            {
                _logQueue.Enqueue(log);
            }
        }

        void OutputLogThread()
        {
            if (Directory.Exists(outputDir))
            {
                //清理超过保留期限的日志
                var files = Directory.GetFiles(outputDir);
                foreach (var file in files)
                {
                    var tempFi = new FileInfo(file);
                    var tn = DateTime.Now - tempFi.CreationTime;
                    if (tn.TotalDays >= _keepDays)
                    {
                        tempFi.Delete();
                    }                    
                }
            }

            var logName = string.Format("log_{0}.txt",DateTime.Now.ToString("yyMMdd"));
            var path = FileUtility.CombinePaths(outputDir, logName);
            var fi = new FileInfo(path);
            if (false == fi.Directory.Exists)
            {
                fi.Directory.Create();
            }

            using (var fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                using (var sr = new StreamWriter(fs, Encoding.UTF8))
                {
                    sr.AutoFlush = true;
                    sr.WriteLine();
                    sr.WriteLine("----------------------------Start---------------------------------");
                    sr.WriteLine();
                    while (true)
                    {
                        string[] logs;
                        lock (_logQueue)
                        {
                            logs = _logQueue.ToArray();
                            _logQueue.Clear();
                        }

                        for (int i = 0; i < logs.Length; i++)
                        {
                            var data = Encoding.UTF8.GetBytes(logs[i]);
                            var encoding = sr.Encoding;
                            sr.WriteLine(logs[i]);
                        }
                        Thread.Sleep(100);
                    }
                }
            }
        }
    }
}
