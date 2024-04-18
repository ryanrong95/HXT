using System;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 
    /// </summary>
    public static class ClientConsigneeExtends
    {
        public static Layer.Data.Sqls.ScCustoms.ClientConsignees ToLinq(this ClientConsignee entity)
        {
            return new Layer.Data.Sqls.ScCustoms.ClientConsignees
            {
                ID = entity.ID,
                Name = entity.Name,
                ClientID = entity.ClientID,
                ContactID = entity.Contact.ID,
                Address = entity.Address,
                IsDefault = entity.IsDefault,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = DateTime.Now,
                Summary = entity.Summary
            };
        }
    }
}