using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Chains
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected object data()
        {
            var query = Erp.Current.CrmPlus.MyChains;

            var result = this.Paging(query.OrderByDescending(item => item.CreateDate).ToArray().Select(item => new
            {
                item.ID,
                item.Enterprise.Name,
                item.Grade,
                item.Vip,
                item.ServiceType,
                item.WsCode,
                Owner = item.Owner?.RealName,
                Tracker = item.Tracker?.RealName,
                Referrer = item.Referrer?.RealName,
                item.Status,
                CreteDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")
            }));

            return result;

        }
    }
}