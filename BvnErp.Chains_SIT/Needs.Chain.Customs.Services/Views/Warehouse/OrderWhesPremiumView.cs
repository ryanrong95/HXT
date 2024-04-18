using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{

    /// <summary>
    /// 库房费用
    /// </summary>
    public class OrderWhesPremiumView : UniqueView<Models.OrderWhesPremium, ScCustomsReponsitory>
    {
        public OrderWhesPremiumView()
        {
        }

        internal OrderWhesPremiumView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.OrderWhesPremium> GetIQueryable()
        {
            var ordersView = new OrdersView(this.Reponsitory);
            var adminView = new AdminsTopView2(this.Reponsitory);

            return from fee in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderWhesPremiums>()
                   where fee.Status == (int)Enums.Status.Normal
                   join orders in ordersView on fee.OrderID equals orders.ID
                   join admin in adminView on fee.CreaterID equals admin.OriginID
                   join approval in adminView on fee.ApproverID equals approval.ID into approvals
                   from approval in approvals.DefaultIfEmpty()
                   orderby fee.PremiumsStatus
                   select new Models.OrderWhesPremium
                   {
                       ID = fee.ID,
                       OrderID = fee.OrderID,
                       Creater = admin,
                       Approver = approval,
                       WarehouseType = (Enums.WarehouseType)fee.WarehouseType,
                       WarehousePremiumType = (Enums.WarehousePremiumType)fee.WhesFeeType,
                       WarehousePremiumsStatus = (Enums.WarehousePremiumsStatus)fee.PremiumsStatus,
                       WhsePaymentType = (Enums.WhsePaymentType)fee.PaymentType,
                       Count = fee.Count,
                       UnitPrice = fee.UnitPrice,
                       UnitName = fee.UnitName,
                       Currency = fee.Currency,
                       ExchangeRate = fee.ExchangeRate,
                       ApprovalPrice = fee.ApprovalPrice,
                       Status = (Enums.Status)fee.Status,
                       CreateDate = fee.CreateDate,
                       UpdateDate = fee.UpdateDate,
                       Summary = fee.Summary,
                       //客户
                       Client = orders.Client,
                   };
        }
    }
}
