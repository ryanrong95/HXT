using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Models.Statistics
{
    public class RateLimits
    {
        static object locker = new object();

        /// <summary>
        /// 客户拜访数统计接口
        /// </summary>
        static RateLimit clientVisits;
        static public RateLimit ClientVisits
        {
            get
            {
                if (clientVisits == null)
                {
                    lock (locker)
                    {
                        if (clientVisits == null)
                        {
                            clientVisits = new RateLimit(1000 * 60, 120);
                        }
                    }
                }
                return clientVisits;
            }
        }

        /// <summary>
        /// DI个数统计接口
        /// </summary>
        static RateLimit di;
        static public RateLimit DI
        {
            get
            {
                if (di == null)
                {
                    lock (locker)
                    {
                        if (di == null)
                        {
                            di = new RateLimit(1000 * 60, 120);
                        }
                    }
                }
                return di;
            }
        }

        /// <summary>
        /// DW个数统计接口
        /// </summary>
        static RateLimit dw;
        static public RateLimit DW
        {
            get
            {
                if (dw == null)
                {
                    lock (locker)
                    {
                        if (dw == null)
                        {
                            dw = new RateLimit(1000 * 60, 120);
                        }
                    }
                }
                return dw;
            }
        }

        /// <summary>
        /// 新增客户数统计接口
        /// </summary>
        static RateLimit newClients;
        static public RateLimit NewClients
        {
            get
            {
                if (newClients == null)
                {
                    lock (locker)
                    {
                        if (newClients == null)
                        {
                            newClients = new RateLimit(1000 * 60, 120);
                        }
                    }
                }
                return newClients;
            }
        }

        internal RateLimits()
        {
        }

        /// <summary>
        /// 单例
        /// </summary>
        static RateLimits interior;
        static object tlocker = new object();

        static internal RateLimits Interior
        {
            get
            {
                lock (tlocker)
                {
                    if (interior == null)
                    {
                        interior = new RateLimits();
                    }
                }

                return interior;
            }
        }

        static public RateLimits Current
        {
            get
            {
                return Interior;
            }
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public RateLimit this[StatisticsApi index]
        {
            get
            {
                switch (index)
                {
                    case StatisticsApi.ClientVisits:
                        return ClientVisits;
                    case StatisticsApi.DI:
                        return DI;
                    case StatisticsApi.DW:
                        return DW;
                    case StatisticsApi.NewClients:
                        return NewClients;
                    default:
                        throw new NotSupportedException("不支持");
                }
            }
        }
    }

    public enum StatisticsApi
    {
        ClientVisits = 1,
        DI = 2,
        DW = 3,
        NewClients = 4
    }

    /// <summary>
    /// 做简单的接口限流，超过限制直接抛异常
    /// </summary>
    public class RateLimit
    {
        int rate; //当前接口访问次数
        int period; //时间周期
        int maxRate; //一个时间周期内的最大访问次数

        internal RateLimit(int period, int maxRate)
        {
            this.rate = 0;
            this.period = period;
            this.maxRate = maxRate;

            Thread thread = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        this.rate = 0;
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
                        Thread.Sleep(this.period);
                    }
                }
            })
            {
                IsBackground = true,
                Priority = ThreadPriority.Highest
            };

            thread.Start();
        }

        /// <summary>
        /// 访问频率校验
        /// </summary>
        public void Verify()
        {
            this.rate += 1;

            if (this.rate > this.maxRate)
            {
                throw new Exception($"当前接口访问频率过高，请{this.period / 1000 / 60}分钟后重试");
            }
        }
    }
}
