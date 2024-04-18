using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 开票通知扩展方法
    /// </summary>
    public static class InvoiceNoticeExtends
    {
        public static Layer.Data.Sqls.ScCustoms.InvoiceNotices ToLinq(this Models.InvoiceNotice entity)
        {
            return new Layer.Data.Sqls.ScCustoms.InvoiceNotices
            {
                ID = entity.ID,
                ApplyID = entity.Apply.ID,
                AdminID = entity.Admin?.ID,
                ClientID = entity.Client.ID,
                ClientInvoiceID = entity.ClientInvoice.ID,
                InvoiceType = (int)entity.InvoiceType,
                InvoiceTaxRate = entity.InvoiceTaxRate,
                Address = entity.Address,
                Tel = entity.Tel,
                BankName = entity.BankName,
                BankAccount = entity.BankAccount,
                DeliveryType = (int)entity.DeliveryType,
                MailName = entity.MailName,
                MailMobile = entity.MailMobile,
                MailAddress = entity.MailAddress,
                Status = (int)entity.InvoiceNoticeStatus,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Summary = entity.Summary,
                AmountLimit = entity.AmountLimit
            };
        }
    }
}
