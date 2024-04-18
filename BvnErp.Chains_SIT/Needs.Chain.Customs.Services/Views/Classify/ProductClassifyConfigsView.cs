using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Models;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 归类代码配置的视图
    /// </summary>
    public class ProductClassifyConfigsViewBase<T> : UniqueView<T, ScCustomsReponsitory> where T : Models.ProductClassifyConfig, new()
    {
        public ProductClassifyConfigsViewBase()
        {
        }

        internal ProductClassifyConfigsViewBase(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<T> GetIQueryable()
        {
            return from config in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ProductClassifyConfigs>()
                   select new T
                   {
                       ID = config.ID,
                       Type = (Enums.ClassifyType)config.Type,
                       Step = (Enums.ClassifyStep)config.Step,
                       CompanyType = (Enums.CompanyTypeEnums?)config.CompanyType,
                       ClassName = config.ClassName
                   };
        }
    }

    public class ProductClassifyConfigsView : ProductClassifyConfigsViewBase<Models.ProductClassifyConfig>
    {

    }

    /// <summary>
    /// 产品归类配置的视图
    /// </summary>
    public class OutsideProductConfigsView : ProductClassifyConfigsViewBase<Models.OutsideProductConfig>
    {
        protected override IQueryable<Models.OutsideProductConfig> GetIQueryable()
        {
            return base.GetIQueryable().Where(c => c.Type == Enums.ClassifyType.ProductClassify);
        }
    }

    /// <summary>
    /// 产品预归类配置的视图
    /// </summary>
    public class AdvanceProductConfigsView : ProductClassifyConfigsViewBase<Models.AdvanceProductConfig>
    {
        protected override IQueryable<Models.AdvanceProductConfig> GetIQueryable()
        {
            return base.GetIQueryable().Where(c => c.Type == Enums.ClassifyType.ProductPreClassify);
        }
    }
}
