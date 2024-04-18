using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Linq;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Control.Delivery
{
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadComboBoxData();
        }

        /// <summary>
        /// 初始化下拉框数据
        /// </summary>
        protected void LoadComboBoxData()
        {
            this.Model.Clients = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView.Where(c => c.ClientType == ClientType.Internal).Select(c => new { c.ID, c.Company.Name }).Json();
            this.Model.ClientData = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyClients.Select(item => new { item.ID, item.Company.Name }).Json();
        }

        /// <summary>
        /// 初始化订单数据
        /// </summary>
        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string orderID = Request.QueryString["OrderID"];
            string clientCode = Request.QueryString["ClientCode"];
            string startDate = Request.QueryString["StartDate"];
            string endDate = Request.QueryString["EndDate"];
            var clientID = Request.QueryString["ClientID"];
            var type = Request.QueryString["ClientType"];

            var predicate = PredicateBuilder.Create<Needs.Ccs.Services.Models.OrderPendingDelieveryViewModel>();

            if (!string.IsNullOrEmpty(orderID))
            {
                orderID = orderID.Trim();
                predicate = predicate.And(item => item.ID.Contains(orderID));
            }

            if (!string.IsNullOrEmpty(clientCode))
            {
                clientCode = clientCode.Trim();
                predicate = predicate.And(item => item.ClientCode.Contains(clientCode));
            }

            if (!string.IsNullOrEmpty(startDate))
            {
                startDate = startDate.Trim();
                predicate = predicate.And(item => item.CreateDate > Convert.ToDateTime(startDate));
            }

            if (!string.IsNullOrEmpty(endDate))
            {
                endDate = endDate.Trim();
                predicate = predicate.And(item => item.CreateDate < Convert.ToDateTime(endDate));
            }

            if (!string.IsNullOrEmpty(type))
            {
                type = type.Trim();
                predicate = predicate.And(item => item.ClientType == (Needs.Ccs.Services.Enums.ClientType)Convert.ToInt16(type));
            }
            var testorder = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.DeliveryOrderAdvavceMoneyView;
            testorder.AllowPaging = true;
            testorder.PageIndex = page;
            testorder.PageSize = rows;
            testorder.Predicate = predicate;


            var exitNotices = testorder.ToList();
            int recordCount = exitNotices.Count();

            Response.Write(new
            {
                rows = exitNotices.Select(
                        order => new
                        {
                            order.ID,
                            order.MainOrderID,
                            order.ClientCode,
                            order.ClientName,
                            DeclarePrice = order.DeclarePrice.ToRound(2).ToString("0.00"),
                            order.Currency,
                            CreateDate = order.CreateDate.ToShortDateString(),
                            order.HasNotified,
                            order.Amount,
                            order.AdvanceRecordID
                        }
                     ).ToArray(),
                total = recordCount,
            }.Json());
        }

        protected void ToDeal()
        {
            try
            {
                string AdvanceRecordID = Request.Form["AdvanceRecordID"];
                //风控修改订单垫资状态
                Needs.Ccs.Services.Models.AdvanceMoneyApplyModel advanceMoneyApply = new Needs.Ccs.Services.Models.AdvanceMoneyApplyModel
                {
                    ID = AdvanceRecordID,
                    UntieAdvance = (int)Needs.Ccs.Services.Enums.UntieAdvanceStatus.UntieAdvance,
                    UpdateDate = DateTime.Now,
                };
                advanceMoneyApply.UntieAdvanceUpdate();

                Response.Write((new { success = true, message = "处理成功！" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "处理失败：" + ex.Message }).Json());
            }
        }
    }
}