using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.Alls
{
    /// <summary>
    /// 预归类产品视图(用于产品预归类/咨询归类已完成列表)
    /// </summary>
    public class PD_PreClassifyProductsDone : Needs.Linq.Generic.Unique1Classics<Models.PD_PreClassifyProduct, ScCustomsReponsitory>
    {
        public PD_PreClassifyProductsDone()
        {
        }

        internal PD_PreClassifyProductsDone(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.PD_PreClassifyProduct> GetIQueryable(
            Expression<Func<Models.PD_PreClassifyProduct, bool>> expression,
            params LambdaExpression[] expressions)
        {
            var preProductCategories = new Origins.PreProductCategoriesOrigin(this.Reponsitory);
            var preProductsView = new Origins.PreProductsOrigin(this.Reponsitory);
            var adminView = new AdminsTopView(this.Reponsitory);
            //var apiNoticeClassifyPushStatusView = new Origins.ApiNoticeClassifyPushStatusViewOrigin(this.Reponsitory);
            var pushFailureIDs = new Origins.ApiNoticeClassifyPushStatusViewOrigin(this.Reponsitory)
                .Where(item => item.PushStatus == Enums.PushStatus.PushFailure).Select(item => item.PreProductCategoryID).ToArray();
            var linq = from entity in preProductCategories
                       join preProduct in preProductsView on entity.PreProductID equals preProduct.ID
                       join admin in adminView on preProduct.AdminID equals admin.ID into admins
                       from register in admins.DefaultIfEmpty()
                       orderby entity.CreateDate descending
                       select new Models.PD_PreClassifyProduct
                       {
                           ID = entity.ID,
                           PreProductID = entity.PreProductID,
                           //PreProduct = preProduct,
                           PreProduct = new Models.PreProduct
                           {
                               ID = preProduct.ID,
                               ClientID = preProduct.ClientID,
                               Price = preProduct.Price,
                               Qty = preProduct.Qty,
                               Currency = preProduct.Currency,
                               AreaOfProduction = preProduct.AreaOfProduction,
                               CompanyType = preProduct.CompanyType,
                               DueDate = preProduct.DueDate,
                               UseType = preProduct.UseType,
                               ProductUnionCode = preProduct.ProductUnionCode,
                               AdminID = preProduct.AdminID
                           },
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
                           IsPushStatusWarning = pushFailureIDs.Contains(entity.ID),
                           Source = preProduct.Source,
                           RegisterName = register == null ? "--" : register.RealName
                       };

            foreach (var predicate in expressions)
            {
                linq = linq.Where(predicate as Expression<Func<Models.PD_PreClassifyProduct, bool>>);
            }

            linq = linq.OrderByDescending(t => t.IsPushStatusWarning);
            linq = linq.Where(expression);

            return linq;
        }

        protected override IEnumerable<Models.PD_PreClassifyProduct> OnReadShips(Models.PD_PreClassifyProduct[] results)
        {
            var adminIDs = (results.Select(item => item.ClassifyFirstOperator)
                .Union(results.Select(item => item.ClassifySecondOperator)))
                .Distinct().ToArray();
            var admins = new AdminsTopView(this.Reponsitory).Where(item => adminIDs.Contains(item.ID)).ToArray();

            var clientIDs = results.Select(item => item.PreProduct.ClientID).Distinct().ToArray();
            var clients = new Rolls.ClientsRoll(this.Reponsitory).Where(item => clientIDs.Contains(item.ID)).ToArray();

            var productLocksView = new Locks_ClassifyAll(this.Reponsitory);

            var data = from result in results
                       join client in clients on result.PreProduct.ClientID equals client.ID
                       join firstOperator in admins on result.ClassifyFirstOperator equals firstOperator.ID into firstOperatorAdminsTopView
                       from firstOperator in firstOperatorAdminsTopView.DefaultIfEmpty()
                       join secondOperator in admins on result.ClassifySecondOperator equals secondOperator.ID into secondOperatorAdminsTopView
                       from secondOperator in secondOperatorAdminsTopView.DefaultIfEmpty()
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
                           ClassifyFirstOperatorName = firstOperator != null ? firstOperator.RealName : "--",
                           ClassifySecondOperator = result.ClassifySecondOperator,
                           ClassifySecondOperatorName = secondOperator != null ? secondOperator.RealName : "--",

                           ProductUnionCode = result.PreProduct.ProductUnionCode,
                           IsPushStatusWarning = result.IsPushStatusWarning,
                           Source = result.Source,
                           RegisterName = result.RegisterName
                       };

            return data;
        }
    }
}
