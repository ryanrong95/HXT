using Needs.Linq;
using Needs.Utils.Descriptions;
using Needs.Utils.Npoi;
using Needs.Utils.Serializers;
using Needs.Wl.Web.Mvc;
using Needs.Wl.Web.Mvc.Utils;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebMvc.Models;

namespace WebMvc.Controllers
{
    /// <summary>
    /// 报表
    /// 海关缴税记录
    /// </summary>
    [UserHandleError(ExceptionType = typeof(Exception))]
    public class ReportsController : UserController
    {
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult TaxRecord()
        {
            return View();
        }

        /// <summary>
        /// 获取缴税记录的数据
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public async Task<JsonResult> GetMyCustomsTaxReports()
        {
            int page = int.Parse(Request.Form["page"]);
            int rows = int.Parse(Request.Form["rows"]);

            var startDate = Request.Form["startDate"];  //日期选择
            var endDate = Request.Form["endDate"];  //日期选择
            var orderID = Request.Form["orderID"];  //订单ID
            var contrNo = Request.Form["contrNo"];  //合同号

            var view = Needs.Wl.User.Plat.UserPlat.Current.MyCustomsTaxReports;
            view.PageSize = rows;
            view.PageIndex = page;

            var predicate = PredicateBuilder.Create<Needs.Wl.Client.Services.Models.CustomsTaxReport>();
            if ((!string.IsNullOrWhiteSpace(startDate)) && (!string.IsNullOrWhiteSpace(endDate)))
            {
                var dStart = DateTime.Parse(startDate);
                dStart = new DateTime(dStart.Year, dStart.Month, dStart.Day);

                var dEnd = DateTime.Parse(endDate);
                dEnd = new DateTime(dEnd.Year, dEnd.Month, dEnd.Day, 23, 59, 59);

                predicate = predicate.And(item => item.PayDate >= dStart && item.PayDate <= dEnd);
            }

            if (!string.IsNullOrWhiteSpace(orderID))
            {
                predicate = predicate.And(item => item.OrderID.Contains(orderID));
            }

            if (!string.IsNullOrWhiteSpace(contrNo))
            {
                predicate = predicate.And(item => item.ContractNo.Contains(contrNo));
            }

            view.Predicate = predicate;
            int total = view.RecordCount;

            var orderlist = await view.ToListAsync();
            var list = orderlist.Select(item => new
            {
                ID = item.ID,
                TaxNumber = item.TaxNumber,
                OrderID = item.OrderID,
                ContrNo = item.ContractNo,
                DecAmount = item.Amount,
                TaxType = item.TaxType.GetDescription(),
                CreateDate = item.PayDate?.ToString("yyyy-MM-dd"),
                isLoading = true,
                isCheck = false,
            });

            return JsonResult(VueMsgType.success, "", new { list = list.ToArray(), total }.Json());
        }

        /// <summary>
        /// 下载税单
        /// </summary>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult DownloadTaxList(string id)
        {
            var ids = JsonConvert.DeserializeObject<string[]>(id);
            //var view = Needs.Wl.User.Plat.UserPlat.Current.MyCustomsTaxReports;
            //view.Predicate = item => ids.Contains(item.ID);
            //view.AllowPaging = false;

           
            var orders = Needs.Wl.User.Plat.UserPlat.Current.WebSite.MyTaxRecordsView;
            var orderList = orders.GetAlls(item => ids.Contains(item.ID), null);

            Encoding encoding = Encoding.UTF8;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(AppConfig.Current.DownLoadInvoiceUrl);
            request.Method = "POST";
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.ContentType = "application/json";

            byte[] buffer = encoding.GetBytes(orderList.Json());
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

        /// <summary>
        /// 缴费流水导出Excel
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult ExportTaxList(string id)
        {
            var ids = JsonConvert.DeserializeObject<string[]>(id);
            var view = Needs.Wl.User.Plat.UserPlat.Current.MyCustomsTaxReports;
            view.Predicate = item => ids.Contains(item.ID);
            view.AllowPaging = false;

            var linq = view.ToList().Select((t, i) => new
            {
                序号 = i + 1,
                订单号 = t.OrderID,
                税费单号 = t.TaxNumber,
                合同号 = t.ContractNo,
                金额 = t.Amount,
                类型 = t.TaxType.GetDescription(),
                缴税日期 = t.PayDate?.ToString("yyyy-MM-dd"),
            });

            IWorkbook workbook = ExcelFactory.Create();
            Needs.Utils.Npoi.NPOIHelper npoi = new Needs.Utils.Npoi.NPOIHelper(workbook);
            int[] columnsWidth = { 5, 30, 30, 30, 30, 30, 30 };
            npoi.EnumrableToExcel(linq, 0, columnsWidth);
            //创建文件夹
            var fileName = DateTime.Now.Ticks + ".xlsx";
            Needs.Wl.Web.Mvc.Utils.FileDirectory file = new Needs.Wl.Web.Mvc.Utils.FileDirectory(fileName);
            file.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload).CreateDateDirectory();
            //保存文件
            npoi.SaveAs(file.FilePath);
            return base.JsonResult(VueMsgType.success, "", file.LocalFileUrl);
        }
    }
}