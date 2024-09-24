using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace WebApp.Ccs.Utils
{
    public class DailyDeclareDataExport
    {
        public DataTable dtSource { get; set; }

        public DataTable DtSummary { get; set; }

        public ExcelConfig excelConfig { get; set; }

        public HSSFWorkbook workbook { get; set; }

        public DailyDeclareDataExport(DataTable dtSource, DataTable dtSummary, ExcelConfig excelConfig)
        {
            this.dtSource = dtSource;
            this.DtSummary = dtSummary;
            this.excelConfig = excelConfig;
            this.workbook = new HSSFWorkbook();
        }

        public DailyDeclareDataExport(DataTable dtSummary, ExcelConfig excelConfig)
        {
            this.DtSummary = dtSummary;
            this.excelConfig = excelConfig;
            this.workbook = new HSSFWorkbook();
        }

        public void Export()
        {
            GenerateDetailSheet();
            GenerateSummarySheet();

            workbook.SetSheetName(0, "完成");
            workbook.SetSheetName(1, "统计");

            string path = excelConfig.FilePath.Substring(0, excelConfig.FilePath.LastIndexOf("\\") + 1);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            using (FileStream file = new FileStream(excelConfig.FilePath, FileMode.Create))
            {
                workbook.Write(file);
                file.Close();
            }
        }

        public void GenerateDetailSheet()
        {
            ISheet sheet = workbook.CreateSheet();
            Dictionary<Color, ICellStyle[]> CellMap = new Dictionary<Color, ICellStyle[]>();
            int coloumnCount = dtSource.Columns.Count;
            if (excelConfig.RowForeColour)
            {
                coloumnCount--;
            }

            for (int i = 0; i < coloumnCount;)
            {
                bool IsExists = false;
                DataColumn column = dtSource.Columns[i];
                for (int j = 0; j < excelConfig.ColumnEntity.Count; j++)
                {
                    if (excelConfig.ColumnEntity[j].Column == column.ColumnName)
                    {
                        IsExists = true;
                        break;
                    }
                }
                if (!IsExists)
                {
                    dtSource.Columns.Remove(column);
                    coloumnCount--;
                }
                else
                {
                    i++;
                }
            }

            #region 右击文件 属性信息
            {
                DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
                dsi.Company = "NPOI";
                workbook.DocumentSummaryInformation = dsi;

                SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
                si.Author = "远大创新"; //填加xls文件作者信息
                si.ApplicationName = "远大创新"; //填加xls文件创建程序信息
                si.LastAuthor = "远大创新"; //填加xls文件最后保存者信息
                si.Comments = "远大创新"; //填加xls文件作者信息
                si.Title = "标题信息"; //填加xls文件标题信息
                si.Subject = "主题信息";//填加文件主题信息
                si.CreateDateTime = System.DateTime.Now;
                workbook.SummaryInformation = si;
            }
            #endregion

            #region 设置标题样式
            ICellStyle headStyle = workbook.CreateCellStyle();
            int[] arrColWidth = new int[dtSource.Columns.Count];
            string[] arrColName = new string[dtSource.Columns.Count];//列名
            ICellStyle[] arryColumStyle = new ICellStyle[dtSource.Columns.Count];//样式表
            headStyle.Alignment = HorizontalAlignment.Center; // ------------------
            if (excelConfig.Background != new Color())
            {
                if (excelConfig.Background != new Color())
                {
                    headStyle.FillPattern = FillPattern.SolidForeground;
                    headStyle.FillForegroundColor = GetXLColour(workbook, excelConfig.Background);
                }
            }
            IFont font = workbook.CreateFont();
            font.FontHeightInPoints = excelConfig.TitlePoint;
            if (excelConfig.ForeColor != new Color())
            {
                font.Color = GetXLColour(workbook, excelConfig.ForeColor);
            }
            font.Boldweight = 700;
            headStyle.SetFont(font);
            #endregion

            #region 列头及样式
            ICellStyle cHeadStyle = workbook.CreateCellStyle();
            cHeadStyle.Alignment = HorizontalAlignment.Center; // ------------------
            IFont cfont = workbook.CreateFont();
            cfont.FontHeightInPoints = excelConfig.HeadPoint;
            cHeadStyle.SetFont(cfont);
            #endregion

            #region 设置内容单元格样式
            for (int i = 0; i < coloumnCount; i++)
            {
                DataColumn item = dtSource.Columns[i];
                ICellStyle columnStyle = workbook.CreateCellStyle();
                columnStyle.Alignment = HorizontalAlignment.Center;
                arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName.ToString()).Length;
                arrColName[item.Ordinal] = item.ColumnName.ToString();
                if (excelConfig.ColumnEntity != null)
                {
                    ColumnEntity columnentity = excelConfig.ColumnEntity.Find(t => t.Column == item.ColumnName);
                    if (columnentity != null)
                    {
                        arrColName[item.Ordinal] = columnentity.ExcelColumn;
                        if (columnentity.Width != 0)
                        {
                            arrColWidth[item.Ordinal] = columnentity.Width;
                        }
                        if (columnentity.Background != new Color())
                        {
                            if (columnentity.Background != new Color())
                            {
                                columnStyle.FillPattern = FillPattern.SolidForeground;
                                columnStyle.FillForegroundColor = GetXLColour(workbook, columnentity.Background);
                            }
                        }
                        if (columnentity.Font != null || columnentity.Point != 0 || columnentity.ForeColor != new Color())
                        {
                            IFont columnFont = workbook.CreateFont();
                            columnFont.FontHeightInPoints = 10;
                            if (columnentity.Font != null)
                            {
                                columnFont.FontName = columnentity.Font;
                            }
                            if (columnentity.Point != 0)
                            {
                                columnFont.FontHeightInPoints = columnentity.Point;
                            }
                            if (columnentity.ForeColor != new Color())
                            {
                                columnFont.Color = GetXLColour(workbook, columnentity.ForeColor);
                            }
                            columnStyle.SetFont(font);
                        }
                        columnStyle.Alignment = getAlignment(columnentity.Alignment);
                    }
                }
                arryColumStyle[item.Ordinal] = columnStyle;
            }
            if (excelConfig.IsAllSizeColumn)
            {
                #region 根据列中最长列的长度取得列宽
                for (int i = 0; i < dtSource.Rows.Count; i++)
                {
                    for (int j = 0; j < dtSource.Columns.Count; j++)
                    {
                        if (arrColWidth[j] != 0)
                        {
                            int intTemp = Encoding.GetEncoding(936).GetBytes(dtSource.Rows[i][j].ToString()).Length;
                            if (intTemp > arrColWidth[j])
                            {
                                arrColWidth[j] = intTemp;
                            }
                        }

                    }
                }
                #endregion
            }
            #endregion

            #region 设置背景色
            if (excelConfig.RowForeColour)
            {
                DataView dView = dtSource.DefaultView;
                DataTable dtColour = dView.ToTable(true, "ForeColour");
                for (int icol = 0; icol < dtColour.Rows.Count; icol++)
                {
                    Color forColour = (Color)dtColour.Rows[icol]["ForeColour"];
                    ICellStyle[] cellStyleForeColours = new ICellStyle[arryColumStyle.Count()];
                    for (int i = 0; i < arryColumStyle.Count(); i++)
                    {
                        cellStyleForeColours[i] = workbook.CreateCellStyle();
                        if (arryColumStyle[i] != null)
                        {
                            cellStyleForeColours[i].CloneStyleFrom(arryColumStyle[i]);
                            cellStyleForeColours[i].FillForegroundColor = GetXLColour(workbook, forColour);
                            cellStyleForeColours[i].FillPattern = FillPattern.SolidForeground;
                            cellStyleForeColours[i].BorderRight = BorderStyle.Thin;
                            cellStyleForeColours[i].BorderBottom = BorderStyle.Thin;
                        }
                    }
                    CellMap.Add(forColour, cellStyleForeColours);
                }
            }
            #endregion

            #region 填充数据            
            ICellStyle dateStyle = workbook.CreateCellStyle();
            IDataFormat format = workbook.CreateDataFormat();
            dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");
            int rowIndex = 0;

            foreach (DataRow row in dtSource.Rows)
            {
                #region 新建表，填充表头，填充列头，样式
                if (rowIndex == 65535 || rowIndex == 0)
                {
                    if (rowIndex != 0)
                    {
                        sheet = workbook.CreateSheet();
                    }

                    #region 表头及样式
                    {
                        if (excelConfig.Title != null)
                        {
                            //IRow headerRow = sheet.CreateRow(0);
                            //if (excelConfig.TitleHeight != 0)
                            //{
                            //    headerRow.Height = (short)(excelConfig.TitleHeight * 20);
                            //}
                            //headerRow.HeightInPoints = 25;
                            //headerRow.CreateCell(0).SetCellValue(excelConfig.Title);
                            //headerRow.GetCell(0).CellStyle = headStyle;
                            //sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, dtSource.Columns.Count - 1)); // ------------------
                        }

                    }
                    #endregion

                    #region 列头及样式
                    {
                        IRow headerRow = sheet.CreateRow(0);
                        #region 如果设置了列标题就按列标题定义列头，没定义直接按字段名输出
                        foreach (DataColumn column in dtSource.Columns)
                        {
                            headerRow.CreateCell(column.Ordinal).SetCellValue(arrColName[column.Ordinal]);
                            headerRow.GetCell(column.Ordinal).CellStyle = cHeadStyle;
                            //设置列宽
                            // sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);
                            int colWidth = (arrColWidth[column.Ordinal] + 1) * 256;
                            if (colWidth < 255 * 256)
                            {
                                sheet.SetColumnWidth(column.Ordinal, colWidth < 3000 ? 3000 : colWidth);
                            }
                            else
                            {
                                sheet.SetColumnWidth(column.Ordinal, 6000);
                            }
                        }
                        #endregion
                    }
                    #endregion

                    rowIndex = 1;
                }
                #endregion

                #region 填充内容
                IRow dataRow = sheet.CreateRow(rowIndex);

                for (int i = 0; i < coloumnCount; i++)
                {
                    DataColumn column = dtSource.Columns[i];
                    ICell newCell = dataRow.CreateCell(column.Ordinal);
                    if (column.Ordinal > 255)
                    {
                        arrColWidth[column.Ordinal] = 254;
                    }
                    if (excelConfig.RowForeColour)
                    {
                        Color currentColour = (Color)row["ForeColour"];
                        ICellStyle[] styles = CellMap[currentColour];
                        newCell.CellStyle = styles[column.Ordinal];
                    }
                    else
                    {
                        newCell.CellStyle = arryColumStyle[column.Ordinal];
                    }
                    string drValue = row[column].ToString();
                    SetCell(newCell, dateStyle, column.DataType, drValue);
                }
                #endregion
                rowIndex++;
            }
            #endregion          
        }

        public void GenerateSummarySheet()
        {
            ISheet sheet = workbook.CreateSheet();

            #region 表头样式
            IFont SummaryHeadFont = workbook.CreateFont();
            SummaryHeadFont.FontHeightInPoints = 11;
            SummaryHeadFont.IsBold = true;
                    
            ICellStyle SummaryHeadStyleGrey = workbook.CreateCellStyle();
            SummaryHeadStyleGrey.Alignment = HorizontalAlignment.Center;
            SummaryHeadStyleGrey.VerticalAlignment = VerticalAlignment.Center;
            SummaryHeadStyleGrey.SetFont(SummaryHeadFont);
            Color GreyColour = Color.FromArgb(172,185,202);
            SummaryHeadStyleGrey.FillForegroundColor = GetXLColour(workbook, GreyColour);
            SummaryHeadStyleGrey.FillPattern = FillPattern.SolidForeground;
            

            ICellStyle SummaryHeadStyleRed = workbook.CreateCellStyle();
            SummaryHeadStyleRed.Alignment = HorizontalAlignment.Center;
            SummaryHeadStyleRed.VerticalAlignment = VerticalAlignment.Center;
            SummaryHeadStyleRed.SetFont(SummaryHeadFont);
            Color RedColour = Color.FromArgb(255, 0, 0);
            SummaryHeadStyleRed.FillForegroundColor = GetXLColour(workbook, RedColour);
            SummaryHeadStyleRed.FillPattern = FillPattern.SolidForeground;

            ICellStyle SummaryHeadStyleTransparent = workbook.CreateCellStyle();
            SummaryHeadStyleTransparent.Alignment = HorizontalAlignment.Center;
            SummaryHeadStyleTransparent.VerticalAlignment = VerticalAlignment.Center;
            SummaryHeadStyleTransparent.SetFont(SummaryHeadFont);

            #endregion

            #region 表体样式
            IFont SummaryBodyFont = workbook.CreateFont();
            SummaryBodyFont.FontHeightInPoints = 11;

            ICellStyle SummaryBodyStyleTransparent = workbook.CreateCellStyle();
            SummaryBodyStyleTransparent.Alignment = HorizontalAlignment.Center;
            SummaryBodyStyleTransparent.SetFont(SummaryBodyFont);
            SummaryBodyStyleTransparent.BorderRight = BorderStyle.Thin;
            SummaryBodyStyleTransparent.BorderBottom = BorderStyle.Thin;

            ICellStyle SummaryBodyStyleRed = workbook.CreateCellStyle();
            SummaryBodyStyleRed.Alignment = HorizontalAlignment.Center;
            SummaryBodyStyleRed.SetFont(SummaryBodyFont);
            SummaryBodyStyleRed.FillForegroundColor = GetXLColour(workbook, RedColour);
            SummaryBodyStyleRed.FillPattern = FillPattern.SolidForeground;
            SummaryBodyStyleRed.BorderRight = BorderStyle.Thin;
            SummaryBodyStyleRed.BorderBottom = BorderStyle.Thin;

            #endregion

            #region 设置宽度
            sheet.SetColumnWidth(0, 6*256);
            sheet.SetColumnWidth(1, 20*256);
            sheet.SetColumnWidth(2, 11 * 256);
            sheet.SetColumnWidth(3, 16 * 256);
            sheet.SetColumnWidth(4, 7 * 256);
            sheet.SetColumnWidth(5, 28 * 256);
            sheet.SetColumnWidth(6, 12 * 256);
            sheet.SetColumnWidth(7, 9 * 256);
            sheet.SetColumnWidth(7, 9 * 256);
            #endregion

            int rowIndex = 0;
            #region 表头
            IRow headerRow = sheet.CreateRow(0);
            headerRow.Height = 30*20;

            headerRow.CreateCell(0).SetCellValue("项号");
            headerRow.GetCell(0).CellStyle = SummaryHeadStyleGrey;            
            headerRow.CreateCell(1).SetCellValue("商品名称");
            headerRow.GetCell(1).CellStyle = SummaryHeadStyleGrey;
            headerRow.CreateCell(2).SetCellValue("成交数量");
            headerRow.GetCell(2).CellStyle = SummaryHeadStyleGrey;
            headerRow.CreateCell(3).SetCellValue("总价");
            headerRow.GetCell(3).CellStyle = SummaryHeadStyleGrey;
            headerRow.CreateCell(4).SetCellValue("币制");
            headerRow.GetCell(4).CellStyle = SummaryHeadStyleGrey;
            headerRow.CreateCell(5).SetCellValue("开票公司");
            headerRow.GetCell(5).CellStyle = SummaryHeadStyleGrey;
            headerRow.CreateCell(6).SetCellValue("件数");
            headerRow.GetCell(6).CellStyle = SummaryHeadStyleRed;
            headerRow.CreateCell(7).SetCellValue("净重");
            headerRow.GetCell(7).CellStyle = SummaryHeadStyleTransparent;
            headerRow.CreateCell(8).SetCellValue("毛重");
            headerRow.GetCell(8).CellStyle = SummaryHeadStyleRed;

            rowIndex++;
            #endregion


            #region 表体           
            //string Currency = dtSource.Rows[0]["TradeCurr"].ToString();
            string currency = "";
            foreach (DataRow row in DtSummary.Rows)
            {
                DataRow dr = dtSource.Select(" ContrNo = '" + row["ContrNo"] + "'").FirstOrDefault();
                currency = dr["TradeCurr"].ToString();
                IRow dataRow = sheet.CreateRow(rowIndex);
                dataRow.CreateCell(0).SetCellValue(rowIndex.ToString());               
                dataRow.CreateCell(1).SetCellValue(row["ContrNo"].ToString());               
                dataRow.CreateCell(2).SetCellValue(Math.Round(Convert.ToDouble(row["GQty"]),0,MidpointRounding.AwayFromZero));               
                dataRow.CreateCell(3).SetCellValue(Math.Round(Convert.ToDouble(row["DeclTotal"]),2,MidpointRounding.AwayFromZero));                
                dataRow.CreateCell(4).SetCellValue(currency);               
                dataRow.CreateCell(5).SetCellValue(row["OwnerName"].ToString());                
                dataRow.CreateCell(6).SetCellValue(Math.Round(Convert.ToDouble(row["PackNo"]),0,MidpointRounding.AwayFromZero));                
                dataRow.CreateCell(7).SetCellValue(Math.Round(Convert.ToDouble(row["NetWt"]),2,MidpointRounding.AwayFromZero));                
                dataRow.CreateCell(8).SetCellValue(Math.Round(Convert.ToDouble(row["GrossWt"]),2,MidpointRounding.AwayFromZero));               

                if (!row["ContrNo"].ToString().ToLower().Contains("sj"))
                {
                    dataRow.GetCell(0).CellStyle = SummaryBodyStyleTransparent;
                    dataRow.GetCell(1).CellStyle = SummaryBodyStyleTransparent;
                    dataRow.GetCell(2).CellStyle = SummaryBodyStyleTransparent;
                    dataRow.GetCell(3).CellStyle = SummaryBodyStyleTransparent;
                    dataRow.GetCell(4).CellStyle = SummaryBodyStyleTransparent;
                    dataRow.GetCell(5).CellStyle = SummaryBodyStyleTransparent;
                    dataRow.GetCell(6).CellStyle = SummaryBodyStyleTransparent;
                    dataRow.GetCell(7).CellStyle = SummaryBodyStyleTransparent;
                    dataRow.GetCell(8).CellStyle = SummaryBodyStyleTransparent;
                }
                else
                {
                    dataRow.GetCell(0).CellStyle = SummaryBodyStyleRed;
                    dataRow.GetCell(1).CellStyle = SummaryBodyStyleRed;
                    dataRow.GetCell(2).CellStyle = SummaryBodyStyleRed;
                    dataRow.GetCell(3).CellStyle = SummaryBodyStyleRed;
                    dataRow.GetCell(4).CellStyle = SummaryBodyStyleRed;
                    dataRow.GetCell(5).CellStyle = SummaryBodyStyleRed;
                    dataRow.GetCell(6).CellStyle = SummaryBodyStyleRed;
                    dataRow.GetCell(7).CellStyle = SummaryBodyStyleRed;
                    dataRow.GetCell(8).CellStyle = SummaryBodyStyleRed;
                }

                rowIndex++;
            }
            #endregion

        }

        public void ExportList()
        {
            GenerateListSheet();
            workbook.SetSheetName(0, "报关清单");
            string path = excelConfig.FilePath.Substring(0, excelConfig.FilePath.LastIndexOf("\\") + 1);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            using (FileStream file = new FileStream(excelConfig.FilePath, FileMode.Create))
            {
                workbook.Write(file);
                file.Close();
            }
        }

        public void GenerateListSheet()
        {
            ISheet sheet = workbook.CreateSheet();

            #region 表头样式
            IFont SummaryHeadFont = workbook.CreateFont();
            SummaryHeadFont.FontHeightInPoints = 11;
            SummaryHeadFont.IsBold = true;

            ICellStyle SummaryHeadStyleGrey = workbook.CreateCellStyle();
            SummaryHeadStyleGrey.Alignment = HorizontalAlignment.Center;
            SummaryHeadStyleGrey.VerticalAlignment = VerticalAlignment.Center;
            SummaryHeadStyleGrey.SetFont(SummaryHeadFont);
            Color GreyColour = Color.FromArgb(172, 185, 202);
            SummaryHeadStyleGrey.FillForegroundColor = GetXLColour(workbook, GreyColour);
            SummaryHeadStyleGrey.FillPattern = FillPattern.SolidForeground;


            ICellStyle SummaryHeadStyleRed = workbook.CreateCellStyle();
            SummaryHeadStyleRed.Alignment = HorizontalAlignment.Center;
            SummaryHeadStyleRed.VerticalAlignment = VerticalAlignment.Center;
            SummaryHeadStyleRed.SetFont(SummaryHeadFont);
            Color RedColour = Color.FromArgb(255, 0, 0);
            SummaryHeadStyleRed.FillForegroundColor = GetXLColour(workbook, RedColour);
            SummaryHeadStyleRed.FillPattern = FillPattern.SolidForeground;

            ICellStyle SummaryHeadStyleTransparent = workbook.CreateCellStyle();
            SummaryHeadStyleTransparent.Alignment = HorizontalAlignment.Center;
            SummaryHeadStyleTransparent.VerticalAlignment = VerticalAlignment.Center;
            SummaryHeadStyleTransparent.SetFont(SummaryHeadFont);

            #endregion

            #region 表体样式
            IFont SummaryBodyFont = workbook.CreateFont();
            SummaryBodyFont.FontHeightInPoints = 11;

            ICellStyle SummaryBodyStyleTransparent = workbook.CreateCellStyle();
            SummaryBodyStyleTransparent.Alignment = HorizontalAlignment.Center;
            SummaryBodyStyleTransparent.SetFont(SummaryBodyFont);
            SummaryBodyStyleTransparent.BorderRight = BorderStyle.Thin;
            SummaryBodyStyleTransparent.BorderBottom = BorderStyle.Thin;

            ICellStyle SummaryBodyStyleRed = workbook.CreateCellStyle();
            SummaryBodyStyleRed.Alignment = HorizontalAlignment.Center;
            SummaryBodyStyleRed.SetFont(SummaryBodyFont);
            SummaryBodyStyleRed.FillForegroundColor = GetXLColour(workbook, RedColour);
            SummaryBodyStyleRed.FillPattern = FillPattern.SolidForeground;
            SummaryBodyStyleRed.BorderRight = BorderStyle.Thin;
            SummaryBodyStyleRed.BorderBottom = BorderStyle.Thin;

            #endregion

            #region 设置宽度
            sheet.SetColumnWidth(0, 6 * 256);
            sheet.SetColumnWidth(1, 20 * 256);
            sheet.SetColumnWidth(2, 40 * 256);
            sheet.SetColumnWidth(3, 25 * 256);
            sheet.SetColumnWidth(4, 25 * 256);
            sheet.SetColumnWidth(5, 25 * 256);         
            #endregion

            int rowIndex = 0;
            #region 表头
            IRow headerRow = sheet.CreateRow(0);
            headerRow.Height = 30 * 20;

            headerRow.CreateCell(0).SetCellValue("序号");
            headerRow.GetCell(0).CellStyle = SummaryHeadStyleGrey;
            headerRow.CreateCell(1).SetCellValue("进口日期");
            headerRow.GetCell(1).CellStyle = SummaryHeadStyleGrey;
            headerRow.CreateCell(2).SetCellValue("境内收发货人");
            headerRow.GetCell(2).CellStyle = SummaryHeadStyleGrey;
            headerRow.CreateCell(3).SetCellValue("合同号");
            headerRow.GetCell(3).CellStyle = SummaryHeadStyleGrey;
            headerRow.CreateCell(4).SetCellValue("报关单号");
            headerRow.GetCell(4).CellStyle = SummaryHeadStyleGrey;
            headerRow.CreateCell(5).SetCellValue("备注");
            headerRow.GetCell(5).CellStyle = SummaryHeadStyleGrey;

            rowIndex++;
            #endregion


            #region 表体                    
            foreach (DataRow row in DtSummary.Rows)
            {
                IRow dataRow = sheet.CreateRow(rowIndex);
                dataRow.CreateCell(0).SetCellValue(rowIndex.ToString());
                dataRow.CreateCell(1).SetCellValue(row["IEDate"].ToString());
                dataRow.CreateCell(2).SetCellValue(row["ConsigneeName"].ToString());
                dataRow.CreateCell(3).SetCellValue(row["ContrNo"].ToString());
                dataRow.CreateCell(4).SetCellValue(row["EntryID"].ToString());
                dataRow.CreateCell(5).SetCellValue(row["Summary"].ToString());            

             
                dataRow.GetCell(0).CellStyle = SummaryBodyStyleTransparent;
                dataRow.GetCell(1).CellStyle = SummaryBodyStyleTransparent;
                dataRow.GetCell(2).CellStyle = SummaryBodyStyleTransparent;
                dataRow.GetCell(3).CellStyle = SummaryBodyStyleTransparent;
                dataRow.GetCell(4).CellStyle = SummaryBodyStyleTransparent;
                dataRow.GetCell(5).CellStyle = SummaryBodyStyleTransparent;              
               

                rowIndex++;
            }
            #endregion

        }


        private static short GetXLColour(HSSFWorkbook workbook, Color SystemColour)
        {
            short s = 0;
            HSSFPalette XlPalette = workbook.GetCustomPalette();
            NPOI.HSSF.Util.HSSFColor XlColour = XlPalette.FindColor(SystemColour.R, SystemColour.G, SystemColour.B);
            if (XlColour == null)
            {
                if (NPOI.HSSF.Record.PaletteRecord.STANDARD_PALETTE_SIZE < 255)
                {
                    XlColour = XlPalette.FindSimilarColor(SystemColour.R, SystemColour.G, SystemColour.B);
                    s = XlColour.Indexed;
                }
                //if (NPOI.HSSF.Record.PaletteRecord.STANDARD_PALETTE_SIZE > 255)
                //{

                //}
            }
            else
                s = XlColour.Indexed;
            return s;
        }

        private static void SetCell(ICell newCell, ICellStyle dateStyle, Type dataType, string drValue)
        {
            switch (dataType.ToString())
            {
                case "System.String"://字符串类型
                    newCell.SetCellValue(drValue);
                    break;
                case "System.DateTime"://日期类型
                    System.DateTime dateV;
                    if (System.DateTime.TryParse(drValue, out dateV))
                    {
                        newCell.SetCellValue(dateV);
                    }
                    else
                    {
                        newCell.SetCellValue("");
                    }
                    newCell.CellStyle = dateStyle;//格式化显示
                    break;
                case "System.Boolean"://布尔型
                    bool boolV = false;
                    bool.TryParse(drValue, out boolV);
                    newCell.SetCellValue(boolV);
                    break;
                case "System.Int16"://整型
                case "System.Int32":
                case "System.Int64":
                case "System.Byte":
                    int intV = 0;
                    int.TryParse(drValue, out intV);
                    newCell.SetCellValue(intV);
                    break;
                case "System.Decimal"://浮点型
                case "System.Double":
                    double doubV = 0;
                    double.TryParse(drValue, out doubV);
                    newCell.SetCellValue(doubV);
                    break;
                case "System.DBNull"://空值处理
                    newCell.SetCellValue("");
                    break;
                default:
                    newCell.SetCellValue("");
                    break;
            }
        }

        private static HorizontalAlignment getAlignment(string style)
        {
            switch (style)
            {
                case "center":
                    return HorizontalAlignment.Center;
                case "left":
                    return HorizontalAlignment.Left;
                case "right":
                    return HorizontalAlignment.Right;
                case "fill":
                    return HorizontalAlignment.Fill;
                case "justify":
                    return HorizontalAlignment.Justify;
                case "centerselection":
                    return HorizontalAlignment.CenterSelection;
                case "distributed":
                    return HorizontalAlignment.Distributed;
            }
            return NPOI.SS.UserModel.HorizontalAlignment.General;


        }
    }
}