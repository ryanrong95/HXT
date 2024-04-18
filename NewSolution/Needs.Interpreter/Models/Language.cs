using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Needs.Interpreter.Extends;

namespace Needs.Interpreter.Models
{
    /// <summary>
    /// 语言
    /// </summary>
    public class Language : IUnique
    {
        public string ID
        {
            get
            {
                return this.ShortName;
            }
            set
            {
                this.ShortName = value;
            }
        }
        /// <summary>
        /// 短名称
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 英文名称（国际名称）
        /// </summary>
        public string EnglishName { get; set; }

        /// <summary>
        /// 数据库名称
        /// </summary>
        public string DataName { get; set; }
    }
}
