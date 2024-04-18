using Needs.Ccs.Services.Models.Express100;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class QueryTrack
    {
        public static string query(QueryTrackReq query)
        {

            var request = ObjectToDictionaryUtils.ObjectToMap(query);
            if (request == null)
            {
                return null;
            }
            var result = HttpUtils.doPostForm(ApiInfoConstant.QUERY_URL, request);
            return result;
        }
    }
}
