using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Srm.AgentBrands
{
    public partial class List : BasePage
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
            string id = Request.QueryString["id"];
            var entity = new YaHv.Csrm.Services.Views.Rolls.nBrandsRoll(id);
            return this.Paging(entity, item => new
            {
                item.ID,
                item.BrandID,
                item.BrandName,
                item.ShortName,
                item.ChineseName,
                //item.EnterpriseName,
            });
        }


        bool AbandonSuccess = false;
        protected bool Del()
        {
            string id = Request["ID"];
            var entity = new nBrandsRoll()[id];
            entity.AbandonSuccess += Entity_AbandonSuccess;
            entity.Abandon();
            return AbandonSuccess;
        }

        private void Entity_AbandonSuccess(object sender, Usually.SuccessEventArgs e)
        {
            AbandonSuccess = true;
        }
    }
}