using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// OrderReceipts 行转列
    /// </summary>
    public class SwapDetailView : UniqueView<Models.SwapDetail, ScCustomsReponsitory>
    {
        public SwapDetailView()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库实体</param>
        public SwapDetailView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.SwapDetail> GetIQueryable()
        {
            var clientsView = new ClientsView(this.Reponsitory);

            var result = from swapItems in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapNoticeItems>()
                         join swapNotice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapNotices>() on swapItems.SwapNoticeID equals swapNotice.ID
                         join decheads in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>() on swapItems.DecHeadID equals decheads.ID
                         join orders in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on decheads.OrderID equals orders.ID
                         join clients in clientsView on orders.ClientID equals clients.ID
                         where swapNotice.Status == (int)SwapStatus.Audited
                         select new Models.SwapDetail
                         {
                             ID = swapItems.ID,
                             SwapNoticeID = swapNotice.ID,
                             ContrNo = decheads.ContrNo,
                             OrderID = orders.ID,
                             ClientName = clients.Company.Name,
                             Currency = swapNotice.Currency,
                             RealExchangeRate = orders.RealExchangeRate,
                             SwapAmount = swapItems.Amount,
                             SwapExchangeRate = swapNotice.ExchangeRate,
                             DDate = decheads.DDate,
                         };


            return result;

        }
    }
}
