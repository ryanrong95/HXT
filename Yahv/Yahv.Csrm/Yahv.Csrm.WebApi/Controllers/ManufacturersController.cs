using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Yahv.Utils.Serializers;
using Yahv.Web.Mvc;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApi.Controllers
{
    public class ManufacturersController : ClientController
    {
        string standardhost = System.Configuration.ConfigurationManager.AppSettings["standardhost"];
        // GET: Manufacturershttp://hv.erp.b1b.com/csrmapi/Properties/
        public ActionResult Index(string key, string callback)
        {
            string url = string.Format($"{standardhost}/Api/standard/manufacturers?key={key}");
            WebRequest request = WebRequest.Create(url);
            request.Method = "get";
            // 添加header
            //request.Headers.Add("key", key);

            //读取返回消息
            string res = string.Empty;
            List<ViewManufacturer> list = new List<ViewManufacturer>();
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                res = reader.ReadToEnd();
                reader.Close();
                //网站的数据
                var arry = res.Replace("&quot;", "\"").JsonTo<ViewManufacturer[]>();
                //后台管理数据
                var my = new ManufacturersRoll().Where(item => item.Name.Contains(key)).ToArray();
                for (int i = 0; i < arry.Length; i++)
                {
                    var model = arry[i];
                    if (my.Any(item => item.Name == model.Name))
                    {
                        var mymodel = my.First(item => item.Name == model.Name);
                        list.Add(new ViewManufacturer
                        {
                            Name = model.Name,
                            Agent = mymodel.Agent
                        });
                    }
                    else
                    {
                        list.Add(new ViewManufacturer
                        {
                            Name = model.Name,
                            Agent = model.Agent
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return this.Jsonp(new Result { Code = "300", Data = ex.Message }, callback);
            }


            if (string.IsNullOrWhiteSpace(callback))
            {
                return this.Json(new Result { Code = "200", Data = list }, JsonRequestBehavior.AllowGet);
            }

            return this.Jsonp(new Result { Code = "200", Data = list }, callback);
            // return View();
        }
        /// <summary>
        /// 标准品牌验证
        /// </summary>
        /// <param name="key"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public ActionResult Validate(string key, string callback)
        {
            string url = string.Format($"{standardhost}/Api/validate/manufacturers?key={key}");
            WebRequest request = WebRequest.Create(url);
            request.Method = "get";
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            //读取返回消息
            string res = string.Empty;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                res = reader.ReadToEnd();
                reader.Close();

                //网站的数据
                dictionary = res.Replace("&quot;", "\"").JsonTo<Dictionary<string, string>>();
                if (dictionary["Standard"] == "yes")
                {
                    if (new ManufacturersRoll().Any(item => item.Name == key))
                    {
                        dictionary["IsAgent"] = new ManufacturersRoll().First(item => item.Name == key).Agent.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                return this.Jsonp(new { Error = ex.Message }, callback);
            }

            return this.Jsonp(dictionary, callback);
        }

    }
}