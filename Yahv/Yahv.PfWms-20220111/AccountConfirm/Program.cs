using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace AccountConfirm
{
    /// <summary>
    /// 待出账改为待确认
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            var MinutesExpired =int.Parse(System.Configuration.ConfigurationManager.AppSettings["MinutesExpired"]?.ToString()??"240");
            using (var rep = new PvCenterReponsitory())
            {
                while (true)
                {
                    try
                    {
                        var listSortingIDs =new  List<paras>() ;
                        var listPaickingIDs = new List<paras>();
                        //获取待出账的订单
                        var orders = new Yahv.Services.Views.WsOrdersTopView<PvWsOrderReponsitory>().Where(item => item.PaymentStatus == OrderPaymentStatus.Waiting).ToList();
                        if (orders.Count > 0)
                        {
                            listSortingIDs.AddRange(orders.Select(item => new paras { WaybillID = item.Input.WayBillID, OrderID = item.ID }));
                            listPaickingIDs.AddRange(orders.Select(item => new paras { WaybillID = item.Output.WayBillID, OrderID = item.ID }));
                        }                      


                        foreach (var id in listSortingIDs)
                        {
                            try
                            {
                                //var linq = from sorting in rep.ReadTable<Layers.Data.Sqls.PvWms.Sortings>()
                                //           join notice in rep.ReadTable<Layers.Data.Sqls.PvWms.Notices>() on sorting.NoticeID equals notice.ID
                                //           where notice.WaybillID == id.WaybillID && sorting.CreateDate.AddMinutes(MinutesExpired) < DateTime.Now
                                //           select new { notice.ID };

                                //修改为全部入库完成
                                var linq = from waybill in rep.ReadTable<Layers.Data.Sqls.PvCenter.Waybills>()
                                           where waybill.ID == id.WaybillID && waybill.ExcuteStatus == (int)SortingExcuteStatus.Stocked
                                           select new { waybill.ID };


                                //有入库信息了
                                if (linq.FirstOrDefault() != null)
                                {
                                    //修改订单状态为待确认
                                    AddOrderLogs(id);
                                    Console.WriteLine($"{id.OrderID},修改完成!\t" + DateTime.Now);

                                }

                                
                            }
                            catch(Exception ex)
                            {
                                Console.WriteLine($"{id.OrderID},修改失败!\t{ex.Message}\t" + DateTime.Now);
                            }
                        }


                        foreach (var id in listPaickingIDs)
                        {
                            try
                            {
                                //var linq = from picking in rep.ReadTable<Layers.Data.Sqls.PvWms.Pickings>()
                                //           join notice in rep.ReadTable<Layers.Data.Sqls.PvWms.Notices>() on picking.NoticeID equals notice.ID
                                //           where notice.WaybillID == id.WaybillID && picking.CreateDate.AddMinutes(MinutesExpired) < DateTime.Now
                                //           select new { notice.ID };

                                //修改为全部入库完成
                                var linq = from waybill in rep.ReadTable<Layers.Data.Sqls.PvCenter.Waybills>()
                                           where waybill.ID == id.WaybillID && waybill.ExcuteStatus == (int)PickingExcuteStatus.OutStock
                                           select new { waybill.ID };

                                //有出库信息了
                                if (linq.FirstOrDefault() != null)
                                {
                                    //修改订单状态为待确认
                                    AddOrderLogs(id);
                                    Console.WriteLine($"{id.OrderID},修改完成!\t" + DateTime.Now);
                                }

                               
                            }
                            catch(Exception ex)
                            {
                                Console.WriteLine($"{id.OrderID},修改失败!\t{ex.Message}\t" + DateTime.Now);
                            }
                        }


                       

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(string.Concat(ex.Message, "\t", DateTime.Now));
                    }
                    System.Threading.Thread.Sleep(5000);
                    Console.WriteLine("没有任务,5秒后重新开始\t" + DateTime.Now);
                }

            }
        }

        private static void AddOrderLogs(paras id)
        {
            new Yahv.Services.Models.Logs_PvWsOrder
            {
                MainID = id.OrderID,
                CreatorID = "NPC",
                Type = OrderStatusType.PaymentStatus,
                Status = (int)OrderPaymentStatus.Confirm
            }.Enter();
        }
    }

    public class paras
    {
        public string WaybillID { get; set; }
        public string OrderID { get; set; }
    }
}
