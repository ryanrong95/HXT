using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class SwapLimitCountryLogsView : UniqueView<Models.SwapLimitCountryLog, ScCustomsReponsitory>
    {
        public SwapLimitCountryLogsView()
        {
        }

        internal SwapLimitCountryLogsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<SwapLimitCountryLog> GetIQueryable()
        {
            var adminsView = new AdminsTopView(this.Reponsitory);
            return from Log in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapLimitCountryLogs>()
                   join admin in adminsView on Log.AdminID equals admin.ID into admins
                   from admin in admins.DefaultIfEmpty()
                   orderby Log.CreateDate
                   select new Models.SwapLimitCountryLog
                   {
                       ID = Log.ID,
                       BankID=Log.BankID,
                       Admin = admin,
                       CreateDate = Log.CreateDate,
                       Summary = Log.Summary
                   };
        }
    }
}
