using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.XSSF.UserModel;
using System.Web.Script.Serialization;
using System.Collections;

namespace Yahv.Utils.Npoi
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

        /// <summary>
        /// Excel导入成Datable
        /// </summary>
        /// <returns></returns>
        public DataTable ExcelToDataTable(int headerIndex = 0, int startIndex = 1)
        {
            DataTable dt = new DataTable();
            IWorkbook workbook;

            string file = this.TempletePath;
            string fileExt = Path.GetExtension(file).ToLower(); //文件后缀
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                //XSSFWorkbook 适用XLSX格式，HSSFWorkbook 适用XLS格式
                if (fileExt == ".xlsx")
                {
                    workbook = new XSSFWorkbook(fs);
                }
                else if (fileExt == ".xls")
                {
                    try
                    {
                        workbook = new HSSFWorkbook(fs);
                    }
                    catch (Exception)
                    {
                        workbook = new XSSFWorkbook(file);
                    }
                }
                else
                {
                    workbook = null;
                }

                ISheet sheet = workbook.GetSheetAt(0);

                //表头
                IRow header = sheet.GetRow(headerIndex);

                List<int> columns = new List<int>();
                for (int i = 0; i < header.LastCellNum; i++)
                {
                    var obj1 = GetValue(header.GetCell(i));

                    if (obj1 != null && obj1.ToString() != string.Empty && !dt.Columns.Contains(obj1.ToString()))
                    {
                        dt.Columns.Add(new DataColumn(obj1.ToString()));
                    }
                    else
                        dt.Columns.Add(new DataColumn("Columns" + i.ToString()));

                    columns.Add(i);
                }
                //数据
                for (int i = startIndex; i <= sheet.LastRowNum; i++)
                {
                    //隐藏行不读取
                    if (sheet.GetRow(i).ZeroHeight)
                    {
                        continue;
                    }

                    DataRow dr = dt.NewRow();
                    bool hasValue = false;
                    foreach (int j in columns)
                    {
                        //隐藏列不读取
                        if (sheet.IsColumnHidden(j))
                        {
                            continue;
                        }


                        dr[j] = GetValue(sheet.GetRow(i).GetCell(j));
                        if (dr[j] != null && dr[j].ToString() != string.Empty)
                        {
                            hasValue = true;
                        }
                    }
                    if (hasValue)
                    {
                        dt.Rows.Add(dr);
                    }
                }
            }
            return dt;
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
                    if (!string.IsNullOrEmpty(sum.Rows[rIndex][cIndex].ToString()))
                    {
                        ICell cell = row.CreateCell(cIndex);
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

        #region 调整excel文件样式

        /// <summary>
        /// 设置表头颜色
        /// </summary>
        /// <param name="rownum">表头行</param>
        /// <param name="cellColors">列颜色</param>
        public void SetHeaderColor(int rownum, short[] cellColors)
        {
            var header = Sheet.GetRow(rownum);
            for (int cellnum = 0; cellnum < cellColors.Length; cellnum++)
            {
                ICellStyle style = GetCellBorder();
                style.FillForegroundColor = cellColors[cellnum];
                style.FillPattern = FillPattern.SolidForeground;
                header.GetCell(cellnum).CellStyle = style;
            }
        }

        /// <summary>
        /// 设置表体颜色
        /// </summary>
        /// <param name="startnum">起始行</param>
        /// <param name="cellColors">列颜色</param>
        public void SetBodyColor(int startnum, short[] cellColors)
        {
            int rownums = Sheet.LastRowNum;

            for (int cellnum = 0; cellnum < cellColors.Length; cellnum++)
            {
                ICellStyle style = GetCellBorder();
                style.FillForegroundColor = cellColors[cellnum];
                style.FillPattern = FillPattern.SolidForeground;

                for (int rownum = startnum; rownum <= rownums; rownum++)
                {
                    var row = Sheet.GetRow(rownum);
                    row.GetCell(cellnum).CellStyle = style;
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

        #region 单元格边框
        private ICellStyle GetCellBorder()
        {
            ICellStyle style = WorkBook.CreateCellStyle();
            style.BorderBottom = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderTop = BorderStyle.Thin;
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

        /// <summary>
        /// 发票导出xml
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="savePath"></param>
        public static void InvoiceInfoXml(string xml, string savePath)
        {
            System.Xml.XmlDocument xdoc = new System.Xml.XmlDocument();
            xdoc.LoadXml(xml);
            xdoc.Save(savePath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="filePath"></param>
        public static void InvoiceInfoExcel(DataTable dt, string filePath)
        {
            //建立空白工作簿
            XSSFWorkbook wb = new XSSFWorkbook();
            #region 标题样式 
            ICellStyle headstyle = wb.CreateCellStyle();
            //水平对齐居中
            headstyle.Alignment = HorizontalAlignment.Center;
            //字体样式
            IFont headfont = wb.CreateFont();
            headfont.FontName = "楷体";
            headfont.Boldweight = (short)FontBoldWeight.Normal;//字体加粗        
            headfont.FontHeightInPoints = 24;//字号                
            //单元格样式(边框)
            headstyle.SetFont(headfont);
            headstyle.BorderBottom = BorderStyle.Thin;
            headstyle.BorderLeft = BorderStyle.Thin;
            headstyle.BorderRight = BorderStyle.Thin;
            headstyle.BorderTop = BorderStyle.Thin;
            #endregion

            #region 正文样式
            ICellStyle borderstyle = wb.CreateCellStyle();
            borderstyle.BorderBottom = BorderStyle.Thin;
            borderstyle.BorderLeft = BorderStyle.Thin;
            borderstyle.BorderRight = BorderStyle.Thin;
            borderstyle.BorderTop = BorderStyle.Thin;
            #endregion

            #region 文字竖直显示
            ICellStyle cellVertical = wb.CreateCellStyle();
            cellVertical.Alignment = HorizontalAlignment.Center;
            cellVertical.VerticalAlignment = VerticalAlignment.Center;
            cellVertical.WrapText = true;
            #endregion

            ISheet sheet = wb.CreateSheet("发票信息");
            IRow row;
            ICell cell;
            int rowindex = 0;
            int columnCount = 13;
            //标题
            row = sheet.CreateRow(rowindex);
            cell = row.CreateCell(0);
            cell.SetCellValue("开票信息表");
            cell.CellStyle = headstyle;
            CreateCell(row, 1, columnCount, headstyle);
            sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, columnCount));

            #region  设置宽度
            //设置列宽度，256*字符数，因为单位是1/256个字符
            sheet.SetColumnWidth(0, 256 * 8);
            sheet.SetColumnWidth(1, 256 * 22);
            sheet.SetColumnWidth(2, 256 * 20);
            sheet.SetColumnWidth(3, 256 * 15);
            sheet.SetColumnWidth(4, 256 * 15);
            sheet.SetColumnWidth(5, 256 * 15);
            sheet.SetColumnWidth(6, 256 * 15);
            sheet.SetColumnWidth(7, 256 * 15);
            sheet.SetColumnWidth(8, 256 * 20);
            sheet.SetColumnWidth(9, 256 * 15);
            sheet.SetColumnWidth(10, 256 * 15);
            sheet.SetColumnWidth(11, 256 * 15);
            sheet.SetColumnWidth(12, 256 * 20);
            sheet.SetColumnWidth(13, 256 * 30);
            #endregion

            #region 正文标题
            rowindex++;
            row = sheet.CreateRow(rowindex);
            var columnName = new string[] { "序号", "品名", "规格型号", "单位", "不含税单价", "数量(PCS)", "不含税金额", "税率", "税收分类编码", "含税单价", "含税金额", "差额", "订单号/合同号", "型号信息分类" };
            for (int i = 0; i < columnName.Length; i++)
            {
                cell = row.CreateCell(i);
                cell.SetCellValue(columnName[i]);
                cell.CellStyle = borderstyle;
            }
            #endregion

            #region 正文信息
            rowindex++;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                row = sheet.CreateRow(rowindex + i);
                for (int j = 0; j < columnName.Length; j++)
                {
                    cell = row.CreateCell(j);
                    cell.SetCellValue(dt.Rows[i][j].ToString());
                    cell.CellStyle = borderstyle;
                }
            }
            rowindex += dt.Rows.Count;
            #endregion

            #region 总计
            row = sheet.CreateRow(rowindex);
            cell = row.CreateCell(0);
            cell.SetCellValue("合计");
            cell.CellStyle = borderstyle;
            //数量，不含税金额，含税金额，差额
            CreateCell(row, 1, 4, borderstyle);
            cell = row.CreateCell(5);
            cell.SetCellValue(dt.Rows[0]["TotalQty"].ToString());
            cell.CellStyle = borderstyle;
            cell = row.CreateCell(6);
            cell.SetCellValue(dt.Rows[0]["TotalTaxOffAmount"].ToString());
            cell.CellStyle = borderstyle;
            CreateCell(row, 7, 9, borderstyle);
            cell = row.CreateCell(10);
            cell.SetCellValue(dt.Rows[0]["TotalTaxAmount"].ToString());
            cell.CellStyle = borderstyle;
            cell = row.CreateCell(11);
            cell.SetCellValue(dt.Rows[0]["TotalDiffAmount"].ToString());
            cell.CellStyle = borderstyle;
            CreateCell(row, 12, 13, borderstyle);
            //合并单元格
            sheet.AddMergedRegion(new CellRangeAddress(rowindex, rowindex, 0, 2));
            #endregion

            #region 开票资料
            rowindex++;
            row = sheet.CreateRow(rowindex);
            cell = row.CreateCell(0);
            cell.SetCellValue("开\r\n票\r\n资\r\n料");
            cell.CellStyle = cellVertical;
            cell = row.CreateCell(1);
            cell.SetCellValue("购货单位名称");
            cell.CellStyle = borderstyle;
            cell = row.CreateCell(2);
            cell.SetCellValue(dt.Rows[0]["CompanyName"].ToString());
            cell.CellStyle = borderstyle;
            CreateCell(row, 3, columnCount, borderstyle);
            sheet.AddMergedRegion(new CellRangeAddress(rowindex, rowindex, 2, columnCount));
            sheet.AddMergedRegion(new CellRangeAddress(rowindex, rowindex + 3, 0, 0));

            rowindex++;
            row = sheet.CreateRow(rowindex);
            cell = row.CreateCell(0);
            cell.SetCellValue("");
            cell.CellStyle = borderstyle;
            cell = row.CreateCell(1);
            cell.SetCellValue("纳税人识别号");
            cell.CellStyle = borderstyle;
            cell = row.CreateCell(2);
            cell.SetCellValue(dt.Rows[0]["TaxNumber"].ToString());
            cell.CellStyle = borderstyle;
            CreateCell(row, 3, columnCount, borderstyle);
            sheet.AddMergedRegion(new CellRangeAddress(rowindex, rowindex, 2, columnCount));

            rowindex++;
            row = sheet.CreateRow(rowindex);
            cell = row.CreateCell(0);
            cell.SetCellValue("");
            cell.CellStyle = borderstyle;
            cell = row.CreateCell(1);
            cell.SetCellValue("地址、电话");
            cell.CellStyle = borderstyle;
            cell = row.CreateCell(2);
            cell.SetCellValue(dt.Rows[0]["AddressPhone"].ToString());
            cell.CellStyle = borderstyle;
            CreateCell(row, 3, columnCount, borderstyle);
            sheet.AddMergedRegion(new CellRangeAddress(rowindex, rowindex, 2, columnCount));

            rowindex++;
            row = sheet.CreateRow(rowindex);
            cell = row.CreateCell(0);
            cell.SetCellValue("");
            cell.CellStyle = borderstyle;
            cell = row.CreateCell(1);
            cell.SetCellValue("开户银行及账号");
            cell.CellStyle = borderstyle;
            cell = row.CreateCell(2);
            cell.SetCellValue(dt.Rows[0]["BankInfo"].ToString());
            cell.CellStyle = borderstyle;
            CreateCell(row, 3, columnCount, borderstyle);
            sheet.AddMergedRegion(new CellRangeAddress(rowindex, rowindex, 2, columnCount));

            rowindex++;
            row = sheet.CreateRow(rowindex);//空白行
            CreateCell(row, 0, columnCount, borderstyle);
            sheet.AddMergedRegion(new CellRangeAddress(rowindex, rowindex, 0, columnCount));
            #endregion

            #region 寄票信息
            rowindex++;
            row = sheet.CreateRow(rowindex);
            cell = row.CreateCell(0);
            cell.SetCellValue("寄\r\n票\r\n信\r\n息");
            cell.CellStyle = cellVertical;
            cell = row.CreateCell(1);
            cell.SetCellValue("收票公司名称");
            cell.CellStyle = borderstyle;
            CreateCell(row, 3, columnCount, borderstyle);
            sheet.AddMergedRegion(new CellRangeAddress(rowindex, rowindex, 2, columnCount));
            sheet.AddMergedRegion(new CellRangeAddress(rowindex, rowindex + 3, 0, 0));

            rowindex++;
            row = sheet.CreateRow(rowindex);
            cell = row.CreateCell(0);
            cell.SetCellValue("");
            cell.CellStyle = borderstyle;
            cell = row.CreateCell(1);
            cell.SetCellValue("收票公司地址");
            cell.CellStyle = borderstyle;
            cell = row.CreateCell(2);
            cell.SetCellValue(dt.Rows[0]["RecipientAddress"].ToString());
            cell.CellStyle = borderstyle;
            CreateCell(row, 3, columnCount, borderstyle);
            sheet.AddMergedRegion(new CellRangeAddress(rowindex, rowindex, 2, columnCount));

            rowindex++;
            row = sheet.CreateRow(rowindex);
            cell = row.CreateCell(0);
            cell.SetCellValue("");
            cell.CellStyle = borderstyle;
            cell = row.CreateCell(1);
            cell.SetCellValue("收件人姓名");
            cell.CellStyle = borderstyle;
            cell = row.CreateCell(2);
            cell.SetCellValue(dt.Rows[0]["RecipientName"].ToString());
            cell.CellStyle = borderstyle;
            CreateCell(row, 3, columnCount, borderstyle);
            sheet.AddMergedRegion(new CellRangeAddress(rowindex, rowindex, 2, columnCount));

            rowindex++;
            row = sheet.CreateRow(rowindex);
            cell = row.CreateCell(0);
            cell.SetCellValue("");
            cell.CellStyle = borderstyle;
            cell = row.CreateCell(1);
            cell.SetCellValue("收件人电话");
            cell.CellStyle = borderstyle;
            cell = row.CreateCell(2);
            cell.SetCellValue(dt.Rows[0]["RecipientPhone"].ToString());
            cell.CellStyle = borderstyle;
            CreateCell(row, 3, columnCount, borderstyle);
            sheet.AddMergedRegion(new CellRangeAddress(rowindex, rowindex, 2, columnCount));
            rowindex++;

            #endregion

            #region 备注
            rowindex++;
            row = sheet.CreateRow(rowindex);
            cell = row.CreateCell(0);
            cell.SetCellValue("备注:");
            cell = row.CreateCell(1);
            //TODO:待确定备注
            cell.SetCellValue("");
            #endregion

            //保存文件
            FileStream file = new FileStream(filePath, FileMode.Create);
            wb.Write(file);
            file.Close();
        }

        /// <summary>
        /// 开票信息导入到Excel
        /// TODO:需要重构导出文件
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="savePath"></param>
        public static void InvoiceInfoExcel(List<DataTable> dts, string savePath, int sheetcount)
        {
            //建立空白工作簿
            XSSFWorkbook wb = new XSSFWorkbook();

            #region 设置标题样式
            ICellStyle style = wb.CreateCellStyle();
            //设置单元格的样式：水平对齐居中
            style.Alignment = HorizontalAlignment.Center;
            //新建一个字体样式对象
            IFont font = wb.CreateFont();
            font.FontName = "楷体";
            font.Boldweight = (short)FontBoldWeight.Normal;//字体加粗        
            font.FontHeightInPoints = 24;//字号                
            //使用SetFont方法将字体样式添加到单元格样式中
            style.SetFont(font);
            style.BorderBottom = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderTop = BorderStyle.Thin;
            #endregion

            #region 文字竖直显示
            ICellStyle cellVertical = wb.CreateCellStyle();
            cellVertical.Alignment = HorizontalAlignment.Center;
            cellVertical.VerticalAlignment = VerticalAlignment.Center;
            cellVertical.WrapText = true;
            #endregion

            #region 设置单元格边框
            ICellStyle borderstyle = wb.CreateCellStyle();
            borderstyle.BorderBottom = BorderStyle.Thin;
            borderstyle.BorderLeft = BorderStyle.Thin;
            borderstyle.BorderRight = BorderStyle.Thin;
            borderstyle.BorderTop = BorderStyle.Thin;
            #endregion


            for (int i = 0; i < sheetcount; i++)
            {
                DataTable dt = dts[i];
                ISheet sheet = wb.CreateSheet(dt.Rows[0]["sheetname"].ToString());
                IRow row;
                NPOI.SS.UserModel.ICell cell;

                #region 标题
                row = sheet.CreateRow(0);
                cell = row.CreateCell(0);
                cell.SetCellValue("开票信息表");
                cell.CellStyle = style;
                cell = row.CreateCell(1);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(2);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(3);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(4);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(5);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(6);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(7);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(8);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(9);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(10);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(11);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(12);
                cell.CellStyle = borderstyle;
                sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 12));
                #endregion

                #region  设置宽度
                //设置列宽度，256*字符数，因为单位是1/256个字符
                sheet.SetColumnWidth(0, 256 * 8);
                sheet.SetColumnWidth(1, 256 * 20);
                sheet.SetColumnWidth(2, 256 * 30);
                sheet.SetColumnWidth(3, 256 * 10);
                sheet.SetColumnWidth(4, 256 * 10);
                sheet.SetColumnWidth(5, 256 * 10);
                sheet.SetColumnWidth(6, 256 * 10);
                sheet.SetColumnWidth(7, 256 * 10);
                sheet.SetColumnWidth(8, 256 * 20);
                sheet.SetColumnWidth(9, 256 * 10);
                sheet.SetColumnWidth(10, 256 * 10);
                sheet.SetColumnWidth(11, 256 * 20);
                sheet.SetColumnWidth(12, 256 * 30);
                #endregion

                #region 子表信息标题
                row = sheet.CreateRow(1);
                cell = row.CreateCell(0);
                cell.SetCellValue("序号");
                cell.CellStyle = borderstyle;

                cell = row.CreateCell(1);
                cell.SetCellValue("品名");
                cell.CellStyle = borderstyle;

                cell = row.CreateCell(2);
                cell.SetCellValue("规格型号");
                cell.CellStyle = borderstyle;

                cell = row.CreateCell(3);
                cell.SetCellValue("单位");
                cell.CellStyle = borderstyle;

                cell = row.CreateCell(4);
                cell.SetCellValue("不含税单价");
                cell.CellStyle = borderstyle;

                cell = row.CreateCell(5);
                cell.SetCellValue("数量(PCS)");
                cell.CellStyle = borderstyle;

                cell = row.CreateCell(6);
                cell.SetCellValue("不含税金额");
                cell.CellStyle = borderstyle;

                cell = row.CreateCell(7);
                cell.SetCellValue("税率");
                cell.CellStyle = borderstyle;

                cell = row.CreateCell(8);
                cell.SetCellValue("税收分类编码");
                cell.CellStyle = borderstyle;

                cell = row.CreateCell(9);
                cell.SetCellValue("含税单价");
                cell.CellStyle = borderstyle;

                cell = row.CreateCell(10);
                cell.SetCellValue("含税金额");
                cell.CellStyle = borderstyle;

                cell = row.CreateCell(11);
                cell.SetCellValue("差额");
                cell.CellStyle = borderstyle;


                cell = row.CreateCell(12);
                cell.SetCellValue("订单号/合同号");
                cell.CellStyle = borderstyle;

                cell = row.CreateCell(13);
                cell.SetCellValue("型号信息分类");
                cell.CellStyle = borderstyle;


                #endregion

                int isiglerow = Convert.ToInt16(dt.Rows[0]["rowscount"].ToString());


                #region 子表数据行
                for (int j = 0; j < isiglerow; j++)
                {
                    row = sheet.CreateRow(j + 2);
                    cell = row.CreateCell(0);
                    cell.SetCellValue(dt.Rows[j]["NO"].ToString());
                    cell.CellStyle = borderstyle;

                    cell = row.CreateCell(1);
                    cell.SetCellValue(dt.Rows[j]["Name"].ToString());
                    cell.CellStyle = borderstyle;

                    cell = row.CreateCell(2);
                    cell.SetCellValue(dt.Rows[j]["Model"].ToString());
                    cell.CellStyle = borderstyle;

                    cell = row.CreateCell(3);
                    //cell.SetCellValue(dt.Rows[j]["IsLocal"].ToString().Equals("1") ? "个" : dt.Rows[j]["Unit"].ToString());
                    cell.SetCellValue(dt.Rows[j]["Unit"].ToString());
                    cell.CellStyle = borderstyle;

                    cell = row.CreateCell(4);
                    cell.SetCellValue(Convert.ToDouble(dt.Rows[j]["TaxoffUnitPrice"].ToString()));
                    // cell.SetCellValue(Convert.ToDecimal(dt.Rows[j]["TaxoffUnitPrice"].ToString()));
                    cell.CellStyle = borderstyle;

                    cell = row.CreateCell(5);
                    cell.SetCellValue(Convert.ToDouble(dt.Rows[j]["Qty"].ToString()));
                    cell.CellStyle = borderstyle;

                    cell = row.CreateCell(6);
                    cell.SetCellValue(Convert.ToDouble(dt.Rows[j]["TaxOffAmount"].ToString()));
                    cell.CellStyle = borderstyle;

                    cell = row.CreateCell(7);
                    cell.SetCellValue(Convert.ToDouble(dt.Rows[j]["TaxPoint"].ToString()));
                    cell.CellStyle = borderstyle;

                    cell = row.CreateCell(8);
                    cell.SetCellValue(dt.Rows[j]["ModelInfoClassificationValue"].ToString());
                    cell.CellStyle = borderstyle;

                    cell = row.CreateCell(9);
                    cell.SetCellValue(Convert.ToDouble(dt.Rows[j]["UnitPriceTax"].ToString()));
                    cell.CellStyle = borderstyle;

                    cell = row.CreateCell(10);
                    cell.SetCellValue(Convert.ToDouble(dt.Rows[j]["AmountTax"].ToString()));
                    cell.CellStyle = borderstyle;

                    cell = row.CreateCell(11);
                    cell.SetCellValue(Convert.ToDouble(dt.Rows[j]["Difference"].ToString()));
                    cell.CellStyle = borderstyle;

                    //if (dt.Rows[j]["IsLocal"].ToString().Equals("1"))
                    //{
                    //    cell = row.CreateCell(11);
                    //    cell.SetCellValue(dt.Rows[j]["ContractNO"].ToString());
                    //    cell.CellStyle = borderstyle;
                    //}
                    //else
                    //{
                    cell = row.CreateCell(12);
                    cell.SetCellValue(dt.Rows[j]["OrderNo"].ToString());
                    cell.CellStyle = borderstyle;
                    //}

                    cell = row.CreateCell(13);
                    cell.SetCellValue(dt.Rows[j]["ModelInfoClassification"].ToString());
                    cell.CellStyle = borderstyle;



                }
                #endregion

                #region 总计
                row = sheet.CreateRow(2 + isiglerow);
                cell = row.CreateCell(0);
                cell.SetCellValue("合计");
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(1);
                cell.SetCellValue("");
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(2);
                cell.CellStyle = borderstyle;
                cell.SetCellValue("");
                cell = row.CreateCell(3);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(4);
                cell.CellStyle = borderstyle;
                cell.SetCellValue("");
                cell = row.CreateCell(5);
                cell.SetCellValue(Convert.ToDouble(dt.Rows[0]["TotalQuantity"]));
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(6);
                if (Convert.ToDouble(dt.Rows[0]["TotalQuantity"]) != 0)
                {
                    // cell.SetCellValue(Convert.ToDouble(dt.Rows[0]["TotalTaxOffAmount"]).ToString("#.00"));
                    cell.SetCellValue(Math.Round(Convert.ToDouble(dt.Rows[0]["TotalTaxOffAmount"]), 2));

                }
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(7);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(8);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(9);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(10);
                cell.SetCellValue(Convert.ToDouble(dt.Rows[0]["TotalAmountTax"]));
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(11);
                cell.SetCellValue(Convert.ToDouble(dt.Rows[0]["TotalDifference"]));
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(12);
                cell.CellStyle = borderstyle;
                #endregion

                #region 开票资料
                row = sheet.CreateRow(2 + isiglerow + 1);
                cell = row.CreateCell(0);
                cell.SetCellValue("开\r\n票\r\n资\r\n料");
                cell.CellStyle = cellVertical;
                cell = row.CreateCell(1);
                cell.SetCellValue("购货单位名称");
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(2);
                cell.SetCellValue(dt.Rows[0]["CompanyName"].ToString());
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(3);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(4);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(5);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(6);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(7);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(8);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(9);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(10);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(11);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(12);
                cell.CellStyle = borderstyle;

                row = sheet.CreateRow(2 + isiglerow + 2);
                cell = row.CreateCell(0);
                cell.SetCellValue("");
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(1);
                cell.SetCellValue("纳税人识别号");
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(2);
                cell.SetCellValue(dt.Rows[0]["TaxNumber"].ToString());
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(3);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(4);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(5);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(6);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(7);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(8);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(9);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(10);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(11);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(12);
                cell.CellStyle = borderstyle;

                row = sheet.CreateRow(2 + isiglerow + 3);
                cell = row.CreateCell(0);
                cell.SetCellValue("");
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(1);
                cell.SetCellValue("地址、电话");
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(2);
                cell.SetCellValue(dt.Rows[0]["AddressPhone"].ToString());
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(3);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(4);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(5);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(6);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(7);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(8);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(9);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(10);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(11);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(12);
                cell.CellStyle = borderstyle;

                row = sheet.CreateRow(2 + isiglerow + 4);
                cell = row.CreateCell(0);
                cell.SetCellValue("");
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(1);
                cell.SetCellValue("开户银行及账号");
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(2);
                cell.SetCellValue(dt.Rows[0]["BankInfo"].ToString());
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(3);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(4);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(5);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(6);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(7);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(8);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(9);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(10);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(11);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(12);
                cell.CellStyle = borderstyle;

                #endregion

                #region 空白行
                row = sheet.CreateRow(2 + isiglerow + 5);
                cell = row.CreateCell(0);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(1);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(2);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(3);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(4);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(5);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(6);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(7);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(8);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(9);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(10);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(11);
                cell.CellStyle = borderstyle;
                cell = row.CreateCell(12);
                cell.CellStyle = borderstyle;
                #endregion


                if (dt.Rows[0]["InvoiceDeliveryMethod"].ToString().Equals("2"))
                {
                    //随货同行
                    #region 寄票信息
                    row = sheet.CreateRow(2 + isiglerow + 6);
                    cell = row.CreateCell(0);
                    cell.SetCellValue("寄票信息");
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(1);
                    cell.SetCellValue("随货同行");
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(2);
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(3);
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(4);
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(5);
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(6);
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(7);
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(8);
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(9);
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(10);
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(11);
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(12);
                    cell.CellStyle = borderstyle;
                    #endregion

                    #region 备注
                    row = sheet.CreateRow(2 + isiglerow + 8);
                    cell = row.CreateCell(0);
                    cell.SetCellValue("备注:");
                    cell = row.CreateCell(1);
                    cell.SetCellValue(dt.Rows[0]["Remark"].ToString());
                    #endregion


                    #region 合并行
                    //合计行
                    sheet.AddMergedRegion(new CellRangeAddress(2 + isiglerow, 2 + isiglerow, 0, 2));
                    //开票资料单员格合并
                    sheet.AddMergedRegion(new CellRangeAddress(2 + isiglerow + 1, 2 + isiglerow + 4, 0, 0));
                    //开票资料
                    sheet.AddMergedRegion(new CellRangeAddress(2 + isiglerow + 1, 2 + isiglerow + 1, 2, 12));
                    sheet.AddMergedRegion(new CellRangeAddress(2 + isiglerow + 2, 2 + isiglerow + 2, 2, 12));
                    sheet.AddMergedRegion(new CellRangeAddress(2 + isiglerow + 3, 2 + isiglerow + 3, 2, 12));
                    sheet.AddMergedRegion(new CellRangeAddress(2 + isiglerow + 4, 2 + isiglerow + 4, 2, 12));
                    //空白行合并
                    sheet.AddMergedRegion(new CellRangeAddress(2 + isiglerow + 5, 2 + isiglerow + 5, 0, 12));
                    //寄票信息合并
                    sheet.AddMergedRegion(new CellRangeAddress(2 + isiglerow + 6, 2 + isiglerow + 6, 1, 12));
                    #endregion
                }
                else if (dt.Rows[0]["InvoiceDeliveryMethod"].ToString().Equals("1"))
                {
                    //邮寄
                    #region 寄票信息
                    row = sheet.CreateRow(2 + isiglerow + 6);
                    cell = row.CreateCell(0);
                    cell.SetCellValue("寄\r\n票\r\n信\r\n息");
                    cell.CellStyle = cellVertical;
                    cell = row.CreateCell(1);
                    cell.SetCellValue("收票公司名称");
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(2);
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(3);
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(4);
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(5);
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(6);
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(7);
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(8);
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(9);
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(10);
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(11);
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(12);
                    cell.CellStyle = borderstyle;

                    row = sheet.CreateRow(2 + isiglerow + 7);
                    cell = row.CreateCell(0);
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(1);
                    cell.SetCellValue("收票公司地址");
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(2);
                    cell.SetCellValue(dt.Rows[0]["ReceiptAddress"].ToString());
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(3);
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(4);
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(5);
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(6);
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(7);
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(8);
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(9);
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(10);
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(11);
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(12);
                    cell.CellStyle = borderstyle;

                    row = sheet.CreateRow(2 + isiglerow + 8);
                    cell = row.CreateCell(0);
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(1);
                    cell.SetCellValue("收件人姓名");
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(2);
                    cell.SetCellValue(dt.Rows[0]["ReceiptName"].ToString());
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(3);
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(4);
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(5);
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(6);
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(7);
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(8);
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(9);
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(10);
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(11);
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(12);
                    cell.CellStyle = borderstyle;

                    row = sheet.CreateRow(2 + isiglerow + 9);
                    cell = row.CreateCell(0);
                    cell.SetCellValue("");
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(1);
                    cell.SetCellValue("收件人电话");
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(2);
                    cell.SetCellValue(dt.Rows[0]["ReceiptPhone"].ToString());
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(3);
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(4);
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(5);
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(6);
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(7);
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(8);
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(9);
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(10);
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(11);
                    cell.CellStyle = borderstyle;
                    cell = row.CreateCell(12);
                    cell.CellStyle = borderstyle;
                    #endregion

                    #region 备注
                    row = sheet.CreateRow(2 + isiglerow + 11);
                    cell = row.CreateCell(0);
                    cell.SetCellValue("备注:");
                    cell = row.CreateCell(1);
                    cell.SetCellValue(dt.Rows[0]["Remark"].ToString());
                    #endregion

                    #region 合并行
                    //合计行
                    sheet.AddMergedRegion(new CellRangeAddress(2 + isiglerow, 2 + isiglerow, 0, 2));
                    //开票资料单员格合并
                    sheet.AddMergedRegion(new CellRangeAddress(2 + isiglerow + 1, 2 + isiglerow + 4, 0, 0));
                    //开票资料
                    sheet.AddMergedRegion(new CellRangeAddress(2 + isiglerow + 1, 2 + isiglerow + 1, 2, 12));
                    sheet.AddMergedRegion(new CellRangeAddress(2 + isiglerow + 2, 2 + isiglerow + 2, 2, 12));
                    sheet.AddMergedRegion(new CellRangeAddress(2 + isiglerow + 3, 2 + isiglerow + 3, 2, 12));
                    sheet.AddMergedRegion(new CellRangeAddress(2 + isiglerow + 4, 2 + isiglerow + 4, 2, 12));
                    //空白行合并
                    sheet.AddMergedRegion(new CellRangeAddress(2 + isiglerow + 5, 2 + isiglerow + 5, 0, 12));
                    //寄票信息单员格合并
                    sheet.AddMergedRegion(new CellRangeAddress(2 + isiglerow + 6, 2 + isiglerow + 9, 0, 0));
                    //寄票信息合并
                    sheet.AddMergedRegion(new CellRangeAddress(2 + isiglerow + 6, 2 + isiglerow + 6, 2, 12));
                    sheet.AddMergedRegion(new CellRangeAddress(2 + isiglerow + 7, 2 + isiglerow + 7, 2, 12));
                    sheet.AddMergedRegion(new CellRangeAddress(2 + isiglerow + 8, 2 + isiglerow + 8, 2, 12));
                    sheet.AddMergedRegion(new CellRangeAddress(2 + isiglerow + 9, 2 + isiglerow + 9, 2, 12));
                    #endregion

                }
            }
            FileStream file = new FileStream(savePath, FileMode.Create);
            wb.Write(file);
            file.Close();
        }

        /// <summary>
        /// 生成错误Excel信息
        /// </summary>
        /// <param name="errorMsgs"></param>
        /// <param name="titleIndex"></param>
        /// <returns></returns>
        public string GenerateErrorExcel(Dictionary<int, string> errorMsgs, int titleIndex = 0)
        {
            string result = string.Empty;

            IWorkbook workbook = null;
            string fileName = this.TempletePath;
            string fileExt = Path.GetExtension(fileName).ToLower(); //文件后缀

            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                //XSSFWorkbook 适用XLSX格式，HSSFWorkbook 适用XLS格式
                if (fileExt == ".xlsx")
                {
                    workbook = new XSSFWorkbook(fs);
                }
                else if (fileExt == ".xls")
                {
                    workbook = new HSSFWorkbook(fs);
                }
                else
                {
                    workbook = null;
                }

                ISheet sheet = workbook.GetSheetAt(0);

                if (sheet != null)
                {
                    //创建一列存储错误信息
                    int lastCellNum = sheet.GetRow(titleIndex).LastCellNum;
                    ICell cell = sheet.GetRow(titleIndex).CreateCell(lastCellNum);
                    cell.SetCellValue("错误信息");

                    var cellstyle = workbook.CreateCellStyle();
                    cellstyle.FillBackgroundColor = HSSFColor.Red.Index;
                    cellstyle.FillPattern = FillPattern.SolidForeground;

                    cell.CellStyle = cellstyle;

                    foreach (var errorMsg in errorMsgs)
                    {
                        sheet.GetRow(errorMsg.Key)?.CreateCell(lastCellNum).SetCellValue(errorMsg.Value);
                    }
                }
                FileInfo fi = new FileInfo(fileName);
                string errorFile = fi.DirectoryName + "\\error" + DateTime.Now.ToString("yyyyMMddHHmmss") + fileExt;
                using (FileStream errorExcel = new FileStream(errorFile, FileMode.Create))
                {
                    workbook.Write(errorExcel);
                    errorExcel.Close();

                    result = errorFile;
                }
            }

            return result;
        }

        private static void CreateCell(IRow row, int startindex, int endindex, ICellStyle cellStyle)
        {
            for (int i = startindex; i < endindex; i++)
            {
                ICell cell = row.CreateCell(i);
                cell.SetCellValue("");
                cell.CellStyle = cellStyle;
            }
        }

        /// <summary>
        /// 获取单元格
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private object GetValue(ICell cell)
        {
            if (cell == null)
                return null;
            switch (cell.CellType)
            {
                case CellType.Blank: //BLANK:  
                    return null;
                case CellType.Boolean: //BOOLEAN:  
                    return cell.BooleanCellValue;
                case CellType.Numeric: //NUMERIC:  
                    //NPOI中数字和日期都是NUMERIC类型的，这里对其进行判断是否是日期类型
                    if (HSSFDateUtil.IsCellDateFormatted(cell))//日期类型
                    {
                        return cell.DateCellValue;
                    }
                    else//其他数字类型
                    {
                        return cell.NumericCellValue;
                    }
                case CellType.String: //STRING:  
                    return cell.StringCellValue;
                case CellType.Error: //ERROR:  
                    return cell.ErrorCellValue;
                case CellType.Formula: //FORMULA:  
                    //return "=" + cell.CellFormula;
                    return cell.NumericCellValue.ToString(CultureInfo.InvariantCulture);
                default:
                    return cell.StringCellValue;
            }
        }

        /// <summary>
        /// Json 字符串 转换为 DataTable数据集合
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static DataTable JsonToDataTable(string json)
        {
            DataTable dataTable = new DataTable();  //实例化
            DataTable result;
            try
            {
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                javaScriptSerializer.MaxJsonLength = Int32.MaxValue; //取得最大数值
                ArrayList arrayList = javaScriptSerializer.Deserialize<ArrayList>(json);
                if (arrayList.Count > 0)
                {
                    foreach (Dictionary<string, object> dictionary in arrayList)
                    {
                        if (dictionary.Keys.Count<string>() == 0)
                        {
                            result = dataTable;
                            return result;
                        }
                        //Columns
                        if (dataTable.Columns.Count == 0)
                        {
                            foreach (string current in dictionary.Keys)
                            {
                                var value = dictionary[current] ?? string.Empty;
                                dataTable.Columns.Add(current, value.GetType());
                            }
                        }
                        //Rows
                        DataRow dataRow = dataTable.NewRow();
                        foreach (string current in dictionary.Keys)
                        {
                            dataRow[current] = dictionary[current] ?? string.Empty;
                        }
                        dataTable.Rows.Add(dataRow); //循环添加行到DataTable中
                    }
                }
            }
            catch (Exception ex)
            {
            }
            result = dataTable;
            return result;
        }

        /// <summary>
        /// 开票明细导入到Excel
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="savePath"></param>
        public static void NPOIExcel(DataTable dt, string savePath)
        {
            // xssfworkbook
            XSSFWorkbook wb = new XSSFWorkbook();
            // HSSFWorkbook wb = new HSSFWorkbook();

            //标题样式样式
            ICellStyle titleStyle = wb.CreateCellStyle();
            IFont font1 = wb.CreateFont();//字体
            font1.FontName = "楷体";
            font1.Color = NPOI.HSSF.Util.HSSFColor.White.Index;//字体颜色
            font1.Boldweight = (short)FontBoldWeight.Normal;//字体加粗
            font1.FontHeight = 17;
            titleStyle.SetFont(font1);//设置字体
            //设置背景色
            titleStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Blue.Index;
            titleStyle.FillPattern = FillPattern.SolidForeground;
            titleStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.Blue.Index;
            titleStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;//文字水平对齐方式
            titleStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;//文字垂直对齐方式

            //创建一个表单
            ISheet sheet = wb.CreateSheet("Sheet0");
            //设置列宽
            int[] columnWidth = { 10, 10, 25, 10, 15, 15, 20, 10, 20, 20, 15, 15, 30 };
            for (int i = 0; i < columnWidth.Length; i++)
            {
                //设置列宽度，256*字符数，因为单位是1/256个字符
                sheet.SetColumnWidth(i, 256 * columnWidth[i]);
            }

            IRow row;
            NPOI.SS.UserModel.ICell cell;


            string[] fieldArr = { "TimeNow", "ProductName", "ProductModel", "Unit", "SalesUnitPrice", "Quantity", "SalesTotalPrice", "UnitPrice", "Amount", "Name", "InvoiceNo", "UpdateDate", "TaxAmount", "OrderID" };
            System.Collections.Generic.Dictionary<string, string> fieldDic = new System.Collections.Generic.Dictionary<string, string> { { "TimeNow", "日期" }, { "ProductName", "品名" },
                { "ProductModel", "型号" }, { "Unit", "计量单位" }, { "SalesUnitPrice", "不含税单价" }, { "Quantity", "数量" }, { "SalesTotalPrice", "不含税金额" }, { "UnitPrice", "含税单价" }, { "Amount", "含税金额" },
                { "Name", "开票公司" }, { "InvoiceNo", "发票号" }, { "UpdateDate", "开票日期" }, { "TaxAmount", "税额" },{ "OrderID","订单号"} };

            //写入标题行
            row = sheet.CreateRow(0);
            for (int i = 0; i < fieldArr.Length; i++)
            {
                cell = row.CreateCell(i);//创建第j列
                cell.CellStyle = titleStyle;
                SetCellValue(cell, fieldDic[fieldArr[i]]);
            }

            //写入数据，从第2行开始
            for (int i = 1; i <= dt.Rows.Count; i++)
            {
                row = sheet.CreateRow(i);//创建第i行
                for (int j = 0; j < fieldArr.Length; j++)
                {
                    cell = row.CreateCell(j);

                    //根据数据类型设置不同类型的cell
                    var field = fieldArr[j];
                    object obj = null;
                    //如果报错，跳过该字段
                    try
                    {
                        obj = dt.Rows[i - 1][field];
                    }
                    catch
                    {
                        continue;
                    }
                    if (obj != null)
                    {
                        if (j == 4 || j == 5 || j == 7 || j == 8 || j == 12)
                        {
                            SetCellValue(cell, Convert.ToDouble(obj));
                        }
                        else if (j == 6)
                        {
                            //不含税金额保留两位小数
                            SetCellValue(cell, decimal.Round(decimal.Parse(Convert.ToString(obj)), 2));
                        }
                        else
                        {
                            SetCellValue(cell, obj);
                        }
                    }
                }
            }


            FileStream file = new FileStream(savePath, FileMode.Create);
            wb.Write(file);
            file.Close();
        }

        /// <summary>
        /// 根据数据类型设置不同类型的cell
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="obj"></param>
        public static void SetCellValue(NPOI.SS.UserModel.ICell cell, object obj)
        {
            try
            {
                if (obj.GetType() == typeof(int))
                {
                    cell.SetCellValue((int)obj);
                }
                else if (obj.GetType() == typeof(double))
                {
                    cell.SetCellValue((double)obj);
                }
                else if (obj.GetType() == typeof(IRichTextString))
                {
                    cell.SetCellValue((IRichTextString)obj);
                }
                else if (obj.GetType() == typeof(string))
                {
                    cell.SetCellValue(obj.ToString());
                }
                else if (obj.GetType() == typeof(DateTime))
                {
                    cell.SetCellValue(Convert.ToDateTime(obj).ToString("yyyy/MM/dd"));
                }
                else if (obj.GetType() == typeof(bool))
                {
                    cell.SetCellValue((bool)obj);
                }
                else
                {
                    cell.SetCellValue(obj.ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
