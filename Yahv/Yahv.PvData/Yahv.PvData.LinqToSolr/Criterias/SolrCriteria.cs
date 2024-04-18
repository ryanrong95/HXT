using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvData.LinqToSolr.Criterias
{
    /// <summary>
    /// Solr查询条件
    /// </summary>
    public class SolrCriteria
    {
        private int take = 10;
        internal int Take
        {
            get
            {
                if (this.take <= 0) return 10;
                else return this.take;
            }
            set { this.take = value; }
        }

        private int skip = 0;
        internal int Skip
        {
            get
            {
                if (this.skip < 0) return 0;
                else return this.skip;
            }
            set { this.skip = value; }
        }

        internal int FacetsLimit { get; set; }
        internal string Filter { get; set; }
        internal SolrSelect Select { get; set; }

        internal List<SolrFilter> Filters { get; set; }
        internal List<SolrFacet> Facets { get; set; }
        internal List<SolrFacet> FacetsToIgnore { get; set; }
        internal List<SolrSort> Sortings { get; set; }
        internal List<string> GroupFields { get; set; }
        internal List<SolrJoin> JoinFields { get; set; }

        public SolrCriteria()
        {
            this.Filters = new List<SolrFilter>();
            this.Facets = new List<SolrFacet>();
            this.FacetsToIgnore = new List<SolrFacet>();
            this.Sortings = new List<SolrSort>();
            this.GroupFields = new List<string>();
            this.JoinFields = new List<SolrJoin>();
        }

        public SolrCriteria AddFilter(LambdaExpression field, params object[] values)
        {
            this.Filters.Add(SolrFilter.Create(field, values));
            return this;
        }

        public SolrCriteria AddFilter(Type objectType, string field, params object[] values)
        {
            this.Filters.Add(SolrFilter.Create(objectType, field, values));
            return this;
        }

        public SolrCriteria AddFilter<T>(string field, params object[] values)
        {
            this.Filters.Add(SolrFilter.Create<T>(field, values));
            return this;
        }

        public SolrCriteria AddSorting(Expression field, SolrSortType order)
        {
            this.Sortings.Add(SolrSort.Create(field, order));
            return this;
        }

        //TODO:SolrFacet实现后在此处添加AddFact和GetFact方法
    }
}
