using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Order.Fee.DecChargeStandard
{
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Model.timestamp = GetTimeStamp();
        }

        private string GetTimeStamp()
        {
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        protected void DecChargeStandardList()
        {
            using (var query = new Needs.Ccs.Services.Views.ChargeStandardListForMaintainView())
            {
                var view = query;

                var stds = view.ToTile();

                Response.Write(new { stds = stds, }.Json());
            }

        }

        protected void DecChargeStandardOneObj()
        {
            using (var query = new Needs.Ccs.Services.Views.ChargeStandardListForMaintainView())
            {
                var view = query;

                var obj = view.ToOneObject();

                Response.Write(new { obj = obj, }.Json());
            }
        }
    }
}