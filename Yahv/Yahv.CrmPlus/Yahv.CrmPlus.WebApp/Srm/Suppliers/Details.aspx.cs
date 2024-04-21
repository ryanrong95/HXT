using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Srm.Suppliers
{
    public partial class Details : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var supplier = Erp.Current.CrmPlus.Suppliers[Request.QueryString["id"]];
                this.Model.Entity = supplier;
                this.Model.LogoUrl = new FilesDescriptionRoll()[supplier.ID, CrmFileType.Logo].FirstOrDefault()?.Url;
                this.Model.Licenses = new FilesDescriptionRoll()[supplier.ID, CrmFileType.License].ToArray();
                this.Model.Qualifications = new FilesDescriptionRoll()[supplier.ID, CrmFileType.QualificationCertificate].ToArray();
            }
        }
    }
}