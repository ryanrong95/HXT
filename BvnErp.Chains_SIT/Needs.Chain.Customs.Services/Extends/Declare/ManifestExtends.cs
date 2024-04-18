using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public static class ManifestExtends
    {
        public static Layer.Data.Sqls.ScCustoms.Manifests ToLinq(this Models.Manifest entity)
        {
            return new Layer.Data.Sqls.ScCustoms.Manifests
            {
                ID = entity.ID,
                TrafMode = entity.TrafMode,
                CustomsCode = entity.CustomsCode,
                CarrierCode = entity.CarrierCode,
                TransAgentCode = entity.TransAgentCode,
                LoadingDate = entity.LoadingDate.Value,
                LoadingLocationCode = entity.LoadingLocationCode,
                ArrivalDate = entity.ArrivalDate,
                CustomMaster = entity.CustomMaster,
                UnitCode = entity.UnitCode,
                MsgRepName = entity.MsgRepName,
                AdditionalInformation = entity.AdditionalInformation,
                CreateTime = entity.CreateTime
            };
        }
    }
}
