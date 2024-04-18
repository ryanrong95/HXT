using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    //public class DeclarationStatisticsView : Needs.Linq.Generic.Query1Classics<Models.DeclarationStatistics, ScCustomsReponsitory>
    //{
    //    public DeclarationStatisticsView()
    //    {
    //    }

    //    internal DeclarationStatisticsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
    //    {
    //    }
    //    protected override IQueryable<Models.DeclarationStatistics> GetIQueryable(Expression<Func<Models.DeclarationStatistics, bool>> expression, params LambdaExpression[] expressions)
    //    {
    //        var ordersView = new OrdersView(this.Reponsitory).
    //            Where(t => t.OrderStatus >= Enums.OrderStatus.Declared && t.OrderStatus >= Enums.OrderStatus.Completed && t.Status == Enums.Status.Normal);
    //        var linq = from entity in ordersView
    //                   select new Models.DeclarationStatistics
    //                   {
    //                       ID = entity.Client.ServiceManager.ID,
    //                       Name = entity.Client.ServiceManager.RealName,
    //                       Currency = entity.Currency,
    //                       OrderDate = entity.CreateDate,
    //                       DeclarePrice = entity.DeclarePrice
    //                   };

    //        foreach (var predicate in expressions)
    //        {
    //            linq = linq.Where(predicate as Expression<Func<Models.DeclarationStatistics, bool>>);
    //        }

    //        return linq.Where(expression);
    //    }

    //    protected override IEnumerable<DeclarationStatistics> OnReadShips(DeclarationStatistics[] results)
    //    {
    //        return results;
    //    }
    //}

    public class DeclarationStatisticsView : UniqueView<Models.DeclarationStatistics, ScCustomsReponsitory>
    {
        public DeclarationStatisticsView()
        {

        }
        public DeclarationStatisticsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<DeclarationStatistics> GetIQueryable()
        {
            var linq = from order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                       where order.OrderStatus >= (int)Enums.OrderStatus.Declared && order.OrderStatus <= (int)Enums.OrderStatus.Completed && order.Status == (int)Enums.Status.Normal
                       join clientAdmin in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAdmins>() on order.ClientID equals clientAdmin.ClientID
                       where clientAdmin.Type == (int)Enums.ClientAdminType.ServiceManager && clientAdmin.Status == (int)Enums.Status.Normal
                       join dechead in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>() on order.ID equals dechead.OrderID
                       where dechead.IsSuccess
                       join client in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>() on order.ClientID equals client.ID
                       join Company in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>() on client.CompanyID equals Company.ID
                       join AdminTopView in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView>() on clientAdmin.AdminID equals AdminTopView.ID
                       select new Models.DeclarationStatistics
                       {
                           ID = clientAdmin.AdminID,
                           Name = AdminTopView.RealName,
                           Currency = order.Currency,
                           OrderDate = dechead.DDate.Value,
                           DeclarePrice = order.DeclarePrice,
                           OrderStatus = (OrderStatus)order.OrderStatus,
                           ClientName = Company.Name,
                           OrderID = order.ID
                       };
            return linq;
        }
    }
}
