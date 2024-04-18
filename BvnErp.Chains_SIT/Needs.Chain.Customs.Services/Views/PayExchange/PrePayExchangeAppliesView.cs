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
    public class PrePayExchangeAppliesView : QueryView<PrePayExchangeAppliesViewModel, ScCustomsReponsitory>
    {
        public PrePayExchangeAppliesView()
        {
        }

        protected PrePayExchangeAppliesView(ScCustomsReponsitory reponsitory, IQueryable<PrePayExchangeAppliesViewModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<PrePayExchangeAppliesViewModel> GetIQueryable()
        {
            var prePaymentApplyItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PrePayExchangeApplies>();
            var companies = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>();
            var clients = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
            var payExchangeApplies = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>();

            var iQuery = from prePaymentApplyItem in prePaymentApplyItems
                         join payExchangeApply in payExchangeApplies on prePaymentApplyItem.PayExchangeApplyID equals payExchangeApply.ID
                         join client in clients on prePaymentApplyItem.ClientID equals client.ID
                         join company in companies on client.CompanyID equals company.ID
                         where prePaymentApplyItem.Status == (int)Enums.Status.Normal
                            && payExchangeApply.Status == (int)Enums.Status.Normal
                            && prePaymentApplyItem.Amount != 0
                         select new PrePayExchangeAppliesViewModel
                         {

                             ID = prePaymentApplyItem.ID,
                             PayExchangeApplyID = prePaymentApplyItem.PayExchangeApplyID,
                             CreateDate = prePaymentApplyItem.CreateDate,
                             ClientID = prePaymentApplyItem.ClientID,
                             ClientName = company.Name,
                             SupplierName = payExchangeApply.SupplierName,
                             SupplierEnglishName = payExchangeApply.SupplierEnglishName,
                             PayExchangeApplyStatus = (Enums.PayExchangeApplyStatus)prePaymentApplyItem.PayExchangeApplyStatus,
                             TotalAmount = prePaymentApplyItem.Amount,
                             Currency = payExchangeApply.Currency,
                             ExchangeRate = prePaymentApplyItem.ExchangeRate,
                             OrderID = prePaymentApplyItem.OrderID,
                         };
            return iQuery;
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<PrePayExchangeAppliesViewModel> iquery = this.IQueryable.Cast<PrePayExchangeAppliesViewModel>().OrderByDescending(item => item.CreateDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_prepaymentApplyRecord = iquery.ToArray();

            var ienum_prepaymentApplyRecords = ienum_prepaymentApplyRecord.Select(t => t.ID).ToArray();

            var ienums_linq = from Records in ienum_prepaymentApplyRecord
                              select new PrePayExchangeAppliesViewModel
                              {
                                  ID = Records.ID,
                                  PayExchangeApplyID = Records.PayExchangeApplyID,
                                  CreateDate = Records.CreateDate,
                                  ClientID = Records.ClientID,
                                  ClientName = Records.ClientName,
                                  SupplierName = Records.SupplierName,
                                  SupplierEnglishName = Records.SupplierEnglishName,
                                  PayExchangeApplyStatus = Records.PayExchangeApplyStatus,
                                  TotalAmount = Records.TotalAmount,
                                  Currency = Records.Currency,
                                  ExchangeRate = Records.ExchangeRate,
                                  OrderID = Records.OrderID,
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

            Func<PrePayExchangeAppliesViewModel, object> convert = item => new
            {
                ID = item.ID,
                PayExchangeApplyID = item.PayExchangeApplyID,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                ClientID = item.ClientID,
                ClientName = item.ClientName,
                SupplierName = item.SupplierName,
                SupplierEnglishName = item.SupplierEnglishName,
                PayExchangeApplyStatus = item.PayExchangeApplyStatus,
                TotalAmount = item.TotalAmount,
                Currency = item.Currency,
                ExchangeRate = item.ExchangeRate,
                OrderID = item.OrderID,
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
        /// 根据申请编号查询
        /// </summary>
        /// <param name="PayExchangeApplyID"></param>
        /// <returns></returns>
        public PrePayExchangeAppliesView SearchByPayExchangeApplyID(string PayExchangeApplyID)
        {
            var linq = from query in this.IQueryable
                       where query.ID == PayExchangeApplyID
                       select query;

            var view = new PrePayExchangeAppliesView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据客户名称查询
        /// </summary>
        /// <param name="ClientName"></param>
        /// <returns></returns>
        public PrePayExchangeAppliesView SearchByClientName(string ClientName)
        {
            var linq = from query in this.IQueryable
                       where query.ClientName.Contains(ClientName)
                       select query;

            var view = new PrePayExchangeAppliesView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据申请时间开始时间查询
        /// </summary>
        /// <param name="begin"></param>
        /// <returns></returns>
        public PrePayExchangeAppliesView SearchByStartDate(DateTime begin)
        {
            var linq = from query in this.IQueryable
                       where query.CreateDate >= begin
                       select query;

            var view = new PrePayExchangeAppliesView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据申请时间结束时间查询
        /// </summary>
        /// <param name="end"></param>
        /// <returns></returns>
        public PrePayExchangeAppliesView SearchByEndDate(DateTime end)
        {
            var linq = from query in this.IQueryable
                       where query.CreateDate < end
                       select query;

            var view = new PrePayExchangeAppliesView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据某个跟单员自己的客户查询
        /// </summary>
        /// <param name="adminID"></param>
        /// <returns></returns>
        public PrePayExchangeAppliesView SearchByClientAdmin(string adminID)
        {
            var clientIds = new ClientAdminsView().Where(t => t.Admin.ID == adminID).Select(t => t.ClientID).ToList();
            var linq = from query in this.IQueryable
                       where clientIds.Contains(query.ClientID)
                       select query;

            var view = new PrePayExchangeAppliesView(this.Reponsitory, linq);
            return view;
        }

    }

    public class PrePayExchangeAppliesViewModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }
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
        /// 客户名称
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// 供应商英文名称
        /// </summary>
        public string SupplierEnglishName { get; set; }

        /// <summary>
        /// 付汇申请状态
        /// </summary>
        public Enums.PayExchangeApplyStatus PayExchangeApplyStatus { get; set; }

        /// <summary>
        /// 付汇汇率
        /// </summary>
        public decimal ExchangeRate { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        ///预付汇申请金额
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string SupplierName { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderID { get; set; }
    }

}

