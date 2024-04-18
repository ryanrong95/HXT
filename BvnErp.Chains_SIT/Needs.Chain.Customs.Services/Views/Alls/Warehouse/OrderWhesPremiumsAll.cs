using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Models;
using System.Linq.Expressions;

namespace Needs.Ccs.Services.Views.Alls
{
    public class OrderWhesPremiumsAll : Needs.Linq.Generic.Unique1Classics<Models.OrderWhesPremium, ScCustomsReponsitory>
    {
        public OrderWhesPremiumsAll()
        {
        }

        internal OrderWhesPremiumsAll(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<OrderWhesPremium> GetIQueryable(Expression<Func<OrderWhesPremium, bool>> expression, params LambdaExpression[] expressions)
        {
            var orders = new Origins.OrdersOrigin(this.Reponsitory);
            var linq = from fee in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderWhesPremiums>()
                       join order in orders on fee.OrderID equals order.ID
                       where fee.Status == (int)Enums.Status.Normal
                       orderby fee.PremiumsStatus
                       orderby fee.CreateDate descending
                       select new Models.OrderWhesPremium
                       {
                           ID = fee.ID,
                           OrderID = fee.OrderID,
                           ClientID = order.ClientID,
                           CreaterID = fee.CreaterID,
                           ApproverID = fee.ApproverID,
                           WarehouseType = (Enums.WarehouseType)fee.WarehouseType,
                           WarehousePremiumType = (Enums.WarehousePremiumType)fee.WhesFeeType,
                           WhsePaymentType = (Enums.WhsePaymentType)fee.PaymentType,
                           WarehousePremiumsStatus = (Enums.WarehousePremiumsStatus)fee.PremiumsStatus,
                           Count = fee.Count,
                           UnitPrice = fee.UnitPrice,
                           UnitName = fee.UnitName,
                           Currency = fee.Currency,
                           ExchangeRate = fee.ExchangeRate,
                           ApprovalPrice = fee.ApprovalPrice,
                           Status = (Enums.Status)fee.Status,
                           CreateDate = fee.CreateDate,
                           UpdateDate = fee.UpdateDate,
                           Summary = fee.Summary
                       };

            foreach (var predicate in expressions)
            {
                linq = linq.Where(predicate as Expression<Func<Models.OrderWhesPremium, bool>>);
            }

            return linq.Where(expression);
        }

        protected override IEnumerable<OrderWhesPremium> OnReadShips(OrderWhesPremium[] results)
        {
            var clientIds = results.Select(r => r.ClientID).ToArray();
            var clients = new Rolls.ClientsRoll(this.Reponsitory).Where(c => clientIds.Contains(c.ID)).ToArray();
            var admins = new AdminsTopView2(this.Reponsitory);

            return from result in results
                   join client in clients on result.ClientID equals client.ID
                   join admin in admins on result.CreaterID equals admin.OriginID
                   join approval in admins on result.ApproverID equals approval.ID into approvals
                   from approval in approvals.DefaultIfEmpty()
                   select new Models.OrderWhesPremium
                   {
                       ID = result.ID,
                       OrderID = result.OrderID,
                       ClientID = result.ClientID,
                       Client = client,
                       Creater = admin,
                       Approver = approval,
                       WarehouseType = result.WarehouseType,
                       WarehousePremiumType = result.WarehousePremiumType,
                       WarehousePremiumsStatus = result.WarehousePremiumsStatus,
                       WhsePaymentType = result.WhsePaymentType,
                       Count = result.Count,
                       UnitPrice = result.UnitPrice,
                       UnitName = result.UnitName,
                       Currency = result.Currency,
                       ExchangeRate = result.ExchangeRate,
                       ApprovalPrice = result.ApprovalPrice,
                       Status = result.Status,
                       CreateDate = result.CreateDate,
                       UpdateDate = result.UpdateDate,
                       Summary = result.Summary,
                   };
        }
    }
}
