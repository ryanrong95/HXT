using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using System.Collections;
using System.Threading;
using Yahv.Payments.Views;

namespace Yahv.Payments
{
    /// <summary>
    /// 汇率管理
    /// </summary>
    public class ExchangeRates : IEnumerable<Rater>
    {
        internal const ExchangeDistrict DefaultDistrict = ExchangeDistrict.Cn;
        Rater[] raters;

        Thread runer;
        ExchangeRates()
        {
            this.Init();
            (this.runer = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        this.Init();
                        this.Sync();
                    }
                    catch (ThreadAbortException) { }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                    finally
                    {
                        Thread.Sleep(1000);
                    }
                }
            })
            {
                IsBackground = true,
                Priority = ThreadPriority.Highest
            }).Start();
        }

        void Init()
        {

            #region bak

            //using (var reponsitory = new OverallsReponsitory())
            //{
            //    var linq = from item in reponsitory.ReadTable<Layers.Data.Sqls.Overalls.ExchangeRates>()
            //               select item;

            //    var arry = linq.ToArray();

            //    this.raters = arry.Select(item => new Rater
            //    {
            //        Type = (ExchangeType)Enum.Parse(typeof(ExchangeType), item.Type),
            //        District = (ExchangeDistrict)item.District,
            //        From = (Currency)item.From,
            //        To = (Currency)item.To,
            //        Value = item.Value,
            //    }).ToArray();
            //}

            #endregion

            using (var view = new ExchangeRatesView())
            {
                var arry = view.ToArray();

                this.raters = arry.Select(item =>
                {
                    Func<Models.ExchangeRate, bool> predicate = i => i.District == item.District
                         && i.From == item.From
                         && i.To == item.To
                         && i.Type == ExchangeType.Preset.ToString();


                    var rater = new Rater
                    {
                        Type = (ExchangeType)Enum.Parse(typeof(ExchangeType), item.Type),
                        District = item.District,
                        From = item.From,
                        To = item.To,
                        Value = item.Value,
                    };

                    if (rater.Type == ExchangeType.Fixed)
                    {
                        var preset = arry.SingleOrDefault(predicate);
                        if (preset?.StartDate <= DateTime.Now)
                        {
                            rater.Value = preset.Value;
                            ExchangeRatesView.Enter<PvDataReponsitory>(rater);
                            ExchangeRatesView.Delete<PvDataReponsitory>(ExchangeType.Preset,
                                item.District,
                                item.From,
                                item.To);
                        }
                    }
                    return rater;
                }).ToArray();


                foreach (var currency in Enum.GetValues(typeof(Currency)).Cast<Currency>())
                {
                    //更新或是插入  Cny 对 currency 的 type 为  ZhATf 的 汇率

                }
            }

        }

        void Sync()
        {
            //仅仅是为了兼容，如果推广了本类的调用。同步就不需要了。
            foreach (var rater in this.raters)
            {
                if (rater.Type == ExchangeType.Preset)
                {
                    continue;
                }
                //向 Overalls 同步
                ExchangeRatesView.Enter<OverallsReponsitory>(rater);
            }
        }

        static object locker = new object();

        static RaterFiler customs;
        /// <summary>
        /// 海关汇率
        /// </summary>
        /// <remarks>
        /// 如果未来开发对账单重新放到财务模块（王辉）这里出具，就确实需要把海关汇率进行手动维护
        /// (报关的对账单是没有从财务这块走，因此海关对账单暂时可以不同步)
        /// 第一步可以先把芯达通的时时汇率同步更新到PvData.dbo.ExchangeRates
        /// 第二部把手动维护海关汇率的维护页面，也放给新框架下进行维护。并通讯给芯达通
        /// </remarks>
        static public RaterFiler Customs
        {
            get
            {
                if (customs == null)
                {
                    lock (locker)
                    {
                        if (customs == null)
                        {
                            customs = new RaterFiler(ExchangeType.Customs);
                        }
                    }
                }

                return customs;
            }
        }

        static RaterFiler fixeds;
        static public RaterFiler Fixed
        {
            get
            {
                if (fixeds == null)
                {
                    lock (locker)
                    {
                        if (fixeds == null)
                        {
                            fixeds = new RaterFiler(ExchangeType.Fixed);
                        }
                    }
                }

                return fixeds;
            }
        }

        static RaterFiler floating;
        static public RaterFiler Floating
        {
            get
            {
                if (floating == null)
                {
                    lock (locker)
                    {
                        if (floating == null)
                        {
                            floating = new RaterFiler(ExchangeType.Floating);
                        }
                    }
                }

                return floating;
            }
        }


        static RaterFiler tenAmChineseBank;
        /// <summary>
        /// 中国银行时时牌价
        /// </summary>
        /// <remarks>
        /// 王增超：纠正是10点后的第一个汇率
        /// 第一步先做到把以上要求在抓取中做个TenAmChineseBank类型的特殊处理
        /// 同时需要增加最后更新时间Modify,如果10点后抓取的汇率首先判断是TenAmChineseBank类型的更新时间是否为今日？（今日，神速）
        /// 如果是今日就不做什么工作了，如果不是今日就更新
        /// 第二步再做到荣检芯达通项目统一使用这个时时汇率
        /// 第三部再做到公司同意管控的这个公司固定汇率（Fixed）
        /// </remarks>
        static public RaterFiler TenAmChineseBank
        {
            get
            {
                if (tenAmChineseBank == null)
                {
                    lock (locker)
                    {
                        if (tenAmChineseBank == null)
                        {
                            tenAmChineseBank = new RaterFiler(ExchangeType.TenAmChineseBank);
                        }
                    }
                }

                return tenAmChineseBank;
            }
        }


        static RaterFiler universal;
        /// <summary>
        /// 公司统一配置
        /// </summary>
        /// <remarks>
        /// 与王辉商议后，目前财务模块统一使用本属性
        /// </remarks>
        static public RaterFiler Universal
        {
            get
            {
                if (universal == null)
                {
                    lock (locker)
                    {
                        if (universal == null)
                        {
                            universal = new RaterFiler(ExchangeType.TenAmChineseBank);
                        }
                    }
                }

                return universal;
            }
        }




        static ExchangeRates current;
        static object tlocker = new object();

        /// <summary>
        /// 系统当前值
        /// </summary>
        static public ExchangeRates Current
        {
            get
            {
                if (current == null)
                {
                    lock (tlocker)
                    {
                        if (current == null)
                        {
                            current = new ExchangeRates();
                        }
                    }
                }

                return current;
            }
        }

        public int District { get; internal set; }

        /// <summary>
        /// 总调用
        /// </summary>
        /// <param name="index">汇率类型</param>
        /// <returns>指定汇率</returns>
        public RaterFiler this[ExchangeType index]
        {
            get
            {
                switch (index)
                {
                    case ExchangeType.Customs:
                        return Customs;
                    case ExchangeType.Floating:
                        return Floating;
                    case ExchangeType.Fixed:
                        return Fixed;
                    case ExchangeType.TenAmChineseBank:
                        return TenAmChineseBank;
                    default:
                        throw new Exception("不支持");
                }
            }
        }

        public IEnumerator<Rater> GetEnumerator()
        {
            return this.raters.Select(item => item).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
