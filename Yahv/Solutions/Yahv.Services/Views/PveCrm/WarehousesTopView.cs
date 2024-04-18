using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models.PveCrm;
using Yahv.Underly;

namespace Yahv.Services.Views.PveCrm
{
    /// <summary>
    /// 库房通用视图
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    public class WarehousesTopView<TReponsitory> : UniqueView<WareHouse, TReponsitory>
            where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WarehousesTopView()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public WarehousesTopView(TReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<WareHouse> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PveCrm.WarehousesTopView>()
                   join region in this.Reponsitory.ReadTable<Layers.Data.Sqls.PveCrm.EnumsDictionariesTopView>() on entity.District equals region.ID
                   select new WareHouse
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       WsCode = entity.WsCode,
                       Region = entity.District,
                       RegionDes = region.Description,
                       Address = entity.Address,
                       DyjCode = entity.DyjCode,
                       Grade = (WarehouseGrade)entity.Grade,
                       Corporation = entity.Corporation,
                       Uscc = entity.Uscc,
                       RegAddress = entity.RegAddress,
                   };
        }
    }
}
