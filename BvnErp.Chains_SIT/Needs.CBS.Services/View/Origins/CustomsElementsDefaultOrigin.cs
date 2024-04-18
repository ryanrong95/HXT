using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Cbs.Services.View.Origins
{
    internal class CustomsElementsDefaultOrigin : UniqueView<Needs.Cbs.Services.Model.Origins.CustomsElementsDefault, ScCustomsReponsitory>
    {
        internal CustomsElementsDefaultOrigin()
        {
        }

        internal CustomsElementsDefaultOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Needs.Cbs.Services.Model.Origins.CustomsElementsDefault> GetIQueryable()
        {
            return from cert in this.Reponsitory.ReadTable<Layer.Data.Sqls.Customs.CustomsElementsDefaults>()
                   select new Needs.Cbs.Services.Model.Origins.CustomsElementsDefault
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
