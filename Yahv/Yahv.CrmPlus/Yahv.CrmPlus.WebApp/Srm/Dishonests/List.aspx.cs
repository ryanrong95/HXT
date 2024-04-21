using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Srm.Dishonests
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected object data()
        {
            var query = new Service.Views.Rolls.SupplierDishonestsRoll();
            return this.Paging(query.OrderByDescending(item => item.CreateDate).ToArray().Select(item => new
            {
                item.ID,
                item.SupplierID,
                item.EnterpriseName,
                item.Reason,
                OccurTime= item.OccurTime.ToShortDateString(),
                item.Code,
                RealName = item.Creator?.RealName,
                item.Summary
            }));
        }
    }
}