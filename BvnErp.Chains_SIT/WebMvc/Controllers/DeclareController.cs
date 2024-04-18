using Needs.Ccs.Services;
using Needs.Linq;
using Needs.Utils.Npoi;
using Needs.Utils.Serializers;
using Needs.Wl.Web.Mvc;
using Needs.Wl.Web.Mvc.Utils;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebMvc.Models;

namespace WebMvc.Controllers
{
    [UserHandleError(ExceptionType = typeof(Exception))]
    public class DeclareController : UserController
    {
        #region 报关单
        /// <summary>
        /// 报关单
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult DeclareOrder()
        {
            return View();
        }

        ///// <summary>
        ///// 获取报关单数据
        ///// </summary>
        ///// <returns></returns>
        //[UserActionFilter(UserAuthorize = true)]
        //public JsonResult GetDeclareOrders2()
        //{
        //    var orderID = Request.Form["orderID"].ToString().Trim();  //订单编号
        //    var contrNo = Request.Form["contrNo"].ToString().Trim();  //合同编号
        //    var decID = Request.Form["decID"].ToString().Trim();  //报关单号
        //    var startDate = Request.Form["startDate"];  //日期选择
        //    var endDate = Request.Form["endDate"];  //日期选择
        //    var dec = Needs.Wl.User.Plat.UserPlat.Current.WebSite.MyClientDeclareOrderView.AsQueryable();

        //    if ((!string.IsNullOrWhiteSpace(startDate)) && (!string.IsNullOrWhiteSpace(endDate)))
        //    {
        //        var dStart = DateTime.Parse(startDate);
        //        dStart = new DateTime(dStart.Year, dStart.Month, dStart.Day);
        //        var dEnd = DateTime.Parse(endDate);
        //        dEnd = new DateTime(dEnd.Year, dEnd.Month, dEnd.Day, 23, 59, 59);
        //        dec = dec.Where(item => item.DDate >= dStart && item.DDate <= dEnd);
        //    }
        //    if (!string.IsNullOrWhiteSpace(contrNo))
        //    {
        //        dec = dec.Where(item => item.ContrNo == contrNo);
        //    }
        //    if (!string.IsNullOrWhiteSpace(orderID))
        //    {
        //        dec = dec.Where(item => item.OrderID == orderID);
        //    }
        //    if (!string.IsNullOrWhiteSpace(decID))
        //    {
        //        dec = dec.Where(item => item.EntryId == decID);
        //    }
        //    Func<Needs.Ccs.Services.Models.ClientDecHead, object> convert = item => new
        //    {
        //        item.OrderID,
        //        item.ID,
        //        item.EntryId,
        //        DDate = item.DDate?.ToString("yyyy-MM-dd"),
        //        item.ContrNo,
        //        item.TotalDeclarePrice
        //    };
        //    return this.Paging(dec, convert);
        //}

        /// <summary>
        /// 获取报关单数据
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public async Task<JsonResult> GetDeclareOrders()
        {
            int page = int.Parse(Request.Form["page"]);
            int rows = int.Parse(Request.Form["rows"]);

            var orderID = Request.Form["orderID"].ToString().Trim();  //订单编号
            var contrNo = Request.Form["contrNo"].ToString().Trim();  //合同编号
            var decID = Request.Form["decID"].ToString().Trim();  //报关单号
            var startDate = Request.Form["startDate"];  //日期选择
            var endDate = Request.Form["endDate"];  //日期选择

            var view = Needs.Wl.User.Plat.UserPlat.Current.MyDecHeads;
            view.PageSize = rows;
            view.PageIndex = page;

            var predicate = PredicateBuilder.Create<Needs.Wl.Client.Services.PageModels.DeclareOrderViewModel>();

            if ((!string.IsNullOrWhiteSpace(startDate)) && (!string.IsNullOrWhiteSpace(endDate)))
            {
                var dStart = DateTime.Parse(startDate);
                dStart = new DateTime(dStart.Year, dStart.Month, dStart.Day);
                var dEnd = DateTime.Parse(endDate);
                dEnd = new DateTime(dEnd.Year, dEnd.Month, dEnd.Day, 23, 59, 59);

                predicate = predicate.And(item => item.DDate >= dStart && item.DDate <= dEnd);
            }

            if (!string.IsNullOrWhiteSpace(contrNo))
            {
                predicate = predicate.And(item => item.ContrNo == contrNo);
            }

            if (!string.IsNullOrWhiteSpace(orderID))
            {
                predicate = predicate.And(item => item.OrderID == orderID);
            }

            if (!string.IsNullOrWhiteSpace(decID))
            {
                predicate = predicate.And(item => item.EntryId == decID);
            }

            view.Predicate = predicate;

            var list = await view.ToListAsync();
            var total = view.RecordCount;

            var array = list.Select(item => new
            {
                item.OrderID,
                item.ID,
                item.EntryId,
                DDate = item.DDate.ToString("yyyy-MM-dd"),
                item.ContrNo,
                item.TotalDeclarePrice,
                item.Currency
            });

            return JsonResult(VueMsgType.success, "", new { list = array, total }.Json());
        }

        /// <summary>
        /// 下载报关单
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult DownloadDeclare(string id)
        {
            var ids = JsonConvert.DeserializeObject<string[]>(id);
            Encoding encoding = Encoding.UTF8;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(AppConfig.Current.DownLoadDecheadUrl);
            request.Method = "POST";
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.ContentType = "application/json";

            byte[] buffer = encoding.GetBytes(ids.Json());
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream s = response.GetResponseStream();
            //读取服务器端返回的消息  
            StreamReader sr = new StreamReader(s);
            string sReturnString = sr.ReadLine();
            var responseData = JsonConvert.DeserializeObject<ResponseData>(sReturnString);
            if (responseData.success == "true")
            {
                return base.JsonResult(VueMsgType.success, "", responseData.url);
            }
            else
            {
                return base.JsonResult(VueMsgType.error, responseData.message);
            }
        }

        #endregion 

        #region 报关数据

        /// <summary>
        /// 报关数据
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult DeclareData()
        {
            return View();
        }

        private List<Needs.Wl.User.Plat.Views.ClientOrderDataViewNewModel> GetData(bool allowPaging, out int recordCountOut, int page = 1, int rows = 10)
        {
            var orderID = Request.Form["orderID"].ToString().Trim();  //订单编号
            var contrNo = Request.Form["contrNo"].ToString().Trim();  //合同编号
            var decHeadID = Request.Form["decHeadID"].ToString().Trim();  //报关单号
            var models = Request.Form["models"].ToString().Trim(); //型号
            var startDate = Request.Form["startDate"];  //日期选择
            var endDate = Request.Form["endDate"];  //日期选择

            var exportDateType = Request.Form["exportDateType"];
            var month = Request.Form["month"];

            var predicate = PredicateBuilder.Create<Needs.Wl.User.Plat.Views.ClientOrderDataViewNewModel>();

            if (!string.IsNullOrWhiteSpace(orderID))
            {
                predicate = predicate.And(item => item.OrderID.Contains(orderID));
            }
            if ((!string.IsNullOrWhiteSpace(contrNo)))
            {
                predicate = predicate.And(item => item.ContrNo.Contains(contrNo));
            }
            if ((!string.IsNullOrWhiteSpace(decHeadID)))
            {
                predicate = predicate.And(item => item.EntryId.Contains(decHeadID));
            }
            if ((!string.IsNullOrWhiteSpace(models)))
            {
                predicate = predicate.And(item => item.GoodsModel.Contains(models));
            }

            if (exportDateType == "day")
            {
                if ((!string.IsNullOrWhiteSpace(startDate)) && (!string.IsNullOrWhiteSpace(endDate)))
                {
                    var dStart = DateTime.Parse(startDate);
                    dStart = new DateTime(dStart.Year, dStart.Month, dStart.Day);
                    var dEnd = DateTime.Parse(endDate);
                    dEnd = new DateTime(dEnd.Year, dEnd.Month, dEnd.Day, 23, 59, 59);
                    predicate = predicate.And(item => item.DDate >= dStart && item.DDate <= dEnd);
                }
            }
            else if (exportDateType == "month")
            {
                if (!string.IsNullOrEmpty(month))
                {
                    string y = month.Split('-')[0];
                    int yint = int.Parse(y);
                    string m = month.Split('-')[1];
                    int mint = int.Parse(m);

                    if (mint != 12)
                    {
                        var dStart = new DateTime(yint, mint, 1);
                        var dEnd = new DateTime(yint, mint + 1, 1);
                        predicate = predicate.And(item => item.DDate >= dStart && item.DDate < dEnd);
                    }
                    else
                    {
                        var dStart = new DateTime(yint, mint, 1);
                        var dEnd = new DateTime(yint + 1, 1, 1);
                        predicate = predicate.And(item => item.DDate >= dStart && item.DDate < dEnd);
                    }
                }
            }


            if (Needs.Wl.User.Plat.UserPlat.Current.IsMain)
            {
                predicate = predicate.And(item => item.ClientID == Needs.Wl.User.Plat.UserPlat.Current.ClientID);
            }
            else
            {
                predicate = predicate.And(item => item.UserID == Needs.Wl.User.Plat.UserPlat.Current.ID);
            }


            Needs.Wl.User.Plat.Views.ClientOrderDataViewNew view = new Needs.Wl.User.Plat.Views.ClientOrderDataViewNew();
            view.AllowPaging = allowPaging;
            view.PageIndex = page;
            view.PageSize = rows;
            view.Predicate = predicate;

            //int recordCount = view.RecordCount;
            recordCountOut = 0;
            if (allowPaging)
            {
                recordCountOut = view.RecordCount;
            }

            var orderData = view.ToList();

            var results2Tabs = view.GetResults2Tab(orderData.ToArray());


            orderData = (from order in orderData
                         join results2Tab in results2Tabs on order.DecHeadID equals results2Tab.DecHeadID into results2TabsInto
                         from results2Tab in results2TabsInto.DefaultIfEmpty()
                             //orderby order.DDate descending, order.ContrNo descending
                         select new Needs.Wl.User.Plat.Views.ClientOrderDataViewNewModel
                         {
                             DecHeadID = order.DecHeadID,
                             ClientID = order.ClientID,
                             UserID = order.UserID,
                             OrderID = order.OrderID,
                             OrderItemID = order.OrderItemID,

                             DecListID = order.DecListID,
                             DDate = order.DDate,
                             CodeTS = order.CodeTS,
                             GName = order.GName,
                             GModel = order.GModel,

                             GoodsBrand = order.GoodsBrand,
                             GoodsModel = order.GoodsModel,
                             OriginCountry = order.OriginCountry,
                             OriginCountryName = order.OriginCountryName,
                             GQty = order.GQty,

                             NetWt = order.NetWt,
                             DeclPrice = order.DeclPrice,
                             DeclTotal = order.DeclTotal,
                             TradeCurr = order.TradeCurr,
                             ContrNo = order.ContrNo,

                             CustomsExchangeRate = order.CustomsExchangeRate,
                             TariffRate = order.TariffRate,
                             Vat = order.Vat,
                             DeclTotalRMB = order.DeclTotalRMB,
                             InvoiceCompany = order.InvoiceCompany,

                             EntryId = order.EntryId,
                             TaxName = order.TaxName,
                             TaxCode = order.TaxCode,
                             ReceiptRate = order.ReceiptRate,
                             ProductUniqueCode = order.ProductUniqueCode,

                             TotalValueVat = results2Tab.TotalValueVat,

                             GNo = order.GNo,
                             ImportTaxCode = order.ImportTaxCode,
                             AddValueTaxCode = order.AddValueTaxCode
                         }).ToList();

            orderData = orderData.OrderByDescending(t => t.DDate).ThenByDescending(t => t.ContrNo).ThenBy(t => t.GNo).ToList();

            return orderData.ToList();
        }

        public JsonResult GetDeclareData()
        {
            int page = int.Parse(Request.Form["page"]);
            int rows = int.Parse(Request.Form["rows"]);

            int recordCount = 0;
            var orderData = GetData(true, out recordCount, page, rows);

            Func<Needs.Wl.User.Plat.Views.ClientOrderDataViewNewModel, object> convert = item => new
            {
                OrderID = item.OrderID,
                ID = item.DecListID,
                CodeTS = item.CodeTS,
                GName = item.GName,
                GModel = item.GModel,
                GoodsBrand = item.GoodsBrand,
                GoodsModel = item.GoodsModel,
                OriginCountry = item.OriginCountryName,
                GQty = item.GQty,
                NetWt = item.NetWt,
                DeclPrice = item.DeclPrice,
                DeclTotal = item.DeclTotal,
                TradeCurr = item.TradeCurr,
                ContrNo = item.ContrNo,
                CustomsRate = item.CustomsExchangeRate.ToString("0.0000"),
                TariffRate = item.TariffRate.ToString("0.0000"),
                DeclTotalRMB = item.DeclTotalRMB.ToRound(0).ToString("0.00"),
                TariffPay = (item.DeclTotalRMB.ToRound(0) * item.TariffRate).ToRound(2),
                TariffPayed = (item.DeclTotalRMB.ToRound(0) * item.ReceiptRate).ToRound(2),
                ValueVat = item.TotalValueVat >= 50 ? ((item.DeclTotalRMB.ToRound(0) + (item.DeclTotalRMB.ToRound(0) * item.TariffRate).ToRound(2)) * item.Vat).ToRound(2) : 0M,
                CustomsValue = item.DeclTotalRMB.ToRound(0) + (item.DeclTotalRMB.ToRound(0) * item.TariffRate).ToRound(2),
                CustomsValueVat = (item.DeclTotalRMB.ToRound(0) + (item.DeclTotalRMB.ToRound(0) * item.TariffRate).ToRound(2)) * ConstConfig.ValueAddedTaxRate,
                InvoiceCompany = item.InvoiceCompany,
                EntryId = item.EntryId,
                CreateDate = item.DDate?.ToString("yyyy-MM-dd"),
                TaxName = item.TaxName,
                TaxCode = item.TaxCode,
                ProductUniqueCode = item.ProductUniqueCode,
                ImportTaxCode = item.ImportTaxCode,
                AddValueTaxCode = item.AddValueTaxCode
            };

            return this.Paging1(orderData, recordCount, convert);
        }

        /// <summary>
        /// 报关数据导出Excel
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult ExportDecDataList()
        {
            var orderData = GetData(false, out int recordCount);

            var linq = orderData.ToList().Select((t, i) => new
            {
                序号 = i + 1,
                报关日期 = t.DDate?.ToString("yyyy-MM-dd"),
                合同号 = t.ContrNo,
                物料号 = t.ProductUniqueCode,
                商品编码 = t.CodeTS,
                规格型号 = t.GoodsModel,
                成交数量 = t.GQty,
                单价 = t.DeclPrice,
                总价 = t.DeclTotal,
                币制 = t.TradeCurr,
                品名 = t.GName,
                海关汇率 = t.CustomsExchangeRate.ToString("0.0000"),
                关税率 = t.TariffRate.ToString("0.0000"),
                报关总价 = t.DeclTotalRMB.ToRound(0).ToString("0.00"),
                应交关税 = (t.DeclTotalRMB.ToRound(0) * t.ReceiptRate).ToRound(2),
                实交增值税 = t.TotalValueVat >= 50 ? ((t.DeclTotalRMB.ToRound(0) + (t.DeclTotalRMB.ToRound(0) * t.TariffRate).ToRound(2)) * t.Vat).ToRound(2) : 0M,
                实交关税 = (t.DeclTotalRMB.ToRound(0) * t.ReceiptRate).ToRound(2),
                完税价格 = t.DeclTotalRMB.ToRound(0) + (t.DeclTotalRMB.ToRound(0) * t.TariffRate).ToRound(2),
                完税价格增值税 = (t.DeclTotalRMB.ToRound(0) + (t.DeclTotalRMB.ToRound(0) * t.TariffRate).ToRound(2)) * ConstConfig.ValueAddedTaxRate,
                开票公司 = t.InvoiceCompany,
                订单编号 = t.OrderID,
                报关单号 = t.EntryId,
                税务名称 = t.TaxName,
                税务编码 = t.TaxCode,
                关税发票号 = t.ImportTaxCode,
                增值税发票号 = t.AddValueTaxCode,
            });

            IWorkbook workbook = ExcelFactory.Create();
            Needs.Utils.Npoi.NPOIHelper npoi = new Needs.Utils.Npoi.NPOIHelper(workbook);
            int[] columnsWidth = { 10, 20, 20, 15, 15, 30, 15, 10, 15, 10, 30, 10, 10, 10, 10, 10, 10, 10, 10, 40, 20, 30, 30, 30, 30, 30 };
            npoi.EnumrableToExcel(linq, 0, columnsWidth);
            //创建文件夹
            var fileName = DateTime.Now.Ticks + ".xlsx";
            FileDirectory file = new FileDirectory(fileName);
            file.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload).CreateDateDirectory();
            //保存文件
            npoi.SaveAs(file.FilePath);
            return base.JsonResult(VueMsgType.success, "", file.LocalFileUrl);
        }
        #endregion

    }
}