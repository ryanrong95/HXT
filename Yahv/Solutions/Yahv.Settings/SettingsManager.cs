#define Config

using Yahv.Settings.Attributes;
using System;
using System.Linq;
using System.Reflection;
using Layers.Data.Sqls;
using Yahv.Utils.Converters.Contents;
using System.Configuration;
using System.Threading;

namespace Yahv.Settings
{
    /// <summary>
    /// 数据库配置管理器
    /// </summary>
    /// <typeparam name="TSource">配置泛型</typeparam>
    public class SettingsManager<TSource> where TSource : ISettings
    {
        TSource source;
        Type currentType;

        SettingsManager()
        {
            Type type = typeof(TSource);
            if (type.IsInterface)
            {
                string name = type.Name.Substring(1);
                var templater = Type.GetType($"Yahv.Settings.Models.{name},Yahv.Settings", false);
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
                source = (TSource)Activator.CreateInstance(this.currentType, true);
            }

            //可以做出以数据库为准的开发，
            //也可以做以程序为准的开发
            //<add key="SettingsManager" value="local,sql"/>

            string configvalue = ConfigurationManager.AppSettings["SettingsManager"];
            if (string.IsNullOrWhiteSpace(configvalue)
                || configvalue.Equals("local", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            this.Ensure();
            this.Regular();
        }

        void Ensure()
        {
            lock (this)
            {
                using (var reponsitory = new OverallsReponsitory())
                {
                    var items = reponsitory.ReadTable<Layers.Data.Sqls.Overalls.Settings>()
                        .Where(item => item.Type == this.currentType.FullName)
                        .ToArray();

                    foreach (var property in this.currentType.GetProperties())
                    {
                        string id = new[] { property.Name, this.currentType.FullName }.MD5();

                        var old = items.SingleOrDefault(item => item.ID == id);

                        if (old == null)
                        {
                            #region 写入数据库

                            var newo = (TSource)Activator.CreateInstance(currentType, false);

                            var entity = new Layers.Data.Sqls.Overalls.Settings
                            {
                                ID = id,
                                Type = this.currentType.FullName,
                                DataType = property.PropertyType.FullName,
                                Name = property.Name,
                                Value = property.GetValue(newo)?.ToString(),
                                CreateDate = DateTime.Now,
                                UpdateDate = DateTime.Now,
                                Summary = "",
                            };

                            var label = property.GetCustomAttribute<LabelAttribute>(true);

                            if (label != null)
                            {
                                entity.Summary = label.Summary;
                            }

                            reponsitory.Insert(entity);

                            #endregion
                        }
                        else
                        {
                            property.SetValue(this.source, Convert.ChangeType(old.Value, Type.GetType(old.Type)));
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

        static SettingsManager<TSource> current;
        static object locker = new object();

        /// <summary>
        /// 全局实例
        /// </summary>
        static public TSource Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new SettingsManager<TSource>();
                        }
                    }
                }
                return current.source;
            }
        }
    }
}
