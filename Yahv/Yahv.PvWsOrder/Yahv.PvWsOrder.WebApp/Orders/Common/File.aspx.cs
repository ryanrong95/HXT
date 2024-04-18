using System;
using System.Linq;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.PvOms.WebApp.Orders.Common
{
    public partial class File : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        protected object data()
        {
            var orderId = Request.QueryString["ID"];
            var query = new PvWsOrder.Services.Views.OrderFilesRoll(orderId).AsEnumerable();
            var linq = query.Select(t => new
            {
                ID = t.ID,
                CustomName = t.CustomName,
                FileType = ((FileType)t.Type).GetDescription(),
                Url = PvWsOrder.Services.Common.FileDirectory.ServiceRoot + t.Url,
                CreateDate = t.CreateDate.Value.ToString("yyyy-MM-dd"),
            });
            return new
            {
                rows = linq.ToArray(),
                total = query.Count()
            };
        }
    }
}