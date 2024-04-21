using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.Underly;
using YaHv.Csrm.Services.Models.Origins;

namespace Yahv.Csrm.WebApp.Crm.nSuppliers.nConsignors
{
    public partial class List : Web.Erp.ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.ClientID = Request.QueryString["clientid"];
                this.Model.nSupplierID = Request.QueryString["supplierid"];
            }
        }
        protected object data()
        {
            string id = Request.QueryString["id"];
            var wsclient = Erp.Current.Whs.WsClients[Request.QueryString["clientid"]];
            var nsupplier = wsclient.nSuppliers[Request.QueryString["supplierid"]];
            var consignor = nsupplier.nConsignors.Where(Predicate());
            return new
            {
                rows = consignor.ToArray().Select(item => new
                {
                    item.ID,
                    item.Address,
                    item.Postzip,
                    ContactName = item.Contact,
                    item.Tel,
                    item.Mobile,
                    item.Email,
                    item.Status,
                    item.Title,
                    IsDefault = item.IsDefault ? "是" : "否",
                    StatusName = item.Status.GetDescription(),
                    Place = Enum.GetValues(typeof(Origin)).Cast<Origin>().Any(i => i.GetOrigin().Code == item.Place) ? Enum.GetValues(typeof(Origin)).Cast<Origin>().SingleOrDefault(i => i.GetOrigin().Code == item.Place).GetOrigin()?.ChineseName : null,
                })
            };
        }
        Expression<Func<nConsignor, bool>> Predicate()
        {
            Expression<Func<nConsignor, bool>> predicate = item => true;
            var address = Request["address"];
            var contactname = Request["contactname"];
            var tel = Request["tel"];
            var status = Request["status"];
            if (!string.IsNullOrWhiteSpace(address))
            {
                predicate = predicate.And(item => item.Address.Contains(address));
            }
            if (!string.IsNullOrWhiteSpace(contactname))
            {
                predicate = predicate.And(item => item.Contact.Contains(contactname));
            }
            if (!string.IsNullOrWhiteSpace(tel))
            {
                predicate = predicate.And(item => item.Tel.Contains(tel) || item.Mobile.Contains(tel));
            }
            return predicate;
        }
        protected void del()
        {
            var wsclient = Erp.Current.Whs.WsClients[Request["clientid"]];
            var nsupllier = wsclient.nSuppliers[Request["supplierid"]];
            var entity = nsupllier.nConsignors[Request["id"]];
            entity.Abandon();
            //调用接口
            var api = System.Configuration.ConfigurationManager.AppSettings["UnifyApiUrl"];
            if (!string.IsNullOrWhiteSpace(api))
            {
                Unify(api, wsclient.Enterprise.Name, nsupllier.RealEnterprise.Name, entity.Address);
            }
        }

        object Unify(string api, string clientname, string suppliername, string address)
        {
            var response = HttpClientHelp.CommonHttpRequest(api + "/suppliers/address?name=" + clientname + "&supplierName=" + suppliername + "&address=" + address.Replace("中国 ", ""), "DELETE");
            return response;
        }
    }
}