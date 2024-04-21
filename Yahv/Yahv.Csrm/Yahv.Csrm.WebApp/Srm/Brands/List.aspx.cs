using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using YaHv.Csrm.Services.Models.Origins;
using Yahv.Underly;

namespace Yahv.Csrm.WebApp.Srm.Brands
{
    public partial class List : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected object data()
        {
            using (var view = new YaHv.Csrm.Services.Views.Rolls.StandardBrandsRoll())
            {
                var query = view;
                var iquery = query.Search(this.GetPredicate());
                return this.Paging(iquery);

            }
        }

        /// <summary>
        /// 获取条件
        /// </summary>
        /// <returns></returns>
        Expression<Func<StandardBrand, bool>> GetPredicate()
        {
            Expression<Func<StandardBrand, bool>> predicate = item => true;
            
            var name = Request["s_name"];

            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Name.Contains(name) || item.ShortName.Contains(name));
            }

            return predicate;
        }


    }
}