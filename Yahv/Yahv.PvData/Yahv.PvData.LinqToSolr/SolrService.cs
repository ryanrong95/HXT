using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Yahv.PvData.LinqToSolr.Attributes;
using Yahv.PvData.LinqToSolr.Criterias;
using Yahv.PvData.LinqToSolr.Extends;
using Yahv.PvData.LinqToSolr.Visitors;

namespace Yahv.PvData.LinqToSolr
{
    public class SolrService : ISolrService
    {
        public SolrCriteria Criteria { get; set; }
        public Type ElementType { get; set; }

        public SolrService()
        {
            this.Criteria = new SolrCriteria();
        }

        public void Delete<T>(Expression<Func<T, bool>> query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var solrCollAttr = Attribute.GetCustomAttribute(typeof(T), typeof(SolrCollectionAttribute)) as SolrCollectionAttribute;
            if (solrCollAttr == null)
            {
                throw new MissingMemberException(string.Format("类 '{0}' 缺少Attribute定义: {1}", typeof(T).Name, "SolrCollectionAttribute"));
            }

            var visitor = new SolrCriteriaVisitor(this, typeof(T));
            var q = Evaluator.PartialEval(query);
            var queryString = visitor.Translate(BooleanVisitor.Process(q));

            Utils.Http.ApiHelper.Current.JPost("http://localhost:9090/solrapi/data/delete", new
            {
                collection = solrCollAttr.Name,
                queryString = queryString
            });
        }

        public object Query(Type type, SolrCriteria query = null)
        {
            this.Criteria = query ?? this.Criteria;

            var solrCollAttr = Attribute.GetCustomAttribute(type, typeof(SolrCollectionAttribute)) as SolrCollectionAttribute;
            if (solrCollAttr == null)
            {
                throw new MissingMemberException(string.Format("类 '{0}' 缺少Attribute定义: {1}", type.Name, "SolrCollectionAttribute"));
            }
            string collection = solrCollAttr.Name;

            SolrWebRequest request = this.QueryRequest(collection);

            var result = Utils.Http.ApiHelper.Current.Get("http://localhost:9090/solrapi/data/query", new
            {
                collection = solrCollAttr.Name,
                queryString = string.Join("&", request.Parameters.Select(x => string.Format("{0}={1}", x.Name, x.Value)).ToArray())
            });

            var listMethod = typeof(List<>).MakeGenericType(this.Criteria.Select != null && !this.Criteria.Select.IsSingleField
                            ? this.Criteria.Select.Type : this.ElementType);

            return JsonConvert.DeserializeObject(result, listMethod) as IEnumerable;
        }

        public IEnumerable<T> Query<T>(SolrCriteria query = null)
        {
            return Query(typeof(T), query) as IEnumerable<T>;
        }

        public void Save<T>(T document)
        {
            this.Save<T>(new T[] { document });
        }

        public void Save<T>(params T[] documents)
        {
            if (documents == null)
                throw new ArgumentNullException(nameof(documents));

            var solrCollAttr = Attribute.GetCustomAttribute(typeof(T), typeof(SolrCollectionAttribute)) as SolrCollectionAttribute;
            if (solrCollAttr == null)
            {
                throw new MissingMemberException(string.Format("类 '{0}' 缺少Attribute定义: {1}", typeof(T).Name, "SolrCollectionAttribute"));
            }

            Utils.Http.ApiHelper.Current.JPost("http://localhost:9090/solrapi/data/save", new
            {
                collection = solrCollAttr.Name,
                documents = documents
            });
        }
    }
}
