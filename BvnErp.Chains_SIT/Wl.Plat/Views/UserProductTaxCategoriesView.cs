using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Wl.User.Plat.Models;

namespace Needs.Wl.User.Plat.Views
{
    /// <summary>
    /// 自定义产品税号
    /// </summary>
    public class UserProductTaxCategoriesView : Needs.Ccs.Services.Views.ClientProductTaxCategoriesAllsView
    {
        IPlatUser user;

        public UserProductTaxCategoriesView(IPlatUser user)
        {
            this.user = user;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.ClientProductTaxCategory> GetIQueryable()
        {
            return from item in base.GetIQueryable()
                   where item.Client.ID == this.user.Client.ID && item.Status == Needs.Ccs.Services.Enums.Status.Normal
                   select item;
        }
    }
}
