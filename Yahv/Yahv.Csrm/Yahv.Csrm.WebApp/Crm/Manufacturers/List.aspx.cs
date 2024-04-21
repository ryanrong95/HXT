using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using YaHv.Csrm.Services.Models.Origins;

namespace Yahv.Csrm.WebApp.Crm.Manufacturers
{
    public partial class List : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected object data()
        {
            var entity = new YaHv.Csrm.Services.Views.Rolls.ManufacturersRoll().Where(Predicate());

            return this.Paging(entity, item => new
            {
                item.ID,
                item.Name,
                item.Agent,
                IsAgent = item.Agent ? "是" : "否"
            });
        }
        Expression<Func<Manufacturer, bool>> Predicate()
        {
            Expression<Func<Manufacturer, bool>> predicate = item => true;

            var name = Request["s_name"];

            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Name.Contains(name));
            }

            return predicate;
        }
        int delCount = 0;
        protected int del()
        {
            var ids = Request.Form["items"].Split(',');
            var mafs = new YaHv.Csrm.Services.Views.Rolls.ManufacturersRoll();
            foreach (var item in ids)
            {
                var entity = mafs[item];
                entity.AbandonSuccess += Entity_AbandonSuccess;
                entity.Abandon();
            }
            return delCount;
        }

        private void Entity_AbandonSuccess(object sender, Usually.SuccessEventArgs e)
        {
            delCount++;
        }
        bool success = false;
        protected bool Agent()
        {
            string id = Request["id"];
            bool agent = bool.Parse(Request["IsAgent"]);
            var maf = new YaHv.Csrm.Services.Views.Rolls.ManufacturersRoll()[id];
            maf.Agent = agent;
            maf.EnterSuccess += Maf_EnterSuccess;
            maf.Enter();
            return success;
        }

        private void Maf_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            success = true;
        }
    }
}