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
    /// 开票通知项
    /// </summary>
    public class InvoiceNoticeItemsOrigin : UniqueView<InvoiceNoticeItem, PvWsOrderReponsitory>
    {
        public InvoiceNoticeItemsOrigin()
        {

        }

        internal InvoiceNoticeItemsOrigin(PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<InvoiceNoticeItem> GetIQueryable()
        {
            var linq = from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.InvoiceNoticeItems>()
                       select new InvoiceNoticeItem
                       {
                           ID = entity.ID,
                           InvoiceNoticeID = entity.InvoiceNoticeID,
                           BillID = entity.BillID,
                           UnitPrice = entity.UnitPrice,
                           Quantity = entity.Quantity,
                           Amount = entity.Amount,
                           Difference = entity.Difference,
                           InvoiceNo = entity.InvoiceNo,
                           Status = (GeneralStatus)entity.Status,
                           CreateDate = entity.CreateDate,
                           Summary = entity.Summary,
                       };
            return linq;
        }
    }
}
