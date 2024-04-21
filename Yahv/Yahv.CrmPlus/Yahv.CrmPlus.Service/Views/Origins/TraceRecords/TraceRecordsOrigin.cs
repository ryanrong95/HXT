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
    public class TraceRecordsOrigin : Yahv.Linq.UniqueView<TraceRecord, PvdCrmReponsitory>
    {

        internal TraceRecordsOrigin()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal TraceRecordsOrigin(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<TraceRecord> GetIQueryable()
        {
            var enterpriseView = new EnterprisesOrigin(this.Reponsitory);
            var adminsView = new AdminsAllRoll(this.Reponsitory);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.TraceRecords>()
                   join enterprise in enterpriseView on entity.ClientID equals enterprise.ID
                   join admin in adminsView on entity.OwnerID equals admin.ID
                   select new TraceRecord
                   {
                       ID = entity.ID,
                       ClientID = entity.ClientID,
                       Enterprise = enterprise,
                       ProjectID = entity.ProjectID,
                       FollowWay = (FollowWay)entity.Type,
                       TraceDate = entity.TraceDate,
                       SupplierStaffs = entity.SupplierStaffs,
                       CompanyStaffs = entity.CompanyStaffs,
                       Context = entity.Context,
                       NextDate = entity.NextDate,
                       NextPlan = entity.NextPlan,
                       ClientContactID = entity.ClientContactID,
                       OwnerID = entity.OwnerID,
                       Owner = admin,
                       CreateDate = entity.CreateDate,
                       ModifyDate = entity.ModifyDate

                   };
        }
    }
}
