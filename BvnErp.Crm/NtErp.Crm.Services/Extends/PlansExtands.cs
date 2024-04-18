using Layer.Data.Sqls.BvCrm;
using Needs.Erp.Generic;
using Needs.Overall;
using NtErp.Crm.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Extends
{
    public static class PlansExtands
    {
        /// <summary>
        /// model数据映射数据库对象
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        static internal Actions ToLinq(this Plan entity)
        {
            return new Actions()
            {
                ID = entity.ID,
                Name = entity.Name,
                ClientID = entity.client.ID,
                CompanyID = entity.Companys.ID,
                SaleID = entity.SaleID,
                SaleManagerID = entity.SaleManagerID,
                Target = (int)entity.Target,
                Methord = (int)entity.Methord,
                CatalogueID = entity.CatalogueID,
                AdminID = entity.AdminID,
                PlanDate = entity.PlanDate,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Summary = entity.Summary
            };
        }
    }
}

