using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WeChat.Api.Model;

namespace WeChat.Api
{
    /// <summary>
    /// Crm接口拓展方法
    /// </summary>
    public static class CrmNoticeExtends
    {
        /// <summary>
        /// Crm客户信息对接接口
        /// </summary>
        /// <param name="Json">客户信息</param>
        /// <returns></returns>
        public static void ClientInfo(string Json)
        {
            //调用芯达通接口，传递订单数据
            var apisetting = new PvCrmApiSetting();
            var apiurl = apisetting.ApiUrl + apisetting.ClientInfo;
            Task<string> task = new Task<string>(() =>
            {
                return HttpPostRaw(apiurl, Json);

            });
            task.Start();
            TaskContinue(task);
        }

        /// <summary>
        /// 后续任务处理
        /// </summary>
        /// <param name="task"></param>
        private static void TaskContinue(Task<string> task)
        {
            task.ContinueWith(t =>
            {
                var a = task.Result;
                var message = a.JsonTo<JMessage>();
                if (!message.success)
                {
                    Needs.Ccs.Services.Models.DeliveryNoticeApiLog apiLog = new Needs.Ccs.Services.Models.DeliveryNoticeApiLog()
                    {
                        ID = Guid.NewGuid().ToString("N"),
                        BatchID = "",
                        OrderID = "",
                        TinyOrderID ="",
                        RequestContent ="微信公众号注册",
                        Status = Needs.Ccs.Services.Enums.Status.Normal,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        Summary = message.data
                    };
                    apiLog.Enter();
                }
            }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        private static string HttpPostRaw(string url, string data)
        {
            var value = "";
            try
            {
                HttpWebRequest reqest = (HttpWebRequest)WebRequest.Create(url);
                reqest.Method = "POST";
                reqest.ContentType = "application/json";
                Stream stream = reqest.GetRequestStream();
                byte[] bs = System.Text.Encoding.UTF8.GetBytes(data);
                stream.Write(bs, 0, bs.Length);
                stream.Flush();
                stream.Close();
                HttpWebResponse response = (HttpWebResponse)reqest.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                value = sr.ReadToEnd();
                response.Close();
                return value;
            }
            catch (Exception ex)
            {

                return "";
            }
        }
    }
}
