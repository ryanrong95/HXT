//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.UI;
//using System.Web.UI.WebControls;

//namespace Yahv.Csrm.WebApp.Crm.WsConsignors
//{
//    public partial class Details : BasePage
//    {
//        protected void Page_Load(object sender, EventArgs e)
//        {
//            if (!IsPostBack)
//            {
//                var wsclient = Erp.Current.Whs.WsClients[Request.QueryString["clientid"]];
//                var wssupplier = wsclient.WsSuppliers[Request.QueryString["supplierid"]];
//                this.Model.Entity = wssupplier.Consignors[Request.QueryString["id"]];
//            }
//        }
//    }
//}