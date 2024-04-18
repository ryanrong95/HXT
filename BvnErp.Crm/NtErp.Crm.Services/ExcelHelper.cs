using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using System.Reflection;
using System.Web;

namespace NtErp.Crm.Services
{
    public class ExcelHelper
    {
        #region 单例
        static object locker = new object();
        static ExcelHelper current;
        static public ExcelHelper Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new ExcelHelper();
                        }
                    }
                }

                return current;
            }
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 从Excel中读取数据
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        protected ISheet GetSheet(FileStream fs, string filepath)
        {
            IWorkbook workbook = GetWorkbook(fs, filepath);
            ISheet sheet = workbook.GetSheetAt(0);
            return sheet;
        }

        /// <summary>
        /// 读取workbook
        /// </summary>
        /// <param name="fs">文件流</param>
        /// <param name="filepath">路径</param>
        /// <returns></returns>
        protected IWorkbook GetWorkbook(FileStream fs, string filepath)
        {
            IWorkbook workbook;

            string fileExt = Path.GetExtension(filepath).ToLower(); //文件后缀

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
                    workbook = new XSSFWorkbook(filepath);
                }
            }
            else
            {
                workbook = null;
            }

            return workbook;

        }


        /// <summary>
        /// Excel导入成Datable
        /// </summary>
        /// <returns></returns>
        public DataTable ExcelToTable(string filepath)
        {
            DataTable dt = new DataTable();
            using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                ISheet sheet = this.GetSheet(fs, filepath);

                //表头(第二行为表头，如果第二行没有数据，直接取第一行)
                IRow header = sheet.GetRow(1);

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

        /// <summary>
        /// Excel导入成IEnumerable
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ExcelProject> ExcelToLinq(string filepath)
        {
            List<ExcelProject> projects = new List<ExcelProject>();
            using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                ISheet sheet = this.GetSheet(fs, filepath);
                //表头(第二行为表头，如果第二行没有数据，直接取第一行)
                IRow header = sheet.GetRow(1);
                List<string> colums = new List<string>();

                for (int i = 0; i < header.LastCellNum; i++)
                {
                    var obj1 = GetValue(header.GetCell(i));
                    if (obj1 != null && obj1.ToString() != string.Empty)
                    {
                        colums.Add(obj1.ToString());
                    }
                    else
                    {
                        colums.Add("");
                    }
                }

                //数据  第三行开始
                for (int i = sheet.FirstRowNum + 2; i <= sheet.LastRowNum; i++)
                {
                    //隐藏行不读取
                    if (sheet.GetRow(i).ZeroHeight)
                    {
                        continue;
                    }
                    var project = new ExcelProject();
                    project.Line = i + 1;
                    bool flag = false;
                    for (int j = 0; j < colums.Count(); j++)
                    {
                        //隐藏列不读取
                        if (sheet.IsColumnHidden(j))
                        {
                            continue;
                        }
                        //var value = GetValue(sheet.GetRow(i).GetCell(j))?.ToString().Trim();
                        var cellValue = sheet.GetRow(i).GetCell(j);
                        string value = string.Empty;
                        if (cellValue != null && cellValue.CellType == CellType.Numeric && DateUtil.IsCellDateFormatted(cellValue))
                        {
                            value = cellValue.DateCellValue.ToShortDateString();
                        }
                        else
                        {
                            value = cellValue?.ToString().Trim();
                        }


                        var property = project.GetType().GetProperty(colums[j]);
                        if (property != null)
                        {
                            //设置值
                            property.SetValue(project, value);
                        }

                        if (!string.IsNullOrEmpty(value))
                        {
                            flag = true;
                        }
                    }
                    if (flag)
                    {
                        projects.Add(project);
                    }
                }
            }
            return projects.ToArray();
        }

        /// <summary>
        /// 导出销售机会文件
        /// </summary>
        /// <param name="fileName">文件</param>
        public void ExportProjectByWeb(string fileName, List<Models.ProjectExcelData> data)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = this.GetWorkbook(fs, fileName);
                ISheet sheet = workbook.GetSheetAt(0);
                IRow row = sheet.GetRow(1); //得到表头
                int cloumn = row.LastCellNum + 1;
                row.CreateCell(cloumn).SetCellValue("导入结果");

                foreach (var item in data)
                {
                    IRow currentRow = sheet.GetRow(item.ExcelProject.Line - 1);
                    ICell cell = currentRow.CreateCell(cloumn);
                    cell.SetCellValue(string.IsNullOrWhiteSpace(item.ExcelProject.Message) ? "成功" : item.ExcelProject.Message);
                }
                
                // 写入文件
                FileStream sw = File.Create(fileName);
                workbook.Write(sw);
                sw.Close();

                string name = fileName.Substring(fileName.LastIndexOf('\\') + 1, fileName.Length - Path.GetExtension(fileName).Length - fileName.LastIndexOf('\\') - 1);
                
                FileInfo fileInfo = new FileInfo(fileName);

                HttpContext curContext = HttpContext.Current;
                curContext.Response.Clear();
                curContext.Response.ClearContent();
                curContext.Response.ClearHeaders();
                curContext.Response.AddHeader("Content-Disposition", "attachment;filename=" + name + Path.GetExtension(fileName).ToLower());
                curContext.Response.AddHeader("Content-Length", fileInfo.Length.ToString());
                curContext.Response.AddHeader("Content-Transfer-Encoding", "binary");
                curContext.Response.ContentType = "application/octet-stream";
                curContext.Response.ContentEncoding = System.Text.Encoding.GetEncoding("gb2312");
                curContext.Response.WriteFile(fileInfo.FullName);
                curContext.Response.Flush();
                if (System.IO.File.Exists(fileName))
                {
                    System.IO.File.Delete(fileName);
                }
                curContext.Response.End();
            }

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
                    return cell.NumericCellValue;
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

    public class ExcelProject
    {
        /// <summary>
        /// 行号
        /// </summary>
        public int Line { get; set; }
        public string 机会类型 { get; set; }

        public string 项目名称 { get; set; }

        public string 客户名称 { get; set; }

        public string 产品全称 { get; set; }

        public string 行业 { get; set; }

        public string 我方公司 { get; set; }

        public string 币种 { get; set; }

        public string 联系人 { get; set; }

        public string 联系电话 { get; set; }

        public string 地址 { get; set; }

        public string 型号 { get; set; }
        public string 型号全称 { get; set; }
        public string 销售状态 { get; set; }
        public string 品牌 { get; set; }
        public string 单机用量 { get; set; }
        public string 项目用量 { get; set; }
        public string 参考单价 { get; set; }
        public string 预计成交量 { get; set; }

        public string 预计成交日期 { get; set; }
        public string 预计成交概率 { get; set; }
        public string 竞争对手型号 { get; set; }
        public string 竞争对手品牌 { get; set; }
        public string 竞争对手单价 { get; set; }

        public string FAE { get; set; }
        public string PM { get; set; }
        public string Sales { get; set; }
        public string CS { get; set; }
        public string MSO { get; set; }

        public string 送样类型 { get; set; }
        public string 送样时间 { get; set; }
        public string 送样数量 { get; set; }
        public string 送样单价 { get; set; }
        public string 送样金额 { get; set; }
        public string 送样联系人 { get; set; }
        public string 送样联系电话 { get; set; }
        public string 送样联系地址 { get; set; }

        public string 报备时间 { get; set; }
        public string 批复时间 { get; set; }
        public string 批复单价 { get; set; }
        public string 原厂型号 { get; set; }
        public string 原厂RFQ号 { get; set; }
        public string MOQ { get; set; }
        public string MPQ { get; set; }
        public string 询价币种 { get; set; }
        public string 汇率 { get; set; }
        public string 税率 { get; set; }
        public string 关税点 { get; set; }
        public string 其他附加点 { get; set; }
        public string 含税人民币成本价 { get; set; }
        public string 有效时间 { get; set; }
        public string 有效数量 { get; set; }
        public string 参考售价 { get; set; }
        public string 特殊备注 { get; set; }

        public string Message { get; set; }
    }
}
