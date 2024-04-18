using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Utils.Serializers;

namespace Yahv.PSLToSolrData.Import
{
    class Program
    {
        static void Main(string[] args)
        {
            //AccessSolrViaApiHelper();

        }

        static void AccessSolrViaApiHelper()
        {
            var pi = new Views.ProductInventoriesView().FirstOrDefault();
            //5fd3f29b76e6f003aa0a02460f81ae3f  1a88e98a36bf5212944be34cb91c2fcb
            var p = new Views.ProductsView().FirstOrDefault(item => item.ID == "5fd3f29b76e6f003aa0a02460f81ae3f");
            var json = new
            {
                add = new
                {
                    doc = p
                }
            }.Json();

            //使用WebClient，需要添加认证
            //client.Credentials = new NetworkCredential("solradmin", "B1b2020");

            string getUrl = "http://solradmin:B1b2020@172.30.10.197:8983/solr/psl/select";
            var getResult = Yahv.Utils.Http.ApiHelper.Current.Get<string>(getUrl);

            string postUrl = "http://solradmin:B1b2020@172.30.10.197:8983/solr/psl/update?wt=json&commit=true";
            var postResult = Yahv.Utils.Http.ApiHelper.Current.JPost<string>(postUrl, new
            {
                add = new
                {
                    doc = p
                }
            });
        }
    }
}
