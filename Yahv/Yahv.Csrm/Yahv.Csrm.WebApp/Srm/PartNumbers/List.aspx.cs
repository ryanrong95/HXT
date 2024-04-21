using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using YaHv.Csrm.Services.Extends;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Srm.PartNumbers
{
    public partial class List : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected object data()
        {
            var entity = new YaHv.Csrm.Services.Views.Rolls.PartNumbersRoll().Where(Predicate());

            return this.Paging(entity, item => new
            {
                item.ID,
                item.Name,
                item.Manufacturer
            });
        }
        Expression<Func<PartNumber, bool>> Predicate()
        {
            Expression<Func<PartNumber, bool>> predicate = item => true;

            var name = Request["s_name"];
            var maf = Request["s_maf"];
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Name.Contains(name) || item.Manufacturer.Contains(name));
            }
            if (!string.IsNullOrWhiteSpace(maf))
            {
                predicate = predicate.And(item => item.Manufacturer.Contains(name));
            }
            return predicate;
        }
        int delCount = 0;
        protected int del()
        {
            var ids = Request.Form["items"].Split(',');
            var pns = new YaHv.Csrm.Services.Views.Rolls.PartNumbersRoll();
            foreach (var item in ids)
            {
                var entity = pns[item];
                entity.AbandonSuccess += Entity_AbandonSuccess1;
                entity.Abandon();
            }
            return delCount;
        }

        private void Entity_AbandonSuccess1(object sender, Usually.SuccessEventArgs e)
        {
            delCount++;
        }
        

    }
}