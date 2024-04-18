using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvData.LinqToSolr.Attributes
{
    /// <summary>
    /// 用于建立 Model 与 Solr Collection(索引库) 之间的映射关系
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class SolrCollectionAttribute : Attribute
    {
        readonly string name;

        public SolrCollectionAttribute(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// 索引库名称
        /// </summary>
        public string Name { get { return this.name; } }
    }
}
