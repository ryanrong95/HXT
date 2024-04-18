using System;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 
    /// </summary>
    public static class ClientInvoiceConsigneesExtends
    {
        public static Layer.Data.Sqls.ScCustoms.ClientInvoiceConsignees ToLinq(this ClientInvoiceConsignee entity)
        {
            return new Layer.Data.Sqls.ScCustoms.ClientInvoiceConsignees
            {
                ID = entity.ID,
                ClientID = entity.ClientID,
                Name = entity.Name,
                Mobile = entity.Mobile,
                Address = entity.Address,
                Tel = entity.Tel,
                Email=entity.Email,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = DateTime.Now,
                Summary = entity.Summary
            };
        }
    }
}