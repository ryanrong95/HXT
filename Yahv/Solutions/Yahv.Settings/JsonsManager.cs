using System;
using System.IO;
using System.Text;
using System.Threading;
using Yahv.Utils.Serializers;

namespace Yahv.Settings
{
    /// <summary>
    /// 数据库配置管理器
    /// </summary>
    /// <typeparam name="T">配置泛型</typeparam>
    public class JsonsManager<T> where T : JsonManager
    {
        T source;

        JsonsManager()
        {
            this.Ensure();
            this.Regular();
        }

        DateTime lastReadedTime;

        void Ensure()
        {
            Type type = typeof(T);

            var entity = (T)Activator.CreateInstance(type);
            string fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "JsonManagers", type.Name + ".json");
            FileInfo info = new FileInfo(fileName);
            if (info.Exists)
            {
                if (this.lastReadedTime >= info.LastWriteTime && this.source != null)
                {
                    return;
                }

                using (var stream = info.OpenRead())
                {
                    byte[] bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, bytes.Length);
                    this.source = (T)Encoding.UTF8.GetString(bytes).JsonTo(type);
                }
            }
            else
            {
                if (!info.Directory.Exists)
                {
                    info.Directory.Create();
                }

                this.source = entity;

                using (var stream = info.OpenWrite())
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(entity.Json(Formatting.Indented));
                    stream.Write(bytes, 0, bytes.Length);
                }
                info.Refresh();
            }
            this.lastReadedTime = info.LastWriteTime;
        }

        void Regular()
        {
            var t = new Thread(delegate ()
            {
                while (true)
                {
                    this.Ensure();
                    Thread.Sleep(1000);
                }
            })
            {
                IsBackground = true,
                Priority = ThreadPriority.Highest,
                Name = "jsonsmanager updates"
            };
            t.Start();
        }

        static JsonsManager<T> current;
        static object locker = new object();

        /// <summary>
        /// 全局实例
        /// </summary>
        static public T Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new JsonsManager<T>();
                        }
                    }
                }
                return current.source;
            }
        }
    }

}
