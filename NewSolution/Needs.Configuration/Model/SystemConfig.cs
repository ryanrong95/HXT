using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Configuration.Model
{
    class SystemConfig : ConfigurationBase, ISystemConfig
    {
        /// <summary>
        /// 数据配置使用的数据库
        /// </summary>
        public string SettingDbName { get; set; }

        /// <summary>
        /// 语言数据库名称
        /// </summary>
        public string LanguageDbName { get; set; }

        protected SystemConfig()
        {
            this.SettingDbName = "BvTester";
            this.LanguageDbName = "BvOther";
        }
    }
}
