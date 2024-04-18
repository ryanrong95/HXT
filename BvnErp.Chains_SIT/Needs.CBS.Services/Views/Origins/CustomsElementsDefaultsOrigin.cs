using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Cbs.Services.Views.Origins
{
    /// <summary>
    /// 申报要素默认值视图
    /// </summary>
    internal class CustomsElementsDefaultsOrigin : UniqueView<Models.Origins.CustomsElementsDefault, ScCustomsReponsitory>
    {
        internal CustomsElementsDefaultsOrigin()
        {
        }

        internal CustomsElementsDefaultsOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Origins.CustomsElementsDefault> GetIQueryable()
        {
            return from cert in this.Reponsitory.ReadTable<Layer.Data.Sqls.Customs.CustomsElementsDefaults>()
                   select new Models.Origins.CustomsElementsDefault
                   {
                       ID = cert.ID,
                       CustomsTariffID = cert.CustomsTariffID,
                       CreateDate = cert.CreateDate,
                       DefaultValue = cert.DefaultValue,
                       ElementName = cert.ElementName,
                   };
        }
    }
}
