using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Yahv.PvData.LinqToSolr.Attributes;

namespace Yahv.PvData.LinqToSolr.Criterias
{
    /// <summary>
    /// 条件查询
    /// </summary>
    public class SolrFilter
    {
        public string Name { get; set; }
        public object[] Values { get; set; }

        public static SolrFilter Create<T>(string field, params object[] values)
        {
            var o = new SolrFilter();
            var prop = typeof(T).GetProperty(field);
            var dataMemberAttribute = prop.GetCustomAttribute<JsonPropertyAttribute>();

            o.Name = dataMemberAttribute?.PropertyName ?? prop.Name;
            o.Values = values.ToArray();

            return o;
        }

        public static SolrFilter Create(Type objectType, string field, params object[] values)
        {
            var o = new SolrFilter();
            var prop = objectType.GetProperty(field);
            var dataMemberAttribute = prop.GetCustomAttribute<JsonPropertyAttribute>();

            o.Name = dataMemberAttribute?.PropertyName ?? prop.Name;
            o.Values = values.ToArray();

            return o;
        }

        public static SolrFilter Create(LambdaExpression fieldExp, params object[] values)
        {
            var filter = new SolrFilter();
            var o = fieldExp.Body as MemberExpression;

            if (o != null)
            {
                var dataMemberAttribute = o.Member.GetCustomAttribute<JsonPropertyAttribute>();
                filter.Name = dataMemberAttribute?.PropertyName ?? o.Member.Name;
                filter.Values = values.ToArray();

                return filter;
            }

            return null;
        }
    }
}
