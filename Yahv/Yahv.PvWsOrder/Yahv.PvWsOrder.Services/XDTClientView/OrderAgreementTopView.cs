using Layers.Data.Sqls;
using System.Linq;
using Yahv.Linq;

namespace Yahv.PvWsOrder.Services.XDTClientView
{
    public class OrderAgreementTopView : QueryView<object, ScCustomReponsitory>
    {
        public OrderAgreementTopView()
        {

        }

        protected override IQueryable<object> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.OrderAgreementTopView>()
                   select new
                   {
                       entity.OrderID,
                       InvoiceType = (XDTInvoiceType)(entity.InvoiceType ?? 0),
                   };
        }

        public XDTInvoiceType GetTypeByOrderID(string OrderID)
        {
            var type = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.OrderAgreementTopView>()
                .FirstOrDefault(item => item.OrderID == OrderID)?.InvoiceType;

            return (XDTInvoiceType)(type ?? 0);
        }

    }
}
