//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.UI;
//using System.Web.UI.WebControls;

//namespace Yahv.Csrm.WebApp.Crm.WsSuppliers
//{
//    public partial class Details : BasePage
//    {
//        protected void Page_Load(object sender, EventArgs e)
//        {
//            if (!IsPostBack)
//            {
//                var supplierid = Request.QueryString["id"];
//                string clientid = Request.QueryString["clientid"];
//                this.Model.Entity = Erp.Current.Whs.WsClients[clientid].WsSuppliers[supplierid];
//            }
//        }
//    }
//}