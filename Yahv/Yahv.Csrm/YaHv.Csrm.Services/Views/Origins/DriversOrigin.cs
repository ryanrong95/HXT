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
    /// 司机
    /// </summary>
    public class DriversOrigin : Yahv.Linq.UniqueView<Driver, PvbCrmReponsitory>
    {
        internal DriversOrigin()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal DriversOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Driver> GetIQueryable()
        {
            var enterprisesView = new EnterprisesOrigin(this.Reponsitory);
            var adminsView = new Rolls.AdminsAllRoll(this.Reponsitory);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Drivers>()
                   join enterprises in enterprisesView on entity.EnterpriseID equals enterprises.ID
                   join _admin in adminsView on entity.Creator equals _admin.ID into _admin
                   from admin in _admin.DefaultIfEmpty()

                   select new Driver
                   {
                       ID = entity.ID,
                       Enterprise = enterprises,
                       Name = entity.Name,
                       IDCard = entity.IDCard,
                       Mobile = entity.Mobile,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       Status = (Yahv.Underly.GeneralStatus)entity.Status,
                       CreatorID = admin.ID,
                       Creator = admin,

                       Mobile2 = entity.Mobile2,
                       CustomsCode = entity.CustomsCode,
                       CardCode = entity.CardCode,
                       LBPassword = entity.LBPassword,
                       PortCode = entity.PortCode,

                       IsChcd = entity.IsChcd//是否中港贸易
                   };
        }
    }
}
