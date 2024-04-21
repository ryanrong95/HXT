using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Reflection;
using System.Text.RegularExpressions;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Underly;
using Yahv.Utils.Validates;
using Yahv.Underly.Enums;

namespace Yahv.Erm.Services
{
    /// <summary>
    /// 工资导出
    /// </summary>
    public class ExportWages
    {
        #region 单例
        static object locker = new object();
        static ExportWages current;
        static public ExportWages Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new ExportWages();
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
        public string MakeExportExcel(string exportName, DataRow[] dataRows, DataColumnCollection dataColumns, List<string> wageItems, List<string> wageItemsRemove = null, string fixedColumns = "序号,地区,所属公司,分公司,部门,ID（工号）,姓名,考核岗位,身份证号码,员工状态")
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
                var columns = fixedColumns.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                //去除不包含的项
                if (wageItemsRemove != null)
                {
                    columns = columns.Where(item => !wageItemsRemove.Contains(item)).ToList();
                    wageItems = wageItems.Where(item => !wageItemsRemove.Contains(item)).ToList();
                }

                //遍历循环添加标题
                foreach (var column in columns)
                {
                    curCell = header.CreateCell(col_index++);
                    curCell.CellStyle = headStyle;
                    curCell.SetCellValue(column);
                }

                //动态列
                foreach (var wi in wageItems)
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

                        if (head == "地区" && dataColumns.Contains("City"))
                        {
                            curCell.SetCellValue(dr["City"].ToString());
                            continue;
                        }

                        if (head == "所属公司" && dataColumns.Contains("CompanyName"))
                        {
                            curCell.SetCellValue(dr["CompanyName"].ToString());
                            continue;
                        }

                        if (head == "分公司" && dataColumns.Contains("DyjCompanyCode"))
                        {
                            curCell.SetCellValue(dr["DyjCompanyCode"].ToString());
                            continue;
                        }

                        if (head == "部门" && dataColumns.Contains("DyjDepartmentCode"))
                        {
                            curCell.SetCellValue(dr["DyjDepartmentCode"].ToString());
                            continue;
                        }

                        if (head == "ID（工号）" && dataColumns.Contains("DyjCode"))
                        {
                            curCell.SetCellValue(dr["DyjCode"].ToString());
                            continue;
                        }

                        if (head == "姓名" && dataColumns.Contains("StaffName"))
                        {
                            curCell.SetCellValue(dr["StaffName"].ToString());
                            continue;
                        }

                        if (head == "考核岗位" && dataColumns.Contains("PostionName"))
                        {
                            curCell.SetCellValue(dr["PostionName"].ToString());
                            continue;
                        }

                        if (head == "身份证号码" && dataColumns.Contains("IDCard"))
                        {
                            curCell.SetCellValue(dr["IDCard"].ToString());
                            continue;
                        }


                        if (head == "银行卡号" && dataColumns.Contains("BankAccount"))
                        {
                            curCell.SetCellValue(dr["BankAccount"].ToString());
                            continue;
                        }

                        if (head == "员工状态" && dataColumns.Contains("StaffStatusName"))
                        {
                            curCell.SetCellValue(dr["StaffStatusName"].ToString());
                            continue;
                        }

                        if (head == "工资日期" && dataColumns.Contains("DateIndex"))
                        {
                            curCell.SetCellValue(dr["DateIndex"].ToString());
                            continue;
                        }

                        //foreach (var column in dataColumns)
                        //{
                        //    if (head == column.ToString().Trim())
                        //    {
                        //        if (dr[column.ToString()].ToString().IsNumber())
                        //        {
                        //            curCell
                        //                .SetCellValue(Convert.ToDouble(dr[column.ToString()].ToString()));
                        //        }
                        //        else
                        //        {
                        //            curCell
                        //                .SetCellValue(dr[column.ToString()].ToString());
                        //        }

                        //        break;
                        //    }
                        //}

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
        /// 生成会计Excel
        /// </summary>
        /// <param name="tempFile">模板文件</param>
        /// <param name="exportName">导出文件名称</param>
        /// <param name="dataRows">源数据</param>
        /// <param name="dataColumns">列集合</param>
        /// <returns>excel文件路径</returns>
        public string MakeFinanceExcel(string tempFile, string exportName, DataRow[] dataRows, DataColumnCollection dataColumns)
        {
            string fileName = string.Empty;

            FileStream fs = null;
            FileStream fsExport = null;

            try
            {
                string fileExt = Path.GetExtension(tempFile).ToLower(); //文件后缀
                using (fs = new FileStream(tempFile, FileMode.Open, FileAccess.Read))
                {
                    IWorkbook workbook;
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

                    //设置标题 2019年4月员工工资表明细
                    //IRow header1 = sheet.GetRow(0); //获取标头
                    //string date = wageDate.Value.Replace("-", "年") + "月";
                    //header1.Cells[0].SetCellValue(date + GetValue(header1.GetCell(0)));

                    //遍历循环数据源
                    int headRowCount = 2; //标题行数
                    IRow headRow2 = sheet.GetRow(headRowCount - 2); //标题行
                    IRow headRow3 = sheet.GetRow(headRowCount - 1); //标题行
                    IRow curRow; //当前行
                    ICell curCell; //当前单元
                    DataRow dr;
                    string head = string.Empty; //标题

                    //样式
                    var cellstyle = GetCellStyle(workbook);

                    for (int i = 0; i < dataRows.Length; i++)
                    {
                        dr = dataRows[i];
                        curRow = sheet.CreateRow(i + headRowCount);

                        for (int m = 0; m < headRow3.LastCellNum; m++)
                        {
                            head = (GetValue(headRow3.GetCell(m)) ?? GetValue(headRow2.GetCell(m)))?.ToString();
                            curCell = curRow.CreateCell(m);
                            curCell.CellStyle = cellstyle;

                            if (head == "序号")
                            {
                                curCell.SetCellValue((i + 1));
                                continue;
                            }

                            if (head == "地区")
                            {
                                curCell.SetCellValue(dr["City"].ToString());
                                continue;
                            }

                            if (head == "所属公司")
                            {
                                curCell.SetCellValue(dr["CompanyName"].ToString());
                                continue;
                            }

                            if (head == "分公司")
                            {
                                curCell.SetCellValue(dr["DyjCompanyCode"].ToString());
                                continue;
                            }

                            if (head == "部门")
                            {
                                curCell.SetCellValue(dr["DyjDepartmentCode"].ToString());
                                continue;
                            }

                            if (head == "ID（工号）")
                            {
                                curCell.SetCellValue(dr["DyjCode"].ToString());
                                continue;
                            }

                            if (head == "姓名")
                            {
                                curCell.SetCellValue(dr["StaffName"].ToString());
                                continue;
                            }

                            if (head == "考核岗位")
                            {
                                curCell.SetCellValue(dr["PostionName"].ToString());
                                continue;
                            }

                            if (head == "身份证号码")
                            {
                                curCell.SetCellValue(dr["IDCard"].ToString());
                                continue;
                            }


                            if (head == "银行卡号")
                            {
                                curCell.SetCellValue(dr["BankAccount"].ToString());
                                continue;
                            }


                            //foreach (var column in dataColumns)
                            //{
                            //    if (head == column.ToString().Trim())
                            //    {
                            //        if (dr[column.ToString()].ToString().IsNumber())
                            //        {
                            //            curCell
                            //                .SetCellValue(Convert.ToDouble(dr[column.ToString()].ToString()));
                            //        }
                            //        else
                            //        {
                            //            curCell
                            //                .SetCellValue(dr[column.ToString()].ToString());
                            //        }

                            //        break;
                            //    }
                            //}

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

                    fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Upload\") + exportName;
                    using (fsExport = new FileStream(fileName, FileMode.Create))
                    {
                        workbook.Write(fsExport);
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                fs?.Close();
                fsExport?.Close();
            }

            return fileName;
        }

        /// <summary>
        /// 生成出纳Excel
        /// </summary>
        /// <param name="exportName">导出文件名称</param>
        /// <param name="dataRows">源数据</param>
        /// <param name="dataColumns">列集合</param>
        /// <returns>excel文件路径</returns>
        public string MakeCashierExcel(string exportName, DataRow[] dataRows)
        {
            string fileName = string.Empty;

            FileStream fsExport = null;

            try
            {
                HSSFWorkbook workbook = new HSSFWorkbook();
                ISheet sheet = workbook.CreateSheet();
                IRow curRow; //当前行
                ICell curCell; //当前单元

                //设置标题 2019年4月员工工资表明细
                IRow header = sheet.CreateRow(0); //获取标头
                int col_index = 0;
                var headStyle = GetHeadStyle(workbook);     //head样式

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("序号");
                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("地区");
                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("所属公司");
                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("ID");
                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("姓名");
                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("实发工资");
                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("银行卡号");

                //遍历循环数据源
                DataRow dr;
                string head = string.Empty; //标题

                //样式
                var cellstyle = GetCellStyle(workbook);

                for (int i = 0; i < dataRows.Length; i++)
                {
                    curRow = sheet.CreateRow(i + 1);
                    dr = dataRows[i];

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
                            case "地区":
                                curCell.SetCellValue(dr["City"].ToString());
                                break;
                            case "所属公司":
                                curCell.SetCellValue(dr["CompanyName"].ToString());
                                break;
                            case "ID":
                                if (dr["DyjCode"].ToString().IsNumber())
                                    curCell.SetCellValue(int.Parse(dr["DyjCode"].ToString()));
                                else
                                    curCell.SetCellValue(dr["DyjCode"].ToString());
                                break;
                            case "姓名":
                                curCell.SetCellValue(dr["StaffName"].ToString());
                                break;
                            case "实发工资":
                                if (dr["实发工资"].ToString().IsNumber())
                                    curCell.SetCellValue(double.Parse(dr["实发工资"].ToString()));
                                else
                                    curCell.SetCellValue(dr["实发工资"].ToString());
                                break;
                            case "银行卡号":
                                curCell.SetCellValue(dr["BankAccount"].ToString());
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

                fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Upload\") + exportName;
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
        /// 生成大赢家Excel
        /// </summary>
        /// <param name="exportName">导出文件名称</param>
        /// <param name="dataRows">源数据</param>
        /// <param name="dataColumns">列集合</param>
        /// <returns>excel文件路径</returns>
        public string MakeDYJExcel(string exportName, DataRow[] dataRows, string date)
        {
            string fileName = string.Empty;

            FileStream fs = null;
            FileStream fsExport = null;

            try
            {
                HSSFWorkbook workbook = new HSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("Sheet1");

                IRow curRow; //当前行
                ICell curCell; //当前单元

                //设置标题 2019年4月员工工资表明细
                IRow header = sheet.CreateRow(0); //获取标头
                int col_index = 0;
                var headStyle = GetHeadStyle(workbook);     //head样式

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("摘要");
                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("付款类型");
                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("员工ID");
                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("金额");
                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("备注");

                //遍历循环数据源
                DataRow dr;
                string head = string.Empty; //标题

                //样式
                var cellstyle = GetCellStyle(workbook);

                for (int i = 0; i < dataRows.Length; i++)
                {
                    curRow = sheet.CreateRow(i + 1);
                    dr = dataRows[i];

                    for (int m = 0; m < header.LastCellNum; m++)
                    {
                        head = (GetValue(header.GetCell(m)))?.ToString();
                        curCell = curRow.CreateCell(m);
                        curCell.CellStyle = cellstyle;


                        if (head == "摘要")
                        {

                            curCell.SetCellValue(date + dr["City"].ToString() + "地区工资");
                            continue;
                        }

                        if (head == "付款类型")
                        {
                            curCell.SetCellValue("工资");
                            continue;
                        }

                        if (head == "员工ID")
                        {
                            if (dr["DyjCode"].ToString().IsNumber())
                                curCell.SetCellValue(int.Parse(dr["DyjCode"].ToString()));
                            else
                                curCell.SetCellValue(dr["DyjCode"].ToString());
                            continue;
                        }

                        if (head == "金额")
                        {
                            if (dr["实发工资"].ToString().IsNumber())
                                curCell.SetCellValue(double.Parse(dr["实发工资"].ToString()));
                            else
                                curCell.SetCellValue(dr["实发工资"].ToString());
                            continue;
                        }

                        if (head == "备注")
                        {
                            curCell.SetCellValue(dr["CompanyName"].ToString());
                            continue;
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
            catch (Exception ex)
            {
            }
            finally
            {
                fs?.Close();
                fsExport?.Close();
            }

            return fileName;
        }

        public string MakeErrorExcel(string fileName, Dictionary<int, string> errorMsgs, int titleIndex = 1)
        {
            string result = string.Empty;

            IWorkbook workbook = null;

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

                string errorFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Upload\") + "error" + DateTime.Now.ToString("yyyyMMddHHmmss") +
                                   fileExt;
                using (FileStream errorExcel = new FileStream(errorFile, FileMode.Create))
                {
                    workbook.Write(errorExcel);
                    errorExcel.Close();

                    result = errorFile;
                }
            }

            return result;
        }

        /// <summary>
        /// 生成员工Excel
        /// </summary>
        /// <param name="exportName"></param>
        /// <param name="dataRows"></param>
        /// <returns></returns>
        public string MakeStaffsExcel(string exportName, Staff[] staffs)
        {
            string fileName = string.Empty;

            FileStream fsExport = null;

            try
            {
                HSSFWorkbook workbook = new HSSFWorkbook();
                ISheet sheet = workbook.CreateSheet();
                IRow curRow; //当前行
                ICell curCell; //当前单元

                //设置标题 2019年4月员工工资表明细
                IRow header = sheet.CreateRow(0); //获取标头
                int col_index = 0;
                var headStyle = GetHeadStyle(workbook);     //head样式

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("序号");
                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("地区");
                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("所属公司");
                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("分公司");
                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("部门");
                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("ID（工号）");
                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("姓名");
                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("考核岗位");
                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("身份证号码");
                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("所属银行");
                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("银行卡号");
                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("状态");

                //遍历循环数据源
                Staff staff;
                string head = string.Empty; //标题

                //样式
                var cellstyle = GetCellStyle(workbook);

                for (int i = 0; i < staffs.Length; i++)
                {
                    curRow = sheet.CreateRow(i + 1);
                    staff = staffs[i];

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
                            case "地区":
                                curCell.SetCellValue(staff.CityName);
                                break;
                            case "所属公司":
                                curCell.SetCellValue(staff.EnterpriseCompany);
                                break;
                            case "分公司":
                                curCell.SetCellValue(staff.DyjCompanyCode);
                                break;
                            case "部门":
                                curCell.SetCellValue(staff.DyjDepartmentCode);
                                break;
                            case "ID（工号）":
                                curCell.SetCellValue(staff.DyjCode);
                                break;
                            case "姓名":
                                curCell.SetCellValue(staff.Name);
                                break;
                            case "考核岗位":
                                curCell.SetCellValue(staff.PostionName);
                                break;
                            case "身份证号码":
                                curCell.SetCellValue(staff.IDCard);
                                break;
                            case "所属银行":
                                curCell.SetCellValue(staff.BankName);
                                break;
                            case "银行卡号":
                                curCell.SetCellValue(staff.BankAccount);
                                break;
                            case "状态":
                                curCell.SetCellValue(staff.Status.GetDescription());
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

                fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Upload\") + exportName;
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

                //设置标题 2019年4月员工工资表明细
                IRow header = sheet.CreateRow(0); //获取标头
                var headStyle = GetHeadStyle(workbook);     //head样式

                for (int i = 0; i < titles.Length; i++)
                {
                    curCell = header.CreateCell((i));
                    curCell.CellStyle = headStyle;
                    curCell.SetCellValue(titles[i]);

                    sheet.AutoSizeColumn(i);
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

        /// <summary>
        /// 生成员工考勤Excel
        /// </summary>
        /// <param name="exportName"></param>
        /// <param name="dataRows"></param>
        /// <returns></returns>
        public string MakeAttendsExcel(string exportName, PastsAttend[] attends)
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
                curCell.SetCellValue("ID");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("员工");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("工作日");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("正常");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("迟到");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("早退");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("旷工");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("事假");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("病假");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("公差");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("公务");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("带薪假");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("补签");

                curCell = header.CreateCell(col_index++);
                curCell.CellStyle = headStyle;
                curCell.SetCellValue("加班");

                //遍历循环数据源
                var staffIds = attends.Select(item => item.StaffID).Distinct().ToArray();
                var staffs = new Yahv.Erm.Services.Views.StaffAlls().Where(item => staffIds.Contains(item.ID)).ToArray();

                string head = string.Empty; //标题

                //样式
                var cellstyle = GetCellStyle(workbook);

                Staff staff;
                string staffId = string.Empty;
                int rowIndex = 0;       //行数

                for (int i = 0; i < staffIds.Length; i++)
                {
                    staffId = staffIds[i];
                    staff = staffs.SingleOrDefault(item => item.ID == staffId);
                    if (staff == null)
                    {
                        continue;
                    }

                    curRow = sheet.CreateRow(rowIndex + 1);

                    for (int m = 0; m < header.LastCellNum; m++)
                    {
                        head = (GetValue(header.GetCell(m)))?.ToString();
                        curCell = curRow.CreateCell(m);
                        curCell.CellStyle = cellstyle;

                        switch (head)
                        {
                            case "ID":
                                curCell.SetCellValue(staff.DyjCode);
                                break;
                            case "员工":
                                curCell.SetCellValue(staff.Name);
                                break;
                            case "所属公司":
                                curCell.SetCellValue(staff.Labour?.EntryCompany);
                                break;
                            case "工作日":
                                curCell.SetCellValue(attends.Where(item => item.StaffID == staffId && (item.InFact != AttendInFactType.LegalHolidays && item.InFact != AttendInFactType.PublicHoliday &&
                        item.InFact != AttendInFactType.Overtime)).Count() * 0.5);
                                break;
                            case "正常":
                                curCell.SetCellValue(attends.Where(item => item.StaffID == staffId && (item.InFact == AttendInFactType.Normal || item.InFact == AttendInFactType.SystemAuthorizing)).Count() * 0.5);
                                break;
                            case "迟到":
                                curCell.SetCellValue(attends.Where(item => item.StaffID == staffId && item.IsLater == true).Count());
                                break;
                            case "早退":
                                curCell.SetCellValue(attends.Where(item => item.StaffID == staffId && item.IsEarly == true).Count());
                                break;
                            case "旷工":
                                curCell.SetCellValue(attends.Where(item => item.StaffID == staffId && item.InFact == AttendInFactType.Absenteeism).Count() * 0.5);
                                break;
                            case "事假":
                                curCell.SetCellValue(attends.Where(item => item.StaffID == staffId && item.InFact == AttendInFactType.CasualLeave).Count() * 0.5);
                                break;
                            case "病假":
                                curCell.SetCellValue(attends.Where(item => item.StaffID == staffId && item.InFact == AttendInFactType.SickLeave).Count() * 0.5);
                                break;
                            case "补签":
                                curCell.SetCellValue(attends.Where(item => item.StaffID == staffId && item.OnWorkRemedy == true).Count() + attends.Where(item => item.StaffID == staffId && item.OffWorkRemedy == true).Count());
                                break;
                            case "公差":
                                curCell.SetCellValue(attends.Where(item => item.StaffID == staffId && item.InFact == AttendInFactType.BusinessTrip).Count() * 0.5);
                                break;
                            case "公务":
                                curCell.SetCellValue(attends.Where(item => item.StaffID == staffId && item.InFact == AttendInFactType.OfficialBusiness).Count() * 0.5);
                                break;
                            case "带薪假":
                                curCell.SetCellValue(attends.Where(item => item.StaffID == staffId && item.InFact == AttendInFactType.PaidLeave).Count() * 0.5);
                                break;
                            case "加班":
                                curCell.SetCellValue(attends.Where(item => item.StaffID == staffId && item.InFact == AttendInFactType.Overtime).Count() * 0.5);
                                break;
                            default:
                                break;
                        }
                    }
                    rowIndex++;
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
        /// 动态Linq方式实现行转列
        /// </summary>
        /// <param name="list">数据</param>
        /// <param name="DimensionList">固定列</param>
        /// <param name="DynamicColumn">动态列</param>
        /// <param name="NumColumn">计算列</param>
        /// <returns>行转列后数据</returns>
        public List<dynamic> DynamicLinq<T>(List<T> list, List<string> DimensionList, string DynamicColumn, string NumColumn, out List<string> AllDynamicColumn) where T : class
        {
            //获取所有动态列
            var columnGroup = list.GroupBy(DynamicColumn, "new(it as Vm)") as IEnumerable<IGrouping<dynamic, dynamic>>;
            List<string> AllColumnList = new List<string>();
            foreach (var item in columnGroup)
            {
                if (!string.IsNullOrEmpty(item.Key))
                {
                    AllColumnList.Add(item.Key);
                }
            }
            AllDynamicColumn = AllColumnList;
            var dictFunc = new Dictionary<string, Func<T, bool>>();
            foreach (var column in AllColumnList)
            {
                var func = DynamicExpression.ParseLambda<T, bool>(string.Format("{0}==\"{1}\"", DynamicColumn, column)).Compile();
                dictFunc[column] = func;
            }
            //获取实体所有属性
            Dictionary<string, PropertyInfo> PropertyInfoDict = new Dictionary<string, PropertyInfo>();
            Type type = typeof(T);
            var propertyInfos = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            //数值列
            List<string> AllNumberField = new List<string>() { NumColumn };
            foreach (var item in propertyInfos)
            {
                PropertyInfoDict[item.Name] = item;
                //if (item.PropertyType == typeof(int) || item.PropertyType == typeof(double) || item.PropertyType == typeof(float) || item.PropertyType == typeof(decimal))
                //{
                //    AllNumberField.Add(item.Name);
                //}
            }
            //分组
            IEnumerable<IGrouping<dynamic, dynamic>> dataGroup = list.GroupBy(string.Format("new ({0})", string.Join(",", DimensionList)), "new(it as Vm)") as IEnumerable<IGrouping<dynamic, dynamic>>;

            List<dynamic> listResult = new List<dynamic>();
            IDictionary<string, object> itemObj = null;
            T vm2 = default(T);
            foreach (var group in dataGroup)
            {
                itemObj = new ExpandoObject();
                var listVm = group.Select(e => e.Vm as T).ToList();
                //维度列赋值
                vm2 = listVm.FirstOrDefault();
                foreach (var key in DimensionList)
                {
                    itemObj[key] = PropertyInfoDict[key].GetValue(vm2);
                }
                foreach (var column in AllColumnList)
                {
                    vm2 = listVm.FirstOrDefault(dictFunc[column]);
                    if (vm2 != null)
                    {
                        foreach (string name in AllNumberField)
                        {
                            //itemObj[name + column] = PropertyInfoDict[name].GetValue(vm2);
                            itemObj[column] = PropertyInfoDict[name].GetValue(vm2);
                        }
                    }
                }
                listResult.Add(itemObj);
            }
            return listResult;
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

        /// <summary>
        /// 获取Excel标头
        /// </summary>
        /// <param name="file"></param>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        public List<string> GetExcelHead(string file, string rowsIndex = "1")
        {
            List<string> result = new List<string>();

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

                var rows = rowsIndex.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var row in rows)
                {
                    //表头(第二行为表头，如果第二行没有数据，直接取第一行)
                    IRow header = sheet.GetRow(int.Parse(row) - 1);

                    List<int> columns = new List<int>();
                    for (int i = 0; i < header.LastCellNum; i++)
                    {
                        var obj = GetValue(header.GetCell(i));
                        if (obj != null && obj.ToString() != string.Empty)
                        {
                            result.Add(obj.ToString());
                        }
                    }
                }
            }

            return result;
        }
        #endregion
    }
}