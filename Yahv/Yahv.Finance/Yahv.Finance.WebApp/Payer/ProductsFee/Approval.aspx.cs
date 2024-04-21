using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Linq;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Finance.Services;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Models.Rolls;
using Yahv.Finance.Services.Views.Origins;
using Yahv.Finance.Services.Views.Rolls;
using Yahv.Underly;
using Yahv.Underly.Enums;
using Yahv.Underly.Enums.PvFinance;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Finance.WebApp.Payer.ProductsFee
{
    public partial class Approval : ErpParticlePage
    {
        private DbTransaction tran = null;

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
                {
                    string paymentMethord = string.Empty;       //付款方式
                    string formCode = string.Empty;         //流水号
                    string paymentDate = string.Empty;      //付款日期

                    var payerRight = payerRightsView.FirstOrDefault(item => item.PayerLeftID == data.PayerLeft.ID);
                    if (payerRight != null && !string.IsNullOrWhiteSpace(payerRight.FlowID))
                    {
                        var flowAccount = Erp.Current.Finance.FlowAccounts.FirstOrDefault(item => item.ID == payerRight.FlowID);
                        if (flowAccount != null && !string.IsNullOrWhiteSpace(flowAccount.ID))
                        {
                            paymentMethord = flowAccount.PaymentMethord.GetDescription();
                            formCode = flowAccount.FormCode;
                            paymentDate = flowAccount.PaymentDate?.ToString("yyyy-MM-dd");
                        }
                    }

                    this.Model.Data = new
                    {
                        PayeeAccountID = data.PayeeAccount?.Name,
                        PayeeCurrency = data.PayeeAccount?.Currency.GetDescription(),
                        PayeeCode = data.PayeeAccount?.Code,
                        PayeeBank = data.PayeeAccount?.BankName,
                        PayerAccountName = data.PayerAccount?.Name,
                        PayerAccountID = data.PayerAccountID,
                        PayerCurrency = data.PayerAccount?.Currency.GetDescription(),
                        PayerCode = data.PayerAccount?.Code,
                        PayerBank = data.PayerAccount?.BankName,
                        IsPaid = data.IsPaid,
                        AccountCatalogID = data.PayerLeft?.AccountCatalogID,
                        Price = data.Price,
                        ApplierID = data.Applier?.RealName,
                        ApproverID = data.Approver?.RealName,
                        Summary = data.Summary,
                        PaymentMethord = paymentMethord,
                        FormCode = formCode,
                        PaymentDate = paymentDate,
                        data.ExcuterID,
                        data.PayerName,
                        Currency = data.Currency.GetDescription(),
                    };
                }

                //付款账户 内部公司
                var accounts = Erp.Current.Finance.Accounts.Where(item => item.Status == GeneralStatus.Normal && item.EnterpriseID == data.PayerID && item.Currency == data.Currency);
                this.Model.PayerAccounts = accounts.Where(item => item.NatureType == NatureType.Public && (item.Enterprise.Type & EnterpriseAccountType.Company) != 0).ToArray()
                    .Select(item => new
                    {
                        item.ID,
                        ShortName = item.ShortName ?? item.Name,
                        CompanyName = item?.Enterprise?.Name,
                        item.BankName,
                        Currency = item.Currency.GetDescription(),
                        CurrencyID = (int)item.Currency,
                        item.Code,
                    });
            }

            //付款类型
            this.Model.AccountCatalogs = AccountCatalogsAlls.Current.Json(AccountCatalogType.Output.GetDescription());

            //审批结果
            this.Model.ApprovalStatus = ExtendsEnum.ToDictionary<Services.Enums.ApprovalStatus>()
                .Where(item => int.Parse(item.Key) > 0 && int.Parse(item.Key) < 100)
                .Select(item => new { text = item.Value, value = item.Key });


        }
        #endregion

        #region 提交保存

        protected object Submit()
        {
            var json = new JMessage() { success = true, data = "审批成功!" };
            PayerApply entity = null;
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            using (tran = reponsitory.OpenTransaction())
            using (var accountsView = new AccountsRoll(reponsitory))
            {
                try
                {
                    //修改申请
                    string id = Request.QueryString["id"];
                    //下次审批人
                    string nextApprover = Request.Form["NextApproverID"];
                    //付款账户ID
                    string payerAccountID = Request.Form["PayerAccountID"];

                    entity = Erp.Current.Finance.PayerAppliesView[id];
                    if (entity == null)
                    {
                        json.success = false;
                        json.data = "审批失败，未找到该申请信息!";
                        return json;
                    }
                    var result = (Services.Enums.ApprovalStatus)int.Parse(Request.Form["radio_result"]);
                    switch (result)
                    {
                        case Services.Enums.ApprovalStatus.Agree:
                            if (string.IsNullOrEmpty(nextApprover))
                            {
                                entity.Status = entity.IsPaid ? ApplyStauts.Completed : ApplyStauts.Paying;
                            }
                            else
                                entity.ApproverID = nextApprover;

                            var payerAccount = accountsView.FirstOrDefault(item => item.ID == payerAccountID);
                            if (payerAccount == null)
                            {
                                json.success = false;
                                json.data = "审批失败，未找到该付款账户信息!";
                                return json;
                            }

                            entity.ExcuterID = payerAccount.OwnerID;
                            entity.PayerAccountID = payerAccountID; 
                            break;
                        case Services.Enums.ApprovalStatus.Reject:
                            entity.Status = ApplyStauts.Rejecting;
                            //新增流水日志、删除核销、删除流水表
                            Delete(entity.PayerLeft.ID);
                            break;
                    }
                    entity.Enter();
                    //添加审批日志
                    Erp.Current.Finance.LogsApplyStepView.Enter(ApplyType.ProductsFee, result, id, Erp.Current.ID, Request.Form["Comments"]);
                    if (tran?.Connection != null)
                    {
                        tran.Commit();
                    }
                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.货款申请, Services.Oplogs.GetMethodInfo(), $"[{Erp.Current.ID}]审批-{result.GetDescription()}", entity.Json());
                }
                catch (Exception ex)
                {
                    if (tran?.Connection != null)
                    {
                        tran.Rollback();
                    }

                    json.success = false;
                    json.data = "审批失败!" + ex.Message;
                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.货款申请, Services.Oplogs.GetMethodInfo(), $"[{Erp.Current.ID}]审批 异常!", new { entity, exception = ex.ToString() }.Json());
                    return json;
                }
            }

            return json;
        }

        #endregion

        #region 功能函数
        /// <summary>
        /// 获取admins
        /// </summary>
        /// <returns></returns>
        protected object getAdmins()
        {
            return Yahv.Erp.Current.Finance.Admins
                    .GetApproveAdmins()
                    .OrderBy(t => t.RealName)
                    .Select(item => new { value = item.ID, text = item.RealName })
                    .ToArray();
        }

        /// <summary>
        /// 获取付款人
        /// </summary>
        /// <returns></returns>
        protected object getExcuterIds()
        {
            var ownerId = Erp.Current.Finance.Accounts
                .FirstOrDefault(item => item.ID == Request.QueryString["payerAccountId"])?.OwnerID;

            return Yahv.Erp.Current.Finance.Admins
                .GetApproveAdmins()
                .OrderBy(t => t.RealName)
                .Select(item => new
                {
                    value = item.ID,
                    text = item.RealName,
                    selected = (ownerId != null && item.ID == ownerId),
                })
                .ToArray();
        }

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
                .OrderByDescending(item => item.CreateDate).ToArray()
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

        #region 私有函数
        /// <summary>
        /// 删除流水、核销
        /// </summary>
        /// <param name="leftId"></param>
        private void Delete(string leftId)
        {
            using (var payerRightsView = new PayerRightsRoll())
            {
                var payerRight = payerRightsView.FirstOrDefault(item => item.PayerLeftID == leftId);
                if (payerRight == null)
                    return;

                var flowAccount = Erp.Current.Finance.FlowAccounts.FirstOrDefault(item => item.ID == payerRight.FlowID);
                if (flowAccount == null)
                    return;

                //新增流水表日志
                new Logs_FlowAccount()
                {
                    Currency = flowAccount.Currency,
                    CreateDate = DateTime.Now,
                    AccountID = flowAccount.AccountID,
                    AccountMethord = flowAccount.AccountMethord,
                    Balance = flowAccount.Balance,
                    Balance1 = flowAccount.Balance1,
                    CreatorID = flowAccount.CreatorID,
                    Currency1 = flowAccount.Currency1,
                    ERate1 = flowAccount.ERate1,
                    FormCode = flowAccount.FormCode,
                    PaymentDate = flowAccount.PaymentDate,
                    PaymentMethord = flowAccount.PaymentMethord,
                    Price = -flowAccount.Price,
                    Price1 = -flowAccount.Price1,
                    SourceCreateDate = flowAccount.CreateDate,
                    SourceID = flowAccount.ID,
                    TargetAccountCode = flowAccount.TargetAccountCode,
                    TargetAccountName = flowAccount.TargetAccountName,
                }.Enter();

                //删除核销
                payerRightsView.Abandon(payerRight.ID);
                //删除流水
                Erp.Current.Finance.FlowAccounts.Abandon(flowAccount.ID);
            }
        }
        #endregion
    }
}