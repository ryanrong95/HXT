using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Srm.SupplierDetails.Commissions
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.Supplierid = Request.QueryString["supplierid"];
            }
        }
        protected object data()
        {
            string Supplierid = Request.QueryString["supplierid"];
            var query = new CommissionsRoll(Supplierid);
            return this.Paging(query.OrderBy(item => item.Msp).ToArray().Select(item => new
            {
                item.ID,
                item.Status,
                Currency = item.Currency.GetDescription(),
                StatusDes = item.Status.GetDescription(),
                item.Radio,
                item.Msp,
                Methord = item.Methord.GetDescription(),
                Type = item.Type.GetDescription()
            }));
        }
        bool success = false;
        protected bool Del()
        {
            string id = Request["id"];
            var entity = new CommissionsRoll()[id];
            entity.AbandonSuccess += Entity_AbandonSuccess;
            entity.Abandon();
            return success;
        }

        private void Entity_AbandonSuccess(object sender, Usually.SuccessEventArgs e)
        {
            success = true;
            var entity = sender as Commission;
            Service.LogsOperating.LogOperating(Erp.Current, entity.ID, $"删除Commissions:{entity.Json()}");
        }
    }
}