using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Yahv.PvData.WebApi.Models;
using Yahv.Underly;
using Yahv.Web.Mvc;
using YaHv.PvData.Services.Models;

namespace Yahv.PvData.WebApi.Controllers
{
    public class ProductController : ClientController
    {
        private Encoding defaultEncoding = Utils.Http.ApiHelper.Current.DefaultEncoding;

        // GET: Order
        public ActionResult Index()
        {
            var json = new JMessage() { code = 200, success = true, data = "ok" };
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取产品ID
        /// </summary>
        /// <param name="partNumber">型号</param>
        /// <param name="manufacturer">品牌/制造商</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetIdByInfo(string partNumber, string manufacturer)
        {
            try
            {
                partNumber = HttpUtility.UrlDecode(partNumber, defaultEncoding);
                manufacturer = HttpUtility.UrlDecode(manufacturer, defaultEncoding);

                var product = new Product
                {
                    PartNumber = partNumber,
                    Manufacturer = manufacturer
                };
                product.EnterSuccess += Product_EnterSuccess;
                product.Enter();

                var json = new JMessage() { code = 200, success = true, data = product.ID };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var json = new JMessage() { code = 300, success = false, data = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        private void Product_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {

        }

        /// <summary>
        /// 产品报价
        /// </summary>
        /// <param name="result">报价信息</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Quote(QuotedResult result)
        {
            try
            {
                var productQuote = new ProductQuote()
                {
                    PartNumber = result.PartNumber,
                    Manufacturer = result.Manufacturer,
                    Origin = result.Origin,
                    Currency = result.Currency,
                    UnitPrice = result.UnitPrice,
                    Quantity = result.Quantity,
                    CIQprice = result.CIQprice
                };
                productQuote.Enter();

                var json = new JMessage()
                {
                    code = 200,
                    success = true,
                    data = "提交成功"
                };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var json = new JMessage() { code = 300, success = false, data = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }
    }
}