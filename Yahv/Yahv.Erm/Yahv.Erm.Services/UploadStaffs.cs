using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Yahv.Erm.Services
{
    public class UploadStaffs
    {
        #region 单例
        static object locker = new object();
        static UploadStaffs current;
        static public UploadStaffs Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new UploadStaffs();
                        }
                    }
                }

                return current;
            }
        }
        #endregion

        #region 公共方法
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

                //表头(第二行为表头，如果第二行没有数据，直接取第一行)
                IRow header = sheet.GetRow(0);

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
                //数据  第三行开始
                for (int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
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

        #region 私有方法
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