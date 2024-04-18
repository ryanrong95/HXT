using Needs.Ccs.Services.ApiSettings;
using Needs.Underly;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class MakeAccountHandler
    {       
        public MKResult PostToK3(object model)
        {

            var SendJson = new
            {
                request_service = MakeAccountSetting.request_service,
                request_item = MakeAccountSetting.request_item,
                token = MakeAccountSetting.token,
                data = model,
            };

            var apiurl = System.Configuration.ConfigurationManager.AppSettings[MakeAccountSetting.ApiName] + MakeAccountSetting.MakeAccountHandle;

            var result = Needs.Utils.Http.ApiHelper.Current.PostData(apiurl, SendJson);
            var jResult = JsonConvert.DeserializeObject<MKResult>(result);

            return jResult;
        }
    }

    public class MKModel
    { 
    
    


    }


    public class MKResult
    {
        public bool success { get; set; }

        public int status_code { get; set; }

        public bool data { get; set; }

        public string msg { get; set; }

        public string user_host_address { get; set; }
    }
}
