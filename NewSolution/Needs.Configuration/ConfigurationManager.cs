using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Needs.Configuration
{
    public class ConfigurationManager<T> where T : IConfiguration
    {
        T source;
        Type currentType;

        ConfigurationManager()
        {
            Type type = typeof(T);
            if (type.IsInterface)
            {
                string name = type.Name.Substring(1);
                var templater = Type.GetType($"Needs.Configuration.Model.{name},Needs.Configuration", false);
                if (templater == null)
                {
                    throw new NotImplementedException($"The system does not implement the type:{type.FullName} of interface!");
                }
                this.currentType = templater;
            }
            else
            {
                this.currentType = type;
            }
            this.Ensure();
            this.Regular();
        }

        DateTime lastReadedTime;

        void Ensure()
        {
            string fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory
                , $"config.{this.currentType.Name.Replace("Config", null)}.json");

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
                    this.source = (T)Encoding.UTF8.GetString(bytes).JsonTo(this.currentType);
                }
            }
            else
            {
                var entity = this.source = (T)Activator.CreateInstance(this.currentType, true);
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
                Name = "configuration updates"
            };
            t.Start();
        }

        static ConfigurationManager<T> current;
        static object locker = new object();

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
                            current = new ConfigurationManager<T>();
                        }
                    }
                }
                return current.source;
            }
        }
    }
}
