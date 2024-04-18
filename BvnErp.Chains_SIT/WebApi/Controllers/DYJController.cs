using Layer.Data.Sqls.ScCustoms;
using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using Needs.Underly;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using WebApi.Models;

namespace WebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [System.Web.Http.Cors.EnableCors(origins: "*", headers: "*", methods: "*")]
    public class DYJController : MyApiController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="InvoiceNo"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public ActionResult GetAddedTaxDetail(string InvoiceNo)
        {
            try
            {
                var json = new JMessage();
                if (string.IsNullOrEmpty(InvoiceNo))
                {
                    json.code = 100;
                    json.success = false;
                    json.data = "参数InvoiceNo为空!";
                    return Json(json, JsonRequestBehavior.AllowGet);
                }

                if (!new Needs.Ccs.Services.Views.AddedTaxDetailsView().CheckInvoiceNo(InvoiceNo))
                {
                    json.code = 100;
                    json.success = false;
                    json.data = "此发票号错误(非大赢家报关数据)!";
                    return Json(json, JsonRequestBehavior.AllowGet);
                }


                //记录日志
                string batchID = Guid.NewGuid().ToString("N");
                Needs.Ccs.Services.Models.DeliveryNoticeApiLog apiLog = new Needs.Ccs.Services.Models.DeliveryNoticeApiLog()
                {
                    ID = Guid.NewGuid().ToString("N"),
                    BatchID = batchID,
                    RequestContent = "InvoiceNo:" + InvoiceNo,
                    Status = Needs.Ccs.Services.Enums.Status.Normal,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Summary = "大赢家请求增值税数据"
                };
                apiLog.Enter();


                //
                var view = new Needs.Ccs.Services.Views.AddedTaxDetailsView();
                var results = view.GetDetailAddedTaxInfo(view.GetAddedTaxListIDs(InvoiceNo).ToArray());
                var decheadid = results.FirstOrDefault().DeclarationID;

                //返回内容
                var model = new Needs.Ccs.Services.Models.DyjAddedTaxResponse();
                var items = new List<DyjAddedTaxItem>();
                foreach (var m in results)
                {
                    var item = new DyjAddedTaxItem();
                    item.ID = m.DYJOrderID;
                    item.Model = m.GoodsModel;
                    item.ProductName = m.GName;
                    item.TaxName = m.TaxName;
                    item.TaxCode = m.TaxCode;
                    item.Unit = m.GUnit;
                    item.Quantity = m.GQty;
                    item.UnitPrice = (m.CustomsValue / m.GQty).ToRound(4);
                    item.Amount = m.CustomsValue;
                    item.TaxRate = m.Vat;
                    item.AddedTaxValue = m.CustomsValueVat;
                    item.RealAddedTaxValue = m.ValueVatPayed;

                    model.TotalQuantity += m.GQty;
                    model.TotalAmount += m.CustomsValue;
                    model.TaxAmount += m.ValueVatPayed;

                    items.Add(item);
                }

                model.InvoiceItems = items;
                model.ContractNo = InvoiceNo;

                //实际交款时间、实际缴纳关税
                var flow = view.GetTaxFolw().Where(t=>t.DecheadID == decheadid).FirstOrDefault();
                if (flow != null)
                {
                    model.FileUrl = "";
                    model.InvoiceDate = flow.PayDate.Value.ToString("yyyy-MM-dd");
                    model.TaxAmount = flow.Amount;
                }
                else
                {
                    model.FileUrl = "";
                    model.InvoiceDate = results.First().DecHeadDDate.Value.ToString("yyyy-MM-dd");
                }

                string FileServerUrl = System.Configuration.ConfigurationManager.AppSettings["FileServerUrl"];
                var file = view.GetAddedTaxFile().Where(t=>t.DecHeadID == decheadid).FirstOrDefault();
                if (file != null)
                {
                    model.FileUrl = FileServerUrl + @"/" + file.Url.ToUrl(); 
                }

                model.Code = 200;
                model.Success = true;

                return Json(model, JsonRequestBehavior.AllowGet);

                //return new HttpResponseMessage()
                //{
                //    Content = new StringContent(JsonConvert.SerializeObject(model), System.Text.Encoding.UTF8, "application/json"),
                //};
            }
            catch (Exception ex)
            {
                ex.CcsLog("大赢家获取增值税");
                var json = new JMessage() { code = 300, success = false, data = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public ActionResult GetPayexchangeFile(string pid)
        {
            var dyjid = pid + ".0";
            var payexchange = new Needs.Ccs.Services.Views.AdminPayExchangeApplyView().Where(t => t.DyjID == dyjid).FirstOrDefault();


            var json = new JMessage();
            if (payexchange == null)
            {
                json.code = 100;
                json.success = false;
                json.data = "无此付汇申请!";
                return Json(json, JsonRequestBehavior.AllowGet);
            }

            //付汇委托书
            var applyFile = new Needs.Ccs.Services.Views.PayExchangeApplyFileView().
                Where(item => item.PayExchangeApplyID == payexchange.ID && item.FileType == Needs.Ccs.Services.Enums.FileType.PayExchange).FirstOrDefault();
            if (applyFile != null)
            {
                var url = FileDirectory.Current.PvDataFileUrl + "/" + applyFile.Url.ToUrl();
                return Json(new {code = 200,success = true, data = url }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { code = 100, success = false, data = "此单无付汇委托书，请联系相关客服" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}