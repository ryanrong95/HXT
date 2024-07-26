using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Needs.Utils.Converters;
using YuanDa_Logistics.Utility;
using System.Configuration;
using Needs.Utils;
using System.IO;

namespace WebApp.SZWareHouse.Exit
{
    /// <summary>
    /// 出库-快递单
    /// 深圳库房
    /// </summary>
    public partial class ExpressBill : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        /// <summary>
        /// 快递单信息
        /// </summary>
        protected void LoadData()
        {
            this.Model.ExitNotice = "";
            string ID = Request["ExitNoticeID"];
            if (string.IsNullOrEmpty(ID) == true)
            {
                return;
            }
            var exitNotice = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.SZExitNotice[ID];
            //var PackingDate = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.EntryNotice
            //    .Where(x => x.Order.ID == exitNotice.Order.ID && x.EntryNoticeStatus == EntryNoticeStatus.Sealed)
            //    .FirstOrDefault()
            //    .UpdateDate;
            var exitNoticeFile = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.ExitNoticeFilesView
                .Where(t => t.ExitNoticeID == ID && t.FileType == Needs.Wl.Models.Enums.FileType.ReceiptConfirmationFile && t.Status == Needs.Wl.Models.Enums.Status.Normal)
                .FirstOrDefault();

            if (exitNotice != null)
            {
                this.Model.ExitNotice = new
                {
                    ExitNoticeID = exitNotice.ExpressBill.ExitNoticeID,
                    OrderID = exitNotice.ExpressBill.OrderID,
                    ClientName = exitNotice.ExpressBill.ClientName,
                    Contactor = exitNotice.ExpressBill.Contactor,//提货人
                    ContactTel = exitNotice.ExpressBill.ContactTel,
                    Address = exitNotice.ExpressBill.Address,
                    ExpressComp = exitNotice.ExpressBill.ExpressComp,
                    ExpressTy = exitNotice.ExpressBill.ExpressTy,
                    ExpressPayType = exitNotice.ExpressBill.ExpressPayType.GetDescription(),
                    PackNo = exitNotice.ExpressBill.PackNo,
                    ExitType = (int)ExitType.Express,
                    DeliveryTime = exitNotice.ExpressBill.DeliveryTime.ToString("yyyy-MM-dd"),
                    //SZPackingDate = PackingDate.ToString("yyyy-MM-dd"),//出库的装箱日期取的是香港的封箱日期
                    Purchaser = PurchaserContext.Current.CompanyName,
                    BillStamp = "../../" + PurchaserContext.Current.BillStamp.ToUrl(),
                    CreateDate = exitNotice.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    IsFileUploaded = exitNoticeFile != null ? true : false,
                    FileName = exitNoticeFile != null ? exitNoticeFile.Name : string.Empty,
                    FileUrl = exitNoticeFile != null ? ConfigurationManager.AppSettings["FileServerUrl"] + @"/" + exitNoticeFile.URL.Replace(@"\", @"/") : string.Empty,
                    ExitNoticeStatus = exitNotice.ExitNoticeStatus,
                }.Json();
            }
        }

        /// <summary>
        /// 待出库商品信息
        /// </summary>
        protected void ProductData()
        {
            string id = Request["ExitNoticeID"];
            var exitNotice = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.SZExitNoticeItem.Where(x => x.ExitNoticeID == id).OrderBy(x => x.StoreStorage.BoxIndex);
            Func<SZExitNoticeItem, object> convert = item => new
            {
                ID = item.ID,
                SortingID = item.Sorting.ID,
                CaseNumber = item.StoreStorage.BoxIndex,
                StockCode = item.StoreStorage.StockCode,
                NetWeight = item.Sorting.NetWeight,
                GrossWeight = item.Sorting.GrossWeight,
                ProductName = item.Sorting.OrderItem.Category.Name,
                Model = item.Sorting.OrderItem.Model,
                Qty = item.Quantity,
                WrapType = item.Sorting.WrapType,
                Manufacturer = item.Sorting.OrderItem.Manufacturer,
                PackingDate = item.UpdateDate.ToString("yyyy-MM-dd"),
            };
            Response.Write(new
            {
                rows = exitNotice.Select(convert).ToArray(),
                total = exitNotice.Count()
            }.Json());

        }

        /// <summary>
        /// 生成快递面单
        /// </summary>
        /// <returns></returns>
        protected void GenerateExpress()
        {
            try
            {
                string ExitNoticeID = Request["ExitNoticeID"];
                var exitNotice = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.SZExitNotice[ExitNoticeID];

                //获取请求数据json
                string requestData = exitNotice.KDDRequestModel.Json();
                //调用快递方法
                var bs = new KdApiEOrder();
                var result = bs.orderTracesSubByJson(requestData);

                //修改请求返回结果
                if (exitNotice.KDDRequestModel.ShipperCode == "KYSY")
                {
                    //跨域默认的发货件数都是1,返回后需修改为真实数量
                    result = result.Replace("件数:1", "件数:" + exitNotice.ExitDeliver.PackNo);
                    result = result.Replace("电子元器件X1", "电子元器件X" + exitNotice.ExitDeliver.PackNo);
                }
                //隐藏月结卡号
                result = result.Replace("月结卡号:7550921123", "月结卡号:" + "075*******23");

                //获取返回结果
                var ResponseResult = result.JsonTo<KDDResultModel>();
                if (ResponseResult.Success)
                {
                    //保存快递面单数据
                    var express = exitNotice.ExitDeliver.Expressage;
                    express.WaybillCode = ResponseResult.Order.LogisticCode;
                    express.SaveKDD();
                    //Response.Write(ResponseResult.PrintTemplate.Json());
                    Response.Write(new
                    {
                        success = ResponseResult.Success,
                        message = ResponseResult.Reason,
                        LogisticCode = ResponseResult.Order?.LogisticCode,
                        PrintTemplate = ResponseResult.PrintTemplate,
                        ResponseResult.SubPrintTemplates,
                        ShipperCode = exitNotice.KDDRequestModel.ShipperCode
                    }.Json());
                }
                else
                {
                    Response.Write((new { success = false, message = ResponseResult.Reason }).Json());
                }
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "请求失败" + ex }).Json());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected Dictionary<string, string> HandleAddress(string Address)
        {
            var Province = "";
            var City = "";
            var Area = "";
            var DetailsAddress = "";
            if (Address.Split(' ').Length == 3)
            {
                Province = Address.Split(' ')[0].Trim();
                City = Address.Split(' ')[0].Trim() + "市";
                Area = Address.Split(' ')[1].Trim();
                DetailsAddress = Address.Split(' ')[2].Trim();
            }
            else
            {
                Province = Address.Split(' ')[0].Trim();
                if (Province == "内蒙古" || Province == "西藏")
                    Province = Address.Split(' ')[0] + "自治区";
                if (Province == "新疆")
                    Province = Address.Split(' ')[0] + "维吾尔自治区";
                if (Province == "广西")
                    Province = Address.Split(' ')[0] + "壮族自治区";
                if (Province == "宁夏")
                    Province = Address.Split(' ')[0] + "回族自治区";
                else
                {
                    Province = Address.Split(' ')[0] + "省";
                }
                City = Address.Split(' ')[1].Trim();
                Area = Address.Split(' ')[2].Trim();
                DetailsAddress = Address.Split(' ')[3].Trim();
            }
            var DicAddres = new Dictionary<string, string>();
            DicAddres.Add("Province", Province);
            DicAddres.Add("City", City);
            DicAddres.Add("Area", Area);
            DicAddres.Add("DetailsAddress", DetailsAddress);
            return DicAddres;
        }

        /// <summary>
        /// 更新打印状态
        /// </summary>

        protected void UpdatePrintStatus()
        {
            try
            {
                string ID = Request["ExitNoticeID"];
                if (string.IsNullOrEmpty(ID))
                {
                    return;
                }
                var exitNotice = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.SZExitNotice[ID];
                exitNotice.UpdatePrintStatus();
            }
            catch (Exception ex)
            {

                Response.Write((new { success = false, message = "打印失败，" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 扫描出库
        /// </summary>
        protected void OutStock()
        {
            try
            {
                string ExitNoticeID = Request["ExitNoticeID"];
                if (string.IsNullOrEmpty(ExitNoticeID))
                {
                    throw new Exception("通知编号为空");
                }
                var ExitNotice = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.SZExitNotice[ExitNoticeID];
                if (ExitNotice == null)
                {
                    throw new Exception("查询通知结果为NULL");
                }
                if (ExitNotice.ExitNoticeStatus == ExitNoticeStatus.Exited)
                {
                    throw new Exception("订单已出库");
                }
                foreach (var item in ExitNotice.SZItems)
                {
                    if (string.IsNullOrEmpty(item.StoreStorage?.StockCode))
                    {
                        throw new Exception("产品还未入库");
                    }
                }
                //出库
                ExitNotice.Admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                ExitNotice.OutStock();

                Response.Write((new { success = true, message = "出库成功！" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "出库失败，" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 上传客户收货确认单
        /// </summary>
        protected void UploadReceiptConfirmFile()
        {
            try
            {
                string exitNoticeID = Request.Form["exitnoticeid"];

                List<dynamic> fileList = new List<dynamic>();
                IList<HttpPostedFile> files = System.Web.HttpContext.Current.Request.Files.GetMultiple("upload-receipt-confirm-file");
                if (files.Count > 0)
                {
                    var validTypes = new List<string>() { ".jpg", ".bmp", ".jpeg", ".gif", ".png", };  //".pdf"
                    for (int i = 0; i < files.Count; i++)
                    {
                        string ext = Path.GetExtension(files[i].FileName);
                        if (!validTypes.Contains(ext.ToLower()))
                        {
                            Response.Write((new { success = false, message = "上传的原始PI只能是图片(jpg、bmp、jpeg、gif、png)格式！" }).Json()); //或pdf
                            return;
                        }

                        //处理附件
                        HttpPostedFile file = files[i];
                        if (file.ContentLength != 0)
                        {
                            //文件保存
                            string fileName = files[i].FileName.ReName();

                            //创建文件目录
                            FileDirectory fileDic = new FileDirectory(fileName);
                            fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Warehouse);
                            fileDic.CreateDataDirectory();
                            file.SaveAs(fileDic.FilePath);

                            //插入数据 ExitNoticeFiles 表
                            //Needs.Wl.Warehouse.Services.Models.ExitNoticeFile exitNoticeFile = new Needs.Wl.Warehouse.Services.Models.ExitNoticeFile()
                            //{
                            //    ID = Guid.NewGuid().ToString("N"),
                            //    ExitNoticeID = exitNoticeID,
                            //    AdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID,
                            //    Name = file.FileName,
                            //    FileType = Needs.Wl.Models.Enums.FileType.ReceiptConfirmationFile,
                            //    FileFormat = file.ContentType,
                            //    URL = fileDic.VirtualPath,
                            //    Status = Needs.Wl.Models.Enums.Status.Normal,
                            //    CreateDate = DateTime.Now,
                            //};
                            //exitNoticeFile.InsertUniqueFileForOneExitNotice(exitNoticeID);

                            Needs.Ccs.Services.Models.ExitNoticeFile exitNoticeFile = new Needs.Ccs.Services.Models.ExitNoticeFile()
                            {
                                ID = Guid.NewGuid().ToString("N"),
                                ExitNoticeID = exitNoticeID,
                                AdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID,
                                Admin = new Admin() { ID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID },
                                Name = file.FileName,
                                FileType = Needs.Ccs.Services.Enums.FileType.ReceiptConfirmationFile,
                                FileFormat = file.ContentType,
                                URL = fileDic.VirtualPath,
                                Status = Needs.Ccs.Services.Enums.Status.Normal,
                                CreateDate = DateTime.Now,
                            };
                            SZExitInsertFileHandler handler = new SZExitInsertFileHandler(exitNoticeFile, exitNoticeID, Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                            handler.Execute();

                            fileList.Add(new
                            {
                                Name = file.FileName,
                                FileType = FileType.ReceiptConfirmationFile.GetDescription(),
                                FileFormat = file.ContentType,
                                VirtualPath = fileDic.VirtualPath,
                                Url = fileDic.FileUrl
                            });
                        }
                    }
                }

                if (fileList.Count() == 0)
                {
                    Response.Write((new { success = false, message = "服务器未保存文件！", }).Json());
                }
                else
                {
                    Response.Write((new { success = true, data = fileList }).Json());
                }
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "上传失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 完成操作
        /// </summary>
        protected void Complete()
        {
            try
            {
                string exitNoticeID = Request.Form["exitnoticeid"];

                SZExitCompleteHandler handler = new SZExitCompleteHandler(exitNoticeID, Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                handler.Execute();

                Response.Write((new { success = true, message = string.Empty, }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "操作失败：" + ex.Message }).Json());
            }
        }

    }
}