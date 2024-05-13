using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.chonggous.Models;
using Yahv.Linq;

namespace Wms.Services.chonggous.Views
{
    public class PartNumberAvgWeightsTopView : QueryView<PartNumberAvgWeight, PvWmsRepository>
    {
        protected override IQueryable<PartNumberAvgWeight> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.PartNumberAvgWeightsTopView>()
                   select new PartNumberAvgWeight
                   {
                       PartNumber = entity.PartNumber,
                       AVGWeight = entity.AVGWeight,
                   };
        }
    }
}
