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
    /// <summary>
    /// 预归类产品视图
    /// </summary>
    public class PD_PreClassifyProductsAll : Needs.Linq.Generic.Unique2Classics<Models.PD_PreClassifyProduct, ScCustomsReponsitory>
    {
        public PD_PreClassifyProductsAll()
        {
        }

        internal PD_PreClassifyProductsAll(ScCustomsReponsitory reponsitory) : base(reponsitory)
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
            var preProductsView = new Rolls.PreProductsRoll(this.Reponsitory);
            var adminsView = new AdminsTopView(this.Reponsitory);
            var linq = from entity in preProductCategories
                       join preProduct in preProductsView on entity.PreProductID equals preProduct.ID
                       join firstOperator in new AdminsTopView(this.Reponsitory) on entity.ClassifyFirstOperator equals firstOperator.ID into firstOperatorAdminsTopView
                       from firstOperator in firstOperatorAdminsTopView.DefaultIfEmpty()
                       join secondOperator in new AdminsTopView(this.Reponsitory) on entity.ClassifySecondOperator equals secondOperator.ID into secondOperatorAdminsTopView
                       from secondOperator in secondOperatorAdminsTopView.DefaultIfEmpty()
                       join register in new AdminsTopView(this.Reponsitory) on preProduct.AdminID equals register.ID into registerAdminsTopView
                       from register in registerAdminsTopView.DefaultIfEmpty()
                           //orderby entity.CreateDate
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
                           ClassifyFirstOperatorName = firstOperator != null ? firstOperator.ByName : "--",
                           ClassifySecondOperator = entity.ClassifySecondOperator,
                           ClassifySecondOperatorName = secondOperator != null ? secondOperator.ByName : "--",
                           Summary = entity.Summary,
                           RegisterName = register != null ? register.RealName : "--",
                           IcgooAdminName = preProduct.IcgooAdmin
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
                //不显示锁定的
                var productClassifyLocks = new Origins.Locks_ClassifyOrigin(this.Reponsitory);
                string[] lockedOrderItemIDs = productClassifyLocks
                    .Where(t => t.LockerID != currentAdminID)
                    .Select(t => t.MainID).ToArray();

                linq = from p in linq
                       where !(lockedOrderItemIDs).Contains(p.ID)
                       select p;
            }

            return linq;
        }

        protected override IEnumerable<Models.PD_PreClassifyProduct> OnReadShips(Models.PD_PreClassifyProduct[] results)
        {
            var productLocksView = new Locks_ClassifyAll(this.Reponsitory);


            return from result in results
                   join productLock in productLocksView on result.ID equals productLock.MainID into productLocks
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
                       PreProduct = result.PreProduct,

                       ClassifyFirstOperator = result.ClassifyFirstOperator,
                       ClassifyFirstOperatorName = result.ClassifyFirstOperatorName,
                       ClassifySecondOperator = result.ClassifySecondOperator,
                       ClassifySecondOperatorName = result.ClassifySecondOperatorName,

                       ProductUnionCode = result.PreProduct.ProductUnionCode,
                       IsPushStatusWarning = result.IsPushStatusWarning,
                       RegisterName = result.RegisterName,
                       IcgooAdminName = result.IcgooAdminName
                   };
        }
    }
}
