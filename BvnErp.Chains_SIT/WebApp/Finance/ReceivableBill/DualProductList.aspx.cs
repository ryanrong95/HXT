using Needs.Ccs.Services.Models;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.Ccs.Utils;

namespace WebApp.Finance.ReceivableBill
{
    public partial class DualProductList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string DDateStartDate = Request.QueryString["DDateStartDate"];
            string DDateEndDate = Request.QueryString["DDateEndDate"];
            string OwnerName = Request.QueryString["OwnerName"];

            using (var query = new Needs.Ccs.Services.Views.DualProductListView())
            {
                var view = query;

                if (!string.IsNullOrEmpty(DDateStartDate))
                {
                    DateTime begin = DateTime.Parse(DDateStartDate);
                    view = view.SearchByDDateBegin(begin);
                }
                if (!string.IsNullOrEmpty(DDateEndDate))
                {
                    DateTime end = DateTime.Parse(DDateEndDate);
                    end = end.AddDays(1);
                    view = view.SearchByDDateEnd(end);
                }
                if (!string.IsNullOrEmpty(OwnerName))
                {
                    OwnerName = OwnerName.Trim();
                    view = view.SearchByOwnerName(OwnerName);
                }

                Response.Write(view.ToMyPage(page, rows).Json());
            }
        }

        /// <summary>
        /// 导出 Excel
        /// </summary>
        protected void ExportExcel()
        {
            try
            {
                string DDateStartDate = Request.Form["DDateStartDate"];
                string DDateEndDate = Request.Form["DDateEndDate"];
                string OwnerName = Request.Form["OwnerName"];

                using (var query = new Needs.Ccs.Services.Views.DualProductListView())
                {
                    var view = query;

                    if (!string.IsNullOrEmpty(DDateStartDate))
                    {
                        DateTime begin = DateTime.Parse(DDateStartDate);
                        view = view.SearchByDDateBegin(begin);
                    }
                    if (!string.IsNullOrEmpty(DDateEndDate))
                    {
                        DateTime end = DateTime.Parse(DDateEndDate);
                        end = end.AddDays(1);
                        view = view.SearchByDDateEnd(end);
                    }
                    if (!string.IsNullOrEmpty(OwnerName))
                    {
                        OwnerName = OwnerName.Trim();
                        view = view.SearchByOwnerName(OwnerName);
                    }

                    var dataListJson = view.ToMyPage().Json();

                    //写入数据
                    DataTable dt = NPOIHelper.JsonToDataTable(dataListJson);

                    string fileName = DateTime.Now.Ticks + ".xls";

                    //创建文件目录
                    FileDirectory fileDic = new FileDirectory(fileName);
                    fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                    fileDic.CreateDataDirectory();

                    #region 设置导出格式

                    var excelconfig = new ExcelConfig();
                    excelconfig.FilePath = fileDic.FilePath;
                    excelconfig.Title = "应收账款-双抬头货款";
                    excelconfig.TitleFont = "微软雅黑";
                    excelconfig.TitlePoint = 16;
                    excelconfig.IsAllSizeColumn = true;
                    //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
                    List<ColumnEntity> listColumnEntity = new List<ColumnEntity>();
                    excelconfig.ColumnEntity = listColumnEntity;

                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "OwnerName", ExcelColumn = "客户名称", Alignment = "center" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ContractNo", ExcelColumn = "合同号", Alignment = "center" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Currency", ExcelColumn = "币种", Alignment = "center" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "DDate", ExcelColumn = "报关日期", Alignment = "center" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "DeclarationAmount", ExcelColumn = "报关金额", Alignment = "left" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "AttorneyAmount", ExcelColumn = "委托金额", Alignment = "left" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ConsignorCode", ExcelColumn = "供应商", Alignment = "center" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ImportTaxValue", ExcelColumn = "关税", Alignment = "left" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "AddedValueTaxValue", ExcelColumn = "增值税", Alignment = "left" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ThatDayExchangeRate", ExcelColumn = "当天汇率", Alignment = "center" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "CustomsExchangeRate", ExcelColumn = "海关汇率", Alignment = "center" });

                    #endregion

                    //调用导出方法
                    NPOIHelper.ExcelDownload(dt, excelconfig);

                    Response.Write((new
                    {
                        success = true,
                        message = "导出成功",
                        url = fileDic.FileUrl
                    }).Json());
                }
            }
            catch (Exception ex)
            {
                Response.Write((new
                {
                    success = false,
                    message = "导出失败：" + ex.Message,
                }).Json());
            }
        }

        /// <summary>
        /// 导出报关单
        /// </summary>
        protected void ExportDecHeadFile()
        {
            try
            {
                string DecHeadIDs = Request.Form["DecHeadIDs"];

                string[] DecHeadIDs_Array = DecHeadIDs.Split(',');

                var relatedFiles = new Needs.Ccs.Services.Views.Origins.DecHeadFilesOrigin()
                    .Where(t => DecHeadIDs_Array.Contains(t.DecHeadID)
                             && t.FileType == Needs.Ccs.Services.Enums.FileType.DecHeadFile
                             && t.Status == Needs.Ccs.Services.Enums.Status.Normal)
                    .ToArray();

                if (relatedFiles == null || relatedFiles.Length <= 0)
                {
                    Response.Write((new { success = false, message = "导出失败：未查询到文件" }).Json());
                    return;
                }

                //1.创建文件夹(文件压缩后存放的地址)
                FileDirectory file = new FileDirectory();
                file.SetChildFolder(Needs.Ccs.Services.SysConfig.ZipFiles);
                file.CreateDataDirectory();
                string filePrefix = file.RootDirectory;

                List<string> files = new List<string>();

                foreach (var relatedFile in relatedFiles)
                {
                    files.Add((filePrefix + @"\" + relatedFile.Url).ToUrl());
                }

                string zipFileName = "报关单_" + DateTime.Now.ToString("yyyyMMddHHmmss") + Needs.Ccs.Services.SysConfig.Postfix;
                ZipFile zip = new ZipFile(zipFileName);
                zip.SetFilePath(file.FilePath);
                zip.Files = files;
                zip.ZipFiles();
                Response.Write((new { success = true, message = "导出成功", url = file.FileUrl + zipFileName }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导出失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 导出增值税发票
        /// </summary>
        protected void ExportDecHeadVatFile()
        {
            try
            {
                string DecHeadIDs = Request.Form["DecHeadIDs"];

                string[] DecHeadIDs_Array = DecHeadIDs.Split(',');

                var relatedFiles = new Needs.Ccs.Services.Views.Origins.DecHeadFilesOrigin()
                    .Where(t => DecHeadIDs_Array.Contains(t.DecHeadID)
                             && t.FileType == Needs.Ccs.Services.Enums.FileType.DecHeadVatFile
                             && t.Status == Needs.Ccs.Services.Enums.Status.Normal)
                    .ToArray();

                if (relatedFiles == null || relatedFiles.Length <= 0)
                {
                    Response.Write((new { success = false, message = "导出失败：未查询到文件" }).Json());
                    return;
                }

                //1.创建文件夹(文件压缩后存放的地址)
                FileDirectory file = new FileDirectory();
                file.SetChildFolder(Needs.Ccs.Services.SysConfig.ZipFiles);
                file.CreateDataDirectory();
                string filePrefix = file.RootDirectory;

                List<string> files = new List<string>();

                foreach (var relatedFile in relatedFiles)
                {
                    files.Add((filePrefix + @"\" + relatedFile.Url).ToUrl());
                }

                string zipFileName = "增值税发票_" + DateTime.Now.ToString("yyyyMMddHHmmss") + Needs.Ccs.Services.SysConfig.Postfix;
                ZipFile zip = new ZipFile(zipFileName);
                zip.SetFilePath(file.FilePath);
                zip.Files = files;
                zip.ZipFiles();
                Response.Write((new { success = true, message = "导出成功", url = file.FileUrl + zipFileName }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导出失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 导出关税发票
        /// </summary>
        protected void ExportDecHeadTariffFile()
        {
            try
            {
                string DecHeadIDs = Request.Form["DecHeadIDs"];

                string[] DecHeadIDs_Array = DecHeadIDs.Split(',');

                var relatedFiles = new Needs.Ccs.Services.Views.Origins.DecHeadFilesOrigin()
                    .Where(t => DecHeadIDs_Array.Contains(t.DecHeadID)
                             && t.FileType == Needs.Ccs.Services.Enums.FileType.DecHeadTariffFile
                             && t.Status == Needs.Ccs.Services.Enums.Status.Normal)
                    .ToArray();

                if (relatedFiles == null || relatedFiles.Length <= 0)
                {
                    Response.Write((new { success = false, message = "导出失败：未查询到文件" }).Json());
                    return;
                }

                //1.创建文件夹(文件压缩后存放的地址)
                FileDirectory file = new FileDirectory();
                file.SetChildFolder(Needs.Ccs.Services.SysConfig.ZipFiles);
                file.CreateDataDirectory();
                string filePrefix = file.RootDirectory;

                List<string> files = new List<string>();

                foreach (var relatedFile in relatedFiles)
                {
                    files.Add((filePrefix + @"\" + relatedFile.Url).ToUrl());
                }

                string zipFileName = "关税发票_" + DateTime.Now.ToString("yyyyMMddHHmmss") + Needs.Ccs.Services.SysConfig.Postfix;
                ZipFile zip = new ZipFile(zipFileName);
                zip.SetFilePath(file.FilePath);
                zip.Files = files;
                zip.ZipFiles();
                Response.Write((new { success = true, message = "导出成功", url = file.FileUrl + zipFileName }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导出失败：" + ex.Message }).Json());
            }
        }
        
    }
}