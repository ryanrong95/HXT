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

namespace Yahv.CrmPlus.WebApp.Srm.SupplierDetails.Specials
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.EnterpriseID = Request.QueryString["id"];
            }
        }
        protected object data()
        {
            string enterpriseid = Request.QueryString["id"];
            var query = new SpecialsRoll(enterpriseid);
            return this.Paging(query.OrderByDescending(item => item.CreateDate).ToArray().Select(item => new
            {
                item.ID,
                item.Status,
                Type = item.Type.GetDescription(),
                item.Brand,
                item.PartNumber,
                StatusDes = item.Status.GetDescription(),
                Summary = item.Summary,
                CreteDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")
            }));

        }
        bool success = false;
        protected bool del()
        {
            string id = Request["id"];
            var entity = new SpecialsRoll()[id];
            entity.AbandonSuccess += Entity_AbandonSuccess;
            entity.Abandon();
            return success;
        }

        private void Entity_AbandonSuccess(object sender, Usually.SuccessEventArgs e)
        {
            success = true;
            var entity = sender as Special;
            Service.LogsOperating.LogOperating(Erp.Current, entity.ID, $"删除Specials:{entity.Json()}");
        }
    }
}