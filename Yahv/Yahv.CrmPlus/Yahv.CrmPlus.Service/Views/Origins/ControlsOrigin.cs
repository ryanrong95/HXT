using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;

namespace Yahv.CrmPlus.Service.Views.Origins
{
    /// <summary>
    /// 管控
    /// </summary>
    public class ControlsOrigin : Yahv.Linq.UniqueView<Control, PvdCrmReponsitory>
    {
        internal ControlsOrigin()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal ControlsOrigin(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Control> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Controls>()
                   select new Control
                   {
                       ID = entity.ID,
                       MainID = entity.MainID,
                       Type = (Underly.Enums.ControlType)entity.Type,
                       Context = entity.Context,
                       Status = (Underly.Enums.ControlStatus)entity.Status,
                       CreatorID = entity.CreatorID,
                       CreateDate = entity.CreateDate
                   };
        }
    }
}
