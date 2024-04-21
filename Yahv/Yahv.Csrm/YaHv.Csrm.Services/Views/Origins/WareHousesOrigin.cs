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
    public class WareHousesOrigin : Yahv.Linq.UniqueView<WareHouse, PvbCrmReponsitory>
    {
        internal WareHousesOrigin()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal WareHousesOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<WareHouse> GetIQueryable()
        {
            var enterprisesView = new EnterprisesOrigin(this.Reponsitory);
            var adminsView = new Rolls.AdminsAllRoll(this.Reponsitory);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.WareHouses>()
                   join enterprise in enterprisesView on entity.ID equals enterprise.ID
                   join admin in adminsView on entity.AdminID equals admin.ID
                   select new WareHouse
                   {
                       ID = entity.ID,
                       WsCode = entity.WsCode,
                       DyjCode = entity.DyjCode,
                       District = (Region)entity.District,
                       Grade = (WarehouseGrade)entity.Grade,
                       Address = entity.Address,
                       Enterprise = enterprise,
                       CreatorID = entity.AdminID,
                       Creator = admin,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       Status = (ApprovalStatus)entity.Status

                   };
        }
    }
}
