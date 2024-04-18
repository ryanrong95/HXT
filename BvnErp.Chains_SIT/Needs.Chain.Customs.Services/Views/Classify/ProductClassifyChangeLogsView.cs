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
    public class ProductClassifyChangeLogsView : UniqueView<Models.ProductClassifyChangeLog, ScCustomsReponsitory>
    {
        public ProductClassifyChangeLogsView()
        {
        }

        internal ProductClassifyChangeLogsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<ProductClassifyChangeLog> GetIQueryable()
        {
            var adminsView = new AdminsTopView(this.Reponsitory);

            return from log in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ProductClassifyChangeLogs>()
                   join admin in adminsView on log.AdminID equals admin.ID
                   orderby log.CreateDate descending
                   select new Models.ProductClassifyChangeLog
                   {
                       ID = log.ID,
                       Model = log.Model,
                       Manufacturer = log.Manufacturer,
                       Declarant = admin,
                       CreateDate = log.CreateDate,
                       Summary = log.Summary
                   };
        }
    }
}
