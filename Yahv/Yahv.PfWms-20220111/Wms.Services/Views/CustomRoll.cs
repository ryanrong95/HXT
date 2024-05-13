using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Wms.Services.Models;
using Yahv.Services.Enums;
using Yahv.Utils.Serializers;
using Yahv.Services.Models;

namespace Wms.Services.Views
{
    public class CustomRoll : QueryView<Models.DataCustomTransport, PvWmsRepository>
    {
        public CustomRoll()
        {

        }

        public CustomRoll(PvWmsRepository reponsitory) : base(reponsitory)
        {

        }

        public CustomRoll(PvWmsRepository reponsitory, IQueryable<DataCustomTransport> iQueryable) : base(reponsitory, iQueryable)
        {

        }
        protected override IQueryable<DataCustomTransport> GetIQueryable()
        {

            return from waybill in new Yahv.Services.Views.WaybillsTopView<PvWmsRepository>(this.Reponsitory)
                   orderby waybill.CreateDate descending
                   group new { waybill.ID, waybill.Type, waybill.CarrierID, waybill.CarrierName, waybill.Status, waybill.ExcuteStatus, waybill.CuttingOrderStatus, waybill.WayChcd.LotNumber, waybill.WayChcd.CarNumber1, waybill.WayChcd.CarNumber2, waybill.WayChcd.Carload, waybill.WayChcd.IsOnevehicle, waybill.WayChcd.Driver, waybill.WayChcd.Phone, waybill.WayChcd.PlanDate, waybill.WayChcd.DepartDate, waybill.WayChcd.TotalQuantity } by waybill.WayChcd.LotNumber into waybills
                   select new Models.DataCustomTransport
                   {
                       WaybillID = waybills.FirstOrDefault().ID,
                       WaybillType = waybills.FirstOrDefault().Type,
                       CarrierID = waybills.FirstOrDefault().CarrierID,
                       CarrierName = waybills.FirstOrDefault().CarrierName,
                       Status = waybills.FirstOrDefault().Status,
                       CuttingOrderStatus = (Yahv.Underly.CgCuttingOrderStatus)waybills.FirstOrDefault().CuttingOrderStatus,
                       LotNumber = waybills.FirstOrDefault().LotNumber,
                       CarNumber1 = waybills.FirstOrDefault().CarNumber1,
                       CarNumber2 = waybills.FirstOrDefault().CarNumber2,
                       Carload = waybills.FirstOrDefault().Carload,
                       IsOnevehicle = waybills.FirstOrDefault().IsOnevehicle,
                       Driver = waybills.FirstOrDefault().Driver,
                       Phone = waybills.FirstOrDefault().Phone,
                       PlanDate = waybills.FirstOrDefault().PlanDate,
                       DepartDate = waybills.FirstOrDefault().DepartDate,
                       TotalQuantity = waybills.FirstOrDefault().TotalQuantity,
                   };
        }


        public CustomRoll SearchByNoticeType(NoticeType type)
        {
            var waybillIDView = from notice in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                                where (int)type == notice.Type && notice.Status != (int)NoticesStatus.Closed
                                select notice.WaybillID;

            var dist = waybillIDView.Distinct();

            var iQuery = from waybill in this.IQueryable
                         join id in dist on waybill.WaybillID equals id
                         select waybill;

            var roll = new CustomRoll(this.Reponsitory, iQuery);

            return roll;
        }

        /// <summary>
        /// 根据库房编号查询运单
        /// </summary>
        /// <param name="warehouseID"></param>
        /// <returns></returns>
        public CustomRoll SearchByWarehouseID(string warehouseID)
        {
            var waybillIDView = from notice in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                                where notice.WareHouseID == warehouseID
                                select notice.WaybillID;

            var topN = waybillIDView.Distinct().OrderBy(item => item).Take(500);
            var iQuery = from waybill in this.IQueryable
                         where topN.Contains(waybill.WaybillID)
                         select waybill;

            return new CustomRoll(this.Reponsitory, iQuery);
        }

        public CustomRoll SearchByLotnumber(string lotnumber)
        {
            var iQuery = from waybill in this.IQueryable
                         where waybill.LotNumber.Contains(lotnumber)
                         select waybill;

            return new CustomRoll(this.Reponsitory, iQuery);
        }

        public CustomRoll SearchByCarrierID(string carreierID)
        {
            var iQuery = from waybill in this.IQueryable
                         where waybill.CarrierID == carreierID
                         select waybill;

            return new CustomRoll(this.Reponsitory, iQuery);
        }
        public CustomRoll SearchByCarNumber(string carNumber)
        {
            var iQuery = from waybill in this.IQueryable
                         where waybill.CarNumber1 == carNumber || waybill.CarNumber2 == carNumber
                         select waybill;

            return new CustomRoll(this.Reponsitory, iQuery);
        }

        public CustomRoll SearchByCuttingOrderStatus(Yahv.Underly.CgCuttingOrderStatus status)
        {
            var iQuery = from waybill in this.IQueryable
                         where waybill.CuttingOrderStatus == status
                         select waybill;

            if (status == Yahv.Underly.CgCuttingOrderStatus.Waiting)
            {
                iQuery = from waybill in this.IQueryable
                         where waybill.CuttingOrderStatus == Yahv.Underly.CgCuttingOrderStatus.Waiting || waybill.CuttingOrderStatus == null
                         select waybill;
            }

            //CuttingOrderStatus? status

            return new CustomRoll(this.Reponsitory, iQuery);
        }

        public object OutputDetail(string warehouseID, string lotnumber)
        {
            using (var rep = new PvWmsRepository())
            {
                //根据库房编号和运输批次号查出来所有运单
                var iQuery = from waybill in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>()
                             join notice in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                             on waybill.wbID equals notice.WaybillID
                             where waybill.chcdLotNumber.Contains(lotnumber) && notice.WareHouseID == warehouseID
                             select waybill.wbID;
                var waybillIDs = iQuery.ToArray();

                #region 第一种写法
                //var noticesView = new BoxesPickingNoticesView().Where(item => waybillIDs.Contains(item.WaybillID));

                //var notices = noticesView.ToArray();

                //var pickingNotices = new PickingNoticesView().Where(item => waybillIDs.Contains(item.WaybillID));

                ////var totalMoney = notices.Sum(item => item.Order.Select(tem => tem.PickingNotice.Select(te => te.Output.Price * te.Picking.Quantity)));

                //var totalMoney = pickingNotices.Sum(item => item.Output.Price * item.Picking.Quantity);
                //var totalWeight = pickingNotices.Sum(item => item.Picking.Weight);

                //var data = from waybill in new Yahv.Services.Views.WaybillsTopView<PvWmsRepository>(this.Reponsitory)
                //           where waybillIDs.Contains(waybill.ID)
                //           select new Models.DataCustomTransport
                //           {
                //               Notices = notices,
                //               WaybillID = waybill.ID,
                //               WaybillType = waybill.Type,
                //               CarrierID = waybill.CarrierID,
                //               CarrierName = waybill.CarrierName,
                //               Status = waybill.Status,
                //               CuttingOrderStatus = (Yahv.Underly.CuttingOrderStatus)waybill.CuttingOrderStatus,
                //               LotNumber = waybill.WayChcd.LotNumber,
                //               CarNumber1 = waybill.WayChcd.CarNumber1,
                //               CarNumber2 = waybill.WayChcd.CarNumber2,
                //               Carload = waybill.WayChcd.Carload,
                //               IsOnevehicle = waybill.WayChcd.IsOnevehicle,
                //               Driver = waybill.WayChcd.Driver,
                //               Phone = waybill.WayChcd.Phone,
                //               PlanDate = waybill.WayChcd.PlanDate,
                //               DepartDate = waybill.WayChcd.DepartDate,
                //               TotalQuantity = waybill.WayChcd.TotalQuantity,
                //               TotalMoney = totalMoney,
                //               TotalWeight = totalWeight,
                //               BoxNumber = notices.Count()
                //           };
                //return data.FirstOrDefault();

                #endregion

                #region 第二种写法

                //根据运单编号查出对应的出库通知
                var noticesView = new CustomPickingNoticeView().Where(item => waybillIDs.Contains(item.WaybillID)).ToArray();

                var boxes = new BoxesView().ToArray();

                //查出出库通知对应的所有小订单
                var tinyOrderIDs = noticesView.Select(item => item.Output.TinyOrderID).Distinct().ToArray();

                //查出出库通知对应的所有箱号
                var AllboxCodes = noticesView.Select(item => item.BoxCode).Distinct().ToArray();

                //客户信息
                var linqClients = from client in rep.ReadTable<Layers.Data.Sqls.PvWms.ClientsTopView>()
                                  select new
                                  {
                                      client.ID,
                                      client.Name,
                                      client.EnterCode,
                                  };
                var clients = linqClients.ToArray();

                var inputIDs = noticesView.Select(item => item.InputID).Distinct().ToArray();
                var inputs = rep.ReadTable<Layers.Data.Sqls.PvWms.Inputs>().Where(item => inputIDs.Contains(item.ID)).ToArray();

                var noticeList = new List<BoxesPickingNotices>();
                var boxList = new List<BoxesNotices>();
                //var orderList = new List<BoxOrder>();
                string clientID = null, clientName = null;
                foreach (var tinyorderID in tinyOrderIDs)
                {
                    //查出小订单共对应的通知的箱号
                    var boxCodes = noticesView.Where(item => item.Output.TinyOrderID == tinyorderID).Select(item => item.BoxCode).Distinct().ToArray();
                    foreach (var boxCode in boxCodes)
                    {
                        //箱号信息
                        var box = boxes.Where(item => item.Code == boxCode).FirstOrDefault();
                        //查出箱号对应的出库通知
                        var pickingNotices = noticesView.Where(item => item.BoxCode == boxCode);
                        ////查出箱号对应的订单号
                        //var orderIDs = notices.Select(item => item.Output.OrderID).Distinct();
                        //查出箱号对应的小订单号（一个箱号只能有一个小订单号）
                        //tinyOrderID = notices.Select(item => item.Output.TinyOrderID).Distinct().FirstOrDefault();

                        //foreach (var orderID in orderIDs)
                        //{
                        //    var pickingNotices = noticesView.Where(item => item.Output.OrderID == orderID);
                        //    var inputID = pickingNotices.Select(item => item.InputID).FirstOrDefault();
                        //    clientID = inputs.Where(item => item.ID == inputID).FirstOrDefault().ClientID;
                        //    clientName = clients.Where(item => item.ID == clientID).FirstOrDefault()?.Name;

                        //    orderList.Add(new BoxOrder
                        //    {
                        //        //OrderID = orderID,
                        //        PickingNotice = pickingNotices.ToArray()
                        //    });
                        //}

                        boxList.Add(new BoxesNotices
                        {
                            ClientID = clientID,
                            ClientName = clientName,
                            Boxes = box,
                            PickingNotice = pickingNotices.ToArray()
                            //Order = orderList.ToArray()
                        });

                        //orderList = new List<BoxOrder>();
                    }
                    noticeList.Add(new BoxesPickingNotices
                    {
                        TinyOrderID = tinyorderID,
                        BoxesNotices = boxList.ToArray()
                    });
                    boxList = new List<BoxesNotices>();
                }

                var totalMoney = noticesView.Sum(item => item.Output.Price * item.Quantity);
                var totalWeight = noticesView.Sum(item => item.Weight);
                var boxNumber = AllboxCodes.Count();
                var data = from waybill in new Yahv.Services.Views.WaybillsTopView<PvWmsRepository>(this.Reponsitory)
                           where waybillIDs.Contains(waybill.ID)
                           select new Models.DataCustomTransport
                           {

                               Notices = noticeList.ToArray(),
                               WaybillID = waybill.ID,
                               WaybillType = waybill.Type,
                               CarrierID = waybill.CarrierID,
                               CarrierName = waybill.CarrierName,
                               Status = waybill.Status,
                               PickingExcuteStatus = (Yahv.Underly.PickingExcuteStatus)waybill.ExcuteStatus,
                               CuttingOrderStatus = (Yahv.Underly.CgCuttingOrderStatus)waybill.CuttingOrderStatus,
                               LotNumber = waybill.WayChcd.LotNumber,
                               CarNumber1 = waybill.WayChcd.CarNumber1,
                               CarNumber2 = waybill.WayChcd.CarNumber2,
                               Carload = waybill.WayChcd.Carload,
                               IsOnevehicle = waybill.WayChcd.IsOnevehicle,
                               Driver = waybill.WayChcd.Driver,
                               Phone = waybill.WayChcd.Phone,
                               PlanDate = waybill.WayChcd.PlanDate,
                               DepartDate = waybill.WayChcd.DepartDate,
                               TotalQuantity = waybill.WayChcd.TotalQuantity,
                               TotalMoney = totalMoney,
                               TotalWeight = totalWeight,
                               BoxNumber = boxNumber
                           };
                return data.FirstOrDefault();

                #endregion


            }

        }


        public object EnterDetail(string warehouseID, string lotnumber)
        {
            using (var rep = new PvWmsRepository())
            {
                var iQuery = from waybill in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>()
                             join notice in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                             on waybill.wbID equals notice.WaybillID
                             where waybill.chcdLotNumber.Contains(lotnumber) && notice.WareHouseID == warehouseID
                             select waybill.wbID;
                var waybillIDs = iQuery.ToArray();
                var noticesView = new SortingNoticesView().Where(item => waybillIDs.Contains(item.WaybillID)).ToArray();

                var linqClients = from client in rep.ReadTable<Layers.Data.Sqls.PvWms.ClientsTopView>()
                                  select new
                                  {
                                      client.ID,
                                      client.Name,
                                      client.EnterCode,
                                  };

                var clients = linqClients.ToArray();

                foreach (var item in noticesView)
                {
                    item.Input.ClientName = clients.Where(tem => tem.ID == item.Input.ClientID).FirstOrDefault().Name;
                }

                var totalWeight = noticesView.Sum(item => item.Sorting.Weight);
                var data = from waybill in new Yahv.Services.Views.WaybillsTopView<PvWmsRepository>(this.Reponsitory)
                           where waybillIDs.Contains(waybill.ID)
                           select new Models.DataCustomTransport
                           {
                               SortingNotices = noticesView,
                               WaybillID = waybill.ID,
                               WaybillType = waybill.Type,
                               CarrierID = waybill.CarrierID,
                               CarrierName = waybill.CarrierName,
                               Status = waybill.Status,
                               //CuttingOrderStatus = (Yahv.Underly.CuttingOrderStatus)waybill.CuttingOrderStatus,
                               LotNumber = waybill.WayChcd.LotNumber,
                               CarNumber1 = waybill.WayChcd.CarNumber1,
                               CarNumber2 = waybill.WayChcd.CarNumber2,
                               Carload = waybill.WayChcd.Carload,
                               IsOnevehicle = waybill.WayChcd.IsOnevehicle,
                               Driver = waybill.WayChcd.Driver,
                               Phone = waybill.WayChcd.Phone,
                               PlanDate = waybill.WayChcd.PlanDate,
                               DepartDate = waybill.WayChcd.DepartDate,
                               TotalQuantity = waybill.WayChcd.TotalQuantity,
                               TotalWeight = totalWeight,
                           };
                return data.FirstOrDefault();
            }

        }

    }


    static public class CustomExtends
    {

        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="iquery"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        static public object ToPage(this IQueryable<DataCustomTransport> iquery, int pageIndex = 1, int pageSize = 20)
        {

            int total = iquery.Count();
            var query = iquery.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToArray();
            //var lotnumbers = query.Select(item => item.LotNumber).Distinct();//取出所有运输批次号

            foreach (var custom in query)
            {
                //查出对应的运单
                var waybills = new ServicesWaybillsTopView().Where(tem => tem.WayChcd.LotNumber == custom.LotNumber).ToArray();

                //查出运输批次号对应的运单
                var waybillIDs = waybills.Select(tem => tem.ID).ToArray().Distinct();
                    
                var noticesView = new PvWmsRepository().ReadTable<Layers.Data.Sqls.PvWms.Notices>().Where(tem => waybillIDs.Contains(tem.WaybillID)).ToArray();//查出运单下对应的通知

                var type = noticesView.Select(tem => tem.Type).FirstOrDefault();
                var boxes = noticesView.Select(tem => tem.BoxCode).Distinct().ToArray();//查出通知下的所有箱号
                custom.BoxNumber = boxes.Count();//总箱数

                int count = 0;
                //if (type != 315)
                //{
                foreach (var boxCode in boxes)
                {
                    //获得一条通知，箱子里可能有好几条通知的信息，但只会对应一个库存信息，所以只需获得一条通知，看是否已上架即可
                    var notice = new SortingNoticesView().Where(item => item.BoxCode == boxCode).FirstOrDefault();

                    //根据通知的库存ID查询是否上架
                    var storage = new PvWmsRepository().ReadTable<Layers.Data.Sqls.PvWms.Storages>().Where(tem => tem.ID == notice.Storage.ID).FirstOrDefault();

                    if (!string.IsNullOrWhiteSpace(storage.ShelveID))
                    {
                        count += 1;
                    }
                }
                //}
                custom.UpperNumber = count;

                //var noticeIDs = noticesView.Select(item => item.ID).ToArray();
                //已上架箱数
                //custom.UpperNumber = new PvWmsRepository().ReadTable<Layers.Data.Sqls.PvWms.Storages>().Where(item => noticeIDs.Contains(item.NoticeID)).Where(item => item.ShelveID != null || item.ShelveID.Trim() != "").Count();//根据通知ID去查所有对应的库存信息，然后去查库存里shelveID不是空的个数就是已上架的箱数
            }

            return new
            {
                Total = total,
                Size = pageSize,
                Index = pageIndex,
                Data = query.Select(item => (DataCustomTransport)item).ToArray()
            };
        }

        //    /// <summary>
        //    /// 报关运输详情页
        //    /// </summary>
        //    /// <param name="iquery"></param>
        //    /// <param name="rep"></param>
        //    /// <returns></returns>
        //    static public DataCustomTransport[] ToFillArray(this IQueryable<DataCustomTransport> iquery, PvWmsRepository rep = null)
        //    {
        //        var reponsitory = rep ?? new PvWmsRepository();
        //        try
        //        {
        //            var arry = iquery.ToArray();

        //            var wblsID = arry.Select(item => item.WaybillID).Distinct();

        //            var customTransportsView = new Custom(reponsitory);
        //            var noticesView = new PickingNoticesView().Where(item => wblsID.Contains(item.WaybillID));

        //            var notices = noticesView.ToArray();


        //            var arrys = arry.Select(custom =>
        //             {
        //                 custom.Notices = notices;
        //                 custom.BoxNumber = notices.Count();
        //                 custom.TotalMoney = notices.Sum(item => item.Output.Price * item.Picking.Quantity);
        //                 custom.TotalWeight = notices.Sum(item => item.Picking.Weight);

        //                 return custom;
        //             }).ToArray();

        //            return arrys;
        //        }
        //        catch (Exception ex)
        //        {
        //            return null;
        //        }
        //        finally
        //        {
        //            if (rep == null)
        //            {
        //                reponsitory.Dispose();
        //            }
        //        }
        //    }
        //}

    }

}
