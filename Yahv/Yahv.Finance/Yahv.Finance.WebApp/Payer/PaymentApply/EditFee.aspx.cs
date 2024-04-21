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
using Yahv.Utils.Extends;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Finance.WebApp.Payer.PaymentApply
{
    public partial class EditFee : ErpParticlePage
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
                    int paymentMethordId = -1; //付款方式Id
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
                            paymentMethordId = (int)flowAccount.PaymentMethord;
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
                        FormCode_Hidden = formCode,
                        PaymentDate_Hidden = paymentDate,
                        PaymentMethord_Hidden = paymentMethordId,
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

        #region 提交保存
        protected object Submit()
        {
            var json = new JMessage() { success = true, data = "提交成功!" };
            PayerApply apply = null;
            var oldValue = string.Empty;
            var newValue = string.Empty;
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            using (var tran = reponsitory.OpenTransaction())
            using (var payerLeftsView = new PayerLeftsRoll(reponsitory))
            using (var flowsView = new FlowAccountsRoll(reponsitory))
            using (var payerRightsView = new PayerRightsRoll(reponsitory))
            {
                try
                {
                    var id = Request.QueryString["id"];
                    if (id.IsNullOrEmpty())
                    {
                        throw new Exception("未找到申请信息!");
                    }

                    apply = Erp.Current.Finance.PayerAppliesView[id];
                    var serviceCharge = Request.Form["ServiceCharge"];
                    var catalogId = new AccountCatalogsRoll().GetID("付款类型", "综合业务", "费用", "银行手续费");
                    var left = payerLeftsView.FirstOrDefault(item => item.AccountCatalogID == catalogId && item.ApplyID == id);
                    var formCode = Request.Form["FormCode_Hidden"];     //流水号
                    var paymentDate = Request.Form["PaymentDate_Hidden"];       //付款日期
                    var paymentMethord = (PaymentMethord)int.Parse(Request.Form["PaymentMethord_Hidden"]);
                    var oldFlowId = string.Empty;

                    oldValue = string.Empty;
                    newValue = Request.Form["ServiceCharge"];

                    //没有修改
                    if (!serviceCharge.IsNullOrEmpty() && left != null && decimal.Parse(serviceCharge) == left.Price)
                    {
                        return json;
                    }

                    //如果之前录入过手续费 进行删除
                    if (left != null && !left.ID.IsNullOrEmpty())
                    {
                        oldValue = left.Price.ToString();

                        var right = payerRightsView.FirstOrDefault(item => item.PayerLeftID == left.ID);
                        if (right != null)
                        {
                            oldFlowId = right.FlowID;

                            //删除右表
                            payerRightsView.Abandon(right.ID);

                            //删除流水
                            flowsView.Abandon(right.FlowID);
                        }

                        //删除左表
                        payerLeftsView.Abandon(left.ID);
                    }

                    var charge = decimal.Parse(serviceCharge);
                    var rate = ExchangeRates.Universal[apply.Currency, Currency.CNY];
                    //应付
                    var payerLeft = new PayerLeft()
                    {
                        PayerAccountID = apply.PayerAccountID,
                        PayerID = apply.PayerID,
                        Currency = apply.Currency,
                        ApplyID = apply.ID,
                        CreatorID = Erp.Current.ID,
                        Currency1 = Currency.CNY,
                        ERate1 = rate,
                        Price1 = (charge * rate).Round(),
                        Price = charge,
                        Status = GeneralStatus.Normal,
                        AccountCatalogID = catalogId,
                    };
                    payerLeft.Enter();

                    //流水
                    var flowId_temp = PKeySigner.Pick(Yahv.Finance.Services.PKeyType.FlowAcc);
                    var flow = new FlowAccount()
                    {
                        ID = flowId_temp,
                        Currency = apply.Currency,
                        CreateDate = DateTime.Now,
                        AccountID = apply.PayerAccountID,
                        AccountMethord = AccountMethord.Payment,
                        CreatorID = Erp.Current.ID,
                        Currency1 = Currency.CNY,
                        ERate1 = rate,
                        FormCode = formCode,
                        PaymentDate = DateTime.Parse(paymentDate),
                        Price = -charge,
                        Price1 = -(charge * rate).Round(),
                        PaymentMethord = paymentMethord,
                    };
                    flow.Enter();

                    //添加核销
                    var payerRight = new PayerRight()
                    {
                        Currency = apply.Currency,
                        CreateDate = DateTime.Now,
                        CreatorID = Erp.Current.ID,
                        Currency1 = Currency.CNY,
                        FlowID = flowId_temp,
                        ERate1 = rate,
                        PayerLeftID = payerLeft.ID,
                        Price = charge,
                        Price1 = (charge * rate).Round()
                    };
                    payerRight.Enter();

                    //日志
                    Erp.Current.Finance.LogsApplyStepView.Enter(ApplyType.CostApply, Services.Enums.ApprovalStatus.Submit, apply.ID, Erp.Current.ID);
                    tran.Commit();

                    //重新计算余额
                    flowsView.Recalculation(apply.PayerAccountID, oldFlowId);
                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.货款申请, Services.Oplogs.GetMethodInfo(), "修改手续费", new { oldValue, newValue }.Json());
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    json.success = false;
                    json.data = "添加失败!" + ex.Message;
                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.货款申请, Services.Oplogs.GetMethodInfo(), "修改手续费 异常!", new { apply, oldValue, newValue, exception = ex.ToString() }.Json());
                    return json;
                }
            }
            return json;
        }
        #endregion
    }
}