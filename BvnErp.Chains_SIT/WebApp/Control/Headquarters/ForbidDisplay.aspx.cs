using Needs.Ccs.Services;
using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Models;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Control.Headquarters
{
    /// <summary>
    /// 北京总部禁运产品审批界面
    /// </summary>
    public partial class ForbidDisplay : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        /// <summary>
        /// 初始化管控数据
        /// </summary>
        protected void LoadData()
        {
            string id = Request.QueryString["ID"];
            var control = Needs.Wl.Admin.Plat.AdminPlat.Current.Control.MyHQControls[id];

            this.Model.ControlData = new
            {
                control.ID,
                OrderID = control.Order.ID,
                ClientName = control.Order.Client.Company.Name,
                ClientRank = control.Order.Client.ClientRank,
                DeclarePrice = control.Order.DeclarePrice.ToRound(2).ToString("0.00"),
                Currency = control.Order.Currency,
                Merchandiser = control.Order.Client.Merchandiser.RealName
            }.Json();
        }

        /// <summary>
        /// 初始化管控产品列表
        /// </summary>
        protected void data()
        {
            string id = Request.QueryString["ID"];
            var control = Needs.Wl.Admin.Plat.AdminPlat.Current.Control.MyHQControls[id];

            Func<Needs.Ccs.Services.Models.OrderControlItem, object> convert = item => new
            {
                item.ID,
                item.OrderItem.Category.Name,
                item.OrderItem.Model,
                item.OrderItem.Manufacturer,
                item.OrderItem.Category.HSCode,
                item.OrderItem.Quantity,
                UnitPrice = item.OrderItem.UnitPrice.ToString("0.0000"),
                TotalPrice = item.OrderItem.TotalPrice.ToRound(2).ToString("0.00"),
                item.OrderItem.Origin,
                Declarant = item.OrderItem.Category.ClassifySecondOperator.RealName
            };

            Response.Write(new
            {
                rows = control.Items.Select(convert).ToList(),
                total = control.Items.Count()
            }.Json());
        }

        /// <summary>
        /// 同意禁运产品报关，审批通过
        /// </summary>
        protected void Approve()
        {
            try
            {
                string id = Request.Form["ID"];
                var control = Needs.Wl.Admin.Plat.AdminPlat.Current.Control.MyHQControls[id];
                var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

                control.SetAdmin(admin);
                control.Approved += Control_ApproveSuccess;
                control.Approve();
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "审批失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 拒绝禁运产品报关，审批未通过
        /// </summary>
        protected void Reject()
        {
            try
            {
                string id = Request.Form["ID"];
                var control = Needs.Wl.Admin.Plat.AdminPlat.Current.Control.MyHQControls[id];
                var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

                control.SetAdmin(admin);
                control.Rejected += Control_RejectSuccess;
                control.Reject();
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "审批失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 拒绝禁运产品报关，可转第三方报关
        /// </summary>
        protected void Turn()
        {
            try
            {
                string id = Request.Form["ID"];
                var control = Needs.Wl.Admin.Plat.AdminPlat.Current.Control.MyHQControls[id];
                var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

                control.SetAdmin(admin);
                control.Rejected += Control_RejectSuccess;
                control.Turn();
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "审批失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 审批通过触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Control_ApproveSuccess(object sender, OrderControledEventArgs e)
        {
            NoticeLog noticeLog = new NoticeLog();
            noticeLog.MainID = e.OrderControl.Order.ID;           
            noticeLog.NoticeType = Needs.Ccs.Services.Enums.SendNoticeType.Forbid;
            noticeLog.Readed = true;
            noticeLog.SendNotice();
            

            Response.Write((new { success = true, message = "审批成功！" }).Json());
        }

        /// <summary>
        /// 审批未通过触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Control_RejectSuccess(object sender, OrderControledEventArgs e)
        {
            NoticeLog noticeLog = new NoticeLog();
            noticeLog.MainID = e.OrderControl.Order.ID;          
            noticeLog.NoticeType = Needs.Ccs.Services.Enums.SendNoticeType.Forbid;
            noticeLog.Readed = true;
            noticeLog.SendNotice();

            noticeLog.NoticeType = Needs.Ccs.Services.Enums.SendNoticeType.ForbidRejected;
            noticeLog.Readed = false;
            noticeLog.SendNotice();

            Response.Write((new { success = true, message = "审批成功！" }).Json());
        }
    }
}