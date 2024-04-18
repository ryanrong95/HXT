using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvWsOrder.Services.Views
{
    public class VourcherInfoView
    {
        private string[] _VourcherIDs { get; set; }

        public VourcherInfoView(string[] vourcherIDs)
        {
            this._VourcherIDs = vourcherIDs;
        }

        public GetVourcherInfoResult GetVourcherInfo()
        {
            var strJsonReq = JsonConvert.SerializeObject(new { VoucherIDs = string.Join(",", this._VourcherIDs), });

            string SzApiUrl = ConfigurationManager.AppSettings["SzApiUrl"];
            string url = string.Join(@"/", SzApiUrl, "Voucher/GetVoucherInfo");

            string apiResult = string.Empty;
            using (WebClient client = new WebClient { Encoding = Encoding.UTF8 })
            {
                client.Headers.Add("Content-Type", "application/json");
                client.Headers.Add("Accept", "application/json");
                client.Headers.Add("User-Agent", "POST");
                apiResult = client.UploadString(url, "POST", strJsonReq);
            }

            var apiResultModel = JsonConvert.DeserializeObject<GetVourcherInfoResult>(apiResult);
            return apiResultModel;
        }

        public class GetVourcherInfoResult
        {
            public bool success { get; set; }

            public string msg { get; set; }

            public List<VoucherinfosItem> voucherinfos { get; set; }
        }

        public class VoucherinfosItem
        {
            public string VoucherID { get; set; }

            public string CutDateIndex { get; set; }
        }


    }
}
