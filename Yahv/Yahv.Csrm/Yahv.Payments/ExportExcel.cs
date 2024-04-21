using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Yahv.Payments.Models;
using Yahv.Services.Models;

namespace Yahv.Payments
{
    /// <summary>
    /// 导出Excel
    /// </summary>
    public class ExportExcel
    {
        #region 单例

        static object locker = new object();
        static ExportExcel current;

        public static ExportExcel Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new ExportExcel();
                        }
                    }
                }

                return current;
            }
        }

        #endregion

        #region 导出模板
        /// <summary>
        /// 导出收入统计列表
        /// </summary>
        /// <param name="exportName"></param>
        /// <returns></returns>
        public string MakeIncomeExcel(string exportName, WmsStats[] items)
        {
            string fileName = string.Empty;

            FileStream fsExport = null;

            try
            {
                HSSFWorkbook workbook = new HSSFWorkbook();
                ISheet sheet = workbook.CreateSheet();
                IRow curRow; //当前行
                ICell curCell; //当前单元

                //设置标题
                IRow header = sheet.CreateRow(0); //获取标头
                int col_index = 0;
                var headStyle = GetHeadStyle(workbook);     //head样式

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("序号");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("订单号");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("付款公司");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("分类");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("科目");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("币种");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("记账");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("现金");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("创建时间");

                //遍历循环数据源
                WmsStats item;
                string head = string.Empty; //标题

                //样式
                var cellstyle = GetCellStyle(workbook);

                for (int i = 0; i < items.Length; i++)
                {
                    curRow = sheet.CreateRow(i + 1);
                    item = items[i];

                    for (int m = 0; m < header.LastCellNum; m++)
                    {
                        head = (GetValue(header.GetCell(m)))?.ToString();
                        curCell = curRow.CreateCell(m);
                        curCell.CellStyle = cellstyle;
                        switch (head)
                        {
                            case "序号":
                                curCell.SetCellValue((i + 1));
                                break;
                            case "订单号":
                                curCell.SetCellValue(item?.OrderID);
                                break;
                            case "付款公司":
                                curCell.SetCellValue(item?.PayerName);
                                break;
                            case "分类":
                                curCell.SetCellValue(item?.Catalog);
                                break;
                            case "科目":
                                curCell.SetCellValue(item?.Subject);
                                break;
                            case "币种":
                                curCell.SetCellValue(item?.CurrencyName);
                                break;
                            case "记账":
                                if (item?.LeftPrice != null)
                                {
                                    curCell.SetCellValue(double.Parse(item?.LeftPrice.ToString()));
                                }
                                break;
                            case "现金":
                                if (item?.RightPrice != null)
                                {
                                    curCell.SetCellValue(double.Parse(item?.RightPrice?.ToString()));
                                }
                                break;
                            case "创建时间":
                                curCell.SetCellValue(item?.CreateDate);
                                break;
                            default:
                                break;
                        }
                    }

                }

                //自适应宽度
                for (int n = 0; n < col_index; n++)
                {
                    sheet.AutoSizeColumn(n);
                }

                fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Export\") + exportName;
                using (fsExport = new FileStream(fileName, FileMode.Create))
                {
                    workbook.Write(fsExport);
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                fsExport?.Close();
            }

            return fileName;
        }

        /// <summary>
        /// 导出收入明细列表
        /// </summary>
        /// <param name="exportName"></param>
        /// <returns></returns>
        public string MakeIncomeDetailExcel(string exportName, WmsDetail[] items)
        {
            string fileName = string.Empty;

            FileStream fsExport = null;

            try
            {
                HSSFWorkbook workbook = new HSSFWorkbook();
                ISheet sheet = workbook.CreateSheet();
                IRow curRow; //当前行
                ICell curCell; //当前单元

                //设置标题
                IRow header = sheet.CreateRow(0); //获取标头
                int col_index = 0;
                var headStyle = GetHeadStyle(workbook);     //head样式

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("序号");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("订单号");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("客户名称");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("收款公司");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("分类");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("科目");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("币种");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("类型");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("应收");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("实收");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("操作人");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("操作时间");

                //遍历循环数据源
                WmsDetail item;
                string head = string.Empty; //标题

                //样式
                var cellstyle = GetCellStyle(workbook);

                for (int i = 0; i < items.Length; i++)
                {
                    curRow = sheet.CreateRow(i + 1);
                    item = items[i];

                    for (int m = 0; m < header.LastCellNum; m++)
                    {
                        head = (GetValue(header.GetCell(m)))?.ToString();
                        curCell = curRow.CreateCell(m);
                        curCell.CellStyle = cellstyle;
                        switch (head)
                        {
                            case "序号":
                                curCell.SetCellValue((i + 1));
                                break;
                            case "订单号":
                                curCell.SetCellValue(item?.OrderID);
                                break;
                            case "客户名称":
                                curCell.SetCellValue(item?.ClientName);
                                break;
                            case "收款公司":
                                curCell.SetCellValue(item?.PayeeName);
                                break;
                            case "分类":
                                curCell.SetCellValue(item?.Catalog);
                                break;
                            case "科目":
                                curCell.SetCellValue(item?.Subject);
                                break;
                            case "币种":
                                curCell.SetCellValue(item?.CurrencyName);
                                break;
                            case "操作人":
                                curCell.SetCellValue(item?.CreatorName);
                                break;
                            case "类型":
                                curCell.SetCellValue(item?.Type);
                                break;
                            case "应收":
                                if (item?.LeftPrice != null)
                                {
                                    curCell.SetCellValue(double.Parse(item?.LeftPrice.ToString()));
                                }
                                break;
                            case "实收":
                                if (item?.RightPrice != null)
                                {
                                    curCell.SetCellValue(double.Parse(item?.RightPrice?.ToString()));
                                }
                                break;
                            case "操作时间":
                                curCell.SetCellValue(item?.CreateDate);
                                break;
                            default:
                                break;
                        }
                    }

                }

                //自适应宽度
                for (int n = 0; n < col_index; n++)
                {
                    sheet.AutoSizeColumn(n);
                }

                fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Export\") + exportName;
                using (fsExport = new FileStream(fileName, FileMode.Create))
                {
                    workbook.Write(fsExport);
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                fsExport?.Close();
            }

            return fileName;
        }


        /// <summary>
        /// 导出支出统计列表
        /// </summary>
        /// <param name="exportName"></param>
        /// <returns></returns>
        public string MakePayExcel(string exportName, WmsStats[] items)
        {
            string fileName = string.Empty;

            FileStream fsExport = null;

            try
            {
                HSSFWorkbook workbook = new HSSFWorkbook();
                ISheet sheet = workbook.CreateSheet();
                IRow curRow; //当前行
                ICell curCell; //当前单元

                //设置标题
                IRow header = sheet.CreateRow(0); //获取标头
                int col_index = 0;
                var headStyle = GetHeadStyle(workbook);     //head样式

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("序号");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("订单号");

                //curCell = header.CreateCell(col_index++);
                //curCell.CellStyle = headStyle;
                //curCell.SetCellValue("客户名称");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("收款公司");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("分类");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("科目");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("币种");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("记账");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("现金");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("创建时间");

                //遍历循环数据源
                WmsStats item;
                string head = string.Empty; //标题

                //样式
                var cellstyle = GetCellStyle(workbook);

                for (int i = 0; i < items.Length; i++)
                {
                    curRow = sheet.CreateRow(i + 1);
                    item = items[i];

                    for (int m = 0; m < header.LastCellNum; m++)
                    {
                        head = (GetValue(header.GetCell(m)))?.ToString();
                        curCell = curRow.CreateCell(m);
                        curCell.CellStyle = cellstyle;
                        switch (head)
                        {
                            case "序号":
                                curCell.SetCellValue((i + 1));
                                break;
                            case "订单号":
                                curCell.SetCellValue(item?.OrderID);
                                break;
                            //case "客户名称":
                            //    curCell.SetCellValue(item?.ClientName);
                            //    break;
                            case "收款公司":
                                curCell.SetCellValue(item?.PayeeName);
                                break;
                            case "分类":
                                curCell.SetCellValue(item?.Catalog);
                                break;
                            case "科目":
                                curCell.SetCellValue(item?.Subject);
                                break;
                            case "币种":
                                curCell.SetCellValue(item?.CurrencyName);
                                break;
                            case "记账":
                                if (item?.LeftPrice != null)
                                {
                                    curCell.SetCellValue(double.Parse(item?.LeftPrice.ToString()));
                                }
                                break;
                            case "现金":
                                if (item?.RightPrice != null)
                                {
                                    curCell.SetCellValue(double.Parse(item?.RightPrice?.ToString()));
                                }
                                break;
                            case "创建时间":
                                curCell.SetCellValue(item?.CreateDate);
                                break;
                            default:
                                break;
                        }
                    }

                }

                //自适应宽度
                for (int n = 0; n < col_index; n++)
                {
                    sheet.AutoSizeColumn(n);
                }

                fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Export\") + exportName;
                using (fsExport = new FileStream(fileName, FileMode.Create))
                {
                    workbook.Write(fsExport);
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                fsExport?.Close();
            }

            return fileName;
        }

        /// <summary>
        /// 导出支出明细列表
        /// </summary>
        /// <param name="exportName"></param>
        /// <returns></returns>
        public string MakePayDetailExcel(string exportName, WmsDetail[] items)
        {
            string fileName = string.Empty;

            FileStream fsExport = null;

            try
            {
                HSSFWorkbook workbook = new HSSFWorkbook();
                ISheet sheet = workbook.CreateSheet();
                IRow curRow; //当前行
                ICell curCell; //当前单元

                //设置标题
                IRow header = sheet.CreateRow(0); //获取标头
                int col_index = 0;
                var headStyle = GetHeadStyle(workbook);     //head样式

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("序号");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("订单号");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("客户名称");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("付款公司");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("分类");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("科目");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("币种");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("记账");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("现金");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("操作人");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("操作时间");

                //遍历循环数据源
                WmsDetail item;
                string head = string.Empty; //标题

                //样式
                var cellstyle = GetCellStyle(workbook);

                for (int i = 0; i < items.Length; i++)
                {
                    curRow = sheet.CreateRow(i + 1);
                    item = items[i];

                    for (int m = 0; m < header.LastCellNum; m++)
                    {
                        head = (GetValue(header.GetCell(m)))?.ToString();
                        curCell = curRow.CreateCell(m);
                        curCell.CellStyle = cellstyle;
                        switch (head)
                        {
                            case "序号":
                                curCell.SetCellValue((i + 1));
                                break;
                            case "订单号":
                                curCell.SetCellValue(item?.OrderID);
                                break;
                            case "客户名称":
                                curCell.SetCellValue(item?.ClientName);
                                break;
                            case "付款公司":
                                curCell.SetCellValue(item?.PayerName);
                                break;
                            case "分类":
                                curCell.SetCellValue(item?.Catalog);
                                break;
                            case "科目":
                                curCell.SetCellValue(item?.Subject);
                                break;
                            case "币种":
                                curCell.SetCellValue(item?.CurrencyName);
                                break;
                            case "操作人":
                                curCell.SetCellValue(item?.CreatorName);
                                break;
                            case "记账":
                                if (item?.LeftPrice != null)
                                {
                                    curCell.SetCellValue(double.Parse(item?.LeftPrice.ToString()));
                                }
                                break;
                            case "现金":
                                if (item?.RightPrice != null)
                                {
                                    curCell.SetCellValue(double.Parse(item?.RightPrice?.ToString()));
                                }
                                break;
                            case "操作时间":
                                curCell.SetCellValue(item?.CreateDate);
                                break;
                            default:
                                break;
                        }
                    }

                }

                //自适应宽度
                for (int n = 0; n < col_index; n++)
                {
                    sheet.AutoSizeColumn(n);
                }

                fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Export\") + exportName;
                using (fsExport = new FileStream(fileName, FileMode.Create))
                {
                    workbook.Write(fsExport);
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                fsExport?.Close();
            }

            return fileName;
        }

        #endregion

        #region 公共方法
        /// <summary>
        /// 获取内容样式
        /// </summary>
        /// <param name="workbook"></param>
        /// <returns></returns>
        public ICellStyle GetCellStyle(IWorkbook workbook)
        {
            var cellstyle = workbook.CreateCellStyle();
            cellstyle.Alignment = HorizontalAlignment.Center;
            cellstyle.VerticalAlignment = VerticalAlignment.Center;

            //字体
            IFont font = workbook.CreateFont();
            font.FontName = "宋体";
            font.FontHeightInPoints = 9;

            cellstyle.SetFont(font);

            //有边框
            cellstyle.BorderBottom = BorderStyle.Thin;
            cellstyle.BorderLeft = BorderStyle.Thin;
            cellstyle.BorderRight = BorderStyle.Thin;
            cellstyle.BorderTop = BorderStyle.Thin;

            return cellstyle;
        }

        /// <summary>
        /// 获取标题样式
        /// </summary>
        /// <param name="workbook"></param>
        /// <returns></returns>
        public ICellStyle GetHeadStyle(IWorkbook workbook)
        {
            var cellstyle = workbook.CreateCellStyle();
            cellstyle.Alignment = HorizontalAlignment.Center;
            cellstyle.VerticalAlignment = VerticalAlignment.Center;

            //字体
            IFont font = workbook.CreateFont();
            font.FontName = "宋体";
            font.FontHeightInPoints = 12;
            font.IsBold = true;

            cellstyle.SetFont(font);

            //有边框
            cellstyle.BorderBottom = BorderStyle.Thin;
            cellstyle.BorderLeft = BorderStyle.Thin;
            cellstyle.BorderRight = BorderStyle.Thin;
            cellstyle.BorderTop = BorderStyle.Thin;

            //标题背景颜色
            cellstyle.FillPattern = FillPattern.SolidForeground;
            cellstyle.FillForegroundColor = HSSFColor.Grey25Percent.Index;

            return cellstyle;
        }

        /// <summary>
        /// Excel导入成Datable
        /// </summary>
        /// <param name="file">导入路径(包含文件名与扩展名)</param>
        /// <returns></returns>
        public DataTable ExcelToTable(string file)
        {
            DataTable dt = new DataTable();
            IWorkbook workbook;

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
                    workbook = new HSSFWorkbook(fs);
                }
                else
                {
                    workbook = null;
                }

                ISheet sheet = workbook.GetSheetAt(0);

                //表头(第二行为表头，如果第二行没有数据，直接取第一行)
                IRow header1 = sheet.GetRow(1);
                IRow header2 = sheet.GetRow(0);

                List<int> columns = new List<int>();
                for (int i = 0; i < header1.LastCellNum; i++)
                {
                    var obj1 = GetValue(header1.GetCell(i));
                    var obj2 = GetValue(header2.GetCell(i));

                    if (obj1 != null && obj1.ToString() != string.Empty && !dt.Columns.Contains(obj1.ToString()))
                    {
                        dt.Columns.Add(new DataColumn(obj1.ToString()));
                    }
                    else if (obj2 != null && obj2.ToString() != string.Empty && !dt.Columns.Contains(obj2.ToString()))
                    {
                        dt.Columns.Add(new DataColumn(obj2.ToString()));
                    }
                    else
                        dt.Columns.Add(new DataColumn("Columns" + i.ToString()));

                    columns.Add(i);
                }
                //数据  第三行开始
                for (int i = sheet.FirstRowNum + 2; i <= sheet.LastRowNum; i++)
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
        #endregion
    }
}