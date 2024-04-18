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
    /// 租赁产品通用视图
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    public class LsProductTopView<TReponsitory> : UniqueView<LsProducts, TReponsitory>
         where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public LsProductTopView()
        {

        }

        public LsProductTopView(TReponsitory reponsitory) : base(reponsitory)
        {

        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<LsProducts> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvLsOrder.LsProductsTopView>()
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
                   };
        }
    }
}
