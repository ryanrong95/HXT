using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Crm.Client.Owner
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected object data()
        {
            string id = Request.QueryString["id"];
            var query = new OwnerRoll(id);
            var result = this.Paging(query.OrderByDescending(item=>item.CreateDate).ToArray().Select(item => new
            {
                item.ID,
                ClientName= item.Enterprise.Name,
                CorCompany=item.Company.Name,
                ConductType=item.Type.GetDescription(),
                item.Status,
                StaffName = item.Admin?.RealName,
                Position=item.Admin.RoleName,
                CreteDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")
            }));

            return result;
        }
    }
}