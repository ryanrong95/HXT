using Layers.Data.Sqls;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YaHv.PvData.Services
{
    /// <summary>
    /// 缓存管理
    /// </summary>
    [Obsolete("废弃，不再使用")]
    public class CacheManager
    {
        /// <summary>
        /// 标准型号缓存
        /// </summary>
        public ConcurrentDictionary<string, StandardPartnumberCache> StandardPartnumberCaches { get; private set; }

        private Random random = new Random();

        internal CacheManager()
        {
            this.StandardPartnumberCaches = new ConcurrentDictionary<string, StandardPartnumberCache>();

            var thread = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        //清除过期的缓存数据
                        var keys = this.StandardPartnumberCaches.Keys;
                        foreach (var key in keys)
                        {
                            if (this.StandardPartnumberCaches.ContainsKey(key))
                            {
                                StandardPartnumberCache cache;
                                this.StandardPartnumberCaches.TryGetValue(key, out cache);
                                if (cache != null && DateTime.Now > cache.ExpireDate)
                                {
                                    this.StandardPartnumberCaches.TryRemove(key, out cache);
                                }
                            }
                        }
                    }
                    catch (ThreadAbortException)
                    {
                    }
                    catch (Exception)
                    {
                    }
                    finally
                    {
                        Thread.Sleep(10000);
                    }
                }
            })
            {
                Name = $"{nameof(CacheManager)}_{nameof(StandardPartnumberCaches)}",
                IsBackground = true,
                Priority = ThreadPriority.Highest
            };
            thread.Start();
        }

        /// <summary>
        /// 单例
        /// </summary>
        static CacheManager interior;
        static object locker = new object();

        static internal CacheManager Interior
        {
            get
            {
                lock (locker)
                {
                    if (interior == null)
                    {
                        interior = new CacheManager();
                    }
                }

                return interior;
            }
        }

        static public CacheManager Current
        {
            get
            {
                return Interior;
            }
        }

        public bool AddStandardPartnumberCache(string key, List<Models.IStandardPartnumberForShow> standardPartnumbers)
        {
            var cache = new StandardPartnumberCache
            {
                Key = key,
                Partnumbers = standardPartnumbers,
                //设置一个随机的过期时间，避免缓存同时失效
                ExpireDate = DateTime.Now.AddSeconds(random.Next(5, 10))
            };
            return this.StandardPartnumberCaches.TryAdd(key, cache);
        }

        public List<Models.IStandardPartnumberForShow> GetStandardPartnumberCache(string key)
        {
            StandardPartnumberCache cache;
            this.StandardPartnumberCaches.TryGetValue(key, out cache);
            if (cache == null)
                return null;

            if (DateTime.Now > cache.ExpireDate)
            {
                this.StandardPartnumberCaches.TryRemove(key, out cache);
                return null;
            }

            return cache.Partnumbers;
        }
    }
}
