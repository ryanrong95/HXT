using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Views.Rolls;
using Yahv.Underly;
using Yahv.Underly.Enums;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Finance.WebApp.Payer.PaymentApply
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
                using (var accountsCatalogView = new AccountCatalogsRoll())
                using (var payerLeftsView = new PayerLeftsRoll())
                {
                    string paymentMethord = null; //付款方式
                    string formCode = string.Empty; //流水号
                    string paymentDate = string.Empty; //付款日期
                    decimal? targetRate = null;

                    var payerRight = payerRightsView.FirstOrDefault(item => item.PayerLeftID == data.PayerLeft.ID);
                    if (payerRight != null && !string.IsNullOrWhiteSpace(payerRight.FlowID))
                    {
                        var flowAccount = Erp.Current.Finance.FlowAccounts.FirstOrDefault(item => item.ID == payerRight.FlowID);
                        if (flowAccount != null && !string.IsNullOrWhiteSpace(flowAccount.ID))
                        {
                            paymentMethord = flowAccount.PaymentMethord.GetDescription();
                            formCode = flowAccount.FormCode;
                            paymentDate = flowAccount.PaymentDate?.ToString("yyyy-MM-dd");
                            targetRate = flowAccount.TargetRate;
                        }
                    }

                    var catalogId = new AccountCatalogsRoll().GetID("付款类型", "综合业务", "费用", "银行手续费");
                    var left = payerLeftsView.FirstOrDefault(item => item.ApplyID == id && item.AccountCatalogID == catalogId);
                    
                    this.Model.Data = new
                    {
                        PayeeAccountID = data.PayeeAccount?.Name,
                        PayeeCurrency = data.PayeeAccount?.Currency.GetDescription(),
                        PayeeCode = data.PayeeAccount?.Code,
                        PayeeBank = data.PayeeAccount?.BankName,
                        PayerAccountID = data.PayerAccount?.Name,
                        PayerCurrency = data.PayerAccount?.Currency.GetDescription(),
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
                        ServiceCharge = left?.Price,
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
        #endregion
    }
}