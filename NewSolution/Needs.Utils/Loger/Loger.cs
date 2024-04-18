using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Needs.Utils
{
    class Loger
    {
        ConcurrentQueue<string> queue;
        ConcurrentQueue<string> config;
        string path;
        string configLogPath;
        Loger()
        {
            this.queue = new ConcurrentQueue<string>();
            this.config = new ConcurrentQueue<string>();
            this.path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "logs", "log.txt");
            this.configLogPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "logs", "configLog.txt");
            if (!new FileInfo(path).Directory.Exists)
            {
                new FileInfo(path).Directory.Create();
            }
            if (!new FileInfo(this.configLogPath).Directory.Exists)
            {
                new FileInfo(configLogPath).Directory.Create();
            }
            new Thread(Write).Start();
        }
        void Write()
        {
            while (true)
            {
                string item;
                if (queue.TryDequeue(out item))
                {
                    try
                    {
                        using (var stream = System.IO.File.AppendText(this.path))
                        {
                            stream.WriteLine(item);
                        }
                    }
                    catch (Exception e)
                    {
                        queue.Enqueue("异常:" + e.Message + item);
                    }
                }
                else if (this.config.TryDequeue(out item))
                {
                    try
                    {
                        using (var fileStream = System.IO.File.Open(this.configLogPath, FileMode.OpenOrCreate))
                        {
                            var arry = new byte[1024];
                            System.Text.StringBuilder txt = new StringBuilder();
                            int index = 0;
                            do
                            {
                                index = fileStream.Read(arry, index, arry.Length);
                                txt.Append(System.Text.Encoding.Default.GetString(arry).Trim(' '));
                            } while (fileStream.Length > index);
                            if (!txt.ToString().Contains(item))
                            {
                                foreach (var c in System.Text.Encoding.Default.GetBytes(item))
                                {
                                    fileStream.WriteByte(c);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        this.config.Enqueue("异常:" + e.Message + item);
                    }
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
        }

        public void Add(string text)
        {
            this.queue.Enqueue(text);
        }
        public void AddConfig(string text)
        {
            this.config.Enqueue(text);
        }
        static Loger instance;
        static object locker = new object();
        public static Loger Instance()
        {
            if (instance == null)
            {
                lock (locker)
                {
                    if (instance == null)
                    {
                        instance = new Loger();
                    }
                }
            }
            return instance;
        }
    }
}
