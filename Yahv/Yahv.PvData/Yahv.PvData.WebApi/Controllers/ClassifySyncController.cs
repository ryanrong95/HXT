using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.PvData.WebApi.Models;
using Yahv.Underly;
using Yahv.Web.Mvc;
using YaHv.PvData.Services.Models;
using YaHv.PvData.Services.Utils;

namespace Yahv.PvData.WebApi.Controllers
{
    public class ClassifySyncController : ClientController
    {
        /// <summary>
        /// 同步归类结果
        /// </summary>
        /// <param name="results">归类结果</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SyncClassified(List<ClassifiedResult> results)
        {
            try
            {
                List<OrderedProduct> ops = new List<OrderedProduct>();
                foreach (var result in results)
                {
                    var op = GetOrderedProduct(result);
                    ops.Add(op);
                }
                SyncManager.Current.Classify.For(ops.ToArray()).DoSync();

                //归类数据同步完成
                var json = new JMessage()
                {
                    code = 200,
                    success = true,
                    data = "同步完成"
                };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var json = new JMessage() { code = 300, success = false, data = $"异常信息：{ex.Message}     \r\n堆栈信息：{ex.StackTrace}" };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 同步税费变更
        /// </summary>
        /// <param name="results">归类结果</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SyncTaxChanged(List<ClassifiedResult> results)
        {
            try
            {
                List<OrderedProduct> ops = new List<OrderedProduct>();
                foreach (var result in results)
                {
                    var op = GetOrderedProduct(result);
                    ops.Add(op);
                }
                SyncManager.Current.TaxChange.For(ops.ToArray()).DoSync();

                //归类数据同步完成
                var json = new JMessage()
                {
                    code = 200,
                    success = true,
                    data = "同步完成"
                };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var json = new JMessage() { code = 300, success = false, data = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        private OrderedProduct GetOrderedProduct(ClassifiedResult result)
        {
            return new OrderedProduct()
            {
                ID = result.ItemID,
                PartNumber = result.PartNumber,
                Manufacturer = result.Manufacturer,

                HSCode = result.HSCode,
                TariffName = result.TariffName,
                TaxCode = result.TaxCode,
                TaxName = result.TaxName,
                LegalUnit1 = result.LegalUnit1,
                LegalUnit2 = result.LegalUnit2,
                VATRate = result.VATRate,
                ImportPreferentialTaxRate = result.ImportPreferentialTaxRate,
                OriginATRate = result.OriginRate,
                ExciseTaxRate = result.ExciseTaxRate,
                CIQCode = result.CIQCode,
                Elements = result.Elements,

                Ccc = result.Ccc,
                Embargo = result.Embargo,
                HkControl = result.HkControl,
                Coo = result.Coo,
                CIQ = result.CIQ,
                CIQprice = result.CIQprice,
                IsHighPrice = result.IsHighPrice,
                IsDisinfected = result.IsDisinfected,

                CreatorID = result.CreatorID,

                CreateDate = result.CreateDate,
                UpdateDate = result.UpdateDate
            };
        }
    }
}