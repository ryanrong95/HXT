using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Settings.Extends
{
    static class SettingExtends
    {
        static internal Layer.Data.Sqls.BvOveralls.Settings ToLinq(this Needs.Settings.Models.Setting entity)
        {
            return new Layer.Data.Sqls.BvOveralls.Settings
            {
                ID = entity.ID,
                Type = entity.Type,
                DataType = entity.DataType,
                Name = entity.Name,
                Value = entity.Value,
                Summary = entity.Summary,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
            };
        }
    }
}
