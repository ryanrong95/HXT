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
    public class WhFeeRecords
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
        public WhFeeRecords()
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

        static WhFeeRecords current;
        static object obj = new object();

        /// <summary>
        /// 单例
        /// </summary>
        static public WhFeeRecords Current
        {
            get
            {
                if (current == null)
                {
                    lock (obj)
                    {
                        if (current == null)
                        {
                            current = new WhFeeRecords();
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
            using (var pvcenter = new PvCenterReponsitory())
            using (var pvbcrm = new PvbCrmReponsitory())
            {
                //取出订单的入库信息
                var inNotice = from notice in pvwms.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                               join sorting in pvwms.ReadTable<Layers.Data.Sqls.PvWms.Sortings>()
                               on notice.ID equals sorting.NoticeID
                               join storage in pvwms.ReadTable<Layers.Data.Sqls.PvWms.Storages>()
                               on sorting.ID equals storage.SortingID
                               join input in pvwms.ReadTable<Layers.Data.Sqls.PvWms.Inputs>()
                                     on storage.InputID equals input.ID
                               join waybill in pvcenter.ReadTable<Layers.Data.Sqls.PvCenter.Waybills>()
                               on notice.WaybillID equals waybill.ID
                               where storage.ShelveID != null
                            && notice.WareHouseID.StartsWith("HK")//只记录香港外单
                            && notice.Type == 100 //类型是入库的参考D:\Projects_vs2015\Yahv\Solutions\Yahv.Services\Enums\Enum.Notices.cs
                               group storage by new
                               {
                                   input.OrderID,
                                   notice.CreateDate,
                                   input.ClientID,
                                   input.PayeeID, //收款人ID
                                   storage.ShelveID,
                                   StorageID = storage.ID
                               } into groups
                               select new
                               {
                                   groups.Key.ClientID,//获得客户信息
                                   //ShelveQuantity = groups.Select(item => item.ShelveID).Distinct().Count(),//客户占用的库位数量
                                   groups.Key.OrderID,//获得订单信息
                                   groups.Key.CreateDate,//获得入库时间
                                   groups.Key.StorageID,
                                   groups.Key.PayeeID
                               };
                var ienum_inNotices = inNotice.ToArray();

                //取得订单的出库信息
                var outNotice = from notice in pvwms.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                              //  join waybill in pvcenter.ReadTable<Layers.Data.Sqls.PvCenter.Waybills>()
                              //on notice.WaybillID equals waybill.ID
                              join output in pvwms.ReadTable<Layers.Data.Sqls.PvWms.Outputs>()
                              on notice.OutputID equals output.ID
                              join input in pvwms.ReadTable<Layers.Data.Sqls.PvWms.Inputs>()
                              on output.InputID equals input.ID
                                join picking in pvwms.ReadTable<Layers.Data.Sqls.PvWms.Pickings>()
                                on notice.ID equals picking.NoticeID
                                join storage in pvwms.ReadTable<Layers.Data.Sqls.PvWms.Storages>()
                              on picking.StorageID equals storage.ID
                                where notice.Type == 200
                                   && notice.WareHouseID.StartsWith("HK")//只记录香港外单
                                group storage by new
                                {
                                    input.OrderID,
                                    picking.CreateDate,
                                    StorageID = storage.ID,
                                } into groups
                                select new
                                {
                                    groups.Key.OrderID,//获得订单信息
                                    groups.Key.CreateDate,//获得出库时间
                                    groups.Key.StorageID,//获得库存信息
                                    ShelveQuantity = groups.Select(item => item.ShelveID).Distinct().Count(),//客户出库的库位数量，出库的库位数量大于库存的5天进行收费
                                };
                var ienum_outNotices = outNotice.ToArray();

                var data = from outs in ienum_outNotices
                           join ins in ienum_inNotices
                           on outs.OrderID equals ins.OrderID
                           select new
                           {
                               ins.ClientID,
                               ins.OrderID,
                               ins.PayeeID,
                               outs.ShelveQuantity,
                               InsCreateDate = ins.CreateDate,
                               OutsCreateDate = outs.CreateDate,
                           };

                var ienums_data = data.ToArray();
                var igroup_data = from d in data
                                  group d by new
                                  {
                                      d.ClientID,
                                      d.OrderID,
                                      d.ShelveQuantity,
                                      d.PayeeID
                                  } into groups
                                  select new
                                  {
                                      groups.Key.ClientID,
                                      groups.Key.OrderID,
                                      groups.Key.PayeeID,
                                      groups.Key.ShelveQuantity,
                                      StockTime = (from pres in groups
                                                   group pres by pres.OrderID into group2s
                                                   select new
                                                   {
                                                       OrderID = group2s.Key,
                                                       MinInsCreateDate = group2s.Min(item => item.InsCreateDate),//最早入库时间
                                                       MaxOutsCreateDate = group2s.Max(item => item.OutsCreateDate)//最晚出库时间
                                                   }),

                                  };
                //获取时间
                //计算费用
                foreach (var group in igroup_data)
                {
                    string payeeID = group.PayeeID ;

                    //取出来需要支付的周数
                    int payTotal = (int)(Math.Floor(group.StockTime.Select(item =>
                    {
                        double total = (item.MaxOutsCreateDate - item.MinInsCreateDate).TotalDays - freeDay;
                        return total > 0d ? total : 0d;
                    }).Sum()) / 7d);
                    var payShelveQuantity = group.ShelveQuantity;//需要支付的库位数量


                    //记录过费用的集合
                    var recivables = (pvbcrm.ReadTable<Layers.Data.Sqls.PvbCrm.Receivables>().Where(
                        item => item.Payer == group.ClientID
                             && item.Payee == /*group.PayeeID*/ payeeID
                             && item.Business == ConductConsts.供应链
                             && item.Catalog == CatalogConsts.仓储服务费
                             && item.Subject == SubjectConsts.仓储费
                             && item.Currency == (int)Currency.CNY
                             && item.Price == 100 * payShelveQuantity
                             && item.Status == (int)GeneralStatus.Normal
                    )).ToArray();

                    var paymentManager = PaymentManager.Npc[group.ClientID, payeeID/* group.PayeeID*/][ConductConsts.供应链]
           .Receivable[CatalogConsts.仓储服务费, SubjectConsts.仓储费];

                    for (int index = 1; index <= payTotal; index++)
                    {
                        //仓储费，区分代仓储与代报关的业务
                        //判断是否记录过费用了？如果记录过费用，就直接continue！
                        //否则记录费用
                        if (recivables.Count() < index)
                        {
                            paymentManager.RecordStorage(Currency.CNY, 100 * payShelveQuantity);
                        }
                        else
                        {
                            continue;
                        }
                    }

                }

            }

        }

    }

}
