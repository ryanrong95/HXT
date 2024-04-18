using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.Services.Views
{
    /// <summary>
    /// Eccn 通用视图
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    public class EccnsTopView<TReponsitory> : UniqueView<Models.Eccn, TReponsitory> where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public EccnsTopView()
        {

        }
        public EccnsTopView(TReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Eccn> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvData.EccnsTopView>()
                   select new Models.Eccn
                   {
                       ID = entity.ID,
                       PartNumber = entity.PartNumber,
                       Manufacturer = entity.Manufacturer,
                       Code = entity.Code,
                       LastOrigin = entity.LastOrigin,
                       CreateDate = entity.CreateDate,
                       ModifyDate = entity.ModifyDate
                   };
        }
    }
}
