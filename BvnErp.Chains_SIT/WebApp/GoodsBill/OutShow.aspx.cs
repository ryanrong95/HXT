using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.GoodsBill
{
    public partial class OutShow : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void data()
        {
            var OrderItemID  = Request.QueryString["OrderItemID"];
            var outInfo = new Needs.Ccs.Services.Views.Sz_Cfb_OutViewOrigin().Where(t => t.OrderItemID == OrderItemID).ToList();

            Func<Needs.Ccs.Services.Models.OutStoreViewModel, object> convert = item => new
            {
                ID = item.ID,
                OperatorName = item.OperatorName,
                OutQty = item.OutQty,
                OutStoreDate = item.OutStoreDate.ToString("yyyy-MM-dd"),
            };

            Response.Write(new
            {
                rows = outInfo.Select(convert).ToArray(),
                total = outInfo.Count()
            }.Json());
        }
    }
}