using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Control.Merchandiser
{
    /// <summary>
    /// 用于展示超出垫款上限的管控详情
    /// </summary>
    public partial class ExceedLimitDisplay : Uc.PageBase
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
            string orderId = Request.QueryString["OrderID"];
            //新增的状态，判断是否是风控审批
            string listStatus = Request.QueryString["Ststus"];

            var control = listStatus == "View" ? Needs.Wl.Admin.Plat.AdminPlat.Current.Control.MyMerchandiserControlsNotHangUp[id] : Needs.Wl.Admin.Plat.AdminPlat.Current.Control.RiskControlApprovalNotHangUp1[id];

            var order = control.Order;
            var client = order.Client;
            var clientAgreementsView = new ClientAgreementsView().Where(t => t.ClientID == client.ID && t.Status == Status.Normal).FirstOrDefault();
            var agreement = order.ClientAgreement;

            //垫款上限
            //var productFeeLimit = agreement.ProductFeeClause.UpperLimit.GetValueOrDefault();
            //var taxFeeLimit = agreement.TaxFeeClause.UpperLimit.GetValueOrDefault();
            //var agencyFeeLimit = agreement.AgencyFeeClause.UpperLimit.GetValueOrDefault();
            //var incidentalFeeLimit = agreement.IncidentalFeeClause.UpperLimit.GetValueOrDefault();
            //客户当前协议为准
            var productFeeLimit = clientAgreementsView.ProductFeeClause.UpperLimit.GetValueOrDefault();
            var taxFeeLimit = clientAgreementsView.TaxFeeClause.UpperLimit.GetValueOrDefault();
            var agencyFeeLimit = clientAgreementsView.AgencyFeeClause.UpperLimit.GetValueOrDefault();
            var incidentalFeeLimit = clientAgreementsView.IncidentalFeeClause.UpperLimit.GetValueOrDefault();

            //客户的未付款/欠款
            var unpaidFees = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.OrderReceipts.Where(item => item.ClientID == client.ID &&
                   (item.OrderStatus == OrderStatus.Completed || item.OrderStatus == OrderStatus.Declared || item.OrderStatus == OrderStatus.WarehouseExited)).ToList(); 
            var unpaidProductFee = unpaidFees.Where(item => item.FeeType == OrderFeeType.Product && item.IsLoan)
                                             .Sum(item => item.Amount * item.Rate);
            var unpaidTaxFee = unpaidFees.Where(item => item.FeeType == OrderFeeType.Tariff || item.FeeType == OrderFeeType.ExciseTax || item.FeeType == OrderFeeType.AddedValueTax)
                                         .Sum(item => item.Amount * item.Rate);
            var unpaidAgencyFee = unpaidFees.Where(item => item.FeeType == OrderFeeType.AgencyFee).Sum(item => item.Amount * item.Rate);
            var unpaidIncidentalFee = unpaidFees.Where(item => item.FeeType == OrderFeeType.Incidental).Sum(item => item.Amount * item.Rate);

            //垫款方式及
            var taxPeriodType = clientAgreementsView.TaxFeeClause.PeriodType;
            var taxDaysLimit = clientAgreementsView.TaxFeeClause.DaysLimit.GetValueOrDefault();
            var taxMonthlyDay = clientAgreementsView.TaxFeeClause.MonthlyDay.GetValueOrDefault();
            var taxFeeType = (int)FeeType.Tax;

            var agencyPeriodType = clientAgreementsView.AgencyFeeClause.PeriodType;
            var agencyDaysLimit = clientAgreementsView.AgencyFeeClause.DaysLimit.GetValueOrDefault();
            var agencyMonthlyDay = clientAgreementsView.AgencyFeeClause.MonthlyDay.GetValueOrDefault();
            var agencyFeeType = (int)FeeType.AgencyFee;

            var incidentalPeriodType = clientAgreementsView.IncidentalFeeClause.PeriodType;
            var incidentalDaysLimit = clientAgreementsView.IncidentalFeeClause.DaysLimit.GetValueOrDefault();
            var incidentalMonthlyDay = clientAgreementsView.IncidentalFeeClause.MonthlyDay.GetValueOrDefault();
            var incidentalFeeType = (int)FeeType.Incidental;
            var taxFeeDateTime = "";
            var agencyFeeDateTime = "";
            var incidentalFeeDateTime = "";
            var overdueOrderID = "";
            if (control.ControlType == OrderControlType.OverdueAdvancePayment)
            {
                UnHangUpCheck unHangUpCheck = new UnHangUpCheck(orderId, orderId);
                overdueOrderID = unHangUpCheck.OverOrderID();//超期订单号
                taxFeeDateTime = unHangUpCheck.TaxFeeDateTime(taxPeriodType, taxDaysLimit, taxMonthlyDay, taxFeeType, overdueOrderID);//税费
                agencyFeeDateTime = unHangUpCheck.TaxFeeDateTime(agencyPeriodType, agencyDaysLimit, agencyMonthlyDay, agencyFeeType, overdueOrderID);//代理费
                incidentalFeeDateTime = unHangUpCheck.TaxFeeDateTime(incidentalPeriodType, incidentalDaysLimit, incidentalMonthlyDay, incidentalFeeType, overdueOrderID);//杂费
            }
            this.Model.ControlData = new
            {
                control.ID,
                OrderID = order.ID,
                ClientName = client.Company.Name,
                ClientRank = client.ClientRank,
                DeclarePrice = order.DeclarePrice.ToRound(2).ToString("0.00"),
                Currency = order.Currency,
                Merchandiser = client.Merchandiser.RealName,
                ControlTypeValue = control.ControlType,
                ControlType = control.ControlType.GetDescription(),

                ProductFee = unpaidProductFee - productFeeLimit,
                TaxFee = unpaidTaxFee - taxFeeLimit,
                AgencyFee = unpaidAgencyFee - agencyFeeLimit,
                IncidentalFee = unpaidIncidentalFee - incidentalFeeLimit,

                ProductFeeLimit = productFeeLimit,
                TaxFeeLimit = taxFeeLimit,
                AgencyFeeLimit = agencyFeeLimit,
                IncidentalFeeLimit = incidentalFeeLimit,

                OverdueOrderID = overdueOrderID,
                taxFeeDateTime = taxFeeDateTime,
                agencyFeeDateTime = agencyFeeDateTime,
                incidentalFeeDateTime = incidentalFeeDateTime,
                taxPeriodType = taxPeriodType.GetDescription(),
                agencyPeriodType = agencyPeriodType.GetDescription(),
                incidentalPeriodType = incidentalPeriodType.GetDescription(),
                listStatus
            }.Json();

        }

        /// <summary>
        /// 初始化管控产品列表
        /// </summary>
        protected void data()
        {
            string id = Request.QueryString["ID"];
            //新增的状态，判断是否是风控审批
            string listStatus = Request.QueryString["Ststus"];

            // var control = Needs.Wl.Admin.Plat.AdminPlat.Current.Control.MyMerchandiserControls[id];
            var control = listStatus == "View" ? Needs.Wl.Admin.Plat.AdminPlat.Current.Control.MyMerchandiserControlsNotHangUp[id] : Needs.Wl.Admin.Plat.AdminPlat.Current.Control.RiskControlApprovalNotHangUp1[id];
            var productFeeExchangeRate = control.Order.ProductFeeExchangeRate;
            var taxpoint = 1 + control.Order.ClientAgreement.InvoiceTaxRate;
            var agencyFee = control.Order.AgencyFee * taxpoint;

            var aveAgencyFee = (agencyFee / control.Order.Items.Count()).ToRound(2);
            var orderItems = control.Order.Items;

            Func<Needs.Ccs.Services.Models.OrderItem, object> convert = item => new
            {
                item.ID,
                Name = item.Category.Name,
                Manufacturer = item.Manufacturer,
                Model = item.Model,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                TotalPrice = item.TotalPrice,
                DeclareValue = (item.TotalPrice * productFeeExchangeRate),
                TraiffRate = item.ImportTax.Rate,
                Traiff = item.ImportTax.Value,
                ExciseTaxRate = item.ExciseTax?.Rate ?? 0M,
                ExciseTax = item.ExciseTax?.Value ?? 0M,
                AddTaxRate = item.AddedValueTax.Rate,
                AddTax = item.AddedValueTax.Value,
                AgencyFee = aveAgencyFee,
                InspectionFee = (item.InspectionFee.GetValueOrDefault() * taxpoint),
                listStatus
            };

            Response.Write(new
            {
                rows = orderItems.Select(convert).ToList(),
                total = orderItems.Count()
            }.Json());
        }

        /// <summary>
        /// 审批通过，取消订单挂起，允许客户订单报关
        /// </summary>
        protected void CancelHangUp()
        {
            try
            {
                string id = Request.Form["ID"];
                //新增的状态，判断是否是风控审批
                string listStatus = Request.Form["Status"];

                string bufferDays = Request.Form["BufferDays"];
                string approveSummary = Request.Form["ApproveSummary"];
                //var control = Needs.Wl.Admin.Plat.AdminPlat.Current.Control.MyMerchandiserControls[id];
                var control = listStatus == "View" ? Needs.Wl.Admin.Plat.AdminPlat.Current.Control.MyMerchandiserControlsNotHangUp[id] : Needs.Wl.Admin.Plat.AdminPlat.Current.Control.RiskControlApprovalNotHangUp1[id];

                var adminid = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;

                //默认风控批
                //var RiskID = System.Configuration.ConfigurationManager.AppSettings["RiskManagementID"];
                //if (Needs.Wl.Admin.Plat.AdminPlat.Current.RealName == "张令金" || Needs.Wl.Admin.Plat.AdminPlat.Current.RealName == "张庆永")
                //{
                //    adminid = RiskID;//风控的ID
                //}

                //20230301 adminID记录实际操作人  log summary文字记录 风控

                var admin = Needs.Underly.FkoFactory<Admin>.Create(adminid);

                control.SetAdmin(admin);
                control.HangUpCanceled += Control_CancelHangUpSuccess;
                if (control.ControlType == OrderControlType.OverdueAdvancePayment)
                {
                    control.CancelOverduePaymentHangUp(bufferDays, approveSummary);
                }
                else
                {
                    control.CancelHangUp();
                }
            }
            catch (Exception ex)
            {
                ex.CcsLog("跟单员取消挂起报错");
                Response.Write((new { success = false, message = "审批失败：" + ex.Message }).Json());
            }
        }
        /// <summary>
        /// 订单取消挂起成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Control_CancelHangUpSuccess(object sender, OrderControledEventArgs e)
        {
            //NoticeLog notice = new NoticeLog();
            //notice.MainID = e.OrderControl.Order.ID;
            //notice.NoticeType = SendNoticeType.ExceedLimit;
            //notice.Readed = true;
            //notice.SendNotice();

            Response.Write((new { success = true, message = "订单取消挂起成功！" }).Json());
        }
    }
}
