using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.Underly;
using Yahv.Web.Mvc;
using YaHv.PvData.Services.Extends;

namespace Yahv.PvData.WebApi.Controllers
{
    public class ClassifyInfoController : ClientController
    {
        /// <summary>
        /// 获取归类信息
        /// </summary>
        /// <param name="cpnId">ClassifiedPartNumber ID</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index(string cpnId)
        {
            try
            {
                var cpn = new YaHv.PvData.Services.Views.Alls.ClassifiedPartNumbersAll()[cpnId];
                if (cpn == null)
                {
                    throw new Exception("ClassifiedPartNumberID：【" + cpnId + "】不存在！");
                }

                //返回归类信息
                var info = cpn.FillClassifiedInfo();
                var json = new JSingle<object>()
                {
                    code = 200,
                    success = true,
                    data = new
                    {
                        PartNumber = info.PartNumber,
                        Manufacturer = info.Manufacturer,

                        HSCode = info.HSCode,
                        TariffName = info.TariffName,
                        LegalUnit1 = info.LegalUnit1,
                        LegalUnit2 = info.LegalUnit2,
                        VATRate = info.VATRate,
                        ImportPreferentialTaxRate = info.ImportPreferentialTaxRate,
                        ImportGeneralTaxRate = info.ImportGeneralTaxRate,
                        ExciseTaxRate = info.ExciseTaxRate,
                        Elements = info.ElementsDic,
                        CIQCode = info.CIQCode,
                        TaxCode = info.TaxCode,
                        TaxName = info.TaxName,

                        Ccc = info.Ccc,
                        Embargo = info.Embargo,
                        HkControl = info.HkControl,
                        Coo = info.Coo,
                        CIQ = info.CIQ,
                        CIQprice = info.CIQprice
                    }
                };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var json = new JSingle<object>() { code = 300, success = false, data = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }
    }
}