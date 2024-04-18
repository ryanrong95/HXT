using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
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
    public partial class UnApproveChangeQuantity : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            string orderControlStepID = Request.QueryString["OrderControlStepID"];

            var view = new Needs.Ccs.Services.Views.ApproveGenerateBillInfoView();
            var approveChangeQuantityInfo = view.GetUnApproveResult(orderControlStepID);

            this.Model.ApproveChangeQuantityInfo = new
            {
                OrderControlID = approveChangeQuantityInfo.OrderControlID,
                OrderControlStepID = approveChangeQuantityInfo.OrderControlStepID,
                ControlTypeDes = approveChangeQuantityInfo.ControlType.GetDescription(),
                ApplicantName = approveChangeQuantityInfo.ApplicantName,
                TinyOrderID = approveChangeQuantityInfo.TinyOrderID,
                ClientName = approveChangeQuantityInfo.ClientName,
                Currency = approveChangeQuantityInfo.Currency,
                DeclarePrice = approveChangeQuantityInfo.DeclarePrice,
            }.Json();

            // 显示修改数量信息 Begin

            EventInfoChangeQuantity eventInfoChangeQuantity = JsonConvert.DeserializeObject<EventInfoChangeQuantity>(approveChangeQuantityInfo.EventInfo);
            this.Model.EventInfoChangeQuantity = new
            {
                Model = eventInfoChangeQuantity.Model,
                Manufacturer = eventInfoChangeQuantity.Manufacturer,
                OldQuantity = eventInfoChangeQuantity.OldQuantity,
                NewQuantity = eventInfoChangeQuantity.NewQuantity,
            }.Json();

            // 显示修改数量信息 End

            //查出审批日志 Begin

            var approveLogs = new Needs.Ccs.Services.Views.AttachApprovalLogView().GetResults(approveChangeQuantityInfo.OrderControlID);
            this.Model.ApproveLogs = approveLogs.Select(t => new
            {
                CreateDate = t.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Summary = t.Summary,
            }).Json();

            //查出审批日志 End
        }

        protected void ProductsData()
        {
            string TinyOrderID = Request.QueryString["TinyOrderID"];

            var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.Orders[TinyOrderID];

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
        /// 审批通过
        /// </summary>
        protected void ApproveOk()
        {
            try
            {
                string orderControlStepID = Request.Form["OrderControlStepID"];
                string referenceInfo = Request.Form["ReferenceInfo"];

                var attachApproval = new Needs.Ccs.Services.Models.AttachApproval(orderControlStepID);
                var admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                attachApproval.ApproveSuccess(admin, referenceInfo);
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
                string reason = Request.Form["ApproveCancelReason"];

                var attachApproval = new Needs.Ccs.Services.Models.AttachApproval(orderControlStepID);
                var admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                attachApproval.ApproveRefuse(admin, referenceInfo, reason);

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

                var attachApproval = new Needs.Ccs.Services.Models.AttachApproval(orderControlStepID);
                var admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                attachApproval.CancelApply(admin, referenceInfo);

                Response.Write((new { success = true, message = "操作成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "发生错误：" + ex.Message }).Json());
            }
        }

    }
}