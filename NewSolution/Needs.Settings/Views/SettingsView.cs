using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Settings.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Settings
{
    class SettingsView : QueryView<Setting, BvOverallsReponsitory>
    {
        internal SettingsView()
        {
        }

        protected SettingsView(BvOverallsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Setting> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvOveralls.Settings>()
                   select new Setting
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
