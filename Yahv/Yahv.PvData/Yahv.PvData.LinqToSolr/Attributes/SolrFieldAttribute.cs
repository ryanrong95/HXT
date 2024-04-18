using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvData.LinqToSolr.Attributes
{
    /// <summary>
    /// 用于建立 Model属性 与 Solr Field(索引库字段) 之间的映射关系
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public sealed class SolrFieldAttribute : Attribute
    {
        readonly string name;

        public SolrFieldAttribute(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// 索引库字段名称
        /// </summary>
        public string Name { get { return this.name; } }
    }
}
