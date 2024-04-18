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
    public class ModelAvgWeightOrigin : UniqueView<ModelAvgWeight, ScCustomsReponsitory>
    {
        protected override IQueryable<ModelAvgWeight> GetIQueryable()
        {
            var linq = from sort in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PvWmsPartNumberAvgWeightsTopView>()
                       select new ModelAvgWeight
                       {
                           Model = sort.PartNumber,
                           Weight = sort.AVGWeight                        
                       };

            return linq;
        }
    }
}
