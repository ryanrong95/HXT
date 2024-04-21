using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Views.Origins;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Finance.Services.Views.Rolls
{
    public class CostAppliesRoll : QueryView<CostApply, PvFinanceReponsitory>
    {
        public CostAppliesRoll()
        {
        }

        public CostAppliesRoll(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<CostApply> GetIQueryable()
        {
            var costAppliesOrigin = new CostAppliesOrigin(this.Reponsitory);
            //SenderIDs
            var senderIDs = costAppliesOrigin.Select(item => item.SenderID);

            //PayeeAccountIDs
            var payeeAccountIDs = costAppliesOrigin.Select(item => item.PayeeAccountID);

            //PayerAccountIDs
            var payerAccountIDs = costAppliesOrigin.Select(item => item.PayerAccountID);

            //ApplierIDs
            var applierIDs = costAppliesOrigin.Select(item => item.ApplierID);

            //ApproverIDs
            var approverIDs = costAppliesOrigin.Select(item => item.ApproverID);

            //ExcuterIds
            var excuterIds = costAppliesOrigin.Select(item => item.ExcuterID);

            //payerIds
            var payerIds = costAppliesOrigin.Select(item => item.PayerID);

            #region 来源系统

            var sendersOrigin = new SendersOrigin(this.Reponsitory);

            var linq_sender = from sender in sendersOrigin
                              where senderIDs.Contains(sender.ID)
                              select new
                              {
                                  SenderID = sender.ID,
                                  SenderName = sender.Name,
                              };
            #endregion

            #region 收款方

            var payeeAccountsOrigin = new AccountsOrigin(this.Reponsitory);

            var linq_payeeAccount = from payeeAccount in payeeAccountsOrigin
                                    where payeeAccountIDs.Contains(payeeAccount.ID)
                                    select new
                                    {
                                        PayeeAccountID = payeeAccount.ID,
                                        PayeeAccountName = payeeAccount.Name,
                                        PayeeAccountCode = payeeAccount.Code,
                                    };
            #endregion

            #region 付款账户

            var payerAccountsOrigin = new AccountsOrigin(this.Reponsitory);

            var linq_payerAccount = from payerAccount in payerAccountsOrigin
                                    where payerAccountIDs.Contains(payerAccount.ID)
                                    select new
                                    {
                                        PayerAccountID = payerAccount.ID,
                                        PayerAccountName = payerAccount.Name,
                                        PayerAccountCode = payerAccount.Code,
                                    };
            #endregion

            #region 申请人

            var applierAdminsTopView = new AdminsTopView(this.Reponsitory);

            var linq_applier = from applier in applierAdminsTopView
                               where applierIDs.Contains(applier.ID)
                               select new
                               {
                                   ApplierID = applier.ID,
                                   ApplierName = applier.RealName,
                               };
            #endregion

            #region 审批人
            var approverAdminsTopView = new AdminsTopView(this.Reponsitory);
            var linq_approver = from approver in approverAdminsTopView
                                where approverIDs.Contains(approver.ID)
                                select new
                                {
                                    ID = approver.ID,
                                    Name = approver.RealName,
                                };
            #endregion

            #region 付款人
            var excuterAdminsTopView = new AdminsTopView(this.Reponsitory);
            var linq_excuter = from excuter in excuterAdminsTopView
                               where excuterIds.Contains(excuter.ID)
                               select new
                               {
                                   ID = excuter.ID,
                                   Name = excuter.RealName,
                               };
            #endregion

            #region 付款公司

            var enterprisesView = new EnterprisesOrigin(this.Reponsitory);
            var linq_ep = from ep in enterprisesView
                          where payerIds.Contains(ep.ID)
                          select new
                          {
                              ep.ID,
                              ep.Name,
                          };
            #endregion

            return from costApply in costAppliesOrigin
                   join sender in linq_sender on costApply.SenderID equals sender.SenderID into ienums_sender2
                   from sender in ienums_sender2.DefaultIfEmpty()
                   join payeeAccount in linq_payeeAccount on costApply.PayeeAccountID equals payeeAccount.PayeeAccountID into ienums_payeeAccount2
                   from payeeAccount in ienums_payeeAccount2.DefaultIfEmpty()
                   join payerAccount in linq_payerAccount on costApply.PayerAccountID equals payerAccount.PayerAccountID into ienums_payerAccount2
                   from payerAccount in ienums_payerAccount2.DefaultIfEmpty()
                   join applier in linq_applier on costApply.ApplierID equals applier.ApplierID into ienums_applier2
                   from applier in ienums_applier2.DefaultIfEmpty()
                   join approver in linq_approver on costApply.ApproverID equals approver.ID into _ienums_approver
                   from approver in _ienums_approver.DefaultIfEmpty()
                   join excuter in linq_excuter on costApply.ExcuterID equals excuter.ID into _excuter
                   from excuter in _excuter.DefaultIfEmpty()
                   join ep in linq_ep on costApply.PayerID equals ep.ID into _ep
                   from ep in _ep.DefaultIfEmpty()
                   select new CostApply
                   {
                       ID = costApply.ID,
                       PayeeAccountID = costApply.PayeeAccountID,
                       PayerAccountID = costApply.PayerAccountID,
                       Type = costApply.Type,
                       IsImmediately = costApply.IsImmediately,
                       Currency = costApply.Currency,
                       Price = costApply.Price,
                       Summary = costApply.Summary,
                       CallBackUrl = costApply.CallBackUrl,
                       CallBackID = costApply.CallBackID,
                       SenderID = costApply.SenderID,
                       Department = costApply.Department,
                       ApplierID = costApply.ApplierID,
                       ExcuterID = costApply.ExcuterID,
                       CreatorID = costApply.CreatorID,
                       CreateDate = costApply.CreateDate,
                       ApproverID = costApply.ApproverID,
                       Status = costApply.Status,
                       CostPurpose = costApply.CostPurpose,
                       PayerID = costApply.PayerID,

                       SenderName = sender != null ? sender.SenderName : null,
                       PayeeAccountName = payeeAccount != null ? payeeAccount.PayeeAccountName : null,
                       PayerAccountName = payerAccount != null ? payerAccount.PayerAccountName : null,
                       ApplierName = applier != null ? applier.ApplierName : null,
                       ApproverName = approver.Name,
                       PayerAccountCode = payerAccount != null ? payerAccount.PayerAccountCode : null,
                       PayeeAccountCode = payeeAccount != null ? payeeAccount.PayeeAccountCode : null,
                       ExcuterName = excuter != null ? excuter.Name : null,
                       PayerName = ep != null ? ep.Name : null,
                   };
        }

        #region 索引器

        public CostApply this[string id]
        {
            get { return this.GetIQueryable().Single(item => item.ID == id); }
        }
        #endregion

        #region 获取申请信息
        /// <summary>
        /// 获取申请信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CostApply GetCostApply(string id)
        {
            var apply = this.IQueryable.Single(item => item.ID == id);
            var admins = new AdminsTopView(this.Reponsitory).Where(item => item.ID == apply.ApplierID || item.ID == apply.ExcuterID || item.ID == apply.ApproverID).ToArray();
            var senders = new SendersOrigin(this.Reponsitory);
            var accounts = new AccountsOrigin(this.Reponsitory).Where(item => item.ID == apply.PayeeAccountID || item.ID == apply.PayerAccountID).ToArray();


            apply.ApplierName = admins.FirstOrDefault(item => item.ID == apply.ApplierID)?.RealName;
            apply.ApproverName = admins.FirstOrDefault(item => item.ID == apply.ApproverID)?.RealName;
            apply.SenderName = senders.FirstOrDefault(item => item.ID == apply.SenderID)?.Name;
            apply.ExcuterName = admins.FirstOrDefault(item => item.ID == apply.ExcuterID)?.RealName;
            apply.PayeeAccountName = accounts.FirstOrDefault(item => item.ID == apply.PayeeAccountID)?.Name;
            apply.PayeeAccountCode = accounts.FirstOrDefault(item => item.ID == apply.PayeeAccountID)?.Code;
            apply.PayeeAccountBankName = accounts.FirstOrDefault(item => item.ID == apply.PayeeAccountID)?.BankName;
            apply.PayeeAccountCurrencyDes = accounts.FirstOrDefault(item => item.ID == apply.PayeeAccountID)?.Currency.GetDescription();
            apply.PayeeAccountCurrency = accounts.FirstOrDefault(item => item.ID == apply.PayeeAccountID)?.Currency ?? Currency.Unknown;
            apply.PayerAccountName = accounts.FirstOrDefault(item => item.ID == apply.PayerAccountID)?.Name;
            apply.PayerAccountCode = accounts.FirstOrDefault(item => item.ID == apply.PayerAccountID)?.Code;
            apply.PayerAccountBankName = accounts.FirstOrDefault(item => item.ID == apply.PayerAccountID)?.BankName;
            apply.PayerAccountCurrencyDes = accounts.FirstOrDefault(item => item.ID == apply.PayerAccountID)?.Currency.GetDescription();
            apply.PayerAccountCurrency = accounts.FirstOrDefault(item => item.ID == apply.PayerAccountID)?.Currency ?? Currency.Unknown;
            apply.TypeName = apply.Type.GetDescription();
            apply.CostPurposeDes = apply.CostPurpose.GetDescription();

            var applyItemFirst = new CostApplyItemsRoll().FirstOrDefault(item => item.ApplyID == apply.ID);
            if (applyItemFirst != null)
            {
                apply.IsPaid = applyItemFirst.IsPaid;
                apply.IsPaidDes = apply.IsPaid ? "是" : "否";

                //已付款
                if (applyItemFirst.IsPaid || apply.Status == ApplyStauts.Completed)
                {
                    var flowsFirst = new FlowAccountsOrigin(this.Reponsitory).FirstOrDefault(item => item.ID == applyItemFirst.FlowID);

                    apply.PaymentDateDes = flowsFirst?.PaymentDate?.ToString("yyyy-MM-dd");
                    apply.FormCode = flowsFirst?.FormCode;
                    apply.PaymentMethordDes = flowsFirst?.PaymentMethord.GetDescription();
                }
            }

            return apply;
        }
        #endregion
    }

    #region bak
    public class _bak_CostAppliesRoll : QueryView<CostApply, PvFinanceReponsitory>
    {
        public _bak_CostAppliesRoll()
        {
        }

        protected _bak_CostAppliesRoll(PvFinanceReponsitory reponsitory, IQueryable<CostApply> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<CostApply> GetIQueryable()
        {
            var costAppliesOrigin = new CostAppliesOrigin(this.Reponsitory);

            var iQuery = from costApply in costAppliesOrigin
                         select new CostApply
                         {
                             ID = costApply.ID,
                             PayeeAccountID = costApply.PayeeAccountID,
                             PayerAccountID = costApply.PayerAccountID,
                             Type = costApply.Type,
                             IsImmediately = costApply.IsImmediately,
                             Currency = costApply.Currency,
                             Price = costApply.Price,
                             Summary = costApply.Summary,
                             CallBackUrl = costApply.CallBackUrl,
                             CallBackID = costApply.CallBackID,
                             SenderID = costApply.SenderID,
                             Department = costApply.Department,
                             ApplierID = costApply.ApplierID,
                             ExcuterID = costApply.ExcuterID,
                             CreatorID = costApply.CreatorID,
                             CreateDate = costApply.CreateDate,
                             ApproverID = costApply.ApproverID,
                             Status = costApply.Status,
                         };

            return iQuery;
        }

        #region 索引器

        public CostApply this[string id]
        {
            get { return this.GetIQueryable().Single(item => item.ID == id); }
        }
        #endregion

        #region 分页方法
        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<CostApply> iquery = this.IQueryable.Cast<CostApply>();
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_myCostApply = iquery.ToArray();

            //SenderIDs
            var senderIDs = ienum_myCostApply.Select(item => item.SenderID);

            //PayeeAccountIDs
            var payeeAccountIDs = ienum_myCostApply.Select(item => item.PayeeAccountID);

            //PayerAccountIDs
            var payerAccountIDs = ienum_myCostApply.Select(item => item.PayerAccountID);

            //ApplierIDs
            var applierIDs = ienum_myCostApply.Select(item => item.ApplierID);

            //ApproverIDs
            var approverIDs = ienum_myCostApply.Select(item => item.ApproverID);

            #region 来源系统

            var sendersOrigin = new SendersOrigin(this.Reponsitory);

            var linq_sender = from sender in sendersOrigin
                              where senderIDs.Contains(sender.ID)
                              select new
                              {
                                  SenderID = sender.ID,
                                  SenderName = sender.Name,
                              };

            var ienums_sender = linq_sender.ToArray();

            #endregion

            #region 收款方

            var payeeAccountsOrigin = new AccountsOrigin(this.Reponsitory);

            var linq_payeeAccount = from payeeAccount in payeeAccountsOrigin
                                    where payeeAccountIDs.Contains(payeeAccount.ID)
                                    select new
                                    {
                                        PayeeAccountID = payeeAccount.ID,
                                        PayeeAccountName = payeeAccount.Name,
                                    };

            var ienums_payeeAccount = linq_payeeAccount.ToArray();

            #endregion

            #region 付款账户

            var payerAccountsOrigin = new AccountsOrigin(this.Reponsitory);

            var linq_payerAccount = from payerAccount in payerAccountsOrigin
                                    where payerAccountIDs.Contains(payerAccount.ID)
                                    select new
                                    {
                                        PayerAccountID = payerAccount.ID,
                                        PayerAccountName = payerAccount.Name,
                                    };

            var ienums_payerAccount = linq_payerAccount.ToArray();

            #endregion

            #region 申请人

            var applierAdminsTopView = new AdminsTopView(this.Reponsitory);

            var linq_applier = from applier in applierAdminsTopView
                               where applierIDs.Contains(applier.ID)
                               select new
                               {
                                   ApplierID = applier.ID,
                                   ApplierName = applier.RealName,
                               };

            var ienums_applier = linq_applier.ToArray();

            #endregion

            #region 审批人
            var approverAdminsTopView = new AdminsTopView(this.Reponsitory);
            var linq_approver = from approver in approverAdminsTopView
                                where approverIDs.Contains(approver.ID)
                                select new
                                {
                                    ID = approver.ID,
                                    Name = approver.RealName,
                                };

            var ienums_approver = linq_approver.ToArray();
            #endregion

            var ienums_linq = from costApply in ienum_myCostApply
                              join sender in ienums_sender on costApply.SenderID equals sender.SenderID into ienums_sender2
                              from sender in ienums_sender2.DefaultIfEmpty()
                              join payeeAccount in ienums_payeeAccount on costApply.PayeeAccountID equals payeeAccount.PayeeAccountID into ienums_payeeAccount2
                              from payeeAccount in ienums_payeeAccount2.DefaultIfEmpty()
                              join payerAccount in ienums_payerAccount on costApply.PayerAccountID equals payerAccount.PayerAccountID into ienums_payerAccount2
                              from payerAccount in ienums_payerAccount2.DefaultIfEmpty()
                              join applier in ienums_applier on costApply.ApplierID equals applier.ApplierID into ienums_applier2
                              from applier in ienums_applier2.DefaultIfEmpty()
                              join approver in ienums_approver on costApply.ApproverID equals approver.ID into _ienums_approver
                              from approver in _ienums_approver.DefaultIfEmpty()
                              select new CostApply
                              {
                                  ID = costApply.ID,
                                  PayeeAccountID = costApply.PayeeAccountID,
                                  PayerAccountID = costApply.PayerAccountID,
                                  Type = costApply.Type,
                                  IsImmediately = costApply.IsImmediately,
                                  Currency = costApply.Currency,
                                  Price = costApply.Price,
                                  Summary = costApply.Summary,
                                  CallBackUrl = costApply.CallBackUrl,
                                  CallBackID = costApply.CallBackID,
                                  SenderID = costApply.SenderID,
                                  Department = costApply.Department,
                                  ApplierID = costApply.ApplierID,
                                  ExcuterID = costApply.ExcuterID,
                                  CreatorID = costApply.CreatorID,
                                  CreateDate = costApply.CreateDate,
                                  ApproverID = costApply.ApproverID,
                                  Status = costApply.Status,

                                  SenderName = sender != null ? sender.SenderName : "",
                                  PayeeAccountName = payeeAccount != null ? payeeAccount.PayeeAccountName : "",
                                  PayerAccountName = payerAccount != null ? payerAccount.PayerAccountName : "",
                                  ApplierName = applier != null ? applier.ApplierName : "",
                                  ApproverName = approver.Name,
                              };

            var results = ienums_linq.ToArray();

            Func<CostApply, object> convert = item => new
            {
                ID = item.ID,
                SenderName = item.SenderName,
                PayerAccountName = item.PayerAccountName,
                PayeeAccountName = item.PayeeAccountName,
                CurrencyDes = item.Currency.GetDescription(),
                Price = item.Price.ToRound1(2),
                ApplierName = item.ApplierName,
                StatusDes = GetStatusName(item),
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                item.Status,
            };

            var ss = results.Select(convert).ToArray();

            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                Func<dynamic, object> convertAgain = item => new
                {

                };

                return results.Select(convert).Select(convertAgain).ToArray();
            }

            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = results.Select(convert).ToArray(),
            };
        }

        /// <summary>
        /// 根据付款账户ID查询
        /// </summary>
        /// <param name="payerAccountID"></param>
        /// <returns></returns>
        public _bak_CostAppliesRoll SearchByPayerAccountID(string payerAccountID)
        {
            var linq = from query in this.IQueryable
                       where query.PayerAccountID == payerAccountID
                       select query;

            var view = new _bak_CostAppliesRoll(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据状态查询
        /// </summary>
        /// <param name="stauts"></param>
        /// <returns></returns>
        public _bak_CostAppliesRoll SearchByStatus(ApplyStauts stauts)
        {
            var linq = from query in this.IQueryable
                       where query.Status == stauts
                       select query;

            var view = new _bak_CostAppliesRoll(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据审批人ID查询
        /// </summary>
        /// <param name="approverID"></param>
        /// <returns></returns>
        public _bak_CostAppliesRoll SearchByApproverID(string approverID)
        {
            var linq = from query in this.IQueryable
                       where query.ApproverID == approverID
                       select query;

            var view = new _bak_CostAppliesRoll(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据 CostApplyID 查询
        /// </summary>
        /// <param name="costApplyID"></param>
        /// <returns></returns>
        public _bak_CostAppliesRoll SearchByCostApplyID(string costApplyID)
        {
            var linq = from query in this.IQueryable
                       where query.ID.Contains(costApplyID)
                       select query;

            var view = new _bak_CostAppliesRoll(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 按照 CreateDate 正序排列
        /// </summary>
        /// <returns></returns>
        public _bak_CostAppliesRoll SearchOrderByCreateDate()
        {
            var linq = this.IQueryable.OrderBy(t => t.CreateDate);

            var view = new _bak_CostAppliesRoll(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 按照 CreateDate 倒序排列
        /// </summary>
        /// <returns></returns>
        public _bak_CostAppliesRoll SearchOrderByDescendingCreateDate()
        {
            var linq = this.IQueryable.OrderByDescending(t => t.CreateDate);

            var view = new _bak_CostAppliesRoll(this.Reponsitory, linq);
            return view;
        }
        #endregion

        #region 获取申请信息
        /// <summary>
        /// 获取申请信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CostApply GetCostApply(string id)
        {
            var apply = this.IQueryable.Single(item => item.ID == id);
            var admins = new AdminsTopView(this.Reponsitory).Where(item => item.ID == apply.ApplierID || item.ID == apply.ExcuterID || item.ID == apply.ApproverID).ToArray();
            var senders = new SendersOrigin(this.Reponsitory);
            var accounts = new AccountsOrigin(this.Reponsitory).Where(item => item.ID == apply.PayeeAccountID || item.ID == apply.PayerAccountID).ToArray();


            apply.ApplierName = admins.FirstOrDefault(item => item.ID == apply.ApplierID)?.RealName;
            apply.ApproverName = admins.FirstOrDefault(item => item.ID == apply.ApproverID)?.RealName;
            apply.SenderName = senders.FirstOrDefault(item => item.ID == apply.SenderID)?.Name;
            apply.ExcuterName = admins.FirstOrDefault(item => item.ID == apply.ExcuterID)?.RealName;
            apply.PayeeAccountName = accounts.FirstOrDefault(item => item.ID == apply.PayeeAccountID)?.Name;
            apply.PayeeAccountCode = accounts.FirstOrDefault(item => item.ID == apply.PayeeAccountID)?.Code;
            apply.PayeeAccountBankName = accounts.FirstOrDefault(item => item.ID == apply.PayeeAccountID)?.BankName;
            apply.PayeeAccountCurrencyDes = accounts.FirstOrDefault(item => item.ID == apply.PayeeAccountID)?.Currency.GetDescription();
            apply.PayerAccountName = accounts.FirstOrDefault(item => item.ID == apply.PayerAccountID)?.Name;
            apply.PayerAccountCode = accounts.FirstOrDefault(item => item.ID == apply.PayerAccountID)?.Code;
            apply.PayerAccountBankName = accounts.FirstOrDefault(item => item.ID == apply.PayerAccountID)?.BankName;
            apply.PayerAccountCurrencyDes = accounts.FirstOrDefault(item => item.ID == apply.PayerAccountID)?.Currency.GetDescription();
            apply.TypeName = apply.Type.GetDescription();

            var applyItemFirst = new CostApplyItemsRoll().FirstOrDefault(item => item.ApplyID == apply.ID);
            if (applyItemFirst != null)
            {
                apply.IsPaid = applyItemFirst.IsPaid;
                apply.IsPaidDes = apply.IsPaid ? "是" : "否";

                //已付款
                if (applyItemFirst.IsPaid || apply.Status == ApplyStauts.Completed)
                {
                    var flowsFirst = new FlowAccountsOrigin(this.Reponsitory).FirstOrDefault(item => item.ID == applyItemFirst.FlowID);

                    apply.PaymentDateDes = flowsFirst?.PaymentDate?.ToString("yyyy-MM-dd");
                    apply.FormCode = flowsFirst?.FormCode;
                    apply.PaymentMethordDes = flowsFirst?.PaymentMethord.GetDescription();
                }
            }

            return apply;
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 获取状态名称
        /// </summary>
        /// <param name="apply"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        string GetStatusName(CostApply apply)
        {
            string result = String.Empty;
            switch (apply.Status)
            {
                case ApplyStauts.Completed:
                    result = apply.Status.GetDescription();
                    break;
                case ApplyStauts.Rejecting:
                    result = apply.ApplierName != null ? $"{apply.Status.GetDescription()}({apply.ApplierName})" : apply.Status.GetDescription();
                    break;
                case ApplyStauts.Paying:
                    result = apply.ExcuterName != null ? $"{apply.Status.GetDescription()}({apply.ExcuterName})" : apply.Status.GetDescription();
                    break;
                default:
                    result = apply.ApproverName != null ? $"{apply.Status.GetDescription()}({apply.ApproverName})" : apply.Status.GetDescription();
                    break;
            }
            return result;
        }
        #endregion
    }
    #endregion
}
