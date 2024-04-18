using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.PvOms.WebApp.Stocks.Temporary
{
    public partial class MyList : ErpParticlePage
    {

        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected object data()
        {
            var entercode = Request.QueryString["EnterCode"];
            var query = Erp.Current.WsOrder.TempStorages.GetTempStorage().Where(t => t.EnterCode == entercode).AsEnumerable();

            var linq = query.Select(t => new
            {
                ID = t.ID,
                CreateDate = t.CreateDate.ToString("yyyy-MM-dd"),
                Manufacturer = t.Manufacturer,
                PartNumber = t.PartNumber,
                DateCode = t.DateCode,
                Origin = t.Origin,
                OriginCode = t.Origin.GetOrigin().Code,
                Quantity = t.Quantity,
                StorageID = t.ID,
            });
            return new
            {
                rows = linq.ToArray(),
                total = query.Count()
            };
        }
    }
}