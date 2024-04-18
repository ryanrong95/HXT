using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Cbs.Services.Views.Origins
{
    /// <summary>
    /// 海关基础数据视图
    /// </summary>
    internal class CustomsSettingsOrigin : UniqueView<Models.Origins.CustomsSetting, ScCustomsReponsitory>
    {
        internal CustomsSettingsOrigin()
        {
        }

        internal CustomsSettingsOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Origins.CustomsSetting> GetIQueryable()
        {
            return from cert in this.Reponsitory.ReadTable<Layer.Data.Sqls.Customs.CustomsSettings>()
                   orderby cert.Type
                   select new Models.Origins.CustomsSetting
                   {
                       ID = cert.ID,
                       Code = cert.Code,
                       Name = cert.Name,
                       EnglishName = cert.EnglishName,
                       Summary = cert.Summary,
                       Type = (Enums.BaseType)cert.Type,
                   };
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="type">基础数据类型</param>
        /// <returns></returns>
        internal IQueryable<Models.Origins.CustomsSetting> this[Enums.BaseType type]
        {
            get
            {
                return this.Where(item => item.Type == type);
            }
        }
    }
}
