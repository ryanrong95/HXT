using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvWsOrder.Services.Models
{
    public class WordNoHandler
    {
        public MKWordNo PostToK3(object model)
        {

            var SendJson = new
            {
                request_service = MakeAccountSetting.request_service,
                request_item = MakeAccountSetting.request_wordNo,
                token = MakeAccountSetting.token,
                data = model,
            };

            var apiurl = System.Configuration.ConfigurationManager.AppSettings[MakeAccountSetting.ApiName] + MakeAccountSetting.MakeAccountHandle;

            var result = Yahv.Utils.Http.ApiHelper.Current.Post(apiurl, SendJson);
            var jResult = JsonConvert.DeserializeObject<MKWordNo>(result);

            return jResult;
        }
    }

    public class MKWordNo
    {
        public bool success { get; set; }

        public int status_code { get; set; }

        public WordNoResult data { get; set; }

        public string msg { get; set; }

        public string user_host_address { get; set; }
    }

    public class WordNoResult
    {
        public string 归属方案编号 { get; set; }

        public List<WordNoResultItem> 标识集合 { get; set; }
    }

    public class WordNoResultItem
    {
        public int 生成凭证 { get; set; }
        public string 生成凭证结果 { get; set; }
        public string FNumber { get; set; }
        public string FSerialNum { get; set; }
        public string 标识 { get; set; }
    }

    public class WordNoRequest
    {
        public string 归属方案编号 { get; set; }
        public List<string> 标识集合 { get; set; }
    }
}
