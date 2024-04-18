using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 申报要素默认值的视图
    /// </summary>
    public class CustomsElementsDefaultsView : UniqueView<Models.ElementsDefault, ScCustomsReponsitory>
    {
        public CustomsElementsDefaultsView()
        {
        }

        internal CustomsElementsDefaultsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<ElementsDefault> GetIQueryable()
        {
            return from defaults in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.CustomsElementsDefaults>()
                   select new Models.ElementsDefault
                   {
                       ID = defaults.ID,
                       CustomsTariffID = defaults.CustomsTariffID,
                       ElementName = defaults.ElementName,
                       DefaultValue = defaults.DefaultValue,
                       CreateDate = defaults.CreateDate
                   };
        }
    }
}
