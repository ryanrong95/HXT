using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Srm.SupplierDetails.Files
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected object data()
        {
            string enterpriseid = Request.QueryString["enterpriseid"];
            var query = Yahv.CrmPlus.Service.Files.SupplierFiles(enterpriseid);
            return this.Paging(query.OrderByDescending(item => item.CreateDate).ToArray().Select(item => new
            {
                item.ID,
                item.SubID,
                Type = item.Type.GetDescriptions(),
                item.CustomName,
                item.Url,
                item.CreatorName,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm")
            }));

        }
    }
}