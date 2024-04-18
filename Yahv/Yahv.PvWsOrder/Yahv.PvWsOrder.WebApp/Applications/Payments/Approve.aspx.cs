using Layers.Data.Sqls;
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
using Yahv.PvWsOrder.Services.DyjModels;
using Yahv.PvWsOrder.Services.Extends;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PvOms.WebApp.Applications.Payments
{
    public partial class Approve : ErpParticlePage
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
                    RateToUSD = application.RateToUSD,

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
                };

                var payee = new PayeesTopView().SingleOrDefault(item => item.Account == application.InBankAccount && item.Status == GeneralStatus.Normal && item.Methord != Methord.Cash);
                var credits = new Yahv.Services.Views.CreditsUsdStatisticsView<PvbCrmReponsitory>()
                    .Where(item => item.Payer == application.ClientID && item.Payee == payee.EnterpriseID)
                    .Where(item => item.Currency == Currency.USD && item.Catalog == CatalogConsts.仓储服务费).FirstOrDefault();
                var Balance = credits == null ? 0 : credits.Total - credits.Cost;
                //申请的应收实收账款
                var query = new VouchersStatisticsView().Where(item => item.ApplicationID == application.ID).ToArray();
                var TotalLeft = query.Sum(t => t.LeftPrice);
                var TotalRight = query.Sum(t => t.RightPrice);

                this.Model.BalanceData = new
                {
                    AvailableBalance = Balance,
                };
                this.Model.ReceiveData = new
                {
                    TotalLeft,
                    TotalRight,
                    TotalLeftUSD = TotalLeft * application.RateToUSD,
                    TotalRightUSD = TotalRight * application.RateToUSD,
                    AvailableUSD = Balance + TotalRight * application.RateToUSD,
                };

                //特殊权限
                this.Model.AdminID = Erp.Current.ID;
            }
        }

        /// <summary>
        /// 申请明细项
        /// </summary>
        protected object data()
        {
            string ID = Request.QueryString["ID"];
            var application = Erp.Current.WsOrder.Applications.Where(item => item.ID == ID).FirstOrDefault();

            var query = new PvWsOrder.Services.Views.Alls.ApplicationItemsAll().GetPaymentIQueryable().Where(item => item.ApplicationID == ID).ToArray();
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
        /// 合同发票文件
        /// </summary>
        protected object LoadInvoice()
        {
            string ID = Request.QueryString["ID"];
            var query = new PvWsOrder.Services.Views.ApplicationFilesRoll(ID).Where(item => item.Type == (int)FileType.Invoice).ToArray();
            var linq = query.Select(t => new
            {
                ID = t.ID,
                CustomName = t.CustomName,
                Url = PvWsOrder.Services.Common.FileDirectory.ServiceRoot + t.Url,
            });
            return new
            {
                rows = linq.ToArray(),
                total = query.Count()
            };
        }

        /// <summary>
        /// 代付委托书
        /// </summary>
        protected object LoadProxy()
        {
            string ID = Request.QueryString["ID"];
            var query = new PvWsOrder.Services.Views.ApplicationFilesRoll(ID)
                .Where(item => item.Type == (int)FileType.PaymentEntrust)
                .Where(item => item.Status != Services.Models.FileDescriptionStatus.Delete)
                .ToArray();
            var linq = query.Select(t => new
            {
                ID = t.ID,
                CustomName = t.CustomName,
                Url = PvWsOrder.Services.Common.FileDirectory.ServiceRoot + t.Url,
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
            var linq = query.OrderByDescending(t => t.CreateDate).Select(t => new
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

        /// <summary>
        /// 审批通过
        /// </summary>
        protected void Pass()
        {
            try
            {
                string ID = Request.Form["ID"];
                string Content = Request.Form["Content"];
                var application = Erp.Current.WsOrder.Applications.SingleOrDefault(item => item.ID == ID);
                var payer = new PayersTopView().Where(item => item.Account == application.OutBankAccount && item.Status == GeneralStatus.Normal && item.Methord != Methord.Cash)
                    .FirstOrDefault(t => t.RealEnterpriseID == t.EnterpriseID || t.RealEnterpriseID == null || t.RealEnterpriseID == string.Empty); //我方付款人
                var payee = application.Payees.FirstOrDefault();//供应商收款人

                if (application.ApplicationStatus != PvWsOrder.Services.Enums.ApplicationStatus.Examined)
                {
                    throw new Exception("申请已不是待审批状态了!");
                }
                if (payer == null || payee == null)
                {
                    throw new Exception("未找到付款人或收款人！");
                }

                //添加应付记录
                PaymentManager.Erp(Erp.Current.ID)[payer.EnterpriseID, payee.EnterpriseID][ConductConsts.供应链]
                    .Payable[CatalogConsts.仓储服务费, SubjectConsts.代付货款]
                    .Record(application.Currency, application.TotalPrice, applicationID: application.ID);

                //添加中心付款申请
                var data = new
                {
                    sender = "FSender002",
                    option = "insert",
                    model = new
                    {
                        CreatorID = Erp.Current.ID,
                        Price = application.TotalPrice,
                        PayerCode = application.OutBankAccount,
                        PayeeCurrency = application.Currency.ToString(),
                        PayeeCode = payee?.BankAccount,
                        PayeeName = payee?.EnterpriseName,
                        PayeeBank = payee?.BankName,

                        PayerBank = payer?.Bank,
                        PayerName = payer?.EnterpriseName,
                        PayerCurrency = payer?.Currency.ToString(),
                    },
                };
                var apisetting = new PvWsOrder.Services.PvFinanceApiSetting();
                var url = ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.PayForGoodsEnter;

                ////调用DYJ新增费用单据接口
                //var rate = ExchangeRates.Universal[Currency.USD, Currency.CNY];//获取汇率
                //var usdAmount = 0m;
                //var cnyAmount = 0m;
                //if (application.Currency == Currency.USD)
                //{
                //    usdAmount = application.TotalPrice;
                //    cnyAmount = application.TotalPrice * rate;
                //}
                //if (application.Currency == Currency.CNY)
                //{
                //    usdAmount = application.TotalPrice / rate;
                //    cnyAmount = application.TotalPrice;
                //}

                ////获取pid
                //var receiveableIds = new VouchersStatisticsView().Where(t => t.ApplicationID == application.ID).Select(t => t.ReceivableID).Distinct().ToArray();
                //var flowIds = new ReceivedsStatisticsView().Where(t => receiveableIds.Contains(t.ReceivableID)).Select(t => t.FlowID).Distinct().ToArray();

                //var flowAccoutView = new FlowAccountsTopView();
                //var formcodes = flowAccoutView.Where(t => flowIds.Contains(t.ID)).Select(t => t.FormCode).Distinct().ToArray();
                //var formAccouts = flowAccoutView.Where(t => formcodes.Contains(t.FormCode) && t.Price > 0).ToArray();
                //System.Text.StringBuilder str = new System.Text.StringBuilder();
                //str.Append("此单为代付汇，客户是" + application.Client?.Name + "<br />");
                //foreach (var item in formAccouts)
                //{
                //    str.Append("客户付" + payee?.EnterpriseName + "  " + item.Price.ToString() + "  " + item.FormCode + "<br />");
                //}
                //str.Append("麻烦付" + usdAmount + "美金到以上账户，谢谢");

                //FeeApplyModel model = new FeeApplyModel();
                //model.key = "8c2b75ad115b467a8e976123033319f2";
                //model.data = new FeeApplyDataModel();
                //model.data.CurrencyID = application.Currency == Currency.CNY ? 1 : 2;
                //model.data.CheckID = "558";
                //model.data.CheckName = "孙善华";
                //model.data.Amount = cnyAmount;
                //model.data.FAmount = usdAmount;
                //model.data.PayCompany = payer?.EnterpriseName;
                //model.data.Note = str.ToString();

                //model.data.ClientID = "0";
                //model.data.ClientName = application.Client?.Name;
                //model.data.ClientLinkName = application.Client?.Contact?.Name;
                //model.data.ClientBank = payee?.BankName;
                //model.data.ClientBankNum = payee?.BankAccount;
                //model.data.ClientBankAddress = "0";

                //model.data.Provider = "0";
                //model.data.ProviderName = payer?.EnterpriseName;
                //model.data.ProviderLinkName = payer?.Contact;
                //model.data.ProviderBank = payer?.Bank;
                //model.data.ProviderBankNum = payer?.Account;
                //model.data.ProviderBankAddress = payer?.BankAddress;

                //执行审批通过
                var log = new Application_Logs()
                {
                    ApplicationID = ID,
                    AdminID = Erp.Current.ID,
                    StepName = "经理审批",
                    Status = PvWsOrder.Services.Enums.ApprovalStatus.Agree,
                    Summary = Content /*model.Json()*/,
                };
                application.Approve(true, log);
                var result = Yahv.Utils.Http.ApiHelper.Current.JPost(url, data);

                //var url2 = ConfigurationManager.AppSettings["XDTSFK"] + "api/XDTSFK/SetFeeApplyS";
                //var reponse = Yahv.Utils.Http.ApiHelper.Current.JPost(url2, model);

                Response.Write((new { success = true, message = "成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "失败：" + ex.Message }).Json());
            }
        }
    }
}