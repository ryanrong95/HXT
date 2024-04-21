using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Srm.SupplierDetails.Contacts
{
    public partial class Detail : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //subid为了迎合PcFile.ascx控件
                var contact = Erp.Current.CrmPlus.MyContacts[Request.QueryString["subid"]];
                this.Model.Entity = contact;
                //this.Model.Card = new FilesDescriptionRoll()[contact.EnterpriseID, contact.ID, CrmFileType.VisitingCard].FirstOrDefault();
            }
        }
    }
}