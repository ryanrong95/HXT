using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class ReceiptNoticesViewForReceivingListNew : QueryView<ReceiptNoticesViewForReceivingListNewModel, ScCustomsReponsitory>
    {
        public ReceiptNoticesViewForReceivingListNew()
        {
        }

        protected ReceiptNoticesViewForReceivingListNew(ScCustomsReponsitory reponsitory, IQueryable<ReceiptNoticesViewForReceivingListNewModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<ReceiptNoticesViewForReceivingListNewModel> GetIQueryable()
        {
            var receiptNotices = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ReceiptNotices>();
            var clients = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
            var companies = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>();
            var financeReceipts = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceReceipts>();
            var orderReceipts = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>();

            var lastReceipts = from orderReceipt in orderReceipts
                               where orderReceipt.FinanceReceiptID != null
                                  && orderReceipt.Status == (int)Enums.Status.Normal
                                  && orderReceipt.Type == (int)Enums.OrderReceiptType.Received
                               orderby orderReceipt.CreateDate descending
                               group orderReceipt by new { orderReceipt.FinanceReceiptID } into g
                               select new
                               {
                                   FinanceReceiptID = g.Key.FinanceReceiptID,
                                   CreateDate = g.FirstOrDefault().CreateDate,
                               };

            var iQuery = from receiptNotice in receiptNotices
                         join client in clients on receiptNotice.ClientID equals client.ID
                         join company in companies on client.CompanyID equals company.ID
                         join financeReceipt in financeReceipts on receiptNotice.ID equals financeReceipt.ID

                         join lastReceipt in lastReceipts on receiptNotice.ID equals lastReceipt.FinanceReceiptID into lastReceipts2
                         from lastReceipt in lastReceipts2.DefaultIfEmpty()

                         where receiptNotice.Status == (int)Enums.Status.Normal
                            && client.Status == (int)Enums.Status.Normal
                            && company.Status == (int)Enums.Status.Normal
                         orderby financeReceipt.ReceiptDate, receiptNotice.CreateDate descending
                         select new ReceiptNoticesViewForReceivingListNewModel
                         {
                             ReceiptNoticeID = receiptNotice.ID,
                             ClientID = receiptNotice.ClientID,
                             ClientName = company.Name,
                             ReceiptDate = financeReceipt.ReceiptDate,
                             SeqNo = financeReceipt.SeqNo,
                             FeeType = (Enums.FinanceFeeType)financeReceipt.FeeType,
                             Amount = financeReceipt.Amount,
                             ClearAmount = receiptNotice.ClearAmount,
                             FinanceVaultID = financeReceipt.FinanceVaultID,
                             FinanceAccountID = financeReceipt.FinanceAccountID,

                             LastReceiptDate = lastReceipt != null ? (DateTime?)lastReceipt.CreateDate : null,
                             ReceiptNoticeCreateDate = receiptNotice.CreateDate,
                         };

            return iQuery;
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<ReceiptNoticesViewForReceivingListNewModel> iquery = this.IQueryable.Cast<ReceiptNoticesViewForReceivingListNewModel>()
                .OrderByDescending(item => item.ReceiptDate).OrderByDescending(item => item.ReceiptNoticeCreateDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_myReceiptNotices = iquery.ToArray();

            var financeVaultIDs = ienum_myReceiptNotices.Select(t => t.FinanceVaultID);
            var financeAccountIDs = ienum_myReceiptNotices.Select(t => t.FinanceAccountID);
            var clientIDs = ienum_myReceiptNotices.Select(t => t.ClientID);

            #region 金库名称

            var financeVaults = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceVaults>();

            var linq_financeVault = from financeVault in financeVaults
                                    where financeVault.Status == (int)Enums.Status.Normal
                                       && financeVaultIDs.Contains(financeVault.ID)
                                    select new
                                    {
                                        FinanceVaultID = financeVault.ID,
                                        VaultName = financeVault.Name,
                                    };

            var ienums_financeVault = linq_financeVault.ToArray();

            #endregion

            #region 账户名称

            var financeAccounts = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceAccounts>();

            var linq_financeAccount = from financeAccount in financeAccounts
                                      where financeAccount.Status == (int)Enums.Status.Normal
                                         && financeAccountIDs.Contains(financeAccount.ID)
                                      select new
                                      {
                                          FinanceAccountID = financeAccount.ID,
                                          AccountName = financeAccount.AccountName,
                                      };

            var ienums_financeAccount = linq_financeAccount.ToArray();

            #endregion

            #region 客户类型：单抬头/双抬头

            var clientAgreements = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAgreements>();

            var linq_clientAgreement = from clientAgreement in clientAgreements
                                       where clientIDs.Contains(clientAgreement.ClientID)
                                          && clientAgreement.Status == (int)Enums.Status.Normal
                                       orderby clientAgreement.CreateDate descending
                                       group clientAgreement by new { clientAgreement.ClientID, } into g
                                       select new
                                       {
                                           ClientID = g.Key.ClientID,
                                           InvoiceTypeInt = g.FirstOrDefault().InvoiceType,
                                       };

            var ienums_clientAgreement = linq_clientAgreement.ToArray();

            #endregion

            var ienums_linq = from receiptNotice in ienum_myReceiptNotices
                              join financeVault in ienums_financeVault on receiptNotice.FinanceVaultID equals financeVault.FinanceVaultID
                              join financeAccount in ienums_financeAccount on receiptNotice.FinanceAccountID equals financeAccount.FinanceAccountID
                              join clientAgreement in ienums_clientAgreement on receiptNotice.ClientID equals clientAgreement.ClientID into ienums_clientAgreement2
                              from clientAgreement in ienums_clientAgreement2.DefaultIfEmpty()
                              select new ReceiptNoticesViewForReceivingListNewModel
                              {
                                  ReceiptNoticeID = receiptNotice.ReceiptNoticeID,
                                  ClientID = receiptNotice.ClientID,
                                  ClientName = receiptNotice.ClientName,
                                  ReceiptDate = receiptNotice.ReceiptDate,
                                  SeqNo = receiptNotice.SeqNo,
                                  FeeType = receiptNotice.FeeType,
                                  Amount = receiptNotice.Amount,
                                  ClearAmount = receiptNotice.ClearAmount,
                                  FinanceVaultID = receiptNotice.FinanceVaultID,

                                  LastReceiptDate = receiptNotice.LastReceiptDate,
                                  ReceiptNoticeCreateDate = receiptNotice.ReceiptNoticeCreateDate,

                                  VaultName = financeVault.VaultName,
                                  AccountName = financeAccount.AccountName,

                                  InvoiceTypeInt = clientAgreement != null ? (int?)clientAgreement.InvoiceTypeInt : null,
                              };

            var results = ienums_linq.ToArray();

            for (int i = 0; i < results.Length; i++)
            {
                results[i].InvoiceTypeName = "";

                if (results[i].InvoiceTypeInt != null)
                {
                    switch (results[i].InvoiceTypeInt)
                    {
                        case (int)Enums.InvoiceType.Full:
                            results[i].InvoiceTypeName = "单抬头";
                            break;
                        case (int)Enums.InvoiceType.Service:
                            results[i].InvoiceTypeName = "双抬头";
                            break;
                        default:
                            break;
                    }
                }
            }

            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                return results.Select(item =>
                {
                    object o = item;
                    return o;
                }).ToArray();
            }

            Func<ReceiptNoticesViewForReceivingListNewModel, object> convert = item => new
            {
                ID = item.ReceiptNoticeID,
                ClientID = item.ClientID, //用于传递到下一个页面
                item.SeqNo,
                VaultName = item.VaultName,
                item.AccountName,
                ClientName = item.ClientName,
                FinanceReceiptFeeType = item.FeeType.GetDescription(),
                Amount = item?.Amount.ToString("#0.00"),
                ClearAmount = item.ClearAmount.ToString("#0.00"),
                QuerenStatus = item.ClearAmount < item.Amount ? "未确认" : "已确认",
                ReceiptDate = item?.ReceiptDate.ToString("yyyy-MM-dd"),

                LastReceiptDate = item.LastReceiptDate?.ToString("yyyy-MM-dd"),
                InvoiceTypeName = item.InvoiceTypeName,
            };

            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = results.Select(convert).ToArray(),
            };
        }

        /// <summary>
        /// 根据客户名称查询
        /// </summary>
        /// <param name="clientName"></param>
        /// <returns></returns>
        public ReceiptNoticesViewForReceivingListNew SearchByClientName(string clientName)
        {
            var linq = from query in this.IQueryable
                       where query.ClientName.Contains(clientName)
                       select query;

            var view = new ReceiptNoticesViewForReceivingListNew(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据收款日期开始时间查询
        /// </summary>
        /// <param name="begin"></param>
        /// <returns></returns>
        public ReceiptNoticesViewForReceivingListNew SearchByReceiptDateBegin(DateTime begin)
        {
            var linq = from query in this.IQueryable
                       where query.ReceiptDate >= begin
                       select query;

            var view = new ReceiptNoticesViewForReceivingListNew(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据收款日期开始时间查询
        /// </summary>
        /// <param name="begin"></param>
        /// <returns></returns>
        public ReceiptNoticesViewForReceivingListNew SearchByReceiptDateEnd(DateTime end)
        {
            var linq = from query in this.IQueryable
                       where query.ReceiptDate < end
                       select query;

            var view = new ReceiptNoticesViewForReceivingListNew(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据最近核销日期开始时间查询
        /// </summary>
        /// <param name="begin"></param>
        /// <returns></returns>
        public ReceiptNoticesViewForReceivingListNew SearchByLastReceiptDateBegin(DateTime begin)
        {
            var linq = from query in this.IQueryable
                       where query.LastReceiptDate >= begin
                       select query;

            var view = new ReceiptNoticesViewForReceivingListNew(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据最近核销日期结束时间查询
        /// </summary>
        /// <param name="end"></param>
        /// <returns></returns>
        public ReceiptNoticesViewForReceivingListNew SearchByLastReceiptDateEnd(DateTime end)
        {
            var linq = from query in this.IQueryable
                       where query.LastReceiptDate < end
                       select query;

            var view = new ReceiptNoticesViewForReceivingListNew(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据是否确认状态查询
        /// </summary>
        /// <param name="querenStatus">0 - 全部, 1 - 未确认, 2 - 已确认</param>
        /// <returns></returns>
        public ReceiptNoticesViewForReceivingListNew SearchByQuerenStatus(string querenStatus)
        {
            var linq = this.IQueryable;

            if (querenStatus == "1")
            {
                linq = linq.Where(x => x.ClearAmount < x.Amount);
            }
            else if (querenStatus == "2")
            {
                linq = linq.Where(x => x.ClearAmount >= x.Amount);
            }

            var view = new ReceiptNoticesViewForReceivingListNew(this.Reponsitory, linq);
            return view;
        }

    }

    public class ReceiptNoticesViewForReceivingListNewModel
    {
        /// <summary>
        /// ReceiptNoticeID
        /// </summary>
        public string ReceiptNoticeID { get; set; }

        /// <summary>
        /// ClientID
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 收款流水号
        /// </summary>
        public string SeqNo { get; set; }

        /// <summary>
        /// 金库名称
        /// </summary>
        public string VaultName { get; set; }

        /// <summary>
        /// 账户名称
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// 财务收款类型：预收账款、资金调入、银行利息、借款、还款
        /// </summary>
        public Enums.FinanceFeeType FeeType { get; set; }

        /// <summary>
        /// 收款金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 已添加到订单收款明细的金额
        /// </summary>
        public decimal ClearAmount { get; set; }

        /// <summary>
        /// 收款日期
        /// </summary>
        public DateTime ReceiptDate { get; set; }

        /// <summary>
        /// 最近核销日期
        /// </summary>
        public DateTime? LastReceiptDate { get; set; }

        /// <summary>
        /// FinanceVaultID
        /// </summary>
        public string FinanceVaultID { get; set; }

        /// <summary>
        /// FinanceAccountID
        /// </summary>
        public string FinanceAccountID { get; set; }

        /// <summary>
        /// ReceiptNoticeCreateDate
        /// </summary>
        public DateTime ReceiptNoticeCreateDate { get; set; }

        /// <summary>
        /// InvoiceTypeInt
        /// </summary>
        public int? InvoiceTypeInt { get; set; }

        /// <summary>
        /// 客户类型：单抬头/双抬头
        /// </summary>
        public string InvoiceTypeName { get; set; }
    }

}
