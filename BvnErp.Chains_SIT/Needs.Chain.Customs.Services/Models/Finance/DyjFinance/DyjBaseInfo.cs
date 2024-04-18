using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public static class DyjBaseInfo
    {
        public static DyjBaseInfoData GetBaseInfo()
        {
            var api = System.Configuration.ConfigurationManager.AppSettings["DYJFeeApplyApiURL"];
            //调用接口
            var post = PostDYJ(api + DyjCwConfig.GetFeeApplyABaseInfo + "?key=" + DyjCwConfig.Key, "Get", string.Empty);
            //格式化json字符串
            var result = new Newtonsoft.Json.JsonSerializer().Deserialize(new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader(post)));
            //反序列化Response
            var response = Newtonsoft.Json.JsonConvert.DeserializeObject<DyjBaseInfoResponse>(result.ToString());
            if (!response.isSuccess || response.status != "200")
            {
                var ex = new Exception(response.message);
                ex.CcsLog("付款调用大赢家错误-获取基础信息");
            }
            return response.data;
        }

        /// <summary>
        /// 调用大赢家接口
        /// </summary>
        /// <param name="api"></param>
        /// <returns></returns>
        public static string PostDYJ(string url,string method, string requestData)
        {
            string result = "";

            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

                req.Method = method;// "POST";

                req.Timeout = 30 * 1000;//设置请求超时时间，单位为毫秒

                req.ContentType = "application/json";

                byte[] data = Encoding.UTF8.GetBytes(requestData);

                if (method != "Get")
                {
                    req.ContentLength = data.Length;

                    using (Stream reqStream = req.GetRequestStream())
                    {
                        reqStream.Write(data, 0, data.Length);

                        reqStream.Close();
                    }
                }

                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

                Stream stream = resp.GetResponseStream();

                //获取响应内容
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                }

                return result;
            }
            catch (Exception ex)
            {
                ex.CcsLog("调用大赢家接口错误" + url);
                throw ex;
            }
           
        }

    }

    #region 大赢家接口Response固定框架

    /// <summary>
    /// 大赢家接口回执
    /// </summary>
    public class DyjResponse
    {
        //"\"{\\\"status\\\":\\\"200\\\",\\\"message\\\":\\\"成功\\\",\\\"isSuccess\\\":true,\\\"isPage\\\":false,\\\"pageInfo\\\":{\\\"counts\\\":0,\\\"pagesize\\\":50,\\\"pages\\\":1,\\\"currentpage\\\":1},\\\"list\\\":null,\\\"data\\\":null}\""
        public string status { get; set; }

        public string message { get; set; }

        public bool isSuccess { get; set; }

        public bool isPage { get; set; }

        public DyjPageInfo pageInfo { get; set; }

        public string list { get; set; }

        public string data { get; set; }

        public string filelist { get; set; }

        public string taken { get; set; }
    }

    public class DyjPageInfo
    {
        public int counts { get; set; }

        public int pagesize { get; set; }

        public int pages { get; set; }

        public int currentpage { get; set; }
    }

    #endregion


    #region 大赢家基础信息返回结果

    public class DyjBaseInfoResponse : DyjResponse
    { 
        public DyjBaseInfoData data { get; set; }
    }

    public class DyjBaseInfoData 
    {
        public List<DyjBaseInfoData币种> 币种 { get; set; }

        public List<DyjBaseInfoData指定审批人> 指定审批人 { get; set; }

        public List<DyjBaseInfoData开票类型> 开票类型 { get; set; }

        public List<DyjBaseInfoData付款公司> 付款公司 { get; set; }

        public List<DyjBaseInfoData付款类型> 付款类型 { get; set; }
    }

    public class DyjBaseInfoData币种
    {
        public int 编号 { get; set; }

        public string 名称 { get; set; }

        public decimal 汇率 { get; set; }

        public string 单位 { get; set; }
    }

    public class DyjBaseInfoData指定审批人
    {
        public int 编号 { get; set; }

        public string 名称 { get; set; }
    }

    public class DyjBaseInfoData开票类型
    {
        public int 编号 { get; set; }

        public string 名称 { get; set; }
    }

    public class DyjBaseInfoData付款公司
    {
        public int 编号 { get; set; }

        public string 名称 { get; set; }

        public bool 增值税票 { get; set; }

        public bool 一般纳税人普票 { get; set; }

        public bool 普通发票 { get; set; }

        public bool 国外公司 { get; set; }
    }

    public class DyjBaseInfoData付款类型
    {
        public int 编号 { get; set; }

        public string 名称 { get; set; }

        public string 类型 { get; set; }
    }

    #endregion


    /// <summary>
    /// 收款方式  1-现金 2-支票 3-电汇 4-承兑 5-转账 6-同城转账 7-内部转账 9-预收(付)转应收(付)
    /// </summary>
    public enum DyjReceiptType
    {
        /// <summary>
        /// 现金
        /// </summary>
        [Description("现金")]
        Cash = 1,

        /// <summary>
        /// 支票
        /// </summary>
        [Description("支票")]
        Check = 2,

        /// <summary>
        /// 电汇
        /// </summary>
        [Description("电汇")]
        Wire = 3,

        /// <summary>
        /// 承兑汇票
        /// </summary>
        [Description("承兑")]
        AcceptanceBill = 4,

        /// <summary>
        /// 转账
        /// </summary>
        [Description("转账")]
        TransferAccount = 5,
    }

    /// <summary>
    /// 币种ID 1-人民币 2-美元 3-港币 4-欧元 5-英镑 7-日元
    /// </summary>
    public enum DyjCurrency
    {
        /// <summary>
        /// 人民币
        /// </summary>
        [Description("人民币")]
        CNY = 1,

        /// <summary>
        /// 美元
        /// </summary>
        [Description("美元")]
        USD = 2,

        /// <summary>
        /// 港币
        /// </summary>
        [Description("港币")]
        HKD = 3,

        /// <summary>
        /// 欧元
        /// </summary>
        [Description("欧元")]
        EUR = 4,

        /// <summary>
        /// 英镑
        /// </summary>
        [Description("英镑")]
        GBP = 5,

        /// <summary>
        /// 日元
        /// </summary>
        [Description("日元")]
        JPY = 7,
    }

}
