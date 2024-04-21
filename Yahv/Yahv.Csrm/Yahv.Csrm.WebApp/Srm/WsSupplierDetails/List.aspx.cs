//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.UI;
//using System.Web.UI.WebControls;

//namespace Yahv.Csrm.WebApp.Srm.WsSupplierDetails
//{
//    public partial class List : BasePage
//    {
//        protected void Page_Load(object sender, EventArgs e)
//        {
//            if (!IsPostBack)
//            {
//                var supplierid = Request.QueryString["id"];
//                this.Model = Erp.Current.Srm.WsSuppliers[supplierid];
//            }
//        }
//    }
//}