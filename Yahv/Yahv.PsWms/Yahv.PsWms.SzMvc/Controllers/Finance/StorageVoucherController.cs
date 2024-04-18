using Layers.Data.Sqls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.PsWms.SzMvc.Models;
using Yahv.PsWms.SzMvc.Services.Common;
using Yahv.PsWms.SzMvc.Services.Enums;
using Yahv.PsWms.SzMvc.Services.Models;
using Yahv.PsWms.SzMvc.Services.Views;
using Yahv.Underly;

namespace Yahv.PsWms.SzMvc.Controllers
{
    public partial class FinanceController : BaseController
    {
        #region 页面

        /// <summary>
        /// 仓储对账
        /// </summary>
        /// <returns></returns>
        public ActionResult StorageVoucher() { return View(); }

        /// <summary>
        /// 对账单详情
        /// </summary>
        /// <returns></returns>
        public ActionResult VoucherDetail() { return View(); }

        #endregion

        /// <summary>
        /// 获取账单文件信息
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public JsonResult GetBillFileInfo(GetBillFileInfoSearchModel searchModel)
        {
            var siteuser = Yahv.PsWms.SzMvc.SiteCoreInfo.Current;
            string theClientID = siteuser.TheClientID;

            var voucher = new SzMvc.Services.Views.Origins.VouchersOrigin()
                .Where(t => t.PayerID == theClientID &&
                            t.CutDateIndex == Convert.ToInt32(searchModel.CutDateIndex) &&
                            t.Mode == VoucherMode.Receivables &&
                            t.Type == SzMvc.Services.Enums.VoucherType.Monthly)
                .FirstOrDefault();

            string fileName = string.Empty;
            string fileUrl = string.Empty;

            if (voucher != null)
            {
                var file = new SzMvc.Services.Views.Origins.PcFilesOrigin().Where(t => t.MainID == voucher.ID).OrderByDescending(t => t.CreateDate).ToArray();

                fileName = file.Count() == 0 ? "" : file[0].CustomName;
                fileUrl = file.Count() == 0 ? "" : file[0].HttpUrl;
            }

            return Json(new { type = "success", msg = "", data = new { fileName = fileName, fileUrl = fileUrl, } }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取仓储对账列表数据
        /// </summary>
        /// <returns></returns>
        public JsonResult GetStorageVoucherList(GetStorageVoucherListSearchModel searchModel)
        {
            var siteuser = Yahv.PsWms.SzMvc.SiteCoreInfo.Current;
            string theClientID = siteuser.TheClientID;

            using (var query = new StorageVoucherListView(theClientID))
            {
                var view = query;

                if (!string.IsNullOrEmpty(searchModel.CutDateIndex))
                {
                    searchModel.CutDateIndex = searchModel.CutDateIndex.Trim();
                    view = view.SearchByCutDateIndex(searchModel.CutDateIndex);
                }

                Func<StorageVoucherListViewModel, GetStorageVoucherListReturnModel> convert = item => new GetStorageVoucherListReturnModel
                {
                    CutDateIndex = Convert.ToString(item.CutDateIndex),
                    TotalAmount = item.TotalAmount,
                };

                var viewData = view.ToMyPage(convert, searchModel.page, searchModel.rows);

                return Json(new { type = "success", msg = "", data = new { list = viewData.Item1, total = viewData.Item2 } }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 获取账单详情中列表数据
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public JsonResult GetVoucherDetailList(GetVoucherDetailListSearchModel searchModel)
        {
            var siteuser = Yahv.PsWms.SzMvc.SiteCoreInfo.Current;
            string theClientID = siteuser.TheClientID;

            using (var query = new VoucherDetailListView(theClientID, Convert.ToInt32(searchModel.CutDateIndex)))
            {
                var view = query;

                Func<VoucherDetailListViewModel, GetVoucherDetailListReturnModel> convert = item => new GetVoucherDetailListReturnModel
                {
                    PayeeLeftCreateDateDes = item.PayeeLeftCreateDate.ToString("yyyy-MM-dd"),
                    OrderID = item.OrderID,
                    OrderStatusDes = item.OrderStatus.GetDescription(),
                    OrderCreateDateDes = item.OrderCreateDate.ToString("yyyy-MM-dd"),
                    ConductDes = item.Conduct.GetDescription(),
                    Subject = item.Subject,
                    UnitPriceDes = item.UnitPrice.ToString("0.0000"),
                    Quantity = item.Quantity,
                    Total = item.Total,
                    TotalDes = item.Total.ToString("0.0000"),
                };

                var viewData = view.ToMyPage(convert);
                var listData = viewData.Item1;
                string totalsum = listData.Sum(t => t.Total).ToString("0.0000");

                return Json(new { type = "success", msg = "", data = new { list = listData, totalsum = totalsum, } }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult ExportBill(ExportBillSearchModel searchModel)
        {
            var siteuser = Yahv.PsWms.SzMvc.SiteCoreInfo.Current;
            string theClientID = siteuser.TheClientID;

            try
            {
                var voucher = new SzMvc.Services.Views.Origins.VouchersOrigin()
                    .Where(t => t.PayerID == theClientID &&
                                t.CutDateIndex == Convert.ToInt32(searchModel.CutDateIndex) &&
                                t.Mode == VoucherMode.Receivables &&
                                t.Type == SzMvc.Services.Enums.VoucherType.Monthly)
                    .FirstOrDefault();
                if (voucher == null)
                {
                    voucher = new Yahv.PsWms.SzMvc.Services.Models.Origin.Voucher();
                    voucher.PayeeID = "DBAEAB43B47EB4299DD1D62F764E6B6A";
                    voucher.PayerID = theClientID;
                    voucher.Type = SzMvc.Services.Enums.VoucherType.Monthly;
                    voucher.Mode = VoucherMode.Receivables;
                    voucher.CutDateIndex = Convert.ToInt32(searchModel.CutDateIndex);
                    voucher.Enter();
                }


                VoucherExportPdf pdf = new VoucherExportPdf(voucher);
                string fileName = DateTime.Now.Ticks + ".pdf";

                FileDirectory.CreateDirectory();
                pdf.SaveAs(FileDirectory.DownLoadRoot + fileName);

                string httpfilePath = @"/Files/DownLoad/" + fileName;

                return Json(new { type = "success", msg = "", httpfilePath = httpfilePath, }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { type = "error", msg = ex.Message, }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 上传账单文件
        /// </summary>
        /// <returns></returns>
        public JsonResult UploadFileBill()
        {
            var siteuser = Yahv.PsWms.SzMvc.SiteCoreInfo.Current;
            string theSiteuserID = siteuser.SiteuserID;
            string theClientID = siteuser.TheClientID;

            try
            {
                string CutDateIndex = Request.QueryString["CutDateIndex"];

                var voucher = new SzMvc.Services.Views.Origins.VouchersOrigin()
                                .Where(t => t.PayerID == theClientID &&
                                            t.CutDateIndex == Convert.ToInt32(CutDateIndex) &&
                                            t.Mode == VoucherMode.Receivables &&
                                            t.Type == SzMvc.Services.Enums.VoucherType.Monthly)
                                .FirstOrDefault();
                if (voucher == null)
                {
                    return Json(new { type = "error", msg = "未生成账单，请导出账单后再上传！" }, JsonRequestBehavior.AllowGet);
                }


                HttpPostedFileBase file = Request.Files["file"];

                //后台也要校验
                string fileFormat = Path.GetExtension(file.FileName).ToLower();
                var fileSize = file.ContentLength / 1024;
                if (fileSize > 1024 * 3 && fileFormat == ".pdf")
                {
                    return Json(new { type = "error", msg = "上传的文件大小不超过3M！" }, JsonRequestBehavior.AllowGet);
                }
                string[] allowFiles = { ".jpg", ".bmp", ".pdf", ".gif", ".png", ".pdf", ".jpeg" };
                if (allowFiles.Contains(fileFormat) == false)
                {
                    return Json(new { type = "error", msg = "文件格式错误，请上传pdf文件！" }, JsonRequestBehavior.AllowGet);
                }

                NewFile newFile = new NewFile(file.FileName, PsOrderFileType.Bill);
                file.SaveAs(newFile.FullName);

                var fileModel = new
                {
                    name = file.FileName,
                    URL = newFile.URL,
                    fullURL = newFile.FullURL,
                    fileFormat = file.ContentType
                };

                //文件信息保存数据库 begin

                using (PsOrderRepository repository = new PsOrderRepository())
                {
                    repository.Insert(new Layers.Data.Sqls.PsOrder.PcFiles
                    {
                        ID = Layers.Data.PKeySigner.Pick(Services.Enums.PKeyType.PcFile),
                        MainID = voucher.ID,
                        Type = (int)PsOrderFileType.Bill,
                        CustomName = fileModel.name,
                        Url = fileModel.URL,
                        CreateDate = DateTime.Now,
                        SiteuserID = theSiteuserID,
                    });
                }

                //文件信息保存数据库 end

                return Json(new { type = "success", data = fileModel, }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { type = "error", msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}