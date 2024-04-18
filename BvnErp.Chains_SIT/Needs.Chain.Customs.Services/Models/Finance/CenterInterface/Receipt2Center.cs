using Needs.Ccs.Services.ApiSettings;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 将核销信息传给中心
    /// </summary>
    public class Receipt2Center
    {
        public List<CenterReceipt> Receipts { get; set; }
        
        public Receipt2Center(List<CenterReceipt> receipts)
        {
            this.Receipts = receipts;
        }

        public bool Send()
        {

            try
            {
                string URL = System.Configuration.ConfigurationManager.AppSettings[FinanceApiSetting.ApiName];
                string requestUrl = URL + FinanceApiSetting.VerificationUrl;                

                HttpRequest.Post(requestUrl, Receipts.Json());
                return true;
            }
            catch(Exception ex)
            {
                ex.CcsLog("核销信息提交中心失败");
                return false;
            }
        }
    }
}
