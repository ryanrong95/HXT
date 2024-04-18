using Needs.Ccs.Services.Enums;
using Needs.Linq;
using Needs.Utils.Descriptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class IcgooPost
    {
        public string id { get; set; }
        public string sale_orderline_id { get; set; }
        public string partno { get; set; }
        public string supplier { get; set; }
        public string mfr { get; set; }
        public string brand { get; set; }
        public string origin { get; set; }
        public decimal? customs_rate { get; set; }
        public decimal? add_rate { get; set; }
        public string product_name { get; set; }
        public string category { get; set; }
        public int? type { get; set; }
        public string hs_code { get; set; }
        public string tax_code { get; set; }
        public string PostUrl { get; set; }
        public int classifyType { get; set; }
        public IcgooPost()
        {

        }
        public IcgooPost(CompanyTypeEnums companyType)
        {
            this.PostUrl = System.Configuration.ConfigurationManager.AppSettings[companyType.GetDescription()];
        }

        public void PostData()
        {
            string postdata = toJson();
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("packetdata", postdata);
            string result = Post(dic);

            List<PostBack> postbacks = JsonConvert.DeserializeObject<List<PostBack>>(result);

            foreach (PostBack p in postbacks)
            {
                p.ID = ChainsGuid.NewGuidUp();
                p.id = this.id;
                p.status = p.status;
                p.msg = p.msg;
                p.CreateDate = DateTime.Now;
                p.RecordStatus = Status.Normal;
                p.Enter();
            }
        }

        public string Post(Dictionary<string, string> dic)
        {
            string result = "";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(this.PostUrl);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            SetHeaderValue(req.Headers, Icgoo.IFAuthorizationName, Icgoo.IFAuthorization);
            #region 添加Post 参数
            StringBuilder builder = new StringBuilder();
            int i = 0;
            foreach (var item in dic)
            {
                if (i > 0)
                    builder.Append("&");
                builder.AppendFormat("{0}={1}", item.Key, item.Value);
                i++;
            }
            byte[] data = Encoding.UTF8.GetBytes(builder.ToString());
            req.ContentLength = data.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }
            #endregion
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();
            //获取响应内容
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }
        private string toJson()
        {
            string json = JsonConvert.SerializeObject(new
            {
                id = this.id,
                sale_orderline_id = this.sale_orderline_id,
                partno = this.partno,
                supplier = this.supplier,
                mfr = this.mfr,
                brand = this.brand,
                origin = this.origin != null ? StringToUnicode(this.origin) : "",
                customs_rate = this.customs_rate,
                add_rate = this.add_rate,
                product_name = StringToUnicode(this.product_name),
                category = this.category != null ? StringToUnicode(this.category) : "",
                type = this.classifyType,
                hs_code = this.hs_code,
                tax_code = this.tax_code,
            });

            string postjson = "{\"total_item_num\":1,\"custom_partner\":94,\"item\":[ " + json + "]}";

            return postjson.Replace("&", "%26");
        }
        private static string StringToUnicode(string value)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(value);
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i += 2)
            {
                stringBuilder.AppendFormat("\\u{0}{1}", bytes[i + 1].ToString("x").PadLeft(2, '0'), bytes[i].ToString("x").PadLeft(2, '0'));
            }
            return stringBuilder.ToString();
        }

        private static void SetHeaderValue(WebHeaderCollection header, string name, string value)
        {
            var property = typeof(WebHeaderCollection).GetProperty("InnerCollection", BindingFlags.Instance | BindingFlags.NonPublic);
            if (property != null)
            {
                var collection = property.GetValue(header, null) as NameValueCollection;
                collection[name] = value;
            }
        }

        public string PostInside(string postMessage)
        {
            string result = "";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(this.PostUrl);
            req.Method = "POST";
            req.ContentType = "application/json";                  
            #region 添加Post 参数
          
            byte[] data = Encoding.UTF8.GetBytes(postMessage);
            req.ContentLength = data.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }
            #endregion
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();
            //获取响应内容
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }

        public string PostIcgooDutiablePrice(string postMessage)
        {
            string postdata = postMessage.Replace("&", "%26");
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("packetdata", postdata);
            string result = Post(dic);

            return result;
        }
       
    }
}
