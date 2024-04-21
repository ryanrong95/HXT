using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
//using Yahv.Services.Views;

namespace Yahv.CrmPlus.Service.Views.Rolls
{
    public class LogsOperatingRoll : Yahv.Linq.UniqueView<LogOperating, PvdCrmReponsitory>
    {
        protected override IQueryable<LogOperating> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.LogsOperating>()
                   join admin in new YaHv.CrmPlus.Services.Views.Rolls.AdminsAllRoll(this.Reponsitory) on entity.OperatorID equals admin.ID
                   select new LogOperating
                   {
                       CreateDate = entity.CreateDate,
                       Context = entity.Context,
                       MainID = entity.MainID,
                       SubID = entity.SubID,
                       Admin = new YaHv.CrmPlus.Services.Models.Origins.Admin
                       {
                           ID = admin.ID,
                           RealName = admin.RealName,
                           UserName = admin.UserName,
                           RoleName = admin.RoleName,
                           StaffID = admin.StaffID,
                           Status = admin.Status,
                       }
                   };
        }
    }
}
