using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.ClientModels;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    public class CustomerServiceLinksView : UniqueView<CustomerServiceLink, PvWsOrderReponsitory>
    {
        protected override IQueryable<CustomerServiceLink> GetIQueryable()
        {
            return from customerServiceLink in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.CustomerServiceLinks>()
                   select new CustomerServiceLink
                   {
                       ID = customerServiceLink.ID,
                       Link = customerServiceLink.Link,
                   };
        }
    }
}
