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
    /// <typeparam name="TReponsitory">连接类型</typeparam>
    /// <remarks>
    /// 需要补充逻辑
    /// 根据承运商获取司机、运载工具等
    /// </remarks>
    public class CarriersTopView<TReponsitory> : UniqueView<Carrier, TReponsitory>
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
                   where entity.Status == (int)Underly.GeneralStatus.Normal
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
                       Status = (Underly.GeneralStatus)entity.Status,
                       Place = entity.Place,
                       IsInternational = entity.IsInternational
                   };
        }
    }

    public class CarriersTopView : CarriersTopView<Layers.Data.Sqls.PvbCrmReponsitory>
    {

    }
}
