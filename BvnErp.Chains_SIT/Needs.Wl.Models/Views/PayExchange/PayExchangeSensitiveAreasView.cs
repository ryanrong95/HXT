using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Models.Views
{
    public class PayExchangeSensitiveAreasView : View<Models.PayExchangeSensitiveArea, ScCustomsReponsitory>
    {
        public PayExchangeSensitiveAreasView()
        {

        }

        internal PayExchangeSensitiveAreasView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.PayExchangeSensitiveArea> GetIQueryable()
        {
            return from payExchangeSensitiveArea in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeSensitiveAreas>()
                   where payExchangeSensitiveArea.Status == (int)Enums.Status.Normal
                   select new Models.PayExchangeSensitiveArea
                   {
                       ID = payExchangeSensitiveArea.ID,
                       Type = (Enums.PayExchangeSensitiveAreaType)payExchangeSensitiveArea.Type,
                       Name = payExchangeSensitiveArea.Name,
                       Status = payExchangeSensitiveArea.Status,
                       CreateDate = payExchangeSensitiveArea.CreateDate,
                       UpdateDate = payExchangeSensitiveArea.UpdateDate,
                       Summary = payExchangeSensitiveArea.Summary,
                   };
        }
    }
}
