using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PvRoute.WebApp.LogisticsInfo.WaybillLogs
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected object data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string faceOrderID = Request.QueryString["faceOrderID"];
            using (var query = Erp.Current.PvRoute.TransportLogs)
            {
                var view = query;

                if (!string.IsNullOrWhiteSpace(faceOrderID))
                {
                    view = view.SearchByFaceOrderID(faceOrderID);
                }

                return view.ToMyPage(page, rows).Json();
            }
        }

    }
}