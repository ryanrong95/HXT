using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Cbs.Services.View.Origins
{
    internal class CustomsSettingOrigin : UniqueView<Needs.Cbs.Services.Model.Origins.CustomsSetting, ScCustomsReponsitory>
    {
        internal CustomsSettingOrigin()
        {
        }

        internal CustomsSettingOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Model.Origins.CustomsSetting> GetIQueryable()
        {
            return from cert in this.Reponsitory.ReadTable<Layer.Data.Sqls.Customs.CustomsSettings>()
                   select new Needs.Cbs.Services.Model.Origins.CustomsSetting
                   {
                       ID = cert.ID,
                       Code = cert.Code,
                       Name = cert.Name,
                       EnglishName = cert.EnglishName,
                       Summary = cert.Summary,
                       Type = (Enums.BaseType)cert.Type,
                   };
        }
    }
}
