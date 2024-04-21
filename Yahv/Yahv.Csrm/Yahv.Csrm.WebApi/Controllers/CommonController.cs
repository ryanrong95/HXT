using Layers.Data;
using Layers.Data.Sqls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Yahv.Csrm.WebApi.Models;
using Yahv.Services;
using Yahv.Systematic;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Mvc;


namespace Yahv.Csrm.WebApi.Controllers
{
    public class CommonController : ClientController
    {
        string wayjd = System.Configuration.ConfigurationManager.AppSettings["wayjd"];
        string appkey = System.Configuration.ConfigurationManager.AppSettings["AppKey"];
        /// <summary>
        /// 精准查询
        /// </summary>
        /// <returns></returns>
        public ActionResult GetEnterpriseByName(string key, string adminID = null, string siteuserID = null)
        {
            try
            {
              var response = Commons.CommonHttpRequest(wayjd + "/detail_info?name=" + key + "&appkey=" + appkey, "POST");
              //暂时写死测试
              //  var tempPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\jdBackData.json");
               // var response = System.IO.File.ReadAllText(tempPath,Encoding.UTF8);
                Task t1 = new Task(() =>
                {
                    if (response != null)
                    {
                        using (PvDataReponsitory reponsitory = new PvDataReponsitory())
                        {

                            reponsitory.Insert(new Layers.Data.Sqls.PvData.Logs_WayjdByName
                            {
                                ID = PKeySigner.Pick(PKeyType.Logs_WayjdByName),
                                RequestContent = key,
                                ResponseContent = response,
                                CreateDate = DateTime.Now,
                                AdminID = adminID,
                                SiteUserID = siteuserID,
                            });

                        };
                    }
                });
                // t1.Start();
                // Task.WaitAll(t1);
                return Json(new JMessage { code = 200, success = true, data = response }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 300, success = false, data = ex.Message }, JsonRequestBehavior.AllowGet);
            }

           // return eJson();
        }

        /// <summary>
        /// 模糊查询
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="adminID">管理端操作人</param>
        /// <param name="siteuser">客户端操作人</param>
        /// <returns></returns>
        public ActionResult GetEnterpriseByKeyword(string keyword, string adminID = null, string siteuser = null)
        {
             var response = Commons.CommonHttpRequest(wayjd + "/search_info4?keyword=" + keyword + "&appkey=" + appkey, "POST");
            // var response = System.IO.File.ReadAllText(@"D:\Projects_vs2015\Yahv\Yahv.Csrm\Yahv.Csrm.WebApi\App_Data\jdBackData2.json", Encoding.UTF8);
           // var tempPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data\\jdBackData2.json");
           
           // var response = System.IO.File.ReadAllText(tempPath, Encoding.UTF8);
            try
            {
                if (response != null)
                {
                    var jObject = JObject.Parse(response);

                    var jResult = jObject["result"];
                    var JsonResult = jResult["result"];
                    var data = new
                    {
                        code = jObject["code"].Value<string>(),
                        result = new
                        {
                            error_code = jResult["error_code"].Value<string>(),
                            result = new
                            {
                                num = JsonResult["num"].Value<int>(),
                                items = JsonResult["items"].Select(item => new
                                {
                                    id = item["id"].Value<string>(),
                                    name = item["name"].Value<string>(),
                                    estiblishTime = item["estiblishTime"].Value<DateTime>(),
                                    legalPersonName = item["legalPersonName"].Value<string>()
                                }),
                            }
                        }
                    };
                    //异步批量插入 ，防止前台请求速度变慢
                    Task t1 = new Task(() =>
                    {
                        try
                        {

                            using (PvDataReponsitory reponsitory = new PvDataReponsitory())
                            {

                                var offerids = reponsitory.ReadTable<Layers.Data.Sqls.PvData.Logs_WayjdByKeyword>().Select(x => x.OfferID).ToArray();
                              
                                reponsitory.Insert(data.result.result.items.Where(x=>offerids.Contains(x.id)==false).Select(item => new Layers.Data.Sqls.PvData.Logs_WayjdByKeyword
                                {
                                    ID = PKeySigner.Pick(PKeyType.Logs_WayjdByKeyword),
                                    OfferID = item.id,
                                    Name = item.name,
                                    EstiblishTime = item.estiblishTime,
                                    LegalPersonName = item.legalPersonName,
                                    CreateDate = DateTime.Now,
                                    AdminID = adminID,
                                    SiteuserID = siteuser
                                }));

                            }

                        }
                        catch (Exception ex)
                        {
                            Json(new JMessage { code = 400, success = false, data = "表：Logs_WayjdByKeyword 操作失败：" + ex }, JsonRequestBehavior.AllowGet);
                        }

                    });
                }

             return   Json(new JMessage { code = 200, success = true, data = response }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 300, success = false, data = ex.Message }, JsonRequestBehavior.AllowGet);
            //  return   eJson(new JMessage { code = 300, success = false, data = ex.Message });
            }
           
        }
    }
}