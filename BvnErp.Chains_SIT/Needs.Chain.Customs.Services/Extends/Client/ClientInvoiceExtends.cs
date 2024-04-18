using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 
    /// </summary>
    public static class ClientInvoiceExtends
    {
        public static Layer.Data.Sqls.ScCustoms.ClientInvoices ToLinq(this Models.ClientInvoice entity)
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

        /// <summary>
        /// 客户发票 写入客户日志
        /// </summary>
        /// <param name="client"></param>
        /// <param name="summary"></param>
        public static void Log(this Models.ClientInvoice clientInvoice, Models.Client client, string summary)
        {
            ClientLog log = new ClientLog();
            log.ClientID = client.ID;
            log.Admin = client.Admin;
            log.ClientRank = client.ClientRank;
            log.Summary = summary;
            log.Enter();
        }
    }
}