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
    /// 税务归类的视图
    /// </summary>
    public class TaxCategoriesDefaultsAllsView : UniqueView<Models.TaxCategoriesDefault, ScCustomsReponsitory>
    {
        public TaxCategoriesDefaultsAllsView()
        {
        }

        internal TaxCategoriesDefaultsAllsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.TaxCategoriesDefault> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.TaxCategoriesDefaults>()
                   select new Models.TaxCategoriesDefault
                   {
                       ID = entity.ID,
                       TaxCode = entity.TaxCode,
                       TaxFirstCategory = entity.TaxFirstCategory,
                       TaxSecondCategory = entity.TaxSecondCategory,
                       TaxThirdCategory = entity.TaxThirdCategory,
                       Status = (Enums.Status)entity.Status,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       Summary = entity.Summary
                   };
        }
    }

    /// <summary>
    /// 根据报关品名查询对应的税务归类
    /// </summary>
    public sealed class TaxCategoriesDefaultsView : TaxCategoriesDefaultsAllsView
    {
        string Name = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">报关品名</param>
        public TaxCategoriesDefaultsView(string name)
        {
            this.Name = name;
        }

        protected override IQueryable<Models.TaxCategoriesDefault> GetIQueryable()
        {
            return from entity in base.GetIQueryable()
                   where entity.TaxSecondCategory.Contains(this.Name)
                   select entity;
        }
    }
}
