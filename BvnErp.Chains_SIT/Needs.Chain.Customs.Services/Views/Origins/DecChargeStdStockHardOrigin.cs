using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.Origins
{
    public class DecChargeStdStockHardOrigin : UniqueView<Models.DecChargeStdStockHard, ScCustomsReponsitory>
    {
        public DecChargeStdStockHardOrigin()
        {
        }

        public DecChargeStdStockHardOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<DecChargeStdStockHard> GetIQueryable()
        {
            var linq = from entity in Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecChargeStdStockHard>()
                       select new DecChargeStdStockHard()
                       {
                           ID = entity.ID,
                           FatherID = entity.FatherID,
                           Unit = entity.Unit,
                           Price = entity.Price,
                           Remark1 = entity.Remark1,
                           Remark2 = entity.Remark2,
                           Remark3 = entity.Remark3,
                           Status = (Enums.Status)entity.Status,
                           CreateDate = entity.CreateDate,
                           UpdateDate = entity.UpdateDate,
                           Summary = entity.Summary,
                       };
            return linq;
        }
    }
}
