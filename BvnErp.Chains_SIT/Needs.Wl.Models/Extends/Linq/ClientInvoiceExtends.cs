using System;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 
    /// </summary>
    public static class ClientInvoiceExtends
    {
        public static Layer.Data.Sqls.ScCustoms.ClientInvoices ToLinq(this ClientInvoice entity)
        {
            return new Layer.Data.Sqls.ScCustoms.ClientInvoices
            {
                ID = entity.ID,
                ClientID = entity.ClientID,
                Title = entity.Title,
                TaxCode = entity.TaxCode,
                Address = entity.Address,
                Tel = entity.Tel,
                BankName = entity.BankName,
                BankAccount = entity.BankAccount,
                DeliveryType = (int)entity.DeliveryType,
                InvoiceStatus = (int)entity.InvoiceStatus,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = DateTime.Now,
                Summary = entity.Summary
            };
        }
    }
}