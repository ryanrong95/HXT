using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Finance.Services;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Models.Rolls;
using Yahv.Finance.Services.Views.Rolls;
using Yahv.Underly;
using Yahv.Underly.Enums.PvFinance;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Finance.WebApp.Payer.ChargeApply
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
                var data = Erp.Current.Finance.ChargeApplies.GetApply(id);
                this.Model.Data = data;

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
            Services.Models.Origins.ChargeApply entity = null;
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

                    entity = Erp.Current.Finance.ChargeApplies.GetApply(id);
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
                                //未付款 状态改为待支付； 已付款 状态改为已完成
                                entity.Status = entity.IsPaid ? ApplyStauts.Completed : ApplyStauts.Paying;
                                if (entity.IsPaid)
                                    entity.EnterSuccess += Entity_EnterSuccess;
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
                            //新增流水日志、修改申请项支付状态、删除流水表
                            Delete(entity);
                            break;
                    }
                    entity.Enter();
                    //添加审批日志
                    Erp.Current.Finance.LogsApplyStepView.Enter(ApplyType.CostApply, result, id, Erp.Current.ID, Request.Form["Comments"]);
                    if (tran?.Connection != null)
                    {
                        tran.Commit();
                    }

                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.费用申请, Services.Oplogs.GetMethodInfo(), $"[{Erp.Current.ID}]审批-{result.GetDescription()}", entity.Json());
                }
                catch (Exception ex)
                {
                    if (tran?.Connection != null)
                    {
                        tran.Rollback();
                    }
                    json.success = false;
                    json.data = "审批失败!" + ex.Message;
                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.费用申请, Services.Oplogs.GetMethodInfo(), $"[{Erp.Current.ID}]审批 异常!", entity.Json());
                    return json;
                }
            }

            return json;
        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as Services.Models.Origins.ChargeApply;
            var result = string.Empty;
            var url = XdtApi.FinanceFee.GetApiAddress();
            InputParam<ChargeInputDto> data = null;
            try
            {
                if (tran?.Connection != null)
                {
                    tran.Commit();
                }

                using (var chargeItemsView = new ChargeApplyItemsRoll())
                using (var flowsView = new FlowAccountsRoll())
                using (var accountsView = new AccountsRoll())
                using (var filesView = new FilesDescriptionRoll())
                {
                    var chargeItems = chargeItemsView.Where(item => item.ApplyID == entity.ID).ToArray();
                    var flowsId = chargeItems.Select(item => item.FlowID).ToArray();
                    var flows = flowsView.Where(item => flowsId.Contains(item.ID)).ToArray();
                    var files = filesView.SearchByFilesMapValue(FilesMapName.ChargeApplyID.ToString(), entity.ID).ToArray();



                    if (flows == null || flows.Length <= 0 || chargeItems == null || chargeItems.Length <= 0)
                    {
                        Services.Oplogs.Oplog(Erp.Current.ID, LogModular.费用接口_Api, Services.Oplogs.GetMethodInfo(), "Api 异常", $"未找到费用信息!{entity.Json()}", url: url);
                        return;
                    }
                    else
                    {
                        var payeeAccount = accountsView.FirstOrDefault(item => item.ID == entity.PayeeAccountID);
                        var payerAccount = accountsView.FirstOrDefault(item => item.ID == entity.PayerAccountID);

                        if (payerAccount == null || string.IsNullOrEmpty(payerAccount.ID))
                        {
                            Services.Oplogs.Oplog(Erp.Current.ID, LogModular.费用接口_Api, Services.Oplogs.GetMethodInfo(), "Api 异常", $"未找到付款账户信息!{entity.Json()}", url: url);
                            return;
                        }

                        //付款账户 如果不是芯达通不需要互通
                        if (ConfigurationManager.AppSettings["XdtCompanyName"] != null && !ConfigurationManager
                            .AppSettings["XdtCompanyName"].Contains($",{payerAccount?.Enterprise?.Name},"))
                        {
                            return;
                        }

                        List<ChargeItemDto> feeItems = new List<ChargeItemDto>();

                        foreach (var chargeItem in chargeItems)
                        {
                            feeItems.Add(new ChargeItemDto()
                            {
                                FeeType = chargeItem.AccountCatalogID,
                                Amount = chargeItem?.Price ?? 0,
                                FeeDesc = chargeItem.Summary,
                            });
                        }

                        var flow = flows.FirstOrDefault();
                        data = new InputParam<ChargeInputDto>()
                        {
                            Sender = SystemSender.Center.GetFixedID(),
                            Option = OptionConsts.insert,
                            Model = new ChargeInputDto()
                            {
                                CreatorID = entity.ApplierID,
                                Rate = flow.ERate1,
                                Amount = entity.Price,
                                SeqNo = flow.FormCode,
                                Currency = entity.Currency.GetCurrency().ShortName,
                                ReceiveAccountNo = payeeAccount?.Code,
                                PaymentType = (int)flow.PaymentMethord,
                                AccountNo = payerAccount.Code,
                                PaymentDate = DateTime.Parse(flow.PaymentDate.ToString()),
                                FeeItems = feeItems,
                                MoneyType = entity.IsPaid ? 2 : 1,
                                Files = files.Select(item => new CenterFeeFile()
                                {
                                    Url = string.Concat(ConfigurationManager.AppSettings["FileWebUrlPrefix"], "/" + item.Url),
                                    FileFormat = VirtualPathUtility.GetExtension(item.Url),
                                    FileName = item.CustomName,
                                }).ToList()
                            }
                        };
                    };
                    result = Yahv.Utils.Http.ApiHelper.Current.JPost(url, data);
                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.费用接口_Api, Services.Oplogs.GetMethodInfo(), "Api 新增", result + data.Json(), url: url);
                }
            }
            catch (Exception ex)
            {
                Services.Oplogs.Oplog(Erp.Current.ID, LogModular.费用接口_Api, Services.Oplogs.GetMethodInfo(), "Api 新增异常!" + ex.Message, result + data.Json(), url: url);
            }
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
                .SearchByFilesMapValue(FilesMapName.ChargeApplyID.ToString(), Request.QueryString["id"]);
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

        /// <summary>
        /// 加载申请项
        /// </summary>
        /// <returns></returns>
        protected object data()
        {
            var id = Request.QueryString["id"];
            if (string.IsNullOrWhiteSpace(id)) return null;
            var applyItems = Erp.Current.Finance.ChargeApplyItems.Where(item => item.ApplyID == id).ToArray();
            var catalogs = Erp.Current.Finance.AccountCatalogs.ToArray();

            return from apply in applyItems
                   join catalog in catalogs on apply.AccountCatalogID equals catalog.ID
                   select new
                   {
                       AccountCatalogName = catalog.Name,
                       Price = apply.Price,
                       CreateDate = apply.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                       apply.Summary,
                   };
        }
        #endregion

        #region 私有函数
        /// <summary>
        /// 删除流水、核销
        /// </summary>
        /// <param name="applyId"></param>
        private void Delete(Services.Models.Origins.ChargeApply apply)
        {
            //资金申请项
            var applyItems = Erp.Current.Finance.ChargeApplyItems.Where(item => item.ApplyID == apply.ID).ToArray();
            var flowIds = applyItems.Select(a => a.FlowID).ToArray();       //要删除的流水表ID

            //已付款
            if (apply.IsPaid)
            {

                var flowsAccount = Erp.Current.Finance.FlowAccounts.Where(item => flowIds.Contains(item.ID)).ToArray();

                foreach (var applyItem in applyItems)
                {
                    var flowAccount = flowsAccount.FirstOrDefault(item => item.ID == applyItem.FlowID);

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


                }

            }

            //删除资金申请项
            Erp.Current.Finance.ChargeApplyItems.Abandon(applyItems.Select(item => item.ID).ToArray());

            //删除流水
            if (apply.IsPaid)
                Erp.Current.Finance.FlowAccounts.Abandon(flowIds);
        }
        #endregion
    }
}