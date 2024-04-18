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

namespace Yahv.PvOms.WebApp.Applications.Payments
{
    public partial class ListFinance : ErpParticlePage
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
            this.Model.PaymentStatus = ExtendsEnum.ToArray<ApplicationPaymentStatus>().Select(item => new
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

            var query = Erp.Current.WsOrder.Applications.Where(expression);
            var linq = query.Skip(rows * (page - 1)).Take(rows).ToArray();
            return new
            {
                rows = linq.OrderByDescending(t=>t.CreateDate).ThenBy(t => t.ApplicationStatus).ThenBy(t=>t.ReceiveStatus).ThenBy(t=>t.PaymentStatus)
                .Select(t => new
                {
                    ID = t.ID,
                    ClientName = t.Client.Name,
                    EnterCode = t.Client.EnterCode,
                    Type = t.Type.GetDescription(),
                    PayerName = t.Payers.FirstOrDefault()?.Contact,
                    CurrencyDec = t.Currency.GetDescription(),
                    TotalPrice = t.TotalPrice.ToString("0.00"),
                    ApplicationStatus = t.ApplicationStatus,
                    ReceiveStatus = t.ReceiveStatus,
                    PaymentStatus = t.PaymentStatus,
                    ApplicationStatusDec = t.ApplicationStatus.GetDescription(),
                    ReceiveStatusDec = t.ReceiveStatus.GetDescription(),
                    PaymentStatusDec = t.PaymentStatus.GetDescription(),
                    CreateDate = t.CreateDate.ToString("yyyy-MM-dd"),
                }).ToArray(),
                total = query.Count(),
            }.Json();
        }

        Expression<Func<Application, bool>> Predicate()
        {
            Expression<Func<Application, bool>> predicate = item => true;
            predicate = predicate.And(item => item.Type == ApplicationType.Payment);

            //查询参数
            var ApplicationID = Request.QueryString["ApplicationID"];
            var OrderID = Request.QueryString["OrderID"];
            var ClientName = Request.QueryString["ClientName"];
            var EnterCode = Request.QueryString["EnterCode"];
            var StartDate = Request.QueryString["StartDate"];
            var EndDate = Request.QueryString["EndDate"];
            //快速筛选参数
            var Status = Request.QueryString["ApplicationStatus"];
            var ReceiveStatus = Request.QueryString["ReceiveStatus"];
            var PaymentStatus = Request.QueryString["PaymentStatus"];

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
            if (!string.IsNullOrWhiteSpace(Status))
            {
                var status = (ApplicationStatus)Enum.Parse(typeof(ApplicationStatus), Status);
                predicate = predicate.And(item => item.ApplicationStatus == status);
            }
            if (!string.IsNullOrWhiteSpace(ReceiveStatus))
            {
                var status = (ApplicationReceiveStatus)Enum.Parse(typeof(ApplicationReceiveStatus), ReceiveStatus);
                predicate = predicate.And(item => item.ReceiveStatus == status);
            }
            if (!string.IsNullOrWhiteSpace(PaymentStatus))
            {
                var status = (ApplicationPaymentStatus)Enum.Parse(typeof(ApplicationPaymentStatus), PaymentStatus);
                predicate = predicate.And(item => item.PaymentStatus == status);
            }
            return predicate;
        }
    }
}