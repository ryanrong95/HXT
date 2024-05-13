//using Layers.Data;
//using Layers.Data.Sqls;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Transactions;
//using Wms.Services.Extends;
//using Wms.Services.Views;
//using Yahv.Services.Enums;
//using Yahv.Services.Models;
//using Yahv.Underly;
//using Yahv.Underly.Enums;
//using Yahv.Underly.Erps;
//using Yahv.Usually;
//using Yahv.Utils.Converters.Contents;
//using Yahv.Utils.Serializers;

//namespace Wms.Services
//{
//    public class CustomServieces
//    {

//        public CustomServieces()
//        {

//        }
//        IErpAdmin admin;
//        public CustomServieces(IErpAdmin admin)
//        {
//            this.admin = admin;
//        }

//        public event SuccessHanlder Success;
//        public event ErrorHanlder Failed;
//        public event SuccessHanlder OutputSuccess;
//        public event ErrorHanlder OutputFailed;
//        public event ErrorHanlder NotCutting;//未截单
//        public event ErrorHanlder StoQuantityLess;//库存数量不足，无法出库


//        /// <summary>
//        /// 目前为香港报关运输和深圳入库
//        /// </summary>
//        /// <param name="type"></param>
//        /// <param name="warehouseID"></param>
//        /// <param name="lotnumber"></param>
//        /// <param name="startdate"></param>
//        /// <param name="enddate"></param>
//        /// <param name="carrierID"></param>
//        /// <param name="carNumber"></param>
//        /// <param name="pageindex"></param>
//        /// <param name="pagesize"></param>
//        /// <returns></returns>
//        public object GetCustomTransport(int type, string warehouseID, string lotnumber = null, string startdate = null, string enddate = null, string carrierID = null, string carNumber = null, CuttingOrderStatus? status = null,
//             int pageindex = 1, int pagesize = 20)
//        {


//            using (var rep = new PvWmsRepository())
//            {
//                var roll = new CustomRoll(rep);
//                roll = roll.SearchByNoticeType((NoticeType)type);
//                roll = roll.SearchByWarehouseID(warehouseID);
//                if (!string.IsNullOrWhiteSpace(lotnumber))
//                {
//                    roll = roll.SearchByLotnumber(lotnumber);
//                }
//                DateTime startDate;
//                if (DateTime.TryParse(startdate, out startDate))
//                {
//                    var view = roll.Where(item => item.DepartDate >= startDate);
//                    roll = new CustomRoll(rep, view);
//                }
//                DateTime endDate;
//                if (DateTime.TryParse(enddate, out endDate))
//                {
//                    var view = roll.Where(item => item.DepartDate < endDate.AddDays(1));
//                    roll = new CustomRoll(rep, view);
//                }
//                if (!string.IsNullOrWhiteSpace(carrierID))
//                {
//                    roll = new CustomRoll(rep, roll).SearchByCarrierID(carrierID);
//                }
//                if (!string.IsNullOrWhiteSpace(carNumber))
//                {
//                    roll = new CustomRoll(rep, roll).SearchByCarNumber(carNumber);
//                }
//                if (status.HasValue)
//                {
//                    roll = new CustomRoll(rep, roll).SearchByCuttingOrderStatus(status.Value);
//                }

//                return roll.ToPage(pageindex, pagesize);
//            }
//        }

//        public object Detail(string warehouseID, string lotnumber)
//        {
//            using (var rep = new PvWmsRepository())
//            {
//                var roll = new CustomRoll(rep);
//                return roll.OutputDetail(warehouseID, lotnumber);
//                //var roll = new Custom(rep);
//                //roll.SearchByWarehouseID(warehouseID);
//                //roll.SearchByLotnumber(lotnumber);
//                //var data = roll.ToFillArray().FirstOrDefault();
//                //return data;
//            }
//        }

//        public void OutputEnter(/*string warehouseID,*/ string lotnumber)
//        {



//            using (var rep = new PvWmsRepository())
//            {
//                try
//                {
//                    //根据运输批次号获得对应的运单
//                    var waybills = new ServicesWaybillsTopView().Where(item => item.WayChcd.LotNumber == lotnumber).ToArray();

//                    //根据运输批次号获得对应的运单编号
//                    var waybillIDs = waybills.Select(item => item.ID).Distinct().ToArray();

//                    //判断运单是否截单
//                    if (waybills.Any(item => item.CuttingOrderStatus == 100))
//                    {
//                        if (this != null && this.NotCutting != null)
//                        {
//                            //订单未截单
//                            this.NotCutting(this, new ErrorEventArgs("Waybill is not cutting!!"));
//                            return;
//                        }
//                    }

//                    List<Notice> szEnterNotices = new List<Notice>();
//                    List<Input> szEnterInputs = new List<Input>();
//                    List<Sorting> szEnterSorting = new List<Sorting>();
//                    List<Yahv.Services.Models.Storage> szEnterStorage = new List<Yahv.Services.Models.Storage>();
//                    List<Form> szEnterForm = new List<Form>();


//                    List<Models.PickingNotice> noticeList = new List<Models.PickingNotice>();

//                    //获得运单下所有的通知
//                    var notices = rep.ReadTable<Layers.Data.Sqls.PvWms.Notices>().Where(item => waybillIDs.Contains(item.WaybillID)).ToArray();
//                    if (notices != null)
//                    {
//                        foreach (var notice in notices)
//                        {
//                            //对应的库存流水信息
//                            var forms = rep.ReadTable<Layers.Data.Sqls.PvWms.Forms>().Where(item => item.StorageID == notice.StorageID);
//                            //对应的库存数量
//                            var stoQuantity = forms.Sum(item => item.Quantity);

//                            //通知
//                            if (notice.Quantity > stoQuantity)
//                            {
//                                if (this != null && this.StoQuantityLess != null)
//                                {
//                                    this.StoQuantityLess(this, new ErrorEventArgs("Storage Quantity is not enough!!"));
//                                    return ;
//                                }
//                            }
                       
//                            //根据inputID找到notice对应的output销项信息
//                            var output = rep.ReadTable<Layers.Data.Sqls.PvWms.Outputs>().Where(item => item.InputID == notice.InputID).FirstOrDefault();
//                            var pickingID = PKeySigner.Pick(PkeyType.Pickings, null);
//                            noticeList.Add(new Models.PickingNotice
//                            {
//                                ID = notice.ID,
//                                Picking = new Picking
//                                {
//                                    ID = pickingID,
//                                    NoticeID = notice.ID,
//                                    BoxCode = notice.BoxCode,
//                                    Quantity = notice.Quantity,
//                                    AdminID = admin.ID,
//                                    CreateDate = DateTime.Now,
//                                    Weight = notice.Weight,
//                                    Volume = notice.Volume,
//                                    NetWeight = notice.NetWeight,
//                                    StorageID = notice.StorageID
//                                },
//                                Output = new Yahv.Services.Models.Output
//                                {
//                                    OrderID = output.OrderID,
//                                    ID = output.ID
//                                }
//                            });

//                            var waybill = waybills.Where(item => item.ID == notice.WaybillID).FirstOrDefault();

//                            //深圳库房的入库通知
//                            var input = rep.ReadTable<Layers.Data.Sqls.PvWms.Inputs>().Where(item => item.ID == notice.InputID).FirstOrDefault();
//                            var szNoticeID = PKeySigner.Pick(PkeyType.Notices, null);
//                            var szInputID = PKeySigner.Pick(PKeyType.Input, null);
//                            var szWarehouseID = "SZ01_XDT";
//                            //通知信息
//                            szEnterNotices.Add(new Notice
//                            {
//                                ID = szNoticeID,
//                                Type = CgNoticeType.Enter,
//                                WareHouseID = szWarehouseID,
//                                WaybillID = notice.WaybillID,
//                                InputID = szInputID,
//                                OutputID = null,
//                                ProductID = notice.ProductID,
//                                Supplier = notice.Supplier,
//                                DateCode = notice.DateCode,
//                                Quantity = notice.Quantity,
//                                Conditions = notice.Conditions?.JsonTo<NoticeCondition>(),
//                                CreateDate = DateTime.Now,
//                                Status = NoticesStatus.Waiting,
//                                Source = CgNoticeSource.AgentEnter,
//                                Target = (NoticesTarget)notice.Target,
//                                BoxCode = notice.BoxCode,
//                                Weight = notice.Weight,
//                                NetWeight = notice.NetWeight,
//                                Volume = notice.Volume,
//                                ShelveID = notice.ShelveID,
//                                BoxingSpecs = notice.BoxingSpecs,
//                            });

//                            //进项信息
//                            szEnterInputs.Add(new Input
//                            {
//                                ID = szInputID,
//                                Code = szInputID,
//                                OriginID = null,
//                                OrderID = output.OrderID,
//                                TinyOrderID = output.TinyOrderID,
//                                ItemID = output.ItemID,
//                                ProductID = notice.ProductID,
//                                ClientID = input.ClientID,
//                                PayeeID = input.PayeeID,
//                                ThirdID = input.ThirdID,
//                                TrackerID = input.TrackerID,
//                                SalerID = input.SalerID,
//                                PurchaserID = input.PurchaserID,
//                                Currency = output.Currency == null ? Currency.HKD : (Currency)output.Currency,
//                                UnitPrice = output.Price,
//                                CreateDate = DateTime.Now
//                            });


//                            var szSortingID = PKeySigner.Pick(Wms.Services.PkeyType.Sortings, null);
//                            szEnterSorting.Add(new Sorting
//                            {

//                                ID = szSortingID,
//                                NoticeID = szNoticeID,
//                                InputID = szInputID,
//                                WaybillID = notice.WaybillID,
//                                BoxCode = notice.BoxCode,
//                                Quantity = notice.Quantity,
//                                AdminID = admin.ID,
//                                CreateDate = DateTime.Now,
//                                Weight = notice.Weight,
//                                NetWeight = notice.NetWeight,
//                                Volume = notice.Volume,

//                            }
//                            );

//                            var szStorageID = PKeySigner.Pick(Wms.Services.PkeyType.Storages, null);
//                            szEnterStorage.Add(new Yahv.Services.Models.Storage
//                            {
//                                ID = szStorageID,
//                                Type = StoragesType.Inventory,
//                                WareHouseID = notice.WareHouseID,
//                                SortingID = szSortingID,
//                                InputID = notice.InputID,
//                                ProductID = notice.ProductID,
//                                Quantity = notice.Quantity,
//                                NoticeID = szNoticeID,
//                                Origin = notice.Origin,
//                                IsLock = false,
//                                CreateDate = DateTime.Now,
//                                Status = GeneralStatus.Normal,
//                                DateCode = notice.DateCode
//                            }
//                            );

//                            var szFormID = PKeySigner.Pick(Wms.Services.PkeyType.StoragesForm, null);
//                            szEnterForm.Add(new Form
//                            {
//                                ID = szFormID,
//                                StorageID = szStorageID,
//                                Quantity = notice.Quantity,
//                                NoticeID = szNoticeID,
//                                Status = FormStatus.Facted
//                            }
//                            );
//                        }
//                    }

//            //        using (var trans = new TransactionScope())
//            //        {
//            //            var status = (int)PickingExcuteStatus.OutStock;
//            //            //foreach (var waybill in waybills)
//            //            //{
//            //            //    //更新运单信息
//            //            //    rep.Command($"update {nameof(Layers.Data.Sqls.PvWms.WaybillsTopView)} set {nameof(Layers.Data.Sqls.PvWms.WaybillsTopView.wbExcuteStatus)}={status} where {nameof(Layers.Data.Sqls.PvWms.WaybillsTopView.wbID)}='{waybill.ID}'");

//            //            //    //保存运单日志
//            //            //    rep.Command($"update {nameof(Layers.Data.Sqls.PvWms.Logs_WaybillsTopView)} set IsCurrent=0 where Type=3 and MainID='{waybill.ID}'");
//            //            //    rep.Command($"insert {nameof(Layers.Data.Sqls.PvWms.Logs_WaybillsTopView)} values('{Guid.NewGuid().ToString().MD5()}','{waybill.ID}',{(int)WaybillStatusType.ExecutionStatus},{status},getdate(),'{admin.ID ?? ""}',1)");
//            //            //}
//            //            //foreach (var notice in noticeList)
//            //            //{
//            //            //    //保存订单日志
//            //            //    var orderexcutestatus = ((PickingExcuteStatus)status).ToOrderExcuteStaus();

//            //            //    if (orderexcutestatus != null)
//            //            //    {
//            //            //        rep.Command($"update {nameof(Layers.Data.Sqls.PvWms.Logs_PvWsOrderTopView)} set IsCurrent=0 where  Type=3 and MainID='{notice.Output.OrderID}'");
//            //            //        rep.Command($"insert {nameof(Layers.Data.Sqls.PvWms.Logs_PvWsOrderTopView)} values('{Guid.NewGuid().ToString().MD5()}','{notice.Output.OrderID}',{(int)WaybillStatusType.ExecutionStatus},{(int)orderexcutestatus},getdate(),'{admin.ID ?? ""}',1)");
//            //            //    }

//            //                //保存拣货信息
//            //                //rep.Insert(new Layers.Data.Sqls.PvWms.Pickings
//            //                //{
//            //                //    ID = notice.Picking.ID,        //四位年+2位月+2日+6位流水
//            //                //    StorageID = notice.Picking.StorageID,        // 
//            //                //    NoticeID = notice.Picking.NoticeID,        // 
//            //                //    BoxCode = notice.Picking.BoxCode,        //装箱信息
//            //                //    Quantity = notice.Picking.Quantity,        // 
//            //                //    AdminID = notice.Picking.AdminID,        // 
//            //                //    CreateDate = DateTime.Now,        //CreateDate
//            //                //    Weight = notice.Picking.Weight,        //
//            //                //    NetWeight = notice.NetWeight,
//            //                //    Volume = notice.Picking.Volume,        // 
//            //                //});


//            //                ////保存通知(200  分拣完成)
//            //                //rep.Update<Layers.Data.Sqls.PvWms.Notices>(new { Status = 200 }, item => item.ID == notice.ID);

//            //            }


//            //            //保存深圳入库通知和进项信息
//            //            if (szEnterNotices.Count > 0)
//            //            {
//            //                rep.Insert(szEnterNotices.Select(item => new Layers.Data.Sqls.PvWms.Notices
//            //                {
//            //                    ID = item.ID,
//            //                    Type = (int)item.Type,
//            //                    WareHouseID = item.WareHouseID,
//            //                    WaybillID = item.WaybillID,
//            //                    InputID = item.InputID,
//            //                    OutputID = item.OutputID,
//            //                    ProductID = item.ProductID,
//            //                    Supplier = item.Supplier,
//            //                    DateCode = item.DateCode,
//            //                    Quantity = item.Quantity,
//            //                    Conditions = item.Conditions.Json(),
//            //                    CreateDate = item.CreateDate,
//            //                    Status = (int)item.Status,
//            //                    Source = (int)item.Source,
//            //                    Target = (int)item.Target,
//            //                    BoxCode = item.BoxCode,
//            //                    Weight = item.Weight,
//            //                    NetWeight = item.NetWeight,
//            //                    Volume = item.Volume,
//            //                    ShelveID = item.ShelveID,
//            //                    BoxingSpecs = item.BoxingSpecs,
//            //                }).ToArray());

//            //                rep.Insert(szEnterInputs.Select(item => new Layers.Data.Sqls.PvWms.Inputs
//            //                {
//            //                    ID = item.ID,
//            //                    Code = item.Code,
//            //                    OriginID = item.OriginID,
//            //                    OrderID = item.OrderID,
//            //                    TinyOrderID = item.TinyOrderID,
//            //                    ItemID = item.ItemID,
//            //                    ProductID = item.ProductID,
//            //                    ClientID = item.ClientID,
//            //                    PayeeID = item.PayeeID,
//            //                    ThirdID = item.ThirdID,
//            //                    TrackerID = item.TrackerID,
//            //                    SalerID = item.SalerID,
//            //                    PurchaserID = item.PurchaserID,
//            //                    Currency = (int)item.Currency,
//            //                    UnitPrice = item.UnitPrice,
//            //                    CreateDate = item.CreateDate,
//            //                }).ToArray());

//            //                rep.Insert(szEnterSorting.Select(item => new Layers.Data.Sqls.PvWms.Sortings
//            //                {
//            //                    ID = item.ID,
//            //                    NoticeID = item.NoticeID,
//            //                    InputID = item.InputID,
//            //                    WaybillID = item.WaybillID,
//            //                    BoxCode = item.BoxCode,
//            //                    Quantity = item.Quantity,
//            //                    AdminID = item.AdminID,
//            //                    CreateDate = item.CreateDate,
//            //                    Weight = item.Weight,
//            //                    NetWeight = item.NetWeight,
//            //                    Volume = item.Volume

//            //                }).ToArray());

//            //                rep.Insert(szEnterStorage.Select(item => new Layers.Data.Sqls.PvWms.Storages
//            //                {
//            //                    ID = item.ID,
//            //                    Type = (int)item.Type,
//            //                    WareHouseID = item.WareHouseID,
//            //                    SortingID = item.SortingID,
//            //                    InputID = item.InputID,
//            //                    ProductID = item.ProductID,
//            //                    Quantity = item.Quantity,
//            //                    Origin = item.Origin,
//            //                    IsLock = item.IsLock,
//            //                    CreateDate = item.CreateDate,
//            //                    Status = (int)item.Status,
//            //                    DateCode = item.DateCode,


//            //                }).ToArray());

//            //                rep.Insert(szEnterForm.Select(item => new Layers.Data.Sqls.PvWms.Forms
//            //                {
//            //                    ID = item.ID,
//            //                    StorageID = item.StorageID,
//            //                    Quantity = item.Quantity,
//            //                    NoticeID = item.NoticeID,
//            //                    Status = (int)item.Status


//            //                }).ToArray());
//            //            }


//            //            rep.Submit();
//            //            trans.Complete();
//            //            trans.Dispose();

//            //            if (this != null && this.OutputSuccess != null)
//            //            {
//            //                this.OutputSuccess(this, new SuccessEventArgs("Success!!"));
//            //            }
//            //        }
//            //    }
//            //    catch (Exception ex)
//            //    {
//            //        if (this != null && this.OutputFailed != null)
//            //        {
//            //            this.OutputFailed(this, new ErrorEventArgs("Failed!!"));
//            //        }
//            //    }

//            //}


//        }



//        public object EnterDetail(string warehouseID, string lotnumber)
//        {
//            using (var rep = new PvWmsRepository())
//            {
//                var roll = new CustomRoll(rep);
//                return roll.EnterDetail(warehouseID, lotnumber);
//            }
//        }

//        public void UpperShelf(string[] boxCodes, string newShelveID)
//        {
//            using (var rep = new PvWmsRepository())
//            {
//                try
//                {

//                    foreach (var boxCode in boxCodes)
//                    {
//                        //获得一条通知，箱子里可能有好几条通知的信息，但只会对应一个库存信息，所以只需获得一条通知，根据库存去上架
//                        var notice = new SortingNoticesView().Where(item => item.BoxCode == boxCode).FirstOrDefault();
//                        rep.Update(new Layers.Data.Sqls.PvWms.Storages
//                        {
//                            ShelveID = newShelveID
//                        }, item => item.ID == notice.Storage.ID);
//                    }
//                    if (this != null && this.Success != null)
//                    {
//                        this.Success(this, new SuccessEventArgs("Success!!"));
//                    }

//                }
//                catch
//                {

//                    if (this != null && this.Failed != null)
//                    {
//                        this.Failed(this, new ErrorEventArgs("Failed!!"));
//                    }
//                }

//            }
//        }


//    }

//}
