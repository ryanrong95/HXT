using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class TaxCategoriesAllsView : UniqueView<Models.TaxCategory, ScCustomsReponsitory>
    {
        public TaxCategoriesAllsView()
        {
        }

        internal TaxCategoriesAllsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.TaxCategory> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.TaxCategories>()
                   select new Models.TaxCategory
                   {
                       ID = entity.ID,
                       TaxCode = entity.TaxCode,
                       TaxName = entity.TaxName,
                       KeyWords = entity.KeyWords,
                       Description = entity.Description,
                       IsElectronic = entity.IsElectronic
                   };
        }
    }

    /// <summary>
    /// 税务名称
    /// </summary>
    public sealed class TaxNameTaxCategoriesView : TaxCategoriesAllsView
    {
        string TaxName = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taxName">名称</param>
        public TaxNameTaxCategoriesView(string taxName)
        {
            this.TaxName = taxName;
        }

        protected override IQueryable<Models.TaxCategory> GetIQueryable()
        {
            return from entity in base.GetIQueryable()
                   where entity.TaxName == this.TaxName
                   select entity;
        }
    }

    /// <summary>
    /// 品名
    /// </summary>
    public sealed class KeywordsTaxCategoriesView : TaxCategoriesAllsView
    {
        string Name = string.Empty;

        /// <summary>
        /// TODO：关键词匹配算法确定后，需要修改查询条件 where entity.KeyWords.Contains(this.Name)
        /// </summary>
        /// <param name="name">报关品名</param>
        public KeywordsTaxCategoriesView(string name)
        {
            this.Name = name;
        }

        protected override IQueryable<Models.TaxCategory> GetIQueryable()
        {
            return from entity in base.GetIQueryable()
                   where entity.KeyWords.Contains(this.Name)
                   select entity;
        }
    }

    /// <summary>
    /// 税务编码
    /// </summary>
    public sealed class TaxCodeTaxCategoriesView : TaxCategoriesAllsView
    {
        string TaxCode = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taxCode">税号</param>
        public TaxCodeTaxCategoriesView(string taxCode)
        {
            this.TaxCode = taxCode;
        }

        protected override IQueryable<Models.TaxCategory> GetIQueryable()
        {
            return from entity in base.GetIQueryable()
                   where entity.TaxCode == this.TaxCode
                   select entity;
        }
    }

    /// <summary>
    /// 产品税务归类
    /// </summary>
    public sealed class MyTaxCategoriesView : TaxCategoriesAllsView
    {
        string CilentID = string.Empty;
        string Name = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taxCode">税号</param>
        public MyTaxCategoriesView(string clientID, string name)
        {
            this.CilentID = clientID;
            this.Name = name;
        }

        protected override IQueryable<Models.TaxCategory> GetIQueryable()
        {
            //1.查询客户自定义的产品税务归类
            var view = new ClientProductTaxCategoriesView(this.CilentID, this.Name);
            if (view.Count() > 0)
            {
                return view.Select(item => new Models.TaxCategory
                {
                    ID = item.ID,
                    Name = item.Name,
                    TaxCode = item.TaxCode,
                    TaxName = item.TaxName,
                    CreateDate = item.CreateDate
                });
            }

            //2.如果客户没有自定义，则查询产品税务归类历史记录
            var productTaxCategories = new ProductTaxCategoriesView(this.Name);
            if (productTaxCategories.Count() > 0)
            {
                return productTaxCategories.Select(item => new Models.TaxCategory
                {
                    ID = item.ID,
                    Name = item.Name,
                    TaxCode = item.TaxCode,
                    TaxName = item.TaxName,
                    CreateDate = item.CreateDate
                });
            }

            //3.客户没有自定义，也没有产品税务归类历史记录，则查询税务分类基础信息
            return new TaxCategoriesDefaultsView(this.Name).Select(item => new Models.TaxCategory
            {
                ID = item.ID,
                Name = item.TaxSecondCategory,
                TaxCode = item.TaxCode,
                TaxName = item.TaxSecondCategory,
                CreateDate = item.CreateDate
            });
        }
    }
}