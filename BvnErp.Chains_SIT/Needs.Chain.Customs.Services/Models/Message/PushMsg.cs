using Needs.Ccs.Services.ApiSettings;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class PushMsg
    {     
        public int SpotType { get; set; }
        public string OrderID { get; set; }
        public string ExpressNo { get; set; }

        public PushMsg(int spotType,string orderID)
        {          
            this.SpotType = spotType;
            this.OrderID = orderID;
        }

        public PushMsg(int spotType, string orderID,string expressNo)
        {
            this.SpotType = spotType;
            this.OrderID = orderID;
            this.ExpressNo = ExpressNo;
        }

        public void push()
        {
            try
            {
                MsgDTO msgDTO = new MsgDTO();
                msgDTO.clientCode = this.OrderID.Substring(0, 5);
                msgDTO.systemID = 1;
                msgDTO.spotType = this.SpotType;
                msgDTO.orderID = this.OrderID;
                msgDTO.expressNo = this.ExpressNo;

                string URL = System.Configuration.ConfigurationManager.AppSettings[MsgApiSetting.ApiName];
                string requestUrl = URL + MsgApiSetting.MsgUrl;

                HttpResponseMessage response = new HttpResponseMessage();
                response = new HttpUtility.HttpClientHelp().HttpClient("POST", requestUrl, msgDTO.Json());
            
            }
            catch(Exception ex)
            {
                ex.CcsLog(this.OrderID+" "+this.SpotType+"推送消息失败");
            }           
        }
    }
}
