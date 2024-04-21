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
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Views.Rolls;
using Yahv.Underly;
using Yahv.Underly.Enums;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Finance.WebApp.Payer.RefundApply
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

            using (var view = new RefundAppliesRoll())
            {
                var data = view[id];
                this.Model.Data = new
                {
                    PayeeLeftID = data.PayeeLeftID,
                    PayeeCurrency = data.CurrencyName,
                    PayerAccountName = data.PayerAccountName,
                    PayeeAccountName = data.PayeeAccountName,
                    Price = data.Price,
                    ApplierName = data.ApplierName,
                    ApproverName = data.ApproverName,
                    Summary = data.Summary,
                    PayerAccountID = data.PayerAccountID,
                    data.TypeName,
                };

                //付款账户 内部公司
                var accounts = Erp.Current.Finance.Accounts
                    .Where(item => item.Status == GeneralStatus.Normal
                                   //&& item.EnterpriseID == data.PayeeLeftAccountID
                                   && item.Currency == data.Currency);
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
            Services.Models.Origins.RefundApply entity = null;
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            using (tran = reponsitory.OpenTransaction())
            using (var refundApliesView = new RefundAppliesRoll(reponsitory))
            using (var accountsView = new AccountsRoll(reponsitory))
            {
                try
                {
                    //修改申请
                    string id = Request.QueryString["id"];
                    //下次审批人
                    string nextApprover = Request.Form["NextApproverID"];
                    var payerAccountID = Request.Form["PayerAccountID"];

                    entity = refundApliesView.FindRefundApply(id);
                    if (entity == null)
                    {
                        json.success = false;
                        json.data = "审批失败，未找到该申请信息!";
                        return json;
                    }

                    var payerAccount = accountsView.FirstOrDefault(item => item.ID == payerAccountID);
                    if (payerAccount == null)
                    {
                        json.success = false;
                        json.data = "审批失败，未找到该付款账户!";
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

                            entity.ExcuterID = payerAccount.OwnerID;
                            entity.PayerAccountID = payerAccount.ID;
                            break;
                        case Services.Enums.ApprovalStatus.Reject:
                            entity.Status = ApplyStauts.Rejecting;
                            break;
                    }
                    entity.Enter();
                    //添加审批日志
                    Erp.Current.Finance.LogsApplyStepView.Enter(ApplyType.RefundApply, result, id, Erp.Current.ID, Request.Form["Comments"]);
                    if (tran?.Connection != null)
                    {
                        tran.Commit();
                    }
                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.预收退款申请, Services.Oplogs.GetMethodInfo(), $"[{Erp.Current.ID}]审批-{result.GetDescription()}", entity.Json());
                }
                catch (Exception ex)
                {
                    if (tran?.Connection != null)
                    {
                        tran.Rollback();
                    }

                    json.success = false;
                    json.data = "审批失败!" + ex.Message;
                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.预收退款申请, Services.Oplogs.GetMethodInfo(), $"[{Erp.Current.ID}]审批 异常!", new { entity, exception = ex.ToString() }.Json());
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
                .SearchByFilesMapValue(FilesMapName.RefundApplyID.ToString(), Request.QueryString["id"]);
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