//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Net;
//using System.Text;
//using System.Web;
//using System.Web.UI;
//using System.Web.UI.WebControls;
//using Yahv.Linq.Extends;
//using Yahv.Underly;
//using Yahv.Utils.Serializers;
//using YaHv.Csrm.Services.Extends;
//using YaHv.Csrm.Services.Models.Origins;

//namespace Yahv.Csrm.WebApp.Crm.WsSuppliers
//{
//    public partial class Selected : BasePage
//    {
//        protected void Page_Load(object sender, EventArgs e)
//        {
//            this.Model.ClientID = Request.QueryString["id"];
//        }

//        protected object data()
//        {

//            var wsclient = Erp.Current.Whs.WsClients[Request.QueryString["id"]];
//            if (wsclient == null)
//            {
//                return null;
//            }
//            else
//            {
//                return new
//                {
//                    rows = wsclient.WsSuppliers.ToArray().Select(item => new
//                    {
//                        item.ID,
//                        Name = item.Enterprise.Name,
//                        item.ChineseName,
//                        item.EnglishName,
//                        item.Enterprise.AdminCode,
//                        item.Grade,
//                        item.Enterprise.District,
//                        Admin = item.Admin == null ? null : item.Admin.RealName,
//                        item.Enterprise.Uscc,
//                        item.Enterprise.Corporation,
//                        item.Enterprise.RegAddress,
//                        item.CreateDate,
//                        item.UpdateDate
//                    })
//                };
//            };
//        }

//        protected object DelMaps()
//        {
//            var wsclient = Erp.Current.Whs.WsClients[Request["ClientID"]];
//            var wssupplier = wsclient.WsSuppliers[Request["SupplierID"]];
//            if (wsclient == null || wssupplier == null)
//            {
//                return new { msg = "客户或客户不存在", success = false };
//            }
//            else
//            {
//                wssupplier.Abandon();
//                Unify("/clients/suppliers", wssupplier, wsclient, true);
//                return new { msg = "操作成功", success = true };
//            }
//        }
//        protected object CreateMaps()
//        {
//            var wsclient = Erp.Current.Whs.WsClients[Request["ClientID"]];
//            var wssupplier = wsclient.WsSuppliers[Request["SupplierID"]];
//            if (wsclient == null || wssupplier == null)
//            {
//                return new { msg = "客户或供应商不存在", success = false };
//            }
//            else
//            {
//                wssupplier.MapsSupplier(wssupplier.ID, Erp.Current.ID);
//                Unify("/clients/suppliers", wssupplier, wsclient);
//                return new { msg = "操作成功", success = true };
//            }
//        }


//        object Unify(string url, WsSupplier supplier, WsClient client, bool isdel = false)
//        {
//#pragma warning disable
//#if WHSL_1_0
//            if (isdel)
//            {
//                var response = new System.Net.Http.HttpClient().DeleteAsync(System.Configuration.ConfigurationManager.AppSettings["UnifyApiUrl"] + url + "?name=" + client.Enterprise.Name + "&supplierName=" + supplier.Enterprise.Name);
                
//            }
//            else
//            {
//                var response = HttpClientHelp.HttpPostRaw(System.Configuration.ConfigurationManager.AppSettings["UnifyApiUrl"] + url, new
//                {
//                    Enterprise = client.Enterprise,
//                    ChineseName = supplier.ChineseName,
//                    EnglishName = supplier.EnglishName,
//                    Summary = supplier.Summary
//                }.Json());
//                return response;
//            }

//#endif

//            return null;
//#pragma warning restore
//        }

        
     

 

//    }
//}