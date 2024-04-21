using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Finance.Services;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Views.Rolls;
using Yahv.Underly;
using Yahv.Underly.Enums;
using Yahv.Underly.Enums.PvFinance;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using ApprovalStatus = Yahv.Underly.ApprovalStatus;

namespace Yahv.Finance.WebApp.Payer.ProductsFee
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
            var id = Request.QueryString["id"];
            if (!string.IsNullOrWhiteSpace(id))
            {
                var data = Erp.Current.Finance.PayerAppliesView[id];

                using (var payerRightsView = new PayerRightsRoll())
                using (var endorsView = new EndorsementsRoll())
                using (var moneyOrdersView = new MoneyOrdersRoll())
                {
                    string paymentMethord = null; //付款方式
                    string formCode = string.Empty; //流水号
                    string paymentDate = string.Empty; //付款日期
                    decimal? targetRate = 1;
                    string moneyOrderID = string.Empty;     //承兑汇票
                    string endorseDate = string.Empty;      //背书日期
                    bool isTransfer = false;        //是否允许转让
                    string moneyOrderCode = string.Empty;       //汇票号码

                    var payerRight = payerRightsView.FirstOrDefault(item => item.PayerLeftID == data.PayerLeft.ID);
                    if (payerRight != null && !string.IsNullOrWhiteSpace(payerRight.FlowID))
                    {
                        var flowAccount = Erp.Current.Finance.FlowAccounts.FirstOrDefault(item => item.ID == payerRight.FlowID);
                        if (flowAccount != null && !string.IsNullOrWhiteSpace(flowAccount.ID))
                        {
                            paymentMethord = flowAccount.PaymentMethord.GetDescription();
                            formCode = flowAccount.FormCode;
                            paymentDate = flowAccount.PaymentDate?.ToString("yyyy-MM-dd");
                            targetRate = flowAccount?.TargetRate ?? 1;
                            moneyOrderID = flowAccount.MoneyOrderID;

                            //承兑汇票信息 根据承兑汇票ID和持票人ID获取背书信息
                            if (!string.IsNullOrEmpty(flowAccount.MoneyOrderID))
                            {
                                var moneyOrder = moneyOrdersView[moneyOrderID];
                                if (moneyOrder != null)
                                {
                                    moneyOrderCode = moneyOrder.Code;
                                }


                                var endorsment = endorsView[flowAccount.MoneyOrderID, flowAccount.AccountID];
                                if (endorsment != null)
                                {
                                    endorseDate = endorsment.EndorseDate.ToString("yyyy-MM-dd");
                                    isTransfer = endorsment.IsTransfer;
                                }
                            }
                        }
                    }

                    this.Model.Data = new
                    {
                        PayeeAccountID = data.PayeeAccount?.Name,
                        PayeeCurrency = data.PayeeAccount?.Currency.GetDescription(),
                        PayeeCode = data.PayeeAccount?.Code,
                        PayeeBank = data.PayeeAccount?.BankName,
                        PayerAccountID = data.PayerAccount?.Name ?? data.PayerName,
                        PayerCurrency = data.PayerAccount?.Currency.GetDescription() ?? data.Currency.GetDescription(),
                        PayerCode = data.PayerAccount?.Code,
                        PayerBank = data.PayerAccount?.BankName,

                        IsPaid = data.IsPaid ? "是" : "否",
                        PayerLeft = data.PayerLeft,
                        ApplierID = data.Applier?.RealName,
                        ApproverID = data.Approver?.RealName,
                        Summary = data.Summary,
                        Price = data.Price,
                        FormCode = formCode,
                        PaymentDate = paymentDate,
                        PaymentMethord = paymentMethord,
                        data.Status,
                        TargetRate = targetRate,
                        MoneyOrderID = moneyOrderID,
                        EndorseDate = endorseDate,
                        IsTransfer = isTransfer,
                        MoneyOrderCode = moneyOrderCode,
                    };
                }
            }
            //付款类型
            this.Model.AccountCatalogs = AccountCatalogsAlls.Current.Json(AccountCatalogType.Output.GetDescription());
        }
        #endregion

        #region 功能函数
        /// <summary>
        /// 加载附件
        /// </summary>
        protected void filedata()
        {
            var files = Erp.Current.Finance.FilesDescriptionView
                .SearchByFilesMapValue(FilesMapName.PayerApplyID.ToString(), Request.QueryString["id"]);
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