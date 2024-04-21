using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;

namespace Yahv.Csrm.WebApp.Crm.nSuppliers.nContacts
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
            var query = Erp.Current.Whs.WsClients[ClientID].nSuppliers[nSupplierID].nContacts;
            return this.Paging(query.OrderByDescending(item => item.ID), item => new
            {
                item.ID,
                item.Name,
                item.Mobile,
                item.Tel,
                item.Email,
                item.QQ,
                item.Fax,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                UpdateDate = item.UpdateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Status = item.Status,
                StatusName = item.Status.GetDescription()
            });
        }
        protected bool del()
        {
            string ClientID = Request["clientid"];
            string nSupplierID = Request["supplierid"];
            string id = Request["id"];
            var entity = Erp.Current.Whs.WsClients[ClientID].nSuppliers[nSupplierID].nContacts[id];
            entity.AbandonSuccess += Entity_AbandonSuccess;
            entity.Abandon();
            return success;
        }

        private void Entity_AbandonSuccess(object sender, Usually.SuccessEventArgs e)
        {
            success = true;
        }
    }
}