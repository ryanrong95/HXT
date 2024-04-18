using NPOI;
using NPOI.HPSF;
using NPOI.HSSF;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.POIFS;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NtErp.Crm.Services
{
    public class NPOIHelper
    {
        /// <summary>
        /// DataTable导出到Excel文件
        /// </summary>
        /// <param name="dtSource">源DataTable</param>
        /// <param name="strHeaderText">表头文本</param>
        /// <param name="strFileName">保存位置</param>
        public static void Export(DataTable dtSource, string strHeaderText, string strFileName)
        {
            using (MemoryStream ms = Export(dtSource, strHeaderText))
            {
                using (FileStream fs = new FileStream(strFileName, FileMode.Create, FileAccess.Write))
                {
                    byte[] data = ms.ToArray();
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                }
            }
        }

        /// <summary>
        /// DataTable导出到Excel的MemoryStream
        /// </summary>
        /// <param name="dtSource">源DataTable</param>
        /// <param name="strHeaderText">表头文本</param>
        public static MemoryStream Export(DataTable dtSource, string strHeaderText)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
            HSSFSheet sheet = (HSSFSheet)workbook.CreateSheet();

            #region 右击文件 属性信息
            {
                DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
                dsi.Company = "NPOI";
                workbook.DocumentSummaryInformation = dsi;

                SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
                si.Author = "文件作者信息"; //填加xls文件作者信息
                si.ApplicationName = "创建程序信息"; //填加xls文件创建程序信息
                si.LastAuthor = "最后保存者信息"; //填加xls文件最后保存者信息
                si.Comments = "作者信息"; //填加xls文件作者信息
                si.Title = "标题信息"; //填加xls文件标题信息
                si.Subject = "主题信息";//填加文件主题信息
                si.CreateDateTime = DateTime.Now;
                workbook.SummaryInformation = si;
            }
            #endregion

            HSSFCellStyle dateStyle = (HSSFCellStyle)workbook.CreateCellStyle();
            HSSFDataFormat format = (HSSFDataFormat)workbook.CreateDataFormat();
            dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");

            //取得列宽
            int[] arrColWidth = new int[dtSource.Columns.Count];
            foreach (DataColumn item in dtSource.Columns)
            {
                arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName.ToString()).Length;
            }
            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
                for (int j = 0; j < dtSource.Columns.Count; j++)
                {
                    int intTemp = Encoding.GetEncoding(936).GetBytes(dtSource.Rows[i][j].ToString()).Length;
                    if (intTemp > arrColWidth[j])
                    {
                        arrColWidth[j] = intTemp;
                    }
                }
            }
            int rowIndex = 0;
            foreach (DataRow row in dtSource.Rows)
            {
                #region 新建表，填充表头，填充列头，样式
                if (rowIndex == 65535 || rowIndex == 0)
                {
                    if (rowIndex != 0)
                    {
                        sheet = (HSSFSheet)workbook.CreateSheet();
                    }

                    #region 表头及样式
                    {
                        HSSFRow headerRow = (HSSFRow)sheet.CreateRow(0);
                        headerRow.HeightInPoints = 25;
                        headerRow.CreateCell(0).SetCellValue(strHeaderText);

                        HSSFCellStyle headStyle = (HSSFCellStyle)workbook.CreateCellStyle();
                        headStyle.Alignment = HorizontalAlignment.Center;
                        HSSFFont font = (HSSFFont)workbook.CreateFont();
                        font.FontHeightInPoints = 20;
                        font.Boldweight = 700;
                        headStyle.SetFont(font);
                        headerRow.GetCell(0).CellStyle = headStyle;
                    }
                    #endregion


                    #region 列头及样式
                    {
                        HSSFRow headerRow = (HSSFRow)sheet.CreateRow(1);
                        HSSFCellStyle headStyle = (HSSFCellStyle)workbook.CreateCellStyle();
                        headStyle.Alignment = HorizontalAlignment.Center;
                        HSSFFont font = (HSSFFont)workbook.CreateFont();
                        font.FontHeightInPoints = 10;
                        font.Boldweight = 700;
                        headStyle.SetFont(font);
                        foreach (DataColumn column in dtSource.Columns)
                        {
                            headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                            headerRow.GetCell(column.Ordinal).CellStyle = headStyle;

                            //设置列宽
                            sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);
                        }
                    }
                    #endregion

                    rowIndex = 2;
                }
                #endregion


                #region 填充内容
                HSSFRow dataRow = (HSSFRow)sheet.CreateRow(rowIndex);
                foreach (DataColumn column in dtSource.Columns)
                {
                    HSSFCell newCell = (HSSFCell)dataRow.CreateCell(column.Ordinal);

                    string drValue = row[column].ToString();

                    switch (column.DataType.ToString())
                    {
                        case "System.String"://字符串类型
                            newCell.SetCellValue(drValue);
                            break;
                        case "System.DateTime"://日期类型
                            DateTime dateV;
                            DateTime.TryParse(drValue, out dateV);
                            newCell.SetCellValue(dateV);

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
                #endregion

                rowIndex++;
            }
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;
                return ms;
            }
        }

        /// <summary>
        /// 用于Web导出
        /// </summary>
        /// <param name="dtSource">源DataTable</param>
        /// <param name="strHeaderText">表头文本</param>
        /// <param name="strFileName">文件名</param>
        public static void ExportByWeb(DataTable dtSource, string strHeaderText, string strFileName)
        {
            HttpContext curContext = HttpContext.Current;

            // 设置编码和附件格式
            curContext.Response.ContentType = "application/vnd.ms-excel";
            curContext.Response.ContentEncoding = Encoding.UTF8;
            curContext.Response.Charset = "";
            curContext.Response.AppendHeader("Content-Disposition",
                "attachment;filename=" + HttpUtility.UrlEncode(strFileName, Encoding.UTF8));

            curContext.Response.BinaryWrite(Export(dtSource, strHeaderText).GetBuffer());
            curContext.Response.End();
        }

        /// <summary>
        /// 数据导出excel
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="s_filename"></param>
        /// <returns></returns>
        public static string DataToExcel(DataTable dt, string s_filename)
        {
            string strline = string.Empty;
            string FileName = AppDomain.CurrentDomain.BaseDirectory + "/UploadFiles/Excel/" + s_filename + ".xls";
            string fileurl = "/UploadFiles/Excel/" + s_filename + ".xls";
            if (File.Exists(FileName))
            {
                File.Delete(FileName);
            }
            FileStream fileStream = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.Unicode);
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                strline = strline + dt.Columns[i].Caption.ToString() + Convert.ToChar(9);
            }
            streamWriter.WriteLine(strline);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                strline = string.Empty;
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (dt.Rows[i].ItemArray[j] == null)
                        strline = strline + " " + Convert.ToChar(9);
                    else
                    {
                        string rowstr = "";
                        rowstr = dt.Rows[i].ItemArray[j].ToString();
                        if (rowstr.IndexOf("\r\n") > 0)
                            rowstr = rowstr.Replace("\r\n", " ");
                        if (rowstr.IndexOf("\t") > 0)
                            rowstr = rowstr.Replace("\t", " ");
                        strline = strline + rowstr + Convert.ToChar(9);
                    }
                }
                streamWriter.WriteLine(strline);
            }
            streamWriter.Close();
            fileStream.Close();
            return fileurl;
        }

        /// <summary>
        /// 实体集合转Excel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="s_filename"></param>
        /// <returns></returns>
        public static string EnumrableToExcel<T>(IEnumerable<T> query, string s_filename)
        {
            string strline = string.Empty;
            string FileName = AppDomain.CurrentDomain.BaseDirectory + "/UploadFiles/" + s_filename + ".xls";
            string fileurl = "/UploadFiles/" + s_filename + ".xls";
            if (File.Exists(FileName))
            {
                File.Delete(FileName);
            }
            FileStream fileStream = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.Unicode);
            var properties = typeof(T).GetProperties();
            for (int i = 0; i < properties.Count(); i++)
            {
                strline = strline + properties[i].Name + Convert.ToChar(9);
            }
            streamWriter.WriteLine(strline);
            foreach (var linq in query)
            {
                strline = string.Empty;
                for (int j = 0; j < properties.Count(); j++)
                {
                    var value = properties[j].GetValue(linq);
                    if (value == null)
                        strline = strline + " " + Convert.ToChar(9);
                    else
                        strline = strline + value + Convert.ToChar(9);
                }
                streamWriter.WriteLine(strline);
            }
            streamWriter.Close();
            fileStream.Close();
            return fileurl;
        }
    }
}
