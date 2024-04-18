using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.Views.Origins
{
    /// <summary>
    /// 代收付款申请项视图
    /// </summary>
    public class ApplicationItemsOrigin : UniqueView<ApplicationItem, PvWsOrderReponsitory>
    {
        public ApplicationItemsOrigin()
        {

        }

        public ApplicationItemsOrigin(PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<ApplicationItem> GetIQueryable()
        {
            var linq = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.ApplicationItems>()
                       select new ApplicationItem()
                       {
                           ID = entity.ID,
                           ApplicationID = entity.ApplicationID,
                           OrderID = entity.OrderID,
                           Amount = entity.Amount,
                           Status = (GeneralStatus)entity.Status,
                       };
            return linq;
        }
    }
}
