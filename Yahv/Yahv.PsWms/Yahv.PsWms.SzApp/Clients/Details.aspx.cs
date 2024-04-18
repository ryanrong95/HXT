using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.PsWms.SzMvc.Services.Models.Origin;
using Yahv.PsWms.SzMvc.Services.Views.Roll;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PsWms.SzApp.Clients
{
    public partial class Details : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <returns></returns>
        protected void LoadData()
        {
            string ClientID = Request.QueryString["ID"];

            var client = new SzMvc.Services.Views.Origins.ClientsOrigin().SingleOrDefault(t => t.ID == ClientID);
            var site = new SzMvc.Services.Views.Origins.SiteusersOrigin().SingleOrDefault(t => t.ID == client.SiteuserID);
            var invoice = new SzMvc.Services.Views.Origins.InvoicesOrigin().SingleOrDefault(t => t.ClientID == ClientID);
            var address = new SzMvc.Services.Views.Origins.AddressesOrigin().Where(t => t.Status == Underly.GeneralStatus.Normal)
                .Where(t => t.ClientID == ClientID).ToArray().OrderBy(t => t.TypeDec);

            this.Model.clientData = new
            {
                Client = client,
                Site = site,
                Invoice = invoice,
                Address = address,
            };
        }

        protected object data()
        {
            return new
            {
                rows = new List<object>(),
                total = 0,
            }.Json();
        }
    }
}