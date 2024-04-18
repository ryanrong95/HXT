using Needs.Utils.Descriptions;
using Needs.Wl.Web.WeChat;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using WebWeChat.Models;

namespace WebWeChat.Controllers
{
    [UserAuthorize(UserAuthorize = false)]
    [UserHandleError(ExceptionType = typeof(Exception))]
    public class FinanceManageController : Controller
    {
        /// <summary>
        /// 发票扫码页面
        /// </summary>
        /// <returns></returns>
        public ActionResult InvoiceScan()
        {
            #region 拼接正确的该页面地址

            string host = Request.Url.Host;
            host = Regex.Replace(host, @":(\d)+$", "");
            string controllerName = (string)RouteData.Values["controller"];
            string actionName = (string)RouteData.Values["action"];
            string queryString = Request.QueryString?.ToString();
            queryString = !string.IsNullOrEmpty(queryString) ? "?" + queryString : queryString;

            ViewBag.CurrentUrl = $"http://{host}/{controllerName}/{actionName}{queryString}";

            #endregion

            #region 获取该 InvoiceNotice 信息

            string invoiceNoticeID = Request.QueryString["InvoiceNoticeID"];

            Needs.Ccs.Services.Views.InvoiceNoticeInfoForWxViewModel invoiceNoticeInfo;
            using (var query = new Needs.Ccs.Services.Views.InvoiceNoticeInfoForWxView(invoiceNoticeID))
            {
                invoiceNoticeInfo = query.GetInvoiceNoticeInfo();
            }

            #endregion

            var showInfo = new InvoiceScanReturnModel
            {
                InvoiceNoticeID = invoiceNoticeInfo.InvoiceNoticeID,
                InvoiceTypeName = invoiceNoticeInfo.InvoiceType.GetDescription(),
                ClientID = invoiceNoticeInfo.ClientID,
                CompanyName = invoiceNoticeInfo.CompanyName,
                BankName = invoiceNoticeInfo.BankName,
                BankAccount = invoiceNoticeInfo.BankAccount,
                TaxCode = invoiceNoticeInfo.TaxCode,
                Summary = invoiceNoticeInfo.Summary,
                Amount = invoiceNoticeInfo.Amount.ToString("0.00"),
                Difference = invoiceNoticeInfo.Difference.ToString("0.00"),
            };

            return View(showInfo);
        }

        /// <summary>
        /// 获取发票信息 (根据 InvoiceNoticeID)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public JsonResult GetInvoiceList(GetInvoiceListModel request)
        {
            try
            {
                using (var query = new Needs.Ccs.Services.Views.TaxManageForWxView(request.InvoiceNoticeID))
                {
                    var view = query;

                    return Json(new { success = true, data = view.ToMyPage(), }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, err = ex.Message + "||" + ex.StackTrace, }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 新增发票信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public JsonResult InsertInvoice(InsertInvoiceModel request)
        {
            try
            {
                Needs.Ccs.Services.Views.TaxManageForWxViewModel[] taxManages = null;
                using (var query = new Needs.Ccs.Services.Views.TaxManageForWxView(request.InvoiceNoticeID))
                {
                    taxManages = query.GetTaxManages();
                }

                if (taxManages != null && taxManages.Length > 0)
                {
                    var theSameInvoice = taxManages.Where(t => t.InvoiceCode == request.InvoiceCode
                                                            && t.InvoiceNo == request.InvoiceNo
                                                            && t.InvoiceDate == request.InvoiceDateDt).FirstOrDefault();
                    if (theSameInvoice != null)
                    {
                        return Json(new { success = false, message = "该发票已提交", }, JsonRequestBehavior.AllowGet);
                    }
                }

                decimal amountDec;
                if (!decimal.TryParse(request.InvoiceAmount, out amountDec))
                {
                    return Json(new { success = false, message = "金额不是数值，请联系管理员", }, JsonRequestBehavior.AllowGet);
                }

                if (!Regex.IsMatch(request.InvoiceDate, @"^(\d{4})(\d{2})(\d{2})", RegexOptions.Singleline))
                {
                    return Json(new { success = false, message = "日期格式有变化，请联系管理员", }, JsonRequestBehavior.AllowGet);
                }

                var newTaxManageModel = new Needs.Ccs.Services.Views.TaxManageForWxInsertModel
                {
                    InvoiceNoticeID = request.InvoiceNoticeID,
                    //InvoiceTypeInt = request.InvoiceTypeInt,
                    InvoiceCode = request.InvoiceCode,
                    InvoiceNo = request.InvoiceNo,
                    InvoiceAmount = amountDec,
                    InvoiceDate = (DateTime)request.InvoiceDateDt,
                };
                new Needs.Ccs.Services.Views.TaxManageForWxView().InsertNewInvoice(newTaxManageModel);

                return Json(new { success = true, message = "", }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "提交出错", err = ex.Message + "||" + ex.StackTrace, }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 删除发票信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public JsonResult DeleteInvoice(DeleteInvoiceModel request)
        {
            try
            {
                new Needs.Ccs.Services.Views.TaxManageForWxView().DeleteInvoice(request.TaxManageID, request.InvoiceNoticeID);

                return Json(new { success = true, message = "", }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, err = ex.Message + "||" + ex.StackTrace, }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}