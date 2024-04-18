using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using YuanDa_Logistics.Utility;

namespace WebApp.SZWareHouse.Exit
{
    /// <summary>
    /// 待出库信息显示
    /// 深圳库房
    /// </summary>
    public partial class LadingBill : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        /// <summary>
        /// 提货单信息
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
            //  var SZPackingDate = exitNotice.SZItems.Select(x => x.Sorting.SZPackingDate).FirstOrDefault();
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
                    ExitNoticeID = exitNotice.LadingBill.ExitNoticeID,
                    OrderID = exitNotice.LadingBill.OrderID,
                    ClientName = exitNotice.LadingBill.ClientName,
                    DeliveryName = exitNotice.LadingBill.DeliveryName,//提货人
                    DeliveryTel = exitNotice.LadingBill.DeliveryTel,
                    IDType = exitNotice.LadingBill.IDType,
                    IDCard = exitNotice.LadingBill.IDCard,
                    PackNo = exitNotice.LadingBill.PackNo,
                    ExitType = (int)ExitType.PickUp,
                    DeliveryTime = exitNotice.LadingBill.DeliveryTime.ToString("yyyy-MM-dd"),
                    //SZPackingDate= PackingDate.ToString("yyyy-MM-dd"),//出库的装箱日期取的是香港的封箱日期
                    Purchaser = PurchaserContext.Current.CompanyName,
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
        /// 确认出库
        /// </summary>
        protected void ConfirmExitStock()
        {
            try
            {
                string id = Request["ID"];
                var ExitNotice = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.SZExitNotice[id];
                ExitNotice.OutStock();
                Response.Write((new { success = true, message = "出库成功！" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "处理失败：" + ex.Message }).Json());
            }
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
        /// 只上传文件
        /// </summary>
        protected void OnlyUploadFile()
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
                            SZExitOnlyInsertFileHandler handler = new SZExitOnlyInsertFileHandler(exitNoticeFile, exitNoticeID);
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

        protected void Lading()
        {
            try
            {
                string exitNoticeID = Request.Form["exitnoticeid"];

                SZExitCompleteHandler handler = new SZExitCompleteHandler(exitNoticeID, Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                handler.Execute();

                Response.Write((new { success = true, message = "提货成功！", }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "操作失败：" + ex.Message }).Json());
            }
        }

    }
}