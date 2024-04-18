using Needs.Ccs.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 会员扩展方法
    /// </summary>
    public static class ClientExtends
    {
        public static Layer.Data.Sqls.ScCustoms.Clients ToLinq(this Models.Client entity)
        {
            return new Layer.Data.Sqls.ScCustoms.Clients
            {
                ID = entity.ID,
                CompanyID = entity.Company.ID,
                ClientType = (int)entity.ClientType,
                ClientRank = (int)entity.ClientRank,
                ClientCode = entity.ClientCode,
                AdminID = entity.Admin?.ID,
                ClientStatus = (int)entity.ClientStatus,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = DateTime.Now,
                Summary = entity.Summary,
                ServiceType = (int)entity.ServiceType,
                ClientNature = entity.ClientNature,
                IsValid = entity.IsValid,
                IsStorageValid = entity.IsStorageValid,
                StorageType = (int)entity.StorageType,
                ChargeWH = (int)entity.ChargeWH,
                ChargeType = entity.ChargeType != null ? (int?)entity.ChargeType : null,
                AmountWH = entity.AmountWH,
                IsNormal = entity.IsNormal,
                IsQualified = entity.IsQualified,
                IsDownloadDecTax = entity.IsDownloadDecTax,
                IsApplyInvoice = entity.IsApplyInvoice
            };
        }
    }
}
