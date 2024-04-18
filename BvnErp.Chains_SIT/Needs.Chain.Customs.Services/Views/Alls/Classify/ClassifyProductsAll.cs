using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.Alls
{
    public class ClassifyProductsAll : Needs.Linq.Generic.Unique2Classics<Models.ClassifyProduct, ScCustomsReponsitory>
    {
        public ClassifyProductsAll()
        {
        }
        internal ClassifyProductsAll(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<ClassifyProduct> GetIQueryable(
            Expression<Func<ClassifyProduct, bool>> expression,
            LambdaExpression[] orderByAscDateTimeExpressions,
            LambdaExpression[] orderByDescDateTimeExpressions,
            string currentAdminID,
            bool isShowLocked = true,
            params LambdaExpression[] expressions)
        {
            var orders = new Origins.OrdersOrigin(this.Reponsitory).Where(item => item.OrderStatus >= Enums.OrderStatus.Confirmed && item.OrderStatus != Enums.OrderStatus.Returned && item.OrderStatus != Enums.OrderStatus.Canceled);
            var orderItems = new Origins.OrderItemsOrigin(this.Reponsitory);
            var categoriesView = new Origins.OrderItemCategoriesOrigin(this.Reponsitory);
            var linq = from entity in orderItems
                       join order in orders on entity.OrderID equals order.ID
                       //orderby entity.ID
                       join category in categoriesView on entity.ID equals category.OrderItemID into categories
                       from category in categories.DefaultIfEmpty()
                       select new Models.ClassifyProduct
                       {
                           ID = entity.ID,
                           OrderID = entity.OrderID,
                           OrderType = order.Type,
                           ClientID = order.ClientID,
                           //ProductID = entity.ProductID,
                           //Product = entity.Product,
                           Name = entity.Name,
                           Model = entity.Model,
                           Manufacturer = entity.Manufacturer,
                           Batch = entity.Batch,
                           Origin = entity.Origin,
                           Quantity = entity.Quantity,
                           DeclaredQuantity = entity.DeclaredQuantity,
                           Unit = entity.Unit,
                           UnitPrice = entity.UnitPrice,
                           TotalPrice = entity.TotalPrice,
                           Currency = order.Currency,
                           GrossWeight = entity.GrossWeight,
                           ClassifyStatus = entity.ClassifyStatus,
                           Status = entity.Status,
                           CreateDate = entity.CreateDate,
                           UpdateDate = entity.UpdateDate,
                           Summary = entity.Summary,
                           ClassifyFirstOperator = category.ClassifyFirstOperatorID,
                           ClassifySecondOperator = category.ClassifySecondOperatorID,

                           Category = category,
                           OrderStatus = order.OrderStatus,
                       };

            foreach (var predicate in expressions)
            {
                linq = linq.Where(predicate as Expression<Func<Models.ClassifyProduct, bool>>);
            }

            if (orderByAscDateTimeExpressions != null && orderByAscDateTimeExpressions.Any())
            {
                foreach (var orderByAscDateTimeExpression in orderByAscDateTimeExpressions)
                {
                    linq = linq.OrderBy(orderByAscDateTimeExpression as Expression<Func<Models.ClassifyProduct, DateTime>>);
                }
            }

            if (orderByDescDateTimeExpressions != null && orderByDescDateTimeExpressions.Any())
            {
                foreach (var orderByDescDateTimeExpression in orderByDescDateTimeExpressions)
                {
                    linq = linq.OrderByDescending(orderByDescDateTimeExpression as Expression<Func<Models.ClassifyProduct, DateTime>>);
                }
            }

            linq = linq.Where(expression);

            if (!isShowLocked)
            {
                //不显示锁定的
                var productClassifyLocks = new Origins.ProductClassifyLockOrigin(this.Reponsitory);
                string[] lockedOrderItemIDs = productClassifyLocks
                    .Where(t => t.AdminID != currentAdminID)
                    .Select(t => t.ID).ToArray();

                linq = from p in linq
                       where !(lockedOrderItemIDs).Contains(p.ID)
                       select p;
            }

            return linq;
        }

        protected override IEnumerable<ClassifyProduct> OnReadShips(ClassifyProduct[] results)
        {
            var ids = results.Select(r => r.ID).ToArray();
            var taxesView = new Origins.OrderItemTaxsOrigin(this.Reponsitory);
            var taxArr = (from tax in taxesView
                          where ids.Contains(tax.OrderItemID)
                          select tax).ToArray();

            var premiumsView = new Origins.OrderPremiumsOrigin(this.Reponsitory);
            var premiumArr = (from premium in premiumsView
                              where ids.Contains(premium.OrderItemID) && premium.Type == Enums.OrderPremiumType.InspectionFee
                              select premium).ToArray();

            var clientsView = new Rolls.ClientsRoll(this.Reponsitory);
            var adminsView = new AdminsTopView(this.Reponsitory);
            var productLocksView = from productLock in new Origins.ProductClassifyLockOrigin(this.Reponsitory)
                                   join admin in adminsView on productLock.AdminID equals admin.ID
                                   select new
                                   {
                                       productLock.ID,
                                       productLock.IsLocked,
                                       productLock.LockDate,
                                       Locker = admin
                                   };

            return from result in results
                   join importTax in taxArr.Where(t => t.Type == Enums.CustomsRateType.ImportTax) on result.ID equals importTax.OrderItemID into importTaxes
                   from importTax in importTaxes.DefaultIfEmpty()
                   join addedValueTax in taxArr.Where(t => t.Type == Enums.CustomsRateType.AddedValueTax) on result.ID equals addedValueTax.OrderItemID into addedValueTaxes
                   from addedValueTax in addedValueTaxes.DefaultIfEmpty()
                   join exciseTax in taxArr.Where(t => t.Type == Enums.CustomsRateType.ConsumeTax) on result.ID equals exciseTax.OrderItemID into exciseTaxes
                   from exciseTax in exciseTaxes.DefaultIfEmpty()
                   join inspFee in premiumArr on result.ID equals inspFee.OrderItemID into inspFees
                   from inspFee in inspFees.DefaultIfEmpty()
                   join client in clientsView on result.ClientID equals client.ID
                   join productLock in productLocksView on result.ID equals productLock.ID into productLocks
                   from productLock in productLocks.DefaultIfEmpty()

                   join firstOperator in new AdminsTopView(this.Reponsitory) on result.ClassifyFirstOperator equals firstOperator.ID into firstOperatorAdminsTopView
                   from firstOperator in firstOperatorAdminsTopView.DefaultIfEmpty()
                   join secondOperator in new AdminsTopView(this.Reponsitory) on result.ClassifySecondOperator equals secondOperator.ID into secondOperatorAdminsTopView
                   from secondOperator in secondOperatorAdminsTopView.DefaultIfEmpty()
                   select new Models.ClassifyProduct
                   {
                       ID = result.ID,
                       OrderID = result.OrderID,
                       OrderType = result.OrderType,
                       ClientID = result.ClientID,
                       Client = client,
                       //ProductID = result.ProductID,
                       //Product = result.Product,
                       Name = result.Name,
                       Model = result.Model,
                       Manufacturer = result.Manufacturer,
                       Batch = result.Batch,
                       Origin = result.Origin,
                       Quantity = result.Quantity,
                       DeclaredQuantity = result.DeclaredQuantity,
                       Unit = result.Unit,
                       UnitPrice = result.UnitPrice,
                       TotalPrice = result.TotalPrice,
                       Currency = result.Currency,
                       GrossWeight = result.GrossWeight,
                       ClassifyStatus = result.ClassifyStatus,
                       Status = result.Status,
                       CreateDate = result.CreateDate,
                       UpdateDate = result.UpdateDate,
                       Summary = result.Summary,
                       Category = result.Category,
                       ImportTax = importTax,
                       AddedValueTax = addedValueTax,
                       ExciseTax = exciseTax,
                       InspectionFee = inspFee == null ? null : (decimal?)inspFee.UnitPrice * inspFee.Count * inspFee.Rate,
                       IsLocked = productLock == null ? false : productLock.IsLocked,
                       Locker = productLock == null ? null : productLock.Locker,
                       LockDate = productLock == null ? null : (DateTime?)productLock.LockDate,
                       OrderStatus = result.OrderStatus,

                       ClassifyFirstOperator = result.ClassifyFirstOperator ?? string.Empty,
                       ClassifyFirstOperatorName = firstOperator != null ? firstOperator.RealName : string.Empty,
                       ClassifySecondOperator = result.ClassifySecondOperator ?? string.Empty,
                       ClassifySecondOperatorName = secondOperator != null ? secondOperator.RealName : string.Empty,
                   };
        }
    }
}
