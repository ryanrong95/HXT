using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class CustomsInvoiceView : View<Needs.Ccs.Services.Views.CustomsInvoiceView.CustomsInvoiceViewModels, ScCustomsReponsitory>
    {
        public class CustomsInvoiceViewModels :IUnique
        {
            public string ID { get; set; }
            /// <summary>
            /// 海关票日期
            /// </summary>
            public DateTime? CustomsInvoiceDate { get; set; }
            /// <summary>
            /// 海关缴款书号码
            /// </summary>
            public string TaxNumber { get; set; }
            /// <summary>
            /// 税额
            /// </summary>
            public decimal Amount { get; set; }
            /// <summary>
            /// 有效税额
            /// </summary>
            public decimal? VaildAmount { get; set; }
            /// <summary>
            /// 所属月份
            /// </summary>
            public DateTime? DeductionMonth { get; set; }
        }

        protected override IQueryable<CustomsInvoiceViewModels> GetIQueryable()
        {
            var ressult = from c in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxFlows>()
                          //where c.Status == (int)Enums.Status.Normal
                          select new CustomsInvoiceViewModels
                          {
                              CustomsInvoiceDate = c.PayDate,
                              TaxNumber = c.TaxNumber,
                              Amount = c.Amount,
                              VaildAmount = c.VaildAmount,
                              DeductionMonth = c.DeductionMonth
                          };

            return ressult;
        }
    }
}
