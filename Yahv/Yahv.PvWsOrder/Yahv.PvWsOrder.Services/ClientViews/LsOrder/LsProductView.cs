using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Services.Models.LsOrder;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    /// <summary>
    /// 租赁产品价格通用视图
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    public class LsProductView: UniqueView<LsProducts, PvLsOrderReponsitory>
    {
        public LsProductView()
        {

        }

        public LsProductView(PvLsOrderReponsitory reponsitory) : base(reponsitory)
        {

        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<LsProducts> GetIQueryable()
        {
            var priceView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvLsOrder.LsProductsPricesTopView>().Select(item => new LsProductPrices
            {
                ID = item.ID,
                ProductID = item.ProductID,
                Month = item.Month,
                Price = item.Price,
                Currency = (Currency)item.Currency,
                CreateDate = item.CreateDate,
            });
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvLsOrder.LsProductsTopView>()
                   join price in priceView on entity.ID equals price.ProductID into prices
                   where entity.Quantity > 0
                   select new LsProducts
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       SpecID = entity.SpecID,
                       Load = entity.Load,
                       Quantity = entity.Quantity,
                       Volume = entity.Volume,
                       CreateDate = entity.CreateDate,
                       ModifyDate = entity.ModifyDate,
                       Summary = entity.Summary,
                       LsProductPrice = prices.ToArray(),
                   };
        }
    }
}
