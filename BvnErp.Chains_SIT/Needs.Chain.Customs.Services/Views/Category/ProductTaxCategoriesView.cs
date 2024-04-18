using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class ProductTaxCategoriesAllsView : UniqueView<Models.ProductTaxCategory, ScCustomsReponsitory>
    {
        public ProductTaxCategoriesAllsView()
        {
        }

        internal ProductTaxCategoriesAllsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ProductTaxCategory> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ProductTaxCategories>()
                   select new Models.ProductTaxCategory
                   {
                       ID = entity.ID,
                       TaxCode = entity.TaxCode,
                       TaxName = entity.TaxName,
                       Name = entity.Name,
                       Model = entity.Model,
                       CreateDate = entity.CreateTime
                   };
        }
    }

    /// <summary>
    /// 产品税务归类
    /// </summary>
    public sealed class ProductTaxCategoriesView : ProductTaxCategoriesAllsView
    {
        string Name = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model">品名</param>
        public ProductTaxCategoriesView(string name)
        {
            this.Name = name;
        }

        protected override IQueryable<Models.ProductTaxCategory> GetIQueryable()
        {
            return from entity in base.GetIQueryable()
                   where entity.Name.Contains(this.Name)
                   select entity;
        }
    }

    /// <summary>
    /// 产品税务归类
    /// </summary>
    public sealed class MyProductTaxCategoriesView : ProductTaxCategoriesAllsView
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model">品名</param>
        public MyProductTaxCategoriesView()
        {
           
        }

        internal MyProductTaxCategoriesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.ProductTaxCategory> GetIQueryable()
        {
            return from entity in base.GetIQueryable()
                   select entity;
        }
    }
}