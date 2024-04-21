using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.Payments.Views
{
   
    /// <summary>
    /// 应开的服务费发票或 全额发票
    /// </summary>
    public class OrderInvoiceSynonymTopView : QueryView<RelatedInvoice, PvbCrmReponsitory>
    {
        class MyClass
        {
            public object[] fwffp { get; set; }
            public object[] hgfp { get; set; }
        }

        protected override IQueryable<RelatedInvoice> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.OrderInvoicesTopView>()
                   select new RelatedInvoice
                   {
                       OrderID = entity.MainOrderID,
                       InvoiceType = (InvoiceType)entity.InvoiceType,
                       LeftPrice = entity.LeftPrice,
                       ClientName = entity.Name
                       //InvoiceDate = entity.InvoiceDate,
                       //InvoiceNo = entity.InvoiceNo,
                       //LeftPrice = entity.Amount,
                       //Difference = entity.Difference,
                       //RightPrice = entity.Amount - entity.Difference,

                   };
        }
    }

    /// <summary>
    /// 海关发票
    /// </summary>
    /// <remarks>
    /// 一个视图
    /// </remarks>
    public class CustomsInvoiceSynonymTopView : QueryView<RelatedInvoice, PvbCrmReponsitory>
    {
        protected override IQueryable<RelatedInvoice> GetIQueryable()
        {
            return from cutoms in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.CustomsInvoiceSynonymTopView>()
                   select new RelatedInvoice
                   {
                       OrderID = cutoms.MainOrderID,
                       InvoiceDate = cutoms.InvoiceDate.Value,
                       LeftPrice = cutoms.LeftTotal,
                       RightPrice = cutoms.RightTotal,
                       ClientName = cutoms.Name,
                   };
        }
    }

    /// <summary>
    /// 应开的增值税发票
    /// </summary>
    /// <remarks>
    /// 一个视图
    /// </remarks>
    public class VATInvoice : QueryView<RelatedInvoice, PvbCrmReponsitory>
    {
    protected override IQueryable<RelatedInvoice> GetIQueryable()
    {
        return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.OrderInvoicesTopView>()
               where entity.InvoiceType == (int)InvoiceType.VATInvoice
               select new RelatedInvoice
               {
                   ClientName = entity.Name,
                   OrderID = entity.MainOrderID,
                   InvoiceType = (InvoiceType)entity.InvoiceType,
                   LeftPrice = entity.LeftPrice,
                   //InvoiceDate = entity.InvoiceDate,
                   //InvoiceNo = entity.InvoiceNo,
                   //Difference = entity.Difference,
                   //RightPrice = entity.Amount - entity.Difference,
               };
    }
    }

    /// <summary>
    /// 应开的服务费发票
    /// </summary>
    public class ServiceInvoice : QueryView<RelatedInvoice, PvbCrmReponsitory>
    {
        protected override IQueryable<RelatedInvoice> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.OrderInvoicesTopView>()
                   where entity.InvoiceType == (int)InvoiceType.Servicing
                   select new RelatedInvoice
                   {
                       ClientName = entity.Name,
                       OrderID = entity.MainOrderID,
                       InvoiceType = (InvoiceType)entity.InvoiceType,
                       LeftPrice = entity.LeftPrice
                       //InvoiceDate = entity.InvoiceDate,
                       //InvoiceNo = entity.InvoiceNo,
                       //Difference = entity.Difference,
                       //RightPrice = entity.Amount - entity.Difference,
                   };
        }
    }

    /// <summary>
    /// 实开的
    /// </summary>
    public class OpenedServiceInvoice : QueryView<RelatedInvoice, PvbCrmReponsitory>
    {
        protected override IQueryable<RelatedInvoice> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.OpenedOrderInvoicesTopView>()
                   select new RelatedInvoice
                   {
                       ClientName = entity.Name,
                       OrderID = entity.OrderID,
                       InvoiceType = (InvoiceType)entity.InvoiceType,
                       InvoiceDate = entity.UpdateDate,
                       InvoiceNo = entity.InvoiceNo,
                       Difference = entity.Difference,
                       RightPrice = entity.Amount,
                   };
        }
    }

}
