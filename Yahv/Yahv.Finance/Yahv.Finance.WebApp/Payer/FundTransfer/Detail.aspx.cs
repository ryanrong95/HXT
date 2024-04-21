using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Views.Rolls;
using Yahv.Underly;
using Yahv.Underly.Enums;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Finance.WebApp.Payer.FundTransfer
{
    public partial class Detail : ErpParticlePage
    {
        #region 加载数据
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitData();
            }
        }

        public void InitData()
        {
            try
            {
                var id = Request.QueryString["id"];
                if (!string.IsNullOrWhiteSpace(id))
                {
                    var data = Erp.Current.Finance.SelfAppliesView[id];
                    var flows = Erp.Current.Finance.FlowAccounts;

                    using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
                    using (var lefts = new SelfLeftsRoll(reponsitory))
                    using (var rights = new SelfRightsRoll(reponsitory))
                    using (var moneyOrdersView = new MoneyOrdersRoll(reponsitory))
                    using (var accountsView = new AccountsRoll(reponsitory))
                    using (var endorsView = new EndorsementsRoll(reponsitory))
                    {
                        var leftsID = lefts.Where(item => item.ApplyID == id).Select(item => item.ID).ToArray();
                        var flowsID = from r in rights.Where(item => leftsID.Contains(item.SelfLeftID)).ToArray()
                                      join l in lefts.Where(item => item.ApplyID == id).ToArray() on r.SelfLeftID equals l.ID
                                      select r.FlowID;
                        var outFlow = flows.FirstOrDefault(item => flowsID.Contains(item.ID) && item.AccountMethord == AccountMethord.Output);
                        var inFlow = flows.FirstOrDefault(item => flowsID.Contains(item.ID) && item.AccountMethord == AccountMethord.Input);

                        string moneyOrderCode = string.Empty;       //票据号码
                        string endorsDate = string.Empty;   //背书日期
                        string isTransfer = string.Empty;       //是否允许转让
                        string exchangeDate = string.Empty;     //承兑日期

                        if ((outFlow?.PaymentMethord == PaymentMethord.BankAcceptanceBill ||
                            outFlow?.PaymentMethord == PaymentMethord.CommercialAcceptanceBill) && !string.IsNullOrEmpty(outFlow.MoneyOrderID))
                        {
                            var moneyOrder = moneyOrdersView[outFlow?.MoneyOrderID];

                            if (moneyOrder != null)
                            {
                                moneyOrderCode = moneyOrder.Code;
                                exchangeDate = moneyOrder.ExchangeDate?.ToString("yyyy-MM-dd");
                            }

                            //调入账户是承兑户
                            if (accountsView.IsAcceptanceAccount(inFlow?.AccountID))
                            {
                                var endors = endorsView[outFlow?.MoneyOrderID, outFlow.AccountID];
                                if (endors != null)
                                {
                                    endorsDate = endors.EndorseDate.ToString("yyyy-MM-dd");
                                    isTransfer = endors.IsTransfer ? "是" : "否";
                                }
                            }
                        }

                        this.Model.Data = new
                        {
                            PayeeAccountID = data.PayeeAccount?.ShortName ?? data.PayeeAccount?.Name,
                            PayeeCurrency = data.PayeeAccount?.Currency.GetDescription(),
                            PayeeCode = data.PayeeAccount?.Code,
                            PayeeBank = data.PayeeAccount?.BankName,
                            PayerAccountID = data.PayerAccount?.ShortName ?? data.PayerAccount?.Name,
                            PayerCurrency = data.PayerAccount?.Currency.GetDescription(),
                            PayerCode = data.PayerAccount?.Code,
                            PayerBank = data.PayerAccount?.BankName,
                            ApplierID = data.Applier?.RealName,
                            ApproverID = data.Approver?.RealName,
                            Summary = data.Summary,
                            Price = data.Price,
                            data.Status,
                            AccountCatalogID = lefts.FirstOrDefault(item => item.ApplyID == id)?.AccountCatalogID,
                            PayerPrice = data.Price,
                            PayeePrice = data.TargetPrice,
                            Rate = data.TargetERate,
                            PaymentMethord = outFlow?.PaymentMethord.GetDescription(),
                            OutFormCode = outFlow?.FormCode,
                            InFormCode = inFlow?.FormCode,
                            PaymentDate = outFlow?.PaymentDate?.ToString("yyyy-MM-dd"),
                            MoneyOrderCode = moneyOrderCode,
                            EndorseDate = endorsDate,
                            IsTransfer = isTransfer,
                            ExchangeDate = exchangeDate,

                        };
                    }
                }
                //付款类型
                //this.Model.AccountCatalogs = AccountCatalogsAlls.Current.Json(AccountCatalogType.Output.GetDescription());
                this.Model.AccountCatalogs = ExtendsEnum.ToDictionary<FundTransferType>().Select(item => new { text = item.Value, value = item.Key });
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region 功能函数
        /// <summary>
        /// 加载附件
        /// </summary>
        protected void filedata()
        {
            var files = Erp.Current.Finance.FilesDescriptionView
                .SearchByFilesMapValue(FilesMapName.SelfApplyID.ToString(), Request.QueryString["id"]);
            string fileWebUrlPrefix = ConfigurationManager.AppSettings["FileWebUrlPrefix"];
            Func<FilesDescription, object> convert = item => new
            {
                ID = item.ID,
                CustomName = item.CustomName,
                FileFormat = "",
                Url = item.Url,    //数据库相对路径
                WebUrl = string.Concat(fileWebUrlPrefix, "/" + item.Url),
            };
            Response.Write(new
            {
                rows = files.Select(convert).ToArray(),
                total = files.Count(),
            }.Json());
        }

        /// <summary>
        /// 审批日志
        /// </summary>
        /// <returns></returns>
        protected object getLogs()
        {
            string applyId = Request.QueryString["id"];

            return Erp.Current.Finance.LogsApplyStepView.Where(item => item.ApplyID == applyId)
                .OrderByDescending(item => item.CreateDate).ToArray().ToArray()
                .Select(item => new
                {
                    item.ID,
                    CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    ApproverName = item.Approver?.RealName,
                    Status = item.Status.GetDescription(),
                    Summary = item.Summary,
                });
        }
        #endregion
    }
}