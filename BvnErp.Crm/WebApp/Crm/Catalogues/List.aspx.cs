using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.Catalogues
{
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {

            }
        }

        protected void data()
        {
            string ID = Request.QueryString["ID"];
            var catalogues = Needs.Erp.ErpPlot.Current.ClientSolutions.Catelogues.Where(item => true);
            if (!string.IsNullOrWhiteSpace(ID))
            {
                var data = catalogues.Where(item => item.ID == ID);
            }

            this.Paging(catalogues);
        }
    }
}