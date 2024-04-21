using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Crm.nSuppliers.nPayees
{
    public partial class List : Web.Erp.ErpParticlePage
    {
        bool success = false;
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
            string ClientID = Request.QueryString["clientid"];
            string nSupplierID = Request.QueryString["supplierid"];
            var query = Erp.Current.Whs.WsClients[ClientID].nSuppliers[nSupplierID].nPayees;
            return this.Paging(query.OrderByDescending(item => item.ID), item => new
            {
                item.ID,
                Bank = item.Bank,
                BankAddress = item.BankAddress,
                Currency = item.Currency.GetDescription(),
                Methord = item.Methord.GetDescription(),
                Account = item.Account,
                SwiftCode = item.SwiftCode,
                Contact = item.Contact,
                Mobile = item.Mobile,
                Tel = item.Tel,
                Email = item.Email,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                UpdateDate = item.UpdateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Status = item.Status,
                StatusName = item.Status.GetDescription(),
                IsDefault = item.IsDefault ? "是" : "否",
                Place = Enum.GetValues(typeof(Origin)).Cast<Origin>().Any(i => i.GetOrigin().Code == item.Place) ? Enum.GetValues(typeof(Origin)).Cast<Origin>().SingleOrDefault(i => i.GetOrigin().Code == item.Place).GetOrigin()?.ChineseName : null,
            });
        }
        protected bool del()
        {
            string ClientID = Request["clientid"];
            string nSupplierID = Request["supplierid"];
            string id = Request["id"];
            var entity = Erp.Current.Whs.WsClients[ClientID].nSuppliers[nSupplierID].nPayees[id];
            entity.AbandonSuccess += Entity_AbandonSuccess;
            entity.Abandon();
            return success;
        }

        private void Entity_AbandonSuccess(object sender, Usually.AbandonedEventArgs e)
        {
            success = true;
            var payee = sender as nPayee;
            var api = System.Configuration.ConfigurationManager.AppSettings["UnifyApiUrl"];
            if (!string.IsNullOrWhiteSpace(api))
            {
                var clientname = new EnterprisesRoll()[payee.EnterpriseID].Name;
                Unify(api, clientname, payee.RealEnterprise.Name, payee.Account);
            }

        }
        //同步接口
        object Unify(string api, string clientname, string suppliername, string account)
        {
            var response = HttpClientHelp.CommonHttpRequest(api + "/suppliers/banks?name=" + clientname + "&supplierName=" + suppliername + "&account=" + account, "DELETE");
            return response;
        }
    }
}