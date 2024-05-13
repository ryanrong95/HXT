using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.Models;
using Yahv.Linq;
using Layers.Data.Sqls;

namespace Wms.Services.Views
{
    public class LsNoticeView : UniqueView<LsNotice, PvWmsRepository>
    {

        public object GetLsNoticeData()
        {
            return from entity in this.GetIQueryable()
                   group entity by entity.OrderID into groups
                   select new
                   {
                       OrderID = groups.Key,
                       CreateDate = groups.Select(item => item.CreateDate).FirstOrDefault(),
                       ClientID = groups.Select(item => item.ClientID).FirstOrDefault(),
                       Count = groups.Key.Count(),
                       Status = groups.Select(item => item.Status).FirstOrDefault(),

                   };
        }

        protected override IQueryable<LsNotice> GetIQueryable()
        {

            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.LsNotice>()

                   select new LsNotice
                   {
                       ID = entity.ID,
                       SpecID = entity.SpecID,
                       Quantity = entity.Quantity,
                       StartDate = entity.StartDate,
                       EndDate = entity.EndDate,
                       CreateDate = entity.CreateDate,
                       Summary = entity.Summary,
                       OrderID = entity.OrderID,
                       ClientID = entity.ClientID,
                       PayeeID = entity.PayeeID,
                       Status = (LsNoticeStatus)entity.Status,
                   };
        }
    }
}
