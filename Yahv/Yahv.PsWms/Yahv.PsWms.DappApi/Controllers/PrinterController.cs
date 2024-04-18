using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Yahv.PsWms.PvRoute.Services.Models;
using Yahv.PsWms.PvRoute.Services.Views;
using Yahv.Web.Mvc;

namespace Yahv.PsWms.DappApi.Controllers
{
    public class PrinterController : Controller
    {
        [HttpPost]
        public ActionResult Print(FaceOrder entity)
        {
            using (FaceOrdersTopView orderView = new FaceOrdersTopView())
            {
                orderView.Enter(entity);
            }

            return Json(new
            {
                Success = true,
                Message = "成功录入",
            });
        }

        /// <summary>
        /// 获取面单数据
        /// </summary>
        /// <returns>面单数据</returns>
        [HttpGet]
        public ActionResult GetFaceSheet()
        {

            string code = Request.QueryString["code"];

            //暂时先不用做限制
            //if (string.IsNullOrWhiteSpace(code))
            //{
            //    return Json(null, JsonRequestBehavior.AllowGet);
            //}

            using (var faceOrderView = new FaceOrdersTopView())
            {

                var linq = from faceOrder in faceOrderView
                           where faceOrder.Code == code
                           select new
                           {
                               ID = faceOrder.ID,
                               SendJson = faceOrder.SendJson,
                               Html = faceOrder.Html,
                               MainID = faceOrder.MainID,
                               Source = (int)faceOrder.Source
                           };

                var entity = linq.SingleOrDefault();
                return Json(entity, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult GetKD100Data(string param, string sign)
        {
            #region 第一种写法（接收回调错误）
            //var query = Request.QueryString;

            //var param = query["param"];
            #endregion

            #region 第二种写法（接收回调错误）
            //var arguments = new
            //{
            //    param = jpost["param"].Value<string>(),
            //    sign = jpost["sign"].Value<string>(),
            //};
            #endregion

            //第三种写法：直接string param,string sign接收
            Log(param, sign);

            //{
            //    "result": true,
            //    "returnCode": "200",
            //    "message": "提交成功"
            //}

            return Json(new
            {
                result = true,
                returnCode= "200",
                message="提交成功"
            });
        }

        internal static void Log(string msg, string sign)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "testlog", DateTime.Now.ToString("yyyyMMdd") + ".txt");

            var fileInfo = new FileInfo(path);
            if (!fileInfo.Directory.Exists)
            {
                fileInfo.Directory.Create();
            }

            using (var stream = fileInfo.Open(FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
            {
                writer.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                writer.Write('：');
                writer.WriteLine();
                writer.Write("msg:");
                writer.WriteLine(msg);
                writer.WriteLine();
                writer.Write("sign:");
                writer.WriteLine(sign);
                writer.WriteLine();

                writer.Close();
            }
        }
    }
}