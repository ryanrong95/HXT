using System;
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
using Yahv.Finance.Services.Views.Rolls;
using Yahv.Underly;
using Yahv.Underly.Enums;
using Yahv.Underly.Enums.PvFinance;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Finance.WebApp.Payer.FundTransfer
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
                var data = Erp.Current.Finance.SelfAppliesView[id];
                using (var view = new SelfLeftsRoll())
                {
                    this.Model.Data = new
                    {
                        PayeeAccountID = data.PayeeAccount?.ShortName ?? data.PayeeAccount?.Name,
                        PayeeCurrency = data.PayeeAccount?.Currency.GetDescription(),
                        PayeeCode = data.PayeeAccount?.Code,
                        PayeeBank = data.PayeeAccount?.BankName,
                        PayerAccountName = data.PayerAccount?.ShortName ?? data.PayerAccount?.Name,
                        PayerAccountID = data.PayerAccountID,
                        PayerCurrency = data.PayerAccount?.Currency.GetDescription(),
                        PayerCode = data.PayerAccount?.Code,
                        PayerBank = data.PayerAccount?.BankName,
                        AccountCatalogID = view.FirstOrDefault(item => item.ApplyID == id)?.AccountCatalogID,
                        ApplierID = data.Applier?.RealName,
                        ApproverID = data.Approver?.RealName,
                        Summary = data.Summary,
                        PayerPrice = data.Price,
                        PayeePrice = data.TargetPrice,
                        Rate = data.TargetERate,
                        data.ExcuterID,
                    };
                }
            }

            //调拨类型
            //this.Model.AccountCatalogs = AccountCatalogsAlls.Current.Json(AccountCatalogType.Output.GetDescription());
            this.Model.AccountCatalogs = ExtendsEnum.ToDictionary<FundTransferType>().Select(item => new { text = item.Value, value = item.Key });

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

            //修改申请
            string id = Request.QueryString["id"];
            //下次审批人
            string nextApprover = Request.Form["NextApproverID"];
            SelfApply entity = null;

            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            using (tran = reponsitory.OpenTransaction())
            {
                try
                {
                    entity = Erp.Current.Finance.SelfAppliesView[id];
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
                                entity.Status = ApplyStauts.Paying;
                            }
                            else
                                entity.ApproverID = nextApprover;

                            entity.ExcuterID = Request.Form["ExcuterID"];
                            break;
                        case Services.Enums.ApprovalStatus.Reject:
                            entity.Status = ApplyStauts.Rejecting;
                            break;
                    }
                    entity.Enter();
                    //添加审批日志
                    Erp.Current.Finance.LogsApplyStepView.Enter(ApplyType.FundTransfer, result, id, Erp.Current.ID, Request.Form["Comments"]);

                    if (tran?.Connection != null)
                    {
                        tran.Commit();
                    }

                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.资金调拨申请, Services.Oplogs.GetMethodInfo(), $"[{Erp.Current.ID}]审批-{result.GetDescription()}", entity.Json());
                }
                catch (Exception ex)
                {
                    if (tran?.Connection != null)
                    {
                        tran.Rollback();
                    }
                    json.success = false;
                    json.data = "审批失败!" + ex.Message;
                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.资金调拨申请, Services.Oplogs.GetMethodInfo(), $"[{Erp.Current.ID}]审批 异常!", new { entity, exception = ex.ToString() }.Json());
                    return json;
                }
            }

            return json;
        }

        /// <summary>
        /// 芯达通 同步数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Entity_Success(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as SelfApply;
            var result = string.Empty;
            var url = XdtApi.FundTransfer.GetApiAddress();
            InputParam<FundTransferInputDto> data = null;
            try
            {
                if (tran?.Connection != null)
                {
                    tran.Commit();
                }

                using (var selfLeftsView = new SelfLeftsRoll())
                using (var selfRightsView = new SelfRightsRoll())
                using (var flowsView = new FlowAccountsRoll())
                using (var accountsView = new AccountsRoll())
                {
                    var outLeft = selfLeftsView.FirstOrDefault(item => item.ApplyID == entity.ID && item.AccountMethord == AccountMethord.Output);
                    var inLeft = selfLeftsView.FirstOrDefault(item => item.ApplyID == entity.ID && item.AccountMethord == AccountMethord.Input);

                    var outRight = selfRightsView.FirstOrDefault(item => item.SelfLeftID == outLeft.ID);
                    var inRight = selfRightsView.FirstOrDefault(item => item.SelfLeftID == inLeft.ID);

                    var outFlow = flowsView.FirstOrDefault(item => item.ID == outRight.FlowID);
                    var inFlow = flowsView.FirstOrDefault(item => item.ID == inRight.FlowID);

                    if (outFlow == null || string.IsNullOrEmpty(outFlow.ID) || inFlow == null || string.IsNullOrEmpty(inFlow.ID))
                    {
                        Services.Oplogs.Oplog(Erp.Current.ID, LogModular.调拨接口_Api, Services.Oplogs.GetMethodInfo(), "Api 异常", $"未找到流水!{entity.Json()}", url: url);
                        return;
                    }
                    else
                    {
                        var payeeAccount = accountsView.FirstOrDefault(item => item.ID == entity.PayeeAccountID);
                        var payerAccount = accountsView.FirstOrDefault(item => item.ID == entity.PayerAccountID);
                        var erate = ExchangeRates.Universal[outFlow.Currency, inFlow.Currency];

                        //调出账户 如果不是芯达通不需要互通
                        if (ConfigurationManager.AppSettings["XdtCompanyName"] != null && !ConfigurationManager
                            .AppSettings["XdtCompanyName"].Contains($",{payerAccount?.Enterprise?.Name},"))
                        {
                            return;
                        }

                        data = new InputParam<FundTransferInputDto>()
                        {
                            Sender = SystemSender.Center.GetFixedID(),
                            Option = OptionConsts.insert,
                            Model = new FundTransferInputDto()
                            {
                                CreatorID = entity.ApplierID,
                                Rate = erate,
                                FeeType = outLeft.AccountCatalogID,
                                PaymentType = (int)outFlow.PaymentMethord,
                                PaymentDate = DateTime.Parse(outFlow.PaymentDate.ToString()),

                                OutAmount = Math.Abs(outFlow.Price),
                                OutCurrency = outFlow.Currency.GetCurrency().ShortName,
                                OutSeqNo = outFlow.FormCode,
                                OutAccountNo = payerAccount.Code,

                                InAmount = inFlow.Price,
                                InCurrency = inFlow.Currency.GetCurrency().ShortName,
                                InSeqNo = inFlow.FormCode,
                                InAccountNo = payeeAccount.Code,

                            }
                        };
                    };
                    result = Yahv.Utils.Http.ApiHelper.Current.JPost(url, data);
                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.调拨接口_Api, Services.Oplogs.GetMethodInfo(), "向芯达通互通", result + data.Json(), url: url);
                }
            }
            catch (Exception ex)
            {
                Services.Oplogs.Oplog(Erp.Current.ID, LogModular.调拨接口_Api, Services.Oplogs.GetMethodInfo(), "向芯达通互通异常!" + ex.Message, result + data.Json(), url: url);
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
    }
}