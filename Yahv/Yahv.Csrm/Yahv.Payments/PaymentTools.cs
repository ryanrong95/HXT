using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Yahv.Payments.Tools;
using Yahv.Underly;

namespace Yahv.Payments
{
    public class PaymentTools : IEnumerable<PayTool>
    {
        IEnumerable<PayTool> data;
        PaymentTools()
        {
            //初始化数据
            try
            {
                //using (StreamReader r = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "db", "PayTools.json")))
                //{
                //    string json = r.ReadToEnd();
                //    this.data = JsonConvert.DeserializeObject<List<PayTool>>(json);
                //}

                using (var subjectsView = new SubjectsView())
                {
                    this.data = subjectsView.Select(item => new PayTool()
                    {
                        Conduct = item.Conduct,
                        Type = item.Type == SubjectType.Input ? PayItemType.Receivables : PayItemType.Payables,
                        Name = item.Name,
                        Subject = item.Name,
                        Catalog = item.Catalog,
                        Quotes = new PayQuotes(new PayQuote() { Currency = item.Currency, Price = item.Price })
                    }).ToArray();
                }
            }
            catch (Exception ex)
            {
                //页面测试使用
                //using (StreamReader r = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "/bin/db/PayTools.json"))
                //{
                //    string json = r.ReadToEnd();
                //    this.data = JsonConvert.DeserializeObject<List<PayTool>>(json);
                //}
            }
        }

        /// <summary>
        /// 据实
        /// </summary>
        static readonly public PayTool Fact = PayTool.Fact;


        static ReceivablesTool receivables;
        static object locker_rec = new object();
        /// <summary>
        /// 应收模块费用
        /// </summary>
        static public ReceivablesTool Receivables
        {
            get
            {
                if (receivables == null)
                {
                    lock (locker_rec)
                    {
                        if (receivables == null)
                        {
                            receivables = new ReceivablesTool();
                        }
                    }
                }

                return receivables;
            }
        }

        static PayablesTool payables;
        static object lockerPay = new object();
        /// <summary>
        /// 应付模块费用
        /// </summary>
        static public PayablesTool Payables
        {
            get
            {
                if (payables == null)
                {
                    lock (lockerPay)
                    {
                        if (payables == null)
                        {
                            payables = new PayablesTool();
                        }
                    }
                }

                return payables;
            }
        }


        static PaymentTools current;
        static object locker = new object();
        /// <summary>
        /// 应付模块费用
        /// </summary>
        static public PaymentTools Data
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new PaymentTools();
                        }
                    }
                }

                return current;
            }
        }


        //载重工具 ，只表示应收
        //static Carloads carloadsTool;
        //static object locker_carload = new object();
        ///// <summary>
        ///// 载重工具费用(应收)
        ///// </summary>
        ///// <remarks>
        ///// 只用于应收 代仓储 杂费
        ///// </remarks>
        //static public Carloads Carloads
        //{
        //    get
        //    {
        //        if (carloadsTool == null)
        //        {
        //            lock (locker_carload)
        //            {
        //                if (carloadsTool == null)
        //                {
        //                    carloadsTool = new Carloads();
        //                }
        //            }
        //        }

        //        return carloadsTool;
        //    }
        //}

        ////地区工具， 只表示应收， 注意区分打车与小车
        //static FreightDetails freightdetails;
        //static object locker_freight = new object();
        ///// <summary>
        ///// 地区工具费用(应收)
        ///// </summary>
        ///// <remarks>
        ///// 只用于应收 代仓储 杂费
        ///// </remarks>
        //static public FreightDetails FreightDetails
        //{
        //    get
        //    {
        //        if (freightdetails == null)
        //        {
        //            lock (locker_freight)
        //            {
        //                if (freightdetails == null)
        //                {
        //                    freightdetails = new FreightDetails();
        //                }
        //            }
        //        }

        //        return freightdetails;
        //    }
        //}

        ////标签费 ，只表示应收
        //static LabelFee labelFeeTool;
        //static object locker_label = new object();
        ///// <summary>
        ///// 标签费(应收)
        ///// </summary>
        //static public LabelFee LabelFee
        //{
        //    get
        //    {
        //        if (labelFeeTool == null)
        //        {
        //            lock (locker_label)
        //            {
        //                if (labelFeeTool == null)
        //                {
        //                    labelFeeTool = new LabelFee();
        //                }
        //            }
        //        }

        //        return labelFeeTool;
        //    }
        //}

        public IEnumerator<PayTool> GetEnumerator()
        {
            return this.data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
