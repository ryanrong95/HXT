using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class ClientLogsView : UniqueView<Models.ClientLog, ScCustomsReponsitory>
    {
        public ClientLogsView()
        {
        }

        protected ClientLogsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected ClientLogsView(ScCustomsReponsitory reponsitory, IQueryable<Models.ClientLog> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<Models.ClientLog> GetIQueryable()
        {
            var adminsTopView = new Views.AdminsTopView(this.Reponsitory);
            return from log in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientLogs>()
                   join admin in adminsTopView on log.AdminID equals admin.ID
                   select new Models.ClientLog
                   {
                       ID = log.ID,
                       ClientID = log.ClientID,
                       Admin = admin,
                       ClientRank=(Enums.ClientRank)log.ClientRank,
                       CreateDate = log.CreateDate,
                       Summary = log.Summary
                   };
        }

    }
}
