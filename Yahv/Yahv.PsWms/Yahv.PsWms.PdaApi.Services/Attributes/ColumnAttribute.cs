using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PsWms.PdaApi.Services.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public sealed class ColumnAttribute : Attribute
    {
        readonly string name;
        readonly string definition;
        readonly string target;

        public ColumnAttribute(string name, string definition, string target)
        {
            this.name = name;
            this.definition = definition;
            this.target = target;
        }

        /// <summary>
        /// 字段名
        /// </summary>
        public string Name { get { return this.name; } }

        /// <summary>
        /// 字段定义
        /// </summary>
        public string Definition { get { return this.definition; } }

        /// <summary>
        /// 目标字段名
        /// </summary>
        public string Target { get { return this.target; } }
    }
}
