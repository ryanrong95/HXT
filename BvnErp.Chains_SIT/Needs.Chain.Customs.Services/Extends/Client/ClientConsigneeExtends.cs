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
    public static class ClientConsigneeExtends
    {
        public static Layer.Data.Sqls.ScCustoms.ClientConsignees ToLinq(this Models.ClientConsignee entity)
        {
            return new Layer.Data.Sqls.ScCustoms.ClientConsignees
            {
                ID = entity.ID,
                Name=entity.Name,
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