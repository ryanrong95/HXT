using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvData.LinqToSolr.Criterias
{
    /// <summary>
    /// 排序查询
    /// </summary>
    public class SolrSort
    {
        public string Name { get; set; }
        public SolrSortType Sort { get; set; }

        public static SolrSort Create(Expression fieldExp, SolrSortType sort)
        {
            var o = new SolrSort();
            var fb = fieldExp as MemberExpression;

            if (fb != null)
            {
                var dataMemberAttribute = fb.Member.GetCustomAttribute<JsonPropertyAttribute>();
                o.Name = dataMemberAttribute?.PropertyName ?? fb.Member.Name;
                o.Sort = sort;

                return o;
            }

            return null;
        }
    }
}
