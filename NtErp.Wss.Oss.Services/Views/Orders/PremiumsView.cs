using NtErp.Wss.Oss.Services;
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
    /// 附加价值视图
    /// </summary>
    public class PremiumsView : UniqueView<Models.Premium, CvOssReponsitory>
    {
        internal PremiumsView()
        {

        }
        internal PremiumsView(CvOssReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Premium> GetIQueryable()
        {
            var linq = from entity in Reponsitory.ReadTable<Layer.Data.Sqls.CvOss.Premiums>()
                       select new Models.Premium
                       {
                           ID = entity.ID,
                           OrderID = entity.OrderID,
                           OrderItemID = entity.OrderItemID,
                           Count = entity.Count,
                           Price = entity.Price,
                           Name = entity.Name,
                           Summary = entity.Summary,
                           CreateDate = entity.CreateDate
                       };

            return linq;
        }

    }
}
