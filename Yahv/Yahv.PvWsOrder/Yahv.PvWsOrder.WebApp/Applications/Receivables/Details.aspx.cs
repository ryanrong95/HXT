using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Payments;
using Yahv.Payments.Views;
using Yahv.PvWsOrder.Services.Common;
using Yahv.PvWsOrder.Services.Extends;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PvOms.WebApp.Applications.Receivables
{
    public partial class Details : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        protected void LoadData()
        {
            string ID = Request.QueryString["ID"];
            var application = Erp.Current.WsOrder.Applications.Where(item => item.ID == ID).FirstOrDefault();

            if (application != null)
            {
                var payerID = application.Payers.FirstOrDefault()?.PayerID;
                var payer = new PayersTopView().SingleOrDefault(t => t.ID == payerID);
                this.Model.ApplicationData = new
                {
                    CreateDate = application.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    ClientName = application.Client.Name,
                    EnterCode = application.Client.EnterCode,
                    Currency = application.Currency.GetDescription(),

                    PayerName = payer?.Contact,
                    PayerBankName = application.Payers.FirstOrDefault()?.BankName,
                    PayerBankAccount = application.Payers.FirstOrDefault()?.BankAccount,
                    PayerMethod = application.Payers.FirstOrDefault()?.Method.GetDescription(),
                    PayerCurrency = application.Payers.FirstOrDefault()?.Currency.GetDescription(),

                    PayeeName = application.Payees.FirstOrDefault()?.EnterpriseName,
                    PayeeBankName = application.Payees.FirstOrDefault()?.BankName,
                    PayeeBankAccount = application.Payees.FirstOrDefault()?.BankAccount,
                    PayeeMethod = application.Payees.FirstOrDefault()?.Method.GetDescription(),
                    PayeeCurrency = application.Payees.FirstOrDefault()?.Currency.GetDescription(),

                    InCompanyName = application.InCompanyName,
                    InBankName = application.InBankName,
                    InBankAccount = application.InBankAccount,

                    OutCompanyName = application.OutCompanyName,
                    OutBankName = application.OutBankName,
                    OutBankAccount = application.OutBankAccount,

                    IsEntry = application.IsEntry == true ? "是" : "否",
                    CheckDelivery = application.CheckDelivery == null ? "" : application.CheckDelivery.GetDescription(),
                    CheckCarrier = application.CheckCarrier,
                    CheckConsignee = application.CheckConsignee,
                    CheckPayeeAccount = application.CheckPayeeAccount,
                    CheckTitle = application.CheckTitle
                };

                var payee = new PayeesTopView().SingleOrDefault(item => item.Account == application.InBankAccount && item.Status == GeneralStatus.Normal && item.Methord != Methord.Cash);
                var credits = new Yahv.Services.Views.CreditsUsdStatisticsView<Layers.Data.Sqls.PvbCrmReponsitory>()
                    .Where(item => item.Payer == application.ClientID && item.Payee == payee.EnterpriseID)
                    .Where(item => item.Currency == Currency.USD && item.Catalog == CatalogConsts.仓储服务费).FirstOrDefault();
                var Balance = credits == null ? 0 : credits.Total - credits.Cost;
                this.Model.BalanceData = new
                {
                    AvailableBalance = Balance,
                };

                //申请的应收实收账款
                var query = new Yahv.Payments.Views.VouchersStatisticsView().Where(item => item.ApplicationID == application.ID).ToArray();
                var TotalLeft = query.Sum(t => t.LeftPrice);
                var TotalRight = query.Sum(t => t.RightPrice);
                this.Model.ReceiveData = new
                {
                    TotalLeft,
                    TotalRight,
                };
            }
        }

        /// <summary>
        /// 申请明细项
        /// </summary>
        protected object data()
        {
            string ID = Request.QueryString["ID"];
            var application = Erp.Current.WsOrder.Applications.Where(item => item.ID == ID).FirstOrDefault();

            var query = new PvWsOrder.Services.Views.Alls.ApplicationItemsAll().GetReceiveIQueryable().Where(item => item.ApplicationID == ID).ToArray();
            var linq = query.Select(t => new
            {
                OrderID = t.OrderID,
                Currency = application.Currency.GetDescription(),
                TotalPrice = t.TotalPrice,
                AppliedPrice = t.AppliedPrice - t.Amount,
                CurrentPrice = t.Amount
            });
            return new
            {
                rows = linq.ToArray(),
                total = query.Count()
            };
        }

        /// <summary>
        /// 收付款凭证
        /// </summary>
        protected object LoadConfirm()
        {
            string ID = Request.QueryString["ID"];
            var query = new Yahv.Services.Views.ApplicationFilesTopView<Layers.Data.Sqls.PvbCrmReponsitory>().Where(item => item.ApplicationID == ID);
            var linq = query.ToArray().Select(t => new
            {
                ID = t.ID,
                CustomName = t.CustomName,
                FileType = ((FileType)t.Type).GetDescription(),
                Url = PvWsOrder.Services.Common.FileDirectory.ServiceRoot + t.Url,
            });
            return new
            {
                rows = linq.ToArray(),
                total = query.Count()
            };
        }

        /// <summary>
        /// 审批日志
        /// </summary>
        /// <returns></returns>
        protected object LoadLogs()
        {
            string ID = Request.QueryString["ID"];
            var query = new PvWsOrder.Services.Views.Origins.Application_LogsOrigin().Where(item => item.ApplicationID == ID).ToArray();
            var linq = query.OrderByDescending(t=>t.CreateDate).Select(t => new
            {
                CreateDate = t.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                StepName = t.StepName,
                Status = t.Status.GetDescription(),
                Summary = t.Summary,
            });
            return new
            {
                rows = linq.ToArray(),
                total = query.Count()
            };
        }
    }
}