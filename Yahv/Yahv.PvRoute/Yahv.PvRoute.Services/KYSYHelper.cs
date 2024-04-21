using kyeSDK;
using kyeSDK.model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.PvRoute.Services.Models;
using Yahv.Utils.Serializers;

namespace Yahv.PvRoute.Services
{
    public class KYSYHelper
    {
        public void CreateFaceOrderLog(string faceOrderID)
        {
            string appkey = "80631";
            string appsecret = "6C4E0274101AEF1BA4AD31EF866D572D";

            var paramInfo = new
            {
                waybillNumbers = new[]
                {
                    faceOrderID
                }
            }.Json();

            DefaultKyeClient kyeClient = new DefaultKyeClient(appkey, appsecret);

            var kyeRequest = new KyeRequest
            {
                contentType = "application/json",
                methodCode = "open.api.openCommon.queryPublicRoute",
                paramInfo = paramInfo,
                responseFormat = "json"
            };
            //提交电子运单接口
            string resJson = kyeClient.Execute(kyeRequest, false);  //沙盒环境执行下单 生产不传false 默认为生产

            JObject rlt = resJson.JsonTo<JObject>();
            var code = rlt["code"].Value<int>();
            //Console.WriteLine(resJson2);

            if (code == 10000)
            {
                var success = rlt["success"].Value<bool>();

                if (success == true)
                {
                    JToken message = rlt["data"];

                    JToken esWaybill = message["esWaybill"];

                    foreach (var waybill in esWaybill)
                    {
                        var waybillNumber = waybill["waybillNumber"].Value<string>();

                        if (waybillNumber != faceOrderID)
                        {
                            throw new Exception("不可思议的错误！！");
                        }

                        var exteriorRouteList = waybill["exteriorRouteList"];
                        foreach (var route in exteriorRouteList)
                        {
                            //请求Logs_Transport的Enter方法，注意要去重
                            new Logs_Transport()
                            {
                                FaceOrderID = faceOrderID,
                                Summary = route["routeDescription"].Value<string>(),
                                Json = kyeRequest.Json()
                            }.Enter();
                        }
                    }
                }
            }
        }
    }
}
