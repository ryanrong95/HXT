//using Layers.Data;
//using Layers.Data.Sqls;
//using Layers.Data.Sqls.PvWms;
//using System;
//using System.Collections.Generic;
//using System.Collections.Specialized;
//using System.Drawing;
//using System.Linq;
//using System.Net;
//using System.Text;
//using System.Threading.Tasks;
//using System.Transactions;
//using Wms.Services;
//using Wms.Services.Enums;
//using Wms.Services.Extends;
//using Wms.Services.Models;
//using Wms.Services.Views;
//using Yahv.Services.Enums;
//using Yahv.Services.Extends;
//using Yahv.Services.Models;
//using Yahv.Underly;
//using Yahv.Underly.Enums;
//using Yahv.Underly.Erps;
//using Yahv.Usually;
//using Yahv.Utils.Converters.Contents;
//using Yahv.Utils.Serializers;

//namespace Wms.Services
//{
//    public class WayBillServices
//    {

//        public WayBillServices()
//        {

//        }

//        IErpAdmin admin;

//        public WayBillServices(IErpAdmin admin)
//        {
//            this.admin = admin;
//        }

//        public static object locksortingobj = new object();

//        public object WayBills(string warehouseID,
//            string key = null,
//            string partnumber = null,
//            string supplier = null,
//            string startdate = null,
//            string enddate = null,
//            int? status = null,
//            int? source = null,
//            int? ntype = null,
//            int pageindex = 1,
//            int pagesize = 20)
//        {

//            //Request.To
//            //NameValueCollection


//#if DEBUG
//            lock (locksortingobj)
//            {
//#endif

//                using (PvWmsRepository repository = new PvWmsRepository())
//                {
//                    var roll = new SortingsWayBillRoll(repository);
//                    if (ntype != null && ntype.ToString().Trim() != "")
//                    {
//                        roll = roll.SearchByNoticeType((NoticeType)ntype);
//                    }
//                    else
//                    {
//                        roll = roll.SearchByNoticeType(NoticeType.CustomsEnter, NoticeType.GiftEnter, NoticeType.PurchasingEnter, NoticeType.ReceiptEnter, NoticeType.ReplenishmentEnter, NoticeType.ReturnEnter, NoticeType.StockUpEnter, NoticeType.TransferEnter);
//                    }

//                    roll = roll.SearchByDataStatus(GeneralStatus.Normal);
//                    if (!string.IsNullOrWhiteSpace(warehouseID))
//                    {
//                        roll = roll.SearchByWareHouseID(warehouseID);
//                        //var view  = roll.Where(item=>item)
//                    }

//                    if (!string.IsNullOrEmpty(supplier))
//                    {
//                        //roll = roll.SearchBySupplier(supplier);

//                        var view = roll.Where(item => item.Supplier.Contains(supplier));
//                        roll = new SortingsWayBillRoll(repository, view);
//                    }

//                    DateTime startDate;
//                    if (DateTime.TryParse(startdate, out startDate))
//                    {
//                        var view = roll.Where(item => item.CreateDate >= startDate);
//                        roll = new SortingsWayBillRoll(repository, view);
//                    }
//                    DateTime endDate;
//                    if (DateTime.TryParse(enddate, out endDate))
//                    {
//                        var view = roll.Where(item => item.CreateDate < endDate.AddDays(1));
//                        roll = new SortingsWayBillRoll(repository, view);
//                    }

//                    if (!string.IsNullOrEmpty(key))
//                    {
//                        roll = new SortingsWayBillRoll(repository, roll).SearchByID(key);
//                    }
//                    if (!string.IsNullOrEmpty(partnumber))
//                    {
//                        roll = new SortingsWayBillRoll(repository, roll).SearchByPartNumber(partnumber);
//                    }

//                    if (source.HasValue && source != 0)
//                    {
//                        roll = new SortingsWayBillRoll(repository, roll).SearchBySource((NoticeSource)source);
//                    }


//                    if (status != null && status.ToString().Trim() != "")
//                    {
//                        if (status != 0)
//                        {
//                            roll = new SortingsWayBillRoll(repository, roll).SearchByStatus((SortingExcuteStatus)status);
//                        }
//                    }
//                    else
//                    {
//                        roll = new SortingsWayBillRoll(repository, roll).SearchByStatus(SortingExcuteStatus.Anomalous, SortingExcuteStatus.PartStocked, SortingExcuteStatus.PendingStorage, SortingExcuteStatus.Sorting, SortingExcuteStatus.Taking, SortingExcuteStatus.Testing, SortingExcuteStatus.WaitTake);
//                    }


//                    return roll.ToPage(pageindex, pagesize);

//                }
//#if DEBUG
//            }
//#endif

//        }


//        public object BoxProducts(string whid, int all, int status = 0, string key = null, int pageindex = 1, int pagesize = 20)
//        {
//            var roll = new BoxProductsRoll(this.admin);

//            if (!string.IsNullOrEmpty(whid))
//            {
//                roll = roll.SearchByWarehouseID(whid);
//            }

//            if (all == 0)
//            {
//                roll = roll.SearchByAdmins(this.admin.ID);
//            }
//            if (status != 0)
//            {
//                roll = roll.SearchByStatus(status);
//            }

//            if (!string.IsNullOrEmpty(key))
//            {
//                roll = roll.SearchBykey(key);
//            }



//            return roll.ToPage(whid);
//        }


//        public object TinyOrderProducts(string whid, int all, int status = 0, string key = null, int pageindex = 1, int pagesize = 20)
//        {
//            var roll = new BoxProductsRoll(this.admin);

//            if (!string.IsNullOrEmpty(whid))
//            {
//                roll = roll.SearchByWarehouseID(whid);
//            }

//            if (all == 0)
//            {
//                roll = roll.SearchByAdmins(this.admin.ID);
//            }
//            if (status != 0)
//            {
//                roll = roll.SearchByStatus(status);
//            }

//            if (!string.IsNullOrEmpty(key))
//            {
//                roll = roll.SearchBykey(key);
//            }



//            return roll.ToPage(whid);
//        }


//        public List<DeclarationApplyItem> BoxdeClareCustomApply(string whid, string[] boxids)
//        {
//            var roll = new BoxProductsRoll(this.admin);

//            if (boxids != null && boxids.Length > 0)
//            {
//                roll = roll.SearchByBoxIDs(boxids);
//            }
//            var d = roll.ToData(whid, 1, 10000);
//            List<DeclarationApplyItem> list = new List<DeclarationApplyItem>();
//            var data = d.SelectMany(item => item.Notices);
//            foreach (var item in data)
//            {
//                list.Add(new DeclarationApplyItem { OrderItemID = item.Input.ItemID, VastOrderID = item.Input.OrderID, TinyOrderID = item.Input.TinyOrderID, SortingID = item.Sorting?.ID ?? "" });
//            }

//            return list;
//        }


//        public void ChangeBoxCode(string whid, string oldCode, string newCode)
//        {
//            using (PvWmsRepository rep = new PvWmsRepository())
//            {
//                var notices = rep.ReadTable<Layers.Data.Sqls.PvWms.Notices>().Where(item => item.WareHouseID == whid && item.BoxCode == oldCode).Select(item => new { item.ID, item.InputID }).ToArray();
//                var ids = notices.Select(item => item.ID);
//                var inputids = notices.Select(item => item.InputID);
//                var sortingids = rep.ReadTable<Layers.Data.Sqls.PvWms.Sortings>().Where(item => ids.Contains(item.NoticeID) && inputids.Contains(item.InputID)).Select(item => item.ID).ToArray();

//                rep.Update<Notices>(new { BoxCode = newCode }, item => ids.Contains(item.ID));
//                rep.Update<Sortings>(new { BoxCode = newCode }, item => sortingids.Contains(item.ID));
//            }
//        }


//        public SortingWaybill WayBill(string warehouseID, string waybillid, string key = null)
//        {
//            using (PvWmsRepository rep = new PvWmsRepository())
//            {
//                var roll = new SortingsWayBillRoll().SearchByDataStatus(GeneralStatus.Normal);
//                if (!string.IsNullOrEmpty(warehouseID))
//                {
//                    roll = roll.SearchByWareHouseID(warehouseID);
//                }


//                var data = roll.SearchByWaybillD(waybillid).ToFillArray();
//                //var data = new SortingsWayBillRoll(rep, roll).SearchByWaybillD(waybillid).ToFillArray();
//                if (!string.IsNullOrEmpty(key))
//                {
//                    return new SortingsWayBillRoll(rep, roll).SearchByPartNumberOrManufacturer(data.First(), key);
//                }
//                var d = data.FirstOrDefault();
//                if (d != null)
//                {

//                    d.Source = d.Notices.First().Source;
//                }
//                return d;
//            }
//        }

//        public static object obj = new object();

//        public object PickWayBills(string warehouseID,
//           string key = null,
//           string partnumber = null,
//           string CustomName = null,
//           string startdate = null,
//           string enddate = null,
//           string status = null,
//           int? source = null,
//           int? ntype = null,
//           int pageindex = 1,
//           int pagesize = 20)
//        {

//            //            //Request.To
//            //            //NameValueCollection

//            //#if DEBUG
//            //            lock (obj)
//            //            {
//            //#endif


//            //                using (PvWmsRepository repository = new PvWmsRepository())
//            //                {

//            //                    var roll = new PickingWaybillRoll(repository);


//            //                    if (ntype != null && ntype.ToString().Trim() != "")
//            //                    {
//            //                        roll = roll.SearchByNoticeType((NoticeType)ntype);
//            //                    }
//            //                    else
//            //                    {
//            //                        roll = roll.SearchByNoticeType(NoticeType.CustomsOut, NoticeType.GiftOut, NoticeType.ReplenishmentOut, NoticeType.SendOut, NoticeType.SaleOut, NoticeType.TransferOut);
//            //                    }


//            //                    roll = roll.SearchByDataStatus(GeneralStatus.Normal);
//            //                    if (!string.IsNullOrWhiteSpace(warehouseID))
//            //                    {
//            //                        roll = roll.SearchByWareHouseID(warehouseID);
//            //                        //var view  = roll.Where(item=>item)
//            //                    }

//            //                    if (!string.IsNullOrEmpty(CustomName))
//            //                    {

//            //                        var view = roll.Where(item => item.ClientName.Contains(CustomName));
//            //                        roll = new PickingWaybillRoll(repository, view);
//            //                    }

//            //                    DateTime startDate;
//            //                    if (DateTime.TryParse(startdate, out startDate))
//            //                    {
//            //                        roll = roll.SearchByStartDate(startDate);
//            //                    }
//            //                    DateTime endDate;
//            //                    if (DateTime.TryParse(enddate, out endDate))
//            //                    {
//            //                        roll = roll.SearchByEndDate(endDate);
//            //                    }

//            //                    if (!string.IsNullOrEmpty(key))
//            //                    {
//            //                        roll = new PickingWaybillRoll(repository, roll).SearchByID(key);
//            //                    }
//            //                    if (!string.IsNullOrEmpty(partnumber))
//            //                    {
//            //                        roll = new PickingWaybillRoll(repository, roll).SearchByPartNumber(partnumber);
//            //                    }

//            //                    if (source.HasValue && source != 0)
//            //                    {
//            //                        roll = new PickingWaybillRoll(repository, roll).SearcBySource((NoticeSource)source);
//            //                    }

//            //                    if (status != null && status.ToString().Trim() != "")
//            //                    {
//            //                        if (status == "0")
//            //                        {
//            //                            roll = new PickingWaybillRoll(repository, roll).SearchByStatus(PickingExcuteStatus.Anomalous, PickingExcuteStatus.Picking, PickingExcuteStatus.WaitDelivery, PickingExcuteStatus.Waiting);
//            //                        }
//            //                        else
//            //                        {
//            //                            var es = new List<PickingExcuteStatus>();
//            //                            PickingExcuteStatus sta = PickingExcuteStatus.Anomalous;
//            //                            foreach (var item in status.Split(','))
//            //                            {
//            //                                if (Enum.TryParse(item, out sta))
//            //                                {
//            //                                    es.Add(sta);
//            //                                }
//            //                            }

//            //                            roll = new PickingWaybillRoll(repository, roll).SearchByStatus(es.ToArray());
//            //                        }
//            //                    }
//            //                    else
//            //                    {
//            //                        roll = new PickingWaybillRoll(repository, roll).SearchByStatus(PickingExcuteStatus.Anomalous, PickingExcuteStatus.Picking, PickingExcuteStatus.WaitDelivery, PickingExcuteStatus.Waiting);

//            //                    }

//            //                    roll = new PickingWaybillRoll(repository, roll.Select(item => item).GroupBy(item => item.WaybillID).Select(item => item.First()));

//            //                    return roll.ToPage(pageindex, pagesize);

//            //                }
//            //#if DEBUG
//            //            }
//            //#endif

//            throw new Exception();

//        }


//        public Wms.Services.Models.PickingWaybill PickWayBill(string warehouseID, string waybillid, string key = null)
//        {
//            //using (PvWmsRepository rep = new PvWmsRepository())
//            //{
//            //    var roll = new PickingWaybillRoll().SearchByWareHouseID(warehouseID).Where(item => item.WaybillID == waybillid);

//            //    if (roll.Count() == 0)
//            //    {
//            //        return null;
//            //    }

//            //    var data = roll.ToFillArray();

//            //    if (!string.IsNullOrEmpty(key) && data.Count() > 0)
//            //    {
//            //        return new PickingWaybillRoll(rep, roll).SearchByPartNumberOrManufacturer(data.First(), key);
//            //    }
//            //    var d = data.FirstOrDefault();

//            //    if (d != null && d.Notices.Length > 0)
//            //    {
//            //        d.Source = d.Notices.First().Source;
//            //    }
//            //    return d;
//            //}
//            throw new Exception();
//        }

//        //public string[] History(string waybillid)
//        //{
//        //    using (PvWmsRepository rep = new PvWmsRepository())
//        //    {
//        //        return new SortingsWayBillRoll(rep).History(waybillid);
//        //    }
//        //}

//        //public object HistoryDetail(string waybillid, string orderid)
//        //{
//        //    using (PvWmsRepository rep = new PvWmsRepository())
//        //    {
//        //        return new SortingsWayBillRoll(rep).HistoryDetail(waybillid, orderid);
//        //    }
//        //}


//        public string[] HistoryToWaybillCode_Date(string waybillid)
//        {
//            using (PvWmsRepository rep = new PvWmsRepository())
//            {
//                return new SortingsWayBillRoll(rep).Historys(waybillid);
//            }
//        }

//        public SortingWaybill HistoryDetail(string waybillID)
//        {
//            var roll = new SortingsWayBillRoll().SearchByWaybillD(waybillID);
//            var waybill = roll.FirstOrDefault();

//            var wb = this.WayBill(null, waybill.FatherID);



//            var notices = wb.Notices;     //.Where(item => item.WayBillID == waybillID).ToArray();
//            if (notices.Length > 0)
//            {

//                var ns = notices.SelectMany(item => (Wms.Services.Models.SortingNotice[])item.Sorted).Where(item => item.WayBillID == waybillID);

//                if (ns.Count() > 0)
//                {
//                    wb.Notices = ns.ToArray();
//                    wb.Source = ns.FirstOrDefault().Source;
//                }


//                wb.TotalCount = ns.Sum(item => item.Quantity);

//            }


//            var wbl = (SortingWaybill)waybill;
//            wb.Files = wb.DataFiles?.Where(item => item.WaybillID == waybillID).ToArray();
//            var sort = new PvWmsRepository().ReadTable<Layers.Data.Sqls.PvWms.Sortings>().Where(item => item.WaybillID == waybillID).FirstOrDefault();
//            if (sort != null)
//            {
//                wb.AdminID = sort.AdminID;
//                wb.AdminName = new PvWmsRepository().ReadTable<Layers.Data.Sqls.PvWms.AdminsTopView>().Where(item => item.ID == wb.AdminID).FirstOrDefault()?.RealName;
//            }

//            return wb;
//        }


//        public void TakeGoods(string waybillid)
//        {

//            using (PvWmsRepository rep = new PvWmsRepository())
//            {
//                rep.Command($"update {nameof(WaybillsTopView)} set {nameof(WaybillsTopView.wbExcuteStatus)}={(int)SortingExcuteStatus.Taking} where {nameof(WaybillsTopView.wbID)}='{waybillid}'");

//                rep.Command($"update {nameof(Logs_WaybillsTopView)} set IsCurrent=0 where  Type=3 and MainID='{waybillid}'");
//                rep.Command($"insert {nameof(Logs_WaybillsTopView)} values('{Guid.NewGuid().ToString().MD5()}','{waybillid}',{(int)WaybillStatusType.ExecutionStatus},{(int)SortingExcuteStatus.Taking},getdate(),'{admin.ID ?? ""}',1)");


//            }

//        }

//        public string GetInputID()
//        {
//            return PKeySigner.Pick(Yahv.Underly.PKeyType.Input, null);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="obj"></param>
//        /// <param name="Summary"></param>
//        /// <param name="Status"></param>
//        public void Enter(SortingWaybill waybill, string summary, int Status)
//        {

//            throw new Exception("现有逻辑错误，现有Storage表中不再包含NoticeID");
//            /*
//            SortingExcuteStatus status = (SortingExcuteStatus)Status;

//            string currentWaybillID = waybill.WaybillID;
//            //重新获取运单，判断运单分检状态是完成入库还是全部到货还是分批到货。
//            if (status == SortingExcuteStatus.Stocked)
//            {

//                if (waybill.Notices.Sum(item => item.TruetoQuantity ?? 0) >= waybill.Notices.Where(tem => tem.IsOriginalNotice).Sum(item => item.Quantity))
//                {
//                    status = SortingExcuteStatus.Stocked;
//                }
//                else
//                {
//                    status = SortingExcuteStatus.PartStocked;
//                    //重建waybill信息关联本次的waybill

//                    var newwaybill = new Yahv.Services.Models.Waybills(this.admin)
//                    {

//                        FatherID = waybill.WaybillID,
//                        Code = waybill.Code,
//                        Type = waybill.WaybillType,
//                        CarrierID = waybill.CarrierID,
//                        ConsignorID = waybill.ConsignorID,
//                        ConsigneeID = waybill.ConsigneeID,
//                        TotalParts = waybill.TotalParts ?? 0,
//                        TotalWeight = waybill.TotalWeight ?? 0,
//                        TotalVolume = waybill.TotalVolume ?? 0,
//                        CarrierAccount = waybill.CarrierID,
//                        VoyageNumber = waybill.VoyageNumber,
//                        Condition = waybill.Condition,
//                        EnterCode = waybill.EnterCode,
//                        Status = waybill.Status,
//                        TransferID = waybill.TransferID,
//                        Packaging = waybill.Packaging,
//                        Supplier = waybill.Supplier,
//                        ExcuteStatus = (int)SortingExcuteStatus.Stocked,
//                        Summary = waybill.Summary,
//                        Consignee = waybill.Consignee,
//                        Consignor = waybill.Consignor,
//                        WayCharge = waybill.WayCharge,
//                        WayChcd = waybill.WayChcd,
//                        WayLoading = waybill.WayLoading,
//                        CreatorID = this.admin.ID
//                    };
//                    newwaybill.Enter();

//                    currentWaybillID = newwaybill.ID;
//                }
//            }   

//            List<CenterProduct> products = new List<CenterProduct>();
//            var noticeType = 0;

//            var originNotices = this.WayBill(waybill.Notices.First().WareHouseID, waybill.WaybillID).Notices;

//            var listids = new List<string>();
//            listids.AddRange(originNotices.Select(item => item.InputID).Distinct().ToArray());

//            var nociceids = waybill.Notices.Where(item => item.Input != null).Select(item => item.ID);

//            using (PvWmsRepository rep = new PvWmsRepository())
//            {
//                listids.AddRange(rep.ReadTable<Layers.Data.Sqls.PvWms.Storages>().Where(item => nociceids.Contains(item.NoticeID)).Select(item => item.InputID).Distinct().ToArray());

//                var inputids = listids.Distinct();

//                if (waybill.Files != null)
//                {
//                    foreach (var file in waybill.Files)
//                    {
//                        //file.ID = PKeySigner.Pick(PkeyType.FileInfos, null);   因为只负责修改，不用重新生成编号
//                        if (string.IsNullOrEmpty(file.WaybillID))
//                        {
//                            file.WaybillID = currentWaybillID;
//                        }

//                        if (string.IsNullOrEmpty(file.AdminID))
//                        {
//                            file.AdminID = admin.ID;
//                        }

//                    }
//                }

//                var inputlist = new List<Input>();

//                if (waybill.Notices != null)
//                {
//                    foreach (var notice in waybill.Notices.Where(item => item.Input != null && item.Enabled))
//                    {
//                        var intputid = notice.InputID;

//                        if (noticeType == 0)
//                        {
//                            noticeType = (int)notice.Type;
//                        }


//                        //如果有拆项就优先按拆项处理
//                        if (notice.ID.StartsWith("CX"))
//                        {
//                            notice.ID = notice.PID;


//                        }


//                        var product = new CenterProduct { PartNumber = notice.Product.PartNumber, Manufacturer = notice.Product.Manufacturer, PackageCase = notice.Product.PackageCase, Packaging = notice.Product.Packaging };

//                        if (!rep.ReadTable<Layers.Data.Sqls.PvWms.ProductsTopView>().Where(item => item.ID == product.ID).Any())
//                        {
//                            try
//                            {
//                                products.Add(product);
//                            }
//                            catch
//                            { }
//                        }




//                        ////通知有变更时增加 新的进项信息   废弃，采用新规则
//                        //if (notice.IsChange)
//                        //{
//                        //    var inputID = PKeySigner.Pick(Yahv.Underly.PKeyType.Input);
//                        //    notice.Input.OriginID = notice.Input.ID;
//                        //    notice.Input.ID = inputID;
//                        //    notice.Input.ProductID = product.ID;
//                        //    notice.Input.DateCode = notice.DateCode;
//                        //    notice.Input.Code = inputID;

//                        //    inputlist.Add(notice.Input);
//                        //}

//                        //异常到货时，如果有实际货数，就按实际到货数，如果没有就按通知数
//                        //if (notice.Enabled && notice.TruetoQuantity.ToString() != null && status == SortingExcuteStatus.Anomalous)
//                        //{
//                        //    notice.TruetoQuantity = notice.Quantity;
//                        //}

//                        //只处理实际到货的信息
//                        if (!string.IsNullOrEmpty(notice.TruetoQuantity?.ToString()) && notice.TruetoQuantity > 0)
//                        {

//                            if (status == SortingExcuteStatus.PartStocked)
//                            {
//                                //如果分批到货时，产生新的input
//                                intputid = PKeySigner.Pick(Yahv.Underly.PKeyType.Input, null);
//                                notice.Input.ID = intputid;
//                                notice.Input.Code = intputid;
//                                notice.InputID = intputid;

//                            }

//                            var soringID = PKeySigner.Pick(PkeyType.Sortings, null);

//                            notice.Sorting = new Sorting
//                            {
//                                ID = soringID,
//                                InputID = intputid,
//                                NoticeID = notice.ID,
//                                BoxCode = notice.BoxCode,
//                                Quantity = notice.TruetoQuantity ?? 0,
//                                AdminID = admin.ID,
//                                CreateDate = DateTime.Now,
//                                Weight = notice.Weight,
//                                Volume = notice.Volume,
//                                NetWeight = notice.NetWeight,
//                                WaybillID = currentWaybillID

//                            };


//                            var storageID = PKeySigner.Pick(PkeyType.Storages, null);

//                            if (notice.Files != null)
//                            {
//                                foreach (var file in notice.Files)
//                                {
//                                    file.NoticeID = notice.ID;
//                                    file.StorageID = storageID;
//                                    if (string.IsNullOrEmpty(file.WaybillID))
//                                    {
//                                        file.WaybillID = currentWaybillID;
//                                    }
//                                    file.InputID = intputid;
//                                    if (string.IsNullOrEmpty(file.AdminID))
//                                    {
//                                        file.AdminID = admin.ID;
//                                    }
//                                }
//                            }

//                            StoragesType stype = StoragesType.Inventory;

//                            if (status == (SortingExcuteStatus)130)
//                            {
//                                stype = StoragesType.AbnormalStock;
//                            }

//                            notice.Storage = new Wms.Services.Models.Storage
//                            {
//                                ID = storageID,
//                                Type = stype,
//                                WareHouseID = notice.WareHouseID,
//                                SortingID = soringID,
//                                InputID = intputid,
//                                ProductID = product.ID,
//                                Quantity = notice.TruetoQuantity ?? 0,
//                                NoticeID = notice.ID,
//                                Supplier = notice.Supplier,
//                                DateCode = notice.Input.DateCode,
//                                IsLock = false,
//                                CreateDate = DateTime.Now,
//                                Status = Enums.StoragesStatus.Normal,
//                                ShelveID = notice.ShelveID,
//                                Forms = new Form[]
//                                {
//                                    new Form{
//                                        ID =PKeySigner.Pick(PkeyType.StoragesForm, null)
//                                        ,NoticeID=notice.ID,
//                                        StorageID=storageID,
//                                        Quantity=notice.TruetoQuantity ?? 0,
//                                        Status=FormStatus.Facted
//                                    }
//                                }
//                            };

//                        }

//                        //原数据的OriginID设计为空
//                        if (notice.Input.ID == notice.Input.OriginID)
//                        {
//                            notice.Input.OriginID = null;
//                        }

//                        notice.Input.ProductID = product.ID;
//                        notice.ProductID = product.ID;


//                        inputlist.Add(notice.Input);
//                    }

//                }

//                //using ()
//                //{

//                //}
//                using (var trans = new System.Transactions.TransactionScope())
//                {
//                    //更新运单信息
//                    rep.Command($"update {nameof(WaybillsTopView)} set {nameof(WaybillsTopView.wbExcuteStatus)}={(int)status},{nameof(WaybillsTopView.wbSummary)}='{summary}',{nameof(WaybillsTopView.wbCode)}='{waybill.Code}',{nameof(WaybillsTopView.wbCarrierID)}='{waybill.CarrierID}' where {nameof(WaybillsTopView.wbID)}='{waybill.WaybillID}'");

//                    //保存运单日志
//                    rep.Command($"update {nameof(Logs_WaybillsTopView)} set IsCurrent=0 where  Type=3 and MainID='{waybill.WaybillID}'");
//                    rep.Command($"insert {nameof(Logs_WaybillsTopView)} values('{Guid.NewGuid().ToString().MD5()}','{waybill.WaybillID}',{(int)WaybillStatusType.ExecutionStatus},{(int)status},getdate(),'{admin.ID ?? ""}',1)");

//                    //保存订单日志
//                    var orderexcutestatus = ((SortingExcuteStatus)status).ToOrderExcuteStaus();

//                    if (orderexcutestatus != null)
//                    {
//                        rep.Command($"update {nameof(Logs_PvWsOrderTopView)} set IsCurrent=0 where  Type=3 and  MainID='{waybill.OrderID}'");
//                        rep.Command($"insert {nameof(Logs_PvWsOrderTopView)} values('{Guid.NewGuid().ToString().MD5()}','{waybill.OrderID}',{(int)WaybillStatusType.ExecutionStatus},{(int)orderexcutestatus},getdate(),'{admin.ID ?? ""}',1)");
//                    }

//                    //更新发货地
//                    rep.Command($"update {nameof(WaybillsTopView)} set {nameof(WaybillsTopView.corPlace)}='{waybill.Place}' where {nameof(WaybillsTopView.corID)}='{waybill.ConsignorID}'");

//                    //保存运单文件
//                    if (waybill.Files != null && waybill.Files.Length > 0)
//                    {
//                        waybill.Files.Update(rep);
//                    }

//                    //保存产品信息


//                    foreach (var product in products)
//                    {

//                        try
//                        {
//                            var sql = $"insert {nameof(ProductsTopView)} values('{product.ID}','{product.PartNumber}','{product.Manufacturer}','{product.PackageCase}','{product.Packaging}',getdate())";
//                            rep.Command(sql);

//                            //vs2019以下的版本不支持
//                            //rep.Insert(new ProductsTopView { ID = product.ID, PartNumber = product.PartNumber, Manufacturer = product.Manufacturer, PackageCase = product.PackageCase, Packaging = product.Packaging, CreateDate = DateTime.Now });
//                        }
//                        catch
//                        { }
//                    }

//                    //保存通知
//                    if (waybill.Notices != null)
//                    {

//                        //保存input
//                        var oldInputList = inputlist.Where(tem => inputids.Contains(tem.ID));
//                        var newInputList = inputlist.Where(tem => !inputids.Contains(tem.ID));
//                        if (newInputList.Count() > 0)
//                        {
//                            //rep.Delete<Layers.Data.Sqls.PvWms.Inputs>(tem=>inputlist.Select(t=>t.ID).Contains(tem.ID));

//                            rep.Insert(newInputList.Select(item => new Layers.Data.Sqls.PvWms.Inputs
//                            {
//                                ID = item.ID,
//                                Code = item.Code,
//                                OriginID = item.OriginID,
//                                OrderID = item.OrderID,
//                                TinyOrderID = item.TinyOrderID,
//                                ItemID = item.ItemID,
//                                ProductID = item.ProductID,
//                                ClientID = item.ClientID,
//                                PayeeID = item.PayeeID,
//                                ThirdID = item.ThirdID,
//                                TrackerID = item.TrackerID,
//                                SalerID = item.SalerID,
//                                PurchaserID = item.PurchaserID,
//                                Currency = (int)item.Currency,
//                                UnitPrice = item.UnitPrice,
//                                CreateDate = DateTime.Now


//                            }).ToArray());
//                        }
//                        //存在就修改
//                        if (oldInputList.Count() > 0)
//                        {
//                            foreach (var item in oldInputList)
//                            {
//                                rep.Update<Layers.Data.Sqls.PvWms.Inputs>(new
//                                {

//                                    Code = item.Code,
//                                    OriginID = item.OriginID,
//                                    OrderID = item.OrderID,
//                                    item.TinyOrderID,
//                                    ItemID = item.ItemID,
//                                    ProductID = item.ProductID,
//                                    ClientID = item.ClientID,
//                                    PayeeID = item.PayeeID,
//                                    ThirdID = item.ThirdID,
//                                    TrackerID = item.TrackerID,
//                                    SalerID = item.SalerID,
//                                    PurchaserID = item.PurchaserID,
//                                    Currency = (int)item.Currency,
//                                    UnitPrice = item.UnitPrice,
//                                    DateCode = item.DateCode,
//                                    Origin = item.Origin,
//                                    CreateDate = DateTime.Now


//                                }, tem => tem.ID == item.ID);
//                            }
//                        }

//                        //取出可用的并且数量大于0的通知入库

//                        var enableNotices = waybill.Notices.Where(item => item.Enabled && !string.IsNullOrEmpty(item.TruetoQuantity.ToString()) && item.TruetoQuantity > 0).ToArray();

//                        foreach (var notice in enableNotices)
//                        {

//                            //保存分拣信息
//                            rep.Insert(new Layers.Data.Sqls.PvWms.Sortings
//                            {
//                                ID = notice.Sorting.ID,
//                                NoticeID = notice.ID,
//                                InputID = notice.Input.ID,
//                                BoxCode = notice.Sorting.BoxCode,
//                                Quantity = notice.Sorting.Quantity,
//                                AdminID = this.admin.ID,
//                                CreateDate = DateTime.Now,
//                                Weight = notice.Sorting.Weight,
//                                NetWeight = notice.NetWeight,
//                                Volume = notice.Sorting.Volume,
//                                WaybillID = currentWaybillID,
//                            });




//                            //保存库存信息
//                            rep.Insert(new Layers.Data.Sqls.PvWms.Storages
//                            {
//                                ID = notice.Storage.ID,
//                                Type = (int)notice.Storage.Type,
//                                WareHouseID = notice.Storage.WareHouseID,
//                                SortingID = notice.Storage.SortingID,
//                                InputID = notice.Storage.InputID,
//                                ProductID = notice.Storage.ProductID,
//                                Quantity = notice.Storage.Quantity,
//                                DateCode = notice.Input.DateCode,
//                                Supplier = notice.Supplier,
//                                NoticeID = notice.ID,
//                                IsLock = false,
//                                CreateDate = DateTime.Now,
//                                Status = (int)Enums.StoragesStatus.Normal,
//                                ShelveID = notice.Storage.ShelveID,
//                                Origin = notice.Input.Origin


//                            });

//                            if (notice.Storage.Forms != null)
//                            {
//                                foreach (var form in notice.Storage.Forms)
//                                {
//                                    rep.Insert(new Layers.Data.Sqls.PvWms.Forms
//                                    {
//                                        ID = form.ID,
//                                        NoticeID = form.ID,
//                                        StorageID = form.StorageID,
//                                        Quantity = form.Quantity,
//                                        Status = (int)FormStatus.Facted

//                                    });
//                                }
//                            }


//                            //保存通知(200  分拣完成)


//                            rep.Update<Layers.Data.Sqls.PvWms.Notices>(new { Status = 200, BoxCode = notice.BoxCode, ShelveID = notice.ShelveID }, item => item.ID == notice.ID);

//                        }

//                        foreach (var item in waybill.Notices)
//                        {

//                            //保存文件信息

//                            if (item.Files != null && item.Files.Length > 0)
//                            {
//                                item.Files.Update(rep);
//                            }
//                        }
//                    }



//                    trans.Complete();
//                    trans.Dispose();
//                }



//                if (noticeType == (int)NoticeType.TransferEnter)
//                {
//                    var listNotices = new List<PickingNotice>();
//                    var listForms = new List<Form>();
//                    var listOutPuts = new List<Wms.Services.Models.Output>();

//                    foreach (var item in waybill.Notices.Where(item => item.Input != null && item.Storage != null))
//                    {
//                        try
//                        {
//                            var noticeID = PKeySigner.Pick(PkeyType.Notices);
//                            var outputID = PKeySigner.Pick(PkeyType.Outputs);
//                            var formID = PKeySigner.Pick(PkeyType.StoragesForm);

//                            listNotices.Add(new PickingNotice
//                            {
//                                ID = noticeID,
//                                Type = CgNoticeType.Out,
//                                WareHouseID = item.WareHouseID,
//                                WaybillID = waybill.TransferID,
//                                InputID = item.InputID,
//                                OutputID = outputID,
//                                ProductID = item.ProductID,
//                                Supplier = item.Supplier,
//                                DateCode = item.DateCode,
//                                Quantity = item.TruetoQuantity ?? 0,
//                                Conditions = item.Condition,
//                                CreateDate = DateTime.Now,
//                                Status = item.Status,
//                                Source = item.Source,
//                                Target = item.Target,
//                                BoxCode = item.BoxCode,
//                                Weight = item.Weight,
//                                NetWeight = item.NetWeight,
//                                Volume = item.Volume,
//                                ShelveID = item.ShelveID,

//                            });
//                            listOutPuts.Add(new Wms.Services.Models.Output
//                            {
//                                ID = outputID,
//                                InputID = item.InputID,
//                                OrderID = item.Input.OrderID,
//                                TinyOrderID = item.Input.TinyOrderID,
//                                ItemID = item.Input.ItemID,
//                                OwnerID = item.Input.ClientID,
//                                SalerID = item.Input.SalerID,
//                                CustomerServiceID = item.InputID,
//                                PurchaserID = item.Input.PurchaserID,
//                                Currency = item.Input.Currency,
//                                Price = item.Input.UnitPrice,
//                                CreateDate = DateTime.Now,
//                                StorageID = item.Storage.ID,
//                                Checker = null,
//                            });

//                            listForms.Add(new Form
//                            {
//                                ID = formID,
//                                NoticeID = noticeID,
//                                StorageID = item.Storage.ID,
//                                Quantity = 0 - item.TruetoQuantity ?? 0,
//                                Status = FormStatus.Facted
//                            });
//                        }
//                        catch (Exception ex)
//                        {
//                            var m = ex.Message;
//                        }
//                    }

//                    using (var trans = new System.Transactions.TransactionScope())
//                    {
//                        try
//                        {
//                            //保存出库通知
//                            rep.Insert(listNotices.Select(item => new Layers.Data.Sqls.PvWms.Notices
//                            {
//                                ID = item.ID,        //唯一码
//                                Type = (int)item.Type,        //通知类型：入库通知、出库通知、分拣通知、检测通知、捡货通知、客户自提通知、装箱通知
//                                WareHouseID = item.WareHouseID,        //仓库编号
//                                WaybillID = item.WaybillID,        //运单编号
//                                InputID = item.InputID,        //进项编号
//                                OutputID = item.OutputID,        //销项编号
//                                ProductID = item.ProductID,        //产品编号
//                                Supplier = item.Supplier,        //
//                                DateCode = item.DateCode,        //批次号
//                                Quantity = item.Quantity,        //数量
//                                Conditions = item.Conditions.Json(),        //条件
//                                CreateDate = item.CreateDate,        //创建时间
//                                Status = (int)item.Status,        //状态：等待Waiting、关闭Closed、（货物）丢失Lost、完成Completed
//                                Source = (int)item.Source,        //来源
//                                Target = (int)item.Target,        //目标
//                                BoxCode = item.BoxCode,        //箱号
//                                Weight = item.Weight,        //重量
//                                Volume = item.Volume,        //体积
//                                ShelveID = item.ShelveID,        //货架编号

//                            }).ToArray());


//                            rep.Insert(listForms.Select(item => new Layers.Data.Sqls.PvWms.Forms
//                            {
//                                ID = item.ID,        //唯一码
//                                StorageID = item.StorageID,        //库存ID
//                                Quantity = item.Quantity,        //数量
//                                NoticeID = item.NoticeID,        //通知ID
//                                Status = (int)FormStatus.Frozen,        //冻结：Frozen,真实的（真正执行的）：Facted});
//                            }).ToArray());


//                            //保存销项                        
//                            rep.Insert(listOutPuts.Select(item => new Layers.Data.Sqls.PvWms.Outputs
//                            {
//                                ID = item.ID,        //四位年+2位月+2日+6位流水
//                                InputID = item.InputID,        //
//                                OrderID = item.OrderID,        //MainID
//                                TinyOrderID = item.TinyOrderID,
//                                ItemID = item.ItemID,        //项ID
//                                OwnerID = item.OwnerID,        //法人
//                                SalerID = item.SalerID,        //AdminID
//                                CustomerServiceID = item.CustomerServiceID,        //跟单员
//                                PurchaserID = item.PurchaserID,        //
//                                Currency = (int)item.Currency,        //保值
//                                Price = item.Price,        //保值
//                                CreateDate = item.CreateDate,        //发生时间
//                                StorageID = item.StorageID,        //出库的库存ID
//                            }).ToArray());

//                            rep.Command($"update {nameof(WaybillsTopView)} set {nameof(WaybillsTopView.wbStatus)}={(int)GeneralStatus.Normal} where {nameof(WaybillsTopView.wbID)}='{waybill.TransferID}'");

//                            //提交事务
//                            trans.Complete();
//                        }
//                        catch (Exception ex)
//                        {

//                            throw ex;
//                        }
//                    }






//                }


//            }


//            try
//            {

//                string api = string.Concat(FromType.OrderApi.GetDescription(), "/Sorted/SubmitSorted");

//                var sc = new StoreChange();
//                sc.List.Add(new ChangeItem { orderid = waybill.OrderID });

//                var result = Yahv.Utils.Http.ApiHelper.Current.JPost(api, sc);

//                using (PvCenterReponsitory reponsitory = new PvCenterReponsitory())
//                {
//                    reponsitory.Insert<Layers.Data.Sqls.PvCenter.Logs_Operating>(new Layers.Data.Sqls.PvCenter.Logs_Operating()
//                    {
//                        ID = Guid.NewGuid().ToString(),
//                        Type = (int)Yahv.Services.Enums.LogType.WsOrder,
//                        MainID = waybill.OrderID,
//                        Operation = "库房调用根据订单号修改信息的接口!",
//                        Creator = "NPC",
//                        CreateDate = DateTime.Now,
//                        Summary = result
//                    });
//                    reponsitory.Submit();
//                }

//            }
//            catch (Exception ex)
//            {

//                using (PvCenterReponsitory reponsitory = new PvCenterReponsitory())
//                {
//                    reponsitory.Insert<Layers.Data.Sqls.PvCenter.Logs_Operating>(new Layers.Data.Sqls.PvCenter.Logs_Operating()
//                    {
//                        ID = Guid.NewGuid().ToString(),
//                        Type = (int)Yahv.Services.Enums.LogType.WsOrder,
//                        MainID = waybill.OrderID,
//                        Operation = "库房调用根据订单号修改信息的接口!",
//                        Creator = "NPC",
//                        CreateDate = DateTime.Now,
//                        Summary = ex.Message,
//                    });
//                    reponsitory.Submit();
//                }

//            }


//            if (waybill.Notices.First().Source == NoticeSource.AgentBreakCustoms)
//            {
//                try
//                {

//                    string api = string.Concat(FromType.ArrivalInfoToXDT.GetDescription());

//                    var obj = new { VastOrderID = waybill.OrderID };

//                    var result = Yahv.Utils.Http.ApiHelper.Current.JPost(api, obj);

//                    using (PvCenterReponsitory reponsitory = new PvCenterReponsitory())
//                    {
//                        reponsitory.Insert<Layers.Data.Sqls.PvCenter.Logs_Operating>(new Layers.Data.Sqls.PvCenter.Logs_Operating()
//                        {
//                            ID = Guid.NewGuid().ToString(),
//                            Type = (int)Yahv.Services.Enums.LogType.WsOrder,
//                            MainID = waybill.OrderID,
//                            Operation = "库房到货信息告知华芯通！",
//                            Creator = "NPC",
//                            CreateDate = DateTime.Now,
//                            Summary = result
//                        });
//                        reponsitory.Submit();
//                    }

//                }
//                catch (Exception ex)
//                {

//                    using (PvCenterReponsitory reponsitory = new PvCenterReponsitory())
//                    {
//                        reponsitory.Insert<Layers.Data.Sqls.PvCenter.Logs_Operating>(new Layers.Data.Sqls.PvCenter.Logs_Operating()
//                        {
//                            ID = Guid.NewGuid().ToString(),
//                            Type = (int)Yahv.Services.Enums.LogType.WsOrder,
//                            MainID = waybill.OrderID,
//                            Operation = "库房到货信息告知华芯通",
//                            Creator = "NPC",
//                            CreateDate = DateTime.Now,
//                            Summary = ex.Message,
//                        });
//                        reponsitory.Submit();
//                    }

//                }
//            }
//            */
//        }

//        public void NewInput()
//        {

//        }


//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="obj"></param>
//        /// <param name="Summary"></param>
//        /// <param name="Status"></param>
//        public void PickEnter(Wms.Services.Models.PickingWaybill waybill, string summary, int Status)
//        {


//            int status = Status;
//            using (PvWmsRepository rep = new PvWmsRepository())
//            {


//                if (waybill.Files != null)
//                {
//                    foreach (var file in waybill.Files)
//                    {
//                        //file.ID = PKeySigner.Pick(PkeyType.FileInfos, null);  因为只负责修改，不用重新生成编号
//                        file.WaybillID = waybill.WaybillID;
//                        if (string.IsNullOrEmpty(file.AdminID))
//                        {
//                            file.AdminID = admin.ID;
//                        }
//                    }
//                }

//                if (waybill.Notices != null)
//                {

//                    foreach (var notice in waybill.Notices)
//                    {

//                        if (notice.Files != null)
//                        {
//                            foreach (var file in notice.Files)
//                            {
//                                //file.ID = PKeySigner.Pick(PkeyType.FileInfos, null);   因为只负责修改，不用重新生成编号
//                                file.NoticeID = notice.ID;
//                                file.StorageID = notice.StorageID;
//                                if (string.IsNullOrEmpty(file.AdminID))
//                                {
//                                    file.AdminID = admin.ID;
//                                }
//                            }
//                        }

//                        var pickingID = PKeySigner.Pick(PkeyType.Pickings, null);
//                        notice.Picking = new Picking
//                        {
//                            ID = pickingID,
//                            NoticeID = notice.ID,
//                            BoxCode = notice.BoxCode,
//                            Quantity = notice.Quantity,
//                            AdminID = admin.ID,
//                            CreateDate = DateTime.Now,
//                            Weight = notice.Weight,
//                            Volume = notice.Volume,
//                            NetWeight = notice.NetWeight,
//                            StorageID = notice.StorageID

//                        };




//                    }
//                }

//                using (var trans = new System.Transactions.TransactionScope())
//                {


//                    //更新运单信息
//                    rep.Command($"update {nameof(WaybillsTopView)} set {nameof(WaybillsTopView.wbExcuteStatus)}={status},{nameof(WaybillsTopView.wbCode)}='{waybill.Code}',{nameof(WaybillsTopView.wbSummary)}='{summary}',{nameof(WaybillsTopView.wbCarrierID)}='{waybill.CarrierID}' where {nameof(WaybillsTopView.wbID)}='{waybill.WaybillID}'");

//                    //保存运单日志
//                    rep.Command($"update {nameof(Logs_WaybillsTopView)} set IsCurrent=0 where Type=3 and MainID='{waybill.WaybillID}'");
//                    rep.Command($"insert {nameof(Logs_WaybillsTopView)} values('{Guid.NewGuid().ToString().MD5()}','{waybill.WaybillID}',{(int)WaybillStatusType.ExecutionStatus},{status},getdate(),'{admin.ID ?? ""}',1)");

//                    //保存订单日志

//                    var orderexcutestatus = ((PickingExcuteStatus)status).ToOrderExcuteStaus();

//                    if (orderexcutestatus != null)
//                    {
//                        rep.Command($"update {nameof(Logs_PvWsOrderTopView)} set IsCurrent=0 where  Type=3 and MainID='{waybill.OrderID}'");
//                        rep.Command($"insert {nameof(Logs_PvWsOrderTopView)} values('{Guid.NewGuid().ToString().MD5()}','{waybill.OrderID}',{(int)WaybillStatusType.ExecutionStatus},{(int)orderexcutestatus},getdate(),'{admin.ID ?? ""}',1)");
//                    }

//                    //更新发货地
//                    rep.Command($"update {nameof(WaybillsTopView)} set {nameof(WaybillsTopView.corPlace)}='{waybill.Place}' where {nameof(WaybillsTopView.corID)}='{waybill.ConsignorID}'");

//                    //更新运单号和承运商
//                    rep.Command($"update {nameof(WaybillsTopView)} set {nameof(WaybillsTopView.wbCode)}='{waybill.Code}',{nameof(WaybillsTopView.wbCarrierID)}='{waybill.CarrierID}' where {nameof(WaybillsTopView.wbID)}='{waybill.WaybillID}'");


//                    //保存运单文件
//                    if (waybill.Files != null)
//                    {
//                        waybill.Files.Update(rep);
//                    }



//                    //保存通知
//                    if (waybill.Notices != null)
//                    {

//                        foreach (var notice in waybill.Notices)
//                        {

//                            //保存拣货信息
//                            rep.Insert(new Layers.Data.Sqls.PvWms.Pickings
//                            {
//                                ID = notice.Picking.ID,        //四位年+2位月+2日+6位流水
//                                StorageID = notice.Picking.StorageID,        // 
//                                NoticeID = notice.Picking.NoticeID,        // 
//                                BoxCode = notice.Picking.BoxCode,        //装箱信息
//                                Quantity = notice.Picking.Quantity,        // 
//                                AdminID = notice.Picking.AdminID,        // 
//                                CreateDate = DateTime.Now,        //CreateDate
//                                Weight = notice.Picking.Weight,        //
//                                NetWeight = notice.NetWeight,
//                                Volume = notice.Picking.Volume,        // 


//                            });


//                            //保存文件信息

//                            if (notice.Files != null)
//                            {
//                                waybill.Files.Update(rep);
//                            }

//                            //保存通知(200  分拣完成)

//                            rep.Update<Layers.Data.Sqls.PvWms.Notices>(new { Status = 200, Weight = notice.Weight, BoxCode = notice.BoxCode, BoxingSpecs = notice.BoxingSpecs }, item => item.ID == notice.ID);
//                        }


//                    }

//                    ////提货人信息
//                    //if (string.IsNullOrEmpty(waybill.WayLoading.ID))
//                    //{
//                    //    rep.Command($"insert {nameof(WayLoadingsTopView)} values('{waybill.WaybillID}',getdate(),'{waybill.WayLoading.TakingAddress}','{waybill.WayLoading.TakingContact}','{waybill.WayLoading.TakingPhone}','{waybill.WayLoading.CarNumber1}','{waybill.WayLoading.Driver}',{waybill.WayLoading.Carload ?? 0},getdate(),getdate(),'{admin.ID ?? ""}','{admin.ID ?? ""}')");
//                    //}
//                    //else
//                    //{
//                    //    rep.Command($"update {nameof(WayLoadingsTopView)} set {nameof(waybill.WayLoading.CarNumber1)}='{waybill.WayLoading.CarNumber1}',{nameof(waybill.WayLoading.Driver)}='{waybill.WayLoading.Driver}',{nameof(waybill.WayLoading.ModifyDate)}=getdate(),{nameof(waybill.WayLoading.ModifierID)}='{this.admin.ID}' where ID='{waybill.WaybillID}'");
//                    //}


//                    rep.Submit();
//                    trans.Complete();
//                    trans.Dispose();
//                }
//            }


//            try
//            {
//                //调用其它应用下的接口

//                string api = string.Concat(FromType.OrderApi.GetDescription(), "/Sorted/SubmitSorted");

//                var sc = new StoreChange();
//                sc.List.Add(new ChangeItem { orderid = waybill.OrderID });

//                Yahv.Utils.Http.ApiHelper.Current.JPost(api, sc);

//            }
//            catch
//            {


//            }

//        }

//        public void Submit(SortingWaybill waybill)
//        {

//            using (PvWmsRepository rep = new PvWmsRepository())
//            {

//                if (waybill.Files != null)
//                {
//                    foreach (var file in waybill.Files)
//                    {
//                        //file.ID = PKeySigner.Pick(PkeyType.FileInfos, null);
//                        file.WaybillID = waybill.WaybillID;
//                    }
//                }

//                var listOriginInputIDs = new List<string>();
//                var listOriginNoticeIDs = new List<string>();

//                if (waybill.Notices != null)
//                {
//                    //异常后重新发入为通知
//                    //获取旧的通知
//                    var originNotices = rep.ReadTable<Layers.Data.Sqls.PvWms.Notices>().Where(item => item.WaybillID == waybill.WaybillID).ToList();

//                    listOriginInputIDs.AddRange(originNotices.Select(tem => tem.InputID));

//                    listOriginNoticeIDs.AddRange(originNotices.Select(tem => tem.ID));

//                    //新旧通知对比比，没有的删除，有的修改，新的新增
//                    if (originNotices.Count() > 0)
//                    {
//                        var newinputids = waybill.Notices.Select(item => item.Input.ID);

//                        //处理通知
//                        foreach (var item in originNotices)
//                        {

//                            if (!newinputids.Contains(item.InputID))
//                            {
//                                //把无效通知及其相关的信息删除          
//                                //var oldInputIDs = originNotices.Select(tem => tem.InputID).Distinct();
//                                //rep.Delete<Layers.Data.Sqls.PvWms.Inputs>(tem => oldInputIDs.Contains(tem.ID));


//                                //var storageIDs = rep.ReadTable<Storages>().Where(tem => tem.NoticeID == item.ID)?.Select(tem => tem.ID).Distinct().ToArray();
//                                //if (storageIDs != null && storageIDs.Length > 0)
//                                //{
//                                //    rep.Delete<Layers.Data.Sqls.PvWms.Forms>(tem => storageIDs.Contains(tem.StorageID));
//                                //}

//                                //rep.Delete<Layers.Data.Sqls.PvWms.Storages>(tem => tem.NoticeID == item.ID);
//                                //rep.Delete<Layers.Data.Sqls.PvWms.Sortings>(tem => tem.NoticeID == item.ID);
//                                //rep.Delete<Layers.Data.Sqls.PvWms.Notices>(tem => tem.ID == item.ID);

//                                //2020年01月8日,把通知改为不可用
//                                rep.Update<Layers.Data.Sqls.PvWms.Notices>(new { Status = (int)NoticesStatus.Closed }, tem => tem.ID == item.ID);

//                            }
//                            else
//                            {
//                                var notices = waybill.Notices.Where(tem => tem.InputID == item.InputID);
//                                foreach (var tem in notices)
//                                {
//                                    tem.ID = item.ID;
//                                    tem.Status = NoticesStatus.Waiting;

//                                }

//                            }

//                        }

//                    }


//                    foreach (var notice in waybill.Notices)
//                    {
//                        if (string.IsNullOrEmpty(notice.ID))
//                        {
//                            notice.ID = PKeySigner.Pick(PkeyType.Notices, null);
//                        }

//                        if (notice.Files != null)
//                        {
//                            foreach (var file in notice.Files)
//                            {
//                                //file.ID = PKeySigner.Pick(PkeyType.FileInfos, null);
//                                file.WaybillID = waybill.WaybillID;
//                                file.NoticeID = notice.ID;
//                            }
//                        }

//                    }
//                }

//                using (var trans = new System.Transactions.TransactionScope())
//                {



//                    //保存通知
//                    if (waybill.Notices != null)
//                    {
//                        foreach (var notice in waybill.Notices)
//                        {
//                            if (!listOriginInputIDs.Contains(notice.InputID))
//                            {
//                                //存进项
//                                rep.Insert(new Layers.Data.Sqls.PvWms.Inputs
//                                {
//                                    ID = notice.Input.ID,
//                                    Code = notice.Input.Code,
//                                    OriginID = notice.Input.OriginID,
//                                    OrderID = notice.Input.OrderID,
//                                    TinyOrderID = notice.Input.TinyOrderID,
//                                    ItemID = notice.Input.ItemID,
//                                    ProductID = notice.Input.ProductID,
//                                    ClientID = notice.Input.ClientID,
//                                    TrackerID = notice.Input.TrackerID,
//                                    SalerID = notice.Input.SalerID,
//                                    PurchaserID = notice.Input.PurchaserID,
//                                    Currency = (int)notice.Input.Currency,
//                                    UnitPrice = notice.Input.UnitPrice,
//                                    CreateDate = DateTime.Now,
//                                    PayeeID = notice.Input.PayeeID,
//                                    ThirdID = notice.Input.ThirdID,

//                                });
//                            }
//                            else
//                            {
//                                rep.Update<Layers.Data.Sqls.PvWms.Inputs>(new
//                                {
//                                    Code = notice.Input.Code,
//                                    OriginID = notice.Input.OriginID,
//                                    OrderID = notice.Input.OrderID,
//                                    notice.Input.TinyOrderID,
//                                    ItemID = notice.Input.ItemID,
//                                    ProductID = notice.Input.ProductID,
//                                    ClientID = notice.Input.ClientID,
//                                    TrackerID = notice.Input.TrackerID,
//                                    SalerID = notice.Input.SalerID,
//                                    PurchaserID = notice.Input.PurchaserID,
//                                    Currency = (int)notice.Input.Currency,
//                                    UnitPrice = notice.Input.UnitPrice,
//                                    DateCode = notice.Input.DateCode,
//                                    Origin = notice.Input.Origin,
//                                    CreateDate = DateTime.Now,
//                                    PayeeID = notice.Input.PayeeID,
//                                    ThirdID = notice.Input.ThirdID,

//                                }, tem => tem.ID == notice.InputID);
//                            }



//                            //保存通知(200  分拣完成)

//                            if (!listOriginNoticeIDs.Contains(notice.ID))
//                            {

//                                rep.Insert(new Layers.Data.Sqls.PvWms.Notices
//                                {
//                                    ID = notice.ID,
//                                    Type = (int)notice.Type,
//                                    WareHouseID = notice.WareHouseID,
//                                    WaybillID = notice.WayBillID,
//                                    InputID = notice.Input?.ID,
//                                    OutputID = null,
//                                    ProductID = notice.ProductID,
//                                    Supplier = notice.Supplier,
//                                    DateCode = notice.DateCode,
//                                    Quantity = notice.Quantity,
//                                    Conditions = notice.Condition.Json(),
//                                    CreateDate = DateTime.Now,
//                                    Status = (int)notice.Status,
//                                    Source = (int)notice.Source,
//                                    Target = (int)notice.Target,
//                                    BoxCode = notice.BoxCode,
//                                    Weight = notice.Weight,
//                                    Volume = notice.Volume,
//                                    NetWeight = notice.NetWeight,
//                                    ShelveID = notice.ShelveID,
//                                });
//                            }
//                            else
//                            {
//                                rep.Update<Layers.Data.Sqls.PvWms.Notices>(new
//                                {

//                                    Type = (int)notice.Type,
//                                    WareHouseID = notice.WareHouseID,
//                                    WaybillID = notice.WayBillID,
//                                    InputID = notice.Input.ID,
//                                    OutputID = "",
//                                    ProductID = notice.ProductID,
//                                    Supplier = notice.Supplier,
//                                    DateCode = notice.DateCode,
//                                    Quantity = notice.Quantity,
//                                    Conditions = notice.Condition.Json(),
//                                    CreateDate = DateTime.Now,
//                                    Status = (int)notice.Status,
//                                    Source = (int)notice.Source,
//                                    Target = (int)notice.Target,
//                                    BoxCode = notice.BoxCode,
//                                    Weight = notice.Weight,
//                                    Volume = notice.Volume,
//                                    NetWeight = notice.NetWeight,
//                                    ShelveID = notice.ShelveID,
//                                }, tem => tem.ID == notice.ID);
//                            }

//                            if (notice.Files != null)
//                            {
//                                notice.Files.Update(rep);
//                            }
//                        }


//                    }

//                    if (waybill.Files != null && waybill.Files.Length > 0)
//                    {
//                        waybill.Files.Update(rep);
//                    }


//                    rep.Submit();
//                    trans.Complete();

//                }

//            }
//        }

//        /// <summary>
//        /// 暂存运单或异常运单显示
//        /// </summary>
//        /// <param name="warehouseID">库房ID，必填</param>
//        /// <param name="excuteStatus">执行状态，必填</param>
//        /// <param name="waybillCode">运单号</param>
//        /// <param name="carrierID">承运商编号</param>
//        /// <param name="place">发货地</param>
//        /// <param name="shelveID">库位</param>
//        /// <param name="createDate">创建时间</param>
//        /// <param name="startDate">开始时间</param>
//        /// <param name="endDate">结束时间</param>
//        /// <param name="enterCode">入仓号</param>
//        /// <param name="tempEnterCode">入库单号</param>
//        /// <param name="pageindex">当前页码</param>
//        /// <param name="pagesize">每页显示记录数</param>
//        /// <returns></returns>
//        public object TempWaybill(string warehouseID, string excuteStatus, string waybillCode = null, string carrierID = null, string place = null, string shelveID = null, string createDate = null, string startDate = null, string endDate = null, string enterCode = null, string tempEnterCode = null, int pageindex = 1,
//         int pagesize = 20)
//        {
//            using (PvWmsRepository repository = new PvWmsRepository())
//            {
//                var roll = new TempStoragesWayBillRoll(repository);
//                if (!string.IsNullOrWhiteSpace(warehouseID))
//                {
//                    roll = roll.SearchByWarehouseID(warehouseID);
//                }

//                if (!string.IsNullOrWhiteSpace(excuteStatus))
//                {
//                    roll = new TempStoragesWayBillRoll(repository, roll).SearchByStatus((TempStockExcuteStatus)(int.Parse(excuteStatus)));
//                }

//                if (!string.IsNullOrEmpty(enterCode))
//                {
//                    roll = new TempStoragesWayBillRoll(repository, roll).SearchByEnterCode(enterCode);
//                }

//                if (!string.IsNullOrEmpty(tempEnterCode))
//                {
//                    roll = new TempStoragesWayBillRoll(repository, roll).SearchByTempEnterCode(tempEnterCode);
//                }

//                if (!string.IsNullOrEmpty(waybillCode))
//                {
//                    roll = new TempStoragesWayBillRoll(repository, roll).SearchByCode(waybillCode);
//                }
//                if (!string.IsNullOrEmpty(carrierID))
//                {
//                    roll = new TempStoragesWayBillRoll(repository, roll).SearchByCarrierID(carrierID);
//                }
//                if (!string.IsNullOrEmpty(place))
//                {
//                    roll = new TempStoragesWayBillRoll(repository, roll).SearchByPlace(place);
//                }
//                if (!string.IsNullOrEmpty(shelveID))
//                {
//                    roll = new TempStoragesWayBillRoll(repository, roll).SearchByShelveID(shelveID);
//                }
//                DateTime Date;

//                if (DateTime.TryParse(createDate, out Date))
//                {
//                    roll = new TempStoragesWayBillRoll(repository, roll).SearchByDate(createDate);
//                }

//                DateTime _startDate;
//                if (DateTime.TryParse(startDate, out _startDate))
//                {
//                    var view = roll.Where(item => item.CreateDate >= _startDate);
//                    roll = new TempStoragesWayBillRoll(repository, view);
//                }
//                DateTime _endDate;
//                if (DateTime.TryParse(endDate, out _endDate))
//                {
//                    var view = roll.Where(item => item.CreateDate < _endDate.AddDays(1));
//                    roll = new TempStoragesWayBillRoll(repository, view);
//                }

//                roll = new TempStoragesWayBillRoll(repository, roll.Select(item => item).GroupBy(item => item.WaybillID).Select(item => item.First()));

//                return roll.ToPage(pageindex, pagesize);

//            }


//        }

//        /// <summary>
//        /// 暂存运单或异常运单详情
//        /// </summary>
//        /// <param name="warehouseID"></param>
//        /// <param name="waybillid"></param>
//        /// <returns></returns>
//        public TempStorageWaybill TempStorageWayBill(string warehouseID, string waybillid)
//        {
//            using (PvWmsRepository rep = new PvWmsRepository())
//            {
//                var roll = new TempStoragesWayBillRoll().SearchByWarehouseID(warehouseID);
//                roll = roll.SearchByID(waybillid);
//                return roll.ToFillArray().FirstOrDefault();
//            }
//        }

//        /// <summary>
//        /// 关联订单
//        /// </summary>
//        /// <param name="enterCode">入仓号</param>
//        /// <param name="tempEnterCode">入库单号</param>
//        /// <param name="partNumber">型号</param>
//        /// <param name="manufacture">品牌</param>
//        /// <param name="number">数量</param>
//        /// <param name="place">产地</param>
//        /// <param name="carrierID">承运商</param>
//        /// <returns></returns>
//        public void LinkedOrder(string warehouseID, string enterCode = null, string tempEnterCode = null, string partNumber = null, string manufacture = null, int? quantity = null, string place = null, string carrierID = null)
//        {
//            using (PvWmsRepository repository = new PvWmsRepository())
//            {
//                var roll = new TempStoragesWayBillRoll(repository);
//                if (!string.IsNullOrWhiteSpace(warehouseID))
//                {
//                    roll = roll.SearchByWarehouseID(warehouseID);
//                }
//                //IQueryable<Waybill> waybills = null;
//                if (!string.IsNullOrWhiteSpace(enterCode))
//                {
//                    roll = new TempStoragesWayBillRoll(repository, roll).SearchByEnterCode(enterCode);
//                    //waybills = new Views.ServicesWaybillsTopView().Where(item => item.EnterCode == enterCode);
//                }
//                else if (string.IsNullOrWhiteSpace(enterCode) && !string.IsNullOrWhiteSpace(tempEnterCode))
//                {
//                    roll = new TempStoragesWayBillRoll(repository, roll).SearchByTempEnterCode(tempEnterCode);
//                    //waybills = new Views.ServicesWaybillsTopView().Where(item => item.TempEnterCode == tempEnterCode);
//                }
//                else if (string.IsNullOrWhiteSpace(enterCode) && string.IsNullOrWhiteSpace(tempEnterCode))
//                {
//                    //if (!string.IsNullOrWhiteSpace(partNumber))
//                    //{
//                    //    roll = new TempStoragesWayBillRoll(repository, roll).SearchByPartNumber(partNumber);
//                    //}

//                    //if (!string.IsNullOrWhiteSpace(manufacture))
//                    //{
//                    //    roll = new TempStoragesWayBillRoll(repository, roll).SearchByManufacture(manufacture);
//                    //}

//                    //if (quantity != null)
//                    //{
//                    //    roll = new TempStoragesWayBillRoll(repository, roll).SearchByQuantity(quantity);
//                    //}

//                    //if (!string.IsNullOrWhiteSpace(place))
//                    //{
//                    //    roll = new TempStoragesWayBillRoll(repository, roll).SearchByPlace(place);
//                    //}

//                    //if (!string.IsNullOrWhiteSpace(carrierID))
//                    //{
//                    //    roll = new TempStoragesWayBillRoll(repository, roll).SearchByCarrierID(carrierID);
//                    //}

//                }
//            }
//        }

//        public void UpdateWayBillCode(string waybillids, string code)
//        {
//            using (var rep = new Layers.Data.Sqls.PvCenterReponsitory())
//            {
//                var ids = waybillids.Split(',');
//                rep.Update<Layers.Data.Sqls.PvCenter.Waybills>(new { Code = code }, item => ids.Contains(item.ID));
//                rep.Submit();
//            }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="waybillids"></param>
//        /// <param name="Subcodes">以","分隔</param>
//        public void UpdateWayBillSubCode(string waybillid, string Subcodes)
//        {
//            using (var rep = new Layers.Data.Sqls.PvCenterReponsitory())
//            {
//                rep.Update<Layers.Data.Sqls.PvCenter.Waybills>(new { Subcodes = Subcodes }, item => item.ID == waybillid);
//            }
//        }


//        //更新截单状态
//        public void UpdateCuttingOrderStatus(string[] waybillIDs, int cuttingOrderStatus)
//        {
//            using (var rep = new Layers.Data.Sqls.PvCenterReponsitory())
//            {
//                rep.Update<Layers.Data.Sqls.PvCenter.Waybills>(new { CuttingOrderStatus = cuttingOrderStatus }, item => waybillIDs.Contains(item.ID));
//                rep.Submit();
//            }
//        }


//        public void UpdateWayBillInfo(string ID, string Code, int? TotalParts, decimal? TotalWeight, decimal? TotalVolume, string CarrierID)
//        {
//            using (var rep = new Layers.Data.Sqls.PvCenterReponsitory())
//            {
//                rep.Update<Layers.Data.Sqls.PvCenter.Waybills>(new { Code, TotalParts, TotalWeight, TotalVolume, CarrierID }, item => item.ID == ID);
//            }
//        }


//        public void UpdateInfoByInputID(InfoByInput entity)
//        {
//            using (var rep = new Layers.Data.Sqls.PvWmsRepository())
//            {
//                rep.Update<Notices>(new { Weight = entity.Weight }, item => item.InputID == entity.InputID);
//                rep.Update<Sortings>(new { Weight = entity.Weight }, item => item.InputID == entity.InputID);
//            }
//        }


//        public string GetWayBillSubCode(string waybillid)
//        {
//            using (var rep = new Layers.Data.Sqls.PvCenterReponsitory())
//            {
//                return rep.ReadTable<Layers.Data.Sqls.PvCenter.Waybills>().Where(item => item.ID == waybillid).FirstOrDefault().Subcodes ?? "";
//            }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="outputID"></param>
//        public void Check(string outputID)
//        {
//            using (var rep = new Layers.Data.Sqls.PvWmsRepository())
//            {
//                rep.Update<Layers.Data.Sqls.PvWms.Outputs>(new { Checker = this.admin.ID }, item => item.ID == outputID);
//            }
//        }

//        /// <summary>
//        /// 根据进项ID修改订单项ItemID
//        /// </summary>
//        /// <param name="inputID"></param>
//        /// <param name="itemID"></param>
//        public void UpdateItem(string inputID, string itemID = null, string tinyOrderID = null)
//        {
//            using (var rep = new PvWmsRepository())
//            {

//                if (string.IsNullOrWhiteSpace(itemID))
//                {
//                    rep.Update(new Layers.Data.Sqls.PvWms.Inputs
//                    {
//                        TinyOrderID = tinyOrderID,
//                    }, item => item.ID == inputID);
//                }
//                else if (string.IsNullOrWhiteSpace(tinyOrderID))
//                {
//                    rep.Update(new Layers.Data.Sqls.PvWms.Inputs
//                    {
//                        ItemID = itemID,
//                    }, item => item.ID == inputID);
//                }
//                else if (!string.IsNullOrWhiteSpace(itemID) && !string.IsNullOrWhiteSpace(tinyOrderID))
//                {
//                    rep.Update(new Layers.Data.Sqls.PvWms.Inputs
//                    {
//                        ItemID = itemID,
//                        TinyOrderID = tinyOrderID
//                    }, item => item.ID == inputID);
//                }

//            }
//        }

//        /// <summary>
//        /// 删除未出库的出库通知
//        /// </summary>
//        /// <param name="noticeID"></param>
//        public void DeleteOutStock(string noticeID)
//        {
//            throw new Exception("现有逻辑错误，Storages表中不再包含noticeID");
//            /*
//            using (var rep = new PvWmsRepository())
//            {
//                //查出对应的通知信息
//                var notice = new Views.PickingNoticesView().Where(item => item.ID == noticeID).FirstOrDefault();
//                //rep.ReadTable<Layers.Data.Sqls.PvWms.Notices>().Where(item => item.ID == noticeID).FirstOrDefault();
//                //if (notice == null)
//                //{
//                //    //所要删除的信息不存在
//                //    return;
//                //}

//                var outputID = notice.OutputID;
//                var storageIDs = rep.ReadTable<Layers.Data.Sqls.PvWms.Storages>().Where(item => item.NoticeID == noticeID).Select(item => item.ID).ToArray();

//                using (var trans = new TransactionScope())
//                {
//                    try
//                    {
//                        rep.Delete<Layers.Data.Sqls.PvWms.Forms>(item => item.NoticeID == noticeID && storageIDs.Contains(item.StorageID));

//                        rep.Delete<Layers.Data.Sqls.PvWms.Pickings>(item => item.NoticeID == noticeID);

//                        rep.Delete<Layers.Data.Sqls.PvWms.Outputs>(item => item.ID == outputID);

//                        rep.Delete<Layers.Data.Sqls.PvWms.Notices>(item => item.ID == noticeID);

//                        trans.Complete();
//                    }
//                    finally
//                    {
//                        trans.Dispose();
//                    }

//                }

//            }
//            */
//        }

//        /// <summary>
//        /// 出库拣货通知
//        /// </summary>
//        /// <param name="warehouseID">库房编号</param>
//        /// <param name="key">型号/制造商</param>
//        /// <param name="waybillID">运单编号</param>
//        /// <param name="tinyOrderID">小订单编号</param>
//        /// <param name="vastOrderID">订单编号</param>
//        /// <param name="pageIndex">当前页码</param>
//        /// <param name="pageSize">每页记录数</param>
//        /// <returns></returns>
//        public object GetPickingNotice(string warehouseID, string key = null, string waybillID = null, string tinyOrderID = null, string vastOrderID = null, int pageIndex = 1, int pageSize = 20)
//        {
//            using (PvWmsRepository repository = new PvWmsRepository())
//            {
//                var roll = new SZPickingNoticesRoll(repository);
//                if (!string.IsNullOrWhiteSpace(warehouseID))
//                {
//                    roll = roll.SearchByWarehouseID(warehouseID);
//                }

//                if (!string.IsNullOrWhiteSpace(key))
//                {
//                    roll = new SZPickingNoticesRoll(repository, roll).SearchByKey(key);
//                }
//                if (!string.IsNullOrWhiteSpace(waybillID))
//                {
//                    roll = new SZPickingNoticesRoll(repository, roll).SearchByKey(waybillID);
//                }
//                if (!string.IsNullOrWhiteSpace(tinyOrderID))
//                {
//                    roll = new SZPickingNoticesRoll(repository, roll).SearchByKey(tinyOrderID);
//                }
//                if (!string.IsNullOrWhiteSpace(vastOrderID))
//                {
//                    roll = new SZPickingNoticesRoll(repository, roll).SearchByKey(vastOrderID);
//                }
//                roll = new SZPickingNoticesRoll(repository, roll.Select(item => item).GroupBy(item => item.WaybillID).Select(item => item.First()));

//                return roll.ToPage(pageIndex, pageSize);
//            }
//        }

//        public void UpdateFile(string id, string waybillID, string customName, int type)
//        {
//            using (PvWmsRepository rep = new PvWmsRepository())
//            {

//                new CenterFileDescription[]
//                {
//                  new CenterFileDescription
//                  {
//                   AdminID = admin.ID,
//                   ID = id,
//                   WaybillID = waybillID,
//                   CustomName=customName,
//                   Type = type
//                 }
//               }.Update(rep);

//                //累计运单状态，不可逆，判断运单状态不是200就改成200，否则不予修改，运单修改一下，累计logs_waybill 状态
//                var waybill = new Yahv.Services.Views.WaybillsTopView<PvWmsRepository>().Where(item => item.ID == waybillID).FirstOrDefault();
//                if (waybill.ConfirmReceiptStatus != 200)
//                {
//                    //修改对应运单状态
//                    rep.Command($"update {nameof(Layers.Data.Sqls.PvWms.WaybillsTopView)} set {nameof(Layers.Data.Sqls.PvWms.WaybillsTopView.ConfirmReceiptStatus)}=200 where {nameof(Layers.Data.Sqls.PvWms.WaybillsTopView.wbID)}='{waybill.ID}'");

//                    //新增一条对应的运单日志
//                    rep.Command($"insert into {nameof(Layers.Data.Sqls.PvWms.Logs_WaybillsTopView)} values('{Guid.NewGuid().ToString().MD5()}','{waybill.ID}',{(int)WaybillStatusType.ConfirmReceiptStatus},'200',getdate(),'{admin.ID ?? ""}',1)");
//                }
//            }
//        }


//        public void DeleteFile(string id)
//        {
//            using (PvWmsRepository rep = new PvWmsRepository())
//            {
//                rep.Command($"delete from FilesDescriptionTopView where ID='{id}'");
//                rep.Submit();
//            }
//        }

//        //public object GetCustomTransport()
//        //{
//        //    using (var rep = new PvWmsRepository())
//        //    {
//        //        //var data = rep.ReadTable<Layers.Data.Sqls.PvCenter.WayChcd>();
//        //        var data = new Views.WayChcdTopView();
//        //        return data;

//        //    }
//        //}
//    }
//}
