using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.InvoiceService.Services.InvVeri
{
    public class InvoiceInfoForCom
    {
        static readonly string openPlatformUrl = "https://open.leshui365.com";

        public static Tuple<bool, string, string> CheckInvoice(string invoiceCode, string invoiceNumber, string billTime, string invoiceAmount)
        {
            string myAppKey = ConfigurationManager.AppSettings["myAppKey"];
            string myAppSecret = ConfigurationManager.AppSettings["myAppSecret"];

            //获取 Token
            var rtv = GetToken(myAppKey, myAppSecret);
            if (!rtv.Success)
            {
                return new Tuple<bool, string, string>(false, "请求Token时发生错误:" + rtv.Message, null);
            }

            //查询发票
            var invoiceArgs = new Dictionary<string, string>
            {
                {"invoiceCode", invoiceCode},
                {"invoiceNumber", invoiceNumber},
                {"billTime", billTime},
                {"invoiceAmount", invoiceAmount},
                {"token", rtv.Data}
            };

            rtv = GetInvoiceInfo(JsonConvert.SerializeObject(invoiceArgs));
            if (!rtv.Success)
            {
                return new Tuple<bool, string, string>(false, "查询发票时发生错误:" + rtv.Message, null);
            }

            //查询发票信息成功
            var rtvJsonString = rtv.Data;
            return new Tuple<bool, string, string>(true, "查询成功", rtvJsonString);
        }

        /// <summary>
        /// 获取授权码
        /// </summary>
        /// <param name="appKey">客户应用key：需向开放平台申请</param>
        /// <param name="appSecret">客户应用密钥：需向开放平台申请</param>
        /// <returns></returns>
        private static ReturnValue<string> GetToken(string appKey, string appSecret)
        {
            var rtv = new ReturnValue<string> { Success = false };

            try
            {

                var url = string.Format("{0}/getToken?appKey={1}&appSecret={2}", openPlatformUrl, appKey, appSecret);

                var client = new HttpClient();
                var task = client.GetStringAsync(url);
                task.Wait();

                rtv.Data = ((JObject)JsonConvert.DeserializeObject(task.Result))["token"].ToString();
                rtv.Success = true;
            }
            catch (AggregateException ex)
            {
                rtv.Message = ex.InnerException.Message;
            }
            catch (Exception ex)
            {
                rtv.Message = ex.Message;
            }

            return rtv;
        }

        private static ReturnValue<string> GetInvoiceInfo(string invoiceParams)
        {
            var rtv = new ReturnValue<string> { Success = false };

            try
            {
                var url = openPlatformUrl + "/api/invoiceInfoForCom";
                var httpContent = new StringContent(invoiceParams, Encoding.UTF8, "application/json");

                var client = new HttpClient();
                var postTask = client.PostAsync(url, httpContent);
                postTask.Wait();

                var readTask = postTask.Result.Content.ReadAsStringAsync();
                readTask.Wait();

                rtv.Data = readTask.Result;
                rtv.Success = true;
            }
            catch (AggregateException ex)
            {
                rtv.Message = ex.InnerException.Message;
            }
            catch (Exception ex)
            {
                rtv.Message = ex.Message;
            }

            return rtv;
        }
    }
}
