#define Config

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Needs.Utils.Converters;
using Needs.Utils.Convertibles;
using Needs.Settings.Models;
using Needs.Settings.Extends;
using Layer.Data.Sqls;

namespace Needs.Settings
{
    /// <summary>
    /// 相当一个类型一个接口
    /// </summary>
    public class SettingsManager<T> where T : ISettings
    {
        T source;
        Type currentType;

        SettingsManager()
        {
            Type type = typeof(T);
            if (type.IsInterface)
            {
                string name = type.Name.Substring(1);
                var templater = Type.GetType($"Needs.Settings.Models.{name},Needs.Settings", false);
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

            if (source == null)
            {
                source = (T)Activator.CreateInstance(this.currentType, true);
            }

            //this.Ensure();
            //this.Regular();
        }

        void Ensure()
        {
            using (var view = new SettingsView())
            using (var reponsitory = new BvOverallsReponsitory())
            {
                var items = view.Where(item => item.Type == currentType.FullName).ToArray();

                foreach (var property in this.currentType.GetProperties())
                {
                    string id = new[] { property.Name, this.currentType.FullName }.MD5();
                    var old = items.SingleOrDefault(item => item.ID == id);

                    if (old == null)
                    {
                        var newo = (T)Activator.CreateInstance(currentType, false);

                        Setting entity = new Setting
                        {
                            ID = id,
                            Type = this.currentType.FullName,
                            DataType = property.PropertyType.FullName,
                            Name = property.Name,
                            Value = (property.GetValue(newo) ?? property.PropertyType.GetDefaultValue())?.ToString(),
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now,
                            Summary = "",
                        };

                        var label = property.GetCustomAttribute<LabelAttribute>(true);

                        if (label != null)
                        {
                            entity.Summary = label.Summary;
                        }

                        reponsitory.Insert(entity.ToLinq());
                    }
                    else
                    {
                        var value = Convert.ChangeType(old.Value, property.PropertyType);
                        if (value != null)
                        {
                            property.SetValue(source, value);
                        }
                    }
                }
            }
        }

        void Regular()
        {
#if !DEBUG
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
                Name = "settings updates"
            };
            t.Start();
#endif
        }

        static SettingsManager<T> current;
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
                            current = new SettingsManager<T>();
                        }
                    }
                }
                return current.source;
            }
        }
    }
}
