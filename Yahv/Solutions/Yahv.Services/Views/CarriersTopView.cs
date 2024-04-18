using System;
using System.Collections.Generic;
using System.Linq;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Utils.Converters.Contents;

namespace Yahv.Services.Views
{
    /// <summary>
    /// 承运商通用视图
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    public class CarriersTopView  <TReponsitory> : UniqueView<Carrier, TReponsitory>
            where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public CarriersTopView()
        {

        }
        public CarriersTopView(TReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Carrier> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.CarriersTopView>()
                   select new Carrier
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       Code = entity.Code,
                       Icon = entity.Icon,
                       District = entity.District,
                       Corporation = entity.Corporation,
                       Uscc = entity.Uscc,
                       RegAddress = entity.RegAddress,
                       Creator = entity.Creator,
                   };
        }
    }
}
