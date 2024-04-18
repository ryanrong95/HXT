using Layers.Data.Sqls.PvbCrm;
using System;
using System.Linq;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.Services.Views
{
    /// <summary>
    /// 库房视图
    /// </summary>
    public class WarehousesTopView<TReponsitory> : UniqueView<Warehouse, TReponsitory>
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
        protected override IQueryable<Warehouse> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<WarehousesTopView>()
                   select new Warehouse
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       WsCode = entity.WsCode,
                       Region = (Region)entity.District,
                       Address = entity.Address,
                       DyjCode = entity.DyjCode,
                       Grade = (WarehouseGrade)entity.Grade,
                       Corporation = entity.Corporation,
                       Uscc = entity.Uscc,
                       RegAddress = entity.RegAddress,
                       Status = (ApprovalStatus)entity.Status
                   };
        }
    }
}
