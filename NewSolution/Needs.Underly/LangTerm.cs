using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Underly
{
    /// <summary>
    /// 语言
    /// </summary>
    public sealed class LangTerm
    {
        /// <summary>
        /// 短名称
        /// </summary>
        public string ShortName { get; internal set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; internal set; }

        /// <summary>
        /// 英文名称（国际名称）
        /// </summary>
        public string EnglishName { get; internal set; }

        /// <summary>
        /// 数据库名称
        /// </summary>
        public string DataName { get; internal set; }

        /// <summary>
        /// 只能程序中进行构造
        /// </summary>
        internal LangTerm()
        {
        }
    }
}
