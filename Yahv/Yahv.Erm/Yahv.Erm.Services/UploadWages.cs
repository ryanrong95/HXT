using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Yahv.Erm.Services.Views.Rolls;
using Yahv.Utils;
using Yahv.Utils.Serializers;

namespace Yahv.Erm.Services
{
    /// <summary>
    /// 工资表上传
    /// </summary>
    /*abstract*/
    public class UploadWages
    {
        #region 状态
        public class _Status
        {
            /// <summary>
            /// 文件个数
            /// </summary>
            public int FileCount { get; internal set; }

            /// <summary>
            /// 完成个数
            /// </summary>
            public int Completed { get; internal set; }

            /// <summary>
            /// 当前文件总行数
            /// </summary>
            public int TotalData { get; internal set; }

            /// <summary>
            /// 当前文件读取数
            /// </summary>
            public int ReadData { get; internal set; }

            /// <summary>
            /// 读取百分比
            /// </summary>
            public decimal TotalReadRate
            {
                get
                {
                    return (decimal)ReadData / (decimal)TotalData;
                }
            }

            public _Status()
            {
                this.FileCount = 0;
                this.Completed = 0;
            }
        }

        public _Status Status { get; set; }
        #endregion

        UploadWages()
        {
            Thread mainThread;
            (mainThread = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        this.DoSomething();
                    }
                    catch (ThreadAbortException ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        Thread.Sleep(1000);
                    }
                }
            })).Start();
        }

        void DoSomething()
        {
            DirectoryInfo di = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Upload\"));
            var files = di.GetFiles("*.wages.*");
            if (files.Length <= 0) return;

            this.Status = new _Status
            {
                FileCount = files.Length,
            };

            DataTable dt;

            foreach (var fi in files)
            {
                //Excel导入成Datable
                dt = ExcelToTable(fi.FullName);

                using (var roll = new StaffPayItems())
                {
                    this.Status.ReadData = 1;
                    this.Status.TotalData = dt.Rows.Count;

                    //将部分行转为json
                    var list = JArray.FromObject(dt, JsonSerializer.CreateDefault(new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include }));
                    int count = 10;     //一次向存储过程提交的数据个数
                    for (int i = 0; i < list.Count; i = i + count)
                    {
                        roll.Import(list.Skip(i).Take(count).Json(), fi.Name.Split('-')[0], fi.Name.Split('-')[1]);


                        if (i != 0)
                            this.Status.ReadData += count;
                        else
                            this.Status.ReadData = count > list.Count ? list.Count : count;
                    }

                    this.Status.Completed += 1;
                }


                #region Test
                //this.Status.TotalData = 500;

                //for (int i = 0; i < 500; i = i + 5)
                //{
                //    Thread.Sleep(500);

                //    this.Status.ReadData += 5;
                //}

                //this.Status.Completed += 1;
                #endregion


                //修改以上所有文件额名称
                FileHelper.Rename(fi.FullName, fi.Name.Replace(".wages.", ".delete."));
            }





        }

        //abstract protected void Read(FileInfo fi);
        public void Execute()
        {

        }

        #region 单例
        static object locker = new object();
        static UploadWages current;
        static public UploadWages Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new UploadWages();
                        }
                    }
                }

                return current;
            }
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// Excel导入成Datable
        /// </summary>
        /// <param name="file">导入路径(包含文件名与扩展名)</param>
        /// <returns></returns>
        private DataTable ExcelToTable(string file)
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
