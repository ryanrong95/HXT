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
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Finance.WebApp.Payer.AcceptanceApply
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
                using (var view = new AcceptanceAppliesRoll())
                using (var moneyOrdersView = new MoneyOrdersRoll())
                using (var endorsView = new EndorsementsRoll())
                using (var flowsView = new FlowAccountsRoll())
                {
                    var data = view.Find(id);

                    var leftOut = view.GetAcceptanceLeft(id, AccountMethord.Output);
                    var leftIn = view.GetAcceptanceLeft(id, AccountMethord.Input);
                    var moneyOrder = moneyOrdersView[data.MoneyOrderID];

                    string endorsDate = string.Empty;   //背书日期
                    string isTransfer = string.Empty;       //是否允许转让
                    string paymentMethord = string.Empty;
                    string paymentDate = string.Empty;
                    string outFormCode = string.Empty;
                    string inFormCode = string.Empty;
                    decimal? payeePrice = leftIn.Price;

                    var rights = view.GetAcceptanceRightsByApplyId(id);
                    if (rights != null && rights.Any())
                    {
                        var outFlow = flowsView.FirstOrDefault(item => rights.FirstOrDefault(r => r.AcceptanceLeftID == leftOut.ID).FlowID == item.ID);
                        var inFlow = flowsView.FirstOrDefault(item => rights.FirstOrDefault(r => r.AcceptanceLeftID == leftIn.ID).FlowID == item.ID);

                        if (outFlow != null)
                        {
                            paymentMethord = outFlow.PaymentMethord.GetDescription();
                            paymentDate = outFlow.PaymentDateDes;
                            outFormCode = outFlow.FormCode;
                        }

                        if (inFlow != null)
                        {
                            inFormCode = inFlow.FormCode;
                            payeePrice = inFlow?.Price;
                        }

                        if (data.Type == AcceptanceType.Endorsement)
                        {
                            var endors = endorsView[outFlow?.MoneyOrderID, outFlow?.AccountID];
                            if (endors != null)
                            {
                                endorsDate = endors.EndorseDate.ToString("yyyy-MM-dd");
                                isTransfer = endors.IsTransfer ? "是" : "否";
                            }
                        }
                    }

                    this.Model.Data = new
                    {
                        Type = data.Type,
                        PayeeAccountID = data.PayeeAccountID,
                        PayeeCode = data.PayeeCode,
                        PayeeBank = data.PayeeBank,
                        PayerAccountName = data.PayerAccountName,

                        data.PayeeAccountName,
                        PayerAccountID = data.PayerAccountID,
                        PayerCode = data.PayerCode,
                        PayerBank = data.PayerBank,

                        ApplierID = data.ApplierName,
                        ApproverID = data.ApproverID,
                        Summary = data.Summary,
                        PayerPrice = leftOut.Price,
                        PayeePrice = payeePrice,
                        data.ExcuterID,
                        data.TypeName,
                        data.MoneyOrderID,
                        data.ApproverName,
                        PayerCurrency = data.Currency.GetDescription(),
                        PayeeCurrency = data.Currency.GetDescription(),
                        MoneyOrderCode = moneyOrder?.Code,
                        EndorseDate = endorsDate,
                        IsTransfer = isTransfer,
                        ExchangeDate = moneyOrder?.ExchangeDate?.ToString("yyyy-MM-dd"),
                        PaymentMethord = paymentMethord,
                        PaymentDate = paymentDate,
                        OutFormCode = outFormCode,
                        InFormCode = inFormCode,
                    };
                }
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
                .SearchByFilesMapValue(FilesMapName.AcceptanceApplyID.ToString(), Request.QueryString["id"]);
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