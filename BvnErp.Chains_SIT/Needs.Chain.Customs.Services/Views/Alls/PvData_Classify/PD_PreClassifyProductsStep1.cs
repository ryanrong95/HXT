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
    public class PD_PreClassifyProductsStep1 : Needs.Linq.Generic.Unique2Classics<Models.PD_PreClassifyProduct, ScCustomsReponsitory>
    {
        public PD_PreClassifyProductsStep1()
        {
        }

        internal PD_PreClassifyProductsStep1(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.PD_PreClassifyProduct> GetIQueryable(
            Expression<Func<Models.PD_PreClassifyProduct, bool>> expression,
            LambdaExpression[] orderByAscDateTimeExpressions,
            LambdaExpression[] orderByDescDateTimeExpressions,
            string currentAdminID,
            bool isShowLocked = true,
            params LambdaExpression[] expressions)
        {
            var preProductCategories = new Origins.PreProductCategoriesOrigin(this.Reponsitory);
            var preProductsView = new Origins.PreProductsOrigin(this.Reponsitory);
            var linq = from entity in preProductCategories
                       join preProduct in preProductsView on entity.PreProductID equals preProduct.ID
                       select new Models.PD_PreClassifyProduct
                       {
                           ID = entity.ID,
                           PreProductID = entity.PreProductID,
                           PreProduct = preProduct,
                           Model = entity.Model,
                           Manufacturer = entity.Manufacturer,
                           ProductName = entity.ProductName,
                           HSCode = entity.HSCode,
                           TariffRate = entity.TariffRate,
                           AddedValueRate = entity.AddedValueRate,
                           ExciseTaxRate = entity.ExciseTaxRate,
                           TaxCode = entity.TaxCode,
                           TaxName = entity.TaxName,
                           Type = entity.Type,
                           InspectionFee = entity.InspectionFee,
                           Unit1 = entity.Unit1,
                           Unit2 = entity.Unit2,
                           CIQCode = entity.CIQCode,
                           Elements = entity.Elements,
                           ClassifyStatus = entity.ClassifyStatus,
                           Status = entity.Status,
                           CreateDate = entity.CreateDate,
                           UpdateDate = entity.UpdateDate,
                           ClassifyFirstOperator = entity.ClassifyFirstOperator,
                           ClassifySecondOperator = entity.ClassifySecondOperator,
                           Summary = entity.Summary,
                           Source = preProduct.Source,
                       };

            foreach (var predicate in expressions)
            {
                linq = linq.Where(predicate as Expression<Func<PD_PreClassifyProduct, bool>>);
            }

            if (orderByAscDateTimeExpressions != null && orderByAscDateTimeExpressions.Any())
            {
                foreach (var orderByAscDateTimeExpression in orderByAscDateTimeExpressions)
                {
                    linq = linq.OrderBy(orderByAscDateTimeExpression as Expression<Func<Models.PD_PreClassifyProduct, DateTime>>);
                }
            }

            if (orderByDescDateTimeExpressions != null && orderByDescDateTimeExpressions.Any())
            {
                foreach (var orderByDescDateTimeExpression in orderByDescDateTimeExpressions)
                {
                    linq = linq.OrderByDescending(orderByDescDateTimeExpression as Expression<Func<Models.PD_PreClassifyProduct, DateTime>>);
                }
            }

            linq = linq.Where(expression);

            if (!isShowLocked)
            {
                var locksView = new Origins.Locks_ClassifyOrigin(this.Reponsitory).Where(t => t.LockerID != currentAdminID);
                linq = from entity in linq
                       join locker in locksView on entity.ID equals locker.MainID into lockers
                       from locker in lockers.DefaultIfEmpty()
                       where locker == null
                       select entity;
            }

            return linq;
        }

        protected override IEnumerable<Models.PD_PreClassifyProduct> OnReadShips(Models.PD_PreClassifyProduct[] results)
        {
            var ids = results.Select(item => item.ID).ToArray();
            var ienum_productLocks = new Locks_ClassifyAll(this.Reponsitory).Where(item => ids.Contains(item.MainID)).ToArray();

            var clientIDs = results.Select(item => item.PreProduct.ClientID).Distinct().ToArray();
            var ienum_clients = new Rolls.ClientsRoll(this.Reponsitory).Where(item => clientIDs.Contains(item.ID)).ToArray();

            return from result in results
                   join client in ienum_clients on result.PreProduct.ClientID equals client.ID
                   join productLock in ienum_productLocks on result.ID equals productLock.MainID into productLocks
                   from productLock in productLocks.DefaultIfEmpty()
                   select new Models.PD_PreClassifyProduct
                   {
                       ID = result.ID,
                       ProductName = result.ProductName,
                       Model = result.Model,
                       Manufacturer = result.Manufacturer,
                       ClassifyStatus = result.ClassifyStatus,
                       IsLocked = productLock == null ? false : true,
                       Locker = productLock == null ? null : productLock.Locker,
                       LockDate = productLock == null ? null : (DateTime?)productLock.LockDate,

                       HSCode = result.HSCode,
                       TariffRate = result.TariffRate,
                       AddedValueRate = result.AddedValueRate,
                       ExciseTaxRate = result.ExciseTaxRate,
                       TaxCode = result.TaxCode,
                       TaxName = result.TaxName,
                       Type = result.Type,
                       InspectionFee = result.InspectionFee,
                       Unit1 = result.Unit1,
                       Unit2 = result.Unit2,
                       CIQCode = result.CIQCode,
                       Elements = result.Elements,
                       Status = result.Status,
                       CreateDate = result.CreateDate,
                       UpdateDate = result.UpdateDate,
                       Summary = result.Summary,

                       PreProductID = result.PreProductID,
                       PreProduct = new Models.PreProduct
                       {
                           ID = result.PreProduct.ID,
                           ClientID = result.PreProduct.ClientID,
                           Client = client,
                           Price = result.PreProduct.Price,
                           Qty = result.PreProduct.Qty,
                           Currency = result.PreProduct.Currency,
                           AreaOfProduction = result.PreProduct.AreaOfProduction,
                           CompanyType = result.PreProduct.CompanyType,
                           DueDate = result.PreProduct.DueDate,
                           UseType = result.PreProduct.UseType,
                           ProductUnionCode = result.PreProduct.ProductUnionCode
                       },

                       ClassifyFirstOperator = result.ClassifyFirstOperator,
                       ClassifySecondOperator = result.ClassifySecondOperator,

                       ProductUnionCode = result.PreProduct.ProductUnionCode,
                       IsPushStatusWarning = result.IsPushStatusWarning,
                       Source = result.Source,
                   };
        }
    }
}
