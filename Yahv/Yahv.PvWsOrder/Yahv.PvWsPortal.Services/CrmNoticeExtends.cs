using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using Yahv.Utils.Serializers;

namespace Yahv.PvWsPortal.Services
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

            Logger.log("NPC", new Yahv.Services.Models.OperatingLog
            {
                MainID = "",
                Operation = "官网会员注册",
                Summary = Json,
            });

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
                Logger.log("NPC", new Yahv.Services.Models.OperatingLog
                {
                    MainID = "",
                    Operation = "官网会员注册",
                    Summary = message.data,
                });
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
                Logger.log("HttpPostRaw", new Yahv.Services.Models.OperatingLog
                {
                    MainID = "error",
                    Operation = "接口调用异常",
                    Summary = ex.Message,
                });
                return "";
            }
        }
    }
}
