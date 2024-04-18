using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 暂存扩展方法
    /// </summary>
    public static class TemporaryExtends
    {

        public static Layer.Data.Sqls.ScCustoms.Temporarys ToLinq(this Models.Temporary entity)
        {
            return new Layer.Data.Sqls.ScCustoms.Temporarys
            {
                ID = entity.ID,
                AdminID = entity.Admin.ID,
                EntryNumber = entity.EntryNumber,
                CompanyName = entity.CompanyName,
                ShelveNumber = entity.ShelveNumber,
                EntryDate = entity.EntryDate,
                WaybillCode = entity.WaybillCode,
                WrapType = (int)entity.WrapType,
                PackNo = entity.PackNo,
                TemporaryStatus = (int)entity.TemporaryStatus,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Summary = entity.Summary
            };
        }
    }
}
