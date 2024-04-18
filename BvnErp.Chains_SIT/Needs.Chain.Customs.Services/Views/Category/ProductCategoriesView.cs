using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class ProductCategoriesAllsView : UniqueView<Models.ProductCategory, ScCustomsReponsitory>
    {
        public ProductCategoriesAllsView()
        {
        }

        internal ProductCategoriesAllsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ProductCategory> GetIQueryable()
        {
            var adminsView = new AdminsTopView(this.Reponsitory);
            var customsTariffs = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.CustomsTariffs>();
            return from catogory in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ProductCategories>()
                   join admin in adminsView on catogory.AdminID equals admin.ID into admins
                   from admin in admins.DefaultIfEmpty()
                   join  custom in customsTariffs on catogory.HSCode equals custom.HSCode
                   orderby catogory.CreateDate descending
                   select new Models.ProductCategory
                   {
                       ID = catogory.ID,
                       Name = catogory.Name,
                       Model = catogory.Model,
                       HSCode = catogory.HSCode,
                       Elements = catogory.Elements,
                       CreateDate = catogory.CreateDate,
                       TariffRate = custom.MFN,
                       AddedValueRate = custom.AddedValue,
                       InspectionFee = catogory.InspectionFee,
                       UnitPrice = catogory.UnitPrice,
                       Qty = catogory.Quantity,
                       Declarant = admin
                   };
        }
    }

    /// <summary>
    /// 产品海关归类记录
    /// </summary>
    public sealed class ProductCategoriesView : ProductCategoriesAllsView
    {
        string Model = string.Empty;

        bool IsModelLike = false;

        /// <summary>
        /// 产品管控
        /// </summary>
        /// <param name="model">型号</param>
        public ProductCategoriesView(string model)
        {
            this.Model = model;
        }

        public ProductCategoriesView(string model, bool isModelLike)
        {
            this.Model = model;
            this.IsModelLike = isModelLike;
        }

        protected override IQueryable<Models.ProductCategory> GetIQueryable()
        {
            List<LambdaExpression> expressions = new List<LambdaExpression>();

            //this.Model 判断条件是否使用 Like
            if (this.IsModelLike)
            {
                expressions.Add((Expression<Func<Models.ProductCategory, bool>>)(item => item.Model.Contains(this.Model)));
            }
            else
            {
                expressions.Add((Expression<Func<Models.ProductCategory, bool>>)(item => item.Model == this.Model));
            }

            //给 view 添加条件
            var view = base.GetIQueryable();
            foreach (var expression in expressions)
            {
                view = view.Where(expression as Expression<Func<Models.ProductCategory, bool>>);
            }

            return view;
        }
    }
}