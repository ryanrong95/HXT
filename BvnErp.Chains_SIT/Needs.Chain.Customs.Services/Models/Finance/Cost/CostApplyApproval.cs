using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class CostApplyApproval
    {
        private string _costApplyID = string.Empty;

        public CostApplyApproval(string costApplyID)
        {
            this._costApplyID = costApplyID;
        }

        /// <summary>
        /// 检查费用申请状态
        /// </summary>
        /// <param name="costApplyID"></param>
        /// <returns></returns>
        private string CheckCostApplyStatus(string costApplyID, Enums.CostStatusEnum[] costStatuses)
        {
            int[] costStatusesInt = costStatuses.Select(t => (int)t).ToArray();

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var costApply = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.CostApplies>()
                    .Where(item => item.ID == costApplyID
                        && item.Status == (int)Enums.Status.Normal
                        && costStatusesInt.Contains(item.CostStatus))
                    .FirstOrDefault();
                if (costApply == null)
                {
                    string[] costStatusesStr = costStatuses.Select(t => t.GetDescription()).ToArray();
                    return "该申请已经不是" + string.Join("、", costStatusesStr) + "状态";
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// 审批通过
        /// </summary>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public bool Approve(Admin approver, Admin payer, string approveSummary,
            Enums.CostStatusEnum currentCostStatus,
            Enums.CostStatusEnum nextCostStatus,
            out string errMsg)
        {
            var checkCostStatus = new List<Enums.CostStatusEnum>();
            checkCostStatus.Add(currentCostStatus);

            string rtnMsg = CheckCostApplyStatus(this._costApplyID, checkCostStatus.ToArray());
            if (!string.IsNullOrEmpty(rtnMsg))
            {
                errMsg = rtnMsg;
                return false;
            }

            //Enums.MoneyTypeEnum moneyType = Enums.MoneyTypeEnum.IndividualApply;

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //var costApply = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.CostApplies>().Where(t => t.ID == this._costApplyID).FirstOrDefault();
                //moneyType = (Enums.MoneyTypeEnum)costApply.MoneyType;

                reponsitory.Update<Layer.Data.Sqls.ScCustoms.CostApplies>(new
                {
                    CostStatus = (int)nextCostStatus,
                    UpdateDate = DateTime.Now,
                }, item => item.ID == this._costApplyID);

                string CostApplyFinanceStaffName = ConfigurationManager.AppSettings["CostApplyFinanceStaffName"];
                string CostApplyManagerName = ConfigurationManager.AppSettings["CostApplyManagerName"];
                string approverRealName = approver.ByName;
                if (currentCostStatus == Enums.CostStatusEnum.FinanceStaffUnApprove)
                {
                    approverRealName = CostApplyFinanceStaffName;
                }
                else if (currentCostStatus == Enums.CostStatusEnum.ManagerUnApprove)
                {
                    approverRealName = CostApplyManagerName;
                }

                reponsitory.Insert<Layer.Data.Sqls.ScCustoms.CostApplyLogs>(new Layer.Data.Sqls.ScCustoms.CostApplyLogs
                {
                    ID = Guid.NewGuid().ToString("N"),
                    CostApplyID = this._costApplyID,
                    AdminID = approver.OriginID,
                    CurrentCostStatus = (int)currentCostStatus,
                    NextCostStatus = (int)nextCostStatus,
                    CreateDate = DateTime.Now,
                    Summary = approverRealName + "通过了费用申请." + approveSummary,
                    Reason = approveSummary,
                });
            }

            //if (moneyType == Enums.MoneyTypeEnum.IndividualApply)
            //{
            if (nextCostStatus == Enums.CostStatusEnum.UnPay)
            {
                //插入数据到 PaymentNotices 表 Begin

                var costApply = new Needs.Ccs.Services.Views.CostApplyDetailView().GetResult(this._costApplyID);
                var costApplyItem = new Needs.Ccs.Services.Views.CostApplyItemsView().Where(t => t.CostApplyID == this._costApplyID).FirstOrDefault();

                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.PaymentNotices>(new Layer.Data.Sqls.ScCustoms.PaymentNotices
                    {
                        ID = Needs.Overall.PKeySigner.Pick(PKeyType.PaymentNotice),
                        AdminID = costApply.ApplicantID, //申请人
                        PayerID = payer.OriginID,  //财务的ID
                        FeeType = (int)costApplyItem.FeeType,
                        FeeDesc = costApply.FeeDesc,
                        PayeeName = costApply.PayeeName,
                        BankName = costApply.PayeeBank,
                        BankAccount = costApply.PayeeAccount,
                        Amount = costApply.Amount,
                        Currency = costApply.Currency,
                        PayDate = DateTime.Now,
                        PayType = (int)Enums.PaymentType.TransferAccount,
                        Status = (int)Enums.PaymentNoticeStatus.UnPay,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        CostApplyID = costApply.CostApplyID,
                        ExchangeRate = 1,
                        Summary = approveSummary,
                    });
                }

                //插入数据到 PaymentNotices 表 End

                //费用审批状态同步大赢家
                if (costApply.DyjID != null)
                {
                    var approve = new Finance.DyjFinance.DyjPayApprove(costApply, payer, approveSummary, true);
                    approve.PostToDYJ();
                }

            }
            //}
            //else if (moneyType == Enums.MoneyTypeEnum.BankAutoApply)
            //{

            //}

            errMsg = string.Empty;
            return true;
        }

        /// <summary>
        /// 审批拒绝
        /// </summary>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public bool Refuse(Admin admin, string approveSummary,
            Enums.CostStatusEnum currentCostStatus,
            out string errMsg)
        {
            var checkCostStatus = new List<Enums.CostStatusEnum>();
            checkCostStatus.Add(currentCostStatus);

            string rtnMsg = CheckCostApplyStatus(this._costApplyID, checkCostStatus.ToArray());
            if (!string.IsNullOrEmpty(rtnMsg))
            {
                errMsg = rtnMsg;
                return false;
            }

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.CostApplies>(new
                {
                    CostStatus = (int)Enums.CostStatusEnum.UnSubmit,
                    UpdateDate = DateTime.Now,
                }, item => item.ID == this._costApplyID);

                string CostApplyFinanceStaffName = ConfigurationManager.AppSettings["CostApplyFinanceStaffName"];
                string CostApplyManagerName = ConfigurationManager.AppSettings["CostApplyManagerName"];
                string adminRealName = admin.ByName;
                if (currentCostStatus == Enums.CostStatusEnum.FinanceStaffUnApprove)
                {
                    adminRealName = CostApplyFinanceStaffName;
                }
                else if (currentCostStatus == Enums.CostStatusEnum.ManagerUnApprove)
                {
                    adminRealName = CostApplyManagerName;
                }

                reponsitory.Insert<Layer.Data.Sqls.ScCustoms.CostApplyLogs>(new Layer.Data.Sqls.ScCustoms.CostApplyLogs
                {
                    ID = Guid.NewGuid().ToString("N"),
                    CostApplyID = this._costApplyID,
                    AdminID = admin.OriginID,
                    CurrentCostStatus = (int)currentCostStatus,
                    NextCostStatus = (int)Enums.CostStatusEnum.UnSubmit,
                    CreateDate = DateTime.Now,
                    Summary = adminRealName + "拒绝了费用申请." + approveSummary,
                    Reason = approveSummary,
                });
            }

            var costApply = new Needs.Ccs.Services.Views.CostApplyDetailView().GetResult(this._costApplyID);
            //费用审批状态同步大赢家
            if (costApply.DyjID != null)
            {
                var approve = new Finance.DyjFinance.DyjPayApprove(costApply, null, approveSummary, false);
                approve.PostToDYJ();
            }

            errMsg = string.Empty;
            return true;
        }

        /// <summary>
        /// 撤销申请
        /// </summary>
        /// <param name="admin"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public bool Cancel(Admin admin, Enums.CostStatusEnum currentCostStatus, out string errMsg)
        {
            string rtnMsg = CheckCostApplyStatus(this._costApplyID,
                new Enums.CostStatusEnum[] { Enums.CostStatusEnum.UnSubmit, Enums.CostStatusEnum.FinanceStaffUnApprove });
            if (!string.IsNullOrEmpty(rtnMsg))
            {
                errMsg = rtnMsg;
                return false;
            }

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.CostApplies>(new
                {
                    CostStatus = (int)Enums.CostStatusEnum.Cancel,
                    UpdateDate = DateTime.Now,
                }, item => item.ID == this._costApplyID);

                reponsitory.Insert<Layer.Data.Sqls.ScCustoms.CostApplyLogs>(new Layer.Data.Sqls.ScCustoms.CostApplyLogs
                {
                    ID = Guid.NewGuid().ToString("N"),
                    CostApplyID = this._costApplyID,
                    AdminID = admin.ID,
                    CurrentCostStatus = (int)currentCostStatus,
                    NextCostStatus = (int)Enums.CostStatusEnum.Cancel,
                    CreateDate = DateTime.Now,
                    Summary = admin.ByName + "撤销了费用申请",
                });
            }

            errMsg = string.Empty;
            return true;
        }

        /// <summary>
        /// 更新 CostApply 的付款时间
        /// </summary>
        public void UpdatePayTime(DateTime payTime)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.CostApplies>(new
                {
                    PayTime = payTime,
                }, item => item.ID == this._costApplyID);
            }
        }

    }
}
