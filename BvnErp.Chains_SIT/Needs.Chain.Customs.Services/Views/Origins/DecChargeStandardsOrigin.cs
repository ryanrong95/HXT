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
    public class DecChargeStandardsOrigin : UniqueView<Models.DecChargeStandard, ScCustomsReponsitory>
    {
        public DecChargeStandardsOrigin()
        {
        }

        public DecChargeStandardsOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<DecChargeStandard> GetIQueryable()
        {
            var linq = from entity in Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecChargeStandards>()
                       select new DecChargeStandard()
                       {
                           ID = entity.ID,
                           FatherID = entity.FatherID,
                           EnumValue = entity.EnumValue,
                           SerialNo = entity.SerialNo,
                           Type = entity.Type != null ? (Enums.ChargeStandardType?)entity.Type : null,
                           IsMenuLeaf = entity.IsMenuLeaf,
                           Name = entity.Name,
                           Unit1 = entity.Unit1,
                           Unit2 = entity.Unit2,
                           Price = entity.Price,
                           Remark1 = entity.Remark1,
                           Remark2 = entity.Remark2,
                           Status = (Enums.Status)entity.Status,
                           CreateDate = entity.CreateDate,
                           UpdateDate = entity.UpdateDate,
                           Summary = entity.Summary,
                       };
            return linq;
        }
    }
}
