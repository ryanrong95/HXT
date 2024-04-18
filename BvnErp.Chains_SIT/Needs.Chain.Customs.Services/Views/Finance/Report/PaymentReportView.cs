using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class PaymentReportView : UniqueView<Views.PaymentReportViewModel, ScCustomsReponsitory>
    {
        protected override IQueryable<PaymentReportViewModel> GetIQueryable()
        {
            throw new NotImplementedException();
        }
    }

    public class PaymentReportViewModel : IUnique
    {
        public string ID { get; set; }
    }
}
