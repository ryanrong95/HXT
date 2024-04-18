using Needs.Ccs.Services.Views;
using Needs.Erp.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Admin.Plat.Views
{
    /// <summary>
    /// 客户自定义产品税务归类的视图（Admin过滤）
    /// </summary>
    public class MyClientProductTaxCategoriesView : ClientProductTaxCategoriesAllsView
    {
        IGenericAdmin Admin;

        public MyClientProductTaxCategoriesView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.ClientProductTaxCategory> GetIQueryable()
        {
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable();
            }

            return from taxCategory in base.GetIQueryable()
                   where taxCategory.Client.Merchandiser.ID == this.Admin.ID &&
                   taxCategory.Status == Needs.Ccs.Services.Enums.Status.Normal
                   select taxCategory;
        }
    }
}
