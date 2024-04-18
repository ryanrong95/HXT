using Layer.Data.Sqls.BvCrm;
using NtErp.Crm.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Extends
{
    public static class ConsigneeExtends
    {
        /// <summary>
        /// model数据映射数据库对象
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        static internal Consignees ToLinq(this Consignee entity)
        {
            return new Consignees()
            {
               ID = entity.ID,             
               CompanyID = entity.CompanyID,
               ContactID = entity.Contact.ID,
               ClientID = entity.ClientID,
               Address = entity.Address,
               Zipcode = entity.Zipcode,
               Status = (int)entity.Status,
               CreateDate = entity.CreateDate,
               UpdateDate = entity.UpdateDate,
            };
        }
    }
}
