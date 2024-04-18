using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models;

namespace Yahv.Services.Views
{
    /// <summary>
    /// 运输工具视图
    /// </summary>
    /// <typeparam name="TReponsitory">连接对象</typeparam>
    public class TransportTopView<TReponsitory> : UniqueView<Transport, TReponsitory>
            where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public TransportTopView()
        {

        }
        public TransportTopView(TReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Transport> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.TransportsTopView>()
                   select new Transport
                   {
                       ID = entity.ID,
                       EnterpriseID = entity.EnterpriseID,
                       Type = (Yahv.Underly.VehicleType)entity.Type,
                       CarNumber1 = entity.CarNumber1,
                       CarNumber2 = entity.CarNumber2,
                       Weight = entity.Weight,
                       Status = (Yahv.Underly.GeneralStatus)entity.Status,
                   };

        }
    }

    /// <summary>
    /// 默认运输工具视图视图
    /// </summary>
    public class TransportTopView : TransportTopView<Layers.Data.Sqls.PvbCrmReponsitory>
    {

    }
}
