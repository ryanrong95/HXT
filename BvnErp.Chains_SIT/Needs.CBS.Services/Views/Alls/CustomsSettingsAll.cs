using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Cbs.Services.Views.Alls
{
    /// <summary>
    /// 海关基础数据视图
    /// </summary>
    public class CustomsSettingsAll : UniqueView<Models.Origins.CustomsSetting, ScCustomsReponsitory>
    {
        public CustomsSettingsAll()
        {
        }

        protected override IQueryable<Models.Origins.CustomsSetting> GetIQueryable()
        {
            return new Origins.CustomsSettingsOrigin();
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="type">基础数据类型</param>
        /// <returns></returns>
        public IQueryable<Models.Origins.CustomsSetting> this[Enums.BaseType type]
        {
            get
            {
                return this.Where(item => item.Type == type);
            }
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="type">基础数据类型</param>
        /// <param name="code">基础数据代码</param>
        /// <returns></returns>
        public Models.Origins.CustomsSetting this[Enums.BaseType type, string code]
        {
            get
            {
                return this.FirstOrDefault(item => item.Type == type && item.Code == code);
            }
        }
    }
}
