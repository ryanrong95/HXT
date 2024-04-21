using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Origins
{
    /// <summary>
    /// 承运商
    /// </summary>
    public class CarriersOrigin : Yahv.Linq.UniqueView<Carrier, PvbCrmReponsitory>
    {
        internal CarriersOrigin()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal CarriersOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Carrier> GetIQueryable()
        {
            var enterprisesView = new EnterprisesOrigin(this.Reponsitory);
            var adminsView = new Rolls.AdminsAllRoll(this.Reponsitory);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Carriers>()
                   join enterprises in enterprisesView on entity.ID equals enterprises.ID
                   join _admin in adminsView on entity.Creator equals _admin.ID into _admin
                   from admin in _admin.DefaultIfEmpty()

                   select new Carrier
                   {
                       ID = entity.ID,
                       Enterprise = enterprises,
                       Code = entity.Code,
                       Icon = entity.Icon,
                       Type = (Yahv.Underly.CarrierType)entity.Type,
                       Summary = entity.Summary,
                       CreatorID = entity.Creator,
                       Creator = admin,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       Status = (Yahv.Underly.GeneralStatus)entity.Status,
                       IsInternational = entity.IsInternational
                   };
        }
    }



}
