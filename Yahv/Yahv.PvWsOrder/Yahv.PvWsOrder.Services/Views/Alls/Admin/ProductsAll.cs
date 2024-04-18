using Layers.Data.Sqls;
using Layers.Data.Sqls.PvWsOrder;
using Layers.Linq;
using System;
using System.Linq;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.Views
{
    /// <summary>
    /// 标准产品
    /// </summary>
    public class ProductsAll : Yahv.Services.Views.ProductsTopView<PvWsOrderReponsitory>
    {
        public ProductsAll()
        {

        }

        public ProductsAll(PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Yahv.Services.Models.CenterProduct> GetIQueryable()
        {
            return base.GetIQueryable();
        }
    }
}
