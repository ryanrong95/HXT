using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Origins
{
    /// <summary>
    /// 运输工具
    /// </summary>
    public class TansportOrigin : Yahv.Linq.UniqueView<Transport, PvbCrmReponsitory>
    {
        internal TansportOrigin()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal TansportOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Transport> GetIQueryable()
        {
            var enterprisesView = new EnterprisesOrigin(this.Reponsitory);
            var adminsView = new Rolls.AdminsAllRoll(this.Reponsitory);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Transports>()
                   join enterprises in enterprisesView on entity.EnterpriseID equals enterprises.ID
                   join _admin in adminsView on entity.Creator equals _admin.ID into _admin
                   from admin in _admin.DefaultIfEmpty()
                   select new Transport
                   {
                       ID = entity.ID,
                       Enterprise = enterprises,
                       Type = (Yahv.Underly.VehicleType)entity.Type,
                       CarNumber1 = entity.CarNumber1,
                       CarNumber2 = entity.CarNumber2,
                       Weight = entity.Weight,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       Status = (Yahv.Underly.GeneralStatus)entity.Status,
                       CreatorID = entity.Creator,
                       Creator = admin,
                   };
        }
    }
}
