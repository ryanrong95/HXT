using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services;
using Wms.Services.Enums;
using Wms.Services.Models;
using Wms.Services.Views;
using Yahv.Services.Enums;
using Yahv.Services.Models;
using Yahv.Services.Models.PvCenter;
using Yahv.Underly;
using Yahv.Underly.Erps;
using Yahv.Underly.Logs;
using Yahv.Usually;
using Yahv.Utils.Serializers;

namespace Yahv.Systematic
{
    public class WareHourse : IAction
    {

        #region 事件

        public event EventHandler<EventArgs> LackStockEvent;

        #region 暂存事件
        //暂存成功
        public event SuccessHanlder TempStorageSuccess;

        //暂存失败
        public event ErrorHanlder TempStorageFailed;

        //statistics


        public event EventHandler<GeneralEventArgs> TransferEvent;
        public event EventHandler<GeneralEventArgs> NOArrivalEvent;
        //登录过期事件
        public event EventHandler<GeneralEventArgs> LogonExpEvent;

        public event EventHandler<GeneralEventArgs> BreakCustomEvent;

        public event EventHandler<GeneralEventArgs> NoBoxCodeEvent;
        #endregion

        #region 运单事件
        public event SuccessHanlder WaybillExist;
        //运单不存在
        public event ErrorHanlder WaybillNotExist;

        //运单关闭
        public event EventHandler WaybillClosed;

        //运单已出库
        public event EventHandler WaybillOutStock;
        #endregion

        #region 租赁通知事件
        //租赁通知录入成功
        public event SuccessHanlder LsnoticeSuccess;

        //租赁通知录入失败
        public event ErrorHanlder LsnoticeFailed;

        //库位不能被使用（已被签过合同）
        public event ErrorHanlder ShelveNoUse;

        //库位重复
        public event ErrorHanlder ShelveRepeated;


        #endregion

        #region 深圳入库PDA事件
        public event SuccessHanlder NoticeSuccess;
        public event ErrorHanlder NoticeFailed;
        #endregion

        #region 根据运输批次号入库事件
        public event SuccessHanlder OutputSuccess;
        public event ErrorHanlder OutputFailed;
        public event ErrorHanlder NotCutting;//未截单
        public event ErrorHanlder StoQuantityLess;//库存数量不足，无法出库

        #endregion

        #region 删除出库通知事件
        //该通知不存在
        public event ErrorHanlder NoticeNotExist;
        #endregion

        #endregion

        IErpAdmin admin;
        public WareHourse(IErpAdmin admin)
        {
            if (admin == null)
            {
                throw new Exception("没有登录或登录已过期！");
            }
            this.admin = admin;
        }


        //public object BoxProducts(string whid, int all, int status = 0, string key = null, int pageindex = 1, int pagesize = 20)
        //{
        //    return new Wms.Services.WayBillServices(this.admin).BoxProducts(whid, all, status, key, pageindex, pagesize);
        //}

        //public object TinyOrderProducts(string whid, int all, int status = 0, string key = null, int pageindex = 1, int pagesize = 20)
        //{
        //    return new Wms.Services.WayBillServices(this.admin).TinyOrderProducts(whid, all, status, key, pageindex, pagesize);
        //}


        ///// <summary>
        ///// 更改箱号
        ///// </summary>
        ///// <param name="whid"></param>
        ///// <param name="oldCode"></param>
        ///// <param name="newCode"></param>

        //public void ChangeBoxCode(string whid, string oldCode, string newCode)
        //{
        //    new Wms.Services.WayBillServices(this.admin).ChangeBoxCode(whid, oldCode, newCode);
        //}

        //public void EnterWaybillBox(WaybillBox entity)
        //{
        //    entity.Enter();
        //}

        //public void EnterWaybillBox(object entity)
        //{
        //    throw new Exception();
        //    //entity.Enter();
        //}

        public void UpdateVoyage(Voyage entity)
        {
            entity.Enter();
        }

        public void FeeEnter(Fee fee)
        {
            new Fee(this.admin)
            {
                Conduct = fee.Conduct,
                WaybillID = fee.WaybillID,
                OrderID = fee.OrderID,
                TinyOrderID = fee.TinyOrderID,
                Payer = fee.Payer,
                Payee = fee.Payee,
                LeftPrice = fee.LeftPrice,
                RightPrice = fee.RightPrice,
                Currency = fee.Currency,
                FeeType = fee.FeeType,
                Subject = fee.Subject,
                ArrivalBatch = fee.ArrivalBatch,
                Files = fee.Files,
                Quantity = fee.Quantity,
                Anonymity = fee.Anonymity,
                Source = fee.Source,
                TrackingNumber = fee.TrackingNumber,
                Catalog = fee.Catalog,
                Data = fee.Data,
            }.Enter();
        }

        public void SortingEnter(object obj, string Summary, int Status)
        {
            var waybill = obj.ToString().JsonTo<SortingWaybill>();

            using (PvCenterReponsitory rep = new PvCenterReponsitory())
            {
                if (rep.ReadTable<Layers.Data.Sqls.PvCenter.Waybills>().Where(item => item.ID == waybill.WaybillID).FirstOrDefault().Status == (int)GeneralStatus.Closed)
                {
                    this.WaybillClosed(this, new EventArgs());
                    return;
                }
            }

            if ((SortingExcuteStatus)Status == SortingExcuteStatus.Stocked)
            {
                if (waybill.Notices.Where(item => item.Enabled && !string.IsNullOrEmpty(item.TruetoQuantity.ToString()) && item.TruetoQuantity > 0).Count() <= 0)
                {
                    NOArrivalEvent(this, new GeneralEventArgs("没有填写到货数量"));
                    return;
                }
            }

            foreach (var item in waybill.Notices.Where(item => item.ID.StartsWith("CX")))
            {
                item.IsOriginalNotice = false;
            }

            if (waybill.Source == NoticeSource.AgentBreakCustoms)
            {
                if (waybill.Notices.Where(item => item.IsOriginalNotice).Sum(item => item.Quantity) < waybill.Notices.Sum(item => item.TruetoQuantity))
                {
                    BreakCustomEvent(this, new GeneralEventArgs("实际到货数量不能大于通知的数量!"));
                    return;
                }

                if (waybill.Notices.Where(item => item.TruetoQuantity > 0).Any(item => string.IsNullOrEmpty(item.BoxCode)))
                {
                    NoBoxCodeEvent(this, new GeneralEventArgs("请选择箱号!"));
                    return;
                }
            }

            var nType = waybill.Notices.First().Type;
            if (nType == NoticeType.TransferEnter)
            {
                if (TransferEvent == null)
                {
                    throw new NotImplementedException("没有实现转运事件");
                }
            }

            //var service = new Wms.Services.WayBillServices(this.admin);

            //service.Enter(waybill, Summary, Status);

            //if (nType == NoticeType.TransferEnter)
            //{
            //    TransferEvent(this, new GeneralEventArgs(waybill.TransferID));
            //}
        }



        public void TakeGoods(string waybillid)
        {
            //new Wms.Services.WayBillServices(this.admin).TakeGoods(waybillid);
        }

        public void BoxEnter(Wms.Services.Models.Boxes entity)
        {

            new Wms.Services.Models.Boxes(this.admin)
            {
                DateStr = entity.DateStr,
                Summary = entity.Summary,
                WarehouseID = entity.WarehouseID,
                CodePrefix = entity.CodePrefix,


            }.Enter();
        }

        public void UpdateWaybillCode(string waybillids, string code)
        {
            //new Wms.Services.WayBillServices(this.admin).UpdateWayBillCode(waybillids, code);
        }


        public void UpdateCuttingOrderStatus(string[] waybillids, int cuttingOrderStatus)
        {
            //new Wms.Services.WayBillServices(this.admin).UpdateCuttingOrderStatus(waybillids, cuttingOrderStatus);
        }

        public void CustomsApply(string whid, string[] boxids)
        {

            //var model = new DeclarationApply { AdminID = this.admin.ID, Items = new Wms.Services.WayBillServices(this.admin).BoxdeClareCustomApply(whid, boxids) };
            //Yahv.Utils.Http.ApiHelper.Current.PostData(Wms.Services.FromType.CustomApply.GetDescription(), model);
            //using (var rep = new Layers.Data.Sqls.PvWmsRepository())
            //{
            //    rep.Update<Layers.Data.Sqls.PvWms.Boxes>(new { Status = (int)BoxesStatus.Declared }, item => boxids.Contains(item.ID));
            //}


        }

        public void UpdateWayBillInfo(string ID, string Code, int? TotalParts, decimal? TotalWeight, decimal? TotalVolume, string CarrierID)
        {
            //new Wms.Services.WayBillServices(this.admin).UpdateWayBillInfo(ID, Code, TotalParts, TotalWeight, TotalVolume, CarrierID);
        }


        public void UpdateInfoByInputID(InfoByInput entity)
        {
            //new Wms.Services.WayBillServices(this.admin).UpdateInfoByInputID(entity);
        }

        public IList<Boxes> Boxes()
        {
            return new Wms.Services.Views.BoxesView().Where(item => item.AdminID == admin.ID).OrderByDescending(item => item.ID).ToList();
        }



        public void PickingEnter(Wms.Services.Models.PickingWaybill waybill, string Summary, int Status)
        {
            throw new Exception("逻辑错误，当前Outputs表中不再包含StorageID字段");
            /*
            using (var rep = new PvCenterReponsitory())
            {

                if (rep.ReadTable<Layers.Data.Sqls.PvCenter.Waybills>().Where(item => item.ID == waybill.WaybillID).FirstOrDefault().Status == (int)GeneralStatus.Closed)
                {
                    //订单关闭
                    WaybillClosed(this, new EventArgs());
                    return;
                }

            }


            var storageids = waybill.Notices.Select(item => item.Output.StorageID).Distinct().ToArray();

            if (new PvWmsRepository().ReadTable<Layers.Data.Sqls.PvWms.Forms>().Where(item => storageids.Contains(item.StorageID)).Select(item => new { item.StorageID, item.Quantity }).GroupBy(item => item.StorageID).Where(item => item.Sum(tem => tem.Quantity) < 0).Any())
            {
                LackStockEvent?.Invoke(this, new EventArgs());
                return;
            }

            var service = new Wms.Services.WayBillServices(this.admin);

            service.PickEnter(waybill, Summary, Status);


            if (waybill.Notices.First().WareHouseID.IndexOf("SZ") > 0)
            {
                try
                {
                    //深圳库房出库成功后调用华芯通的接口
                    var result = Yahv.Utils.Http.ApiHelper.Current.JPost(Wms.Services.FromType.SZOutOfStockToXDT.GetDescription(), new SZToXDT { AdminID = this.admin.ID, WaybillID = waybill.WaybillID });

                }
                catch
                {


                }
            }
            */
        }

        public void Logs_Error(Logs_Error log)
        {


            using (var repository = new HvRFQReponsitory())
            {
                repository.Insert<Layers.Data.Sqls.HvRFQ.Logs_Errors>(new Layers.Data.Sqls.HvRFQ.Logs_Errors
                {
                    AdminID = admin.ID,
                    Page = log.Page,
                    Message = log.Message,
                    Codes = log.Codes,
                    Source = log.Source,
                    Stack = log.Stack,
                    CreateDate = log.CreateDate
                });
            }
        }

        public void Check(string outputID)
        {
            //new Wms.Services.WayBillServices(this.admin).Check(outputID);

        }


        public void WaybillIsExist(string waybillID)
        {
            using (PvCenterReponsitory rep = new PvCenterReponsitory())
            {

                var waybill = rep.ReadTable<Layers.Data.Sqls.PvCenter.Waybills>().Where(item => item.ID == waybillID).FirstOrDefault();

                if (waybill == null)
                {
                    if (this != null && this.WaybillNotExist != null)
                    {
                        //运单不存在
                        this.WaybillNotExist(this, new ErrorEventArgs("Waybill Not Exist!!"));
                        return;
                    }
                }

                else if (waybill.Status == (int)GeneralStatus.Closed)
                {
                    if (this != null && this.WaybillClosed != null)
                    {
                        //订单关闭
                        this.WaybillClosed(this, new EventArgs());
                        return;
                    }
                }
                else if (waybill.ExcuteStatus == (int)PickingExcuteStatus.OutStock)
                {
                    if (this != null && this.WaybillOutStock != null)
                    {
                        //运单已出库
                        this.WaybillOutStock(this, new EventArgs());
                        return;
                    }
                }
                else
                {
                    if (this != null && this.WaybillExist != null)
                    {
                        //运单存在
                        this.WaybillExist(this, new SuccessEventArgs("Waybill Exist!!"));
                        return;
                    }
                }

            }
        }

        public void UpdateFile(string id, string waybillID, string customName, int type)
        {
            //var service = new Wms.Services.WayBillServices(this.admin);
            //service.UpdateFile(id, waybillID, customName, type);
        }

        public void DeleteFile(string id)
        {
            //var service = new Wms.Services.WayBillServices(this.admin);
            //service.DeleteFile(id);
        }


        /// <summary>
        /// 操作日志
        /// </summary>
        /// <param name="log"></param>
        public void Logs_Operate(Log_Operating log)
        {

            new Yahv.Services.Views.Logs_OperatingTopView<PvCenterReponsitory>().Add(new Yahv.Services.Models.Log_Operating
            {
                ID = Guid.NewGuid().ToString(),
                Type = log.Type,
                CreateDate = DateTime.Now,
                Creator = admin.ID,
                MainID = log.MainID,
                Operation = log.Operation,
                Summary = ""
            });

        }

        #region 报关运输出库
        /// <summary>
        /// 报关运输出库
        /// </summary>
        /// <param name="lotnumber"></param>
        public void OutputEnter(/*string warehouseID,*/ string lotnumber)
        {
            //var servieces = new CustomServieces(admin);
            //servieces.OutputSuccess += Servieces_OutputSuccess;
            //servieces.OutputFailed += Servieces_OutputFailed;
            //servieces.NotCutting += Servieces_NotCutting;
            //servieces.StoQuantityLess += Servieces_StoQuantityLess;
            //servieces.OutputEnter(/*warehouseID,*/ lotnumber);
        }

        private void Servieces_StoQuantityLess(object sender, ErrorEventArgs e)
        {
            this.StoQuantityLess(sender, e);
        }

        private void Servieces_NotCutting(object sender, ErrorEventArgs e)
        {
            this.NotCutting(sender, e);
        }

        private void Servieces_OutputFailed(object sender, ErrorEventArgs e)
        {
            this.OutputFailed(sender, e);
        }

        private void Servieces_OutputSuccess(object sender, SuccessEventArgs e)
        {
            this.OutputSuccess(sender, e);
        }

        #endregion

        #region 暂存

        public void TempStorageEnter(TempStorageWaybill waybill)
        {

            //var storageService = new Wms.Services.StorageServices(this.admin);

            //storageService.Success += TempStorage_Success;
            //storageService.Failed += TempStorage_Failed;
            //storageService.TempStorageEnter(waybill);
        }

        private void TempStorage_Failed(object sender, ErrorEventArgs e)
        {
            TempStorageFailed(sender, e);
        }

        private void TempStorage_Success(object sender, SuccessEventArgs e)
        {
            TempStorageSuccess(sender, e);
        }

        #endregion

        #region 租赁通知
        public void LsNoticeEnter(string orderID, LsNotice[] lsnotices)
        {
            //var lsNoticeService = new Wms.Services.LsNoticeServices(this.admin);
            //lsNoticeService.Success += LsNoticeService_Success; ;
            //lsNoticeService.Failed += LsNoticeService_Failed;
            //lsNoticeService.ShelveNotUse += LsNoticeService_ShelveNotUse;
            //lsNoticeService.ShelveRepeated += LsNoticeService_ShelveRepeated;

            //lsNoticeService.LsNoticeEnter(orderID, lsnotices);
        }

        //public void LsNoticeSubmit(LsNotice[] lsNotices)
        //{
        //    var lsNoticeService = new Wms.Services.LsNoticeServices(this.admin);
        //    lsNoticeService.Submit(lsNotices);
        //}

        private void LsNoticeService_ShelveRepeated(object sender, ErrorEventArgs e)
        {
            this.ShelveRepeated(sender, e);
        }

        private void LsNoticeService_ShelveNotUse(object sender, ErrorEventArgs e)
        {
            this.ShelveNoUse(sender, e);
        }

        private void LsNoticeService_Failed(object sender, ErrorEventArgs e)
        {
            this.LsnoticeFailed(sender, e);
        }

        private void LsNoticeService_Success(object sender, SuccessEventArgs e)
        {
            this.LsnoticeSuccess(sender, e);
        }
        #endregion

        #region 深圳入库(目前PDA功能用)
        public void NoticeEnter(string warehouseID, string shelveID, PDANotices[] notices)
        {
            var serviece = new Wms.Services.NoticeServices(this.admin);
            serviece.Success += Serviece_Success;
            serviece.Failed += Serviece_Failed;
            serviece.Enter(warehouseID, shelveID, notices);
        }

        public void NoticeEnter(string warehouseID, string shelveID, string key)
        {
            var serviece = new Wms.Services.NoticeServices(this.admin);
            serviece.Success += Serviece_Success;
            serviece.Failed += Serviece_Failed;
            serviece.Enter(warehouseID, shelveID, key);
        }

        private void Serviece_Failed(object sender, ErrorEventArgs e)
        {
            this.NoticeFailed(sender, e);
        }

        private void Serviece_Success(object sender, SuccessEventArgs e)
        {
            this.NoticeSuccess(sender, e);
        }

        #endregion

        #region 发票文件增和删
        public void AddNoticeFiles(InvoiceNoticeFiles[] files)
        {
            var invoice = new Wms.Services.InvoiceServieces();
            invoice.InsertInvoiceNoticeFiles(files);
        }

        public void DeleteNoticeFiles(string[] invoiceNoticeFileIDs)
        {
            var invoice = new Wms.Services.InvoiceServieces();
            invoice.DeleteInvoiceNoticeFiles(invoiceNoticeFileIDs);
        }

        #endregion

        public void DeleteOutStock(string noticeID)
        {
            //using (var rep = new PvWmsRepository())
            //{
            //    var notice = new PickingNoticesView().Where(item => item.ID == noticeID).FirstOrDefault();
            //    if (notice == null)
            //    {
            //        if (this != null && this.NoticeNotExist != null)
            //        {
            //            this.NoticeNotExist(this, new ErrorEventArgs("Notice not exist!!"));
            //            return;
            //        }
            //    }
            //}

            //var service = new WayBillServices(this.admin);
            //service.DeleteOutStock(noticeID);
        }
    }
}
