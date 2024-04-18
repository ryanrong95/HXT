using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// FinanceVoucher 视图
    /// </summary>
    public class FinanceVoucherView : UniqueView<Models.FinanceVoucher, ScCustomsReponsitory>
    {
        public FinanceVoucherView()
        {

        }

        public FinanceVoucherView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<FinanceVoucher> GetIQueryable()
        {
            var financeVouchers = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceVouchers>();

            var results = from financeVoucher in financeVouchers
                          select new Models.FinanceVoucher
                          {
                              ID = financeVoucher.ID,
                              Amount = financeVoucher.Amount,
                              OrderID = financeVoucher.OrderID,
                              UseAdminID = financeVoucher.UseAdminID,
                              UseTime = financeVoucher.UseTime,
                              Status = (Enums.Status)financeVoucher.Status,
                              CreateDate = financeVoucher.CreateDate,
                              UpdateDate = financeVoucher.UpdateDate,
                              Summary = financeVoucher.Summary,
                          };

            return results;
        }

        /// <summary>
        /// 根据主订单号查询使用的抵用券信息
        /// </summary>
        /// <param name="mainOrderID"></param>
        /// <returns></returns>
        public List<Models.FinanceVoucher> GetFinanceVoucherByMainOrderID(string mainOrderID)
        {
            var financeVouchers = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceVouchers>();
            var orders = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();

            var results = from financeVoucher in financeVouchers
                          join order in orders on financeVoucher.OrderID equals order.ID
                          where financeVoucher.Status == (int)Enums.Status.Normal
                             && order.Status == (int)Enums.Status.Normal
                             && order.MainOrderId == mainOrderID
                          orderby financeVoucher.OrderID
                          select new Models.FinanceVoucher
                          {
                              ID = financeVoucher.ID,
                              Amount = financeVoucher.Amount,
                              OrderID = financeVoucher.OrderID,
                              UseAdminID = financeVoucher.UseAdminID,
                              UseTime = financeVoucher.UseTime,
                              Status = (Enums.Status)financeVoucher.Status,
                              CreateDate = financeVoucher.CreateDate,
                              UpdateDate = financeVoucher.UpdateDate,
                              Summary = financeVoucher.Summary,
                          };

            return results.ToList();
        }

    }
}
