using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PvOms.WebApp.LsOrders.BasePrice
{
    public partial class List : ErpParticlePage
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
                CreatorName=item.CreatorName,
                Summary = item.Summary,
            });
            return new
            {
                rows = linq.OrderBy(en => en.Month).OrderBy(en => en.SpecID).ToArray(),
                total = linq.Count(),
            };
        }

        //删除库位配置
        protected void Delete()
        {
            try
            {
                string productPriceID = Request.Form["id"];

                LsProductPrice lsPrice = new LsProductPrice()
                {
                    ID = productPriceID,
                };

                lsPrice.Delete();
                Response.Write((new { success = true, message = "删除成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "删除失败：" + ex.Message }).Json());
            }
        }
    }
}