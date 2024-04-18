using Newtonsoft.Json;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Yahv.PvWsClient.WebAppNew.App_Utils;
using Yahv.PvWsClient.WebAppNew.Controllers.Attribute;
using Yahv.PvWsClient.WebAppNew.Models;
using Yahv.PvWsOrder.Services.ClientModels.Client;
using Yahv.PvWsOrder.Services.ClientViews;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.PvWsOrder.Services.Extends;
using Yahv.PvWsOrder.Services.XDTClientView;
using Yahv.Services.Models;
using Yahv.Services.Views;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;
using Yahv.Utils.Http;
using Yahv.Utils.Npoi;
using Yahv.Utils.Serializers;

namespace Yahv.PvWsClient.WebAppNew.Controllers
{
    public class FilesController : UserController
    {
        #region 文件上传
        /// <summary>
        /// 提货文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public JsonResult UploadPickUpFile(HttpPostedFileBase file)
        {
            try
            {
                //拿到上传来的文件
                file = Request.Files["file"];

                string fileFormat = Path.GetExtension(file.FileName).ToLower();
                string[] Files = { ".pdf", ".doc", ".docx" };
                //后台也要校验
                var fileSize = file.ContentLength / 1024;
                if (fileSize > 1024 * 3 && Files.Contains(fileFormat))
                {
                    return base.JsonResult(VueMsgType.error, "上传的文件大小不超过3M！");
                }

                string[] allowFiles = { ".jpg", ".bmp", ".pdf", ".gif", ".png", ".pdf", ".doc", ".docx", ".jpeg" };
                if (allowFiles.Contains(fileFormat) == false)
                {
                    return base.JsonResult(VueMsgType.error, "文件格式错误，请上传图片、Word文件或者pdf文件！");
                }

                var result = Yahv.Alls.Current.centerFiles.fileSave(file);
                var fullUrl = PvWsOrder.Services.PvClientConfig.DomainUrl + @"/" + result;
                var contentType = file.ContentType;
                if (fileFormat == ".docx")
                {
                    contentType = "application/msword";
                }
                return base.JsonResult(VueMsgType.success, "", new { name = file.FileName, URL = result, fullURL = fullUrl, fileFormat = contentType }.Json());
            }
            catch (Exception ex)
            {
                return base.JsonResult(VueMsgType.error, ex.Message);
            }
        }

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = false)]
        public JsonResult UploadOrderFile(HttpPostedFileBase file)
        {
            try
            {
                //拿到上传来的文件
                file = Request.Files["file"];

                //后台也要校验
                string fileFormat = Path.GetExtension(file.FileName).ToLower();
                var fileSize = file.ContentLength / 1024;
                if (fileSize > 1024 * 3 && fileFormat == ".pdf")
                {
                    return base.JsonResult(VueMsgType.error, "上传的文件大小不超过3M！");
                }
                string[] allowFiles = { ".jpg", ".bmp", ".pdf", ".gif", ".png", ".pdf", ".jpeg" };
                if (allowFiles.Contains(fileFormat) == false)
                {
                    return base.JsonResult(VueMsgType.error, "文件格式错误，请上传图片或者pdf文件！");
                }

                var result = Yahv.Alls.Current.centerFiles.fileSave(file);
                var fullUrl = PvWsOrder.Services.PvClientConfig.DomainUrl + @"/" + result;
                return base.JsonResult(VueMsgType.success, "", new FileModel { name = file.FileName, URL = result, fullURL = fullUrl, fileFormat = file.ContentType }.Json());
            }
            catch (Exception ex)
            {
                return base.JsonResult(VueMsgType.error, ex.Message);
            }
        }
        #endregion

        #region 下载付汇委托书
        /// <summary>
        /// 下载付汇委托书
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public JsonResult DownloadPayProxy(string id)
        {
            try
            {
                id = id.InputText();
                var current = Client.Current;
                var apply = current.MyPayExchangeApplies.GetDetailDataByID(id);
                if (apply == null)
                {
                    return base.JsonResult(VueMsgType.error, "下载付汇委托书失败，付汇申请不存在。");
                }

                FileDirectory file = new FileDirectory(DateTime.Now.Ticks + ".pdf");
                file.SetChildFolder(SysConfig.Dowload);
                file.CreateDateDirectory();

                var proxy = new Yahv.PvWsOrder.Services.XDTModels.PayExchangeAgentProxy(apply);
                proxy.SaveAs(file.FilePath, current.XDTClientType);

                return base.JsonResult(VueMsgType.success, "", "/Files" + file.LocalFileUrl);
            }
            catch (Exception ex)
            {
                return base.JsonResult(VueMsgType.error, "", ex.Message);
            }
        }
        #endregion

        #region 上传付汇委托书
        /// <summary>
        /// 上传付汇委托书
        /// </summary>
        /// <param name="id"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public JsonResult SavePayProxy(string id, string filename, string ext, string url)
        {
            try
            {
                var user = Client.Current;

                var apply = user.MyPayExchangeApplies[id];
                if (apply.PayExchangeApplyStatus != PayExchangeApplyStatus.Auditing)
                {
                    return base.JsonResult(VueMsgType.error, "上传付汇委托书失败，付汇申请已审核完成。");
                }

                var file = new PayExchangeApplyFile();
                file.PayExchangeApplyID = apply.ID;
                file.FileFormat = ext;
                file.Name = filename;
                file.FileType = FileType.PayExchange;
                file.Url = url;
                file.UserID = user.ID;
                file.ClientID = user.MyClients.ID;
                file.Status = (int)FileDescriptionStatus.Normal;
                file.Enter();

                var n_apply = Client.Current.MyPayExchangeApplies.GetDetailDataByID(id);
                var fileURL = Yahv.PvWsOrder.Services.PvClientConfig.FileServerUrl + @"/" + n_apply.PayExchangeFile.Url.ToUrl();
                return base.JsonResult(VueMsgType.success, "上传成功", fileURL);
            }
            catch (Exception ex)
            {
                return base.JsonResult(VueMsgType.error, ex.Message);
            }
        }
        #endregion

        #region 下载代付货款委托书
        /// <summary>
        /// 下载付汇委托书
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public JsonResult DownloadApplyProxy(string id)
        {
            try
            {
                var user = Client.Current;
                id = id.InputText();
                var apply = new PrePayApplyFilesView().GetProxyInfo(id);
                if (apply == null)
                {
                    return base.JsonResult(VueMsgType.error, "下载代付货款委托书失败，代付申请不存在。");
                }

                FileDirectory file = new FileDirectory(DateTime.Now.Ticks + ".pdf");
                file.SetChildFolder(SysConfig.Dowload);
                file.CreateDateDirectory();

                apply.ClientName = user.XDTClientName;
                var proxy = new Yahv.PvWsOrder.Services.XDTModels.PrePayAgentProxy(apply);
                proxy.SaveAs(file.FilePath);

                return base.JsonResult(VueMsgType.success, "", "/Files" + file.LocalFileUrl);
            }
            catch (Exception ex)
            {
                return base.JsonResult(VueMsgType.error, "", ex.Message);
            }
        }
        #endregion

        #region 上传代付货款委托书
        /// <summary>
        /// 上传付汇委托书
        /// </summary>
        /// <param name="id"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public JsonResult SaveApplyProxy(string id, string filename, string ext, string url)
        {
            try
            {
                var user = Client.Current;

                var file = new PrePayApplyFilesView();
                file.ApplicationID = id;
                file.FileFormat = ext;
                file.FileName = filename;
                file.FileType = FileType.PaymentEntrust;
                file.Url = url;
                file.UserID = user.ID;
                file.ClientID = user.MyClients.ID;
                file.Status = (int)FileDescriptionStatus.Normal;
                file.Enter();

                var partUrl = new CenterFilesTopView().FirstOrDefault(m => m.ApplicationID == id)?.Url.ToUrl();
                var fileUrl = Yahv.PvWsOrder.Services.PvClientConfig.FileServerUrl + @"/" + partUrl;
                return base.JsonResult(VueMsgType.success, "上传成功", fileUrl);
            }
            catch (Exception ex)
            {
                return base.JsonResult(VueMsgType.error, ex.Message);
            }
        }
        #endregion

        #region 下载我的协议书

        /// <summary>
        /// 报关下载我的协议书
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public JsonResult DownloadAgreement(string id)
        {
            try
            {
                var url = System.Configuration.ConfigurationManager.AppSettings["DownloadAgreementUrl"] + "?ClientID=" + id;
                string sReturnString = RequestHelper.HttpGet(url);
                var responseData = JsonConvert.DeserializeObject<ResponseData>(sReturnString);
                if (responseData.success == "true")
                {
                    return base.JsonResult(VueMsgType.success, "", responseData.data);
                }
                else
                {
                    return base.JsonResult(VueMsgType.error, responseData.data);
                }
            }
            catch (Exception ex)
            {
                return base.JsonResult(VueMsgType.error, ex.Message);
            }
        }

        /// <summary>
        /// 仓储下载我的协议书
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public JsonResult StorageDownloadAgreement(string id)
        {
            try
            {
                var url = System.Configuration.ConfigurationManager.AppSettings["StorageDownloadAgreementUrl"] + "?ClientID=" + id;
                string sReturnString = RequestHelper.HttpGet(url);
                var responseData = JsonConvert.DeserializeObject<ResponseData>(sReturnString);
                if (responseData.success == "true")
                {
                    return base.JsonResult(VueMsgType.success, "", responseData.data);
                }
                else
                {
                    return base.JsonResult(VueMsgType.error, responseData.data);
                }
            }
            catch (Exception ex)
            {
                return base.JsonResult(VueMsgType.error, ex.Message);
            }
        }

        #endregion

        #region 上传我的协议书

        /// <summary>
        /// 上传我的报关协议书
        /// </summary>
        /// <param name="id"></param>
        /// <param name="filename"></param>
        /// <param name="ext"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public JsonResult SaveAgreement(string id, string filename, string ext, string url)
        {
            try
            {
                var user = Client.Current;

                var file = new ClientAgreementFilesView();
                file.Name = filename;
                file.FileType = FileType.ServiceAgreement;
                file.Url = url;
                file.UserID = user.ID;
                file.ClientID = user.XDTClientID;
                file.Enter();

                var returnUrl = new CenterFilesView().FirstOrDefault(item => item.ClientID == id && item.Type == (int)FileType.ServiceAgreement)?.Url.ToUrl();
                var fileURL = Yahv.PvWsOrder.Services.PvClientConfig.FileServerUrl + @"/" + returnUrl;
                return base.JsonResult(VueMsgType.success, "上传成功", fileURL);
            }
            catch (Exception ex)
            {
                return base.JsonResult(VueMsgType.error, ex.Message);
            }
        }

        /// <summary>
        /// 上传我的仓储协议书
        /// </summary>
        /// <param name="id"></param>
        /// <param name="filename"></param>
        /// <param name="ext"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public JsonResult SaveStorageAgreement(string id, string filename, string ext, string url)
        {
            try
            {
                var user = Client.Current;

                var file = new ClientAgreementFilesView();
                file.Name = filename;
                file.FileType = FileType.StorageAgreement;
                file.Url = url;
                file.UserID = user.ID;
                file.ClientID = user.XDTClientID;
                file.Enter();

                var returnUrl = new CenterFilesView().FirstOrDefault(item => item.ClientID == id && item.Type == (int)FileType.StorageAgreement)?.Url.ToUrl();
                var fileURL = Yahv.PvWsOrder.Services.PvClientConfig.FileServerUrl + @"/" + returnUrl;
                return base.JsonResult(VueMsgType.success, "上传成功", fileURL);
            }
            catch (Exception ex)
            {
                return base.JsonResult(VueMsgType.error, ex.Message);
            }
        }

        #endregion

        #region 报关订单
        /// <summary>
        /// 报关订单产品导入
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public ActionResult DeclareFileUpload(HttpPostedFileBase file)
        {
            var result = new List<object>();
            try
            {
                //拿到上传来的文件
                file = Request.Files["file"];
                string fileExtension = Path.GetExtension(file.FileName).ToLower();//文件拓展名
                string[] allowFiles = { ".xls", ".xlsx" };
                if (allowFiles.Contains(fileExtension) == false)
                {
                    return base.JsonResult(VueMsgType.error, "文件格式错误，请上传.xls或.xlsx格式的文件。");
                }
                System.Data.DataTable dt = App_Utils.NPOIHelper.ExcelToDataTable(fileExtension, file.InputStream, true);
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    var array = row.ItemArray;
                    int no = 0;
                    if (!int.TryParse(array[0].ToString(), out no))
                    {
                        continue;
                    }

                    //型号没有,数据无效行
                    if (array[6].ToString() == string.Empty && array[10].ToString() == string.Empty)
                    {
                        continue;
                    }

                    var orignvalue = "";  //原产地值
                    if (!string.IsNullOrWhiteSpace(array[7].ToString()))
                    {
                        var origin = ExtendsEnum.ToNameDictionary<Origin>().Where(item => item.Name.ToUpper() == array[7].ToString().ToUpper() || item.Value.ToUpper() == array[7].ToString().ToUpper()).ToArray();
                        if (origin.Count() > 0)
                        {
                            orignvalue = origin[0].Value;
                        }
                    }
                    var quantity = array[9].GetType().Name == "DBNull" ? 0 : Convert.ToDecimal(array[9]);
                    var totalPrice = array[11].GetType().Name == "DBNull" ? 0 : Math.Round(Convert.ToDecimal(array[11]), 2, MidpointRounding.AwayFromZero);
                    var model = new
                    {
                        ProductUnicode = array[1].ToString(),
                        Batch = array[2].ToString(),
                        Name = array[4].ToString(),
                        Manufacturer = array[5].ToString(),
                        PartNumber = array[6].ToString(),
                        Origin = orignvalue,
                        Quantity = quantity > 0 ? quantity : 0 - quantity,
                        Unit = (int)LegalUnit.个, //单位默认个
                        UnitLabel = LegalUnit.个.GetUnit().Code + " " + LegalUnit.个.GetDescription(),
                        TotalPrice = totalPrice > 0 ? totalPrice : 0 - totalPrice,
                        UnitPrice = Math.Round(totalPrice / quantity, 4),
                        FileName = file.FileName
                    };
                    result.Add(model);
                }
            }
            catch (Exception ex)
            {

                return base.JsonResult(VueMsgType.error, ex.Message);
            }
            return base.JsonResult(VueMsgType.success, "", result.Json());
        }
        #endregion

        #region 代理报关委托书
        /// <summary>
        /// 导出代理报关委托书
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public JsonResult ExportAgent(string id)
        {
            try
            {
                id = id.InputText();
                var current = Client.Current;

                //获取报关委托书数据
                var orderAgentProxy = current.MyOrder.GetOrderAgentProxy(id);
                //创建文件目录
                FileDirectory file = new FileDirectory(DateTime.Now.Ticks + ".pdf");
                file.SetChildFolder(SysConfig.Dowload);
                file.CreateDateDirectory();

                orderAgentProxy.SaveAs(file.FilePath, current.XDTClientType);
                return base.JsonResult(VueMsgType.success, "", "/Files" + file.LocalFileUrl);
            }
            catch (Exception ex)
            {
                return base.JsonResult(VueMsgType.error, ex.Message);
            }
        }

        /// <summary>
        /// 报关委托书、对账单
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public JsonResult UploadDeclareFile(string id, int type, string filename, string fileurl)
        {
            var UserID = Client.Current.ID;
            //中心文件保存
            var centerfile = new Yahv.Services.Models.CenterFileDescription()
            {
                WsOrderID = id,
                Type = type,
                CustomName = filename,
                Url = fileurl,
                AdminID = UserID,
            };
            new CenterFilesView().XDTUpload(centerfile);
            return base.JsonResult(VueMsgType.success, "上传成功");
        }
        #endregion

        #region 对账单
        /// <summary>
        /// 导出对账单
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public JsonResult ExportBill(string id)
        {
            try
            {
                id = id.InputText();
                var current = Client.Current;

                //获取报关委托书数据
                var orderbillProxy = current.MyOrder.GetOrderBillProxy(id);

                //从 AdvanceRecords 表中根据大订单号查询显示在对账单底部文字的"代垫本金"
                decimal amountFor代垫本金 = new AdvanceRecordsView().GetAmountForDeclareTotalPrice(id);

                //创建文件目录
                FileDirectory file = new FileDirectory(DateTime.Now.Ticks + ".pdf");
                file.SetChildFolder(SysConfig.Dowload);
                file.CreateDateDirectory();
                orderbillProxy.DueDate = current.MyAgreement.GetDueDateNew(id);  //current.MyAgreement.GetDueDate();
                orderbillProxy.SaveAs(file.FilePath, current.MyXDTOrder, amountFor代垫本金, current.XDTClientType);
                return base.JsonResult(VueMsgType.success, "", "/Files" + file.LocalFileUrl);
            }
            catch (Exception ex)
            {
                return base.JsonResult(VueMsgType.error, ex.Message);
            }
        }
        #endregion

        #region 导入待收货订单产品
        /// <summary>
        /// 上传产品明细
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public JsonResult StorageFileUpload(HttpPostedFileBase file)
        {
            try
            {
                //拿到上传来的文件
                file = Request.Files["file"];
                string ext = Path.GetExtension(file.FileName).ToLower();
                string[] exts = { ".xls", ".xlsx" };
                if (exts.Contains(ext) == false)
                {
                    return base.JsonResult(VueMsgType.error, "文件格式错误，请上传.xls或.xlsx文件！");
                }
                System.Data.DataTable dt = App_Utils.NPOIHelper.ExcelToDataTable(ext, file.InputStream, true);

                List<Models.OrderItem> list = new List<Models.OrderItem>();

                if (dt.Rows.Count == 0)
                {
                    return base.JsonResult(VueMsgType.error, "导入的数据不能为空");
                }

                for (var i = 0; i < dt.Rows.Count; i++)
                {
                    var array = dt.Rows[i].ItemArray;
                    var origin = array[4].ToString();
                    var unit = array[6].ToString();
                    Models.OrderItem preData = new Models.OrderItem
                    {
                        Manufacturer = array[0].ToString(),
                        PartNumber = array[1].ToString(),
                        PackageCase = array[2].ToString(),
                        DateCode = array[3].ToString(),
                        Quantity = array[5].GetType().Name == "DBNull" || string.IsNullOrWhiteSpace(array[5].ToString()) ? 0 : Convert.ToInt32(array[5]),
                        UnitPrice = array[7].GetType().Name == "DBNull" || string.IsNullOrWhiteSpace(array[7].ToString()) ? 0 : Convert.ToDecimal(array[7]),
                    };
                    preData.Quantity = Math.Abs(preData.Quantity);
                    preData.UnitPrice = Math.Abs(preData.UnitPrice);
                    preData.TotalPrice = preData.Quantity * preData.UnitPrice;
                    if (!string.IsNullOrWhiteSpace(origin))
                    {
                        var originList = ExtendsEnum.ToNameDictionary<Origin>().Where(item => item.Name.ToUpper() == origin.ToUpper() || item.Value.ToUpper() == origin.ToUpper()).ToArray();
                        if (originList.Count() > 0)
                        {
                            preData.Origin = originList[0].Value;
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(unit))
                    {
                        var unitList = UnitEnumHelper.ToUnitDictionary().Where(item => item.Code.ToUpper() == unit.ToUpper() || item.Value.ToString().ToUpper() == unit.ToUpper() || item.Name.ToUpper() == unit.ToUpper()).FirstOrDefault();
                        if (unitList != null)
                        {
                            preData.Unit = unitList.Value;
                        }
                        else
                        {
                            preData.Unit = (int)LegalUnit.个;
                        }
                    }
                    else
                    {
                        preData.Unit = (int)LegalUnit.个;
                    }
                    list.Add(preData);
                }
                var data = new
                {
                    list,
                    file.FileName
                };
                return base.JsonResult(VueMsgType.success, "导入成功", data.Json());
            }
            catch (Exception ex)
            {
                return base.JsonResult(VueMsgType.error, ex.Message);
            }
        }
        #endregion

        #region 导出入库、出库单
        /// <summary>
        /// 导出入库单
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public JsonResult ExportInstorage(string ids)
        {
            var arr = JsonConvert.DeserializeObject<string[]>(ids); //id数组
            var data = Yahv.Client.Current.MyInStorage.Where(item => arr.Contains(item.ID)).ToList().Select((item, index) => new
            {
                序号 = index + 1,
                入库时间 = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                入库单号 = item.ID,
                型号 = item.PartNumber,
                品牌 = item.Manufacturer,
                币种 = ((Currency?)item.Currency)?.GetDescription(),
                单价 = item.UnitPrice?.ToString("0.00"),
                数量 = item.Quantity.ToString("0.00"),
                金额 = item.TotalPrice?.ToString("0.00"),
                库房 = item.WarehouseID.StartsWith("HK") ? "香港库房" : "深圳库房",
                订单号 = item.OrderID,
                订单类型 = ((OrderType)item.OrderType).GetDescription(),
            });
            IWorkbook workbook = ExcelFactory.Create();
            Utils.Npoi.NPOIHelper npoi = new Utils.Npoi.NPOIHelper(workbook);
            int[] columnsWidth = { 10, 20, 20, 15, 15, 30, 15, 10, 15, 10, 30, 10, 10, 10, 10, 10, 10, 10, 10, 40, 20, 30, 30, 30, 30, 30 };
            npoi.EnumrableToExcel(data, 0, columnsWidth);
            //创建文件夹
            var fileName = DateTime.Now.Ticks + ".xlsx";
            string filepath = Server.MapPath("~/Files/") + fileName; //本地路径
            npoi.SaveAs(filepath);
            var localpath = PvWsOrder.Services.PvClientConfig.DomainUrl + "/Files/" + fileName; //浏览器路径
            return base.JsonResult(VueMsgType.success, "", localpath);
        }

        /// <summary>
        /// 导出出库单
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public JsonResult ExportOutstorage(string ids)
        {
            var arr = JsonConvert.DeserializeObject<string[]>(ids); //id数组
            var data = Yahv.Client.Current.MyOutStorage.Where(item => arr.Contains(item.ID)).ToList().Select((item, index) => new
            {
                序号 = index + 1,
                出库时间 = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                出库单号 = item.ID,
                型号 = item.PartNumber,
                品牌 = item.Manufacturer,
                币种 = ((Currency?)item.Currency)?.GetDescription(),
                单价 = item.UnitPrice?.ToString("0.00"),
                数量 = item.Quantity.ToString("0.00"),
                金额 = item.TotalPrice?.ToString("0.00"),
                库房 = item.WarehouseID.StartsWith("HK") ? "香港库房" : "深圳库房",
                订单号 = item.OrderID,
                订单类型 = ((OrderType)item.OrderType).GetDescription(),
            });
            IWorkbook workbook = ExcelFactory.Create();
            Utils.Npoi.NPOIHelper npoi = new Utils.Npoi.NPOIHelper(workbook);
            int[] columnsWidth = { 10, 20, 20, 15, 15, 30, 15, 10, 15, 10, 30, 10, 10, 10, 10, 10, 10, 10, 10, 40, 20, 30, 30, 30, 30, 30 };
            npoi.EnumrableToExcel(data, 0, columnsWidth);
            //创建文件夹
            var fileName = DateTime.Now.Ticks + ".xlsx";
            string filepath = Server.MapPath("~/Files/") + fileName; //本地路径
            npoi.SaveAs(filepath);
            var localpath = PvWsOrder.Services.PvClientConfig.DomainUrl + "/Files/" + fileName; //浏览器路径
            return base.JsonResult(VueMsgType.success, "", localpath);
        }

        /// <summary>
        /// 导出库存
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public JsonResult Exportstorage(string ids)
        {
            var arr = JsonConvert.DeserializeObject<string[]>(ids); //id数组
            var data = Yahv.Client.Current.MyStorages.Where(item => arr.Contains(item.ID)).ToList().Select((item, index) => new
            {
                序号 = index + 1,
                入库时间 = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                入库单号 = item.ID,
                型号 = item.Product.PartNumber,
                品牌 = item.Product.Manufacturer,
                币种 = item.Input.Currency?.GetDescription(),
                单价 = item.Input.UnitPrice?.ToString("0.00"),
                数量 = item.Quantity.ToString("0.00"),
                金额 = (item.Input.UnitPrice * item.Quantity)?.ToString("0.00"),
                库房 = item.WareHouseID.StartsWith("HK") ? "香港库房" : "深圳库房",
            });
            IWorkbook workbook = ExcelFactory.Create();
            Utils.Npoi.NPOIHelper npoi = new Utils.Npoi.NPOIHelper(workbook);
            int[] columnsWidth = { 10, 20, 20, 15, 15, 30, 15, 10, 15, 10, 30, 10, 10, 10, 10, 10, 10, 10, 10, 40, 20, 30, 30, 30, 30, 30 };
            npoi.EnumrableToExcel(data, 0, columnsWidth);
            //创建文件夹
            var fileName = DateTime.Now.Ticks + ".xlsx";
            string filepath = Server.MapPath("~/Files/") + fileName; //本地路径
            npoi.SaveAs(filepath);
            var localpath = PvWsOrder.Services.PvClientConfig.DomainUrl + "/Files/" + fileName; //浏览器路径
            return base.JsonResult(VueMsgType.success, "", localpath);
        }

        /// <summary>
        /// 导出发票
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public JsonResult ExportInvoices(string ids)
        {
            var arr = JsonConvert.DeserializeObject<string[]>(ids); //id数组
            var data = Yahv.Client.Current.MyInvoices
                .Where(item => arr.Contains(item.TinyOrderID))
                .OrderByDescending(t => t.TinyOrderID)
                .ToList().Select((item, index) => new
                {
                    开票日期 = item.InvoiceDate?.ToString("yyyy-MM-dd HH:mm:ss"),
                    发票号 = item.InvoiceNo,
                    小订单号 = item.TinyOrderID,
                    主订单号 = item.OrderID,
                    订单类型 = item.orderType?.GetDescription(),
                    开票状态 = item.InvoiceStatus?.GetDescription(),
                    发票金额 = item.Amount,
                    发票抬头 = item.InvoiceType?.GetDescription(),
                });
            IWorkbook workbook = ExcelFactory.Create();
            Utils.Npoi.NPOIHelper npoi = new Utils.Npoi.NPOIHelper(workbook);
            int[] columnsWidth = { 10, 20, 20, 15, 15, 30, 15, 10, 15, 10, 30, 10, 10, 10, 10, 10, 10, 10, 10, 40, 20, 30, 30, 30, 30, 30 };
            npoi.EnumrableToExcel(data, 0, columnsWidth);
            //创建文件夹
            var fileName = DateTime.Now.Ticks + ".xlsx";
            string filepath = Server.MapPath("~/Files/") + fileName; //本地路径
            npoi.SaveAs(filepath);
            var localpath = PvWsOrder.Services.PvClientConfig.DomainUrl + "/Files/" + fileName; //浏览器路径
            return base.JsonResult(VueMsgType.success, "", localpath);
        }
        #endregion

        #region 租赁合同
        /// <summary>
        /// 导出租赁合同
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public JsonResult ExportLeasingContract()
        {
            try
            {
                //创建文件夹
                var fileName = DateTime.Now.Ticks + "租仓合同.docx";
                var dirPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files\\Dowload");
                if (!System.IO.Directory.Exists(dirPath))
                {
                    System.IO.Directory.CreateDirectory(dirPath);
                }
                var path = System.IO.Path.Combine(dirPath, fileName);
                string fileurl = $"../../Files/Dowload/{fileName}";
                //var order = Client.Current.MyLsOrders.GetOrderDetail(id);
                var order = new PvWsOrder.Services.ClientModels.LsOrderExtends();
                //order.Export(path);
                return base.JsonResult(VueMsgType.success, "", fileurl);
            }
            catch (Exception e)
            {
                return base.JsonResult(VueMsgType.error, e.Message);
            }
        }

        /// <summary>
        /// 保存合同文件
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public JsonResult UploadContract(string id, string filename, string fileurl)
        {

            var order = Client.Current.MyLsOrdersList.FirstOrDefault(item => item.ID == id);
            if (order == null)
            {
                return base.JsonResult(VueMsgType.error, "上传失败！");
            }
            List<CenterFileDescription> files = new List<CenterFileDescription> { new CenterFileDescription
            {
                CustomName = filename,
                Url = fileurl,
                AdminID = Client.Current.ID,
                LsOrderID = id,
                Type = (int)FileType.Contract,
            }};
            new CenterFilesView().Upload(files.ToArray());
            return base.JsonResult(VueMsgType.success, "上传成功！");
        }

        /// <summary>
        /// 获取租赁合同文件
        /// </summary>
        [UserAuthorize(UserAuthorize = true)]
        public JsonResult GetContract(string lsOrderID)
        {
            var file = new CenterFilesView()
                .Where(t => t.LsOrderID == lsOrderID && t.Type == (int)FileType.Contract)
                .FirstOrDefault();
            var data = new
            {
                FileName = file?.CustomName,
                FileUrl = !string.IsNullOrEmpty(file?.Url) ? PvWsOrder.Services.PvClientConfig.FileServerUrl + @"/" + file?.Url.ToUrl() : "",
            };
            return base.JsonResult(VueMsgType.success, "", data.Json());
        }

        #endregion

        #region 税单下载
        /// <summary>
        /// 税单下载
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public JsonResult DownloadTaxList(string ids)
        {
            if (!Client.Current.UnPayexchangeOrders.CanDownLoadTaxFiles())
            {
                return base.JsonResult(VueMsgType.error, "存在超期未付汇订单，不允许下载缴款书！");
            }

            var arr = ids.Split(','); //id数组
            var orders = Client.Current.MytaxRecords.Where(item => arr.Contains(item.ID)).ToArray();

            Encoding encoding = Encoding.UTF8;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(System.Configuration.ConfigurationManager.AppSettings["DownLoadInvoiceUrl"]);
            request.Method = "POST";
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.ContentType = "application/json";

            byte[] buffer = encoding.GetBytes(orders.Json());
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
        /// 税单下载solo
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public JsonResult DownloadTaxListSolo(string ids)
        {
            try
            {
                if (!Client.Current.UnPayexchangeOrders.CanDownLoadTaxFiles())
                {
                    return base.JsonResult(VueMsgType.error, "存在超期未付汇订单，不允许下载缴款书！");
                }

                var arr = ids.Split(','); //id数组
                var orders = Client.Current.MytaxRecords.Where(item => arr.Contains(item.ID)).ToArray();

                Encoding encoding = Encoding.UTF8;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(System.Configuration.ConfigurationManager.AppSettings["DownLoadInvoiceUrlSolo"]);
                request.Method = "POST";
                request.Accept = "text/html, application/xhtml+xml, */*";
                request.ContentType = "application/json";

                byte[] buffer = encoding.GetBytes(orders.Json());
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
            catch (Exception ex)
            {
                return base.JsonResult(VueMsgType.error, ex.Message);
            }
        }

        #endregion

        #region 报关单下载

        /// <summary>
        /// 下载报关单
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public JsonResult DownloadDeclare(string ids)
        {
            var arr = ids.Split(','); //id数组
            Encoding encoding = Encoding.UTF8;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(System.Configuration.ConfigurationManager.AppSettings["DownLoadDecheadUrl"]);
            request.Method = "POST";
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.ContentType = "application/json";

            byte[] buffer = encoding.GetBytes(arr.Json());
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
        /// 下载报关单solo
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public JsonResult DownloadDeclareSolo(string ids)
        {
            try
            {
                var arr = ids.Split(','); //id数组
                Encoding encoding = Encoding.UTF8;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(System.Configuration.ConfigurationManager.AppSettings["DownLoadDecheadUrlSolo"]);
                request.Method = "POST";
                request.Accept = "text/html, application/xhtml+xml, */*";
                request.ContentType = "application/json";

                byte[] buffer = encoding.GetBytes(arr.Json());
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
            catch (Exception ex)
            {
                return base.JsonResult(VueMsgType.error, ex.Message);
            }
        }

        #endregion

        #region 报关单下载

        /// <summary>
        /// 下载销售合同
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public JsonResult DownloadOrderSales(string id)
        {
            var url = System.Configuration.ConfigurationManager.AppSettings["DownLoadSalesUrl"];
            var result = NoticeExtends.HttpPostRaw(url, string.Format("id={0}", id.InputText()), "application/x-www-form-urlencoded");
            var message = result.JsonTo<JMessage>();
            if (message.success)
            {
                return base.JsonResult(VueMsgType.success, "", message.data);
            }
            else
            {
                return base.JsonResult(VueMsgType.error, message.data);
            }
        }
        #endregion

        #region 产品预归类
        /// <summary>
        /// 上传产品预归类
        /// </summary>
        /// <returns></returns>
        public JsonResult UploadPreProduct(HttpPostedFileBase file)
        {
            try
            {
                //拿到上传来的文件
                file = Request.Files["file"];
                string ext = Path.GetExtension(file.FileName).ToLower();
                string[] exts = { ".xls", ".xlsx" };
                if (exts.Contains(ext) == false)
                {
                    return base.JsonResult(VueMsgType.error, "文件格式错误，仅限.xls或.xlsx格式的文件。");
                }

                System.Data.DataTable dt = App_Utils.NPOIHelper.ExcelToDataTable(ext, file.InputStream, true);

                List<PreProcuctsData> list = new List<PreProcuctsData>();

                if (dt.Rows.Count == 0)
                {
                    return base.JsonResult(VueMsgType.error, "上传的文件错误，请下载模板，按模板要求填入正确的数据后上传。");
                }
                var currencies = ExtendsEnum.ForFrontEnd<Currency>().ToArray();

                for (var i = 0; i < dt.Rows.Count; i++)
                {
                    var array = dt.Rows[i].ItemArray;

                    PreProcuctsData preData = new PreProcuctsData
                    {
                        Name = array[0].ToString(),
                        Batch = array[1].ToString(),
                        Model = array[2].ToString(),
                        Manufacturer = array[3].ToString(),
                        UnitPrice = array[4].ToString(),
                        Currency = array[5].ToString(),
                        UniqueCode = array[6].ToString(),
                        DueDate = array[7].ToString(),
                        Qty = array[8].ToString(),
                    };

                    if (string.IsNullOrWhiteSpace(preData.Name))
                    {
                        return base.JsonResult(VueMsgType.error, string.Format("第{0}行数据产品名称不能为空", i + 3));
                    }
                    if (string.IsNullOrWhiteSpace(preData.Model))
                    {
                        return base.JsonResult(VueMsgType.error, string.Format("第{0}行数据型号不能为空", i + 3));
                    }
                    if (string.IsNullOrWhiteSpace(preData.Manufacturer))
                    {
                        return base.JsonResult(VueMsgType.error, string.Format("第{0}行数据品牌不能为空", i + 3));
                    }
                    if (string.IsNullOrWhiteSpace(preData.UnitPrice))
                    {
                        return base.JsonResult(VueMsgType.error, string.Format("第{0}行数据单价不能为空", i + 3));
                    }
                    if (string.IsNullOrWhiteSpace(preData.Currency))
                    {
                        return base.JsonResult(VueMsgType.error, string.Format("第{0}行数据币种不能为空", i + 3));
                    }

                    if (string.IsNullOrWhiteSpace(preData.UniqueCode))
                    {
                        return base.JsonResult(VueMsgType.error, string.Format("第{0}行数据物料号(产品唯一编码)不能为空", i + 3));
                    }
                    var pre = Client.Current.MyClassifiedPreProducts.ToList().Where(item => item.ClassifyStatus != PvWsOrder.Services.XDTClientView.ClassifyStatus.Anomaly && item.ProductUnionCode == preData.UniqueCode);
                    if (pre.Count() > 0)
                    {
                        return base.JsonResult(VueMsgType.error, string.Format("第{0}行数据物料号(产品唯一编码)已存在", i + 3));
                    }
                    preData.Currency = preData.Currency.ToUpper();

                    var cyrrency = currencies.Where(item => item.Value.ToString().ToUpper() == preData.Currency || item.Name.ToUpper() == preData.Currency || item.Description.ToUpper() == preData.Currency).FirstOrDefault();
                    if (cyrrency == null)
                    {
                        return base.JsonResult(VueMsgType.error, string.Format("第{0}行数据币种错误", i + 3));
                    }
                    else
                    {
                        preData.Currency = cyrrency.Name.ToString();
                    }

                    DateTime dtTry;
                    if (preData.DueDate != "")
                    {
                        if (!DateTime.TryParse(preData.DueDate, out dtTry))
                        {
                            return base.JsonResult(VueMsgType.error, string.Format("第{0}行预计到货日期格式错误", i + 3));
                        }
                    }

                    decimal intTry;
                    if (preData.Qty != "")
                    {
                        if (!decimal.TryParse(preData.Qty, out intTry))
                        {
                            return base.JsonResult(VueMsgType.error, string.Format("第{0}行数据格式错误", i + 3));
                        }
                    }

                    list.Add(preData);
                }

                list.Reverse();

                var current = Client.Current;
                foreach (var item in list)
                {
                    //插入产品
                    var product = new ClientProduct();
                    product.Batch = item.Batch;
                    product.Manufacturer = item.Manufacturer;
                    product.Model = item.Model;
                    product.Name = item.Name;
                    product.ClientID = current.XDTClientID;
                    current.MyClientProducts.Enter(product);


                    var preProduct = new PreProduct();
                    preProduct.ProductName = item.Name;
                    preProduct.ClientID = current.XDTClientID;
                    preProduct.ProductUnionCode = item.UniqueCode;
                    preProduct.Model = item.Model;
                    preProduct.Manufacturer = item.Manufacturer;
                    if (item.Qty != "")
                    {
                        preProduct.Qty = Convert.ToDecimal(item.Qty);
                    }
                    preProduct.Price = decimal.Parse(item.UnitPrice.Trim());
                    preProduct.Currency = item.Currency;
                    preProduct.CompanyType = current.XDTClientType == PvWsOrder.Services.Enums.ClientType.External ? CompanyTypeEnums.OutSide : CompanyTypeEnums.Icgoo;
                    if (item.DueDate != "")
                    {
                        preProduct.DueDate = Convert.ToDateTime(item.DueDate);
                    }
                    preProduct.UseType = PreProduct.PreProductUserType.Pre;
                    current.MyPreProducts.Enter(preProduct);
                }
            }
            catch (Exception ex)
            {
                return base.JsonResult(VueMsgType.error, ex.Message);
            }
            return base.JsonResult(VueMsgType.success, "导入成功");
        }
        #endregion

        /// <summary>
        /// 上传头像
        /// </summary>
        [UserAuthorize(UserAuthorize = true)]
        public JsonResult UpdateAvatar(HttpPostedFileBase file)
        {
            try
            {
                string userId = Client.Current.ID;

                file = Request.Files["file"];

                var fileSize = file.ContentLength / 1024;
                if (fileSize > 1024 * 3)
                {
                    return base.JsonResult(VueMsgType.error, "上传的文件大小不超过3M！");
                }

                string fileFormat = Path.GetExtension(file.FileName).ToLower();
                string[] allowFiles = { ".jpg", ".bmp", ".gif", ".png", ".jpeg" };
                if (allowFiles.Contains(fileFormat) == false)
                {
                    return base.JsonResult(VueMsgType.error, "文件格式错误，请上传图片文件！");
                }

                var result = Yahv.Alls.Current.centerFiles.fileSave(file);
                var fullUrl = PvWsOrder.Services.PvClientConfig.DomainUrl + @"/" + result;
                var contentType = file.ContentType;

                // 删除旧的头像信息
                Expression<Func<Layers.Data.Sqls.PvCenter.FilesDescription, bool>> deleteExpression =
                    item => item.AdminID == userId && item.Type == (int)FileType.Avatar && item.Status != (int)FileDescriptionStatus.Delete;
                new CenterFilesView().DeleteByLambda(deleteExpression);

                // 上传到中心
                List<CenterFileDescription> files = new List<CenterFileDescription>()
                {
                    new CenterFileDescription
                        {
                            CustomName = file.FileName,
                            Url = result, // fileurl,
                            AdminID = userId,
                            Type = (int)FileType.Avatar,
                            ClientID = userId,
                }};
                new CenterFilesView().Upload(files.ToArray());

                // 查询头像信息
                var avatarFile = new CenterFilesView().Where(t => t.AdminID == userId && t.Type == (int)FileType.Avatar).FirstOrDefault();
                var data = new
                {
                    FileName = avatarFile?.CustomName,
                    FileUrl = !string.IsNullOrEmpty(avatarFile?.Url) ? PvWsOrder.Services.PvClientConfig.FileServerUrl + @"/" + avatarFile?.Url.ToUrl() : "",
                };


                // return base.JsonResult(VueMsgType.success, "", new { name = file.FileName, URL = result, fullURL = fullUrl, fileFormat = contentType }.Json());
                return base.JsonResult(VueMsgType.success, "", data.Json());
            }
            catch (Exception ex)
            {
                return base.JsonResult(VueMsgType.error, ex.Message);
            }
        }

    }
}