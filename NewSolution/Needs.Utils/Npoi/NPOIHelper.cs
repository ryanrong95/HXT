using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using Spire.Pdf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Needs.Utils.Npoi
{
    public class NPOIHelper
    {
        private IWorkbook WorkBook;
        private ISheet Sheet;

        private string TempletePath;

        private NPOIHelper()
        {
        }

        #region 实例化workbook

        /// <summary>
        /// 外部实例化
        /// </summary>
        /// <param name="workbook"></param>
        public NPOIHelper(IWorkbook workbook)
        {
            this.WorkBook = workbook;
            this.Sheet = workbook.NumberOfSheets == 0 ? workbook.CreateSheet("Sheet1") : workbook.GetSheetAt(0);
        }

        /// <summary>
        /// 设置当前的Sheet
        /// </summary>
        /// <param name="sheetName"></param>
        public void SetSheet(string sheetName)
        {
            this.Sheet = this.WorkBook.GetSheet(sheetName);
        }

        /// <summary>
        /// 设置单元格的内容
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="content"></param>
        public void SetCellValue(int row, int column, string content)
        {
            this.Sheet.GetRow(row).GetCell(column).SetCellValue(content);
        }

        /// <summary>
        /// 多个sheet实例化
        /// </summary>
        /// <param name="workBook"></param>
        /// <param name="sheetCount">导出sheet的数量</param>
        public NPOIHelper(IWorkbook workBook, int sheetCount)
        {
            this.WorkBook = workBook;
            //多个sheet命名规则：Sheet1,Sheet2.....
            if (sheetCount > 0)
            {
                for (int i = 0; i < sheetCount; i++)
                {
                    workBook.CreateSheet($"Sheet{i + 1}");
                }
            }

            this.Sheet = workBook.GetSheetAt(0);
        }
        #endregion

        #region 实例化Word对象

        public NPOIHelper(string templetepath)
        {
            this.TempletePath = templetepath;
        }

        #endregion

        #region 导入Excel
        /// <summary>
        /// 导入Excel
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="isColumnName"></param>
        /// <returns></returns>
        public DataTable ExcelToDataTable(bool isColumnName)
        {
            DataTable dataTable = null;
            DataColumn column = null;
            DataRow dataRow = null;
            IRow row = null;
            ICell cell = null;
            int startRow = 0;

            if (WorkBook != null)
            {
                Sheet = WorkBook.GetSheetAt(0);//读取第一个sheet，当然也可以循环读取每个sheet
                dataTable = new DataTable();
                if (Sheet != null)
                {
                    int rowCount = Sheet.LastRowNum;//总行数
                    if (rowCount > 0)
                    {
                        IRow firstRow = Sheet.GetRow(0);//第一行
                        int cellCount = firstRow.LastCellNum;//列数

                        //构建datatable的列
                        if (isColumnName)
                        {
                            startRow = 1;//如果第一行是列名，则从第二行开始读取
                            for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                            {
                                cell = firstRow.GetCell(i);
                                if (cell != null)
                                {
                                    if (cell.StringCellValue != null)
                                    {
                                        if (dataTable.Columns.Contains(cell.StringCellValue))
                                            column = new DataColumn(cell.StringCellValue + "A");
                                        else
                                            column = new DataColumn(cell.StringCellValue);
                                        dataTable.Columns.Add(column);
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                            {
                                column = new DataColumn("column" + (i + 1));
                                dataTable.Columns.Add(column);
                            }
                        }

                        //填充行
                        for (int i = startRow; i <= rowCount; ++i)
                        {
                            row = Sheet.GetRow(i);
                            if (row == null) continue;
                            if (row.Cells.Count == 0) continue;

                            dataRow = dataTable.NewRow();
                            for (int j = row.FirstCellNum; j < cellCount; ++j)
                            {
                                if (j == -1)
                                {
                                    continue;
                                }

                                cell = row.GetCell(j);
                                if (cell == null)
                                {
                                    dataRow[j] = "";
                                }
                                else
                                {
                                    //CellType(Unknown = -1,Numeric = 0,String = 1,Formula = 2,Blank = 3,Boolean = 4,Error = 5,)
                                    switch (cell.CellType)
                                    {
                                        case CellType.Boolean:
                                            dataRow[j] = cell.BooleanCellValue;
                                            break;
                                        case CellType.Blank:
                                            dataRow[j] = "";
                                            break;
                                        case CellType.Numeric:
                                            short format = cell.CellStyle.DataFormat;
                                            //对时间格式（2015.12.5、2015/12/5、2015-12-5等）的处理
                                            if (format == 14 || format == 31 || format == 57 || format == 58)
                                                dataRow[j] = cell.DateCellValue;
                                            else
                                            {
                                                //科学记数法
                                                if (cell.NumericCellValue.ToString().Contains("E"))
                                                {
                                                    dataRow[j] = decimal.Parse(cell.NumericCellValue.ToString(), System.Globalization.NumberStyles.Float).ToString();
                                                }
                                                else
                                                {
                                                    dataRow[j] = cell.NumericCellValue;
                                                }
                                            }

                                            break;
                                        case CellType.String:
                                            dataRow[j] = cell.StringCellValue;
                                            break;
                                        case CellType.Formula:
                                            switch (cell.CachedFormulaResultType)
                                            {
                                                case CellType.Boolean:
                                                    dataRow[j] = cell.BooleanCellValue;
                                                    break;
                                                case CellType.Error:
                                                    dataRow[j] = NPOI.SS.Formula.Eval.ErrorEval.GetText(cell.ErrorCellValue);
                                                    break;
                                                case CellType.Numeric:
                                                    if (DateUtil.IsCellDateFormatted(cell))
                                                    {
                                                        dataRow[j] = cell.DateCellValue.ToString("yyyy-MM-dd hh:MM:ss");
                                                    }
                                                    else
                                                    {
                                                        dataRow[j] = cell.NumericCellValue;
                                                    }
                                                    break;
                                                case CellType.String:
                                                    string str = cell.StringCellValue;
                                                    if (!string.IsNullOrEmpty(str))
                                                    {
                                                        dataRow[j] = str.ToString();
                                                    }
                                                    else
                                                    {
                                                        dataRow[j] = null;
                                                    }
                                                    break;
                                                case CellType.Unknown:
                                                case CellType.Blank:
                                                default:
                                                    dataRow[j] = string.Empty;
                                                    break;
                                            }
                                            break;
                                    }
                                }
                            }
                            dataTable.Rows.Add(dataRow);
                        }
                    }
                }
            }
            return dataTable;
        }
        #endregion

        #region 导出Excel

        /// <summary>
        /// 生成基础版excel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">查询集合</param>
        /// <param name="rowStart">要写入excel的起始行号，默认从0开始</param>
        /// <param name = "columnWidth" > 自定义列宽度</param>
        /// <returns></returns>
        public void EnumrableToExcel<T>(IEnumerable<T> query, int rowStart, int[] columnWidth)
        {
            var properties = typeof(T).GetProperties();
            var rowIndex = 0;
            //模板or not
            if (WorkBook == null)
            {
                new NPOIHelper();
            }
            //设置单元格边框
            var style = GetCellBorder();

            //设置列宽
            if (columnWidth != null)
            {
                for (int i = 0; i < columnWidth.Length; i++)
                {
                    Sheet.SetColumnWidth(i, 256 * columnWidth[i]);
                }
            }

            //创建列头行   
            if (rowStart == 0)
            {
                IRow row = Sheet.CreateRow(rowIndex);
                //列名
                for (int i = 0; i < properties.Count(); i++)
                {
                    row.CreateCell(i).SetCellValue(properties[i].Name);
                    row.GetCell(i).CellStyle = style;
                }
            }
            // 模板
            if (rowStart > 0)
            {
                rowIndex = rowStart - 1;
            }

            //填入内容
            rowIndex++;
            foreach (var q in query)
            {
                IRow rows = Sheet.CreateRow(rowIndex);
                for (int j = 0; j < properties.Count(); j++)
                {
                    var value = properties[j].GetValue(q);
                    if (value == null)
                    {
                        rows.CreateCell(j).SetCellValue("");
                    }
                    else
                    {
                        rows.CreateCell(j).SetCellValue(value.ToString());
                    }

                    rows.GetCell(j).CellStyle = style;
                    // Sheet.AutoSizeColumn(j);
                }
                rowIndex++;
            }
        }

        /// <summary>
        /// 生成基础版excel(多个sheet)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="rowStart"></param>
        /// <param name="columnWidth"></param>
        /// <param name="sheetIndex"></param>
        public void EnumrableToExcel<T>(IEnumerable<T> query, int rowStart, int[] columnWidth, int sheetIndex)
        {
            if (WorkBook.GetSheetAt(sheetIndex) == null)
            {
                return;
            }
            Sheet = WorkBook.GetSheetAt(sheetIndex);
            EnumrableToExcel(query, rowStart, columnWidth);
        }

        /// <summary>
        /// excel顶部附加信息/底部汇总信息导出
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">集合</param>
        /// <param name="rowStart">excel起始行，默认从0开始</param>
        /// <param name="columnWidth">列宽数组</param>
        /// <param name="attchHeadTable">顶部附加信息(可选)DataTable</param>
        /// <param name="sumTable">底部汇总DataTable（可选）</param>
        public void EnumrableToExcel<T>(IEnumerable<T> query, int rowStart, int[] columnWidth = null, DataTable attchHeadTable = null, DataTable sumTable = null)
        {
            var properties = typeof(T).GetProperties();
            //行位置跟踪
            var rowIndex = 0;
            //模板or not
            if (WorkBook == null)
            {
                new NPOIHelper();
            }
            //设置单元格边框
            var style = GetCellBorder();

            //设置列宽
            if (columnWidth != null)
            {
                for (int i = 0; i < columnWidth.Length; i++)
                {
                    Sheet.SetColumnWidth(i, 256 * columnWidth[i]);
                }
            }

            //顶部附加信息
            if (attchHeadTable != null)
            {
                AttachHead(attchHeadTable);
                rowIndex = Sheet.PhysicalNumberOfRows;
            }

            //创建列头行/模板可不需要列头
            if (rowStart == 0)
            {
                IRow row = Sheet.CreateRow(rowIndex);
                //列名
                for (int i = 0; i < properties.Count(); i++)
                {
                    row.CreateCell(i).SetCellValue(properties[i].Name);
                    row.GetCell(i).CellStyle = style;
                }
            }

            // 模板
            if (rowStart > 0)
            {
                rowIndex = rowStart - 1;
            }

            rowIndex++;
            //填入内容
            foreach (var q in query)
            {
                IRow rows = Sheet.CreateRow(rowIndex);
                for (int j = 0; j < properties.Count(); j++)
                {
                    var value = properties[j].GetValue(q);
                    if (value == null)
                    {
                        rows.CreateCell(j).SetCellValue("");
                    }
                    else
                    {
                        rows.CreateCell(j).SetCellValue(value.ToString());
                    }

                    rows.GetCell(j).CellStyle = style;
                    //  Sheet.AutoSizeColumn(j);
                }
                rowIndex++;
            }

            // rowIndex++;
            //底部汇总数据
            if (sumTable != null)
            {
                Summary(sumTable, rowIndex);
            }
        }

        /// <summary>
        /// excel指定sheet顶部附加信息/底部汇总信息导出(多sheet表格)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">集合</param>
        /// <param name="rowStart">指定excel开始写入的行号，默认从0开始</param>
        /// <param name="sheetIndex">指定写入的sheet索引号，默认从0开始</param>
        /// <param name="columnWidth">设置列宽的数组</param>
        /// <param name="attchHeadTable">列头前的数据</param>
        /// <param name="sumTable">结尾的数据</param>
        public void EnumrableToExcel<T>(IEnumerable<T> query, int rowStart, int sheetIndex, int[] columnWidth = null, DataTable attchHeadTable = null, DataTable sumTable = null)
        {
            if (WorkBook.GetSheetAt(sheetIndex) == null)
            {
                return;
            }
            Sheet = WorkBook.GetSheetAt(sheetIndex);

            EnumrableToExcel(query, rowStart, columnWidth, attchHeadTable, sumTable);
        }

        /// <summary>
        /// 导出大标题(居中大字体)的excel/大标题+底部汇总信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="config"></param>
        /// <param name = "columnWidth" > 自定义列宽度</param>
        /// <param name = "sumTable" > 底部汇总DataTable(可选)</param>
        public void EnumrableToExcel<T>(IEnumerable<T> query, StyleConfig config, int[] columnWidth, DataTable sumTable = null)
        {
            //模板or not
            if (WorkBook == null)
            {
                new NPOIHelper();
            }

            //设置标题样式
            ICellStyle headStyle = WorkBook.CreateCellStyle();

            headStyle.Alignment = HorizontalAlignment.Center;   //居中
            if (config.Background != new Color())
            {
                headStyle.FillPattern = FillPattern.SolidForeground; //TODO:填充背景色
            }

            //设置单元格边框
            var style = GetCellBorder();

            //设置列宽
            if (columnWidth != null)
            {
                for (int i = 0; i < columnWidth.Length; i++)
                {
                    Sheet.SetColumnWidth(i, 256 * columnWidth[i]);
                }
            }

            var properties = typeof(T).GetProperties();
            var rowIndex = 0;

            //创建标题行
            if (config.Title != null)
            {
                IRow headerRow = Sheet.CreateRow(0);

                if (config.TitleHeight != 0)
                {
                    headerRow.Height = (short)(config.TitleHeight * 20);
                }

                //设置字体
                IFont font = WorkBook.CreateFont();
                font.FontHeightInPoints = config.TitlePoint;
                font.Boldweight = config.Boldweight;
                headStyle.SetFont(font);

                //大标题内容写入，合并
                headerRow.HeightInPoints = 25;
                headerRow.CreateCell(0).SetCellValue(config.Title);
                headerRow.GetCell(0).CellStyle = headStyle;
                Sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, properties.Length - 1));
            }

            rowIndex++;
            IRow row = Sheet.CreateRow(rowIndex);
            //列名
            for (int i = 0; i < properties.Count(); i++)
            {
                row.CreateCell(i).SetCellValue(properties[i].Name);
                row.GetCell(i).CellStyle = style;
            }

            //填入内容
            rowIndex++;
            foreach (var q in query)
            {
                IRow rows = Sheet.CreateRow(rowIndex);
                for (int j = 0; j < properties.Count(); j++)
                {
                    var value = properties[j].GetValue(q);
                    if (value == null)
                        rows.CreateCell(j).SetCellValue("");
                    else
                        rows.CreateCell(j).SetCellValue(value.ToString());
                    rows.GetCell(j).CellStyle = style;
                    // Sheet.AutoSizeColumn(j);
                }

                rowIndex++;
            }

            rowIndex++;
            //底部汇总数据
            if (sumTable != null)
            {
                Summary(sumTable, rowIndex);
            }
        }

        /// <summary>
        /// 导出大标题(居中大字体)的excel/大标题+底部汇总信息(多个sheet)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="config"></param>
        /// <param name="columnWidth"></param>
        /// <param name="sheetIndex"></param>
        /// <param name="sumTable"></param>
        public void EnumrableToExcel<T>(IEnumerable<T> query, StyleConfig config, int[] columnWidth, int sheetIndex, DataTable sumTable = null)
        {
            if (WorkBook.GetSheetAt(sheetIndex) == null)
            {
                return;
            }
            Sheet = WorkBook.GetSheetAt(sheetIndex);
            EnumrableToExcel(query, config, columnWidth, sumTable);
        }




        /// <summary>
        /// 新添加的20190612
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="dtHead"></param>
        /// <param name="config"></param>
        /// <param name="columnWidth"></param>

        public void EnumrableToExcel<T>(IEnumerable<T> query, DataTable attchHeadTable, StyleConfig config, int[] columnWidth)
        {
            //模板or not
            if (WorkBook == null)
            {
                new NPOIHelper();
            }

            //设置标题样式
            ICellStyle headStyle = WorkBook.CreateCellStyle();

            headStyle.Alignment = HorizontalAlignment.Center;   //居中
            if (config.Background != new Color())
            {
                headStyle.FillPattern = FillPattern.SolidForeground; //TODO:填充背景色
            }

            //设置单元格边框
            var style = GetCellBorder();

            //设置列宽
            if (columnWidth != null)
            {
                for (int i = 0; i < columnWidth.Length; i++)
                {
                    Sheet.SetColumnWidth(i, 256 * columnWidth[i]);
                }
            }

            var properties = typeof(T).GetProperties();
            var rowIndex = 0;

            //创建标题行
            if (config.Title != null)
            {
                IRow headerRow = Sheet.CreateRow(0);

                if (config.TitleHeight != 0)
                {
                    headerRow.Height = (short)(config.TitleHeight * 20);
                }

                //设置字体
                IFont font = WorkBook.CreateFont();
                font.FontHeightInPoints = config.TitlePoint;
                font.Boldweight = config.Boldweight;
                headStyle.SetFont(font);

                //大标题内容写入，合并
                headerRow.HeightInPoints = 25;
                headerRow.CreateCell(0).SetCellValue(config.Title);
                headerRow.GetCell(0).CellStyle = headStyle;
                Sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, properties.Length - 1));
            }
            rowIndex++;
            if (attchHeadTable != null)
            {
                Summary(attchHeadTable, rowIndex);
            }

            rowIndex = rowIndex + 6;
            IRow row = Sheet.CreateRow(rowIndex);
            //列名
            for (int i = 0; i < properties.Count(); i++)
            {
                row.CreateCell(i).SetCellValue(properties[i].Name);
                row.GetCell(i).CellStyle = style;
            }

            //填入内容
            rowIndex++;
            foreach (var q in query)
            {
                IRow rows = Sheet.CreateRow(rowIndex);
                for (int j = 0; j < properties.Count(); j++)
                {
                    var value = properties[j].GetValue(q);
                    if (value == null)
                        rows.CreateCell(j).SetCellValue("");
                    else
                        rows.CreateCell(j).SetCellValue(value.ToString());
                    rows.GetCell(j).CellStyle = style;
                    // Sheet.AutoSizeColumn(j);
                }

                rowIndex++;
            }



        }



        #endregion

        #region 根据模板生成excel

        /// <summary>
        /// 模板
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">查询的linq集合</param>
        /// <param name="rowStart">插入数据开始的行号，默认从0开始</param>
        /// <param name="sumTable">底部汇总dataTable(可选)</param>
        public void GenerateExcelByTemplate<T>(IEnumerable<T> query, int rowStart, DataTable sumTable = null)
        {
            if (WorkBook != null && rowStart > 0)
            {
                EnumrableToExcel(query, rowStart);
                //写入汇总数据
                if (sumTable != null)
                {
                    Summary(sumTable, rowStart + query.Count());
                }
            }
        }

        /// <summary>
        /// 模板导出（多个sheet）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="rowStart"></param>
        /// <param name="sheetIndex"></param>
        /// <param name="sumTable"></param>
        public void GenerateExcelByTemplate<T>(IEnumerable<T> query, int rowStart, int sheetIndex, DataTable sumTable = null)
        {
            if (WorkBook.GetSheetAt(sheetIndex) == null)
            {
                return;
            }
            Sheet = WorkBook.GetSheetAt(sheetIndex);
            GenerateExcelByTemplate(query, rowStart);
        }
        #endregion

        #region excel顶部附加信息

        /// <summary>
        /// 传顶部附加信息(DataTable格式)
        /// </summary>
        /// <param name="attachTable"></param>
        /// <param name="align">默认左侧</param>
        private void AttachHead(DataTable attachTable, bool align = false)
        {
            for (int rIndex = 0; rIndex < attachTable.Rows.Count; rIndex++)
            {
                IRow row = Sheet.CreateRow(rIndex);
                for (int cIndex = 0; cIndex < attachTable.Columns.Count; cIndex++)
                {
                    if (!string.IsNullOrEmpty(attachTable.Rows[rIndex][cIndex].ToString()))
                    {
                        ICell cell = row.CreateCell(cIndex);
                        cell.SetCellValue(attachTable.Rows[rIndex][cIndex].ToString());
                    }
                }
            }
        }

        #endregion

        #region excel底部汇总数据
        /// <summary>
        /// 传入需要汇总的数据(DataTable格式)
        /// </summary>
        /// <param name="sum"></param>
        private void Summary(DataTable sum, int rowStart)
        {
            for (int rIndex = 0; rIndex < sum.Rows.Count; rIndex++)
            {
                IRow row = Sheet.CreateRow(rowStart);
                for (int cIndex = 0; cIndex < sum.Columns.Count; cIndex++)
                {
                    ICell cell = row.CreateCell(cIndex);
                    cell.CellStyle = GetSumCellStyle();
                    if (!string.IsNullOrEmpty(sum.Rows[rIndex][cIndex].ToString()))
                    {
                        cell.SetCellValue(sum.Rows[rIndex][cIndex].ToString());
                    }
                }
                rowStart++;
            }
        }
        #endregion

        #region 保存生成的excel文件
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="fileName">文件的全路径</param>
        public void SaveAs(string filePath)
        {
            using (FileStream file = new FileStream(filePath, FileMode.OpenOrCreate))
            {

                if (WorkBook != null)
                {
                    WorkBook.Write(file);
                    file.Close();
                    WorkBook.Close();
                }
                //根据模板生成且模板路径有误
                else
                {
                    throw new Exception("保存失败");

                }
            }
        }
        #endregion

        //07以上的excel不支持
        #region 右击文件 属性信息
        //private void SetDocumentSummary()
        //{
        //    DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
        //    dsi.Company = "NPOI";
        //   this.WorkBook.DocumentSummaryInformation = dsi;

        //    SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
        //    si.Author = "远大创新"; //填加xls文件作者信息
        //    si.ApplicationName = "远大创新"; //填加xls文件创建程序信息
        //    si.LastAuthor = "远大创新"; //填加xls文件最后保存者信息
        //    si.Comments = "远大创新"; //填加xls文件作者信息
        //    si.Title = "标题信息"; //填加xls文件标题信息
        //    si.Subject = "主题信息";//填加文件主题信息
        //    si.CreateDateTime = System.DateTime.Now;
        //    this.WorkBook.SummaryInformation = si;
        //}
        #endregion

        #region 单元格样式
        private ICellStyle GetCellBorder()
        {
            ICellStyle style = WorkBook.CreateCellStyle();
            style.BorderBottom = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderTop = BorderStyle.Thin;
            return style;
        }

        private ICellStyle GetSumCellStyle()
        {
            ICellStyle style = WorkBook.CreateCellStyle();
            style.BorderBottom = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderTop = BorderStyle.Thin;

            IFont font = WorkBook.CreateFont();
            //font.IsItalic = true;
            font.FontHeightInPoints = 12;
            font.IsBold = true;
            //font.Color = NPOI.HSSF.Util.HSSFColor.Red.Index;
            style.SetFont(font);

            return style;
        }
        #endregion

        #region 根据模板生成word

        /// <summary>
        /// 根据模板导出Word
        /// </summary>
        /// <param name="templetePath"></param>
        /// <param name="savePath"></param>
        /// <param name="findText"></param>
        /// <param name="replaceText"></param>
        public NPOI.XWPF.UserModel.XWPFDocument GenerateWordByTemplete(Dictionary<string, string> replaceText)
        {
            using (FileStream stream = File.OpenRead(this.TempletePath))
            {
                NPOI.XWPF.UserModel.XWPFDocument doc = new NPOI.XWPF.UserModel.XWPFDocument(stream);
                //遍历段落
                foreach (var para in doc.Paragraphs)
                {
                    ReplaceKey(para, replaceText);
                }
                return doc;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="para"></param>
        /// <param name="findText"></param>
        /// <param name="replaceText"></param>
        private void ReplaceKey(NPOI.XWPF.UserModel.XWPFParagraph para, Dictionary<string, string> replaceText)
        {
            var runs = para.Runs;
            foreach (var run in runs)
            {
                if (replaceText.ContainsKey(run.ToString()))
                {
                    var value = string.Empty;
                    replaceText.TryGetValue(run.ToString(), out value);
                    run.SetText(value, 0);
                }
                foreach (var dic in replaceText)
                {
                    if (run.ToString().Contains(dic.Key))
                    {
                        run.SetText(run.ToString().Replace(dic.Key, dic.Value), 0);
                    }
                }
            }
        }

        #endregion

        #region 根据变更协议模板生成word   2021-02-26 by yeshuangshuang 

        /// <summary>
        /// 根据变更协议模板导出Word
        /// </summary>
        /// <param name="templetePath"></param>
        /// <param name="savePath"></param>
        /// <param name="findText"></param>
        /// <param name="replaceText"></param>
        public NPOI.XWPF.UserModel.XWPFDocument ChangeAgreementWordByTemplete(Dictionary<string, string> replaceText)
        {
            using (FileStream stream = File.OpenRead(this.TempletePath))
            {
                NPOI.XWPF.UserModel.XWPFDocument doc = new NPOI.XWPF.UserModel.XWPFDocument(stream);
                //遍历段落
                foreach (var para in doc.Paragraphs)
                {
                    ChangeReplaceKey(para, replaceText);
                }
                return doc;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="para"></param>
        /// <param name="findText"></param>
        /// <param name="replaceText"></param>
        private void ChangeReplaceKey(NPOI.XWPF.UserModel.XWPFParagraph para, Dictionary<string, string> replaceText)
        {
            var runs = para.Runs;
            foreach (var run in runs)
            {
                if (replaceText.ContainsKey(run.ToString()))
                {
                    var value = string.Empty;
                    replaceText.TryGetValue(run.ToString(), out value);
                    run.SetText(value, 0);
                }
                foreach (var dic in replaceText)
                {
                    if (run.ToString().Contains(dic.Key))
                    {
                        if (dic.Key == "{NewValue}" && dic.Value != "")
                        {
                            run.AddCarriageReturn();
                        }
                        if (dic.Key == "{NewValue1}" && dic.Value != "")
                        {
                            run.AddCarriageReturn();
                        }
                        if (dic.Key == "{NewValue2}" && dic.Value != "")
                        {
                            run.AddCarriageReturn();
                        }
                        if (dic.Key == "{NewValue3}" && dic.Value != "")
                        {
                            run.AddCarriageReturn();
                        }
                        if (dic.Key == "{NewValue4}" && dic.Value != "")
                        {
                            run.AddCarriageReturn();
                        }
                        if (dic.Key == "{NewValue5}" && dic.Value != "")
                        {
                            run.AddCarriageReturn();
                        }

                        run.SetText(run.ToString().Replace(dic.Key, dic.Value), 0);

                    }
                }
            }
        }

        ///// <summary>
        ///// 输出模板docx文档
        ///// </summary>
        ///// <param name="tempFilePath">模板文件地址</param>
        ///// <param name="outFolder">输出文件夹</param >
        ///// <param name="fileName">文件名</param>
        ///// <param name="data">数据格式Json->new { name = "cjc", age = 29 }</param>
        //public NPOI.XWPF.UserModel.XWPFDocument CreateWord(object data)
        //{
        //    using (FileStream stream = File.OpenRead(this.TempletePath))
        //    {
        //        NPOI.XWPF.UserModel.XWPFDocument doc = new NPOI.XWPF.UserModel.XWPFDocument(stream);
        //        //遍历段落                  
        //        foreach (var para in doc.Paragraphs)
        //        {
        //            NewReplaceKey(para, data);
        //        }   //遍历表格      
        //        var tables = doc.Tables;
        //        foreach (var table in tables)
        //        {
        //            foreach (var row in table.Rows)
        //            {
        //                foreach (var cell in row.GetTableCells())
        //                {
        //                    foreach (var para in cell.Paragraphs)
        //                    {
        //                        NewReplaceKey(para, data);
        //                    }
        //                }
        //            }
        //        }
        //        //单独对表格新增
        //        var oprTable = tables[1];


        //        NPOI.XWPF.UserModel.XWPFTableRow m_Row = oprTable.InsertNewTableRow(1);//创建一行/并且在某个位置添加一行
        //        m_Row.AddNewTableCell().SetText("创建一行仅有一个单元格");
        //        NPOI.XWPF.UserModel.XWPFTableRow m_Row2 = oprTable.InsertNewTableRow(2);//创建一行/并且在某个位置添加一行
        //        NPOI.XWPF.UserModel.XWPFTableCell tc3 = m_Row2.CreateCell();//创建单元格
        //        tc3.SetText("创建一行仅有一个单元格(合并后)");
        //        NPOI.OpenXmlFormats.Wordprocessing.CT_Tc ct3 = tc3.GetCTTc();
        //        NPOI.OpenXmlFormats.Wordprocessing.CT_TcPr cp3 = ct3.AddNewTcPr();
        //        cp3.gridSpan = new CT_DecimalNumber();
        //        cp3.gridSpan.val = "3"; //合并3列   


        //        NPOI.XWPF.UserModel.XWPFTableRow m_Row3 = oprTable.InsertNewTableRow(2);//多个单元格以及合并
        //        m_Row3.AddNewTableCell().SetText("添加的新行单元格1");
        //        m_Row3.AddNewTableCell().SetText("添加的新行单元格2");
        //        m_Row3.AddNewTableCell().SetText("添加的新行单元格3");

        //        return doc;
        //        //var fullPath = Path.Combine(outFolder, fileName);
        //        //FileStream outFile = new FileStream(fullPath, FileMode.Create);
        //        //doc.Write(outFile);
        //        //outFile.Close();
        //    }
        //}
        ///// <summary>
        ///// 遍历替换段落位置字符
        ///// </summary>
        ///// <param name="para">段落参数</param>
        ///// <param name="model">数据</param>
        //private static void NewReplaceKey(NPOI.XWPF.UserModel.XWPFParagraph para, object model)
        //{
        //    string text = para.ParagraphText;
        //    var runs = para.Runs;
        //    string styleid = para.Style;
        //    for (int i = 0; i < runs.Count; i++)
        //    {
        //        var run = runs[i];
        //        text = run.ToString();
        //        Type t = model.GetType();
        //        PropertyInfo[] pi = t.GetProperties();
        //        foreach (PropertyInfo p in pi)
        //        {
        //            //$$与模板中$$对应，也可以改成其它符号，比如{$name},务必做到唯一
        //            if (text.Contains("{$" + p.Name + "}"))
        //            {
        //                text = text.Replace("{$" + p.Name + "}", p.GetValue(model, null).ToString());
        //            }
        //        }
        //        runs[i].SetText(text, 0);
        //    }
        //}
        #endregion
        #region 根据模板生成word

        /// <summary>
        /// 根据模板导出Word（包含页眉 页脚，table）
        /// </summary>
        /// <param name="templetePath"></param>
        /// <param name="savePath"></param>
        /// <param name="findText"></param>
        /// <param name="replaceText"></param>
        public NPOI.XWPF.UserModel.XWPFDocument GenerateWordByTempletePlus(Dictionary<string, string> replaceText)
        {
            using (FileStream stream = File.OpenRead(this.TempletePath))
            {
                NPOI.XWPF.UserModel.XWPFDocument doc = new NPOI.XWPF.UserModel.XWPFDocument(stream);
                //遍历段落
                ReplaceWordParagraphs(doc.Paragraphs, replaceText);
                ReplaceWordTables(doc.Tables, replaceText);
                ReplaceWordHeaders(doc.HeaderList, replaceText);
                ReplaceWordFooters(doc.FooterList, replaceText);
                return doc;
            }
        }

        #region // 读取word 内容

        #region // 查找Run

        private static NPOI.XWPF.UserModel.XWPFRun FindPlaceholderRun(string placeholder, NPOI.XWPF.UserModel.XWPFParagraph paragraph)
        {
            NPOI.XWPF.UserModel.XWPFRun matchRun = null;

            var runs = paragraph.Runs;

            NPOI.XWPF.UserModel.TextSegment found = paragraph.SearchText(placeholder, new NPOI.XWPF.UserModel.PositionInParagraph());
            if (found != null)
            {
                if (found.BeginRun == found.EndRun)
                {
                    // 只有一个Run
                    matchRun = runs[found.BeginRun];
                }
                else
                {
                    // 第一个Run设置文本为占位符
                    matchRun = runs[found.BeginRun];

                    // 清空其他Run的文本
                    for (int runPos = found.BeginRun + 1; runPos <= found.EndRun; runPos++)
                    {
                        NPOI.XWPF.UserModel.XWPFRun partNext = runs[runPos];
                        partNext.SetText(string.Empty, 0);
                    }
                }
            }

            return matchRun;
        }

        #endregion
        /// <summary>
        /// 处理word 文本段落
        /// </summary>
        /// <param name="paragraphs"></param>
        /// <param name="replaceText"></param>
        private void ReplaceWordParagraphs(IList<NPOI.XWPF.UserModel.XWPFParagraph> paragraphs, Dictionary<string, string> replaceText)
        {
            foreach (var para in paragraphs)
            {
                ReplaceKeyPlus(para, replaceText);
            }
        }

        /// <summary>
        /// 处理word 表格内容
        /// </summary>
        /// <param name="tables"></param>
        /// <param name="replaceText"></param>
        private void ReplaceWordTables(IList<NPOI.XWPF.UserModel.XWPFTable> tables, Dictionary<string, string> replaceText)
        {
            foreach (var table in tables)
            {

                foreach (var row in table.Rows)
                {
                    foreach (var cell in row.GetTableCells())
                    {
                        ReplaceWordParagraphs(cell.Paragraphs, replaceText);
                    }
                }
            }
        }

        /// <summary>
        /// 处理word 页眉
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="replaceText"></param>
        private void ReplaceWordHeaders(IList<NPOI.XWPF.UserModel.XWPFHeader> headers, Dictionary<string, string> replaceText)
        {

            foreach (var item in headers)
            {
                ReplaceWordParagraphs(item.Paragraphs, replaceText);
            }
        }


        /// <summary>
        /// 处理word页脚
        /// </summary>
        /// <param name="footers"></param>
        /// <param name="replaceText"></param>
        private void ReplaceWordFooters(IList<NPOI.XWPF.UserModel.XWPFFooter> footers, Dictionary<string, string> replaceText)
        {

            foreach (var item in footers)
            {
                ReplaceWordParagraphs(item.Paragraphs, replaceText);
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="para"></param>
        /// <param name="findText"></param>
        /// <param name="replaceText"></param>
        private void ReplaceKeyPlus(NPOI.XWPF.UserModel.XWPFParagraph para, Dictionary<string, string> replaceText)
        {
            // 替换文本
            if (replaceText != null)
            {
                foreach (var item in replaceText)
                {
                    var matchRun = FindPlaceholderRun(item.Key, para);
                    if (matchRun != null)
                    {
                        var value = item.Value ?? string.Empty;
                        if (string.IsNullOrEmpty(value.Trim()))
                        {
                            int contextLength = Encoding.UTF8.GetBytes(item.Key.ToString()).Length;
                            for (int i = 0; i < contextLength; i++)
                            {
                                value += " ";
                            }
                        }

                        matchRun.SetText(value);
                    }
                }
            }

        }

        #endregion


        #endregion
    }
}
