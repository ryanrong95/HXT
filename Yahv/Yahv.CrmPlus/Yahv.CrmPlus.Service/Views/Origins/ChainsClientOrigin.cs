using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Underly;
using YaHv.CrmPlus.Services.Views.Rolls;

namespace Yahv.CrmPlus.Service.Views.Origins
{
    public class ChainsClientOrigin : Yahv.Linq.UniqueView<ChainsClient, PvdCrmReponsitory>
    {
        internal ChainsClientOrigin()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal ChainsClientOrigin(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<ChainsClient> GetIQueryable()
        {
            var adminsView = new AdminsAllRoll(this.Reponsitory);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Chains>()
                   join enterprise in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Enterprises>() on entity.ID equals enterprise.ID
                   join reigters in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.EnterpriseRegisters>() on entity.ID equals reigters.ID
                   join owners in adminsView on entity.OwnerID equals owners.ID into _owner
                   from owner in _owner.DefaultIfEmpty()
                   join trackers in adminsView on entity.TrackerID equals trackers.ID into _trackers
                   from tracker in _trackers.DefaultIfEmpty()
                   join reffers in adminsView on entity.ReferrerID equals reffers.ID into _reffers
                   from referrer in _reffers.DefaultIfEmpty()
                   select new ChainsClient
                   {
                       ID = entity.ID,
                       CustomCode = entity.CustomCode,
                       Vip = entity.Vip,
                       WsCode = entity.WsCode,
                       Nature = (ClientType)entity.Nature,
                       ServiceType = (ServiceType)entity.ServiceType,
                       Grade = entity.Grade,
                       OwnerID = entity.OwnerID,
                       District = entity.District,
                       Owner = owner,
                       TrackerID = entity.TrackerID,
                       Tracker = tracker,
                       ReferrerID = entity.ReferrerID,
                       Referrer = referrer,
                       CreateDate = entity.CreateDate,
                       ModifyDate = entity.ModifyDate,
                       Status = (ApprovalStatus)entity.Status
                   };
        }
    }
}
