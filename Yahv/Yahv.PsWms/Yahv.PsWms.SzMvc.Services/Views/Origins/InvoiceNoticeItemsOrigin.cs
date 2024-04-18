using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PsWms.SzMvc.Services.Models.Origin;
using Yahv.Underly;

namespace Yahv.PsWms.SzMvc.Services.Views.Origins
{
    public class InvoiceNoticeItemsOrigin : UniqueView<InvoiceNoticeItem, Layers.Data.Sqls.PsOrderRepository>
    {
        public InvoiceNoticeItemsOrigin()
        {

        }

        internal InvoiceNoticeItemsOrigin(Layers.Data.Sqls.PsOrderRepository reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<InvoiceNoticeItem> GetIQueryable()
        {
            var linq = from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsOrder.SzInvoiceNoticeItemsTopView>()
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
