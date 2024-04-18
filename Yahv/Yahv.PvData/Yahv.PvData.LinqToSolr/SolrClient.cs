using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvData.LinqToSolr
{
    public class SolrWebParameter
    {
        public string Name { get; set; }
        public object Value { get; set; }

        public SolrWebParameter(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }
    }

    public class SolrWebRequest
    {
        public string ActionUrl { get; set; }
        public SolrWebMethod Method { get; set; }
        public string Body { get; set; }
        public List<SolrWebParameter> Parameters { get; set; }

        public SolrWebRequest(string action, SolrWebMethod method = SolrWebMethod.GET)
        {
            this.ActionUrl = action;
            this.Method = method;
            this.Parameters = new List<SolrWebParameter>();
        }

        public void AddParameter(string name, object value)
        {
            this.Parameters.Add(new SolrWebParameter(name, value));
        }
    }

    class SolrClient
    {
    }
}
