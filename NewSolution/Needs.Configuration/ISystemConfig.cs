using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Configuration
{
    public interface ISystemConfig : IConfiguration
    {
        /// <summary>
        /// 数据库设置使用数据库名称
        /// </summary>
        string SettingDbName { get; }

        /// <summary>
        /// 语言数据库名称
        /// </summary>
        string LanguageDbName { get; set; }

    }
}
