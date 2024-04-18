//#define Config

//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using Needs.Utils.Converters;
//using Needs.Utils.Convertibles;

//namespace Needs.Settings
//{
//    /// <summary>
//    /// 相当一个类型一个接口
//    /// </summary>
//    public class SetteingManager<T> where T : ISettings
//    {
//        const string TableName = "SystemSettings";

//        T source;
//        Type currentType;

//        /// <summary>
//        /// 数据库名称
//        /// </summary>
//        public string DbName
//        {
//            get
//            {
//                return
//#if Config
//                    Configuration.ConfigurationManager<Configuration.ISystemConfig>.Current.SettingDbName ??
//#endif
//                    "SystemSettings";
//            }
//        }

//        SetteingManager()
//        {
//            Type type = typeof(T);
//            if (type.IsInterface)
//            {
//                string name = type.Name.Substring(1);
//                var templater = Type.GetType($"Needs.Settings.Model.{name},Needs.Settings", false);
//                if (templater == null)
//                {
//                    throw new NotImplementedException($"The system does not implement the type:{type.FullName} of interface!");
//                }
//                this.currentType = templater;
//            }
//            else
//            {
//                this.currentType = type;
//            }

//            if (source == null)
//            {
//                source = (T)Activator.CreateInstance(this.currentType, true);
//            }

//            this.Ensure();
//            this.Regular();
//        }

//        void Ensure()
//        {
//            Type rtype = Type.GetType($"Layer.Data.Sqls.{this.DbName}Reponsitory,Layer.Data", false);
//            if (rtype == null)
//            {
//                throw new NotImplementedException("The system does not implement the type:" + rtype.FullName);
//            }
//            Layer.Linq.IReponsitory reponsitory = Activator.CreateInstance(rtype) as Layer.Linq.IReponsitory;
//            if (reponsitory == null)
//            {
//                throw new NotImplementedException($"The type:{rtype.FullName} does not implement the interface:{typeof(Layer.Linq.IReponsitory).FullName}");
//            }

//            try
//            {
//                Data.Setter example = null;
//                var items = reponsitory.Query<Data.Setter>($"select * from {TableName} where {nameof(example.Type)} = {{0}}", this.currentType.FullName).ToArray();

//                foreach (var property in this.currentType.GetProperties())
//                {
//                    string id = new[] { property.Name, this.currentType.FullName }.MD5();
//                    var old = items.SingleOrDefault(item => item.ID == id);

//                    if (old == null)
//                    {
//                        dynamic entity = Activator.CreateInstance(Type.GetType($"Layer.Data.Sqls.{this.DbName}.{TableName},Layer.Data", false));
//                        entity.ID = id;
//                        entity.Type = this.currentType.FullName;
//                        entity.DataType = property.PropertyType.FullName;
//                        entity.Name = property.Name;
//                        entity.Value = property.PropertyType.GetDefaultValue().ToString();
//                        entity.CreateDate = DateTime.Now;
//                        entity.UpdateDate = DateTime.Now;
//                        entity.Summary = "please input !";

//                        var label = property.GetCustomAttribute<LabelAttribute>(true);

//                        if (label != null)
//                        {
//                            entity.Summary = label.Summary;
//                        }

//                        reponsitory.Insert(entity);
//                    }
//                    else
//                    {
//                        var value = Convert.ChangeType(old.Value, property.PropertyType);
//                        if (value != null)
//                        {
//                            property.SetValue(source, value);
//                        }
//                    }
//                }
//            }
//            finally
//            {
//                reponsitory.Dispose();
//            }
//        }

//        void Regular()
//        {
//            var t = new Thread(delegate ()
//            {
//                while (true)
//                {
//                    this.Ensure();
//                    Thread.Sleep(1000);
//                }
//            })
//            {
//                IsBackground = true,
//                Priority = ThreadPriority.Highest,
//                Name = "settings updates"
//            };
//            t.Start();
//        }

//        static SetteingManager<T> current;
//        static object locker = new object();

//        static public T Current
//        {
//            get
//            {
//                if (current == null)
//                {
//                    lock (locker)
//                    {
//                        if (current == null)
//                        {
//                            current = new SetteingManager<T>();
//                        }
//                    }
//                }
//                return current.source;
//            }
//        }
//    }
//}
