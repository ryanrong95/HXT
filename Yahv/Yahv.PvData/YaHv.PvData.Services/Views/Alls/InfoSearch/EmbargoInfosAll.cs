using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Services.Views;
using Yahv.Services.Models;

namespace YaHv.PvData.Services.Views.Alls
{
    /// <summary>
    /// 禁运信息查询视图
    /// </summary>
    public class EmbargoInfosAll : OthersAll
    {
        protected override IQueryable<Models.Other> GetIQueryable()
        {
            var eccnsView = new EccnsTopView<PvDataReponsitory>(this.Reponsitory);

            return from entity in base.GetIQueryable()
                   join eccn in eccnsView on entity.PartNumber equals eccn.PartNumber into eccns
                   orderby entity.OrderDate descending
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
                       Summary = entity.Summary,
                       CreateDate = entity.CreateDate,
                       OrderDate = entity.OrderDate,

                       Eccns = eccns.ToList()
                   };
        }
    }
}
