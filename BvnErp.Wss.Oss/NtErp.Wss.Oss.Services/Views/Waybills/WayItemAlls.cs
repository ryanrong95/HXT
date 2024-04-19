using NtErp.Wss.Oss.Services.Models;
using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Oss.Services.Views
{
    /// <summary>
    /// 运单项视图
    /// </summary>
    public class WayItemAlls : UniqueView<WayItem, CvOssReponsitory>
    {
        internal WayItemAlls()
        {

        }
        internal WayItemAlls(CvOssReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<WayItem> GetIQueryable()
        {
          

            var linq = from entity in Reponsitory.ReadTable<Layer.Data.Sqls.CvOss.WayItems>()
                       select new WayItem
                       {
                           ID = entity.ID,
                           WaybillID = entity.WaybillID,
                           OrderID = entity.OrderID,
                           OrderItemID = entity.OrderItemID,
                           Weight = entity.Weight,
                           Count = entity.Count,
                           Source = (WayItemSource)entity.Source
                       };

            return linq;
        }
    }
}
