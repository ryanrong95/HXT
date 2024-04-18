using Layer.Data.Sqls.BvCrm;
using NtErp.Crm.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Extends
{
    public static class InvoiceExtends
    {
        /// <summary>
        /// model数据映射数据库对象
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        static internal Invoices ToLinq(this Invoice entity)
        {
            return new Invoices()
            {
               ID = entity.ID,
               ClientID = entity.ClientID,
               Type = (int)entity.InvoiceTypes,
               CompanyID = entity.CompanyID,
               CompanyCode = entity.CompanyCode,              
               Address = entity.Address,              
               Bank = entity.BankName,
               BankAccount = entity.Account,
               Status = (int)entity.Status,
               CreateDate = entity.CreateDate,
               UpdateDate = entity.UpdateDate,
               Phone = entity.Phone,
               ConsigneeID = entity.Consignee.ID,
            };
        }
    }
}
