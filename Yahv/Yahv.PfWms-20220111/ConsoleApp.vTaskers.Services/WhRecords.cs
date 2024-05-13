using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yahv.Payments;
using Yahv.Underly;
using Yahv.Utils.Serializers;

namespace ConsoleApp.vTaskers.Services
{
    /// <summary>
    /// 库房费用的处理
    /// </summary>
    public class WhRecords
    {
        Thread runner;

        object objectLock = new object();
        event SqlErrorEventHandler sqlError;

        /// <summary>
        /// sql错误事件
        /// </summary>
        public event SqlErrorEventHandler SqlError
        {
            add
            {
                lock (objectLock)
                {

                    System.Reflection.EventInfo eventInfo = this.GetType().GetEvent(nameof(this.SqlError));

                    if (sqlError != null)
                    {
                        foreach (Delegate dele in sqlError.GetInvocationList())
                        {
                            sqlError -= (dele as SqlErrorEventHandler);
                            eventInfo.RemoveEventHandler(this, dele);
                        }

                    }
                    sqlError += value;
                }
            }
            remove
            {
                lock (objectLock)
                {
                    sqlError -= value;
                }
            }
        }
        public WhRecords()
        {
            this.runner = new Thread(() =>
            {
                //while (true)
                //{
                try
                {
                    this.StorageFee();
                }
                catch (SqlException ex)
                {
                    //如果Console.WriteLine(); 显示错误需要增加事件
                    if (this != null && this.sqlError != null)
                    {
                        this.sqlError(ex.Message, new EventArgs());
                    }

                    return;
                }
                catch (DbException)
                {
                    //如果Console.WriteLine(); 显示错误需要增加事件
                    //continue;
                }
                catch (Exception ex)
                {
                    Logs.Log(ex.InnerException ?? ex);
                    throw ex;
                }
                finally
                {
                    Thread.Sleep(10);
                    //任务计划加上这句，任务完成后kill当前进程
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                }
                //}
            });
        }
        public void Start()
        {
            this.runner.Start();
        }

        static WhRecords current;
        static object obj = new object();

        /// <summary>
        /// 单例
        /// </summary>
        static public WhRecords Current
        {
            get
            {
                if (current == null)
                {
                    lock (obj)
                    {
                        if (current == null)
                        {
                            current = new WhRecords();
                        }
                    }
                }
                return current;
            }
        }

        //五天内免仓储费用（配置化开发）
        int freeDay = int.Parse(ConfigurationManager.AppSettings["freeDay"]);
        /// <summary>
        /// 记录仓储费用
        /// </summary>
        public void StorageFee()
        {
            #region 逻辑流程
            /*
                1)入仓后，5天内免仓储费用（根据客户配置走）
                2)已经租赁的客户不能记录仓储费用，例如：
                    a)租赁了5个库位，实际上只占用了3个库位，就不能记录仓储费
                    b)租赁了3个库位，实际上占用了5个库位，就需要记录2个库位的仓储费
                3)按照最一开始的方式：满一周才收费，在第二周或更长时间中，不满一周不收费
                每日2点调用一次，目前固定费用100元/立方米
                注意要扣减掉有效租赁的库位
             */
            #endregion

            #region 2020.12.07日修改逻辑流程
            /*
             1)入仓后，5天内免仓储费用（根据客户配置走）
             2）5元/箱/天，大箱：60CM×60CM×60CM以上；3元/箱/天，中箱：40CM×40CM×40CM；1元/箱/天，小箱：尺寸在40CM以下

             */
            #endregion 

            using (var pvwms = new PvWmsRepository())
            using (var pvbcrm = new PvbCrmReponsitory())
            {
                //查出所有已经占用的库位
                //2021.6.9提出问题：是否只记录香港外单
                var linq_storages = from storage in pvwms.ReadTable<Layers.Data.Sqls.PvWms.Storages>()
                                    join input in pvwms.ReadTable<Layers.Data.Sqls.PvWms.Inputs>()
                                    on storage.InputID equals input.ID
                                    where storage.Quantity > 0
                                       && storage.ShelveID != null
                                       && input.ClientID != null
                                       //&&storage.WareHouseID.StartsWith("HK")
                                    select new
                                    {
                                        input.ClientID,
                                        storage.ShelveID,
                                        input.CreateDate,/*重要的在库的逻辑*/
                                        storage.InputID,
                                        input.OrderID
                                    };

                var ienum_storages = linq_storages.ToArray();

                //获得OrderID和payeeid在下面进行分组
                var linq_orders = from storage in pvwms.ReadTable<Layers.Data.Sqls.PvWms.Storages>()
                                  join input in pvwms.ReadTable<Layers.Data.Sqls.PvWms.Inputs>() on storage.InputID equals input.ID
                                  join order in pvwms.ReadTable<Layers.Data.Sqls.PvWms.OrdersTopView>() on input.OrderID equals order.ID
                                  where storage.Quantity > 0
                                     && storage.ShelveID != null
                                     && input.ClientID != null
                                  select new
                                  {
                                      input.OrderID,
                                      order.PayeeID
                                  };
                var ienum_orders = linq_orders.ToArray();

                // 写两遍好发现input.orderid 是否错误了
                //以客户ID和收款人PayeeID为分组 获取每个客户和对应的收款人PayeeID 占用的库位以及库位数量
                var igroup_storages = (from storage in ienum_storages
                                       join order in ienum_orders on storage.OrderID equals order.OrderID
                                       group storage by new
                                       {
                                           storage.ClientID,
                                           order.PayeeID
                                       } into groups
                                       select new
                                       {
                                           ClientID = groups.Key.ClientID,
                                           PayeeID = groups.Key.PayeeID,
                                           //ShelveIDs = groups.Select(item => item.ShelveID).Distinct().ToArray(),
                                           ShelveQuantity = groups.Select(item => item.ShelveID).Distinct().Count(),//客户占用的库位数量
                                           MinCreateDates = (from pres in groups
                                                             group pres by pres.InputID into group2s
                                                             select new
                                                             {
                                                                 InputID = group2s.Key,
                                                                 MinCreateDate = group2s.Min(item => item.CreateDate)
                                                             })//客户占用每个库位的最早时间
                                       }).ToArray();

                //查出租赁信息
                var linq_lsNotice = from lsNotice in pvwms.ReadTable<Layers.Data.Sqls.PvWms.LsNotice>()
                                    select new
                                    {
                                        lsNotice.ClientID,
                                        lsNotice.PayeeID,
                                        lsNotice.Quantity,
                                        lsNotice.EndDate
                                    };

                //以客户ID和收款人ID为分组获取每个客户和 对应的收款人PayeeID 有效租赁的库位数量（eg:一个客户有两个payeeID，分别对应芯达通和恒远,以客户ID和收款人ID为分组可以获得该客户在芯达通和恒远分别对应的租赁的 库位数和 结束日期）
                var igroup_lsNotice = (from item in linq_lsNotice
                                       where item.EndDate >= DateTime.Now//租赁的结束时间必须大于当前时间才能证明是有效库位
                                       group item by new
                                       {
                                           item.ClientID,
                                           item.PayeeID
                                       } into groups
                                       select new
                                       {
                                           ClientID = groups.Key.ClientID,
                                           PayeeID = groups.Key.PayeeID,
                                           ShelveQuantity = groups.Sum(g => g.Quantity),
                                           EndDate = groups.Select(g => g.EndDate).FirstOrDefault()
                                       }).ToArray();

                //如果一个客户有两个收款人ID，证明他在两个地方（如恒远和芯达通）都存着货物，需要分别算出来两地租赁的库位的费用
                foreach (var group in igroup_storages)
                {
                    //做一下订单循环分别给 若干  收款人进行记录
                    //获得该客户与其付款人对应的有效库位数
                    var effectiveShelveQuantity = igroup_lsNotice.
                        Where(item => item.ClientID == group.ClientID && item.PayeeID == group.PayeeID)
                        .Sum(tem => (int?)tem.ShelveQuantity) ?? 0;

                    //租用的库位号-有效租赁的库位号就是要收费的库位号
                    if (group.ShelveQuantity - effectiveShelveQuantity > 0)
                    {
                        //取出来需要支付的周数
                        int payTotal = (int)(Math.Floor(group.MinCreateDates.Select(item =>
                            {
                                double total = (DateTime.Now - item.MinCreateDate).TotalDays - freeDay;
                                return total > 0d ? total : 0d;
                            }).Sum()) / 7d);

                        //记录过费用的集合
                        var recivables = (pvbcrm.ReadTable<Layers.Data.Sqls.PvbCrm.Receivables>().Where(
                            item => item.Payer == group.ClientID
                                 && item.Payee == group.PayeeID
                                 && item.Business == ConductConsts.供应链
                                 && item.Catalog == CatalogConsts.仓储服务费
                                 && item.Subject == SubjectConsts.仓储费
                                 && item.Currency == (int)Currency.CNY
                                 && item.Price == 100 * (group.ShelveQuantity - effectiveShelveQuantity)
                                 && item.Status == (int)GeneralStatus.Normal
                        )).ToArray();

                        var paymentManager = PaymentManager.Npc[group.ClientID, group.PayeeID][ConductConsts.供应链]
               .Receivable[CatalogConsts.仓储服务费, SubjectConsts.仓储费];

                        for (int index = 1; index <= payTotal; index++)
                        {
                            //仓储费，区分代仓储与代报关的业务
                            //判断是否记录过费用了？如果记录过费用，就直接continue！
                            //否则记录费用
                            if (recivables.Count() < index)
                            {
                                paymentManager.RecordStorage(Currency.CNY, 100 * (group.ShelveQuantity - effectiveShelveQuantity));
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                }

                #region 废弃
                ////需要支付的库位总数
                //var paymentShelveCount = group.ShelveQuantity - effectiveShelveQuantity;

                ////取出该客户的库位总数
                //var linq = igroup_storages.Where(tem => tem.ClientID == group.ClientID).FirstOrDefault().ShelveIDs;

                ////该客户需要支付的库位总数
                //var shelves = linq.Take(paymentShelveCount);

                //foreach (var shelve in shelves)
                //{
                //    //库位被占用的最早时间
                //    var earlyTime = ienum_storages.Where(tem => tem.ClientID == group.ClientID && tem.ShelveID == shelve).OrderBy(tem => tem.CreateDate).FirstOrDefault().CreateDate;

                //    TimeSpan timeSpan = DateTime.Now - earlyTime;
                //    timeSpan.TotalDays
                //    if (timeSpan.Days > freeDay)
                //    {
                //        //几周
                //        var week = Math.Floor(Convert.ToDecimal(timeSpan.Days - freeDay) / 7);

                //        //调用王辉的接口？？传值周数、ClientID

                //    }
                //};
                #endregion

            }

        }

    }

}
