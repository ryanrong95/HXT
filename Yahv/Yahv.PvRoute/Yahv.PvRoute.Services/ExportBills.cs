using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Yahv.PvRoute.Services.Enums;
using Yahv.Underly;
using Yahv.Utils.Validates;

namespace Yahv.PvRoute.Services
{
    public class ExportBills
    {

        #region 单例

        static object locker = new object();

        static ExportBills current;

        static public ExportBills Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new ExportBills();
                        }
                    }
                }

                return current;
            }
        }
        #endregion

        #region 模板导出

        /// <summary>
        /// 生成普通Excel
        /// </summary>
        /// <param name="exportName">导出文件名称</param>
        /// <param name="dataRows">源数据</param>
        /// <param name="dataColumns">列集合</param>
        /// <returns>excel文件路径</returns>
        public string MakeExportExcel(string exportName, DataRow[] dataRows, DataColumnCollection dataColumns, List<string> bills, List<string> billsRemove = null)
        {
            string fileName = string.Empty;

            FileStream fsExport = null;

            try
            {
                HSSFWorkbook workbook = new HSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("Sheet1");
                IRow curRow; //当前行
                ICell curCell; //当前单元

                //设置标题
                IRow header = sheet.CreateRow(0); //获取标头
                int col_index = 0;
                var headStyle = GetHeadStyle(workbook);     //head样式

                //固定列
                var columns = new List<string>()
                {
                    "序号"
                };
                //去除不包含的项
                if (billsRemove != null)
                {
                    columns = columns.Where(item => !billsRemove.Contains(item)).ToList();
                    bills = bills.Where(item => !billsRemove.Contains(item)).ToList();
                }

                //遍历循环添加标题
                foreach (var column in columns)
                {
                    curCell = header.CreateCell(col_index++);
                    curCell.CellStyle = headStyle;
                    curCell.SetCellValue(column);
                }

                //动态列
                foreach (var wi in bills)
                {
                    curCell = header.CreateCell(col_index++);
                    curCell.CellStyle = headStyle;
                    curCell.SetCellValue(wi);
                }

                //遍历循环数据源
                int headRowCount = 1; //标题行数
                IRow headRow = sheet.GetRow(headRowCount - 1); //标题行
                DataRow dr;
                string head = string.Empty; //标题

                //样式
                var cellstyle = GetCellStyle(workbook);

                for (int i = 0; i < dataRows.Length; i++)
                {
                    dr = dataRows[i];
                    curRow = sheet.CreateRow(i + headRowCount);

                    for (int m = 0; m < headRow.LastCellNum; m++)
                    {
                        head = (GetValue(headRow.GetCell(m)))?.ToString();
                        curCell = curRow.CreateCell(m);
                        curCell.CellStyle = cellstyle;

                        if (head == "序号")
                        {
                            curCell.SetCellValue((i + 1));
                            continue;
                        }

                        if (head == "快递单号")
                        {
                            curCell.SetCellValue(dr["FaceOrderID"].ToString());
                            continue;
                        }

                        if (head == "件数")
                        {
                            curCell.SetCellValue(dr["Quantity"].ToString());
                            continue;
                        }

                        if (head == "重量")
                        {
                            curCell.SetCellValue(dr["Weight"].ToString());
                            continue;
                        }

                        if (head == "金额")
                        {
                            curCell.SetCellValue(dr["Price"].ToString());
                            continue;
                        }

                        if (head == "币种")
                        {
                            curCell.SetCellValue(dr["Currency"].ToString());
                            continue;
                        }

                        if (head == "承运商")
                        {
                            var carrier = ((PrintSource)(int.Parse(dr["Carrier"].ToString()))).GetDescription();
                            curCell.SetCellValue(carrier);
                            continue;
                        }

                        if (head == "核对人")
                        {
                            curCell.SetCellValue(dr["Checker"].ToString());
                            continue;
                        }


                        if (head == "核对时间")
                        {
                            curCell.SetCellValue(dr["CheckTime"].ToString());
                            continue;
                        }

                        if (head == "审核人")
                        {
                            curCell.SetCellValue(dr["Reviewer"].ToString());
                            continue;
                        }

                        if (head == "审核时间")
                        {
                            curCell.SetCellValue(dr["ReviewTime"].ToString());
                            continue;
                        }

                        if (head == "出纳人")
                        {
                            curCell.SetCellValue(dr["Cashier"].ToString());
                            continue;
                        }

                        if (head == "出纳时间")
                        {
                            curCell.SetCellValue(dr["CashierTime"].ToString());
                            continue;
                        }
                        if (head == "期号")
                        {
                            curCell.SetCellValue(dr["DateIndex"].ToString());
                            continue;
                        }

                        if (dataColumns.Contains(head))
                        {
                            if (dr[head].ToString().IsNumber())
                            {
                                curCell
                                    .SetCellValue(Convert.ToDouble(dr[head].ToString()));
                            }
                            else
                            {
                                curCell
                                    .SetCellValue(dr[head].ToString());
                            }
                        }
                    }

                }

                //自适应宽度
                for (int n = 0; n < col_index; n++)
                {
                    sheet.AutoSizeColumn(n);
                }

                fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Upload\") + exportName;
                using (fsExport = new FileStream(fileName, FileMode.Create))
                {
                    workbook.Write(fsExport);
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                fsExport?.Close();
            }

            return fileName;
        }

        /// <summary>
        /// 生成模板
        /// </summary>
        /// <param name="titles">标题名称</param>
        /// <returns></returns>
        public string MakeTemplateExcel(string[] titles)
        {
            string fileName = string.Empty;

            FileStream fsExport = null;

            try
            {
                HSSFWorkbook workbook = new HSSFWorkbook();
                ISheet sheet = workbook.CreateSheet();
                ICell curCell; //当前单元

                //设置标题 
                IRow header = sheet.CreateRow(0); //获取标头
                var headStyle = GetHeadStyle(workbook);     //head样式

                for (int i = 0; i < titles.Length; i++)
                {
                    curCell = header.CreateCell((i));
                    curCell.CellStyle = headStyle;
                    curCell.SetCellValue(titles[i]);

                    sheet.AutoSizeColumn(i);
                }

                var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Upload\");
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Upload\") + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";

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

        /// <summary>
        /// json转DataTable
        /// </summary>
        /// <param name="strJson"></param>
        /// <returns></returns>
        public DataTable JsonToDataTable(string strJson)
        {
            //转换json格式
            strJson = strJson.Replace(",\"", "*\"").Replace("\":", "\"#").ToString();
            //取出表名   
            var rg = new Regex(@"(?<={)[^:]+(?=:\[)", RegexOptions.IgnoreCase);
            //string strName = rg.Match(strJson).Value;
            string strName = "";
            DataTable tb = null;
            //去除表名   
            if (strJson.Contains("["))
                strJson = strJson.Substring(strJson.IndexOf("[") + 1);
            if (strJson.Contains("]"))
                strJson = strJson.Substring(0, strJson.IndexOf("]"));

            //获取数据   
            rg = new Regex(@"(?<={)[^}]+(?=})");
            MatchCollection mc = rg.Matches(strJson);
            for (int i = 0; i < mc.Count; i++)
            {
                string strRow = mc[i].Value;
                string[] strRows = strRow.Split('*');

                //创建表   
                if (tb == null)
                {
                    tb = new DataTable();
                    tb.TableName = strName;
                    foreach (string str in strRows)
                    {
                        var dc = new DataColumn();
                        string[] strCell = str.Split('#');

                        if (strCell[0].Substring(0, 1) == "\"")
                        {
                            int a = strCell[0].Length;
                            dc.ColumnName = strCell[0].Substring(1, a - 2);
                        }
                        else
                        {
                            dc.ColumnName = strCell[0];
                        }
                        tb.Columns.Add(dc);
                    }
                    tb.AcceptChanges();
                }

                //列个数不同时
                if (tb.Columns.Count < strRows.Length)
                {
                    foreach (string str in strRows)
                    {
                        var dc = new DataColumn();
                        string[] strCell = str.Split('#');

                        if (strCell[0].Substring(0, 1) == "\"")
                        {
                            int a = strCell[0].Length;
                            dc.ColumnName = strCell[0].Substring(1, a - 2);
                        }
                        else
                        {
                            dc.ColumnName = strCell[0];
                        }

                        if (!tb.Columns.Contains(dc.ColumnName))
                            tb.Columns.Add(dc);
                    }
                    tb.AcceptChanges();
                }

                //增加内容   
                DataRow dr = tb.NewRow();
                for (int r = 0; r < strRows.Length; r++)
                {
                    try
                    {
                        string columnName = strRows[r].Split('#')[0].Trim().Replace("\"", "");
                        string a = strRows[r].Split('#')[1].Trim();

                        //如果不包含这一列，重新添加（前边根据列个数判断不严谨，这里进行双重判断）
                        if (!tb.Columns.Contains(columnName))
                        {
                            foreach (string str in strRows)
                            {
                                var dc = new DataColumn();
                                string[] strCell = str.Split('#');

                                if (strCell[0].Substring(0, 1) == "\"")
                                {
                                    int aa = strCell[0].Length;
                                    dc.ColumnName = strCell[0].Substring(1, aa - 2);
                                }
                                else
                                {
                                    dc.ColumnName = strCell[0];
                                }

                                if (!tb.Columns.Contains(dc.ColumnName))
                                    tb.Columns.Add(dc);
                            }
                            tb.AcceptChanges();

                            r = -1;
                            dr = tb.NewRow();
                            continue;
                        }

                        if (a.Equals("null"))
                        {
                            dr[columnName] = "";
                        }
                        else
                        {
                            dr[columnName] = strRows[r].Split('#')[1].Trim().Replace("，", ",").Replace("：", ":").Replace("\"", "");
                        }
                    }
                    catch (Exception e)
                    {

                        throw e;
                    }
                }
                tb.Rows.Add(dr);
                tb.AcceptChanges();
            }

            try
            {
                if (tb != null)
                {
                    return tb;
                }
                else
                {
                    throw new Exception("解析错误");
                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        #endregion
    }
}
