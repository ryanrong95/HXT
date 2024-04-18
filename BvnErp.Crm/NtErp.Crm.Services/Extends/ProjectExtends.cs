using NtErp.Crm.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Extends
{
    static class ProjectExtends
    {
        /// <summary>
        /// model数据映射数据库对象
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        //static internal Layer.Data.Sqls.BvCrm.Projects ToLinq(this Project entity)
        //{
        //    return new Layer.Data.Sqls.BvCrm.Projects {
        //        ID = entity.ID,
        //        Name = entity.Name,
        //        Type = (int)entity.Type,
        //        ClientID = entity.Client.ID,
        //        CompanyID = entity.Company.ID,
        //        Valuation = entity.Valuation ?? 0,
        //        AdminID = entity.Admin.ID,
        //        StartDate = entity.StartDate,
        //        EndDate = entity.EndDate,
        //        CreateDate = entity.CreateDate,
        //        UpdateDate = entity.UpdateDate,
        //        Summary = entity.Summary,
        //        Currency = (int)entity.Currency,
        //        Status = (int)entity.Status
        //    };
        //}
    }
}
