using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models.LsOrder;
using Yahv.Underly;

namespace Yahv.Services.Views
{
    /// <summary>
    /// 租赁产品价格通用视图
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    public class LsProductPriceTopView<TReponsitory> : UniqueView<LsProductPrices, TReponsitory>
         where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public LsProductPriceTopView()
        {

        }

        public LsProductPriceTopView(TReponsitory reponsitory) : base(reponsitory)
        {

        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<LsProductPrices> GetIQueryable()
        {
            var products = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvLsOrder.LsProductsTopView>();

            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvLsOrder.LsProductsPricesTopView>()
                   join product in products on entity.ProductID equals product.ID
                   select new LsProductPrices
                   {
                       ID = entity.ID,
                       Month = entity.Month,
                       Currency = (Currency)entity.Currency,
                       CreateDate = entity.CreateDate,
                       Price = entity.Price,
                       ProductID = product.ID,
                       Creator = entity.Creator,
                       Summary = entity.Summary,
                       LsProduct = new LsProducts()
                       {
                           ID = product.ID,
                           SpecID = product.SpecID,
                           Name = product.Name,
                           Load = product.Load,
                           Volume = product.Volume,
                           Quantity = product.Quantity,
                           CreateDate = product.CreateDate,
                           Summary = product.Summary,
                           ModifyDate = product.ModifyDate,
                       },
                   };
        }
    }
}
