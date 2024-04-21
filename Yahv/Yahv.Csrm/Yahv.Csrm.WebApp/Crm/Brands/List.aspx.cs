using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using YaHv.Csrm.Services.Models.Origins;

namespace Yahv.Csrm.WebApp.Crm.Brands
{
    public partial class List : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected object data()
        {
            var entity = new YaHv.Csrm.Services.Views.Rolls.BrandsRoll().Where(Predicate());

            return this.Paging(entity, item => new
            {
                item.ID,
                item.Name
            });
        }
        Expression<Func<Brand, bool>> Predicate()
        {
            Expression<Func<Brand, bool>> predicate = item => true;

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
            var mafs = new YaHv.Csrm.Services.Views.Rolls.BrandsRoll();
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
    }
}