using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public static class ManifestConsignmentExtends
    {
        public static Layer.Data.Sqls.ScCustoms.ManifestConsignments ToLinq(this Models.ManifestConsignment entity)
        {
            return new Layer.Data.Sqls.ScCustoms.ManifestConsignments {
                ID = entity.ID,
                ManifestID = entity.Manifest.ID,
                CusMftStatus = entity.CusMftStatus,
                ConditionCode = entity.ConditionCode,
                PaymentType = entity.PaymentType,
                GovProcedureCode = entity.GovProcedureCode,
                TransitDestination = entity.TransitDestination,
                PackNum = entity.PackNum,
                PackType = entity.PackType,
                Cube = entity.Cube,
                GrossWt = entity.GrossWt,
                GoodsValue = entity.GoodsValue,
                GoodsQuantity = entity.GoodsQuantity,
                Currency = entity.Currency,
                Consolidator = entity.Consolidator,
                ConsigneeName = entity.ConsigneeName,
                ConsignorName = entity.ConsignorName,
                AdminID = entity.Admin.ID,
                CreateDate = entity.CreateDate,
                MarkingUrl = entity.MarkingUrl
            };
        }
    }
}
