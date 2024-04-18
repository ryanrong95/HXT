using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Npoi;
using Needs.Utils.Serializers;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.Ccs.Utils;

namespace WebApp.Logistics.ManifestVoyage
{
    /// <summary>
    /// 运输批次详情界面
    /// </summary>
    public partial class Detail : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadComboboxData();
            LoadData();
        }

        protected void LoadComboboxData()
        {
            this.Model.Clients = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView.Where(c => c.ClientType == ClientType.Internal).Select(c => new { c.ID, c.Company.Name }).Json();
        }

        protected void LoadData()
        {
            var id = Request.QueryString["ID"];
            var voyage = Needs.Wl.Admin.Plat.AdminPlat.Current.Voyage.ManifestSure[id];

            this.Model.Voyage = new
            {
                voyage.ID,
                VoyageNo = voyage.ID + " | " + voyage.CutStatus.GetDescription(),
                Carrier = voyage.Carrier?.Name,
                voyage.DriverCode,
                voyage.DriverName,
                voyage.HKLicense,
                TransportTime = voyage.TransportTime?.ToString("yyyy-MM-dd") ?? string.Empty,
                VoyageType = voyage.Type.GetDescription(),
                VoyageCutStatus = voyage.CutStatus,
            }.Json();
        }

        /// <summary>
        /// 运输批次附件（提货委托书/六联单）
        /// </summary>
        protected void dataVoyageFiles()
        {
            var voyageID = Request.QueryString["ID"];
            var files = Needs.Wl.Admin.Plat.AdminPlat.Current.Voyage.VoyageFiles.Where(f => f.VoyageID == voyageID);

            Func<VoyageFile, object> convert = file => new
            {
                file.ID,
                file.Name,
                FileType = file.FileType.GetDescription(),
                Url = FileDirectory.Current.FileServerUrl + "/" + file.Url.ToUrl()
            };

            Response.Write(new
            {
                rows = files.Select(convert).ToArray(),
                total = files.Count()
            }.Json());
        }

        /// <summary>
        /// 运输批次的报关单汇总统计(总件数/总数量/总毛重/总金额)
        /// </summary>
        protected void dataVoyageSubtotal()
        {
            var voyageID = Request.Form["ID"];
            var type = Request.Form["ClientType"];
            var clientID = Request.Form["ClientID"];
            var voyageDecHeads = Needs.Wl.Admin.Plat.AdminPlat.Current.Voyage.VoyageDecHeads.AsQueryable();

            if (!string.IsNullOrEmpty(voyageID))
            {
                voyageDecHeads = voyageDecHeads.Where(vd => vd.VoyageNo == voyageID);
            }
            if (!string.IsNullOrEmpty(type))
            {
                if (Int32.Parse(type) != 0)
                {
                    voyageDecHeads = voyageDecHeads.Where(vd => (int)vd.Client.ClientType == Int32.Parse(type));
                }
            }
            if (!string.IsNullOrEmpty(clientID))
            {
                voyageDecHeads = voyageDecHeads.Where(vd => vd.Client.ID == clientID);
            }

            //根据运输批次查询出"客户委托单号" Begin
            string icgooOrders = string.Empty;
            var icgooOrdersByVoyage = new Needs.Ccs.Services.Views.IcgooOrderByVoyageView().GetResults(voyageID);
            if (icgooOrdersByVoyage != null && icgooOrdersByVoyage.Any())
            {
                var icgoos = icgooOrdersByVoyage.Select(t => t.IcgooOrder).Distinct().ToArray();
                icgooOrders = string.Join(", ", icgoos);
            }
            //根据运输批次查询出"客户委托单号" End

            var voyageDecHeadArr = voyageDecHeads.ToArray();
            if (voyageDecHeadArr.Count() > 0)
            {
                VoyagePackNo voyagePackNo = new VoyagePackNo(voyageDecHeadArr.Select(t => t.ID).ToList());
                int totalPack = voyagePackNo.Calculate();
                Response.Write(new
                {
                    TotalPackNo = totalPack,
                    //TotalPackNo = voyageDecHeadArr.Sum(dh => dh.PackNo),
                    TotalQuantity = voyageDecHeadArr.Sum(dh => dh.GQty),
                    TotalGrossWt = voyageDecHeadArr.Sum(dh => dh.GrossWt),
                    TotalAmount = voyageDecHeadArr.Sum(dh => dh.DeclTotal),
                    TotalItems = voyageDecHeadArr.Sum(dh => dh.ItemsCount),

                    IcgooOrders = icgooOrders,
                }.Json());
            }
            else
            {
                Response.Write(new
                {
                    TotalPackNo = "",
                    TotalQuantity = "",
                    TotalGrossWt = "",
                    TotalAmount = "",
                    TotalItems="",

                    IcgooOrders = "",
                }.Json());
            }
        }

        /// <summary>
        /// 运输批次明细
        /// </summary>
        protected void data()
        {
            var voyageID = Request.QueryString["ID"];
            var type = Request.QueryString["ClientType"];
            var clientID = Request.QueryString["ClientID"];
            var voyageDetails = Needs.Wl.Admin.Plat.AdminPlat.Current.Voyage.VoyageDetails.AsQueryable();

            if (!string.IsNullOrEmpty(voyageID))
            {
                voyageDetails = voyageDetails.Where(vd => vd.VoyageNo == voyageID);
            }
            if (!string.IsNullOrEmpty(type))
            {
                if (Int32.Parse(type) != 0)
                {
                    voyageDetails = voyageDetails.Where(vd => (int)vd.Client.ClientType == Int32.Parse(type));
                }
            }
            else
            {
                voyageDetails = voyageDetails.Where(vd => vd.Client.ClientType == ClientType.External);
            }
            if (!string.IsNullOrEmpty(clientID))
            {
                voyageDetails = voyageDetails.Where(vd => vd.Client.ID == clientID);
            }

            Func<VoyageDetail, object> convert = vd => new
            {
                vd.ID,
                vd.OrderID,
                vd.Client.ClientCode,
                ClientName = vd.Client.Company.Name,
                PackingDate = vd.PackingDate.ToShortDateString(),
                vd.BoxIndex,
                vd.ItemsCount
            };

            Response.Write(new
            {
                rows = voyageDetails.Select(convert).ToArray(),
                total = voyageDetails.Count()
            }.Json());
        }

        /// <summary>
        /// 上传提货委托书/六连单
        /// </summary>
        protected void UploadVoyageFile()
        {
            try
            {
                var voyageID = Request.Form["VoyageID"];
                var fileType = (FileType)Enum.Parse(typeof(FileType), Request.Form["FileType"]);
                var file = Request.Files[0];
                var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

                //文件保存
                string ext = System.IO.Path.GetExtension(file.FileName);
                string fileName = DateTime.Now.Ticks + ext;

                //创建文件目录
                FileDirectory fileDic = new FileDirectory(fileName);
                fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Voyage);
                fileDic.CreateDataDirectory();
                file.SaveAs(fileDic.FilePath);

                VoyageFile voyageFile = new VoyageFile();
                voyageFile.VoyageID = voyageID;
                voyageFile.Admin = admin;
                voyageFile.Name = file.FileName;
                voyageFile.FileType = fileType;
                voyageFile.FileFormat = file.ContentType;
                voyageFile.Url = fileDic.VirtualPath;
                voyageFile.Enter();

                Response.Write((new
                {
                    success = true,
                    message = "上传成功",
                    data = new
                    {
                        ID = voyageFile.ID,
                        Name = file.FileName,
                        FileType = fileType.GetDescription(),
                        Url = fileDic.FileUrl
                    }
                }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "上传失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 删除提货委托书/六连单
        /// </summary>
        protected void DeleteVoyageFile()
        {
            try
            {
                var fileID = Request.Form["FileID"];
                var file = Needs.Wl.Admin.Plat.AdminPlat.Current.Voyage.VoyageFiles[fileID];
                file.Abandon();

                Response.Write((new { success = true, message = "删除成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "删除失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 导出货物提货委托书
        /// </summary>
        protected void ExportDeliveryAgentFile()
        {
            try
            {
                var id = Request.Form["ID"];
                var deliveryAgentFile = Needs.Wl.Admin.Plat.AdminPlat.Current.Voyage.DeliveryAgentProxies[id];
                var decHeads = new Needs.Ccs.Services.Views.DecHeadsView().Where(t => t.VoyNo == id).Select(t => t.ID).ToList();
                VoyagePackNo voyagePackNo = new VoyagePackNo(decHeads);
                int totalPack = voyagePackNo.Calculate();
                deliveryAgentFile.TotalPackNo = totalPack;
                //保存文件
                string fileName = DateTime.Now.Ticks + ".pdf";
                FileDirectory fileDic = new FileDirectory(fileName);
                fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                fileDic.CreateDataDirectory();
                deliveryAgentFile.SaveAs(fileDic.FilePath);

                Response.Write((new { success = true, message = "导出成功", url = fileDic.FileUrl }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导出失败：" + ex.Message }).Json());
            }
        }

        #region  /// 导出出库单
        /// <summary>
        /// 导出出库单
        /// </summary>
        protected void ExportExitBill()
        {
            try
            {
                var voyageID = Request.Form["ID"];
                var type = Request.Form["ClientType"];
                var clientID = Request.Form["ClientID"];

                var voyage = Needs.Wl.Admin.Plat.AdminPlat.Current.Voyage.ManifestSure[voyageID];
                var voyageDecHeads = Needs.Wl.Admin.Plat.AdminPlat.Current.Voyage.VoyageDecHeads.AsQueryable();
                var voyageDetails = Needs.Wl.Admin.Plat.AdminPlat.Current.Voyage.VoyageDetails.AsQueryable();

                if (!string.IsNullOrEmpty(voyageID))
                {
                    voyageDecHeads = voyageDecHeads.Where(vd => vd.VoyageNo == voyageID);
                    voyageDetails = voyageDetails.Where(vd => vd.VoyageNo == voyageID);
                }
                if (!string.IsNullOrEmpty(type))
                {
                    if (Int32.Parse(type) != 0)
                    {
                        voyageDecHeads = voyageDecHeads.Where(vd => (int)vd.Client.ClientType == Int32.Parse(type));
                        voyageDetails = voyageDetails.Where(vd => (int)vd.Client.ClientType == Int32.Parse(type));
                    }
                }
                if (!string.IsNullOrEmpty(clientID))
                {
                    voyageDecHeads = voyageDecHeads.Where(vd => vd.Client.ID == clientID);
                    voyageDetails = voyageDetails.Where(vd => vd.Client.ID == clientID);
                }

                var voyageDetail = voyageDetails.Select(x => new
                { 订单编号 = x.OrderID, 客户编号 = x.Client.ClientCode, 客户名称 = x.Client.Company.Name, 装箱日期 = x.PackingDate.ToShortDateString(), 箱号 = x.BoxIndex });

                var voyageDecHeadArr = voyageDecHeads.ToArray();

                var TotalPackNo = "";
                var TotalQuantity = "";
                var TotalGrossWt = "";
                var TotalAmount = "";
                if (voyageDecHeadArr.Count() > 0)
                {
                    TotalPackNo = voyageDecHeadArr.Sum(dh => dh.PackNo).ToString();
                    TotalQuantity = voyageDecHeadArr.Sum(dh => dh.GQty).ToString("0.#####");
                    TotalGrossWt = voyageDecHeadArr.Sum(dh => dh.GrossWt).ToString("0.#####");
                    TotalAmount = voyageDecHeadArr.Sum(dh => dh.DeclTotal).ToString("0.#####");
                }
                DataTable dt = new DataTable("AttachHead");
                dt.Columns.Add("col1");
                dt.Columns.Add("col2");
                dt.Columns.Add("col3");
                dt.Columns.Add("col4");
                dt.Columns.Add("col5");
                DataRow row1 = dt.NewRow();
                row1["col1"] = "货物运输批次号";
                row1["col2"] = voyage.ID + " | " + voyage.CutStatus.GetDescription();
                row1["col3"] = "承运商";
                row1["col4"] = voyage.Carrier?.Name;
                dt.Rows.Add(row1);
                DataRow row2 = dt.NewRow();
                row2["col1"] = "车牌号";
                row2["col2"] = voyage.HKLicense;
                row2["col3"] = "司机姓名";
                row2["col4"] = voyage.DriverName;
                dt.Rows.Add(row2);
                DataRow row3 = dt.NewRow();
                row3["col1"] = "运输时间";
                row3["col2"] = voyage.TransportTime?.ToString("yyyy-MM-dd") ?? string.Empty;
                row3["col3"] = "运输类型";
                row3["col4"] = voyage.Type.GetDescription();
                dt.Rows.Add(row3);
                DataRow row4 = dt.NewRow();
                row4["col1"] = "总件数";
                row4["col2"] = TotalPackNo;
                row4["col3"] = "总数量";
                row4["col4"] = TotalQuantity;
                dt.Rows.Add(row4);
                DataRow row5 = dt.NewRow();
                row5["col1"] = "总毛重";
                row5["col2"] = TotalGrossWt;
                row5["col3"] = "总金额";
                row5["col4"] = TotalAmount;
                dt.Rows.Add(row5);
                IWorkbook workbook = ExcelFactory.Create();
                Needs.Utils.Npoi.NPOIHelper npoi = new Needs.Utils.Npoi.NPOIHelper(workbook);
                StyleConfig config = new StyleConfig() { Title = "出库单" };
                int[] columnsWidth = { 20, 20, 30, 23, 20 };
                npoi.EnumrableToExcel(voyageDetail, dt, config, columnsWidth);

                var fileName = DateTime.Now.Ticks + ".xlsx";
                FileDirectory file = new FileDirectory(fileName);
                file.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                file.CreateDataDirectory();
                //保存文件
                npoi.SaveAs(file.FilePath);

                Response.Write((new { success = true, message = "导出成功", url = file.FileUrl }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导出失败：" + ex.Message }).Json());
            }
        }
        #endregion

        /// <summary>
        /// 确认出库
        /// </summary>
        protected void SaveOutStock()
        {
            try
            {
                //运输批次ID/运输批次号
                string ID = Request["ID"];

                //香港库房出库
                /*
                var ExitNotice = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.HKExitNotice.Where(x => x.DecHead.VoyNo == ID).ToArray();
                Task.Run(() =>
                {
                    foreach (var item in ExitNotice)
                    {
                        item.OutStock();
                    }
                });
                */

                //按运输批次号出库
                Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.HKExitNotice.OutStockByVoyageNo(ID);

                //标记运输批次完成
                var voyage = Needs.Wl.Admin.Plat.AdminPlat.Current.Voyage.ManifestSure[ID];
                voyage.Complate();

                Response.Write((new { success = true, message = "出库成功！" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "处理失败：" + ex.Message }).Json());
            }
        }
    }
}