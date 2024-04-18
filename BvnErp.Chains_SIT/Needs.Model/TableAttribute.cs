using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Needs.Model
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class TableAttribute : Attribute
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public TableAttribute() { }

        /// <summary>
        /// 获取或设置对象对应的数据库表名称
        /// 操作日志名称
        /// </summary>
        public string Name
        {
            get;
            set;
        }
    }
}
