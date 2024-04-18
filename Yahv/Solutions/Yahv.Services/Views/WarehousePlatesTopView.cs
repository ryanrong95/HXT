using Layers.Data.Sqls.PvbCrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.Services.Views
{
    /// <summary>
    /// 库房视图
    /// </summary>
    public class WarehousePlatesTopView<TReponsitory> : UniqueView<WarehousePlate, TReponsitory>
            where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WarehousePlatesTopView()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public WarehousePlatesTopView(TReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<WarehousePlate> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<WarehousePlatesTopView>()
                   select new WarehousePlate
                   {
                       ID = entity.ID,
                       Title = entity.Title,
                       Code = entity.Code,
                       PostZip = entity.Postzip,
                       Address = entity.Address,
                       Status = (ApprovalStatus)entity.Status,
                       EnterpriseID = entity.EnterpriseID,
                       WarehouseName = entity.WarehouseName,
                       WarehouseAddress = entity.WarehouseAddress,
                       WsCode = entity.WsCode,
                       WareHouseStatus = (ApprovalStatus)entity.WarehouseStatus,
                   };
        }


    }
}
