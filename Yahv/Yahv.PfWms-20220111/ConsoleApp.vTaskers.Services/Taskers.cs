using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yahv.Services;
using Yahv.Services.Enums;
using Yahv.Underly;

namespace ConsoleApp.vTaskers.Services
{
    public delegate void SqlErrorEventHandler(object sender, EventArgs e);

    /// <summary>
    /// 任务
    /// </summary>
    public class Taskers
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

        ///// <summary>
        ///// 箱号删除时间
        ///// </summary>
        //int deleteBoxDay = DateTime.Now.AddDays(-1).Day;

        public Taskers()
        {
            //Timer
            //AutoResetEvent autoEvent = new AutoResetEvent(false);
            //TimerCallback timerDelegate = new TimerCallback(CheckStatus);
            //var spanStart = DateTime.Now - DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 17:00:00"));
            //var spanEnd = DateTime.Now - DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 19:00:00"));
            //Timer timer = new Timer(timerDelegate,autoEvent,spanStart,spanEnd);


            this.runner = new Thread(() =>
            {
                while (true)
                {
                    try
                    {

                        var date = DateTime.Now;
                        //var spanStart = date - DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 17:00:00"));
                        //var spanEnd = date - DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 19:00:00"));
                        //if (new TimeSpan(0, 0, 0) < spanStart && spanEnd < new TimeSpan(0, 0, 0))
                        //{
                        //    Timer timer = new Timer(timerDelegate, autoEvent, new TimeSpan(1, 0, 0), new TimeSpan(0, 0, 5));
                        //    autoEvent.WaitOne(1000 * 60 * 120, false);
                        //    timer.Change(new TimeSpan(0), new TimeSpan(0, 0, 5));
                        //    timer.Dispose();
                        //    //if(date<DateTime.Parse("17:00:00")|| date > DateTime.Parse("19:00:00"))
                        //    //{
                        //}
                        //else
                        //{
                        //    this.TaskWaybill();
                        //}
                        ////}

                        if (date < DateTime.Parse("17:00:00") || date > DateTime.Parse("19:00:00"))
                        {
                            this.TaskWaybill();
                        }

                        //if (deleteBoxDay != DateTime.Now.Day)
                        //{
                        //    this.DeleteBox();
                        //    deleteBoxDay = DateTime.Now.Day;
                        //}
                    }
                    catch (SqlException ex)
                    {
                        //如果Console.WriteLine(); 显示错误需要增加事件
                        if (this != null && this.sqlError != null)
                        {
                            this.sqlError(ex.Message, new EventArgs());
                        }

                        Logs.Log(ex.InnerException ?? ex);

                        continue;
                    }
                    catch (DbException)
                    {
                        //如果Console.WriteLine(); 显示错误需要增加事件
                        continue;
                    }
                    catch (Exception ex)
                    {
                        //控制台输出错误
                        Console.WriteLine((ex.InnerException ?? ex).Message);
                        //然后记录日志并进行下次操作
                        Logs.Log(ex.InnerException ?? ex);
                        continue;
                    }
                    finally
                    {
                        Thread.Sleep(10);
                    }
                }
            });
        }

        public void Start()
        {
            this.runner.Start();
        }

        static Taskers current;
        static object obj = new object();

        /// <summary>
        /// 单例
        /// </summary>
        static public Taskers Current
        {
            get
            {
                if (current == null)
                {
                    lock (obj)
                    {
                        if (current == null)
                        {
                            current = new Taskers();
                        }
                    }
                }
                return current;
            }
        }

        /// <summary>
        /// 根据运单ID确认收货
        /// </summary>
        /// <param name="waybillID"></param>
        static public void ReceiptConfirm(string adminID, string waybillID)
        {
            Current.TaskWaybill(adminID, waybillID);
        }

        /// <summary>
        /// 确认天数
        /// </summary>
        int confirmDaysCount = int.Parse(ConfigurationManager.AppSettings["confirmDaysCount"]);

        public void TaskWaybill(string adminID = null, string waybillID = null)
        {
            /*
             逻辑流程：取出香港/深圳的未确认收货的订单，
             一、判断限制条件是否为真，为真是自提/送货上门确认收货
             二、限制条件为假则是快递7天后自动确认收货
            连接output信息得到香港/深圳库房所有的确认收货的通知，以OrderItemID进行分组得到对应的通知数据和订单数据，如果通知数据和订单项数据是相等的，并且对应的通知数量和订单项的数量是相等的，就进行订单确认收货。无论订单是否确认收货，对应的运单都要确认收货
             */

            var limitExcute = !string.IsNullOrWhiteSpace(adminID) && !string.IsNullOrWhiteSpace(waybillID);

            using (var pvwms = new PvWmsRepository())
            using (var pvcenter = new PvCenterReponsitory())
            using (var pvwsorder = new PvWsOrderReponsitory())
            {
                var linq = from waybill in pvwms.ReadTable<Layers.Data.Sqls.PvWms.CgWaybillsTopView>()
                           where (waybill.ConfirmReceiptStatus == 100 || waybill.ConfirmReceiptStatus == null)//未确认
                               && waybill.wbStatus == 200 //   正常出库 
                               && waybill.wbExcuteStatus == (int)CgPickingExcuteStatus.Completed //完成装运
                               && waybill.NoticeType == (int)CgNoticeType.Out
                               && (waybill.Source == (int)CgNoticeSource.AgentSend
                                   || waybill.Source == (int)CgNoticeSource.Transfer
                                   || waybill.WareHouseID.StartsWith("SZ"))
                           orderby waybill.wbID descending
                           select new
                           {
                               WaybillID = waybill.wbID,
                               Type = (WaybillType)waybill.wbType,
                               OrderID = waybill.OrderID,
                               CreateDate = waybill.wbCreateDate,
                               ModifyDate = waybill.wbModifyDate,
                           };

                #region 自提/送货上门确认收货
                if (limitExcute)
                {
                    foreach (var waybill in linq.Where(item => item.WaybillID == waybillID).ToArray())
                    {
                        try
                        {
                            pvcenter.Update<Layers.Data.Sqls.PvCenter.Waybills>(new
                            {
                                ConfirmReceiptStatus = 200     //已确认收货
                            }, item => item.ID == waybill.WaybillID); //更新中心的waybill
                        }
                        catch (Exception ex)
                        {
                            //控制台输出错误
                            Console.WriteLine((ex.InnerException ?? ex).Message);
                            //然后记录日志并进行下次操作
                            Logs.Log($"更新{nameof(Layers.Data.Sqls.PvCenter.Waybills)}状态：ConfirmReceiptStatus = 200 where item.ID == {waybill.WaybillID}", ex.InnerException ?? ex);
                        }


                        try
                        {
                            string url = Wms.Services.FromType.XdtSZSZReceiptConfirm.GetDescription();
                            var data = Yahv.Utils.Http.ApiHelper.Current.JPost(url, new
                            {
                                AdminID = adminID,
                                WaybillID = waybill.WaybillID
                            });
                        }
                        catch (Exception ex)
                        {
                            //控制台输出错误
                            Console.WriteLine((ex.InnerException ?? ex).Message);
                            //然后记录日志并进行下次操作
                            Logs.Log($"请求华芯通：{ Wms.Services.FromType.XdtSZSZReceiptConfirm.GetDescription()}地址： AdminID = {adminID} and WaybillID == {waybill.WaybillID}", ex.InnerException ?? ex);
                        }



                        //处理外单的 未确认收货
                        if (!string.IsNullOrWhiteSpace(waybill.OrderID))
                        {
                            try
                            {
                                this.TaskOrder(waybill.OrderID, adminID);
                            }
                            catch (Exception ex)
                            {

                                //控制台输出错误
                                Console.WriteLine((ex.InnerException ?? ex).Message);
                                //然后记录日志并进行下次操作
                                Logs.Log($"处理订单TaskOrder：OrderID == {waybill.OrderID} and AdminID = {adminID} ", ex.InnerException ?? ex);
                            }

                        }
                        //处理内单的 未确认收货（内单的一个运单或许会有多个订单）
                        else
                        {
                            //取出内单的所有订单
                            var orderIDs = (from notice in pvwms.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                                            join output in pvwms.ReadTable<Layers.Data.Sqls.PvWms.Outputs>() on notice.OutputID equals output.ID
                                            where notice.WaybillID == waybill.WaybillID
                                            select new
                                            {
                                                output.OrderID
                                            }).Distinct().ToArray();

                            //循环订单更改状态为 客户已收货
                            foreach (var item in orderIDs)
                            {
                                try
                                {
                                    this.TaskOrder(item.OrderID, adminID);
                                }
                                catch (Exception ex)
                                {

                                    //控制台输出错误
                                    Console.WriteLine((ex.InnerException ?? ex).Message);
                                    //然后记录日志并进行下次操作
                                    Logs.Log($"处理订单TaskOrder：OrderID == {item.OrderID} and AdminID = {adminID} ", ex.InnerException ?? ex);
                                }
                            }

                        }
                    }
                    return;
                }
                #endregion

                #region 快递7天后自动确认收货
                var waybills = linq.Take(10).ToArray();

                foreach (var waybill in waybills)
                {
                    var lastDate = waybill.ModifyDate ?? waybill.CreateDate;

                    if (waybill.Type == WaybillType.LocalExpress || waybill.Type == WaybillType.InternationalExpress)
                    {
                        //#if DEBUG
                        //if (lastDate.AddSeconds(10) <= DateTime.Now)
                        //#else
                        if (lastDate.AddDays(confirmDaysCount) <= DateTime.Now)
                        //#endif
                        {


                            try
                            {
                                pvcenter.Update<Layers.Data.Sqls.PvCenter.Waybills>(new
                                {
                                    ConfirmReceiptStatus = 200     //已确认收货
                                }, item => item.ID == waybill.WaybillID); //更新中心的waybill
                            }
                            catch (Exception ex)
                            {
                                //控制台输出错误
                                Console.WriteLine((ex.InnerException ?? ex).Message);
                                //然后记录日志并进行下次操作
                                Logs.Log($"更新{nameof(Layers.Data.Sqls.PvCenter.Waybills)}状态：ConfirmReceiptStatus = 200 where item.ID == {waybill.WaybillID}", ex.InnerException ?? ex);
                            }

                            try
                            {
                                string url = Wms.Services.FromType.XdtSZSZReceiptConfirm.GetDescription();
                                var data = Yahv.Utils.Http.ApiHelper.Current.JPost(url, new
                                {
                                    AdminID = adminID ?? Npc.Robot.Obtain(),
                                    WaybillID = waybill.WaybillID
                                });
                            }
                            catch (Exception ex)
                            {
                                //控制台输出错误
                                Console.WriteLine((ex.InnerException ?? ex).Message);
                                //然后记录日志并进行下次操作
                                Logs.Log($"请求华芯通：{ Wms.Services.FromType.XdtSZSZReceiptConfirm.GetDescription()}地址： AdminID = {adminID ?? Npc.Robot.Obtain()} and WaybillID == {waybill.WaybillID}", ex.InnerException ?? ex);
                            }
                        }

                        try
                        {
                            this.TaskOrder(waybill.OrderID, adminID);
                        }
                        catch (Exception ex)
                        {

                            //控制台输出错误
                            Console.WriteLine((ex.InnerException ?? ex).Message);
                            //然后记录日志并进行下次操作
                            Logs.Log($"处理订单TaskOrder：waybill.OrderID == {waybill.OrderID} and AdminID = {adminID} ", ex.InnerException ?? ex);
                        }

                    }

                }

                #endregion
            }
        }


        /// <summary>
        /// 订单确认收货
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="adminID"></param>
        public void TaskOrder(string orderID = null, string adminID = null)
        {
            // 香港出库的可能有报关的，这些运单是不需要确认的
            //自提和送货上门的限制条件

            /*
             逻辑流程：连接output信息得到香港/深圳库房所有的确认收货的通知，以OrderItemID进行分组得到对应的通知数据和订单数据，如果通知数据和订单项数据是相等的，并且对应的通知数量和订单项的数量是相等的，就进行订单确认收货
             */

            NoticeQuery[] notices = null;


            using (var pvwms = new PvWmsRepository())
            using (var pvcenter = new PvCenterReponsitory())
            using (var pvwsorder = new PvWsOrderReponsitory())
            {

                try
                {

                    #region 第三种写法：最后才考虑
                    //var linq_outputs = from output in pvwms.ReadTable<Layers.Data.Sqls.PvWms.Outputs>()
                    //                   where output.OrderID == orderID
                    //                   select new NoticeQuery
                    //                   {
                    //                       OutputID = output.ID,//通知的销项ID
                    //                       ItemID = output.ItemID
                    //                   };
                    //var outputs = linq_outputs.ToArray();

                    ////得到香港/深圳库房所有的确认收货的通知
                    //var linq_notices = from notice in pvwms.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                    //                   join waybill in pvwms.ReadTable<Layers.Data.Sqls.PvWms.WaybillsOriginView>() on notice.WaybillID equals waybill.ID
                    //                   orderby waybill.ID descending
                    //                   where waybill.ConfirmReceiptStatus == 200
                    //                         //&& (waybill.OrderID == orderID)//检查索引(经检查Output的OrderID索引建立不当：已解决)
                    //                         && waybill.Status == 200 //   正常出库 
                    //                         && waybill.ExcuteStatus == (int)CgPickingExcuteStatus.Completed //完成装运
                    //                         && waybill.NoticeType == (int)CgNoticeType.Out
                    //                         && (waybill.Source == (int)CgNoticeSource.AgentSend
                    //                             || waybill.Source == (int)CgNoticeSource.Transfer
                    //                             || notice.WareHouseID.StartsWith("SZ"))//检查索引
                    //                   select new NoticeQuery
                    //                   {
                    //                       WaybillID = waybill.ID,
                    //                       OrderID = waybill.OrderID,
                    //                       ID = notice.ID,
                    //                       SendedQuantity = notice.Quantity, //通知的数量
                    //                       InputID = notice.InputID, //通知的进项ID
                    //                       OutputID = notice.OutputID,//通知的销项ID
                    //                   };

                    //var old_notices = linq_notices.ToArray();
                    //notices = (from output in outputs
                    //           join notice in old_notices on output.OutputID equals notice.OutputID
                    //           where notice.OrderID == orderID
                    //           orderby output.ID
                    //           select new NoticeQuery
                    //           {
                    //               WaybillID = notice.WaybillID,
                    //               OrderID = notice.OrderID,
                    //               ID = notice.ID,
                    //               SendedQuantity = notice.SendedQuantity, //通知的数量
                    //               InputID = notice.InputID, //通知的进项ID
                    //               OutputID = notice.OutputID,//通知的销项ID
                    //               ItemID = output.ItemID
                    //           }).ToArray();
                    #endregion

                    #region 第二种写法：notice join output join waybill 试验效率
                    var linq_notices = from notice in pvwms.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                                       join output in pvwms.ReadTable<Layers.Data.Sqls.PvWms.Outputs>()
                                       on notice.OutputID equals output.ID
                                       join waybill in pvwms.ReadTable<Layers.Data.Sqls.PvWms.WaybillsOriginView>()
                                       on notice.WaybillID equals waybill.ID
                                       orderby waybill.ID descending
                                       where waybill.ConfirmReceiptStatus == 200
                                             && (waybill.OrderID == orderID || output.OrderID == orderID)//检查索引(经检查Output的OrderID索引建立不当:已解决)
                                             && waybill.Status == 200 //   正常出库 
                                             && waybill.ExcuteStatus == (int)CgPickingExcuteStatus.Completed //完成装运
                                             && notice.Type == (int)CgNoticeType.Out
                                             && (notice.Source == (int)CgNoticeSource.AgentSend
                                                 || notice.Source == (int)CgNoticeSource.Transfer
                                                 || notice.WareHouseID.StartsWith("SZ"))//检查索引
                                       select new NoticeQuery
                                       {
                                           WaybillID = waybill.ID,
                                           OrderID = waybill.OrderID,
                                           ID = notice.ID,
                                           SendedQuantity = notice.Quantity, //通知的数量
                                           InputID = notice.InputID, //通知的进项ID
                                           OutputID = notice.OutputID,//通知的销项ID
                                           ItemID = output.ItemID
                                       };
                    notices = linq_notices.ToArray();

                    #endregion

                    #region 第一种写法：因索引问题暂时放弃
                    //   //得到香港/深圳库房所有的确认收货的通知
                    //   var linq_notices = from waybill in pvwms.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>()
                    //                  join notice in pvwms.ReadTable<Layers.Data.Sqls.PvWms.Notices>() on waybill.wbID equals notice.WaybillID
                    //                  join output in pvwms.ReadTable<Layers.Data.Sqls.PvWms.Outputs>() on notice.OutputID equals output.ID
                    //                  orderby waybill.wbID descending
                    //                  where waybill.ConfirmReceiptStatus == 200
                    //                        && (waybill.OrderID == orderID || output.OrderID == orderID)//检查索引(经检查Output的OrderID索引建立不当)
                    //                        && waybill.wbStatus == 200 //   正常出库 
                    //                        && waybill.wbExcuteStatus == (int)CgPickingExcuteStatus.Completed //完成装运
                    //                        && waybill.NoticeType == (int)CgNoticeType.Out
                    //                        && (waybill.Source == (int)CgNoticeSource.AgentSend
                    //                            || waybill.Source == (int)CgNoticeSource.Transfer
                    //                            || notice.WareHouseID.StartsWith("SZ"))//检查索引
                    //                  select new NoticeQuery
                    //                  {
                    //                      WaybillID = waybill.wbID,
                    //                      OrderID  = waybill.OrderID,
                    //                      ID = notice.ID,
                    //                      SendedQuantity = notice.Quantity, //通知的数量
                    //                      InputID = notice.InputID, //通知的进项ID
                    //                      OutputID = notice.OutputID,//通知的销项ID
                    //                      ItemID = output.ItemID
                    //                  };

                    //notices = linq_notices.ToArray();
                    #endregion

                }
                catch (Exception ex)
                {
                    //控制台输出错误
                    Console.WriteLine((ex.InnerException ?? ex).Message);
                    //然后记录日志并进行下次操作
                    Logs.Log($"查询Notice集合信息：", ex.InnerException ?? ex);
                }
                var igroup_notice = (from item in notices.ToArray()
                                     group item by item.ItemID into groups
                                     orderby groups.Key ascending
                                     select new
                                     {
                                         ItemID = groups.Key,
                                         NoticeTotal = groups.Sum(g => g.SendedQuantity)
                                     }).ToArray();

                //获取订单数据
                var linq_orderItems = from item in pvwsorder.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItems>()
                                      where item.OrderID == orderID
                                      orderby item.ID ascending
                                      select new
                                      {
                                          item.ID,
                                          Quantity = item.Quantity,
                                          item.Type
                                      };

                var ienum_orderItems = linq_orderItems.ToArray();
                if (ienum_orderItems.Any(item => item.Type == 2))
                {
                    ienum_orderItems = ienum_orderItems.Where(item => item.Type == 2).ToArray();
                }

                if (igroup_notice.Length == ienum_orderItems.Length)
                {
                    if (ienum_orderItems.Select((item, index) =>
                    {
                        return item.Quantity == igroup_notice[index].NoticeTotal;
                    }).All(item => item))
                    {

                        try
                        {
                            //检查索引（理论上来说索引也并不合格）
                            pvcenter.Update<Layers.Data.Sqls.PvCenter.Logs_PvWsOrder>(new
                            {
                                IsCurrent = false
                            }, item => item.MainID == orderID && item.Type == (int)OrderStatusType.MainStatus);
                        }
                        catch (Exception ex)
                        {
                            //控制台输出错误
                            Console.WriteLine((ex.InnerException ?? ex).Message);
                            //然后记录日志并进行下次操作
                            Logs.Log($"修改Logs_PvWsOrder：item.MainID == {orderID} && item.Type == {(int)OrderStatusType.MainStatus})", ex.InnerException ?? ex);
                        }
                        try
                        {
                            //订单日志修改状态为  客户已收货
                            pvcenter.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder
                            {
                                ID = Guid.NewGuid().ToString(),
                                MainID = orderID,
                                Type = (int)OrderStatusType.MainStatus,
                                Status = (int)CgOrderStatus.客户已收货,
                                CreateDate = DateTime.Now,
                                CreatorID = adminID ?? Npc.Robot.Obtain(),//系统自动
                                IsCurrent = true
                            });
                        }
                        catch (Exception ex)
                        {

                            //控制台输出错误
                            Console.WriteLine((ex.InnerException ?? ex).Message);
                            //然后记录日志并进行下次操作
                            Logs.Log($"插入Logs_PvWsOrder：MainID == {orderID}", ex.InnerException ?? ex);
                        }
                    }


                }
            }
        }



        /// <summary>
        /// 把昨日产生的箱号删除掉(pickings里有的删除)
        /// </summary>
        /// <param name="waybillID"></param>
        internal void DeleteBox()
        {
            using (var repository = new PvWmsRepository())
            {
                var now = DateTime.Now.Date;
                Layers.Data.Sqls.PvWms.Boxes sqlb;
                Layers.Data.Sqls.PvWms.Pickings sqlp;

                repository.Command(string.Format($@"delete from {nameof(Layers.Data.Sqls.PvWms.Boxes)}  where {nameof(sqlb.CreateDate)} 
between '{{0}}' and  '{{1}}' and [Series] in  (SELECT distinct [BoxCode] FROM {nameof(Layers.Data.Sqls.PvWms.Pickings)}   where {nameof(sqlp.CreateDate)}  between '{{0}}' and  '{{1}}') ", now.AddDays(-1), now));

                //第二种处理：查出所有昨天的已经出库的箱号并且删除(容易掉到时间坑里去，把前天的删除而未把昨天的删除)
                //var boxcodes = repository.ReadTable<Layers.Data.Sqls.PvWms.Pickings>().Where(item => item.CreateDate >= DateTime.Now.AddDays(-1) && item.CreateDate < DateTime.Now).Select(item => item.BoxCode).Distinct();
                //repository.Delete<Layers.Data.Sqls.PvWms.Boxes>(item => boxcodes.Contains(item.Series) && item.CreateDate >= DateTime.Now.AddDays(-1) && item.CreateDate < DateTime.Now);
            }
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="ex"></param>


        //void CheckStatus(Object stateInfo)
        //{
        //    AutoResetEvent autoEvent = (AutoResetEvent)stateInfo;

        //    autoEvent.Set();
        //}
    }

    internal class NoticeQuery
    {
        public string WaybillID { get; set; }

        public string OrderID { get; set; }

        public string ID { get; set; }

        public decimal SendedQuantity { get; set; }
        public string InputID { get; set; }
        public string OutputID { get; set; }
        public string ItemID { get; set; }
    }
    internal class Logs
    {
        internal static void Log(Exception ex)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "applog", DateTime.Now.ToString("yyyyMMdd") + ".txt");

            var fileInfo = new FileInfo(path);
            if (!fileInfo.Directory.Exists)
            {
                fileInfo.Directory.Create();
            }

            using (var stream = fileInfo.Open(FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
            {
                writer.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                writer.Write('：');
                writer.Write(ex.Message);
                writer.WriteLine();
                writer.Write("StackTrace：");
                writer.WriteLine(ex.StackTrace);
                writer.WriteLine();
                writer.Write("Source：");
                writer.WriteLine(ex.Source);
                writer.WriteLine();
                writer.Close();
            }
        }

        internal static void Log(string msg, Exception ex)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "applog", DateTime.Now.ToString("yyyyMMdd") + ".txt");

            var fileInfo = new FileInfo(path);
            if (!fileInfo.Directory.Exists)
            {
                fileInfo.Directory.Create();
            }

            using (var stream = fileInfo.Open(FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
            {
                writer.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                writer.Write('：');
                writer.Write(ex.Message);
                writer.WriteLine();
                writer.Write("msg:");
                writer.WriteLine(msg);
                writer.WriteLine();
                writer.Write("StackTrace：");
                writer.WriteLine(ex.StackTrace);
                writer.WriteLine();
                writer.Write("Source：");
                writer.WriteLine(ex.Source);
                writer.WriteLine();
                writer.Close();
            }
        }
    }
}
