using System;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using Yahv.PvWsOrder.Services;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.PvWsOrder.WebApi.Models;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Mvc;

namespace Yahv.PvWsOrder.WebApi.Controllers
{
    public class ClassifyController : ClientController
    {
        /// <summary>
        /// 归类完成，提交归类信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SubmitClassified(ClassifiedResult result)
        {
            try
            {
                var item = Erp.Current.WsOrder.OrderItems.Where(t => t.InputID == result.ItemID && t.Type == OrderItemType.Modified).FirstOrDefault();
                if (item == null)
                {
                    throw new Exception("订单项" + result.ItemID + "不存在");
                }
                Services.Models.ClassifiedResult rel = new Services.Models.ClassifiedResult();
                rel.ItemID = result.ItemID;
                rel.MainID = result.MainID;
                rel.ProductID = result.ProductID;
                rel.HSCodeID = result.HSCodeID;
                rel.CreatorID = result.CreatorID;
                rel.Step = result.Step;
                rel.OriginRate = result.OriginRate;
                rel.FVARate = result.FVARate;
                rel.Ccc = result.Ccc;
                rel.Embargo = result.Embargo;
                rel.HkControl = result.HkControl;
                rel.Coo = result.Coo;
                rel.CIQ = result.CIQ;
                rel.CIQprice = result.CIQprice;
                rel.IsHighPrice = result.IsHighPrice;
                rel.IsDisinfected = result.IsDisinfected;
                rel.IsSysCcc = result.IsSysCcc;
                rel.IsSysEmbargo = result.IsSysEmbargo;
                //人工归类
                item.AdminClassify(rel);
                //返回归类信息
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

        /// <summary>
        /// 继续归类，获取下一条归类信息
        /// </summary>
        /// <param name="step"></param>
        /// <param name="creatorId"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetNext(string step, string creatorId)
        {
            try
            {
                var stepEnum = (ClassifyStep)Enum.Parse(typeof(ClassifyStep), step);
                var next = new Services.Views.Alls.ClassifyProductsAll(stepEnum).GetTop(1, i => !i.IsLocked || i.Locker.ID == creatorId).FirstOrDefault();
                if (next == null)
                {
                    return Json(new JSingle<dynamic>() { code = 100, success = true, data = null }, JsonRequestBehavior.AllowGet);
                }

                //归类锁定
                JMessage result = next.Lock(creatorId, step);
                if (result.code == 300)
                {
                    //锁定失败，抛出异常
                    throw new Exception(result.data);
                }

                ////查询订单的合同发票
                string pis = new Services.Views.OrderFilesRoll(next.OrderID).Where(item => item.Type == (int)FileType.Invoice)
                    .ToList().Select(item => new
                    {
                        ID = item.ID,
                        FileName = item.CustomName,
                        FileFormat = "",
                        Url = Services.Common.FileDirectory.ServiceRoot + item.Url,
                    }).Json();

                //返回归类信息
                var pvdataApi = new PvDataApiSetting();
                var pvwsorderApi = new PvWsOrderApiSetting();
                var json = new JSingle<dynamic>()
                {
                    code = 200,
                    success = true,
                    data = new
                    {
                        ItemID = next.InputID,
                        MainID = next.OrderID,
                        OrderedDate = next.OrderedDate.ToString("yyyy-MM-dd HH:mm:ss"),
                        ClientCode = next.WsClient.EnterCode,
                        ClientName = next.WsClient.Name,

                        next.Product.PartNumber,
                        next.Product.Manufacturer,
                        //Origin = ((Origin)Enum.Parse(typeof(Origin), next.Origin)).GetOrigin().Code,
                        Origin = next.Origin.GetOrigin().Code,
                        UnitPrice = next.UnitPrice.ToString("0.0000"),
                        next.Quantity,
                        Unit = next.Unit.GetUnit().Code,
                        Currency = next.Currency?.GetCurrency().ShortName,
                        TotalPrice = next.TotalPrice.ToString("0.0000"),
                        CreateDate = next.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),

                        next.ClassifiedPartNumber?.HSCode,
                        next.ClassifiedPartNumber?.TariffName,
                        ImportPreferentialTaxRate = next.ClassifiedPartNumber?.ImportPreferentialTaxRate.ToString("0.0000"),
                        VATRate = next.ClassifiedPartNumber?.VATRate.ToString("0.0000"),
                        ExciseTaxRate = next.ClassifiedPartNumber?.ExciseTaxRate.ToString("0.0000"),
                        next.ClassifiedPartNumber?.TaxCode,
                        next.ClassifiedPartNumber?.TaxName,
                        next.ClassifiedPartNumber?.LegalUnit1,
                        next.ClassifiedPartNumber?.LegalUnit2,
                        next.ClassifiedPartNumber?.CIQCode,
                        next.ClassifiedPartNumber?.Elements,

                        OriginATRate = next.OrderItemsTerm?.OriginRate.ToString("0.0000"),
                        CIQ = next.OrderItemsTerm?.CIQ ?? false,
                        CIQprice = next.OrderItemsTerm?.CIQprice,
                        Ccc = next.OrderItemsTerm?.Ccc ?? false,
                        Embargo = next.OrderItemsTerm?.Embargo ?? false,
                        HkControl = next.OrderItemsTerm?.HkControl ?? false,
                        Coo = next.OrderItemsTerm?.Coo ?? false,
                        IsHighPrice = next.OrderItemsTerm?.IsHighPrice ?? false,

                        PIs = pis,
                        PvDataApiUrl = ConfigurationManager.AppSettings[pvdataApi.ApiName],
                        CallBackUrl = ConfigurationManager.AppSettings[pvwsorderApi.ApiName] + pvwsorderApi.SubmitClassified,
                        NextUrl = ConfigurationManager.AppSettings[pvwsorderApi.ApiName] + pvwsorderApi.GetNext,
                        Step = step,
                        CreatorID = creatorId
                    }
                };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var json = new JSingle<dynamic>() { code = 300, success = false, data = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 同步Ccc
        /// </summary>
        /// <param name="itemID">OrderItemID</param>
        /// <param name="isCcc">是否Ccc</param>
        /// <returns></returns>
        public ActionResult SyncCccControl(string itemID, bool isCcc)
        {
            try
            {
                var term = new Yahv.PvWsOrder.Services.Models.OrderItemsTerm()
                {
                    ID = itemID,
                    Ccc = isCcc
                };
                term.UpdateCcc();

                var json = new JMessage()
                {
                    code = 200,
                    success = true,
                    data = "同步成功"
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