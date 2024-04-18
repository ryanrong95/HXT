using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Models;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 产品归类变更日志的视图
    /// </summary>
    public class ProductClassifyLogsView : UniqueView<Models.ProductClassifyLog, ScCustomsReponsitory>
    {
        public ProductClassifyLogsView()
        {
        }

        internal ProductClassifyLogsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<ProductClassifyLog> GetIQueryable()
        {
            var adminsView = new AdminsTopView(this.Reponsitory);

            return from log in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ProductClassifyLogs>()
                   join admin in adminsView on log.AdminID equals admin.ID
                   orderby log.CreateDate descending
                   select new Models.ProductClassifyLog
                   {
                       ID = log.ID,
                       ClassifyProductID = log.ClassifyProductID,
                       Declarant = admin,
                       LogType = (Enums.LogTypeEnums)log.LogType,
                       OperationLog = log.OperationLog,
                       Stauts = (Enums.Status)log.Status,
                       CreateDate = log.CreateDate,
                       UpdateDate = log.UpdateDate,
                       Summary = log.Summary
                   };
        }
    }
}
