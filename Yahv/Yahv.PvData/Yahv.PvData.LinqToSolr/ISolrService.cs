using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Yahv.PvData.LinqToSolr.Criterias;

namespace Yahv.PvData.LinqToSolr
{
    public interface ISolrService
    {
        SolrCriteria Criteria { get; set; }
        Type ElementType { get; set; }

        void Delete<T>(Expression<Func<T, bool>> query);

        object Query(Type type, SolrCriteria query = null);

        IEnumerable<T> Query<T>(SolrCriteria query = null);

        void Save<T>(params T[] documents);

        void Save<T>(T document);
    }
}
