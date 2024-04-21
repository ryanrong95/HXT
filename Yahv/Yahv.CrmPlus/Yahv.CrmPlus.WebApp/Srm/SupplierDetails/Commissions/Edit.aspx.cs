using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Srm.SupplierDetails.Commissions
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.Type = ExtendsEnum.ToDictionary<Underly.CommissionType>().Select(item => new
                {
                    value = int.Parse(item.Key),
                    text = item.Value
                });
                this.Model.Method = ExtendsEnum.ToDictionary<Underly.CommissionMethod>().Select(item => new
                {
                    value = int.Parse(item.Key),
                    text = item.Value
                });
            }
        }
    }
}