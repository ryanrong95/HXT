using System;
using System.Linq;
using Layers.Data.Sqls;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Views.Origins;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Finance.Services.Views.Rolls
{
    public class ChargeAppliesRoll : QueryView<ChargeApply, PvFinanceReponsitory>
    {
        public ChargeAppliesRoll()
        {
        }

        public ChargeAppliesRoll(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<ChargeApply> GetIQueryable()
        {
            var appliesOrigin = new ChargeAppliesOrigin(this.Reponsitory);
            //SenderIDs
            var senderIDs = appliesOrigin.Select(item => item.SenderID);

            //PayeeAccountIDs
            var payeeAccountIDs = appliesOrigin.Select(item => item.PayeeAccountID);

            //PayerAccountIDs
            var payerAccountIDs = appliesOrigin.Select(item => item.PayerAccountID);

            //ApplierIDs
            var applierIDs = appliesOrigin.Select(item => item.ApplierID);

            //ApproverIDs
            var approverIDs = appliesOrigin.Select(item => item.ApproverID);

            //excuterIDs
            var excuterIds = appliesOrigin.Select(item => item.ExcuterID);

            //payerIds
            var payerIds = appliesOrigin.Select(item => item.PayerID);

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

            return from costApply in appliesOrigin
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
                   select new ChargeApply
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

        public ChargeApply this[string id]
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
        public ChargeApply GetApply(string id)
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
            apply.IsImmediatelyDes = (apply?.IsImmediately ?? false) ? "是" : "否";
            apply.PayerName = apply.PayerName;
            apply.CurrencyName = apply.Currency.GetDescription();

            var applyItemFirst = new ChargeApplyItemsRoll().FirstOrDefault(item => item.ApplyID == apply.ID);
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

    #region _bak
    ///// <summary>
    ///// 费用申请视图
    ///// </summary>
    //public class ChargeAppliesRoll : QueryView<ChargeApply, PvFinanceReponsitory>
    //{
    //    public ChargeAppliesRoll()
    //    {
    //    }

    //    protected ChargeAppliesRoll(PvFinanceReponsitory reponsitory, IQueryable<ChargeApply> iQueryable) : base(reponsitory, iQueryable)
    //    {
    //    }

    //    protected override IQueryable<ChargeApply> GetIQueryable()
    //    {
    //        var chargeAppliesOrigin = new ChargeAppliesOrigin(this.Reponsitory);

    //        var iQuery = from costApply in chargeAppliesOrigin
    //                     select new ChargeApply
    //                     {
    //                         ID = costApply.ID,
    //                         PayeeAccountID = costApply.PayeeAccountID,
    //                         PayerAccountID = costApply.PayerAccountID,
    //                         Type = costApply.Type,
    //                         IsImmediately = costApply.IsImmediately,
    //                         Currency = costApply.Currency,
    //                         Price = costApply.Price,
    //                         Summary = costApply.Summary,
    //                         CallBackUrl = costApply.CallBackUrl,
    //                         CallBackID = costApply.CallBackID,
    //                         SenderID = costApply.SenderID,
    //                         Department = costApply.Department,
    //                         ApplierID = costApply.ApplierID,
    //                         ExcuterID = costApply.ExcuterID,
    //                         CreatorID = costApply.CreatorID,
    //                         CreateDate = costApply.CreateDate,
    //                         ApproverID = costApply.ApproverID,
    //                         Status = costApply.Status,
    //                     };

    //        return iQuery;
    //    }

    //    /// <summary>
    //    /// 分页方法
    //    /// </summary>
    //    /// <returns></returns>
    //    public object ToMyPage(int? pageIndex = null, int? pageSize = null)
    //    {
    //        IQueryable<ChargeApply> iquery = this.IQueryable.Cast<ChargeApply>();
    //        int total = iquery.Count();

    //        if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
    //        {
    //            iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
    //        }

    //        //获取数据
    //        var ienum_myChargeApply = iquery.ToArray();

    //        //SenderIDs
    //        var senderIDs = ienum_myChargeApply.Select(item => item.SenderID);

    //        //PayeeAccountIDs
    //        var payeeAccountIDs = ienum_myChargeApply.Select(item => item.PayeeAccountID);

    //        //PayerAccountIDs
    //        var payerAccountIDs = ienum_myChargeApply.Select(item => item.PayerAccountID);

    //        //ApplierIDs
    //        var applierIDs = ienum_myChargeApply.Select(item => item.ApplierID);

    //        #region 来源系统

    //        var sendersOrigin = new SendersOrigin(this.Reponsitory);

    //        var linq_sender = from sender in sendersOrigin
    //                          where senderIDs.Contains(sender.ID)
    //                          select new
    //                          {
    //                              SenderID = sender.ID,
    //                              SenderName = sender.Name,
    //                          };

    //        var ienums_sender = linq_sender.ToArray();

    //        #endregion

    //        #region 收款方

    //        var payeeAccountsOrigin = new AccountsOrigin(this.Reponsitory);

    //        var linq_payeeAccount = from payeeAccount in payeeAccountsOrigin
    //                                where payeeAccountIDs.Contains(payeeAccount.ID)
    //                                select new
    //                                {
    //                                    PayeeAccountID = payeeAccount.ID,
    //                                    PayeeAccountName = payeeAccount.Name,
    //                                };

    //        var ienums_payeeAccount = linq_payeeAccount.ToArray();

    //        #endregion

    //        #region 付款账户

    //        var payerAccountsOrigin = new AccountsOrigin(this.Reponsitory);

    //        var linq_payerAccount = from payerAccount in payerAccountsOrigin
    //                                where payerAccountIDs.Contains(payerAccount.ID)
    //                                select new
    //                                {
    //                                    PayerAccountID = payerAccount.ID,
    //                                    PayerAccountName = payerAccount.Name,
    //                                };

    //        var ienums_payerAccount = linq_payerAccount.ToArray();

    //        #endregion

    //        #region 申请人

    //        var applierAdminsTopView = new AdminsTopView(this.Reponsitory);

    //        var linq_applier = from applier in applierAdminsTopView
    //                           where applierIDs.Contains(applier.ID)
    //                           select new
    //                           {
    //                               ApplierID = applier.ID,
    //                               ApplierName = applier.RealName,
    //                           };

    //        var ienums_applier = linq_applier.ToArray();

    //        #endregion

    //        var ienums_linq = from chargeApply in ienum_myChargeApply
    //                          join sender in ienums_sender on chargeApply.SenderID equals sender.SenderID into ienums_sender2
    //                          from sender in ienums_sender2.DefaultIfEmpty()
    //                          join payeeAccount in ienums_payeeAccount on chargeApply.PayeeAccountID equals payeeAccount.PayeeAccountID into ienums_payeeAccount2
    //                          from payeeAccount in ienums_payeeAccount2.DefaultIfEmpty()
    //                          join payerAccount in ienums_payerAccount on chargeApply.PayerAccountID equals payerAccount.PayerAccountID into ienums_payerAccount2
    //                          from payerAccount in ienums_payerAccount2.DefaultIfEmpty()
    //                          join applier in ienums_applier on chargeApply.ApplierID equals applier.ApplierID into ienums_applier2
    //                          from applier in ienums_applier2.DefaultIfEmpty()
    //                          select new ChargeApply
    //                          {
    //                              ID = chargeApply.ID,
    //                              PayeeAccountID = chargeApply.PayeeAccountID,
    //                              PayerAccountID = chargeApply.PayerAccountID,
    //                              Type = chargeApply.Type,
    //                              IsImmediately = chargeApply.IsImmediately,
    //                              Currency = chargeApply.Currency,
    //                              Price = chargeApply.Price,
    //                              Summary = chargeApply.Summary,
    //                              CallBackUrl = chargeApply.CallBackUrl,
    //                              CallBackID = chargeApply.CallBackID,
    //                              SenderID = chargeApply.SenderID,
    //                              Department = chargeApply.Department,
    //                              ApplierID = chargeApply.ApplierID,
    //                              ExcuterID = chargeApply.ExcuterID,
    //                              CreatorID = chargeApply.CreatorID,
    //                              CreateDate = chargeApply.CreateDate,
    //                              ApproverID = chargeApply.ApproverID,
    //                              Status = chargeApply.Status,

    //                              SenderName = sender != null ? sender.SenderName : "",
    //                              PayeeAccountName = payeeAccount != null ? payeeAccount.PayeeAccountName : "",
    //                              PayerAccountName = payerAccount != null ? payerAccount.PayerAccountName : "",
    //                              ApplierName = applier != null ? applier.ApplierName : "",
    //                          };

    //        var results = ienums_linq.ToArray();

    //        Func<ChargeApply, object> convert = item => new
    //        {
    //            CostApplyID = item.ID,
    //            SenderName = item.SenderName,
    //            PayerAccountName = item.PayerAccountName,
    //            PayeeAccountName = item.PayeeAccountName,
    //            CurrencyDes = item.Currency.GetDescription(),
    //            Price = item.Price.ToRound1(2),
    //            ApplierName = item.ApplierName,
    //            StatusDes = item.Status.GetDescription(),
    //        };

    //        if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
    //        {
    //            Func<dynamic, object> convertAgain = item => new
    //            {

    //            };

    //            return results.Select(convert).Select(convertAgain).ToArray();
    //        }

    //        return new
    //        {
    //            total = total,
    //            Size = pageSize ?? 20,
    //            Index = pageIndex ?? 1,
    //            rows = results.Select(convert).ToArray(),
    //        };
    //    }

    //    /// <summary>
    //    /// 根据付款账户ID查询
    //    /// </summary>
    //    /// <param name="payerAccountID"></param>
    //    /// <returns></returns>
    //    public ChargeAppliesRoll SearchByPayerAccountID(string payerAccountID)
    //    {
    //        var linq = from query in this.IQueryable
    //                   where query.PayerAccountID == payerAccountID
    //                   select query;

    //        var view = new ChargeAppliesRoll(this.Reponsitory, linq);
    //        return view;
    //    }



    //    /// <summary>
    //    /// 根据状态查询
    //    /// </summary>
    //    /// <param name="stauts"></param>
    //    /// <returns></returns>
    //    public ChargeAppliesRoll SearchByStatus(ApplyStauts stauts)
    //    {
    //        var linq = from query in this.IQueryable
    //                   where query.Status == stauts
    //                   select query;

    //        var view = new ChargeAppliesRoll(this.Reponsitory, linq);
    //        return view;
    //    }

    //    /// <summary>
    //    /// 根据审批人ID查询
    //    /// </summary>
    //    /// <param name="approverID"></param>
    //    /// <returns></returns>
    //    public ChargeAppliesRoll SearchByApproverID(string approverID)
    //    {
    //        var linq = from query in this.IQueryable
    //                   where query.ApproverID == approverID
    //                   select query;

    //        var view = new ChargeAppliesRoll(this.Reponsitory, linq);
    //        return view;
    //    }

    //    /// <summary>
    //    /// 根据 CostApplyID 查询
    //    /// </summary>
    //    /// <param name="costApplyID"></param>
    //    /// <returns></returns>
    //    public ChargeAppliesRoll SearchByCostApplyID(string costApplyID)
    //    {
    //        var linq = from query in this.IQueryable
    //                   where query.ID.Contains(costApplyID)
    //                   select query;

    //        var view = new ChargeAppliesRoll(this.Reponsitory, linq);
    //        return view;
    //    }

    //    /// <summary>
    //    /// 按照 CreateDate 正序排列
    //    /// </summary>
    //    /// <returns></returns>
    //    public ChargeAppliesRoll SearchOrderByCreateDate()
    //    {
    //        var linq = this.IQueryable.OrderBy(t => t.CreateDate);

    //        var view = new ChargeAppliesRoll(this.Reponsitory, linq);
    //        return view;
    //    }

    //    /// <summary>
    //    /// 按照 CreateDate 倒序排列
    //    /// </summary>
    //    /// <returns></returns>
    //    public ChargeAppliesRoll SearchOrderByDescendingCreateDate()
    //    {
    //        var linq = this.IQueryable.OrderByDescending(t => t.CreateDate);

    //        var view = new ChargeAppliesRoll(this.Reponsitory, linq);
    //        return view;
    //    }

    //} 
    #endregion
}