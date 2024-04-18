using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.PvWsOrder.Services.Models;

namespace Yahv.PvWsOrder.Services.Views.Origins
{
    /// <summary>
    /// 代收付款申请项视图
    /// </summary>
    public class Application_LogsOrigin : UniqueView<Application_Logs, PvWsOrderReponsitory>
    {
        public Application_LogsOrigin()
        {

        }

        public Application_LogsOrigin(PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Application_Logs> GetIQueryable()
        {
            var linq = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.Application_Logs>()
                       select new Application_Logs()
                       {
                           ID = entity.ID,
                           ApplicationID = entity.ApplicationID,
                           StepName = entity.StepName,
                           Status = (ApprovalStatus)entity.Status,
                           AdminID = entity.AdminID,
                           CreateDate = entity.CreateDate,
                           Summary = entity.Summary,
                       };
            return linq;
        }
    }
}
