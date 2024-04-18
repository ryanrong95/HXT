using System;
using System.Text;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;
using NPOI.XWPF.UserModel;
using System.Reflection;
using System.Data;
using NPOI.SS.Util;
using System.Drawing;
using System.Web;

namespace Needs.Wl.Web.Mvc.Utils
{
    /// <summary>
    /// NPOI
    /// 导出Excel,另存为，HttpResponse下载
    /// 目前仅仅支持一个Sheet,如果要多个Sheet，需要重新扩展
    /// </summary>
    public sealed class NPOIContext
    {
        public IWorkbook WorkBook
        {
            get;
            private set;
        }

        public ISheet Sheets
        {
            get;
            private set;
        }

        public NPOIContext()
        {
            this.WorkBook = new XSSFWorkbook();
            this.Sheets = this.WorkBook.CreateSheet();
        }

        public NPOIContext(string fileName, Stream stream) : this()
        {
            string extendsion = Path.GetExtension(fileName).ToLower();
            switch (extendsion)
            {
                case ".xls":
                    this.WorkBook = new HSSFWorkbook(stream);
                    break;
                case ".xlsx":
                    this.WorkBook = new XSSFWorkbook(stream);
                    break;
            }
            this.Sheets = this.WorkBook.CreateSheet();
        }

        /// <summary>
        /// 列
        /// 参考Items的设计： Columns.Add(new HeaderColumn(列名、属性等))
        /// </summary>
        public object Columns
        {
            get; set;
        }

        /// <summary>
        /// 设置列
        /// 这种方式不是特别好，如果有多个Sheets，就无法实现其他Sheet的标题设置
        /// </summary>
        /// <param name="columns"></param>
        public void SetColumns(IEnumerable<string> columns)
        {
            IRow headerRow = this.Sheets.CreateRow(0);

            int columnIndex = 0;
            foreach (string column in columns)
            {
                headerRow.CreateCell(columnIndex).SetCellValue(column);
                columnIndex++;
            }
        }

        /// <summary>
        /// 设置数据源
        /// </summary>
        public DataTable DataSource
        {
            private get; set;
        }

        /// <summary>
        /// 写入
        /// </summary>
        public void WriteToExcel()
        {
            var dataRows = this.DataSource.Rows;
            var columns = this.DataSource.Columns;

            int rowIndex = 1;
            foreach (DataRow row in dataRows)
            {
                IRow dataRow = this.Sheets.CreateRow(rowIndex);

                foreach (DataColumn column in columns)
                {
                    dataRow.CreateCell(column.Ordinal).SetCellValue(row[column].ToString());
                }

                rowIndex++;
            }
        }

        public void Response(string fileName)
        {
            HttpResponse httpResponse = HttpContext.Current.Response;
            httpResponse.Clear();
            httpResponse.Buffer = true;
            httpResponse.Charset = Encoding.UTF8.BodyName;
            httpResponse.AppendHeader("Content-Disposition", "attachment;filename=" + fileName + ".xls");
            httpResponse.ContentEncoding = Encoding.UTF8;
            httpResponse.ContentType = "application/vnd.ms-excel; charset=UTF-8";
            this.WorkBook.Write(httpResponse.OutputStream);
            httpResponse.End();
        }

        public void ReadToDataTable()
        {
            //TODO:
        }

        public IList<T> ReadToList<T>(bool ignoreFirstLine) where T : new()
        {
            if (this.WorkBook == null)
            {
                throw new Exception("Excel表格工作簿为空");
            }

            IList<T> list = new List<T>();
            for (int i = 0; i < this.WorkBook.NumberOfSheets; i++)
            {
                ISheet sheet = this.WorkBook.GetSheetAt(i);

                if (sheet.PhysicalNumberOfRows > 0)
                {
                    //if (!ignoreFirstLine)
                    //{
                    //    //检查列是否与ExcelAttribute定义的一致
                    //    ValidTableHeader<T>(sheet);
                    //}

                    for (int j = ignoreFirstLine ? 0 : 1; j < sheet.PhysicalNumberOfRows; j++)
                    {
                        var row = sheet.GetRow(j);

                        T entity = new T();
                        var propertys = typeof(T).GetProperties();

                        foreach (var p in propertys)
                        {
                            var excel = Attribute.GetCustomAttribute(p, typeof(ExcelAttribute)) as ExcelAttribute;

                            if (excel != null)
                            {
                                var cellValue = row.GetCell(excel.ColumnIndex);

                                if (cellValue == null || string.IsNullOrEmpty(cellValue.ToString()))
                                    throw new Exception(string.Format("第{0}行“{1}”不能为空", j + 1, excel.ColumnName));

                                string cellValueStr = cellValue.ToString();
                                if (p.PropertyType == typeof(int))
                                {
                                    int temp;
                                    if (!int.TryParse(cellValueStr, out temp))
                                        throw new Exception(string.Format("第{0}行“{1}”应为{2}类型", j + 1, excel.ColumnName, "整数"));
                                    p.SetValue(entity, temp);
                                }
                                else if (p.PropertyType == typeof(DateTime))
                                {
                                    DateTime temp;
                                    if (!DateTime.TryParse(cellValueStr, out temp))
                                        throw new Exception(string.Format("第{0}行“{1}”应为{2}类型", j + 1, excel.ColumnName, "时间"));
                                    p.SetValue(entity, temp);
                                }
                                else if (p.PropertyType == typeof(bool))
                                {
                                    bool temp;
                                    if (!bool.TryParse(cellValueStr, out temp))
                                        throw new Exception(string.Format("第{0}行“{1}”应为{2}类型", j + 1, excel.ColumnName, "布尔"));
                                    p.SetValue(entity, cellValueStr);
                                }
                                else if (p.PropertyType == typeof(string))
                                {
                                    p.SetValue(entity, cellValueStr);
                                }
                                else
                                {
                                    throw new Exception(string.Format("第{0}行“{1}”类型未知，请联系开发人员", j + 1, excel.ColumnName));
                                }
                            }
                        }
                        list.Add(entity);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 检查表头与定义是否匹配
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="firstRow"></param>
        /// <returns></returns>
        private static void ValidTableHeader<T>(ISheet sheet) where T : new()
        {
            var firstRow = sheet.GetRow(0);
            var propertys = typeof(T).GetProperties();

            foreach (var p in propertys)
            {
                var excel = Attribute.GetCustomAttribute(p, typeof(ExcelAttribute)) as ExcelAttribute;

                if (excel != null)
                {
                    if (!firstRow.GetCell(excel.ColumnIndex).StringCellValue.Trim().Equals(excel.ColumnName))
                    {
                        throw new Exception(string.Format("Excel表格第{0}列标题应为{1}", excel.ColumnIndex + 1, excel.ColumnName));
                    }
                }
            }
        }

        /// <summary>
        /// 另存为
        /// </summary>
        /// <param name="fileName">文件的全路径</param>
        public void SaveAs(string fileName)
        {
            using (FileStream file = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                this.WorkBook.Write(file);
                file.Close();
            }
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ExcelAttribute : Attribute
    {
        private string _columnName;

        public string ColumnName
        {
            get { return _columnName; }
            set { _columnName = value; }
        }
        private int _columnIndex;

        public int ColumnIndex
        {
            get { return _columnIndex; }
            set { _columnIndex = value; }
        }

        public ExcelAttribute(string columnName)
        {
            this._columnName = columnName;
        }

        public ExcelAttribute(string columnName, int columnIndex)
        {
            this._columnName = columnName;
            this._columnIndex = columnIndex;
        }
    }

    public class NPOIHelper
    {

        static XSSFWorkbook xssfworkbook;

        public static void WriteToFile(string savePath)
        {
            //Write the stream data of workbook to the root directory
            FileStream file = new FileStream(savePath, FileMode.Create);
            xssfworkbook.Write(file);
            file.Close();
        }

        public static void InitializeWorkbook(string templetePath)
        {
            //read the template via FileStream, it is suggested to use FileAccess.Read to prevent file lock.
            //book1.xls is an Excel-2007-generated file, so some new unknown BIFF records are added. 
            FileStream file = new FileStream(templetePath, FileMode.Open, FileAccess.Read);

            xssfworkbook = new XSSFWorkbook(file);

            ////create a entry of DocumentSummaryInformation
            //DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            //dsi.Company = "NPOI Team";
            //xssfworkbook. .DocumentSummaryInformation = dsi;

            ////create a entry of SummaryInformation
            //SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            //si.Subject = "NPOI SDK Example";
            //xssfworkbook.SummaryInformation = si;
        }

        /// <summary>
        /// 根据模板导出excel
        /// </summary>
        /// <param name="templetePath"></param>
        /// <param name="savePath"></param>
        /// <param name="info"></param>
        /// <param name="data"></param>
        public void GenerateExcelByTemplete(string templetePath, string savePath, Dictionary<object, int[]> info, int rows, int sumnRow, string[,] data = null)
        {

            InitializeWorkbook(templetePath);
            ISheet sheet1 = xssfworkbook.GetSheet("恒远模板");
            //create cell on rows, since rows do already exist,it's not necessary to create rows again.
            XSSFCellStyle stylewrap = (XSSFCellStyle)xssfworkbook.CreateCellStyle();
            stylewrap.WrapText = true;

            //字体
            IFont font1 = xssfworkbook.CreateFont();//字体
            font1.FontHeightInPoints = 9;
            XSSFCellStyle stylewrapandfont = (XSSFCellStyle)xssfworkbook.CreateCellStyle();
            stylewrapandfont.WrapText = true;
            stylewrapandfont.SetFont(font1);

            XSSFCellStyle stylefour = (XSSFCellStyle)xssfworkbook.CreateCellStyle();
            IDataFormat formatfour = xssfworkbook.CreateDataFormat();
            stylefour.SetDataFormat(formatfour.GetFormat("0.0000"));


            XSSFCellStyle styletwo = (XSSFCellStyle)xssfworkbook.CreateCellStyle();
            IDataFormat formattwo = xssfworkbook.CreateDataFormat();
            styletwo.SetDataFormat(formattwo.GetFormat("0.00"));


            foreach (var dic in info)
            {
                if (sheet1.GetRow(dic.Value[0]) == null)
                    sheet1.CreateRow(dic.Value[0]);

                if (sheet1.GetRow(dic.Value[0]).GetCell(dic.Value[1]) == null)
                    sheet1.GetRow(dic.Value[0]).CreateCell(dic.Value[1]);

                sheet1.GetRow(dic.Value[0]).GetCell(dic.Value[1]).SetCellValue(dic.Key.ToString().Replace(dic.Value[0].ToString() + "=" + dic.Value[1].ToString(), ""));


                //数量汇总
                if (dic.Value[0] == sumnRow && (dic.Value[1] == 18))
                {
                    sheet1.GetRow(sumnRow).GetCell(dic.Value[1]).SetCellValue(int.Parse(dic.Key.ToString().Replace(dic.Value[0].ToString() + "=" + dic.Value[1].ToString(), "")));
                }
                //金额汇总
                if (dic.Value[0] == sumnRow && (dic.Value[1] == 12 || dic.Value[1] == 19 || dic.Value[1] == 20))
                {
                    sheet1.GetRow(sumnRow).GetCell(dic.Value[1]).SetCellValue(Convert.ToDouble(dic.Key.ToString().Replace(dic.Value[0].ToString() + "=" + dic.Value[1].ToString(), "")));
                }

                //数量汇总(数量有小数) 2018.06.20 LK
                if (dic.Value[0] == sumnRow && (dic.Value[1] == 4))
                {
                    sheet1.GetRow(sumnRow).GetCell(dic.Value[1]).SetCellValue(Double.Parse(dic.Key.ToString().Replace(dic.Value[0].ToString() + "=" + dic.Value[1].ToString(), "")));
                }
            }

            //C列自动2-7行自动换行         
            sheet1.GetRow(2).GetCell(2).CellStyle = stylewrapandfont;
            sheet1.GetRow(3).GetCell(2).CellStyle = stylewrapandfont;
            sheet1.GetRow(4).GetCell(2).CellStyle = stylewrapandfont;
            sheet1.GetRow(5).GetCell(2).CellStyle = stylewrapandfont;
            sheet1.GetRow(6).GetCell(2).CellStyle = stylewrapandfont;


            if (data != null)
            {
                for (int i = 10; i < data.GetLength(0) + 10; i++)
                {
                    if (sheet1.GetRow(i) == null)
                        sheet1.CreateRow(i);
                    for (int j = 0; j < data.GetLength(1); j++)
                    {
                        if (sheet1.GetRow(i).GetCell(j) == null)
                            sheet1.GetRow(i).CreateCell(j);

                        //设置单元格值，并修改数字格式(成交数量，第二数量，金额)
                        if (j == 0)
                        {
                            sheet1.GetRow(i).GetCell(j).SetCellValue(i - 9); //序号,整数
                        }
                        else if (j == 3)
                        {
                            sheet1.GetRow(i).GetCell(j).SetCellValue(data[i - 10, j]);
                            sheet1.GetRow(i).GetCell(j).CellStyle = stylewrapandfont;
                        }
                        else if (j == 14)
                        {
                            sheet1.GetRow(i).GetCell(j).SetCellValue(int.Parse(data[i - 10, j]));//成交数量,法定数量，征免，数量，整数
                        }

                        else if (j == 8 || j == 12 || j == 19 || j == 20)
                        {
                            if (!string.IsNullOrEmpty(data[i - 10, j]))
                            {
                                sheet1.GetRow(i).GetCell(j).SetCellValue(Convert.ToDouble(data[i - 10, j]));//第二数量，金额，净重，毛重，两位小数
                                sheet1.GetRow(i).GetCell(j).CellStyle = styletwo;
                            }

                        }
                        else if (j == 11)
                        {
                            sheet1.GetRow(i).GetCell(j).SetCellValue(Convert.ToDouble(data[i - 10, j]));//单价保留4位小数
                            sheet1.GetRow(i).GetCell(j).CellStyle = stylefour;
                        }
                        else if (j == 4 || j == 22 || j == 6) //成交数量,数量，法定数量，有小数就保留小数，没小数就整数
                        {
                            if (data[i - 10, j] != "")
                            {
                                sheet1.GetRow(i).GetCell(j).SetCellValue(Convert.ToDouble(Convert.ToDouble(data[i - 10, j]).ToString("0.####")));
                                string[] arryNumber = Convert.ToDouble(data[i - 10, j]).ToString("0.####").Split('.');
                                if (arryNumber.Length > 1)
                                {
                                    if (arryNumber[1].Length > 3)
                                    {
                                        sheet1.GetRow(i).GetCell(j).CellStyle = stylefour;
                                    }
                                }
                            }
                        }
                        else
                        {
                            sheet1.GetRow(i).GetCell(j).SetCellValue(data[i - 10, j]);//其它，文本类型
                        }
                    }
                }
            }

            //合并单元格
            int start = 0;//记录同组开始行号
            int end = 0;//记录同组结束行号
            string temp = "";
            for (int i = 0; i <= rows; i++)
            {
                int j = 18;
                string cellText = i != rows ? data[i, j].ToString() : "";

                if (cellText == temp)//上下行相等，记录要合并的最后一行
                {
                    end = i;
                }
                else//上下行不等，记录
                {
                    if (start != end)
                    {
                        CellRangeAddress region = new CellRangeAddress(start + 10, end + 10, 18, 18);
                        sheet1.AddMergedRegion(region);
                        CellRangeAddress region2 = new CellRangeAddress(start + 10, end + 10, 20, 20);
                        sheet1.AddMergedRegion(region2);
                    }
                    start = i;
                    end = i;
                    temp = cellText;
                }
            }

            //Force excel to recalculate all the formula while open
            sheet1.ForceFormulaRecalculation = true;

            WriteToFile(savePath);
        }

        /// <summary>
        /// 根据模板导出excel-单一窗口
        /// </summary>
        /// <param name="templetePath"></param>
        /// <param name="savePath"></param>
        /// <param name="info"></param>
        /// <param name="rows"></param>
        /// <param name="data"></param>
        public void GenerateExcelByTempleteSW(string templetePath, string savePath, Dictionary<object, int[]> info, int rows, string[,] data = null)
        {

            InitializeWorkbook(templetePath);
            ISheet sheet1 = xssfworkbook.GetSheet("报关单");
            //create cell on rows, since rows do already exist,it's not necessary to create rows again.
            XSSFCellStyle stylewrap = (XSSFCellStyle)xssfworkbook.CreateCellStyle();
            stylewrap.WrapText = true;

            //字体
            IFont font1 = xssfworkbook.CreateFont();//字体
            font1.FontHeightInPoints = 9;
            XSSFCellStyle stylewrapandfont = (XSSFCellStyle)xssfworkbook.CreateCellStyle();
            stylewrapandfont.WrapText = true;
            stylewrapandfont.SetFont(font1);

            XSSFCellStyle stylefour = (XSSFCellStyle)xssfworkbook.CreateCellStyle();
            IDataFormat formatfour = xssfworkbook.CreateDataFormat();
            stylefour.SetDataFormat(formatfour.GetFormat("0.0000"));

            XSSFCellStyle styletwo = (XSSFCellStyle)xssfworkbook.CreateCellStyle();
            IDataFormat formattwo = xssfworkbook.CreateDataFormat();
            styletwo.SetDataFormat(formattwo.GetFormat("0.00"));

            XSSFCellStyle stylethree = (XSSFCellStyle)xssfworkbook.CreateCellStyle();
            IDataFormat formatthree = xssfworkbook.CreateDataFormat();
            stylethree.SetDataFormat(formatthree.GetFormat("0.000"));


            foreach (var dic in info)
            {
                if (sheet1.GetRow(dic.Value[0]) == null)
                    sheet1.CreateRow(dic.Value[0]);

                if (sheet1.GetRow(dic.Value[0]).GetCell(dic.Value[1]) == null)
                    sheet1.GetRow(dic.Value[0]).CreateCell(dic.Value[1]);

                sheet1.GetRow(dic.Value[0]).GetCell(dic.Value[1]).SetCellValue(dic.Key.ToString().Replace(dic.Value[0].ToString() + "=" + dic.Value[1].ToString(), ""));

                //口岸代码
                if ((dic.Value[0] == 2 && (dic.Value[1] == 1))
                    || (dic.Value[0] == 3 && (dic.Value[1] == 4))
                    || (dic.Value[0] == 17 && (dic.Value[1] == 4))
                    || (dic.Value[0] == 81 && (dic.Value[1] == 2))
                    || (dic.Value[0] == 91 && (dic.Value[1] == 2))
                    || (dic.Value[0] == 92 && (dic.Value[1] == 2))
                    || (dic.Value[0] == 91 && (dic.Value[1] == 6)))
                {
                    sheet1.GetRow(dic.Value[0]).GetCell(dic.Value[1]).SetCellValue(int.Parse(dic.Key.ToString().Replace(dic.Value[0].ToString() + "=" + dic.Value[1].ToString(), "")));
                }

                //件数汇总
                if ((dic.Value[0] == 79 && dic.Value[1] == 22) || (dic.Value[0] == 16 && dic.Value[1] == 1))
                {
                    sheet1.GetRow(dic.Value[0]).GetCell(dic.Value[1]).SetCellValue(int.Parse(dic.Key.ToString().Replace(dic.Value[0].ToString() + "=" + dic.Value[1].ToString(), "")));
                }
                //金额汇总
                if (dic.Value[0] == 79 && (dic.Value[1] == 13 || dic.Value[1] == 23 || dic.Value[1] == 24))
                {
                    sheet1.GetRow(dic.Value[0]).GetCell(dic.Value[1]).SetCellValue(Convert.ToDouble(dic.Key.ToString().Replace(dic.Value[0].ToString() + "=" + dic.Value[1].ToString(), "")));
                }

                //数量汇总(数量有小数) 2018.06.20 LK
                if ((dic.Value[0] == 79 && dic.Value[1] == 6) || (dic.Value[0] == 16 && dic.Value[1] == 7))
                {
                    sheet1.GetRow(dic.Value[0]).GetCell(dic.Value[1]).SetCellValue(Double.Parse(dic.Key.ToString().Replace(dic.Value[0].ToString() + "=" + dic.Value[1].ToString(), "")));
                }

                //日期格式
                if (dic.Value[0] == 18 && (dic.Value[1] == 15))
                {
                    //XSSFCellStyle styletime = (XSSFCellStyle)xssfworkbook.CreateCellStyle();
                    //IDataFormat formattime = xssfworkbook.CreateDataFormat();
                    //styletime.SetDataFormat(formattime.GetFormat("yyyy-MM-dd"));
                    //styletime.VerticalAlignment = VerticalAlignment.Center;
                    //styletime.Alignment = HorizontalAlignment.Center;

                    sheet1.GetRow(dic.Value[0]).GetCell(dic.Value[1]).SetCellValue(DateTime.Parse(dic.Key.ToString().Replace(dic.Value[0].ToString() + "=" + dic.Value[1].ToString(), "")));
                    //sheet1.GetRow(dic.Value[0]).GetCell(dic.Value[1]).CellStyle = styletime;
                }
            }

            //C列自动2-7行自动换行         
            //sheet1.GetRow(2).GetCell(2).CellStyle = stylewrapandfont;
            //sheet1.GetRow(3).GetCell(2).CellStyle = stylewrapandfont;
            //sheet1.GetRow(4).GetCell(2).CellStyle = stylewrapandfont;
            //sheet1.GetRow(5).GetCell(2).CellStyle = stylewrapandfont;
            //sheet1.GetRow(6).GetCell(2).CellStyle = stylewrapandfont;


            if (data != null)
            {
                for (int i = 29; i < data.GetLength(0) + 29; i++)
                {
                    if (sheet1.GetRow(i) == null)
                        sheet1.CreateRow(i);
                    for (int j = 0; j < data.GetLength(1); j++)
                    {
                        if (sheet1.GetRow(i).GetCell(j) == null)
                            sheet1.GetRow(i).CreateCell(j);
                        if (j == 0)
                        {
                            sheet1.GetRow(i).GetCell(j).SetCellValue(int.Parse(data[i - 29, j]) + 1);//序号
                        }
                        //设置单元格值，并修改数字格式(成交数量，第二数量，金额)
                        else if (j == 5)
                        {
                            sheet1.GetRow(i).GetCell(j).SetCellValue(data[i - 29, j]);
                            sheet1.GetRow(i).GetCell(j).CellStyle = stylewrapandfont;
                        }
                        else if (j == 34)
                        {
                            sheet1.GetRow(i).GetCell(j).SetCellValue(int.Parse(data[i - 29, j]) + 1);//后面的序号
                        }

                        else if (j == 13 || j == 23 || j == 24)
                        {
                            if (!string.IsNullOrEmpty(data[i - 29, j]))
                            {
                                sheet1.GetRow(i).GetCell(j).SetCellValue(Convert.ToDouble(data[i - 29, j]));//金额，净重，毛重，两位小数
                                sheet1.GetRow(i).GetCell(j).CellStyle = styletwo;
                            }

                        }
                        else if (j == 10)
                        {
                            if (!string.IsNullOrEmpty(data[i - 29, j]))
                            {
                                sheet1.GetRow(i).GetCell(j).SetCellValue(Convert.ToDouble(data[i - 29, j]));//第二数量三位小数
                                sheet1.GetRow(i).GetCell(j).CellStyle = stylethree;
                            }
                        }
                        else if (j == 12)
                        {
                            sheet1.GetRow(i).GetCell(j).SetCellValue(Convert.ToDouble(data[i - 29, j]));//单价保留4位小数
                            sheet1.GetRow(i).GetCell(j).CellStyle = stylefour;
                        }
                        else if (j == 6 || j == 8 || j == 29) //成交数量,数量，法定数量，有小数就保留小数，没小数就整数
                        {
                            if (data[i - 29, j] != "")
                            {
                                sheet1.GetRow(i).GetCell(j).SetCellValue(Convert.ToDouble(Convert.ToDouble(data[i - 29, j]).ToString("0.####")));
                                string[] arryNumber = Convert.ToDouble(data[i - 29, j]).ToString("0.####").Split('.');
                                if (arryNumber.Length > 1)
                                {
                                    if (arryNumber[1].Length > 3)
                                    {
                                        sheet1.GetRow(i).GetCell(j).CellStyle = stylefour;
                                    }
                                }
                            }
                        }
                        else if (j == 25) // 商检，设置公式
                        {
                            if (data[i - 29, j] != null)
                            {
                                sheet1.GetRow(i).GetCell(j).CellFormula = "\";;;;;\" & AC30 & \";\" & AF30 & \";;\"";
                            }
                        }
                        else
                        {
                            sheet1.GetRow(i).GetCell(j).SetCellValue(data[i - 29, j]);//其它，文本类型
                        }
                    }
                }
            }

            //合并单元格
            int start = 0;//记录同组开始行号
            int end = 0;//记录同组结束行号
            string temp = "";
            for (int i = 0; i <= rows; i++)
            {
                int j = 22;
                string cellText = i != rows ? data[i, j].ToString() : "";

                if (cellText == temp)//上下行相等，记录要合并的最后一行
                {
                    end = i;
                }
                else//上下行不等，记录
                {
                    if (start != end)
                    {
                        CellRangeAddress region = new CellRangeAddress(start + 29, end + 29, 22, 22);
                        sheet1.AddMergedRegion(region);
                        CellRangeAddress region2 = new CellRangeAddress(start + 29, end + 29, 24, 24);
                        sheet1.AddMergedRegion(region2);
                    }
                    start = i;
                    end = i;
                    temp = cellText;
                }
            }

            //Force excel to recalculate all the formula while open
            sheet1.ForceFormulaRecalculation = true;

            WriteToFile(savePath);
        }

        /// <summary>
        /// 根据模板导出Word
        /// </summary>
        /// <param name="templetePath"></param>
        /// <param name="savePath"></param>
        /// <param name="findText"></param>
        /// <param name="replaceText"></param>
        public void GenerateWordByTemplete(string templetePath, string savePath, Dictionary<string, string> replaceText)
        {
            using (FileStream stream = File.OpenRead(templetePath))
            {
                XWPFDocument doc = new XWPFDocument(stream);
                //遍历段落
                foreach (var para in doc.Paragraphs)
                {
                    ReplaceKey(para, replaceText);
                }
                //遍历表格
                //var tables = doc.Tables;
                //foreach (var table in tables)
                //{
                //    foreach (var row in table.Rows)
                //    {
                //        foreach (var cell in row.GetTableCells())
                //        {
                //            foreach (var para in cell.Paragraphs)
                //            {
                //                ReplaceKey(para);
                //            }
                //        }
                //    }
                //}
                FileStream file = new FileStream(savePath, FileMode.Create);
                doc.Write(file);
                file.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="para"></param>
        /// <param name="findText"></param>
        /// <param name="replaceText"></param>
        private void ReplaceKey(XWPFParagraph para, Dictionary<string, string> replaceText)
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

        /// <summary>
        /// 产品导入
        /// </summary>
        /// <param name="ext">文件后缀</param>
        /// <param name="stream">文件流</param>
        /// <param name="isColumnName">第一行是否是列名</param>
        /// <returns></returns>
        public static DataTable ExcelToDataTableProducts(string ext, Stream stream, bool isColumnName)
        {
            DataTable dataTable = null;
            DataColumn column = null;
            DataRow dataRow = null;
            IWorkbook workbook = null;
            ISheet sheet = null;
            IRow row = null;
            NPOI.SS.UserModel.ICell cell = null;
            int startRow = 6;
            try
            {
                // 2007版本
                if (ext == ".xlsx")
                    workbook = new XSSFWorkbook(stream);
                // 2003版本
                else if (ext == ".xls")
                    workbook = new HSSFWorkbook(stream);

                if (workbook != null)
                {
                    sheet = workbook.GetSheetAt(0);//读取第一个sheet，当然也可以循环读取每个sheet
                    dataTable = new DataTable();
                    if (sheet != null)
                    {
                        int rowCount = sheet.LastRowNum;//总行数
                        if (rowCount > 0)
                        {
                            IRow firstRow = sheet.GetRow(5);//第一行
                            int cellCount = firstRow.LastCellNum;//列数

                            //构建datatable的列
                            if (isColumnName)
                            {
                                startRow = 6;//如果第一行是列名，则从第二行开始读取
                                for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                                {
                                    cell = firstRow.GetCell(i);
                                    if (cell != null)
                                    {
                                        if (cell.StringCellValue != null)
                                        {
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
                                row = sheet.GetRow(i);
                                if (row == null) continue;

                                dataRow = dataTable.NewRow();
                                for (int j = row.FirstCellNum; j < cellCount; ++j)
                                {
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
                                            case CellType.Blank:
                                                dataRow[j] = "";
                                                break;
                                            case CellType.Numeric:
                                                short format = cell.CellStyle.DataFormat;
                                                //对时间格式（2015.12.5、2015/12/5、2015-12-5等）的处理
                                                if (format == 14 || format == 31 || format == 57 || format == 58)
                                                    dataRow[j] = cell.DateCellValue;
                                                else
                                                    dataRow[j] = cell.NumericCellValue;
                                                break;
                                            case CellType.String:
                                                dataRow[j] = cell.StringCellValue;
                                                break;
                                            case CellType.Formula:
                                                dataRow[j] = cell.NumericCellValue;
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
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// 产品导入
        /// </summary>
        /// <param name="ext">文件后缀</param>
        /// <param name="stream">文件流</param>
        /// <param name="isColumnName">第一行是否是列名</param>
        /// <returns></returns>
        public static DataTable ExcelToDataTable(string ext, Stream stream, bool isColumnName)
        {
            DataTable dataTable = null;
            DataColumn column = null;
            DataRow dataRow = null;
            IWorkbook workbook = null;
            ISheet sheet = null;
            IRow row = null;
            NPOI.SS.UserModel.ICell cell = null;
            int startRow = 0;
            try
            {
                // 2007版本
                if (ext == ".xlsx")
                    workbook = new XSSFWorkbook(stream);
                // 2003版本
                else if (ext == ".xls")
                    workbook = new HSSFWorkbook(stream);

                if (workbook != null)
                {
                    sheet = workbook.GetSheetAt(0);//读取第一个sheet，当然也可以循环读取每个sheet
                    dataTable = new DataTable();
                    if (sheet != null)
                    {
                        int rowCount = sheet.LastRowNum;//总行数
                        if (rowCount > 0)
                        {
                            IRow firstRow = sheet.GetRow(1);//第一行
                            int cellCount = firstRow.LastCellNum;//列数

                            //构建datatable的列
                            if (isColumnName)
                            {
                                startRow = 2;//如果第一行是列名，则从第二行开始读取
                                for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                                {
                                    cell = firstRow.GetCell(i);
                                    if (cell != null)
                                    {
                                        if (cell.StringCellValue != null)
                                        {
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
                                row = sheet.GetRow(i);
                                if (row == null) continue;

                                dataRow = dataTable.NewRow();
                                for (int j = row.FirstCellNum; j < cellCount; ++j)
                                {
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
                                            case CellType.Blank:
                                                dataRow[j] = "";
                                                break;
                                            case CellType.Numeric:
                                                short format = cell.CellStyle.DataFormat;
                                                //对时间格式（2015.12.5、2015/12/5、2015-12-5等）的处理
                                                if (format == 14 || format == 31 || format == 57 || format == 58)
                                                    dataRow[j] = cell.DateCellValue;
                                                else
                                                    dataRow[j] = cell.NumericCellValue;
                                                break;
                                            case CellType.String:
                                                dataRow[j] = cell.StringCellValue;
                                                break;
                                            case CellType.Formula:
                                                dataRow[j] = cell.NumericCellValue;
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
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// 商品型号模板
        /// </summary>
        /// <param name="filePath">excel路径</param>
        /// <param name="isColumnName">第一行是否是列名</param>
        /// <returns>返回datatable</returns>
        public static DataTable ExcelToDataTableProducts(string filePath, bool isColumnName)
        {
            DataTable dataTable = null;
            FileStream fs = null;
            DataColumn column = null;
            DataRow dataRow = null;
            IWorkbook workbook = null;
            ISheet sheet = null;
            IRow row = null;
            NPOI.SS.UserModel.ICell cell = null;
            int startRow = 6;
            try
            {
                using (fs = File.OpenRead(filePath))
                {
                    // 2007版本
                    if (filePath.IndexOf(".xlsx") > 0)
                        workbook = new XSSFWorkbook(fs);
                    // 2003版本
                    else if (filePath.IndexOf(".xls") > 0)
                        workbook = new HSSFWorkbook(fs);

                    if (workbook != null)
                    {
                        sheet = workbook.GetSheetAt(0);//读取第一个sheet，当然也可以循环读取每个sheet
                        dataTable = new DataTable();
                        if (sheet != null)
                        {
                            int rowCount = sheet.LastRowNum;//总行数
                            if (rowCount > 0)
                            {
                                IRow firstRow = sheet.GetRow(5);//第一行
                                int cellCount = firstRow.LastCellNum;//列数

                                //构建datatable的列
                                if (isColumnName)
                                {
                                    startRow = 6;//如果第一行是列名，则从第二行开始读取
                                    for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                                    {
                                        cell = firstRow.GetCell(i);
                                        if (cell != null)
                                        {
                                            if (cell.StringCellValue != null)
                                            {
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
                                    row = sheet.GetRow(i);
                                    if (row == null) continue;

                                    dataRow = dataTable.NewRow();
                                    for (int j = row.FirstCellNum; j < cellCount; ++j)
                                    {
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
                                                case CellType.Blank:
                                                    dataRow[j] = "";
                                                    break;
                                                case CellType.Numeric:
                                                    short format = cell.CellStyle.DataFormat;
                                                    //对时间格式（2015.12.5、2015/12/5、2015-12-5等）的处理
                                                    if (format == 14 || format == 31 || format == 57 || format == 58)
                                                        dataRow[j] = cell.DateCellValue;
                                                    else
                                                        dataRow[j] = cell.NumericCellValue;
                                                    break;
                                                case CellType.String:
                                                    dataRow[j] = cell.StringCellValue;
                                                    break;
                                                case CellType.Formula:
                                                    dataRow[j] = cell.NumericCellValue;
                                                    break;
                                            }
                                        }
                                    }
                                    dataTable.Rows.Add(dataRow);
                                }
                            }
                        }
                    }
                }
                return dataTable;
            }
            catch (Exception)
            {
                if (fs != null)
                {
                    fs.Close();
                }
                return null;
            }
        }

        /// <summary>
        /// 将excel导入到datatable 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="isColumnName"></param>
        /// <returns></returns>
        public static DataTable ExcelToDataTable(string filePath, bool isColumnName)
        {
            DataTable dataTable = null;
            FileStream fs = null;
            DataColumn column = null;
            DataRow dataRow = null;
            IWorkbook workbook = null;
            ISheet sheet = null;
            IRow row = null;
            NPOI.SS.UserModel.ICell cell = null;
            int startRow = 0;
            try
            {
                using (fs = File.OpenRead(filePath))
                {
                    // 2007版本
                    if (filePath.IndexOf(".xlsx") > 0)
                        workbook = new XSSFWorkbook(fs);
                    // 2003版本
                    else if (filePath.IndexOf(".xls") > 0)
                        workbook = new HSSFWorkbook(fs);

                    if (workbook != null)
                    {
                        sheet = workbook.GetSheetAt(0);//读取第一个sheet，当然也可以循环读取每个sheet
                        dataTable = new DataTable();
                        if (sheet != null)
                        {
                            int rowCount = sheet.LastRowNum;//总行数
                            if (rowCount > 0)
                            {
                                IRow firstRow = sheet.GetRow(0);//第一行
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
                                    row = sheet.GetRow(i);
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
                }
                return dataTable;
            }
            catch (Exception ex)
            {
                if (fs != null)
                {
                    fs.Close();
                }
                return null;
            }
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
            int[] columnWidth = { 10, 10, 25, 10, 15, 15, 20, 10, 20, 20, 15, 15 };
            for (int i = 0; i < columnWidth.Length; i++)
            {
                //设置列宽度，256*字符数，因为单位是1/256个字符
                sheet.SetColumnWidth(i, 256 * columnWidth[i]);
            }

            IRow row;
            NPOI.SS.UserModel.ICell cell;

            string[] fieldArr = { "TimeNow", "ModelName", "Model", "Unit", "UnitPrice", "Quantity", "AmountPrice", "UnitPriceTax", "AmountTax", "CompanyName", "InvoiceNumbers", "CreateTime", "Tax" };
            System.Collections.Generic.Dictionary<string, string> fieldDic = new System.Collections.Generic.Dictionary<string, string> { { "TimeNow", "日期" }, { "ModelName", "品名" }, { "Model", "型号" }, { "Unit", "计量单位" }, { "UnitPrice", "不含税价格" }, { "Quantity", "数量" }, { "AmountPrice", "不含税金额" }, { "UnitPriceTax", "含税单价" }, { "AmountTax", "含税金额" }, { "CompanyName", "开票公司" }, { "InvoiceNumbers", "发票号" }, { "CreateTime", "开票日期" }, { "Tax", "税额" } };

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
                        if (j == 4 || j == 5 || j == 6 || j == 7 || j == 8 || j == 12)
                        {
                            SetCellValue(cell, Convert.ToDouble(obj));
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

        /// <summary>
        /// 开票信息导入到Excel
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
                cell.SetCellValue("订单号/合同号");
                cell.CellStyle = borderstyle;

                cell = row.CreateCell(12);
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
                    cell.SetCellValue(dt.Rows[j]["IsLocal"].ToString().Equals("1") ? "个" : dt.Rows[j]["Unit"].ToString());
                    cell.CellStyle = borderstyle;

                    cell = row.CreateCell(4);
                    cell.SetCellValue(Convert.ToDouble(dt.Rows[j]["TaxoffUnitPrice"].ToString()));
                    cell.CellStyle = borderstyle;

                    cell = row.CreateCell(5);
                    cell.SetCellValue(Convert.ToDouble(dt.Rows[j]["Qty"].ToString()));
                    cell.CellStyle = borderstyle;

                    cell = row.CreateCell(6);
                    cell.SetCellValue(Convert.ToDouble(dt.Rows[j]["TaxoffPrice"].ToString()));
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

                    if (dt.Rows[j]["IsLocal"].ToString().Equals("1"))
                    {
                        cell = row.CreateCell(11);
                        cell.SetCellValue(dt.Rows[j]["ContractNO"].ToString());
                        cell.CellStyle = borderstyle;
                    }
                    else
                    {
                        cell = row.CreateCell(11);
                        cell.SetCellValue(dt.Rows[j]["OrderNo"].ToString());
                        cell.CellStyle = borderstyle;
                    }

                    cell = row.CreateCell(12);
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
                    cell.SetCellValue(Convert.ToDouble(dt.Rows[0]["TotalTaxOffAmount"]));
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


                if (dt.Rows[0]["InvoiceDeliveryMethod"].ToString().Equals("1"))
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
                else if (dt.Rows[0]["InvoiceDeliveryMethod"].ToString().Equals("2"))
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

        public static void Profit2Excel(DataTable dt, Dictionary<string, decimal> SumCurrency, string savePath)
        {
            string[] fieldArr = { "OrderCreateTime", "CompanyName", "OrderNO",
                                  "OrderTotalDeclarePrice", "OrderTotalDeclareCNYPrice", "RealExchangeRate", "CustomsExchangeRate", "ServiceChargePoint",
                                 "PaymentAgency", "PaymentIncidental", "ValueAddTax", "Tariff", "TotalReceipt","ProfitDate",
                                 "PaidValueAddTax","PaidTariff","PaidPaymentIncidental","PaidTotalReceipt","OrderProfit","UserName","RegisterTime","CommissionProportion","Profit"
                                };
            System.Collections.Generic.Dictionary<string, string> fieldDic = new System.Collections.Generic.Dictionary<string, string> {
                {"OrderCreateTime","报关日期" },{ "CompanyName", "客户" }, { "OrderNO", "订单号" }, { "OrderTotalDeclarePrice", "报关货值(USD)" }, { "OrderTotalDeclareCNYPrice", "报关货值(RMB)" },
                { "RealExchangeRate", "实时汇率" }, { "CustomsExchangeRate", "海关汇率" }, { "ServiceChargePoint", "代理费率" }, { "PaymentAgency", "代理费" }, { "PaymentIncidental", "杂费" },
                { "ValueAddTax", "增值税" }, { "Tariff", "关税" }, { "TotalReceipt", "税代合计(RMB)" },{ "ProfitDate", "收款日期" }, { "PaidValueAddTax", "增值税" } ,
                { "PaidTariff", "关税" }, { "PaidPaymentIncidental", "杂费" }, { "PaidTotalReceipt", "费用合计" }, { "OrderProfit", "订单利润" },{ "UserName", "业务员" }, { "RegisterTime","注册时间"}, { "CommissionProportion", "提成比例" } ,
                { "Profit", "提成" }
            };

            DT2Excel(dt, SumCurrency, savePath, fieldArr, fieldDic);
        }

        private static void DT2Excel(DataTable dt, Dictionary<string, decimal> SumCurrency, string savePath, string[] fieldArr, System.Collections.Generic.Dictionary<string, string> fieldDic)
        {
            // xssfworkbook
            XSSFWorkbook wb = new XSSFWorkbook();
            // HSSFWorkbook wb = new HSSFWorkbook();

            //字体
            IFont font1 = wb.CreateFont();//字体
            font1.FontName = "楷体";
            font1.Color = NPOI.HSSF.Util.HSSFColor.Black.Index;//字体颜色
            font1.Boldweight = (short)FontBoldWeight.Normal;//字体加粗

            //标题样式样式
            ICellStyle titleStyle = wb.CreateCellStyle();
            titleStyle.SetFont(font1);//设置字体
            //设置背景色
            titleStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey50Percent.Index;
            titleStyle.FillPattern = FillPattern.SolidForeground;
            titleStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.Grey50Percent.Index;
            titleStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;//文字水平对齐方式
            titleStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;//文字垂直对齐方式           

            #region colors
            #region color1
            ICellStyle color1style = wb.CreateCellStyle();
            color1style.SetFont(font1);
            color1style.Alignment = HorizontalAlignment.Center;
            color1style.VerticalAlignment = VerticalAlignment.Center;
            color1style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Turquoise.Index;
            color1style.FillPattern = FillPattern.SolidForeground;
            color1style.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.Turquoise.Index;
            color1style.BorderBottom = BorderStyle.Thin;
            color1style.BorderLeft = BorderStyle.Thin;
            color1style.BorderRight = BorderStyle.Thin;
            color1style.BorderTop = BorderStyle.Thin;
            #endregion

            #region color2
            ICellStyle color2style = wb.CreateCellStyle();
            color2style.SetFont(font1);
            color2style.Alignment = HorizontalAlignment.Center;
            color2style.VerticalAlignment = VerticalAlignment.Center;
            color2style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Pink.Index;
            color2style.FillPattern = FillPattern.SolidForeground;
            color2style.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.Pink.Index;
            color2style.BorderBottom = BorderStyle.Thin;
            color2style.BorderLeft = BorderStyle.Thin;
            color2style.BorderRight = BorderStyle.Thin;
            color2style.BorderTop = BorderStyle.Thin;
            #endregion

            #region color3
            ICellStyle color3style = wb.CreateCellStyle();
            color3style.SetFont(font1);
            color3style.Alignment = HorizontalAlignment.Center;
            color3style.VerticalAlignment = VerticalAlignment.Center;
            color3style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Turquoise.Index;
            color3style.FillPattern = FillPattern.SolidForeground;
            color3style.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.Turquoise.Index;
            color3style.BorderBottom = BorderStyle.Thin;
            color3style.BorderLeft = BorderStyle.Thin;
            color3style.BorderRight = BorderStyle.Thin;
            color3style.BorderTop = BorderStyle.Thin;
            #endregion

            #region color4
            ICellStyle color4style = wb.CreateCellStyle();
            color4style.SetFont(font1);
            color4style.Alignment = HorizontalAlignment.Center;
            color4style.VerticalAlignment = VerticalAlignment.Center;
            color4style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.SkyBlue.Index;
            color4style.FillPattern = FillPattern.SolidForeground;
            color4style.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.SkyBlue.Index;
            color4style.BorderBottom = BorderStyle.Thin;
            color4style.BorderLeft = BorderStyle.Thin;
            color4style.BorderRight = BorderStyle.Thin;
            color4style.BorderTop = BorderStyle.Thin;
            #endregion

            #region color5
            ICellStyle color5style = wb.CreateCellStyle();
            color5style.SetFont(font1);
            color5style.Alignment = HorizontalAlignment.Center;
            color5style.VerticalAlignment = VerticalAlignment.Center;
            color5style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Red.Index;
            color5style.FillPattern = FillPattern.SolidForeground;
            color5style.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.Red.Index;
            color5style.BorderBottom = BorderStyle.Thin;
            color5style.BorderLeft = BorderStyle.Thin;
            color5style.BorderRight = BorderStyle.Thin;
            color5style.BorderTop = BorderStyle.Thin;
            #endregion

            #region color6
            ICellStyle color6style = wb.CreateCellStyle();
            color6style.SetFont(font1);
            color6style.Alignment = HorizontalAlignment.Center;
            color6style.VerticalAlignment = VerticalAlignment.Center;
            color6style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LemonChiffon.Index;
            color6style.FillPattern = FillPattern.SolidForeground;
            color6style.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.LemonChiffon.Index;
            color6style.BorderBottom = BorderStyle.Thin;
            color6style.BorderLeft = BorderStyle.Thin;
            color6style.BorderRight = BorderStyle.Thin;
            color6style.BorderTop = BorderStyle.Thin;
            #endregion

            #region color7
            ICellStyle color7style = wb.CreateCellStyle();
            color7style.SetFont(font1);
            color7style.Alignment = HorizontalAlignment.Center;
            color7style.VerticalAlignment = VerticalAlignment.Center;
            color7style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Coral.Index;
            color7style.FillPattern = FillPattern.SolidForeground;
            color7style.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.Coral.Index;
            color7style.BorderBottom = BorderStyle.Thin;
            color7style.BorderLeft = BorderStyle.Thin;
            color7style.BorderRight = BorderStyle.Thin;
            color7style.BorderTop = BorderStyle.Thin;
            #endregion
            #endregion

            #region 设置单元格边框
            ICellStyle borderstyle = wb.CreateCellStyle();
            borderstyle.BorderBottom = BorderStyle.Thin;
            borderstyle.BorderLeft = BorderStyle.Thin;
            borderstyle.BorderRight = BorderStyle.Thin;
            borderstyle.BorderTop = BorderStyle.Thin;
            #endregion
            //创建一个表单
            ISheet sheet = wb.CreateSheet("Sheet0");

            IRow row;
            NPOI.SS.UserModel.ICell cell;

            //写入标题行
            #region
            //第一行          

            row = sheet.CreateRow(0);

            cell = row.CreateCell(0);
            cell.SetCellValue("报关日期");
            cell.CellStyle = color1style;

            cell = row.CreateCell(1);
            cell.SetCellValue("客户");
            cell.CellStyle = color1style;

            cell = row.CreateCell(2);
            cell.SetCellValue("订单号");
            cell.CellStyle = color2style;

            cell = row.CreateCell(3);
            cell.SetCellValue("报关货值(USD)");
            cell.CellStyle = color1style;

            cell = row.CreateCell(4);
            cell.SetCellValue("报关货值(RMB)");
            cell.CellStyle = color3style;

            cell = row.CreateCell(5);
            cell.SetCellValue("实时汇率");
            cell.CellStyle = color4style;

            cell = row.CreateCell(6);
            cell.SetCellValue("海关汇率");
            cell.CellStyle = color4style;

            cell = row.CreateCell(7);
            cell.SetCellValue("税金 代理费RMB");
            cell.CellStyle = color5style;
            cell = row.CreateCell(8);
            cell.SetCellValue("");
            cell.CellStyle = color5style;
            cell = row.CreateCell(9);
            cell.SetCellValue("");
            cell.CellStyle = color5style;
            cell = row.CreateCell(10);
            cell.SetCellValue("");
            cell.CellStyle = color5style;
            cell = row.CreateCell(11);
            cell.SetCellValue("");
            cell.CellStyle = color5style;
            cell = row.CreateCell(12);
            cell.SetCellValue("");
            cell.CellStyle = color5style;
            cell = row.CreateCell(13);
            cell.SetCellValue("");
            cell.CellStyle = color5style;

            cell = row.CreateCell(14);
            cell.SetCellValue("费用");
            cell.CellStyle = color6style;
            cell = row.CreateCell(15);
            cell.SetCellValue("");
            cell.CellStyle = color6style;
            cell = row.CreateCell(16);
            cell.SetCellValue("");
            cell.CellStyle = color6style;
            cell = row.CreateCell(17);
            cell.SetCellValue("");
            cell.CellStyle = color6style;

            cell = row.CreateCell(18);
            cell.SetCellValue("订单利润");
            cell.CellStyle = color7style;

            cell = row.CreateCell(19);
            cell.SetCellValue("提成核算");
            cell.CellStyle = color7style;
            cell = row.CreateCell(20);
            cell.SetCellValue("");
            cell.CellStyle = color7style;
            cell = row.CreateCell(21);
            cell.SetCellValue("");
            cell.CellStyle = color7style;
            cell = row.CreateCell(22);
            cell.SetCellValue("");
            cell.CellStyle = color7style;


            //第二行
            row = sheet.CreateRow(1);
            cell = row.CreateCell(0);
            cell = row.CreateCell(1);
            cell = row.CreateCell(2);
            cell = row.CreateCell(3);
            cell = row.CreateCell(4);
            cell = row.CreateCell(5);
            cell = row.CreateCell(6);

            cell = row.CreateCell(7);
            cell.SetCellValue("代理费率");
            cell.CellStyle = color5style;
            cell = row.CreateCell(8);
            cell.SetCellValue("代理费");
            cell.CellStyle = color5style;
            cell = row.CreateCell(9);
            cell.SetCellValue("杂费");
            cell.CellStyle = color5style;
            cell = row.CreateCell(10);
            cell.SetCellValue("增值税");
            cell.CellStyle = color5style;
            cell = row.CreateCell(11);
            cell.SetCellValue("关税");
            cell.CellStyle = color5style;
            cell = row.CreateCell(12);
            cell.SetCellValue("税代合计(RMB)");
            cell.CellStyle = color5style;
            cell = row.CreateCell(13);
            cell.SetCellValue("收款日期");
            cell.CellStyle = color5style;

            cell = row.CreateCell(14);
            cell.SetCellValue("增值税");
            cell.CellStyle = color6style;
            cell = row.CreateCell(15);
            cell.SetCellValue("关税");
            cell.CellStyle = color6style;
            cell = row.CreateCell(16);
            cell.SetCellValue("杂费");
            cell.CellStyle = color6style;
            cell = row.CreateCell(17);
            cell.SetCellValue("费用合计");
            cell.CellStyle = color6style;

            cell = row.CreateCell(18);

            cell = row.CreateCell(19);
            cell.SetCellValue("业务员");
            cell.CellStyle = color7style;
            cell = row.CreateCell(20);
            cell.SetCellValue("注册时间");
            cell.CellStyle = color7style;
            cell = row.CreateCell(21);
            cell.SetCellValue("提成比例");
            cell.CellStyle = color7style;
            cell = row.CreateCell(22);
            cell.SetCellValue("提成");
            cell.CellStyle = color7style;

            #endregion

            #region 合并表头单元格
            sheet.AddMergedRegion(new CellRangeAddress(0, 1, 0, 0));
            sheet.AddMergedRegion(new CellRangeAddress(0, 1, 1, 1));
            sheet.AddMergedRegion(new CellRangeAddress(0, 1, 2, 2));
            sheet.AddMergedRegion(new CellRangeAddress(0, 1, 3, 3));
            sheet.AddMergedRegion(new CellRangeAddress(0, 1, 4, 4));
            sheet.AddMergedRegion(new CellRangeAddress(0, 1, 5, 5));
            sheet.AddMergedRegion(new CellRangeAddress(0, 1, 6, 6));

            sheet.AddMergedRegion(new CellRangeAddress(0, 0, 7, 13));

            sheet.AddMergedRegion(new CellRangeAddress(0, 0, 14, 17));
            sheet.AddMergedRegion(new CellRangeAddress(0, 1, 18, 18));
            sheet.AddMergedRegion(new CellRangeAddress(0, 0, 19, 22));
            #endregion

            #region
            //写入数据，从第2行开始
            for (int i = 1; i <= dt.Rows.Count + 1; i++)
            {
                row = sheet.CreateRow(i + 1);//创建第i行
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
                        SetCellValue(cell, obj);
                        cell.CellStyle = borderstyle;
                    }
                }
            }

            //写入币种汇总数据
            sheet.CreateRow(sheet.LastRowNum);
            foreach (var v in SumCurrency)
            {
                row = sheet.CreateRow(sheet.LastRowNum + 1);

                cell = row.CreateCell(2);
                SetCellValue(cell, v.Key);
                cell.CellStyle = borderstyle;

                cell = row.CreateCell(3);
                SetCellValue(cell, v.Value);
                cell.CellStyle = borderstyle;
            }

            #endregion


            FileStream file = new FileStream(savePath, FileMode.Create);
            wb.Write(file);
            file.Close();
        }

        public static void HistoryOrderExcel(List<DataTable> dts, string savePath)
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
            titleStyle.SetFont(font1);//设置字体
            //设置背景色
            titleStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index;
            titleStyle.FillPattern = FillPattern.SolidForeground;
            titleStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.Blue.Index;
            titleStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;//文字水平对齐方式
            titleStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;//文字垂直对齐方式


            for (int idt = 0; idt < dts.Count; idt++)
            {
                DataTable dt = dts[idt];

                //创建一个表单
                ISheet sheet = wb.CreateSheet(dt.Rows[0]["DeclarationDate"].ToString());

                //设置列宽
                int[] columnWidth = { 10, 10, 25, 10, 15, 15, 20, 10, 20, 20, 15, 15 };
                for (int i = 0; i < columnWidth.Length; i++)
                {
                    //设置列宽度，256*字符数，因为单位是1/256个字符
                    sheet.SetColumnWidth(i, 256 * columnWidth[i]);
                }

                IRow row;
                NPOI.SS.UserModel.ICell cell;

                string[] fieldArr = { "NO","CustomsCode","DeclareProductName","Elements","Quantity","Unit","FirstQuantity","FirstLegalUnit","SecondQuantity","SecondLegalUnit","PlaceOfProduction",
            "UnitPrice","TotalDeclarePrice","Currency","Mianzheng","Version","Record","Destination","PackQuantity","NetWeight","GrossWeight","PackNO","TemPackNO","GoodsNO","InvoiceProductName",
            "Function","Brand","Model","PlaceOfProduction2","Quantity2","NetWeight2","UnitPrice2","TotalDeclarePrice2","OrderCreateTime","DeclareCompany","DeclareNO","AgencyFee","Tariff",
            "OtherFee","CompanyName","Supplier","IsInspection","PackagingType","ModelName","ModleValue" };

                System.Collections.Generic.Dictionary<string, string> fieldDic = new System.Collections.Generic.Dictionary<string, string> {
             { "NO", "序号" }, { "CustomsCode", "海关编码" }, { "DeclareProductName", "商品名称" }, { "Elements", "规格型号" }, { "Quantity", "成交数量" },
             { "Unit", "成交单位" }, { "FirstQuantity", "法定数量" }, { "FirstLegalUnit", "法定单位" }, { "SecondQuantity", "第二数量" }, { "SecondLegalUnit", "第二单位" },
             { "PlaceOfProduction", "原产地" }, { "UnitPrice", "单价" }, { "TotalDeclarePrice", "金额" }, { "Currency", "币值" }, { "Mianzheng", "免征" },
             { "Version", "版本" }, { "Record", "备案序号" }, { "Destination", "最终目的国" }, { "PackQuantity", "件数" }, { "NetWeight", "净重" },
             { "GrossWeight", "毛重" }, { "PackNO", "箱号" }, { "TemPackNO", "临时箱号" }, { "GoodsNO", "商品编码" }, { "InvoiceProductName", "品名" },
             { "Function", "功能" }, { "Brand", "品牌" }, { "Model", "规格型号" }, { "PlaceOfProduction2", "产地" }, { "Quantity2", "数量(PCS)" },
             { "NetWeight2", "净重" }, { "UnitPrice2", "报关单价" }, { "TotalDeclarePrice2", "报关总价" }, { "OrderCreateTime", "制单日期" }, { "DeclareCompany", "报关公司" },
             { "DeclareNO", "报关单号" }, { "AgencyFee", "佣金" }, { "Tariff", "关税" }, { "OtherFee", "其他费用" }, { "CompanyName", "下单公司" },
             { "Supplier", "付款公司" },{ "IsInspection", "是否商检" }, { "PackagingType", "是否特殊包装" }, { "ModelName", "型号信息分类" }, { "ModleValue", "型号信息分类值" },

            };

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

                            SetCellValue(cell, obj);

                        }
                    }
                }
            }
            //创建一个表单


            FileStream file = new FileStream(savePath, FileMode.Create);
            wb.Write(file);
            file.Close();


        }

        /// <summary>
        /// 导出Icgoo信息
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="savePath"></param>
        public static void NPOIExcelIcgoo(DataTable dt, string savePath, string[] fieldArr, System.Collections.Generic.Dictionary<string, string> fieldDic)
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
            int[] columnWidth = { 10, 10, 25, 10, 15, 15, 20, 10, 20, 20, 15, 15 };
            for (int i = 0; i < columnWidth.Length; i++)
            {
                //设置列宽度，256*字符数，因为单位是1/256个字符
                sheet.SetColumnWidth(i, 256 * columnWidth[i]);
            }

            IRow row;
            NPOI.SS.UserModel.ICell cell;



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
                        SetCellValue(cell, obj);
                    }
                }
            }


            FileStream file = new FileStream(savePath, FileMode.Create);
            wb.Write(file);
            file.Close();


        }
    }

}
