using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvData.LinqToSolr
{
    public class SolrQuery<T> : IOrderedQueryable<T>
    {
        private Expression expression;
        private IQueryProvider provider;

        public Type ElementType
        {
            get
            {
                return typeof(T);
            }
        }

        public Expression Expression
        {
            get
            {
                if (this.expression == null)
                    this.expression = Expression.Constant(this);
                return this.expression;
            }
        }

        public IQueryProvider Provider
        {
            get
            {
                if (this.provider == null)
                {
                    this.provider = new SolrProvider(this.SolrService);
                }

                return this.provider;
            }
        }

        internal protected ISolrService SolrService { get; private set; }

        public SolrQuery()
        {
            this.SolrService = new SolrService();
            this.SolrService.ElementType = this.ElementType;
        }

        public SolrQuery(ISolrService solrService)
        {
            this.SolrService = solrService;
            this.SolrService.ElementType = this.ElementType;
        }

        internal SolrQuery(Expression expression, SolrProvider provider)
        {
            this.expression = expression;
            this.provider = provider;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return (this.Provider.Execute<IEnumerable<T>>(this.Expression)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public override string ToString()
        {
            return this.Provider.ToString();
        }
    }
}
