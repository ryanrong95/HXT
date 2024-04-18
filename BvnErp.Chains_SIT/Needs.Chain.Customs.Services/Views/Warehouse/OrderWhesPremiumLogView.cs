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
    /// <summary>
    /// 库房费用日志
    /// </summary>
    public class OrderWhesPremiumLogView : UniqueView<Models.OrderWhesPremiumLog, ScCustomsReponsitory>
    {
        public OrderWhesPremiumLogView()
        {
        }

        internal OrderWhesPremiumLogView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<OrderWhesPremiumLog> GetIQueryable()
        {
            var adminView = new AdminsTopView(this.Reponsitory);
            var result = from log in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderWhesPremiumLogs>()
                         join admin in adminView on log.AdminID equals admin.ID
                         select new Models.OrderWhesPremiumLog
                         {
                             ID = log.ID,
                             OrderID = log.OrderID,
                             OrderWhesPremiumID = log.OrderWhesPremiumID,
                             Admin = admin,
                             Type = (Enums.WarehousePremiumType)log.Type,
                             CreateDate = log.CreateDate,
                             Summary = log.Summary,
                         };
            return result;
        }
    }
}
