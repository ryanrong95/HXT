using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Erm.Fingerprints.Services
{
    /// <summary>
    /// 时间
    /// </summary>
    public class TimeServer
    {
        /// <summary>
        /// 差额
        /// </summary>
        long difference;
        TimeServer()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var gain = this.YuanDa().Ticks;
            watch.Stop();
            this.difference = (gain + watch.ElapsedTicks / 2) - DateTime.Now.Ticks;
        }

        /// <summary>
        /// 淘宝获取
        /// </summary>
        public DateTime TaoBo()
        {
            DateTime now = DateTime.Now;
            Stopwatch watch = new Stopwatch();
            try
            {
                watch.Start();
                var result = ApiHelper.Current.Get<TaoBoClass>(LinkSource.TaoboTime);
                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                long lTime = long.Parse(result.data.t + "0000");
                TimeSpan toNow = new TimeSpan(lTime);
                DateTime dtResult = dtStart.Add(toNow);

                //1587708565411
                //1587706170235
                watch.Stop();
                var infact = dtResult.AddMilliseconds(watch.ElapsedMilliseconds);
                return infact;
            }
            catch (Exception)
            {
                //异常情况返回本地时间
                return now;
            }
            //Console.WriteLine(dtResult.ToString("yyyy年MM月dd日 HH:mm:ss"));
        }

        /// <summary>
        /// 淘宝获取
        /// </summary>
        public DateTime YuanDa()
        {
            try
            {
                var result = ApiHelper.Current.Get<YuanDaClass>(LinkSource.YuanDaTime);
                return new DateTime(result.ticks);
            }
            catch (Exception)
            {
                return DateTime.Now;
            }
        }

        /// <summary>
        /// 获取日期
        /// </summary>
        /// <returns></returns>
        public string GetDate()
        {
            return DateTime.Now.AddTicks(difference).ToString("yyyy年MM月dd日");
        }

        /// <summary>
        /// 获取日期时间
        /// </summary>
        /// <returns></returns>
        public string GetDateTime()
        {
            return DateTime.Now.AddTicks(difference).ToString("yyyy年MM月dd日 HH:mm:ss");
        }

        static TimeServer current;
        static object locker = new object();

        /// <summary>
        /// 访问单例
        /// </summary>
        static public TimeServer Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new TimeServer();
                        }
                    }
                }
                return current;
            }
        }

        #region 辅助类

        /// <summary>
        /// 淘宝返回类
        /// </summary>
        internal class TaoBoClass
        {
            internal class Mydata
            {
                public string t { get; set; }
            }

            public string api { get; set; }

            public string v { get; set; }

            public string[] ret { get; set; }
            public Mydata data { get; set; }
        }

        /// <summary>
        /// 远大返回类
        /// </summary>
        internal class YuanDaClass
        {
            public long ticks { get; set; }
        }

        #endregion
    }
}
