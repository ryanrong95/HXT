using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Srm.SupplierDetails.Specials
{
    public partial class Detail : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string id = Request.QueryString["subid"];
                var entity = new SpecialsRoll()[id];
                this.Model.Entity = entity;
                //this.Model.Files = new FilesDescriptionRoll()[entity.EnterpriseID, entity.ID, CrmFileType.SpecialBrands].ToArray();
            }
        }
    }
}