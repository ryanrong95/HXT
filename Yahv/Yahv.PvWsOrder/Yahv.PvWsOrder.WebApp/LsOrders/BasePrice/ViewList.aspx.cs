using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Web.Erp;

namespace Yahv.PvOms.WebApp.LsOrders.BasePrice
{
    public partial class ViewList : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected object data()
        {
            var linq = Erp.Current.WsOrder.LsProductPrice.Select(item => new
            {
                ID = item.ID,
                ProductID = item.LsProduct.ID,
                Name = item.LsProduct.Name,
                SpecID = item.LsProduct.SpecID,
                Load = item.LsProduct.Load,
                Volume = item.LsProduct.Volume,
                Month = item.Month,
                Price = item.Price,
                Summary = item.Summary,
            });
            return new
            {
                rows = linq.OrderBy(en => en.Month).OrderBy(en => en.SpecID).ToArray(),
                total = linq.Count(),
            };
        }
    }
}