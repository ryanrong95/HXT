using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 海关税则扩展方法
    /// </summary>
    public static class CustomsTariffExtends
    {
        public static Layer.Data.Sqls.ScCustoms.CustomsTariffs ToLinq(this Models.CustomsTariff entity)
        {
            return new Layer.Data.Sqls.ScCustoms.CustomsTariffs
            {
                ID = entity.ID,
                HSCode = entity.HSCode,
                Name = entity.Name,
                MFN = entity.MFN,
                General = entity.General,
                AddedValue = entity.AddedValue,
                Consume = entity.Consume,
                Elements = entity.Elements,
                RegulatoryCode = entity.RegulatoryCode,
                Unit1 = entity.Unit1,
                Unit2 = entity.Unit2,
                CIQCode = entity.CIQCode,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = DateTime.Now,
                Summary = entity.Summary,
                InspectionCode = entity.InspectionCode
            };
        }
    }
}