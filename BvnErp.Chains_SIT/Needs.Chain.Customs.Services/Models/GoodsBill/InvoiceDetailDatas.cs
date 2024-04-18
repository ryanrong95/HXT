using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class InvoiceDetailDatas : BaseItems<InvoiceDetailData>
    {
        internal InvoiceDetailDatas(IEnumerable<InvoiceDetailData> enums) : base(enums)
        {
        }

        internal InvoiceDetailDatas(IEnumerable<InvoiceDetailData> enums, Action<InvoiceDetailData> action) : base(enums, action)
        {
        }

        public override void Add(InvoiceDetailData item)
        {
            base.Add(item);
        }

        public override void RemoveAll()
        {
            base.RemoveAll();
        }
    }
}
