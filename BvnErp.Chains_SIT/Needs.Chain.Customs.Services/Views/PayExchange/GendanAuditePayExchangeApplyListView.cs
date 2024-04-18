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
    public class GendanAuditePayExchangeApplyListView : QueryView<GendanAuditePayExchangeApplyListViewModel, ScCustomsReponsitory>
    {
        public GendanAuditePayExchangeApplyListView()
        {
        }

        protected GendanAuditePayExchangeApplyListView(ScCustomsReponsitory reponsitory, IQueryable<GendanAuditePayExchangeApplyListViewModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<GendanAuditePayExchangeApplyListViewModel> GetIQueryable()
        {
            var payExchangeApplies = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>();

            var clients = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();

            var iQuery = from payExchangeApply in payExchangeApplies
                         join oneApply in payExchangeApplies.Where(t => t.FatherID != null) on payExchangeApply.ID equals oneApply.FatherID into one_apply
                         from one in one_apply.DefaultIfEmpty()
                         join client in clients on payExchangeApply.ClientID equals client.ID
                         where payExchangeApply.Status == (int)Enums.Status.Normal
                            && client.Status == (int)Enums.Status.Normal
                            && one.ID == null
                         select new GendanAuditePayExchangeApplyListViewModel
                         {
                             PayExchangeApplyID = payExchangeApply.ID,
                             CreateDate = payExchangeApply.CreateDate,
                             ClientID = payExchangeApply.ClientID,
                             ClientCode = client.ClientCode,
                             SupplierName = payExchangeApply.SupplierName,
                             SupplierEnglishName = payExchangeApply.SupplierEnglishName,
                             BankName = payExchangeApply.BankName,
                             BankAccount = payExchangeApply.BankAccount,
                             PayExchangeApplyStatus = (Enums.PayExchangeApplyStatus)payExchangeApply.PayExchangeApplyStatus,
                             Currency = payExchangeApply.Currency,
                             ExchangeRate = payExchangeApply.ExchangeRate,
                             FatherID = payExchangeApply.FatherID
                         };
            return iQuery;
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<GendanAuditePayExchangeApplyListViewModel> iquery = this.IQueryable.Cast<GendanAuditePayExchangeApplyListViewModel>().OrderByDescending(item => item.CreateDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_payExchangeApplies = iquery.ToArray();

            var payExchangeApplyIDs = ienum_payExchangeApplies.Select(t => t.PayExchangeApplyID).ToArray();

            #region 付汇申请总金额

            var payExchangeApplyItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>();

            var linq_amountSum = from payExchangeApplyItem in payExchangeApplyItems
                                 where payExchangeApplyIDs.Contains(payExchangeApplyItem.PayExchangeApplyID)
                                    && payExchangeApplyItem.Status == (int)Enums.Status.Normal
                                 group payExchangeApplyItem by new { payExchangeApplyItem.PayExchangeApplyID, } into g
                                 select new
                                 {
                                     PayExchangeApplyID = g.Key.PayExchangeApplyID,
                                     TotalAmount = g.Sum(t => t.Amount),
                                 };

            var ienums_amountSum = linq_amountSum.ToArray();

            #endregion

            var ienums_linq = from payExchangeApply in ienum_payExchangeApplies
                              join amountSum in ienums_amountSum on payExchangeApply.PayExchangeApplyID equals amountSum.PayExchangeApplyID into ienums_amountSum2
                              from amountSum in ienums_amountSum2.DefaultIfEmpty()
                              select new GendanAuditePayExchangeApplyListViewModel
                              {
                                  PayExchangeApplyID = payExchangeApply.PayExchangeApplyID,
                                  CreateDate = payExchangeApply.CreateDate,
                                  ClientID = payExchangeApply.ClientID,
                                  ClientCode = payExchangeApply.ClientCode,
                                  SupplierName = payExchangeApply.SupplierName,
                                  SupplierEnglishName = payExchangeApply.SupplierEnglishName,
                                  BankName = payExchangeApply.BankName,
                                  BankAccount = payExchangeApply.BankAccount,
                                  PayExchangeApplyStatus = payExchangeApply.PayExchangeApplyStatus,
                                  Currency = payExchangeApply.Currency,
                                  ExchangeRate = payExchangeApply.ExchangeRate,
                                  TotalAmount = amountSum != null ? (decimal?)amountSum.TotalAmount : null,
                                  FatherID = payExchangeApply.FatherID
                              };

            var results = ienums_linq;

            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                return results.Select(item =>
                {
                    object o = item;
                    return o;
                }).ToArray();
            }

            Func<GendanAuditePayExchangeApplyListViewModel, object> convert = item => new
            {
                ID = item.PayExchangeApplyID,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                ClientCode = item.ClientCode,
                SupplierName = item.SupplierName,
                SupplierEnglishName = item.SupplierEnglishName,
                BankName = item.BankName,
                BankAccount = item.BankAccount,
                Status = item.PayExchangeApplyStatus.GetDescription(),
                Currency = item.Currency,
                TotalAmount = item.TotalAmount,
                ExchangeRate = item.ExchangeRate,
                FatherID = item.FatherID != null ? "Ⅱ" : "Ⅰ",
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
        /// 根据客户编号查询
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public GendanAuditePayExchangeApplyListView SearchByClientCode(string clientCode)
        {
            var linq = from query in this.IQueryable
                       where query.ClientCode.Contains(clientCode)
                       select query;

            var view = new GendanAuditePayExchangeApplyListView(this.Reponsitory, linq);
            return view;
        }


        /// <summary>
        ///根据申请ID查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public GendanAuditePayExchangeApplyListView SearchByID(string id)
        {
            var linq = from query in this.IQueryable
                       where query.PayExchangeApplyID.Contains(id)
                       select query;

            var view = new GendanAuditePayExchangeApplyListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据申请时间开始时间查询
        /// </summary>
        /// <param name="begin"></param>
        /// <returns></returns>
        public GendanAuditePayExchangeApplyListView SearchByStartDate(DateTime begin)
        {
            var linq = from query in this.IQueryable
                       where query.CreateDate >= begin
                       select query;

            var view = new GendanAuditePayExchangeApplyListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据申请时间结束时间查询
        /// </summary>
        /// <param name="end"></param>
        /// <returns></returns>
        public GendanAuditePayExchangeApplyListView SearchByEndDate(DateTime end)
        {
            var linq = from query in this.IQueryable
                       where query.CreateDate < end
                       select query;

            var view = new GendanAuditePayExchangeApplyListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据某个跟单员自己的客户查询
        /// </summary>
        /// <param name="adminID"></param>
        /// <returns></returns>
        public GendanAuditePayExchangeApplyListView SearchByClientAdmin(string adminID)
        {
            var clientIds = new ClientAdminsView().Where(t => t.Admin.ID == adminID).Select(t => t.ClientID).ToList();
            var linq = from query in this.IQueryable
                       where clientIds.Contains(query.ClientID)
                       select query;

            var view = new GendanAuditePayExchangeApplyListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据付汇申请状态查询 Equal
        /// </summary>
        /// <param name="payExchangeApplyStatus"></param>
        /// <returns></returns>
        public GendanAuditePayExchangeApplyListView SearchByPayExchangeApplyStatus_Equal(Enums.PayExchangeApplyStatus payExchangeApplyStatus)
        {
            var linq = from query in this.IQueryable
                       where query.PayExchangeApplyStatus == payExchangeApplyStatus
                       select query;

            var view = new GendanAuditePayExchangeApplyListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据付汇申请状态查询 LargerEqual
        /// </summary>
        /// <param name="payExchangeApplyStatus"></param>
        /// <returns></returns>
        public GendanAuditePayExchangeApplyListView SearchByPayExchangeApplyStatus_LargerEqual(Enums.PayExchangeApplyStatus payExchangeApplyStatus)
        {
            var linq = from query in this.IQueryable
                       where query.PayExchangeApplyStatus >= payExchangeApplyStatus
                       select query;

            var view = new GendanAuditePayExchangeApplyListView(this.Reponsitory, linq);
            return view;
        }

        // <summary>
        /// 根据订单号查询 付汇申请
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public GendanAuditePayExchangeApplyListView SearchByOrderID(string orderid)
        {
            var linq = from query in this.IQueryable
                       join item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>() on query.PayExchangeApplyID equals item.PayExchangeApplyID
                       where item.OrderID == orderid
                       select query;

            var view = new GendanAuditePayExchangeApplyListView(this.Reponsitory, linq);
            return view;
        }

    }

    public class GendanAuditePayExchangeApplyListViewModel
    {
        /// <summary>
        /// PayExchangeApplyID
        /// </summary>
        public string PayExchangeApplyID { get; set; }

        /// <summary>
        /// PayExchangeApply 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// ClientID
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 客户编号/入仓号
        /// </summary>
        public string ClientCode { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string SupplierName { get; set; }

        /// <summary>
        /// 供应商英文名称
        /// </summary>
        public string SupplierEnglishName { get; set; }

        /// <summary>
        /// 银行名称
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// 银行账户
        /// </summary>
        public string BankAccount { get; set; }

        /// <summary>
        /// 付汇申请状态
        /// </summary>
        public Enums.PayExchangeApplyStatus PayExchangeApplyStatus { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 付汇申请总金额
        /// </summary>
        public decimal? TotalAmount { get; set; }

        /// <summary>
        /// 判断是否拆分
        /// </summary>
        public string FatherID { get; set; }

        /// <summary>
        /// 付汇申请汇率
        /// </summary>
        public decimal ExchangeRate { get; set; }
    }

}
