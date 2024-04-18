using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Web.Erp;

namespace Yahv.PvData.WebApp.SysConfig.ClassifyHistory
{
    public partial class Quote : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.PartNumber = Request.QueryString["partNumber"];
            }
        }

        protected object data()
        {
            string partNumber = Request.QueryString["partNumber"];

            var productQuotes = new YaHv.PvData.Services.Views.Alls.ProductQuotesAll()[partNumber, -36]
                    .OrderByDescending(pq => pq.CreateDate).ToArray()
                    .Select(pq => new
                    {
                        pq.ID,
                        pq.Manufacturer,
                        pq.UnitPrice,
                        pq.Currency,
                        pq.Quantity,
                        CreateDate = pq.CreateDate.ToString("yyyy-MM-dd HH:mm")
                    });

            if (productQuotes.Count() == 0)
            {
                productQuotes = new YaHv.PvData.Services.Views.Alls.Logs_ClassifiedPartNumberAll()[partNumber, -36]
                    .OrderByDescending(cpl => cpl.CreateDate).ToArray()
                    .Select(cpl => new
                    {
                        cpl.ID,
                        cpl.Manufacturer,
                        cpl.UnitPrice,
                        cpl.Currency,
                        Quantity = cpl.Quantity.GetValueOrDefault(),
                        CreateDate = cpl.CreateDate.ToString("yyyy-MM-dd HH:mm")
                    });
            }

            return new
            {
                rows = productQuotes,
                total = productQuotes.Count()
            };
        }
    }
}