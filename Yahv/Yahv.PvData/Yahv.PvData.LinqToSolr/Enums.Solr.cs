using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvData.LinqToSolr
{
    /// <summary>
    /// Http请求方式
    /// </summary>
    public enum SolrWebMethod
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    /// <summary>
    /// 排序方式
    /// </summary>
    public enum SolrSortType
    {
        Asc,
        Desc
    }
}
