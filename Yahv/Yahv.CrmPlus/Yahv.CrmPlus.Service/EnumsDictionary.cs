using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Views.Origins;

namespace Yahv.CrmPlus.Service
{
    /// <summary>
    /// 字典单利
    /// </summary>
    /// <remarks>
    /// 当我们有真实字段化的字段使用如下方式从数据库中搜索
    /// </remarks>
    public class EnumsDictionary<T> : IEnumerable<DicItem>, IDisposable
    {
        DicItem[] data;
        Thread thread;
        Type currentType = typeof(T);


        /// <summary>
        /// 内置构造器
        /// </summary>
        EnumsDictionary()
        {
            Action edoLoader = new Action(() =>
            {
                using (var view = new EnumsDictionariesOrigin())
                {
                    this.data = view.Where(item => item.Enum == currentType.Name).Select(item => new DicItem
                    {
                        ID = item.ID,
                        Enum = item.Enum,
                        Description = item.Description,
                        Value = item.Value,
                        Field = item.Field,
                        IsFixed = item.IsFixed,
                    }).ToArray();
                }
            });
            edoLoader();
            thread = new Thread(() =>
            {
                try
                {
                    edoLoader();
                }
                catch (ThreadAbortException)
                {

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    Thread.Sleep(100);
                }
            });
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>枚举名称</returns>
        /// <remarks>商议一致通过，通用这里永久不考虑固定枚举</remarks>
        public string this[string index]
        {
            get
            {
                return this.Where(item => item.Enum == currentType.Name && (item.Field == index || item.ID == index))
                  .FirstOrDefault()?.Description ?? index;
            }
        }


        static object locker = new object();
        static EnumsDictionary<T> current;
        /// <summary>
        /// 单利实现
        /// </summary>
        static public EnumsDictionary<T> Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new EnumsDictionary<T>();
                        }
                    }
                }
                return current;
            }
        }

        ///// <summary>
        ///// 启动函数
        ///// </summary>
        //static public void Boot()
        //{
        //    Current.edoLoader();
        //}


        public IEnumerator<DicItem> GetEnumerator()
        {
            return this.data.Select(item => item).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void Dispose()
        {
            this.thread.Abort();
        }


    }

    /// <summary>
    /// 字典项
    /// </summary>
    public class DicItem
    {
        public string ID { get; internal set; }
        public string Enum { get; internal set; }
        public string Description { get; internal set; }
        public string Value { get; internal set; }
        public bool IsFixed { get; internal set; }
        public string Field { get; internal set; }
        static public T Parse<T>(string name) where T : struct
        {
            T t;
            if (System.Enum.TryParse(name, out t))
            {
                return t;
            }
            return default(T);
        }
    }


    class MyClass
    {
        public MyClass()
        {
            string name = "";

            var fa = DicItem.Parse<Underly.FixedArea>(name);
        }
    }

}