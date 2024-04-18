using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.XdtData.Import.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class TableAttribute : Attribute
    {
        readonly string name;
        readonly string target;

        public TableAttribute(string name, string target)
        {
            this.name = name;
            this.target = target;
        }

        /// <summary>
        /// 临时表名
        /// </summary>
        public string Name { get { return this.name; } }

        /// <summary>
        /// 目标表名
        /// </summary>
        public string Target { get { return this.target; } }
    }
}
