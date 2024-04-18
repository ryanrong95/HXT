using Needs.Overall.Models;
using Needs.Overall.Views;
using Needs.Underly;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Needs.Overall
{
    /// <summary>
    /// 语言集合
    /// </summary>
    public sealed class ExchangeRates<T> : IEnumerable<IExchangeRate> where T : ExchangeRate
    {
        List<ExchangeRate> source;

        ExchangeRates()
        {
            this.source = new List<ExchangeRate>();

            this.Ensure();
            this.Regular();
        }

        public IEnumerable<IExchangeRate> this[District district]
        {
            get
            {
                return this.source.Where(item => item.District == district);
            }
        }


        /// <summary>
        /// 汇率索引
        /// </summary>
        /// <param name="district">交易地区</param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>

        public decimal this[District district, Currency from, Currency to]
        {
            get
            {
                return this.source.Single(item => item.District == district && item.From == from && item.To == to).Value;
            }
        }

        #region 数据源

        void Ensure()
        {
            using (var reponsitory = new Layer.Data.Sqls.BvOverallsReponsitory())
            {
                using (var view = Needs.Linq.Factory<ExchangeRate, ExchangeRatesView>.Current)
                {
                    this.source = view.Where(item => item.Type == typeof(T).Name).ToList();
                }
            }
        }

        void Regular()
        {
            var t = new Thread(delegate ()
            {
                while (true)
                {
                    this.Ensure();
                    Thread.Sleep(5 * 60 * 1000);
                }
            })
            {
                IsBackground = true,
                Priority = ThreadPriority.Highest,
                Name = "ExchangeRate"
            };
            t.Start();
        }

        #endregion

        #region 单例

        static ExchangeRates<T> current;
        static object locker = new object();

        static public ExchangeRates<T> Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new ExchangeRates<T>();
                        }
                    }
                }
                return current;
            }
        }

        #endregion

        public IEnumerator<IExchangeRate> GetEnumerator()
        {
            return this.source.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

    }
}
