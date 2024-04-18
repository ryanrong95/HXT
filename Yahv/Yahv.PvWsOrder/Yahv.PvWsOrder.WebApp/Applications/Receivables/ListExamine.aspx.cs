using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.Payments;
using Yahv.PvWsOrder.Services.Common;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.PvWsOrder.Services.Models;
using Yahv.PvWsOrder.Services.Views;
using Yahv.PvWsOrder.Services.Views.Alls;
using Yahv.PvWsOrder.Services.Views.Origins;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PvOms.WebApp.Applications.Receivables
{
    public partial class ListExamine : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadComboBoxData();
            }
        }

        protected void LoadComboBoxData()
        {
            this.Model.ApplicationStatus = ExtendsEnum.ToArray<ApplicationStatus>().Select(item => new
            {
                value = (int)item,
                text = item.GetDescription()
            });
            this.Model.ReceiveStatus = ExtendsEnum.ToArray<ApplicationReceiveStatus>().Select(item => new
            {
                value = (int)item,
                text = item.GetDescription()
            });
        }

        protected object data()
        {
            Expression<Func<Application, bool>> expression = Predicate();
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            var query = Erp.Current.WsOrder.MyApplication(Erp.Current.Leagues.Current.EnterpriseID).GetPageList(page, rows, expression);
            return new
            {
                rows = query.OrderByDescending(t => t.CreateDate).ThenBy(t => t.ApplicationStatus).ThenBy(t => t.ReceiveStatus)
                .Select(t => new
                {
                    ID = t.ID,
                    ClientName = t.Client.Name,
                    EnterCode = t.Client.EnterCode,
                    Type = t.Type.GetDescription(),
                    CurrencyDec = t.Currency.GetDescription(),
                    TotalPrice = t.TotalPrice.ToString("0.00"),
                    ApplicationStatus = t.ApplicationStatus,
                    ReceiveStatus = t.ReceiveStatus,
                    PaymentStatus = t.PaymentStatus,
                    ApplicationStatusDec = t.ApplicationStatus.GetDescription(),
                    ReceiveStatusDec = t.ReceiveStatus.GetDescription(),
                    PaymentStatusDec = t.PaymentStatus.GetDescription(),
                    CreateDate = t.CreateDate.ToString("yyyy-MM-dd"),
                    Method = t.Payers.FirstOrDefault()?.Method,
                    MethodDec = t.Payers.FirstOrDefault() == null ? "" : t.Payers.FirstOrDefault().Method.GetDescription(),
                    IsEntry = t.IsEntry,
                    IsEntryDec = t.IsEntry == true ? "是" : "否",
                    DelivaryOpportunity = t.DelivaryOpportunity.GetDescription(),
                }).ToArray(),
                total = query.Total,
            }.Json();
        }

        Expression<Func<Application, bool>> Predicate()
        {
            Expression<Func<Application, bool>> predicate = item => true;
            predicate = predicate.And(item => item.Type == ApplicationType.Receival);

            //查询参数
            var ApplicationID = Request.QueryString["ApplicationID"];
            var OrderID = Request.QueryString["OrderID"];
            var ClientName = Request.QueryString["ClientName"];
            var EnterCode = Request.QueryString["EnterCode"];
            var StartDate = Request.QueryString["StartDate"];
            var EndDate = Request.QueryString["EndDate"];

            //快速筛选参数
            var ApplicationStatus = Request.QueryString["ApplicationStatus"];
            var ReceiveStatus = Request.QueryString["ReceiveStatus"];

            if (!string.IsNullOrWhiteSpace(ApplicationID))
            {
                ApplicationID = ApplicationID.Trim();
                predicate = predicate.And(item => item.ID.Contains(ApplicationID));
            }
            if (!string.IsNullOrWhiteSpace(ClientName))
            {
                ClientName = ClientName.Trim();
                predicate = predicate.And(item => item.Client.Name.Contains(ClientName));
            }
            if (!string.IsNullOrWhiteSpace(EnterCode))
            {
                EnterCode = EnterCode.Trim();
                predicate = predicate.And(item => item.Client.EnterCode.Contains(EnterCode));
            }
            if (!string.IsNullOrWhiteSpace(StartDate))
            {
                DateTime start = Convert.ToDateTime(StartDate.Trim());
                predicate = predicate.And(item => item.CreateDate >= start);
            }
            if (!string.IsNullOrWhiteSpace(EndDate))
            {
                DateTime end = Convert.ToDateTime(StartDate.Trim()).AddDays(1);
                predicate = predicate.And(item => item.CreateDate < end);
            }
            if (!string.IsNullOrWhiteSpace(OrderID))
            {
                OrderID = OrderID.Trim();
                predicate = predicate.And(item => item.Items.Select(t => t.OrderID).Contains(OrderID));
            }
            if (!string.IsNullOrWhiteSpace(ApplicationStatus))
            {
                var status = (ApplicationStatus)Enum.Parse(typeof(ApplicationStatus), ApplicationStatus);
                predicate = predicate.And(item => item.ApplicationStatus == status);
            }
            if (!string.IsNullOrWhiteSpace(ReceiveStatus))
            {
                var status = (ApplicationReceiveStatus)Enum.Parse(typeof(ApplicationReceiveStatus), ReceiveStatus);
                predicate = predicate.And(item => item.ReceiveStatus == status);
            }
            return predicate;
        }

        protected void CheckReceived()
        {
            try
            {
                string ID = Request.Form["ID"].Trim();
                var application = Erp.Current.WsOrder.Applications.SingleOrDefault(item => item.ID == ID);

                //收款核销完成
                application.ReveiveWorkOff();

                Response.Write((new { success = true, message = "成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "失败：" + ex.Message }).Json());
            }
        }

        protected void CheckDelivered()
        {
            try
            {
                string ID = Request.Form["ID"].Trim();
                var application = Erp.Current.WsOrder.Applications.SingleOrDefault(item => item.ID == ID);

                //付款核销完成
                application.PaymentWorkOff();

                Response.Write((new { success = true, message = "成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "失败：" + ex.Message }).Json());
            }
        }
    }
}