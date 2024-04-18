using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Needs.Wl;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System.Configuration;
using Newtonsoft.Json;

namespace WebApp.Order
{
    /// <summary>
    /// 订单详情界面2
    /// </summary>
    public partial class Detail2 : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        public void LoadData()
        {
            string ID = Request.QueryString["ID"];
            var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.Orders[ID];
            var waybills = order.Waybills.Select(item => new { CarrierCode = item.Carrier.Code, item.WaybillCode });
            var invoice = order.Client.Invoice ?? new Needs.Ccs.Services.Models.ClientInvoice();

            //运输批次包车记录
            var orderVoyage = Needs.Wl.Admin.Plat.AdminPlat.Current.Voyage.OrderVoyageNo.Where(t =>
                    t.Order.ID == ID && t.Status == Status.Normal &&
                    t.Type == Needs.Ccs.Services.Enums.OrderSpecialType.CharterBus)
                .OrderByDescending(t => t.CreateTime).FirstOrDefault();

            //订单下单是否包车
            var isFullVehicle = order.IsFullVehicle;

            this.Model.OrderInfo = new
            {
                order.ID,
                CompanyName = order.Client.Company.Name,
                DeclarePrice = order.DeclarePrice.ToRound(2).ToString("0.00"),
                Currency = order.Currency,
                OrderStatus = order.OrderStatus.GetDescription(),
                InvoiceType = order.ClientAgreement.InvoiceType.GetDescription(),
                CreateDate = order.CreateDate.ToShortDateString(),
                Consignee = order.OrderConsignee,
                ConsigneeType = order.OrderConsignee.Type.GetDescription(),
                Waybills = waybills,
                Consignor = order.OrderConsignor,
                ConsignorType = order.OrderConsignor.Type.GetDescription(),
                IDType = string.IsNullOrWhiteSpace(order.OrderConsignor.IDType) ? null :
                        ((Needs.Ccs.Services.Enums.IDType)Enum.Parse(typeof(Needs.Ccs.Services.Enums.IDType), order.OrderConsignor.IDType)).GetDescription(),
                //订单是否包车
                IsFullVehicle = order.IsFullVehicle ? "是" : "否",
                VoyageIsFullVehicle = orderVoyage != null ? true : false,
                IsLoan = order.IsLoan ? "是" : "否",
                PackNo = order.PackNo?.ToString() ?? "",
                WarpType = Needs.Wl.Admin.Plat.AdminPlat.BasePackType.Where(pack => pack.Code == order.WarpType).FirstOrDefault()?.Name ?? order.WarpType,

                PayExchangeSuppliers = order.PayExchangeSuppliers,
                //  DeliveryFile = FileDirectory.Current.FileServerUrl + "/" + order.MainOrderFiles.Where(file => file.FileType == FileType.DeliveryFiles).FirstOrDefault()?.Url.ToUrl(),
                DeliveryFile = (DateTime.Compare(order.CreateDate, Convert.ToDateTime(FileDirectory.Current.IsChainsDate)) > 0)
                    ? FileDirectory.Current.PvDataFileUrl + "/" + order.MainOrderFiles.Where(file => file.FileType == FileType.DeliveryFiles).FirstOrDefault()?.Url.ToUrl()
                    : FileDirectory.Current.FileServerUrl + "/" + order.MainOrderFiles.Where(file => file.FileType == FileType.DeliveryFiles).FirstOrDefault()?.Url.ToUrl(),
                Invoice = invoice,
                InvoiceDeliveryType = (((int)invoice.DeliveryType == 0) ? InvoiceDeliveryType.SendByPost : invoice.DeliveryType).GetDescription(),
                InvoiceConsignee = order.Client.InvoiceConsignee
            }.Json().Replace("'", "#39;");

            this.Model.OrderStatus = order.OrderStatus;
        }

        protected void data()
        {
            string ID = Request.QueryString["ID"];

            var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.Orders[ID];

            Needs.Ccs.Services.Views.OrderDetailOrderItemsView view = new Needs.Ccs.Services.Views.OrderDetailOrderItemsView(order.ID, order.Type);
            view.AllowPaging = false;
            IList<Needs.Ccs.Services.Views.OrderDetailOrderItemsModel> list = view.ToList();

            CheckOrderItemIsShowModifyBtn(order, list, view);

            Func<Needs.Ccs.Services.Views.OrderDetailOrderItemsModel, object> convert = item => new
            {
                ID = item.OrderItemID,
                Name = !string.IsNullOrEmpty(item.OrderItemCategoryName) ? item.OrderItemCategoryName : item.OrderItemName,
                Manufacturer = item.Manufacturer,
                Model = item.Model,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice.ToString("0.0000"),
                TotalPrice = item.TotalPrice.ToRound(2).ToString("0.00"),
                Unit = item.Unit,
                Origin = item.Origin,
                GrossWeight = item.GrossWeight?.ToString("0.0000"),
                IsShowModifyBtn = item.IsShowModifyBtn,
                NotShowReason = item.NotShowReason,
            };

            Response.Write(new
            {
                rows = list.Select(convert).ToArray(),
                total = order.Items.Count()
            }.Json());




            //string ID = Request.QueryString["ID"];

            //var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.Orders[ID];

            //Func<Needs.Ccs.Services.Models.OrderItem, object> convert = item => new
            //{
            //    item.ID,
            //    Name = item.Category?.Name ?? item.Name,
            //    Manufacturer = item.Manufacturer,
            //    Model = item.Model,
            //    Quantity = item.Quantity,
            //    UnitPrice = item.UnitPrice.ToString("0.0000"),
            //    TotalPrice = item.TotalPrice.ToRound(2).ToString("0.00"),
            //    Unit = item.Unit,
            //    Origin = item.Origin,
            //    GrossWeight = item.GrossWeight?.ToString("0.0000")
            //};

            //Response.Write(new
            //{
            //    rows = order.Items.Select(convert).ToArray(),
            //    total = order.Items.Count()
            //}.Json());
        }

        private void CheckOrderItemIsShowModifyBtn(
            Needs.Ccs.Services.Models.Order order,
            IList<Needs.Ccs.Services.Views.OrderDetailOrderItemsModel> list,
            Needs.Ccs.Services.Views.OrderDetailOrderItemsView view)
        {
            if (order.OrderStatus == OrderStatus.Draft)
            {
                //订单状态草稿不能操作
                foreach (var item in list)
                {
                    item.IsShowModifyBtn = false;
                    item.NotShowReason = "订单状态：草稿";
                }

                return;
            }

            if (order.Type == OrderType.Icgoo || order.Type == OrderType.Inside)
            {
                bool decHeadIsSuccess = view.GetDecHeadIsSuccess();
                if (decHeadIsSuccess)
                {
                    //1. 报关完成不能操作
                    foreach (var item in list)
                    {
                        item.IsShowModifyBtn = false;
                        item.NotShowReason = "已报关完成";
                    }

                    return;
                }
            }
            else if (order.Type == OrderType.Outside)
            {
                Needs.Ccs.Services.Enums.EntryNoticeStatus hkEntryNoticeStatus = view.GetHkEntryNoticeStatus();
                if (hkEntryNoticeStatus == Needs.Ccs.Services.Enums.EntryNoticeStatus.Sealed)
                {
                    //1.订单已封箱，不能操作
                    foreach (var item in list)
                    {
                        item.IsShowModifyBtn = false;
                        item.NotShowReason = "订单已封箱";
                    }

                    return;
                }
            }

            //为了简化库房逻辑，验证转报关不允许删除型号和修改数量 ryan 20200910
            var turnOrder = new Needs.Ccs.Services.Views.WsOrderTopView().GetAll();
            if (turnOrder.Any(t=>t.ID == order.MainOrderID && t.Type == 4))
            {
                //1.转报关订单，不能操作
                foreach (var item in list)
                {
                    item.IsShowModifyBtn = false;
                    item.NotShowReason = "转报关订单不能操作";
                }

                return;
            }

            if (order.Type == OrderType.Outside)
            {
                //2. 型号已装箱，不能操作
                foreach (var item in list)
                {
                    if (item.IsHkPacked)
                    {
                        item.IsShowModifyBtn = false;
                        item.NotShowReason = "型号已装箱";
                    }
                }
            }
        }

        /// <summary>
        /// 初始化订单附件
        /// </summary>
        protected void dataFiles()
        {
            string orderID = Request.QueryString["ID"];
            if (!string.IsNullOrEmpty(orderID))
            {
                var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.Orders[orderID];
                var files = order.MainOrderFiles.Where(file => file.FileType == FileType.OriginalInvoice);
                var t1 = Convert.ToDateTime(FileDirectory.Current.IsChainsDate);
                Func<Needs.Ccs.Services.Models.MainOrderFile, object> convert = orderFile => new
                {
                    orderFile.ID,
                    orderFile.Name,
                    FileType = orderFile.FileType.GetDescription(),
                    orderFile.FileFormat,
                    Url = DateTime.Compare(orderFile.CreateDate, t1) > 0 ? FileDirectory.Current.PvDataFileUrl + "/" + orderFile.Url.ToUrl() :
                      FileDirectory.Current.FileServerUrl + "/" + orderFile.Url.ToUrl(),
                   // Url = FileDirectory.Current.FileServerUrl + "/" + orderFile.Url.ToUrl()
                };

                Response.Write(new
                {
                    rows = files.Select(convert).ToList(),
                    total = files.Count()
                }.Json());
            }
            else
            {
                Response.Write(new { }.Json());
            }
        }

        /// <summary>
        /// 取消包车
        /// </summary>
        protected void Cancel()
        {
            try
            {
                var orderId = Request.Form["OrderId"];
                var orderVoyage = Needs.Wl.Admin.Plat.AdminPlat.Current.Voyage.OrderVoyageNo.Where(t =>
                        t.Order.ID == orderId && t.Status == Status.Normal &&
                        t.Type == Needs.Ccs.Services.Enums.OrderSpecialType.CharterBus)
                    .OrderByDescending(t => t.CreateTime).FirstOrDefault();

                //无运输批次取消
                if (orderVoyage == null)
                {
                    return;
                }
                //已报关：提示此单已报关成功，取消包车失败
                if (orderVoyage.Order.OrderStatus >= OrderStatus.Declared)
                {
                    Response.Write((new { success = false, message = $"订单{orderId}已报关成功，取消包车失败" }).Json());
                }
                //未报关，已定车 提示//未定车删除
                else
                {
                    orderVoyage.AbandonSuccess += OrderVoyage_AbandonSuccess;
                    orderVoyage.Abandon();
                    Response.Write((new { success = true, message = "取消包车成功" }).Json());
                }

            }
            catch (Exception e)
            {
                Response.Write((new { success = false, message = "取消包车失败: " + e.Message }).Json());
            }
        }

        private void OrderVoyage_AbandonSuccess(object sender, SuccessEventArgs e)
        {
            var orderVoyage = (OrderVoyage)e.Object;
            var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
            var summary = "跟单员[" + admin.RealName + "]取消包车运输";
            orderVoyage.Order.Log(admin, summary);
        }

        /// <summary>
        /// 删除型号
        /// </summary>
        protected void DeleteModel()
        {
            try
            {
                string approveOnOff = ConfigurationManager.AppSettings["ApproveOnOff"];

                var OrderItemID = Request.Form["OrderItemID"];
                var OrderID = Request.Form["OrderID"];

                string Model = Request.Form["Model"];
                string Manufacturer = Request.Form["Manufacturer"];
                string Quantity = Request.Form["Quantity"];
                int QuantityInt = int.Parse(Quantity);

                string rtnMsg = string.Empty;

                //如果该小订单只有一个型号, 阻止删除型号 Begin
                int orderItemCount = new Needs.Ccs.Services.Views.Origins.OrderItemsOrigin().Where(t => t.OrderID == OrderID && t.Status == Status.Normal).Count();
                if (orderItemCount <= 1)
                {
                    Response.Write((new { success = false, message = "订单 " + OrderID + " 中只有一个型号，不能删除", }).Json());
                    return;
                }

                //如果该小订单只有一个型号, 阻止删除型号 End

                if (string.IsNullOrEmpty(approveOnOff) || "no" == approveOnOff)
                {
                    rtnMsg = DoDeleteModelPage(OrderItemID, OrderID);
                }
                else
                {
                    rtnMsg = PreApproveDeleteModel(approveOnOff, OrderID, OrderItemID, Model, Manufacturer, QuantityInt);
                }

                if (string.IsNullOrEmpty(rtnMsg))
                {
                    if (string.IsNullOrEmpty(approveOnOff) || "no" == approveOnOff)
                    {
                        Response.Write((new { success = true, message = "型号 " + Model + " 删除成功！", }).Json());
                    }
                    else
                    {
                        Response.Write((new { success = true, message = "请等待审批通过，可在“订单审批”中查看审批状态", }).Json());
                    }
                }
                else
                {
                    Response.Write((new { success = false, message = rtnMsg, }).Json());
                }
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "删除型号失败：" + ex.Message, }).Json());
            }
        }

        /// <summary>
        /// 页面上执行删除型号
        /// </summary>
        /// <param name="OrderItemID"></param>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        private string DoDeleteModelPage(string OrderItemID, string OrderID)
        {
            DeleteModelHandler deleteModelHandler = new DeleteModelHandler(OrderItemID, OrderID);
            bool result = deleteModelHandler.Execute();
            if (result)
            {

                if (deleteModelHandler.OrderStatus >= Needs.Ccs.Services.Enums.OrderStatus.Classified)
                {
                    try
                    {
                        GenerateBill(OrderID);
                    }
                    catch (Exception e)
                    {

                        throw new Exception("重新生成对账单内执行错误：" + e.Message);
                    }
                }
                else if (deleteModelHandler.OrderStatus == Needs.Ccs.Services.Enums.OrderStatus.Confirmed)
                {
                    deleteModelHandler.UpdateOrder(Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID), deleteModelHandler.OrderType);
                }

                //调用代仓储前端接口，删除型号
                deleteModelHandler.CallPvWsOrderConfirm(
                    new Admin
                    {
                        ID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID,
                        ErmAdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ErmAdminID
                    });

                return string.Empty;
            }
            else
            {
                return deleteModelHandler.Msg;
            }
        }

        /// <summary>
        /// 自动执行的删除型号（注意，该函数有地方通过反射调用）
        /// </summary>
        /// <param name="OrderItemID"></param>
        /// <param name="OrderID"></param>
        /// <param name="adminID"></param>
        public void DoDeleteModelAuto(string OrderItemID, string OrderID, Admin admin)
        {
            DeleteModelHandler deleteModelHandler = new DeleteModelHandler(OrderItemID, OrderID);
            bool result = deleteModelHandler.Execute();
            if (result)
            {

                if (deleteModelHandler.OrderStatus >= Needs.Ccs.Services.Enums.OrderStatus.Classified)
                {
                    try
                    {
                        GenerateBill(OrderID);
                    }
                    catch (Exception e)
                    {

                        throw new Exception("重新生成对账单内执行错误：" + e.Message);
                    }
                }
                else if (deleteModelHandler.OrderStatus == Needs.Ccs.Services.Enums.OrderStatus.Confirmed)
                {
                    deleteModelHandler.UpdateOrder(admin, deleteModelHandler.OrderType);
                }

                //调用代仓储前端接口，删除型号
                deleteModelHandler.CallPvWsOrderConfirm(
                    new Admin
                    {
                        ID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID,
                        ErmAdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ErmAdminID
                    });
            }
            else
            {
                //删除型号执行错误，记录日志
            }
        }

        /// <summary>
        /// 删除型号 PreApprove 操作
        /// </summary>
        /// <param name="approveOnOff"></param>
        /// <param name="OrderID"></param>
        /// <param name="OrderItemID"></param>
        /// <param name="Model"></param>
        /// <param name="Manufacturer"></param>
        /// <param name="QuantityInt"></param>
        /// <returns></returns>
        private string PreApproveDeleteModel(string approveOnOff, string OrderID, string OrderItemID, string Model, string Manufacturer, int QuantityInt)
        {
            string rtnMsg = string.Empty;

            var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

            string eventInfo = JsonConvert.SerializeObject(new EventInfoDeleteModel
            {
                ApplyAdminName = admin.RealName,
                TinyOrderID = OrderID,
                OrderItemID = OrderItemID,
                Model = Model,
                Manufacturer = Manufacturer,
                Quantity = QuantityInt,
            });

            var attachApproval = new Needs.Ccs.Services.Models.AttachApproval(
                                        admin.ID,
                                        Needs.Ccs.Services.Enums.ApprovalType.DeleteModelApproval,
                                        null,
                                        OrderID,
                                        OrderItemID,
                                        eventInfo);
            if (attachApproval.IsUnApprovedConflictEvent)
            {
                rtnMsg = "订单 " + OrderID + " 有未审批的订单申请：" + attachApproval.StrUnApprovedConflictEventTypes;
                return rtnMsg;
            }
            attachApproval.GenerateUnApprovalInfo();

            if ("auto" == approveOnOff) //自动审批
            {
                string approveManID = ConfigurationManager.AppSettings["ApproveManID"];
                var XDTAdmin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(approveManID);
                string referenceInfo = attachApproval.GetReferenceInfoHtmlForDeleteModelAndChangeModel(OrderID);
                attachApproval.ApproveSuccess(XDTAdmin, referenceInfo, isAuto: true);  //审批通过
                attachApproval.ExecuteTargetOperation();  //执行目标操作
            }

            return string.Empty;
        }

        /// <summary>
        /// 修改数量
        /// </summary>
        protected void ChangeQuantity()
        {
            try
            {
                string approveOnOff = ConfigurationManager.AppSettings["ApproveOnOff"];

                var orderItemID = Request.Form["OrderItemID"];
                var OrderID = Request.Form["OrderID"];
                var newQuantity = Request.Form["NewQuantity"];
                int newQuantityInt = int.Parse(newQuantity);

                string Model = Request.Form["Model"];
                string Manufacturer = Request.Form["Manufacturer"];
                string OldQuantity = Request.Form["OldQuantity"];
                int OldQuantityInt = int.Parse(OldQuantity);

                string rtnMsg = string.Empty;

                if (string.IsNullOrEmpty(approveOnOff) || "no" == approveOnOff)
                {
                    rtnMsg = DoChangeQuantityPage(orderItemID, OrderID, newQuantityInt);
                }
                else
                {
                    rtnMsg = PreApproveChangeQuantity(approveOnOff, OrderID, orderItemID, Model, Manufacturer, OldQuantityInt, newQuantityInt);
                }

                if (string.IsNullOrEmpty(rtnMsg))
                {
                    if (string.IsNullOrEmpty(approveOnOff) || "no" == approveOnOff)
                    {
                        Response.Write((new { success = true, message = "型号 " + Model + " 数量修改成功！", }).Json());
                    }
                    else
                    {
                        Response.Write((new { success = true, message = "请等待审批通过，可在“订单审批”中查看审批状态", }).Json());
                    }
                }
                else
                {
                    Response.Write((new { success = false, message = rtnMsg, }).Json());
                }
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "修改数量失败：" + ex.Message, }).Json());
            }
        }

        private string DoChangeQuantityPage(string orderItemID, string OrderID, int newQuantityInt)
        {
            ChangeQuantityHandler changeQuantityHandler = new ChangeQuantityHandler(orderItemID, OrderID, newQuantityInt);
            bool result = changeQuantityHandler.Execute();
            if (result)
            {
                try
                {
                    if (changeQuantityHandler.OrderStatus >= Needs.Ccs.Services.Enums.OrderStatus.Confirmed)
                    {
                        GenerateBill(OrderID);
                    }
                }
                catch (Exception e)
                {

                    throw new Exception("重新生成对账单内执行错误：" + e.Message);
                }

                //调用代仓储前端接口，修改型号数量
                changeQuantityHandler.CallPvWsOrderConfirm(
                    new Admin
                    {
                        ID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID,
                        ErmAdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ErmAdminID
                    });

                return string.Empty;
            }
            else
            {
                return changeQuantityHandler.Msg;
            }
        }

        /// <summary>
        /// 自动执行的修改数量（注意，该函数有地方通过反射调用）
        /// </summary>
        /// <param name="orderItemID"></param>
        /// <param name="OrderID"></param>
        /// <param name="newQuantityInt"></param>
        public void DoChangeQuantityAuto(string orderItemID, string OrderID, int newQuantityInt)
        {
            ChangeQuantityHandler changeQuantityHandler = new ChangeQuantityHandler(orderItemID, OrderID, newQuantityInt);
            bool result = changeQuantityHandler.Execute();
            if (result)
            {
                try
                {
                    if (changeQuantityHandler.OrderStatus >= Needs.Ccs.Services.Enums.OrderStatus.Confirmed)
                    {
                        GenerateBill(OrderID);
                    }
                }
                catch (Exception e)
                {

                    throw new Exception("重新生成对账单内执行错误：" + e.Message);
                }

                //调用代仓储前端接口，修改型号数量
                changeQuantityHandler.CallPvWsOrderConfirm(
                    new Admin
                    {
                        ID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID,
                        ErmAdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ErmAdminID
                    });
            }
            else
            {
                //修改数量执行错误，记录日志
            }
        }

        private string PreApproveChangeQuantity(string approveOnOff, string OrderID, string OrderItemID, string Model, string Manufacturer, int OldQuantityInt, int NewQuantityInt)
        {
            string rtnMsg = string.Empty;

            var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

            string eventInfo = JsonConvert.SerializeObject(new EventInfoChangeQuantity
            {
                ApplyAdminName = admin.RealName,
                TinyOrderID = OrderID,
                OrderItemID = OrderItemID,
                Model = Model,
                Manufacturer = Manufacturer,
                OldQuantity = OldQuantityInt,
                NewQuantity = NewQuantityInt,
            });

            var attachApproval = new Needs.Ccs.Services.Models.AttachApproval(
                                        admin.ID,
                                        Needs.Ccs.Services.Enums.ApprovalType.ChangeQuantityApproval,
                                        null,
                                        OrderID,
                                        OrderItemID,
                                        eventInfo);
            if (attachApproval.IsUnApprovedConflictEvent)
            {
                rtnMsg = "订单 " + OrderID + " 有未审批的订单申请：" + attachApproval.StrUnApprovedConflictEventTypes;
                return rtnMsg;
            }
            attachApproval.GenerateUnApprovalInfo();

            if ("auto" == approveOnOff) //自动审批
            {
                string approveManID = ConfigurationManager.AppSettings["ApproveManID"];
                var XDTAdmin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(approveManID);
                string referenceInfo = attachApproval.GetReferenceInfoHtmlForDeleteModelAndChangeModel(OrderID);
                attachApproval.ApproveSuccess(XDTAdmin, referenceInfo, isAuto: true);  //审批通过
                attachApproval.ExecuteTargetOperation();  //执行目标操作
            }

            return string.Empty;
        }

        /// <summary>
        /// 重新生成对账单
        /// </summary>
        /// <param name="orderID"></param>
        private void GenerateBill(string orderID)
        {
            //var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyOrders[orderID];
            var order = new Needs.Ccs.Services.Views.OrdersView()[orderID];
            var bill = order.Files.Where(file => file.FileType == FileType.OrderBill && file.Status == Status.Normal).SingleOrDefault();

            decimal PointedAgencyFee = 0;
            if (order.OrderBillType == OrderBillType.Pointed)
            {
                var agency = new Needs.Ccs.Services.Views.Origins.OrderPremiumsOrigin().Where(
                                                            t => t.OrderID == order.ID
                                                                && t.Type == OrderPremiumType.AgencyFee
                                                                && t.Status == Status.Normal).FirstOrDefault();
                if (agency != null)
                {
                    PointedAgencyFee = agency.UnitPrice;
                }
            }

            bill?.Abandon();
            order.GenerateBill(order.OrderBillType, PointedAgencyFee);
        }

    }
}