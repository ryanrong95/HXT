using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Yahv.PvData.LinqToSolr.Extends;
using Yahv.PvData.LinqToSolr.Visitors;

namespace Yahv.PvData.LinqToSolr
{
    public class SolrProvider : IQueryProvider
    {
        private string query = string.Empty;
        private Type ElementType { get; set; }
        internal protected ISolrService SolrService { get; private set; }

        internal SolrProvider(ISolrService solrService)
        {
            this.SolrService = solrService;
            this.ElementType = solrService.ElementType;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            this.ElementType = expression.Type.GetSeqType();
            return (IQueryable)Activator.CreateInstance(typeof(SolrQuery<>).MakeGenericType(this.ElementType), new object[] { expression, this });
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new SolrQuery<TElement>(expression ,this);
        }

        public object Execute(Expression expression)
        {
            return this.Execute<object>(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            bool isEnumerable = (typeof(TResult).Name == "IEnumerable`1");
            var query = GetSolrQuery(expression).Query(this.ElementType);

            var result = isEnumerable ? query : ((IEnumerable)query).Cast<TResult>().FirstOrDefault();
            return (TResult)result;
        }

        private ISolrService GetSolrQuery(Expression expression)
        {
            this.SolrService.ElementType = expression.Type.GetSeqType();
            var qt = new SolrCriteriaVisitor(this.SolrService);

            expression = Evaluator.PartialEval(expression, e => e.NodeType != ExpressionType.Parameter && !typeof(IQueryable).IsAssignableFrom(e.Type));
            this.SolrService.Criteria.Filter = qt.Translate(BooleanVisitor.Process(expression));

            return this.SolrService;
        }

        public override string ToString()
        {
            return this.query;
        }
    }
}
