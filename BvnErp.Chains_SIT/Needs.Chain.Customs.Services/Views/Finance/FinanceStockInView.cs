using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Models;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 财务入库的视图
    /// </summary>
    public class FinanceStockInView : UniqueView<FinanceStockInModel, ScCustomsReponsitory>
    {


        public FinanceStockInView()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库实体</param>
        public FinanceStockInView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<FinanceStockInModel> GetIQueryable()
        {
            var decTaxs = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxs>();
            var decHeads = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();
            var orders = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var clients = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
            var companies = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>();

            var result = from decTax in decTaxs
                         join decHead in decHeads on decTax.ID equals decHead.ID
                         join order in orders
                              on new { OrderID = decHead.OrderID, OrderIDDataStatus = (int)Enums.Status.Normal, }
                              equals new { OrderID = order.ID, OrderIDDataStatus = order.Status, }
                         join client in clients on order.ClientID equals client.ID
                         join company in companies on client.CompanyID equals company.ID
                         where decTax.Status >= (int)Enums.DecTaxStatus.Paid
                         && order.Type != (int)Enums.OrderType.Inside
                         && decHead.IsSuccess
                         && (decTax.IsPutInSto == 0 || Nullable<int>.Equals(decTax.IsPutInSto, null))
                         orderby decHead.DDate descending
                         select new FinanceStockInModel
                         {
                             ID = decHead.ID,
                             DecHeadID = decHead.ID,
                             ContrNo = decHead.ContrNo,
                             OrderID = decHead.OrderID,
                             DDate = decHead.DDate,
                             EntryId = decHead.EntryId,
                             DecTaxStatus = (Enums.DecTaxStatus)decTax.Status,
                             ClientName = company.Name
                         };
            return result;
        }
    }



    public class FinanceStockInStatisticView : UniqueView<FinanceStockInModel, ScCustomsReponsitory>
    {


        public FinanceStockInStatisticView()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库实体</param>
        public FinanceStockInStatisticView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<FinanceStockInModel> GetIQueryable()
        {
            var decTaxs = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>();

            var result = from decTax in decTaxs
                         group decTax by new { DeclarationID = decTax.DeclarationID,Currency = decTax.TradeCurr } into g_dectax
                         select new FinanceStockInModel
                         {
                             ID = g_dectax.Key.DeclarationID,
                             Currency = g_dectax.Key.Currency,
                             DecTotalAmount = g_dectax.Sum(t=>t.DeclTotal)                             
                         };
            return result;
        }
    }
}
