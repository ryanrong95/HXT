using Needs.Utils.Serializers;
using NtErp.Crm.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Extends
{
    static class ContactExtends
    {
        /// <summary>
        /// model数据映射数据库对象
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        static internal Layer.Data.Sqls.BvCrm.Contacts ToLinq(this Contact entity)
        {
            entity.Detail = "";
            return new Layer.Data.Sqls.BvCrm.Contacts
            {
                ID = entity.ID,
                ClientID = entity.ClientID,
                Name = entity.Name,
                Type = (int)entity.Types,
                CompanyID = entity.CompanyID,
                Position = entity.Position,
                Email = entity.Email,
                Moblie = entity.Mobile,
                Tel = entity.Tel,             
                Detail = entity.Json(),
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
            };
        }

        static internal Layer.Data.Sqls.BvCrm.Contacts ToInoviceLinq(this Contact entity)
        {
            return new Layer.Data.Sqls.BvCrm.Contacts {
                Name = entity.Name,
                Moblie = entity.Mobile,                
            };
        }
    }
}
