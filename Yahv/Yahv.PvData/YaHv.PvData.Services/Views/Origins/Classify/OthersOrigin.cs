using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace YaHv.PvData.Services.Views.Origins
{
    /// <summary>
    /// 产品归类特殊类型
    /// </summary>
    internal class OthersOrigin : UniqueView<Models.Other, PvDataReponsitory>
    {
        internal OthersOrigin()
        {
        }

        internal OthersOrigin(PvDataReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Other> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvData.Others>()
                   select new Models.Other
                   {
                       ID = entity.ID,
                       PartNumber = entity.PartNumber,
                       Manufacturer = entity.Manufacturer,
                       Ccc = entity.Ccc,
                       Embargo = entity.Embargo,
                       HkControl = entity.HkControl,
                       Coo = entity.Coo,
                       CIQ = entity.CIQ,
                       CIQprice = entity.CIQprice,
                       CreateDate = entity.CreateDate,
                       OrderDate = entity.OrderDate,
                       Summary = entity.Summary
                   };
        }
    }
}
