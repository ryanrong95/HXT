using Needs.Ccs.Services;
using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Control.AttachApproval.Approver
{
    public partial class UnApproveSplitOrder : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            string orderControlStepID = Request.QueryString["OrderControlStepID"];

            var view = new Needs.Ccs.Services.Views.ApproveGenerateBillInfoView();
            var approveSplitOrderInfo = view.GetUnApproveResult(orderControlStepID);

            this.Model.ApproveSplitOrderInfo = new
            {
                OrderControlID = approveSplitOrderInfo.OrderControlID,
                OrderControlStepID = approveSplitOrderInfo.OrderControlStepID,
                ControlTypeDes = approveSplitOrderInfo.ControlType.GetDescription(),
                ApplicantName = approveSplitOrderInfo.ApplicantName,
                TinyOrderID = approveSplitOrderInfo.TinyOrderID,
                ClientName = approveSplitOrderInfo.ClientName,
                Currency = approveSplitOrderInfo.Currency,
                DeclarePrice = approveSplitOrderInfo.DeclarePrice,
            }.Json();

            // 显示拆分订单信息 Begin

            EventInfoSplitOrder eventInfoSplitOrder = JsonConvert.DeserializeObject<EventInfoSplitOrder>(approveSplitOrderInfo.EventInfo);
            this.Model.EventInfoSplitOrder = new
            {
                TinyOrderID = eventInfoSplitOrder.TinyOrderID,
                Packs = eventInfoSplitOrder.Packs,
            }.Json();

            // 显示拆分订单信息 End

            //查出审批日志 Begin

            var approveLogs = new Needs.Ccs.Services.Views.AttachApprovalLogView().GetResults(approveSplitOrderInfo.OrderControlID);
            this.Model.ApproveLogs = approveLogs.Select(t => new
            {
                CreateDate = t.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Summary = t.Summary,
            }).Json();

            //查出审批日志 End
        }

        protected void productsData()
        {
            string TinyOrderID = Request.QueryString["TinyOrderID"];
            //var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyOrders[TinyOrderID];
            var order = new Needs.Ccs.Services.Views.OrdersView()[TinyOrderID];

            Func<Needs.Ccs.Services.Models.OrderItem, object> convert = item => new
            {
                item.ID,
                Batch = item.Batch,
                Name = item.Category == null ? item.Name : item.Category.Name,
                Manufacturer = item.Manufacturer,
                Model = item.Model,
                Quantity = item.Quantity,
                DeclaredQuantity = item.DeclaredQuantity ?? 0,
                Origin = item.Origin,
                GrossWeight = item.GrossWeight,
                TotalPrice = item.TotalPrice.ToRound(2),
                ProductDeclareStatus = item.ProductDeclareStatus.GetDescription()
            };

            Response.Write(new
            {
                rows = order.Items.Select(convert).ToArray(),
                total = order.Items.Count()
            }.Json());
        }

        /// <summary>
        /// 装箱信息
        /// </summary>
        protected void dataPackings()
        {
            string OrderID = Request.QueryString["TinyOrderID"];
            var packingBill = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.SortingPackings.GetSortingPacking();
            var datas = packingBill.Where(Item => Item.OrderID == OrderID);
            Func<SortingPacking, object> convert = item => new
            {
                ID = item.ID,
                PackingID = item.Packing.ID,
                SortingID = item.ID,
                BoxIndex = item.BoxIndex,
                NetWeight = item.NetWeight,
                GrossWeight = item.GrossWeight,
                Model = item.OrderItem.Model,//产品型号
                ProductName = item.OrderItem.Name,  //产品名称
                CustomsName = item.OrderItem.Category.Name,  //报关品名
                Quantity = item.Quantity,
                Origin = item.OrderItem.Origin,
                Manufacturer = item.OrderItem.Manufacturer,
                DecStatus = item.DecStatus.GetDescription(),
                Status = item.Packing.PackingStatus.GetDescription(),
                PickDate = item.Packing.PackingDate.ToString("yyyy-MM-dd")
            };
            Response.Write(new
            {
                rows = datas.Select(convert).ToArray(),
                total = datas.Count()
            }.Json());
        }

        /// <summary>
        /// 审批通过
        /// </summary>
        protected void ApproveOk()
        {
            try
            {
                string orderControlStepID = Request.Form["OrderControlStepID"];
                string referenceInfo = Request.Form["ReferenceInfo"];
                string referenceInfo2 = Request.Form["ReferenceInfo2"];

                var attachApproval = new Needs.Ccs.Services.Models.AttachApproval(orderControlStepID);
                var admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                attachApproval.ApproveSuccess(admin, referenceInfo + "这是一个超级分隔符" + referenceInfo2);
                attachApproval.ExecuteTargetOperation();

                Response.Write((new { success = true, message = "操作成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "发生错误：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 审批拒绝
        /// </summary>
        protected void ApproveRefuse()
        {
            try
            {
                string orderControlStepID = Request.Form["OrderControlStepID"];
                string referenceInfo = Request.Form["ReferenceInfo"];
                string referenceInfo2 = Request.Form["ReferenceInfo2"];
                string reason = Request.Form["ApproveCancelReason"];

                var attachApproval = new Needs.Ccs.Services.Models.AttachApproval(orderControlStepID);
                var admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                attachApproval.ApproveRefuse(admin, referenceInfo + "这是一个超级分隔符" + referenceInfo2, reason);

                Response.Write((new { success = true, message = "操作成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "发生错误：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 撤销申请
        /// </summary>
        protected void ApproveCancel()
        {
            try
            {
                string orderControlStepID = Request.Form["OrderControlStepID"];
                string referenceInfo = Request.Form["ReferenceInfo"];
                string referenceInfo2 = Request.Form["ReferenceInfo2"];

                var attachApproval = new Needs.Ccs.Services.Models.AttachApproval(orderControlStepID);
                var admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                attachApproval.CancelApply(admin, referenceInfo + "这是一个超级分隔符" + referenceInfo2);

                Response.Write((new { success = true, message = "操作成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "发生错误：" + ex.Message }).Json());
            }
        }

    }
}