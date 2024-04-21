using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.Payments.Views
{
    //public class CustomsInvoiceSynonymTopView : QueryView<RelatedInvoice, PvbCrmReponsitory>
    //{
    //    protected override IQueryable<RelatedInvoice> GetIQueryable()
    //    {
    //        return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.CustomsInvoiceSynonymTopView>()
    //               select new RelatedInvoice
    //               {
    //                   OrderID = entity.MainOrderID,
    //                   //InvoiceType = 1,
    //                   InvoiceDate = entity.InvoiceDate.Value,
    //                   LeftPrice = entity.LeftTotal,
    //                   RightPrice = entity.RightTotal,
    //                   ClientName = entity.Name
    //               };
    //    }
    //}

}
