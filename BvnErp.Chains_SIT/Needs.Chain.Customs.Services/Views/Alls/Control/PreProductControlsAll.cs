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
    /// 预归类产品管控审批视图
    /// </summary>
    public class PreProductControlsAll : Needs.Linq.Generic.Unique1Classics<Models.PreProductControl, ScCustomsReponsitory>
    {
        public PreProductControlsAll()
        {
        }

        internal PreProductControlsAll(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<PreProductControl> GetIQueryable(Expression<Func<PreProductControl, bool>> expression, params LambdaExpression[] expressions)
        {
            var preProductControls = new Origins.PreProductControlsOrigin(this.Reponsitory);
            var preProducts = new Rolls.PreProductsRoll(this.Reponsitory);

            var linq = from entity in preProductControls
                       join pp in preProducts on entity.PreProductID equals pp.ID
                       select new PreProductControl()
                       {
                           ID = entity.ID,
                           PreProductID = entity.PreProductID,
                           Type = entity.Type,
                           Status = entity.Status,
                           CreateDate = entity.CreateDate,
                           ApproveDate = entity.ApproveDate,
                           ApproverID = entity.ApproverID,
                           Summary = entity.Summary,
                           PreProduct = pp
                       };

            foreach (var predicate in expressions)
            {
                linq = linq.Where(predicate as Expression<Func<PreProductControl, bool>>);
            }

            linq = linq.Where(expression);

            return linq;
        }

        protected override IEnumerable<PreProductControl> OnReadShips(PreProductControl[] results)
        {
            var ppIDs = results.Select(item => item.PreProductID).Distinct().ToArray();
            var preProductCategories = new Origins.PreProductCategoriesOrigin(this.Reponsitory).Where(item => ppIDs.Contains(item.PreProductID)).ToArray();

            var adminIDs = (preProductCategories.Select(item => item.ClassifyFirstOperator)
                            .Union(preProductCategories.Select(item => item.ClassifySecondOperator))
                            ).Distinct().ToArray();
            var admins = new AdminsTopView(this.Reponsitory).Where(item => adminIDs.Contains(item.ID)).ToArray();

            return from result in results
                   join ppc in preProductCategories on result.PreProductID equals ppc.PreProductID
                   join firstOperator in admins on ppc.ClassifyFirstOperator equals firstOperator.ID into firstOperatorAdmins
                   from firstOperator in firstOperatorAdmins.DefaultIfEmpty()
                   join secondOperator in admins on ppc.ClassifySecondOperator equals secondOperator.ID into secondOperatorAdmins
                   from secondOperator in secondOperatorAdmins.DefaultIfEmpty()
                   select new PreProductControl()
                   {
                       ID = result.ID,
                       PreProductID = result.PreProductID,
                       Type = result.Type,
                       Status = result.Status,
                       CreateDate = result.CreateDate,
                       ApproveDate = result.ApproveDate,
                       ApproverID = result.ApproverID,
                       Summary = result.Summary,
                       PreProduct = result.PreProduct,
                       Category = new PreClassifyProduct()
                       {
                           ID = ppc.ID,
                           HSCode = ppc.HSCode,
                           ProductName = ppc.ProductName,
                           Type = ppc.Type,
                           ClassifyFirstOperator = ppc.ClassifyFirstOperator,
                           ClassifyFirstOperatorName = firstOperator != null ? firstOperator.RealName : "--",
                           ClassifySecondOperator = ppc.ClassifySecondOperator,
                           ClassifySecondOperatorName = secondOperator != null ? secondOperator.RealName : "--",
                       }
                   };
        }
    }
}
