using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Crm.Contacts
{
    public partial class Detail : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var contact = Erp.Current.CrmPlus.MyContacts[Request.QueryString["id"]];
                this.Model.Entity = contact;
                this.Model.Card = new FilesDescriptionRoll()[contact.EnterpriseID, contact.ID, CrmFileType.VisitingCard].FirstOrDefault();
            }
        }
    }
}