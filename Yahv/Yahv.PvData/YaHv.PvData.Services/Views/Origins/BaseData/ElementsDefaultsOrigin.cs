using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace YaHv.PvData.Services.Views.Origins
{
    internal class ElementsDefaultsOrigin : UniqueView<Models.ElementsDefault, PvDataReponsitory>
    {
        internal ElementsDefaultsOrigin()
        {
        }

        internal ElementsDefaultsOrigin(PvDataReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ElementsDefault> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvData.ElementsDefaults>()
                   select new Models.ElementsDefault
                   {
                       ID = entity.ID,
                       TariffID = entity.TariffID,
                       ElementName = entity.ElementName,
                       DefaultValue = entity.DefaultValue,
                       CreateDate = entity.CreateDate
                   };
        }
    }
}
