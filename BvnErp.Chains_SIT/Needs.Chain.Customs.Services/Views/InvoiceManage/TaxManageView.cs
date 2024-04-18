using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class TaxManageView : View<Needs.Ccs.Services.Views.TaxManageView.TaxManageViewModels, ScCustomsReponsitory>
    {
        public class TaxManageViewModels : IUnique
        {
            public string ID { get; set; }
            public string InvoiceCode { get; set; }
            public string InvoiceNo { get; set; }
            public DateTime? InvoiceDate { get; set; }
            public string SellsName { get; set; }
            public decimal Amount { get; set; }
            public decimal? VaildAmount { get; set; }
            public DateTime? ConfrimDate { get; set; }
            public InvoiceVaildStatus InvoiceStatus { get; set; }
            public string InvoiceDetailID { get; set; }
            public BusinessType BusinessType { get; set; }
            public InvoiceType InvoiceType { get; set; }
            public DateTime? AuthenticationMonth { get; set; }
        }

        protected override IQueryable<TaxManageViewModels> GetIQueryable()
        {
            var ressult = from c in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.TaxManage>()
                              where c.Status == (int)Enums.Status.Normal
                          select new TaxManageViewModels
                          {
                             InvoiceCode = c.InvoiceCode,
                             InvoiceNo = c.InvoiceNo,
                             InvoiceDate = c.InvoiceDate,
                             SellsName = c.SellsName,
                             Amount = c.Amount,
                             VaildAmount = c.VaildAmount,
                             ConfrimDate = c.ConfrimDate,
                             InvoiceDetailID = c.InvoiceDetailID,
                             InvoiceStatus = (InvoiceVaildStatus)c.IsVaild,
                             BusinessType = (BusinessType)c.BusinessType,
                             InvoiceType = (InvoiceType)c.InvoiceType,
                             AuthenticationMonth = c.AuthenticationMonth
                          };

            return ressult;
        }
    }
}
