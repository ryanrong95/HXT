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

namespace Yahv.Finance.WebApp.Payer.AcceptanceApply
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

                using (var view = new AcceptanceAppliesRoll())
                using (var moneyOrdersView = new MoneyOrdersRoll())
                {
                    var data = view.Find(id);

                    var leftOut = view.GetAcceptanceLeft(id, AccountMethord.Output);
                    var leftIn = view.GetAcceptanceLeft(id, AccountMethord.Input);
                    var moneyOrder = moneyOrdersView[data.MoneyOrderID];

                    this.Model.Data = new
                    {
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
                        PayeePrice = leftIn.Price,
                        data.ExcuterID,
                        data.TypeName,
                        data.Type,
                        data.MoneyOrderID,
                        data.ApproverName,
                        MoneyOrderCode = moneyOrder?.Code,
                    };
                }
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

            //修改申请
            string id = Request.QueryString["id"];
            //下次审批人
            string nextApprover = Request.Form["NextApproverID"];
            Services.Models.Origins.AcceptanceApply entity = null;

            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            using (tran = reponsitory.OpenTransaction())
            using (var appliesView = new AcceptanceAppliesRoll(reponsitory))
            {
                try
                {
                    entity = appliesView[id];
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
                    Erp.Current.Finance.LogsApplyStepView.Enter(ApplyType.AcceptanceApply, result, id, Erp.Current.ID, Request.Form["Comments"]);

                    if (tran?.Connection != null)
                    {
                        tran.Commit();
                    }

                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.承兑调拨申请, Services.Oplogs.GetMethodInfo(), $"[{Erp.Current.ID}]审批-{result.GetDescription()}", entity.Json());
                }
                catch (Exception ex)
                {
                    if (tran?.Connection != null)
                    {
                        tran.Rollback();
                    }
                    json.success = false;
                    json.data = "审批失败!" + ex.Message;
                    Services.Oplogs.Oplog(Erp.Current.ID, LogModular.承兑调拨申请, Services.Oplogs.GetMethodInfo(), $"[{Erp.Current.ID}]审批 异常!", new { entity, exception = ex.ToString() }.Json());
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