using Layers.Data.Sqls;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yahv.Utils.Serializers;

namespace YaHv.PvData.Services
{
    public class CacherInitEventArgs : EventArgs
    {
        public IEnumerable Data { get; internal set; }
    }

    public class Cachers<T>
    {
        /// <summary>
        /// 过期验证时间
        /// </summary>
        public virtual int vTimeout { get; set; } = 100;

        ConcurrentDictionary<string, T> data;

        static public event EventHandler<CacherInitEventArgs> Init;
        static public event EventHandler<CacherInitEventArgs> Timeout;

        public static void RegisterInit(EventHandler<CacherInitEventArgs> init)
        {
            if (Init == null)
                Init += init;
        }

        public static void RegisterTimeout(EventHandler<CacherInitEventArgs> timeout)
        {
            if (Timeout == null)
                Timeout += timeout;
        }

        protected Cachers()
        {
            Thread thread = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        if (this.data != null && this.data.Count > 0)
                        {
                            var e = new CacherInitEventArgs() { Data = this.data };
                            Timeout?.Invoke(this, e);

                            var collection = e.Data.Cast<KeyValuePair<string, T>>();
                            current.data = new ConcurrentDictionary<string, T>(collection);
                        }
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
                        Thread.Sleep(this.vTimeout);
                    }
                }
            })
            {
                Name = $"{nameof(Cachers<T>)}_{typeof(T)}",
                IsBackground = true,
                Priority = ThreadPriority.Highest
            };

            thread.Start();
        }

        static object locker = new object();
        static Cachers<T> current;

        static public Cachers<T> Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new Cachers<T>();

                            if (Init != null)
                            {
                                var e = new CacherInitEventArgs();
                                Init.Invoke(current, e);

                                var collection = e.Data.Cast<KeyValuePair<string, T>>();
                                current.data = new ConcurrentDictionary<string, T>(collection);
                            }
                        }
                    }
                }

                return current;
            }
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T this[string key]
        {
            get
            {
                return this.Get(key);
            }
        }

        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get(string key)
        {
            T cache;
            this.data.TryGetValue(key, out cache);

            return cache;
        }

        /// <summary>
        /// 添加缓存数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Add(string key, T value)
        {
            return this.data.TryAdd(key, value);
        }
    }

    public class MyCachers
    {
        /// <summary>
        /// 大赢家冷偏型号缓存
        /// </summary>
        public Cachers<DyjUnpopularCache> DyjUnpopularCaches
        {
            get
            {
                Cachers<DyjUnpopularCache>.Current.vTimeout = 60 * 60 * 1000;
                return Cachers<DyjUnpopularCache>.Current;
            }
        }

        /// <summary>
        /// 标准型号缓存
        /// </summary>
        public Cachers<StandardPartnumberCache> StandardPartnumberCaches
        {
            get
            {
                Cachers<StandardPartnumberCache>.Current.vTimeout = 5000;
                return Cachers<StandardPartnumberCache>.Current;
            }
        }

        public MyCachers()
        {
            Cachers<DyjUnpopularCache>.RegisterInit(DyjUnpopularCaches_Init);
            Cachers<DyjUnpopularCache>.RegisterTimeout(DyjUnpopularCaches_Timeout);

            Cachers<StandardPartnumberCache>.RegisterInit(StandardPartnumberCaches_Init);
            Cachers<StandardPartnumberCache>.RegisterTimeout(StandardPartnumberCaches_Timeout);
        }

        private void DyjUnpopularCaches_Timeout(object sender, CacherInitEventArgs e)
        {
            //读取大赢家的数据写入本地文件？
            using (var conn = new PvDataReponsitory().CreateConnection())
            {
                SqlCommand sqlcmd = conn.CreateCommand();
                sqlcmd.CommandText = "select distinct(型号) as partnumber from dyj.ErpV4_TongJi.dbo.V_LP order by 型号;";

                HashSet<string> hset = new HashSet<string>();
                //从数据库查询数据
                using (var reader = sqlcmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        hset.Add(reader.GetFieldValue<string>(0));
                    }
                }

                var file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data",
                    "dyj.unpopular.partnumbers.txt");
                File.SetAttributes(file, FileAttributes.Normal);
                File.WriteAllLines(file, hset.ToArray());
            }

            DyjUnpopularCaches_Init(sender, e);
        }

        private void DyjUnpopularCaches_Init(object sender, CacherInitEventArgs e)
        {
            FileInfo fInfo = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "dyj.unpopular.partnumbers.txt"));
            using (var file = fInfo.OpenText())
            {
                var dic = new ConcurrentDictionary<string, DyjUnpopularCache>();
                while (true)
                {
                    string pn = file.ReadLine();
                    if (pn == null)
                        break;

                    pn = pn.Trim();
                    if (pn.Length < 2)
                        continue;

                    string key = pn.Substring(0, 2);
                    if (dic.ContainsKey(key))
                    {
                        var cache = dic[key];
                        cache.Partnumbers.Add(pn);
                    }
                    else
                    {
                        var cache = new DyjUnpopularCache
                        {
                            Key = key,
                            Partnumbers = new List<string> { pn }
                        };
                        dic.TryAdd(key, cache);
                    }
                }

                e.Data = dic;
            }
        }

        private void StandardPartnumberCaches_Timeout(object sender, CacherInitEventArgs e)
        {
            //清除过期的缓存数据
            var collection = e.Data.Cast<KeyValuePair<string, StandardPartnumberCache>>();
            var dic = new ConcurrentDictionary<string, StandardPartnumberCache>(collection);
            var keys = dic.Keys;

            foreach (var key in keys)
            {
                if (dic.ContainsKey(key))
                {
                    StandardPartnumberCache cache;
                    dic.TryGetValue(key, out cache);
                    if (cache != null && DateTime.Now > cache.ExpireDate)
                    {
                        dic.TryRemove(key, out cache);
                    }
                }
            }

            e.Data = dic;
        }

        private void StandardPartnumberCaches_Init(object sender, CacherInitEventArgs e)
        {
            e.Data = new ConcurrentDictionary<string, StandardPartnumberCache>();
        }
    }

    /// <summary>
    /// 大赢家冷偏型号缓存
    /// </summary>
    public class DyjUnpopularCache
    {
        /// <summary>
        /// 查询关键字
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 冷偏型号
        /// </summary>
        public List<string> Partnumbers { get; set; }
    }

    /// <summary>
    /// 标准型号缓存
    /// </summary>
    public class StandardPartnumberCache
    {
        /// <summary>
        /// 查询关键字
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 标准型号
        /// </summary>
        public List<Models.IStandardPartnumberForShow> Partnumbers { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime ExpireDate { get; set; }
    }
}
