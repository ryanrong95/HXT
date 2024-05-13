using Layers.Data;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbCrm;
using Layers.Data.Sqls.PvWms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Wms.Services.Enums;
using Yahv.Linq.Extends;
using Yahv.Services.Enums;
using Yahv.Services.Extends;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Serializers;

namespace Wms.Services.Models
{
    public class DataPickingWaybill : Wms.Services.Models.Waybills
    {
        public int? ExcuteStatus { get; set; }
        public PickingNotice[] Notices { get; set; }
        public string TotalGoodsValue { get; set; }
        public CgNoticeSource Source { get; set; }
        public decimal TotalPieces { get; set; }
        public string CuttingOrderStatusDescription
        {
            get
            {
                return this.CuttingOrderStatus.GetDescription();
            }
        }

        public static implicit operator PickingWaybill(DataPickingWaybill entity)
        {
            return new PickingWaybill
            {
                ClientName = entity.ClientName,
                EnterCode = entity.EnterCode,
                CreateDate = entity.CreateDate,
                DataFiles = entity.DataFiles,
                Notices = entity.Notices,
                WaybillType = entity.WaybillType,
                WaybillID = entity.WaybillID,
                Code = entity.Code,
                Supplier = entity.Supplier,
                CarrierID = entity.CarrierID,
                CarrierName = entity.CarrierName,
                Place = entity.Place,
                Condition = entity.Condition,
                ConsignorID = entity.ConsignorID,
                ExcuteStatus = (PickingExcuteStatus)(entity.ExcuteStatus ?? 200),
                WayLoading = entity.WayLoading,
                Consignee = entity.Consignee,
                Consignor = entity.Consignor,
                TotalGoodsValue = entity.TotalGoodsValue,
                TotalPieces = entity.TotalPieces,
                Packaging = entity.Packaging,
                TransferID = entity.TransferID,
                WayCharge = entity.WayCharge,
                WayChcd = entity.WayChcd,
                ConsigneeID = entity.ConsigneeID,
                Status = entity.Status,
                Summary = entity.Summary,
                TotalParts = entity.TotalParts,
                TotalVolume = entity.TotalVolume,
                TotalWeight = entity.TotalWeight,
                VoyageNumber = entity.VoyageNumber,
                FatherID = entity.FatherID,
                Source = entity.Source,
            };
        }
    }

    public class PickingWaybill : Wms.Services.Models.Waybills
    {

        public PickingNotice[] Notices { get; set; }

        public bool IsAuto { get; set; }

        /// <summary>
        /// 运单冗余执行状态
        /// </summary>
        /// <remarks>
        /// 冗余字段
        /// </remarks>
        public PickingExcuteStatus? ExcuteStatus { get; set; }

        public string ExcuteStatusDescription
        {
            get
            {
                if (this.ExcuteStatus == null)
                {
                    return null;
                }
                return this.ExcuteStatus.GetDescription();
            }
        }

        public string CuttingOrderStatusDescription
        {
            get
            {
                if (this.CuttingOrderStatus == null)
                {
                    return null;
                }
                return this.CuttingOrderStatus.GetDescription();
            }
        }

        public string TotalGoodsValue { get; set; }

        /// <summary>
        /// 总件数
        /// </summary>
        public decimal TotalPieces { get; set; }

        private CenterFileDescription[] files;
        public CenterFileDescription[] Files
        {
            get
            {
                if (files == null && this.DataFiles != null)
                {
                    files = this.DataFiles.Where(item => string.IsNullOrEmpty(item.NoticeID)).ToArray();
                }
                return files;
            }
            set { files = value; }
        }

        string orderID;

        public string OrderID
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(orderID))
                {
                    return this.orderID;
                }

                if (this.Notices != null)
                {
                    return string.Join(",", this.Notices.Select(item => item.Output.OrderID).Distinct());
                }
                return this.orderID;
            }
            set
            {
                this.orderID = value;
            }
        }

        public string WaybillTypeDescription
        {
            get
            {
                return this.WaybillType.GetDescription();
            }
        }

        public CgNoticeSource Source { get; set; }

        public string SourceDescription
        {
            get
            {
                return this.Source.GetDescription();
            }
        }

        public string PlaceID
        {
            get
            {
                if (string.IsNullOrEmpty(this.Place))
                { return ""; }
                var index = (int)Enum.GetValues(typeof(Origin)).Cast<Origin>().Where(item => item.GetOrigin().Code == this.Place).FirstOrDefault();
                return index.ToString();
            }
        }

        public string PlaceDescription
        {
            get
            {
                if (string.IsNullOrEmpty(this.Place))
                { return ""; }
                var origins = Enum.GetValues(typeof(Origin)).Cast<Origin>().Select(item => item.GetOrigin());
                origins = origins.Where(item => item.Code == this.Place);
                if (origins.Count() == 0)
                {
                    try
                    {
                        return ((Origin)int.Parse(this.Place)).GetOrigin().ChineseName;
                    }
                    catch
                    {
                        return "";
                    }
                }

                var name = origins.FirstOrDefault().ChineseName;
                return name;
            }
        }

        public WayCondition Conditions
        {
            get
            {
                if (Condition != null)
                {
                    return this.Condition.JsonTo<WayCondition>();
                }
                else
                {

                    return null;
                }
            }
        }

        public event EventHandler<EventArgs> LackStockEvent;

        /// <summary>
        /// 出库通知对接提交
        /// </summary>
        public void Submit()
        {
            List<Form> forms = new List<Form>();
            using (var rep = new Layers.Data.Sqls.PvWmsRepository())
            {

                if (!string.IsNullOrEmpty(this.CarrierName) && this.WayChcd != null)
                {
                    if (this.WayChcd.Carload != null)
                    {
                        VehicleType carload;

                        if (Enum.TryParse(this.WayChcd.Carload.ToString(), out carload))
                        {
                            var carrier = new Yahv.Services.Models.Carrier();
                            carrier.Enter(this.CarrierName, this.WayChcd.Driver, this.WayChcd.CarNumber1, carload);

                            this.CarrierID = carrier.ID;

                        }

                    }
                }

                if (string.IsNullOrEmpty(this.WaybillID) && this.Source == CgNoticeSource.AgentBreakCustoms)
                {
                    var waybiill = new Yahv.Services.Models.Waybills()
                    {
                        ID = this.WaybillID,
                        FatherID = this.FatherID,
                        Code = this.Code,
                        Type = (WaybillType)this.WaybillType,
                        CarrierID = this.CarrierID,
                        ConsignorID = this.ConsignorID,
                        ConsigneeID = this.ConsigneeID,
                        TotalParts = this.TotalParts ?? 0,
                        TotalWeight = this.TotalWeight ?? 0,
                        TotalVolume = this.TotalVolume ?? 0,
                        VoyageNumber = this.VoyageNumber,
                        Condition = this.Condition,
                        CreateDate = DateTime.Now,
                        EnterCode = this.EnterCode ?? "",
                        Status = this.Status,
                        TransferID = this.TransferID,
                        Packaging = this.Packaging,
                        Supplier = this.Supplier,
                        ExcuteStatus = (int)(this.ExcuteStatus ?? PickingExcuteStatus.Waiting),
                        Summary = this.Summary,
                        WayLoading = this.WayLoading,
                        WayChcd = this.WayChcd,
                        Consignee = this.Consignee,
                        Consignor = this.Consignor,
                        CarrierName = this.CarrierName,
                        IsClearance = false,
                        ModifyDate = DateTime.Now,
                        WayCharge = this.WayCharge,
                        CreatorID = "NPC"



                    };

                    waybiill.Enter();
                    this.WaybillID = waybiill.ID;

                }
                else
                {

                    //获取旧的通知
                    var originNotices = rep.ReadTable<Layers.Data.Sqls.PvWms.Notices>().Where(item => item.WaybillID == this.WaybillID).ToList();

                    //新旧通知对比比，没有的删除，有的修改，新的新增
                    if (originNotices.Count() > 0)
                    {
                        var newoutputids = this.Notices.Select(item => item.Output.ID);
                        //处理通知
                        foreach (var item in originNotices)
                        {

                            var oldOuputIDs = originNotices.Select(tem => tem.OutputID).Distinct();
                            rep.Delete<Layers.Data.Sqls.PvWms.Outputs>(tem => oldOuputIDs.Contains(tem.ID));

                            var sortingids = rep.ReadTable<Sortings>().Where(tem => tem.NoticeID == item.ID).Select(tem => tem.ID);

                            var storageIDs = rep.ReadTable<Storages>().Where(tem => sortingids.Contains(tem.SortingID))?.Select(tem => tem.ID).Distinct().ToArray();
                            if (storageIDs != null && storageIDs.Length > 0)
                            {
                                rep.Delete<Layers.Data.Sqls.PvWms.Forms>(tem => storageIDs.Contains(tem.StorageID));
                            }

                            rep.Delete<Layers.Data.Sqls.PvWms.Storages>(tem => storageIDs.Contains(tem.ID));

                            rep.Delete<Layers.Data.Sqls.PvWms.Pickings>(tem => tem.NoticeID == item.ID);
                            rep.Delete<Layers.Data.Sqls.PvWms.Notices>(tem => tem.ID == item.ID);


                        }

                    }
                }

                var files = new List<CenterFileMessage>();
                foreach (var item in this.Notices)
                {
                    if (string.IsNullOrEmpty(item.ID))
                    {
                        item.ID = PKeySigner.Pick(PkeyType.Notices);
                    }

                    //兼容再次异常后再次提交的，这个不能去掉。
                    if (string.IsNullOrEmpty(this.WaybillID))
                    {
                        this.WaybillID = item.WaybillID;
                    }

                    item.WaybillID = this.WaybillID;

                    if (string.IsNullOrEmpty(item.Output.ID))
                    {
                        item.Output.ID = PKeySigner.Pick(Yahv.Underly.PKeyType.Output);
                    }

                    item.OutputID = item.Output.ID;
                    item.Output.CreateDate = DateTime.Now;

                    if (item.Files != null)
                    {
                        foreach (var tem in item.Files)
                        {
                            //tem.ID = PKeySigner.Pick(PkeyType.FileInfos);
                            tem.NoticeID = item.ID;
                            tem.WaybillID = this.WaybillID;
                            if (item.Output != null)
                            {
                                tem.StorageID = item.StorageID;
                            }
                            files.Add(tem);
                        }
                    }
                    item.CreateDate = DateTime.Now;
                    item.Status = Yahv.Services.Enums.NoticesStatus.Waiting;

                    forms.Add(new Form
                    {
                        ID = PKeySigner.Pick(PkeyType.StoragesForm),        //唯一码
                        StorageID = item.StorageID,        //库存ID
                        Quantity = 0 - item.Quantity,        //数量
                        NoticeID = item.ID,        //通知ID
                        Status = FormStatus.Frozen,        //冻结：Frozen,真实的（真正执行的）：Facted});
                    });
                }

                if (this.Files != null)
                {
                    foreach (var item in this.files)
                    {
                        item.WaybillID = this.WaybillID;
                        files.Add(item);
                    }
                }

                var storageids = this.Notices.Select(item => item.StorageID).Distinct().ToArray();

                if (rep.ReadTable<Layers.Data.Sqls.PvWms.Forms>().Where(item => storageids.Contains(item.StorageID)).Select(item => new { item.StorageID, item.Quantity }).GroupBy(item => item.StorageID).Where(item => item.Sum(tem => tem.Quantity) < 0).Any())
                {
                    LackStockEvent?.Invoke(this, new EventArgs());

                    throw new Exception("库存不足");
                }


                if (this.WayChcd != null)
                {
                    this.WayChcd.ID = this.WaybillID;
                    using (var reponsitory = new Layers.Data.Sqls.PvCenterReponsitory())
                    {
                        var count = reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.WayChcd>().Count(item => item.ID == this.WaybillID);
                        if (count <= 0)
                        {
                            reponsitory.Insert(new Layers.Data.Sqls.PvCenter.WayChcd
                            {
                                ID = this.WaybillID,
                                LotNumber = this.WayChcd.LotNumber,
                                CarNumber1 = this.WayChcd.CarNumber1,
                                CarNumber2 = this.WayChcd.CarNumber2,
                                Carload = this.WayChcd.Carload ?? 0,
                                IsOnevehicle = this.WayChcd.IsOnevehicle ?? false,
                                Driver = this.WayChcd.Driver,
                                PlanDate = DateTime.Now,
                                DepartDate = DateTime.Now,
                                TotalQuantity = this.WayChcd.TotalQuantity ?? 0,



                            });
                        }
                        else
                        {
                            reponsitory.Update(new Layers.Data.Sqls.PvCenter.WayChcd
                            {
                                LotNumber = this.WayChcd.LotNumber,
                                CarNumber1 = this.WayChcd.CarNumber1,
                                CarNumber2 = this.WayChcd.CarNumber2,
                                Carload = this.WayChcd.Carload ?? 0,
                                IsOnevehicle = this.WayChcd.IsOnevehicle ?? false,
                                Driver = this.WayChcd.Driver,
                                PlanDate = DateTime.Now,
                                DepartDate = DateTime.Now,
                                TotalQuantity = this.WayChcd.TotalQuantity ?? 0
                            }, item => item.ID == this.WaybillID);
                        }
                    }


                }




                using (var trans = new TransactionScope())
                {
                    try
                    {

                        //保存通知
                        rep.Insert(this.Notices.Select(item => new Layers.Data.Sqls.PvWms.Notices
                        {
                            ID = item.ID,        //唯一码
                            Type = (int)CgNoticeType.Picking,        //通知类型：入库通知、出库通知、分拣通知、检测通知、捡货通知、客户自提通知、装箱通知
                            WareHouseID = item.WareHouseID,        //仓库编号
                            WaybillID = item.WaybillID,        //运单编号
                            InputID = item.InputID,        //进项编号
                            OutputID = item.OutputID,        //销项编号
                            ProductID = item.ProductID,        //产品编号
                            Supplier = item.Supplier,        //
                            DateCode = item.DateCode,        //批次号
                            Quantity = item.Quantity,        //数量
                            Conditions = item.Conditions.Json(),        //条件
                            CreateDate = DateTime.Now,        //创建时间
                            Status = (int)item.Status,        //状态：等待Waiting、关闭Closed、（货物）丢失Lost、完成Completed
                            Source = (int)item.Source,        //来源
                            Target = (int)item.Target,        //目标
                            BoxCode = item.BoxCode,        //箱号
                            Weight = item.Weight,        //重量
                            Volume = item.Volume,        //体积
                            ShelveID = item.ShelveID,        //货架编号

                        }).ToArray());

                        //保存froms

                        //rep.Insert(this.Notices.Select(item => new Layers.Data.Sqls.PvWms.Forms
                        //{
                        //    ID = item.ID,        //唯一码
                        //    StorageID = item.Output.StorageID,        //库存ID
                        //    Quantity = 0 - item.Quantity,        //数量
                        //    NoticeID = item.ID,        //通知ID
                        //    Status = (int)FormStatus.Frozen,        //冻结：Frozen,真实的（真正执行的）：Facted});
                        //}).ToArray());

                        rep.Insert(forms.Select(item => new Layers.Data.Sqls.PvWms.Forms
                        {
                            ID = item.ID,        //唯一码
                            StorageID = item.StorageID,        //库存ID
                            Quantity = item.Quantity,        //数量
                            NoticeID = item.NoticeID,        //通知ID
                            Status = (int)FormStatus.Frozen,        //冻结：Frozen,真实的（真正执行的）：Facted});
                        }).ToArray());



                        //保存销项                        
                        rep.Insert(this.Notices.Select(item => new Layers.Data.Sqls.PvWms.Outputs
                        {
                            ID = item.Output.ID,        //四位年+2位月+2日+6位流水
                            InputID = item.Output.InputID,        //
                            OrderID = item.Output.OrderID,        //MainID
                            TinyOrderID = item.Output.TinyOrderID,
                            ItemID = item.Output.ItemID,        //项ID
                            OwnerID = item.Output.OwnerID ?? "",        //法人
                            SalerID = item.Output.SalerID,        //AdminID
                            CustomerServiceID = item.Output.CustomerServiceID,        //跟单员
                            PurchaserID = item.Output.PurchaserID,        //
                            Currency = (int)(item.Output.Currency ?? Currency.HKD),        //保值
                            Price = item.Output.Price ?? 0,        //保值
                            CreateDate = DateTime.Now,        //发生时间

                        }).ToArray());


                        //更新文件
                        if (this.files != null)
                        {
                            this.files.ToArray().Update(rep);
                        }

                        //提交事务
                        trans.Complete();

                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }
                }


                //try
                //{

                //    string api = string.Concat(FromType.ArrivalInfoToXDT.GetDescription());

                //    var obj = new { VastOrderID = this.OrderID };

                //    var result = Yahv.Utils.Http.ApiHelper.Current.JPost(api, obj);

                //    using (PvCenterReponsitory reponsitory = new PvCenterReponsitory())
                //    {
                //        reponsitory.Insert<Layers.Data.Sqls.PvCenter.Logs_Operating>(new Layers.Data.Sqls.PvCenter.Logs_Operating()
                //        {
                //            ID = Guid.NewGuid().ToString(),
                //            Type = (int)Yahv.Services.Enums.LogType.WsOrder,
                //            MainID = this.OrderID,
                //            Operation = "库房到货信息告知芯达通！",
                //            Creator = "NPC",
                //            CreateDate = DateTime.Now,
                //            Summary = result
                //        });
                //        reponsitory.Submit();
                //    }

                //}
                //catch (Exception ex)
                //{

                //    using (PvCenterReponsitory reponsitory = new PvCenterReponsitory())
                //    {
                //        reponsitory.Insert<Layers.Data.Sqls.PvCenter.Logs_Operating>(new Layers.Data.Sqls.PvCenter.Logs_Operating()
                //        {
                //            ID = Guid.NewGuid().ToString(),
                //            Type = (int)Yahv.Services.Enums.LogType.WsOrder,
                //            MainID = this.OrderID,
                //            Operation = "库房到货信息告知芯达通",
                //            Creator = "NPC",
                //            CreateDate = DateTime.Now,
                //            Summary = ex.Message,
                //        });
                //        reponsitory.Submit();
                //    }

                //}

            }

        }

        /// <summary>
        /// 确认出库
        /// </summary>
        /// <param name="orderid"></param>
        /// <param name="itemid"></param>
        public void ConfirmOrder(string orderid, string itemid = null)
        {
            throw new Exception("现有逻辑错误，当前Outputs表中不再包含StorageID字段");
            /*
            using (var rep = new Layers.Data.Sqls.PvWmsRepository())
            {

                Expression<Func<Layers.Data.Sqls.PvWms.Outputs, bool>> exp = item => item.OrderID == orderid;
                if (!string.IsNullOrEmpty(itemid))
                {
                    exp = exp.And(item => item.ItemID == itemid);
                }
                var linq = from output in rep.ReadTable<Layers.Data.Sqls.PvWms.Outputs>().Where(exp)
                           join notice in rep.ReadTable<Layers.Data.Sqls.PvWms.Notices>() on output.ID equals notice.OutputID
                           select new { output.StorageID, notice.ID };

                var noticeids = linq.Select(item => item.ID).Distinct().ToArray();
                var storageids = linq.Select(item => item.StorageID).Distinct().ToArray();

                rep.Update(new Layers.Data.Sqls.PvWms.Forms { Status = (int)FormStatus.Facted }, item => storageids.Contains(item.StorageID) && noticeids.Contains(item.NoticeID));

            }
            */
        }

        /// <summary>
        /// 取消出库
        /// </summary>
        /// <param name="orderid"></param>
        /// <param name="itemid"></param>
        public void CancelOrder(string orderid, string itemid = null)
        {
            throw new Exception("现有逻辑错误，当前Outputs表中不再包含StorageID字段");
            /*
            using (var rep = new Layers.Data.Sqls.PvWmsRepository())
            {

                Expression<Func<Layers.Data.Sqls.PvWms.Outputs, bool>> exp = item => item.OrderID == orderid;
                if (!string.IsNullOrEmpty(itemid))
                {
                    exp = exp.And(item => item.ItemID == itemid);
                }
                var linq = from output in rep.ReadTable<Layers.Data.Sqls.PvWms.Outputs>().Where(exp)
                           join notice in rep.ReadTable<Layers.Data.Sqls.PvWms.Notices>() on output.ID equals notice.OutputID
                           select new { output.StorageID, notice.ID };

                var noticeids = linq.Select(item => item.ID).Distinct().ToArray();
                var storageids = linq.Select(item => item.StorageID).Distinct().ToArray();

                using (var trans = new TransactionScope())
                {
                    try
                    {
                        //删除锁定的库存
                        rep.Delete<Layers.Data.Sqls.PvWms.Forms>(item => storageids.Contains(item.StorageID) && noticeids.Contains(item.NoticeID));
                        //删除捡货信息（装箱时产生）
                        rep.Delete<Layers.Data.Sqls.PvWms.Pickings>(item => storageids.Contains(item.StorageID) && noticeids.Contains(item.NoticeID));
                        rep.Submit();

                        trans.Complete();
                    }
                    catch
                    {

                    }
                }

            }
            */
        }

        public event EventHandler OutOfStock;

        public static object lockStoreObj = new object();

        /// <summary>
        /// 锁库
        /// </summary>
        /// <param name="orderid"></param>
        /// <param name="itemid"></param>
        public void LockStore(Yahv.Services.Models.LockStoreParam entitiy)
        {
            if (OutOfStock == null)
            {
                throw new NotImplementedException("未实现");
            }

            lock (lockStoreObj)
            {
                using (var rep = new Layers.Data.Sqls.PvWmsRepository())
                {
                    foreach (var item in entitiy.Params)
                    {
                        var stockQuantity = rep.ReadTable<Layers.Data.Sqls.PvWms.Forms>().Where(tem => tem.StorageID == item.StorageID).Sum(tem => tem.Quantity);
                        if (stockQuantity < item.Quantity)
                        {
                            OutOfStock(this, new EventArgs());
                            return;
                        }
                    }

                    foreach (var item in entitiy.Params)
                    {
                        item.ID = PKeySigner.Pick(Wms.Services.PkeyType.StoragesForm);
                    }


                    rep.Insert(entitiy.Params.Select(item => new Layers.Data.Sqls.PvWms.Forms { ID = item.ID, SessionID = item.SessionID, StorageID = item.StorageID, Quantity = 0 - item.Quantity, Status = (int)FormStatus.Frozen }).ToArray());
                }

            }
        }

        /// <summary>
        /// 取消锁库
        /// </summary>
        /// <param name="orderid"></param>
        /// <param name="itemid"></param>
        public void CancelLockStore(Yahv.Services.Models.LockStoreParam entity)
        {
            var sessionids = entity.Params.Select(item => item.SessionID).Distinct();
            using (var rep = new Layers.Data.Sqls.PvWmsRepository())
            {
                rep.Delete<Layers.Data.Sqls.PvWms.Forms>(item => sessionids.Contains(item.SessionID));
            }
        }
    }

    public class PickingNotice : Yahv.Services.Models.PickingNotice
    {


    }
}