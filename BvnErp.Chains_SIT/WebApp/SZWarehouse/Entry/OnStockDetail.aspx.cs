using Needs.Ccs.Services;
using Needs.Utils;
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
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.SZWarehouse.Entry
{
    public partial class OnStockDetail : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Model.OnStockListPageNumber = Request.QueryString["OnStockListPageNumber"];
            this.Model.OnStockListPageSize = Request.QueryString["OnStockListPageSize"];
            this.Model.OnStockListScVoyageID = Server.UrlDecode(Request.QueryString["OnStockListScVoyageID"]);
            this.Model.OnStockListScCarrierName = Server.UrlDecode(Request.QueryString["OnStockListScCarrierName"]);

            string voyageID = Request.QueryString["VoyageID"];
            if (!string.IsNullOrEmpty(voyageID))
            {
                voyageID = voyageID.Trim();
            }

            var voyage = Needs.Wl.Admin.Plat.AdminPlat.Current.Voyage.ManifestSure[voyageID];
            this.Model.VoyageInfo = new
            {
                VoyageID = voyage.ID,
                VoyageNo = voyage.ID,
                Carrier = voyage.Carrier?.Name,
                voyage.DriverCode,
                voyage.DriverName,
                voyage.HKLicense,
                TransportTime = voyage.TransportTime?.ToString("yyyy-MM-dd") ?? string.Empty,
                VoyageType = voyage.Type.GetDescription()
            }.Json();


            //var voyageDecHeads = Needs.Wl.Admin.Plat.AdminPlat.Current.Voyage.VoyageDecHeads.AsQueryable();
            //voyageDecHeads = voyageDecHeads.Where(t => t.VoyageNo == voyageID);
            //this.Model.VoyageDecHead = new
            //{
            //    TotalPackNo = voyageDecHeads.Sum(t => t.PackNo),
            //    TotalGrossWt = voyageDecHeads.Sum(t => t.GrossWt),
            //}.Json();

            //计算该运输批次号下总箱数
            this.Model.TotalPackNo = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.SZOnStockView.CaleBoxNum(voyageID);
            //计算该运输批次号下总毛重
            this.Model.TotalGrossWt = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.SZOnStockView.CalcTotalGrossWt(voyageID);

            //内单客户下拉框
            this.Model.InternalClients = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView.Where(c => c.ClientType == Needs.Ccs.Services.Enums.ClientType.Internal)
                .Select(c => new { c.ID, c.Company.Name }).Json();
        }

        /// <summary>
        /// 该运输批次中的箱子列表
        /// </summary>
        protected void BoxInfoList()
        {
            var VoyageID = Request.QueryString["VoyageID"];

            string ClientCode = Request.QueryString["ClientCode"];
            string ClientName = Request.QueryString["ClientName"];
            string PackingDate = Request.QueryString["PackingDate"];
            string BoxIndex = Request.QueryString["BoxIndex"];
            string ClientType = Request.QueryString["ClientType"];
            string ClientID = Request.QueryString["ClientID"];

            List<LambdaExpression> lamdas = new List<LambdaExpression>();

            if (!string.IsNullOrEmpty(ClientCode))
            {
                ClientCode = ClientCode.Trim();
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.SZOnStockView.BoxInfoListModel, bool>>)(t => t.ClientCode.Contains(ClientCode)));
            }
            if (!string.IsNullOrEmpty(ClientName))
            {
                ClientName = ClientName.Trim();
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.SZOnStockView.BoxInfoListModel, bool>>)(t => t.ClientName.Contains(ClientName)));
            }
            if (!string.IsNullOrEmpty(PackingDate))
            {
                PackingDate = PackingDate.Trim();
                DateTime dt;
                if (DateTime.TryParse(PackingDate, out dt))
                {
                    lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.SZOnStockView.BoxInfoListModel, bool>>)(t => t.PackingDate == dt));
                }
            }
            if (!string.IsNullOrEmpty(BoxIndex))
            {
                BoxIndex = BoxIndex.Trim();
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.SZOnStockView.BoxInfoListModel, bool>>)(t => t.BoxIndex == BoxIndex));
            }
            if (!string.IsNullOrEmpty(ClientType))
            {
                int ClientTypeInt = 0;
                if (int.TryParse(ClientType, out ClientTypeInt))
                {
                    lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.SZOnStockView.BoxInfoListModel, bool>>)(t => t.ClientType == ClientTypeInt));
                }
            }
            if (!string.IsNullOrEmpty(ClientID))
            {
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.SZOnStockView.BoxInfoListModel, bool>>)(t => t.ClientID == ClientID));
            }

            int totalCount = 0;

            var listPacking = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.SZOnStockView.GetBoxInfoListModel(out totalCount, VoyageID, lamdas.ToArray()).ToList();

            Func<Needs.Ccs.Services.Views.SZOnStockView.BoxInfoListModel, object> convert = item => new
            {
                OrderID = item.OrderID,
                ClientCode = item.ClientCode,
                ClientName = item.ClientName,
                PackingDate = item.PackingDate.ToString("yyyy-MM-dd"),
                BoxIndex = item.BoxIndex,
                StockCode = string.IsNullOrEmpty(item.StockCode) ? "--" : item.StockCode,
                IsOnStock = item.IsOnStock ? "已上架" : "未上架",
                IsOnStockValue = item.IsOnStock,
            };

            Response.Write(new
            {
                rows = listPacking.Select(convert).ToArray(),
                total = totalCount,
            }.Json());
        }

        /// <summary>
        /// 上架
        /// </summary>
        protected void OnStock()
        {
            try
            {
                string voyageID = Request.Form["VoyageID"];
                string stockCode = Request.Form["StockCode"];
                stockCode = stockCode.Trim();
                var boxInfoForm = Request.Form["BoxInfo"].Replace("&quot;", "\'").Replace("amp;", "");
                var boxInfos = boxInfoForm.JsonTo<dynamic>();

                List<Needs.Ccs.Services.Views.SZOnStockView.TargetSortingModel> listBox = new List<Needs.Ccs.Services.Views.SZOnStockView.TargetSortingModel>();

                foreach (var boxInfo in boxInfos)
                {
                    listBox.Add(new Needs.Ccs.Services.Views.SZOnStockView.TargetSortingModel()
                    {
                        OrderID = boxInfo.OrderID,
                        BoxIndex = boxInfo.BoxIndex,
                    });
                }

                new Needs.Ccs.Services.Models.SZEntryNotice().OnStock(voyageID, stockCode, listBox);

                Response.Write((new { success = "true", message = "提交成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = "false", message = "提交失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 完成
        /// </summary>
        protected void Complete()
        {
            try
            {
                string voyageID = Request.Form["VoyageID"];

                new Needs.Ccs.Services.Models.SZEntryNotice().Complete(voyageID);

                Response.Write((new { success = "true", message = "提交成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = "false", message = "提交失败：" + ex.Message }).Json());
            }
        }

        /*
        /// <summary>
        /// 导出入库单
        /// </summary>
        protected void ExportEntryBill()
        {
            try
            {
                var voyageID = Request.Form["VoyageID"];
                if (!string.IsNullOrEmpty(voyageID))
                {
                    voyageID = voyageID.Trim();
                }

                var voyage = Needs.Wl.Admin.Plat.AdminPlat.Current.Voyage.ManifestSure[voyageID];

                var listPacking = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.SZOnStockView.GetBoxInfoListModel(voyageID);
                var voyageDetail = listPacking.Select(x => new
                { 订单编号 = x.OrderID, 客户编号 = x.ClientCode, 客户名称 = x.ClientName, 装箱日期 = x.PackingDate.ToString("yyyy-MM-dd"), 箱号 = x.BoxIndex });

                //计算该运输批次号下总箱数
                var TotalPackNo = Convert.ToString(Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.SZOnStockView.CaleBoxNum(voyageID));
                //计算该运输批次号下总毛重
                var TotalGrossWt = Convert.ToString(Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.SZOnStockView.CalcTotalGrossWt(voyageID));

                DataTable dt = new DataTable("AttachHead");
                dt.Columns.Add("col1");
                dt.Columns.Add("col2");
                dt.Columns.Add("col3");
                dt.Columns.Add("col4");
                dt.Columns.Add("col5");
                DataRow row1 = dt.NewRow();
                row1["col1"] = "运输批次";
                row1["col2"] = voyage.ID;
                row1["col3"] = "承运商";
                row1["col4"] = voyage.Carrier?.Name;
                dt.Rows.Add(row1);
                DataRow row2 = dt.NewRow();
                row2["col1"] = "车牌号";
                row2["col2"] = voyage.Vehicle?.HKLicense == null ? "" : voyage.Vehicle?.HKLicense;
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
                row4["col2"] = Convert.ToInt32(TotalPackNo);
                row4["col3"] = "总毛重(KG)";
                row4["col4"] = TotalGrossWt;
                dt.Rows.Add(row4);

                IWorkbook workbook = ExcelFactory.Create();
                Needs.Utils.Npoi.NPOIHelper npoi = new Needs.Utils.Npoi.NPOIHelper(workbook);
                StyleConfig config = new StyleConfig() { Title = "入库单" };
                int[] columnsWidth = { 20, 20, 30, 23, 10, 10 };
                npoi.EnumrableToExcel(voyageDetail, dt, config, columnsWidth);

                var fileName = DateTime.Now.Ticks + ".xlsx";
                FileDirectory file = new FileDirectory(fileName);
                file.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                file.CreateDataDirectory();
                //保存文件
                npoi.SaveAs(file.FilePath);

                Response.Write((new { success = true, message = "导出成功(点击确定后开始下载)", url = file.FileUrl }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导出失败：" + ex.Message }).Json());
            }
        }
        */

        /// <summary>
        /// 导出入库单
        /// </summary>
        protected void ExportEntryBill()
        {
            try
            {
                var voyageID = Request.Form["VoyageID"];
                if (!string.IsNullOrEmpty(voyageID))
                {
                    voyageID = voyageID.Trim();
                }

                var voyage = Needs.Wl.Admin.Plat.AdminPlat.Current.Voyage.ManifestSure[voyageID];

                //计算该运输批次号下总箱数
                var TotalPackNo = Convert.ToString(Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.SZOnStockView.CaleBoxNum(voyageID));
                //计算该运输批次号下总毛重
                var TotalGrossWt = Convert.ToString(Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.SZOnStockView.CalcTotalGrossWt(voyageID));
                if (!string.IsNullOrEmpty(TotalGrossWt))
                {
                    TotalGrossWt = TotalGrossWt.Trim('0').Trim('.');
                }

                int totalCount = 0;
                var listPacking = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.SZOnStockView.GetBoxInfoListModel(out totalCount, voyageID);


                IWorkbook WorkBook = ExcelFactory.Create(ExcelFactory.ExcelVersion.Excel07);
                ISheet Sheet = WorkBook.NumberOfSheets == 0 ? WorkBook.CreateSheet("Sheet1") : WorkBook.GetSheetAt(0);

                SetInfoInExcel(Sheet, 0, 0, 10, "入库单", 25, CreateHeadStyle(WorkBook));


                ICellStyle thirdInfoStyle = CreateThirdInfoStyle(WorkBook);

                int rownum = 1;
                IRow rowa = Sheet.CreateRow(rownum);
                //运输批次号
                SetInfoInExcel(Sheet, rownum, 0, 1, "运输批次号", 15, thirdInfoStyle, rowa);
                SetInfoInExcel(Sheet, rownum, 2, 4, voyage.ID, 15, thirdInfoStyle, rowa);
                //承运商
                SetInfoInExcel(Sheet, rownum, 5, 6, "承运商", 15, thirdInfoStyle, rowa);
                SetInfoInExcel(Sheet, rownum, 7, 9, voyage.Carrier?.Name, 15, thirdInfoStyle, rowa);

                rownum = 2;
                IRow rowb = Sheet.CreateRow(rownum);
                //车牌号
                SetInfoInExcel(Sheet, rownum, 0, 1, "车牌号", 15, thirdInfoStyle, rowb);
                SetInfoInExcel(Sheet, rownum, 2, 4, voyage.HKLicense, 15, thirdInfoStyle, rowb);
                //司机姓名
                SetInfoInExcel(Sheet, rownum, 5, 6, "司机姓名", 15, thirdInfoStyle, rowb);
                SetInfoInExcel(Sheet, rownum, 7, 9, voyage.DriverName, 15, thirdInfoStyle, rowb);

                rownum = 3;
                IRow rowc = Sheet.CreateRow(rownum);
                //运输时间
                SetInfoInExcel(Sheet, rownum, 0, 1, "运输时间", 15, thirdInfoStyle, rowc);
                SetInfoInExcel(Sheet, rownum, 2, 4, voyage.TransportTime?.ToString("yyyy-MM-dd") ?? string.Empty, 15, thirdInfoStyle, rowc);
                //运输类型
                SetInfoInExcel(Sheet, rownum, 5, 6, "运输类型", 15, thirdInfoStyle, rowc);
                SetInfoInExcel(Sheet, rownum, 7, 9, voyage.Type.GetDescription(), 15, thirdInfoStyle, rowc);

                rownum = 4;
                IRow rowd = Sheet.CreateRow(rownum);
                //总件数
                SetInfoInExcel(Sheet, rownum, 0, 1, "总件数", 15, thirdInfoStyle, rowd);
                SetInfoInExcel(Sheet, rownum, 2, 4, TotalPackNo, 15, thirdInfoStyle, rowd);
                //总毛重(KG)
                SetInfoInExcel(Sheet, rownum, 5, 6, "总毛重(KG)", 15, thirdInfoStyle, rowd);
                SetInfoInExcel(Sheet, rownum, 7, 9, TotalGrossWt, 15, thirdInfoStyle, rowd);

                rownum = 6;
                ICellStyle tableTitleStyle = CreateTableTitleStyle(WorkBook);
                IRow rowTableTitle = Sheet.CreateRow(rownum);
                SetInfoInExcel(Sheet, rownum, 0, 1, "订单编号", 15, tableTitleStyle, rowTableTitle);
                SetInfoInExcel(Sheet, rownum, 2, 3, "客户编号", 15, tableTitleStyle, rowTableTitle);
                SetInfoInExcel(Sheet, rownum, 4, 6, "客户名称", 15, tableTitleStyle, rowTableTitle);
                SetInfoInExcel(Sheet, rownum, 7, 8, "装箱日期", 15, tableTitleStyle, rowTableTitle);
                SetInfoInExcel(Sheet, rownum, 9, 10, "箱号", 15, tableTitleStyle, rowTableTitle);

                var properties = typeof(Needs.Ccs.Services.Views.SZOnStockView.BoxInfoListModel).GetProperties();

                int index = 1;
                foreach (var q in listPacking)
                {
                    ICellStyle tableBodyStyle = CreateTableBodyStyle(WorkBook);
                    IRow rowTableBody = Sheet.CreateRow(rownum + index);
                    SetInfoInExcel(Sheet, rownum + index, 0, 1, GetValue(q, properties, 0), 15, tableBodyStyle, rowTableBody);
                    SetInfoInExcel(Sheet, rownum + index, 2, 3, GetValue(q, properties, 3), 15, tableBodyStyle, rowTableBody);
                    SetInfoInExcel(Sheet, rownum + index, 4, 6, GetValue(q, properties, 4), 15, tableBodyStyle, rowTableBody);
                    SetInfoInExcel(Sheet, rownum + index, 7, 8, SubString(GetValue(q, properties, 5), 0 , 10), 15, tableBodyStyle, rowTableBody);
                    SetInfoInExcel(Sheet, rownum + index, 9, 10, GetValue(q, properties, 6), 15, tableBodyStyle, rowTableBody);

                    index++;
                }

                //设置列宽
                int[] columnWidth = { 12, 6, 5, 5, 15, 6, 10, 6, 6, 10, 4 };
                for (int i = 0; i < columnWidth.Length; i++)
                {
                    Sheet.SetColumnWidth(i, 256 * columnWidth[i]);
                }


                string fileName = Guid.NewGuid().ToString("N") + ".xlsx";
                FileDirectory file = new FileDirectory(fileName);
                file.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                file.CreateDataDirectory();

                //保存文件
                using (FileStream fs = new FileStream(file.FilePath, FileMode.OpenOrCreate))
                {

                    if (WorkBook != null)
                    {
                        WorkBook.Write(fs);
                        fs.Close();
                        WorkBook.Close();
                    }
                    //根据模板生成且模板路径有误
                    else
                    {
                        throw new Exception("保存失败");

                    }
                }

                Response.Write((new { success = true, message = "导出成功(点击确定后开始下载)", url = file.FileUrl }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导出失败：" + ex.Message }).Json());
            }
        }

        private string SubString(string str, int start, int len)
        {
            string result = string.Empty;

            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            if (str.Length < len)
            {
                return str;
            }

            return str.Substring(start, len);
        }

        private void SetInfoInExcel(ISheet Sheet, int rownum, int firstCol, int lastCol, string value, float heightInPoints, ICellStyle cellStyle)
        {
            IRow row = Sheet.CreateRow(rownum);
            row.HeightInPoints = heightInPoints;
            row.CreateCell(firstCol).SetCellValue(value);
            row.GetCell(firstCol).CellStyle = cellStyle;
            Sheet.AddMergedRegion(new CellRangeAddress(rownum, rownum, firstCol, lastCol));
        }

        /// <summary>
        /// 大标题样式
        /// </summary>
        /// <param name="WorkBook"></param>
        /// <returns></returns>
        private ICellStyle CreateHeadStyle(IWorkbook WorkBook)
        {
            //字体
            IFont font = WorkBook.CreateFont();
            font.FontName = "宋体";
            font.FontHeight = 18;
            font.IsBold = true;

            //单元格样式
            ICellStyle style = WorkBook.CreateCellStyle();
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.SetFont(font);

            return style;
        }

        private void SetInfoInExcel(ISheet Sheet, int rownum, int firstCol, int lastCol, string value, float heightInPoints, ICellStyle cellStyle, IRow row)
        {
            row.HeightInPoints = heightInPoints;
            row.CreateCell(firstCol).SetCellValue(value);
            row.GetCell(firstCol).CellStyle = cellStyle;
            for (int i = firstCol + 1; i <= lastCol; i++)
            {
                row.CreateCell(i).SetCellValue("");
                row.GetCell(i).CellStyle = cellStyle;
            }
            Sheet.AddMergedRegion(new CellRangeAddress(rownum, rownum, firstCol, lastCol));
        }

        /// <summary>
        /// 第三种信息样式
        /// </summary>
        /// <param name="WorkBook"></param>
        /// <returns></returns>
        private ICellStyle CreateThirdInfoStyle(IWorkbook WorkBook)
        {
            //字体
            IFont font = WorkBook.CreateFont();
            font.FontName = "宋体";
            font.FontHeight = 11;

            //单元格样式
            ICellStyle style = WorkBook.CreateCellStyle();
            style.VerticalAlignment = VerticalAlignment.Center;
            style.SetFont(font);

            return style;
        }

        /// <summary>
        /// 表格头样式
        /// </summary>
        /// <param name="WorkBook"></param>
        /// <returns></returns>
        private ICellStyle CreateTableTitleStyle(IWorkbook WorkBook)
        {
            //字体
            IFont font = WorkBook.CreateFont();
            font.FontName = "宋体";
            font.FontHeight = 11;
            font.IsBold = true;

            //单元格样式
            ICellStyle style = WorkBook.CreateCellStyle();
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.SetFont(font);

            style.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;

            return style;
        }

        /// <summary>
        /// 表格体样式
        /// </summary>
        /// <param name="WorkBook"></param>
        /// <returns></returns>
        private ICellStyle CreateTableBodyStyle(IWorkbook WorkBook)
        {
            //字体
            IFont font = WorkBook.CreateFont();
            font.FontName = "宋体";
            font.FontHeight = 11;

            //单元格样式
            ICellStyle style = WorkBook.CreateCellStyle();
            style.VerticalAlignment = VerticalAlignment.Center;
            style.SetFont(font);
            style.WrapText = true;

            style.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;

            return style;
        }

        private string GetValue<T>(T q, PropertyInfo[] properties, int j)
        {
            var value = properties[j].GetValue(q);
            if (value == null)
            {
                return string.Empty;
            }
            else
            {
                return value.ToString();
            }
        }


    }
}