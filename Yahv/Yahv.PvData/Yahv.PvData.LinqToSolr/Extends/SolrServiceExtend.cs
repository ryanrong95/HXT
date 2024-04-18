using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvData.LinqToSolr.Extends
{
    /// <summary>
    /// SolrService扩展方法
    /// </summary>
    public static class SolrServiceExtend
    {
        public static SolrWebRequest QueryRequest(this ISolrService service, string collection)
        {
            string path = string.Format("/{0}/select", collection);
            var request = new SolrWebRequest(path);

            request.AddParameter("q", "*");
            request.AddParameter("wt", "json");
            request.AddParameter("indent", "true");
            request.AddParameter("rows", service.Criteria.Take);
            request.AddParameter("start", service.Criteria.Skip);

            if (service.Criteria.JoinFields.Any())
            {
                foreach (var joiner in service.Criteria.JoinFields)
                {
                    //TODO:SolrJoin实现后在此处添加相关业务处理
                }
            }

            if (service.Criteria.GroupFields.Any())
            {
                request.AddParameter("group", "true");
                request.AddParameter("group.limit", service.Criteria.Take);
                request.AddParameter("group.offset", service.Criteria.Skip);
                foreach (var groupField in service.Criteria.GroupFields)
                {
                    request.AddParameter("group.field", groupField);
                }
            }

            if (service.Criteria.Filters.Any())
            {
                foreach (var filter in service.Criteria.Filters)
                {
                    request.AddParameter("fq", string.Format("{0}: ({1})", filter.Name,
                        string.Join(" OR ", filter.Values.Select(x => string.Format("\"{0}\"", x)).ToArray()
                        )));
                }
            }

            if (!string.IsNullOrEmpty(service.Criteria.Filter))
            {
                foreach (var fstring in service.Criteria.Filter.Split(new[] { "&fq=" }, StringSplitOptions.None))
                {
                    if (!string.IsNullOrEmpty(fstring))
                    {
                        request.AddParameter("fq", fstring);
                    }
                }
            }

            if (service.Criteria.Sortings.Any())
            {
                request.AddParameter("sort", string.Join(", ", service.Criteria.Sortings.Select(x =>
                        string.Format("{0} {1}", x.Name, x.Sort == SolrSortType.Desc ? "DESC" : "ASC")).ToArray()));
            }

            if (service.Criteria.Facets.Any())
            {
                request.AddParameter("facet", "true");
                request.AddParameter("facet.mincount", "1");

                if (service.Criteria.FacetsLimit > 0)
                {
                    request.AddParameter("facet.limit", service.Criteria.FacetsLimit.ToString());
                }

                //TODO:SolrFacet实现后在此处添加相关业务处理
            }

            if (service.Criteria.Select != null)
            {
                request.AddParameter("fl", service.Criteria.Select.GetSelectFields());
            }

            return request;
        }
    }
}
