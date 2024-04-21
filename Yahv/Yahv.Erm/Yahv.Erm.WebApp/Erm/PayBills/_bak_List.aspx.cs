//using System.Linq.Dynamic;
//using ICSharpCode.SharpZipLib.Checksum;
//using ICSharpCode.SharpZipLib.Zip;
//using NPOI.HSSF.UserModel;
//using NPOI.SS.UserModel;
//using NPOI.SS.Util;
//using NPOI.XSSF.UserModel;
using Yahv.Web.Erp;
//using DynamicExpression = System.Linq.Dynamic.DynamicExpression;

namespace Yahv.Erm.WebApp.Erm.PayBills
{
    public partial class _bak_List : ErpParticlePage
    {
        //#region 变量
        //const string GZHJ = "工资合计";
        //const string ZXKC = "专项扣除";     //五险一金
        //const string MSSR = "免税收入";
        //const string ZXFJKC = "专项附加扣除";
        //const string BYGS = "本月个税";

        //const string SFZH = "身份证号码";
        //const string XM = "姓名";
        //const string GH = "ID（工号）";

        //const decimal MSJE = 5000;      //免税收入

        ///// <summary>
        ///// 员工ID
        ///// </summary>
        //public string Arg_StaffID { get; set; }

        ///// <summary>
        ///// 工资日期
        ///// </summary>
        //public string Arg_DateIndex { get; set; }
        //#endregion

        //#region Page_Load

        //protected void Page_Load(object sender, EventArgs e)
        //{
        //    if (!IsPostBack)
        //    {
        //        wageDate.Value = DateTime.Now.AddMonths(-1).ToString("yyyy-MM");
        //        string dateIndex = wageDate.Value.Replace("-", "");
        //        Model = GetData(item => item.DateIndex == dateIndex);
        //    }
        //}

        //#endregion

        //#region 功能按钮

        ///// <summary>
        ///// 导入
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //protected void btnImport_Click(object sender, EventArgs e)
        //{
        //    Stopwatch sw = new Stopwatch();
        //    sw.Start();

        //    string fileName = UploadFile();
        //    if (string.IsNullOrWhiteSpace(fileName))
        //    {
        //        Easyui.Alert("操作提示", "上传文件失败!", Sign.Error);
        //        return;
        //    }

        //    Dictionary<int, string> errorMsgs = new Dictionary<int, string>();

        //    var dt = ExcelToTable(fileName);

        //    InsertData(dt, out errorMsgs);

        //    //导出错误excel
        //    if (errorMsgs.Count > 0)
        //    {
        //        MakeErrorExcel(fileName, errorMsgs);
        //    }

        //    sw.Stop();
        //    Easyui.Alert("操作提示", $"导入成功，用时{sw.ElapsedMilliseconds / 1000}秒");

        //    btnSearch_Click(null, null);
        //}

        //protected void btnImport1_Click(object sender, EventArgs e)
        //{
        //    Stopwatch sw = new Stopwatch();
        //    sw.Start();

        //    string fileName = UploadFile();
        //    if (string.IsNullOrWhiteSpace(fileName))
        //    {
        //        Easyui.Alert("操作提示", "上传文件失败!", Sign.Error);
        //        return;
        //    }

        //    string dateIndex = wageDate.Value.Replace("-", "");     //工资日期

        //    Dictionary<int, string> errorMsgs = new Dictionary<int, string>();

        //    var dt = ExcelToTable(fileName);
        //    string msg = string.Empty;      //错误消息

        //    using (var roll = new StaffPayItems())
        //    {
        //        var list = JArray.FromObject(dt, JsonSerializer.CreateDefault(new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        //        int count = 10;
        //        for (int i = 0; i < list.Count; i = i + count)
        //        {
        //            msg = msg + roll.Import(list.Skip(i).Take(count).Json(), dateIndex, Erp.Current.ID);
        //        }
        //    }

        //    sw.Stop();

        //    Easyui.Alert("操作提示", $"导入成功，用时{sw.ElapsedMilliseconds / 1000}秒" + msg);

        //    btnSearch_Click(null, null);
        //}

        ///// <summary>
        ///// 搜索
        ///// </summary>
        //protected void btnSearch_Click(object sender, EventArgs e)
        //{
        //    if (string.IsNullOrWhiteSpace(wageDate.Value))
        //    {
        //        Easyui.Alert("操作提示", "请您选择工资日期", Sign.Error);
        //        return;
        //    }

        //    Model = GetData(GetExpression());
        //}

        ///// <summary>
        ///// 保存
        ///// </summary>
        ///// <returns></returns>
        //protected object save()
        //{
        //    try
        //    {
        //        var data = Request["source"].Replace("&quot;", "\"").JsonTo<PayItem[]>();

        //        if (data.Length <= 0)
        //        {
        //            return new
        //            {
        //                code = 400,
        //                message = "未修改数据!"
        //            };
        //        }

        //        var list = RemoveDuplicate(data);

        //        //判断是否已经封账
        //        string dateIndex = Request["dateIndex"].Replace("-", "");
        //        if (IsClosed(dateIndex))
        //        {
        //            return new
        //            {
        //                code = 400,
        //                message = $"{dateIndex}已经封账!"
        //            };
        //        }

        //        //普通工资项计算
        //        foreach (var payItem in data)
        //        {
        //            UpdatePayItemByHandstable(payItem);      //工资项修改
        //        }


        //        string staffId = string.Empty;
        //        //计算列工资项
        //        foreach (var payId in data.ToList().GroupBy(t => t.PayID).Select(t => t.Key))
        //        {
        //            staffId = GetStaffIdByPayId(payId);
        //            InitArguments(staffId, dateIndex);        //初始化参数，为了自定义方法
        //            InsertOrUpdatePayItem_Calc(payId, dateIndex, JsonToDataTable(GetDataByPayId(payId).Json()).Rows[0], GetMyWageItems(staffId));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return new
        //        {
        //            code = 400,
        //            message = "异常错误：" + ex.Message
        //        };
        //    }

        //    return new
        //    {
        //        code = 200,
        //    };
        //}

        ///// <summary>
        ///// 封账
        ///// </summary>
        ///// <returns></returns>
        //protected object closed()
        //{
        //    try
        //    {
        //        //判断是否已经封账
        //        string dateIndex = Request["dateIndex"].Replace("-", "");
        //        if (IsClosed(dateIndex))
        //        {
        //            return new
        //            {
        //                code = 400,
        //                message = $"{dateIndex}已经封账!"
        //            };
        //        }

        //        new PayBill().Closed(dateIndex);

        //        return new
        //        {
        //            code = 200
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new
        //        {
        //            code = 400,
        //            message = "异常错误：" + ex.Message
        //        };
        //    }
        //}

        ///// <summary>
        ///// 导出文件
        ///// </summary>
        //protected void btnExport_Click(object sender, EventArgs e)
        //{
        //    string file = GetTemplate();       //复制模板文件
        //    if (string.IsNullOrWhiteSpace(file))
        //    {
        //        Easyui.Alert("操作提示", "未找到模板文件!", Sign.Error);
        //        return;
        //    }

        //    var data = GetData(GetExpression());
        //    if (data.Count <= 0)
        //    {
        //        Easyui.Alert("操作提示", "未找到数据!", Sign.Error);
        //        return;
        //    }

        //    var dataTable = JsonToDataTable(data.Json());
        //    if (dataTable == null || dataTable.Rows.Count <= 0)
        //    {
        //        Easyui.Alert("操作提示", "未找到数据!", Sign.Error);
        //        return;
        //    }


        //    var companyies = from DataRow myRow in dataTable.Rows
        //                     group myRow by myRow["CompanyName"];

        //    string date = wageDate.Value.Replace("-", "年") + "月";
        //    string fileExt = Path.GetExtension(file).ToLower(); //文件后缀

        //    List<string> files = companyies.Select(company => MakeFinanceExcel(file, date + company.Key + "--给会计" + fileExt, dataTable.Select("CompanyName='" + company.Key + "'"), dataTable.Columns)).ToList();

        //    string fileName = Server.MapPath("~/Upload/") + DateTime.Now.ToString("yyyyMMddHHmmss") + ".zip";
        //    Zip(files.ToArray(), fileName);

        //    //下载文件
        //    DownLoadFile(fileName);

        //    btnSearch_Click(null, null);
        //}
        //#endregion

        //#region 私有函数

        ///// <summary>
        ///// Excel导入成Datable
        ///// </summary>
        ///// <param name="file">导入路径(包含文件名与扩展名)</param>
        ///// <returns></returns>
        //private DataTable ExcelToTable(string file)
        //{
        //    DataTable dt = new DataTable();
        //    IWorkbook workbook;

        //    string fileExt = Path.GetExtension(file).ToLower(); //文件后缀
        //    using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
        //    {
        //        //XSSFWorkbook 适用XLSX格式，HSSFWorkbook 适用XLS格式
        //        if (fileExt == ".xlsx")
        //        {
        //            workbook = new XSSFWorkbook(fs);
        //        }
        //        else if (fileExt == ".xls")
        //        {
        //            workbook = new HSSFWorkbook(fs);
        //        }
        //        else
        //        {
        //            workbook = null;
        //        }

        //        ISheet sheet = workbook.GetSheetAt(0);

        //        //表头(第三行为表头，如果第三行没有数据，直接取第二行)
        //        IRow header1 = sheet.GetRow(2);
        //        IRow header2 = sheet.GetRow(1);

        //        List<int> columns = new List<int>();
        //        for (int i = 0; i < header1.LastCellNum; i++)
        //        {
        //            var obj1 = GetValue(header1.GetCell(i));
        //            var obj2 = GetValue(header2.GetCell(i));

        //            if (obj1 != null && obj1.ToString() != string.Empty && !dt.Columns.Contains(obj1.ToString()))
        //            {
        //                dt.Columns.Add(new DataColumn(obj1.ToString()));
        //            }
        //            else if (obj2 != null && obj2.ToString() != string.Empty && !dt.Columns.Contains(obj2.ToString()))
        //            {
        //                dt.Columns.Add(new DataColumn(obj2.ToString()));
        //            }
        //            else
        //                dt.Columns.Add(new DataColumn("Columns" + i.ToString()));

        //            columns.Add(i);
        //        }
        //        //数据  第三行开始
        //        for (int i = sheet.FirstRowNum + 3; i <= sheet.LastRowNum; i++)
        //        {
        //            DataRow dr = dt.NewRow();
        //            bool hasValue = false;
        //            foreach (int j in columns)
        //            {
        //                dr[j] = GetValue(sheet.GetRow(i).GetCell(j));
        //                if (dr[j] != null && dr[j].ToString() != string.Empty)
        //                {
        //                    hasValue = true;
        //                }
        //            }
        //            if (hasValue)
        //            {
        //                dt.Rows.Add(dr);
        //            }
        //        }
        //    }
        //    return dt;
        //}

        ///// <summary>
        ///// 获取单元格
        ///// </summary>
        ///// <param name="cell"></param>
        ///// <returns></returns>
        //private object GetValue(ICell cell)
        //{
        //    if (cell == null)
        //        return null;
        //    switch (cell.CellType)
        //    {
        //        case CellType.Blank: //BLANK:  
        //            return null;
        //        case CellType.Boolean: //BOOLEAN:  
        //            return cell.BooleanCellValue;
        //        case CellType.Numeric: //NUMERIC:  
        //            return cell.NumericCellValue;
        //        case CellType.String: //STRING:  
        //            return cell.StringCellValue;
        //        case CellType.Error: //ERROR:  
        //            return cell.ErrorCellValue;
        //        case CellType.Formula: //FORMULA:  
        //            //return "=" + cell.CellFormula;
        //            return cell.NumericCellValue.ToString(CultureInfo.InvariantCulture);
        //        default:
        //            return cell.StringCellValue;
        //    }
        //}

        ///// <summary>
        ///// 复制行
        ///// </summary>
        ///// <param name="workbook"></param>
        ///// <param name="worksheet"></param>
        ///// <param name="sourceRow"></param>
        ///// <param name="errorRowNum"></param>
        //private void CopyRow(IWorkbook workbook, ISheet worksheet, IRow sourceRow, int errorRowNum)
        //{
        //    // Get the source / new row
        //    IRow newRow = worksheet.CreateRow(errorRowNum);

        //    // Loop through source columns to add to new row
        //    for (int i = 0; i < sourceRow.LastCellNum; i++)
        //    {
        //        // Grab a copy of the old/new cell
        //        ICell oldCell = sourceRow.GetCell(i);
        //        ICell newCell = newRow.CreateCell(i);

        //        // If the old cell is null jump to next cell
        //        if (oldCell == null)
        //        {
        //            newCell = null;
        //            continue;
        //        }

        //        // Copy style from old cell and apply to new cell
        //        try
        //        {
        //            ICellStyle newCellStyle = workbook.CreateCellStyle();
        //            newCellStyle.CloneStyleFrom(oldCell.CellStyle);
        //            ;
        //            newCell.CellStyle = newCellStyle;
        //        }
        //        catch (Exception)
        //        {
        //        }

        //        // If there is a cell comment, copy
        //        if (newCell.CellComment != null) newCell.CellComment = oldCell.CellComment;

        //        // If there is a cell hyperlink, copy
        //        if (oldCell.Hyperlink != null) newCell.Hyperlink = oldCell.Hyperlink;

        //        // Set the cell data type
        //        newCell.SetCellType(oldCell.CellType);

        //        // Set the cell data value
        //        switch (oldCell.CellType)
        //        {
        //            case CellType.Blank:
        //                newCell.SetCellValue(oldCell.StringCellValue);
        //                break;
        //            case CellType.Boolean:
        //                newCell.SetCellValue(oldCell.BooleanCellValue);
        //                break;
        //            case CellType.Error:
        //                newCell.SetCellErrorValue(oldCell.ErrorCellValue);
        //                break;
        //            case CellType.Formula:
        //                //newCell.SetCellFormula(oldCell.CellFormula);
        //                newCell.SetCellFormula(oldCell.NumericCellValue.ToString(CultureInfo.InvariantCulture));
        //                break;
        //            case CellType.Numeric:
        //                newCell.SetCellValue(oldCell.NumericCellValue);
        //                break;
        //            case CellType.String:
        //                newCell.SetCellValue(oldCell.RichStringCellValue);
        //                break;
        //            case CellType.Unknown:
        //                newCell.SetCellValue(oldCell.StringCellValue);
        //                break;
        //        }
        //    }

        //    // If there are are any merged regions in the source row, copy to new row
        //    for (int i = 0; i < worksheet.NumMergedRegions; i++)
        //    {
        //        CellRangeAddress cellRangeAddress = worksheet.GetMergedRegion(i);
        //        if (cellRangeAddress.FirstRow == sourceRow.RowNum)
        //        {
        //            CellRangeAddress newCellRangeAddress = new CellRangeAddress(newRow.RowNum,
        //                (newRow.RowNum +
        //                 (cellRangeAddress.FirstRow -
        //                  cellRangeAddress.LastRow)),
        //                cellRangeAddress.FirstColumn,
        //                cellRangeAddress.LastColumn);

        //            worksheet.AddMergedRegion(newCellRangeAddress);
        //        }
        //    }

        //}

        ///// <summary>
        ///// 复制行格式并插入指定行数
        ///// </summary>
        ///// <param name="sheet">当前sheet</param>
        ///// <param name="startRowIndex">起始行位置</param>
        ///// <param name="sourceRowIndex">模板行位置</param>
        ///// <param name="insertCount">插入行数</param>
        //public void CopyRow1(ISheet sheet, int startRowIndex, int sourceRowIndex, int insertCount)
        //{
        //    IRow sourceRow = sheet.GetRow(sourceRowIndex);
        //    int sourceCellCount = sourceRow.Cells.Count;

        //    //1. 批量移动行,清空插入区域
        //    sheet.ShiftRows(startRowIndex, //开始行
        //        sheet.LastRowNum, //结束行
        //        insertCount, //插入行总数
        //        true, //是否复制行高
        //        false //是否重置行高
        //        );

        //    int startMergeCell = -1; //记录每行的合并单元格起始位置
        //    for (int i = startRowIndex; i < startRowIndex + insertCount; i++)
        //    {
        //        IRow targetRow = null;
        //        ICell sourceCell = null;
        //        ICell targetCell = null;

        //        targetRow = sheet.CreateRow(i);
        //        targetRow.Height = sourceRow.Height; //复制行高

        //        for (int m = sourceRow.FirstCellNum; m < sourceRow.LastCellNum; m++)
        //        {
        //            sourceCell = sourceRow.GetCell(m);
        //            if (sourceCell == null)
        //                continue;
        //            targetCell = targetRow.CreateCell(m);
        //            targetCell.CellStyle = sourceCell.CellStyle; //赋值单元格格式
        //            targetCell.SetCellType(sourceCell.CellType);

        //            //以下为复制模板行的单元格合并格式
        //            if (sourceCell.IsMergedCell)
        //            {
        //                if (startMergeCell <= 0)
        //                    startMergeCell = m;
        //                else if (startMergeCell > 0 && sourceCellCount == m + 1)
        //                {
        //                    sheet.AddMergedRegion(new CellRangeAddress(i, i, startMergeCell, m));
        //                    startMergeCell = -1;
        //                }
        //            }
        //            else
        //            {
        //                if (startMergeCell >= 0)
        //                {
        //                    sheet.AddMergedRegion(new CellRangeAddress(i, i, startMergeCell, m - 1));
        //                    startMergeCell = -1;
        //                }
        //            }
        //        }
        //    }
        //}

        ///// <summary>
        ///// 上传文件
        ///// </summary>
        ///// <returns></returns>
        //private string UploadFile()
        //{
        //    string fileFullName = string.Empty; //上传文件地址

        //    try
        //    {
        //        if (fileUpload.HasFile)
        //        {
        //            string filePath = Server.MapPath("~/Upload/");
        //            //string fileName = Path.GetFileNameWithoutExtension(fileUpload.PostedFile.FileName); //获取文件名（不包括扩展名）
        //            string fileName = DateTime.Now.ToString("yyyyMMddHHmmss");
        //            string extension = Path.GetExtension(fileUpload.PostedFile.FileName); //获取扩展名
        //            fileFullName = filePath + fileName + extension;

        //            fileUpload.SaveAs(fileFullName);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        fileFullName = string.Empty;
        //    }

        //    return fileFullName;
        //}

        //private string MakeErrorExcel(string fileName, Dictionary<int, string> errorMsgs)
        //{
        //    string result = string.Empty;

        //    IWorkbook workbook = null;
        //    IWorkbook errorBook = null;

        //    string fileExt = Path.GetExtension(fileName).ToLower(); //文件后缀

        //    using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
        //    {
        //        //XSSFWorkbook 适用XLSX格式，HSSFWorkbook 适用XLS格式
        //        if (fileExt == ".xlsx")
        //        {
        //            workbook = new XSSFWorkbook(fs);
        //            errorBook = new XSSFWorkbook();
        //        }
        //        else if (fileExt == ".xls")
        //        {
        //            workbook = new HSSFWorkbook(fs);
        //            errorBook = new HSSFWorkbook();
        //        }
        //        else
        //        {
        //            workbook = null;
        //        }
        //        if (workbook == null)
        //        {
        //            errorBook = new HSSFWorkbook(fs);
        //            return null;
        //        }
        //        ISheet sheet = workbook.GetSheetAt(0);
        //        ISheet errorSheet = errorBook.CreateSheet();

        //        if (sheet != null)
        //        {
        //            //复制表头
        //            for (int i = 0; i < 3; i++)
        //            {
        //                CopyRow(errorBook, errorSheet, sheet.GetRow(i), i);
        //            }

        //            //创建一列存储错误信息
        //            int lastCellNum = errorSheet.GetRow(2).LastCellNum;
        //            errorSheet.GetRow(2).CreateCell(lastCellNum).SetCellValue("ErrorMsg");

        //            int index = 3; //错误excel行
        //            foreach (var errorMsg in errorMsgs)
        //            {
        //                CopyRow(errorBook, errorSheet, sheet.GetRow(errorMsg.Key), index);
        //                errorSheet.GetRow(index).CreateCell(lastCellNum).SetCellValue(errorMsg.Value);

        //                index++;
        //            }
        //        }

        //        string errorFile = Server.MapPath("~/Upload/") + "eroor_" + DateTime.Now.ToString("yyyyMMddHHmmss") +
        //                           fileExt;
        //        using (FileStream errorExcel = new FileStream(errorFile, FileMode.Create))
        //        {
        //            errorBook.Write(errorExcel);
        //            errorExcel.Close();

        //            result = errorFile;
        //        }

        //    }

        //    return result;
        //}

        ///// <summary>
        ///// 插入数据表
        ///// </summary>
        ///// <param name="dt"></param>
        //public void InsertData(DataTable dt, out Dictionary<int, string> errorMsgs)
        //{
        //    errorMsgs = new Dictionary<int, string>(); //记录行号，将错误数据生成excel

        //    if (dt == null || dt.Rows.Count <= 0) return;

        //    string dateIndex = wageDate.Value.Replace("-", "");
        //    string payID = string.Empty;
        //    Staff staff;
        //    List<StaffWageItem> myWageItems = new List<StaffWageItem>(); //我的工资项
        //    DataRow dr;

        //    //判断是否已经封账
        //    if (IsClosed(dateIndex))
        //    {
        //        Easyui.Alert("操作提示", $"{dateIndex}已经封账!", Sign.Error);
        //        return;
        //    }

        //    //循环数据集
        //    for (int i = 0; i < dt.Rows.Count; i++)
        //    {
        //        dr = dt.Rows[i];

        //        //获取员工信息
        //        staff = GetStaff(dr);

        //        //判断员工是否存在
        //        if (staff == null)
        //        {
        //            errorMsgs.Add(i + 3, $"未找到该员工{dr["姓名"]}!");
        //            continue;
        //        }

        //        //获取我的工资项
        //        myWageItems = GetMyWageItems(staff.ID);
        //        if (myWageItems.Count <= 0)
        //        {
        //            errorMsgs.Add(i + 3, $"未找到[{staff.Name}]的工资项!");
        //            continue;
        //        }

        //        //添加月账单
        //        payID = InsertOrUpdatePayBill(staff, dateIndex);
        //        if (string.IsNullOrWhiteSpace(payID))
        //        {
        //            errorMsgs.Add(i + 3, "添加月账单失败!");
        //            continue;
        //        }



        //        //添加修改普通工资项
        //        if (!string.IsNullOrWhiteSpace(InsertOrUpdatePayItem(payID, dateIndex, dr, myWageItems)))
        //        {
        //            errorMsgs.Add(i + 3, $"[{staff.Name}]添加普通工资项失败!");
        //            continue;
        //        }

        //        //添加计算工资项
        //        InitArguments(staff.ID, dateIndex);        //初始化参数，为了自定义方法
        //        if (!string.IsNullOrWhiteSpace(InsertOrUpdatePayItem_Calc(payID, dateIndex, dr, myWageItems)))
        //        {
        //            errorMsgs.Add(i + 3, $"{staff.Name}添加计算工资项失败!");
        //            continue;
        //        }
        //    }
        //}

        ///// <summary>
        ///// 获取员工信息（需要修改）
        ///// </summary>
        ///// <param name="dr"></param>
        ///// <returns></returns>
        //private Staff GetStaff(DataRow dr)
        //{
        //    Staff staff = null;

        //    if (dr == null) return staff;

        //    using (var staffs = new StaffsRoll())
        //    {
        //        //身份证
        //        var idCard = dr[SFZH].ToString();
        //        var name = dr[XM].ToString();
        //        var id = dr[GH].ToString();

        //        //根据身份证获取信息
        //        if (!string.IsNullOrWhiteSpace(idCard))
        //        {
        //            staff = staffs.FirstOrDefault(item => item.IDCard == idCard);
        //        }
        //        else if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(id))
        //        {
        //            //如果根据姓名和工号查出多个员工，则直接复制为null
        //            if (
        //                staffs.Count(
        //                    item => item.Name == name && item.SelCode == id && item.Status == StaffStatus.Normal) > 1)
        //            {
        //                staff = null;
        //            }
        //            else
        //            {
        //                staff = staffs.FirstOrDefault(item => item.Name == name && item.SelCode == id);
        //            }

        //        }
        //    }

        //    return staff;
        //}

        ///// <summary>
        ///// 添加月账单
        ///// </summary>
        ///// <param name="staff">员工信息</param>
        ///// <param name="dataIndex">工资日期</param>
        ///// <returns>月账单ID</returns>
        //private string InsertOrUpdatePayBill(Staff staff, string dataIndex)
        //{
        //    string result;

        //    using (var payView = new PayBillsRoll())
        //    {
        //        var entity = payView.FirstOrDefault(item => item.StaffID == staff.ID && item.DateIndex == dataIndex);

        //        if (entity != null && !string.IsNullOrWhiteSpace(entity.ID))
        //        {
        //            return entity.ID;
        //        }

        //        var payBill = new PayBill()
        //        {
        //            Status = PayBillStatus.Check,
        //            DateIndex = dataIndex,
        //            CreaetDate = DateTime.Now,
        //            StaffID = staff.ID,
        //            StaffCode = staff.Code,
        //        };

        //        payBill.Enter();

        //        result = payBill.ID;
        //    }

        //    return result;
        //}

        ///// <summary>
        ///// 添加或修改工资项
        ///// </summary>
        ///// <param name="dr"></param>
        ///// <param name="payID"></param>
        //private string InsertOrUpdatePayItem(string payID, string dateIndex, DataRow dr, List<StaffWageItem> wageItems)
        //{
        //    string result = string.Empty;
        //    PayItem payItem;

        //    try
        //    {
        //        //只添加非计算列
        //        wageItems = wageItems.Where(item => !item.IsCalc).ToList();

        //        //删除当前月历史数据
        //        payItem = new PayItem() { PayID = payID };
        //        payItem.Abandon();

        //        //循环我的工资项 与 excel列进行匹配，如果是空的指定默认值
        //        //我的工资项没有的，不进行插入
        //        foreach (var wageItem in wageItems)
        //        {
        //            payItem = new PayItem()
        //            {
        //                Name = wageItem.WageItemName,
        //                DateIndex = dateIndex,
        //                AdminID = Erp.Current.ID,
        //                PayID = payID,
        //                Value = wageItem.DefaultValue ?? 0 //取员工工资项默认值
        //            };

        //            //判断导入excel，是否包含工资列
        //            if (dr.Table.Columns.Contains(wageItem.WageItemName) &&
        //                dr[wageItem.WageItemName].ToString().IsNumber())
        //            {
        //                payItem.Value = decimal.Parse(dr[wageItem.WageItemName].ToString());
        //            }

        //            payItem.Enter();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result = ex.Message;
        //    }

        //    return result;
        //}

        ///// <summary>
        ///// 修改工资项（通过handsontable）
        ///// </summary>
        ///// <param name="payItem"></param>
        ///// <returns></returns>
        //private string UpdatePayItemByHandstable(PayItem payItem)
        //{
        //    string result = string.Empty;

        //    try
        //    {
        //        using (var view = new PayItemsRoll())
        //        {
        //            var model = view.FirstOrDefault(item => item.PayID == payItem.PayID && item.Name == payItem.Name);
        //            if (model != null)
        //            {
        //                model.Value = payItem.Value;
        //                model.Enter();
        //            }
        //            else
        //            {
        //                result = "未找到该工资项";
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result = ex.Message;
        //    }

        //    return result;
        //}

        ///// <summary>
        ///// 添加或修改计算工资项
        ///// </summary>
        ///// <param name="dr"></param>
        ///// <param name="payID"></param>
        //private string InsertOrUpdatePayItem_Calc(string payID, string dateIndex, DataRow dr,
        //    List<StaffWageItem> wageItems)
        //{
        //    string result = string.Empty;
        //    PayItem payItem;

        //    try
        //    {
        //        //只添加计算列
        //        var wageItemsCalc = wageItems.Where(item => item.IsCalc).OrderBy(item => item.CalcOrder).ToList();

        //        DataTable dt; //用于计算
        //        string formula;

        //        //计算列插入
        //        foreach (var wageItem in wageItemsCalc)
        //        {
        //            payItem = new PayItem()
        //            {
        //                Name = wageItem.WageItemName,
        //                DateIndex = dateIndex,
        //                AdminID = Erp.Current.ID,
        //                PayID = payID,
        //                WageItemFormula = wageItem.Formula,     //工资项的公式
        //                Value = 0 //默认给0
        //            };

        //            //根据payId获取所有工资项
        //            dt = PayItemsToTable(payID);

        //            //将公式里的方法替换成结果数字
        //            formula = ExecuteFunc(wageItem.Formula);

        //            //计算
        //            Calculation(dt, formula, payItem);

        //            //如果是负数的话，赋值为0
        //            payItem.Value = payItem.Value < 0 ? 0 : payItem.Value;

        //            payItem.Enter();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result = ex.Message;
        //    }

        //    return result;
        //}

        ///// <summary>
        ///// 获取我的工资项
        ///// </summary>
        ///// <returns></returns>
        //private List<StaffWageItem> GetMyWageItems(string staffId)
        //{
        //    using (var view = new MyWageItemsRoll(staffId))
        //    {
        //        return view.ToList();
        //    }
        //}

        ///// <summary>
        ///// 根据payId获取员工Id
        ///// </summary>
        ///// <returns></returns>
        //private string GetStaffIdByPayId(string payId)
        //{
        //    string result = string.Empty;

        //    using (var payView = new PayBillsRoll())
        //    {
        //        var payItem = payView.FirstOrDefault(item => item.ID == payId);
        //        if (payItem != null)
        //        {
        //            result = payItem.StaffID;
        //        }
        //    }

        //    return result;
        //}

        ///// <summary>
        ///// 根据月账单Id清除工资
        ///// </summary>
        ///// <param name="payId">月账单Id</param>
        //private void DeletePayItems(string payId)
        //{
        //    if (!string.IsNullOrWhiteSpace(payId))
        //    {
        //        (new PayItem() { PayID = payId }).Abandon();
        //    }
        //}

        ///// <summary>
        ///// 根据payId获取payItems并转为table
        ///// </summary>
        ///// <param name="payId"></param>
        //private DataTable PayItemsToTable(string payId)
        //{
        //    DataTable dt = new DataTable();

        //    using (var view = new PayItemsRoll())
        //    {
        //        var list = view.Where(item => item.PayID == payId).ToList();

        //        if (list.Count > 0)
        //        {
        //            foreach (var payItem in list)
        //            {
        //                dt.Columns.Add(payItem.Name, typeof(decimal));
        //            }

        //            DataRow dr = dt.NewRow();

        //            for (int i = 0; i < list.Count; i++)
        //            {
        //                dr[list[i].Name] = list[i].Value;
        //            }

        //            dt.Rows.Add(dr);
        //        }
        //    }

        //    return dt;
        //}

        ///// <summary>
        ///// 根据公式计算
        ///// </summary>
        ///// <param name="dt">当前员工的工资项</param>
        ///// <param name="formula">公式</param>
        ///// <param name="model">工资项</param>
        ///// <returns></returns>
        //private PayItem Calculation(DataTable dt, string formula, PayItem model)
        //{
        //    var payItem = model;

        //    if (dt == null || dt.Rows.Count <= 0)
        //    {
        //        payItem.Value = 0;
        //        return payItem;
        //    }

        //    try
        //    {
        //        var colName = Guid.NewGuid().ToString("N");

        //        System.Data.DataColumn column = new DataColumn(colName, typeof(decimal));

        //        dt.Columns.Add(column);

        //        column.Expression = formula;

        //        var obj = dt.Select()[0][colName];
        //        if (obj != null && !string.IsNullOrWhiteSpace(obj.ToString()))
        //        {
        //            payItem.Value = decimal.Parse(obj.ToString());
        //            payItem.ActualFormula = formula;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        if (ex.Message.Contains("["))
        //        {
        //            //去除不包含的项
        //            var item = ex.Message.Substring(ex.Message.IndexOf("[") + 1, ex.Message.IndexOf("]") - ex.Message.IndexOf("[") - 1);
        //            formula = formula.Replace(item.Trim(), "");

        //            return Calculation(dt, formula, model);
        //        }
        //        else
        //        {
        //            payItem.Value = 0;
        //            payItem.ActualFormula = formula;
        //            payItem.Description = ex.Message;
        //            return payItem;
        //        }
        //    }
        //    return payItem;
        //}

        ///// <summary>
        ///// 去除特殊符号
        ///// </summary>
        ///// <param name="dt">数据表</param>
        ///// <param name="formula">公式</param>
        ///// <returns></returns>
        //private string RemoveSpecialSymbols(DataTable dt, string formula)
        //{
        //    string result = formula;

        //    if (!formula.Contains("/"))
        //    {
        //        return result;
        //    }

        //    string colName = string.Empty;
        //    foreach (var column in dt.Columns)
        //    {
        //        colName = column.ToString();

        //        if (colName.Contains("/"))
        //        {
        //            result = formula.Replace(colName, "[" + colName + "]");
        //        }
        //    }

        //    return result;
        //}

        ///// <summary>
        ///// 执行方法
        ///// </summary>
        ///// <param name="funcName">方法名</param>
        ///// <returns></returns>
        //private decimal CallFunc(string funcName)
        //{
        //    funcName = funcName.Replace("(", "").Replace(")", "").Replace("{", "").Replace("}", "");      //去除小括号、去除大括号

        //    MethodInfo method = GetType().GetMethod(funcName);
        //    if (method == null)
        //    {
        //        return 0;
        //    }

        //    Func<decimal> func = (Func<decimal>)Delegate.CreateDelegate(typeof(Func<decimal>), this, method);
        //    return func();
        //}

        ///// <summary>
        ///// 执行字符串里边的方法
        ///// </summary>
        ///// <param name="formula">公式</param>
        ///// <returns></returns>
        //private string ExecuteFunc(string formula)
        //{
        //    string result = formula;

        //    if (!formula.Contains("{")) return result;

        //    string pattern = @"\{([^\}]+)\}";       //只获取大括号里的内容
        //    var collection = Regex.Matches(formula, pattern);

        //    decimal dec = 0;        //结果值

        //    foreach (var fun in collection)
        //    {
        //        dec = CallFunc(fun.ToString());     //执行方法

        //        result = result.Replace(fun.ToString(), dec.ToString());
        //    }

        //    return result;
        //}

        ///// <summary>
        ///// 计算个税
        ///// </summary>
        ///// <returns></returns>
        //public decimal f_CalcPersonal()
        //{
        //    decimal result = 0;

        //    string dateIndex = Arg_DateIndex;
        //    string staffId = Arg_StaffID;

        //    using (var rateView = new PersonalRatesRoll())
        //    {
        //        //获取累计收入、累计免税收入、累计专项扣除、累计专项附加扣除、累计已预扣预缴税额
        //        var dicTotal = GetTotalWageItem(dateIndex, staffId);

        //        //累计预扣预缴应纳税所得额=累计收入-累计免税收入-累计专项扣除-累计专项附加扣除
        //        var ykyj = dicTotal[GZHJ] - dicTotal[MSSR] - dicTotal[ZXKC] - dicTotal[ZXFJKC];

        //        //根据累计预扣预缴应纳税所得额获取预扣率
        //        var rate = rateView.FirstOrDefault(item => ykyj > item.BeginAmount && ykyj <= item.EndAmount);

        //        //本期应预扣预缴税额=（累计预扣预缴应纳税所得额×预扣率-速算扣除数)-累计已预扣预缴税额
        //        result = ykyj * rate.Rate - rate.Deduction - dicTotal[BYGS];
        //    }

        //    //return result < 0 ? 0 : result;
        //    return result;
        //}

        ///// <summary>
        ///// 获取工资项合计
        ///// </summary>
        ///// <param name="dateIndex">工资月</param>
        ///// <param name="staffId">员工Id</param>
        ///// <returns></returns>
        //private Dictionary<string, decimal> GetTotalWageItem(string dateIndex, string staffId)
        //{
        //    Dictionary<string, decimal> dic = new Dictionary<string, decimal>()
        //    {
        //        { GZHJ, 0 },        //工资合计
        //        { ZXKC, 0 },        //专项扣除
        //        { MSSR, 0 },        //免税收入
        //        { ZXFJKC, 0 },      //专项附加扣除
        //        { BYGS, 0 },        //已预缴预扣税额
        //    };


        //    var dateIndexs = GetTotalDateIndex(dateIndex);

        //    if (dateIndexs.Count <= 0)
        //        return dic;


        //    using (var payBill = new PayBillsRoll())
        //    using (var payItems = new PayItemsRoll())
        //    {
        //        foreach (var date in dateIndexs)
        //        {
        //            //根据员工ID和工资月获取当前员工月账单
        //            var bill = payBill.FirstOrDefault(t => t.StaffID == staffId && t.DateIndex == date);        // && t.Status == PayBillStatus.Closed  //需要考虑工资状态

        //            if (bill == null || string.IsNullOrWhiteSpace(bill.ID)) continue;

        //            //根据月账单ID获取薪酬信息
        //            var items = payItems.Where(item => item.PayID == bill.ID);

        //            //工资合计
        //            dic[GZHJ] = dic[GZHJ] + (items.FirstOrDefault(t => t.Name == GZHJ) == null ? 0 : items.FirstOrDefault(t => t.Name == GZHJ).Value);
        //            //专项扣除
        //            dic[ZXKC] = dic[ZXKC] + (items.FirstOrDefault(t => t.Name == ZXKC) == null ? 0 : items.FirstOrDefault(t => t.Name == ZXKC).Value);
        //            //免税收入
        //            dic[MSSR] = dic[MSSR] + MSJE;
        //            //专项附加扣除
        //            dic[ZXFJKC] = dic[ZXFJKC] + (items.FirstOrDefault(t => t.Name == ZXFJKC) == null ? 0 : items.FirstOrDefault(t => t.Name == ZXFJKC).Value);
        //            //本月个税（已预缴预扣税额）
        //            if (date != dateIndex)
        //                dic[BYGS] = dic[BYGS] + (items.FirstOrDefault(t => t.Name == BYGS) == null ? 0 : items.FirstOrDefault(t => t.Name == BYGS).Value);
        //        }
        //    }

        //    return dic;
        //}


        ///// <summary>
        ///// 获取需要合计的工资月
        ///// </summary>
        ///// <param name="dateIndex">工资月</param>
        ///// <returns></returns>
        //private List<string> GetTotalDateIndex(string dateIndex)
        //{
        //    List<string> result = new List<string>();

        //    if (string.IsNullOrWhiteSpace(dateIndex)) return result;

        //    string year = dateIndex.Substring(0, 4);        //获取当前年

        //    DateTime dt1 = DateTime.ParseExact(year + "0101", "yyyyMMdd", Thread.CurrentThread.CurrentCulture);           //今年一月一号
        //    DateTime dt2 = DateTime.ParseExact(dateIndex + "01", "yyyyMMdd", Thread.CurrentThread.CurrentCulture);        //当月一号
        //    if (dt2.Month - dt1.Month < 0) return result;


        //    int count = dt2.Month - dt1.Month;      //计算循环次数

        //    for (int i = 0; i <= count; i++)
        //    {
        //        dt1 = dt1.AddMonths(i == 0 ? 0 : 1);
        //        result.Add(dt1.ToString("yyyyMM"));
        //    }

        //    return result;
        //}

        ///// <summary>
        ///// 初始化参数
        ///// </summary>
        ///// <param name="staff"></param>
        ///// <param name="dateIndex"></param>
        //private void InitArguments(string staffId, string dateIndex)
        //{
        //    Arg_StaffID = staffId;
        //    Arg_DateIndex = dateIndex;
        //}

        ///// <summary>
        ///// 判断工资月是否已经封账
        ///// </summary>
        ///// <param name="dateIndex"></param>
        ///// <returns></returns>
        //private bool IsClosed(string dateIndex)
        //{
        //    bool result = false;

        //    using (var view = new PayBillsRoll())
        //    {
        //        if (view.Any(item => item.DateIndex == dateIndex && item.Status == PayBillStatus.Closed))
        //        {
        //            result = true;
        //        }
        //    }

        //    return result;
        //}

        ///// <summary>
        ///// 是否封账
        ///// </summary>
        ///// <returns></returns>
        //protected object isClosed()
        //{
        //    try
        //    {
        //        //判断是否已经封账
        //        string dateIndex = Request["dateIndex"].Replace("-", "");
        //        if (!IsClosed(dateIndex))
        //        {
        //            return new
        //            {
        //                code = 400,
        //            };
        //        }

        //        return new
        //        {
        //            code = 200
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new
        //        {
        //            code = 400,
        //            message = "异常错误：" + ex.Message
        //        };
        //    }
        //}

        ///// <summary>
        ///// 动态Linq方式实现行转列
        ///// </summary>
        ///// <param name="list">数据</param>
        ///// <param name="DimensionList">固定列</param>
        ///// <param name="DynamicColumn">动态列</param>
        ///// <param name="NumColumn">计算列</param>
        ///// <returns>行转列后数据</returns>
        //private List<dynamic> DynamicLinq<T>(List<T> list, List<string> DimensionList, string DynamicColumn, string NumColumn, out List<string> AllDynamicColumn) where T : class
        //{
        //    //获取所有动态列
        //    var columnGroup = list.GroupBy(DynamicColumn, "new(it as Vm)") as IEnumerable<IGrouping<dynamic, dynamic>>;
        //    List<string> AllColumnList = new List<string>();
        //    foreach (var item in columnGroup)
        //    {
        //        if (!string.IsNullOrEmpty(item.Key))
        //        {
        //            AllColumnList.Add(item.Key);
        //        }
        //    }
        //    AllDynamicColumn = AllColumnList;
        //    var dictFunc = new Dictionary<string, Func<T, bool>>();
        //    foreach (var column in AllColumnList)
        //    {
        //        var func = DynamicExpression.ParseLambda<T, bool>(string.Format("{0}==\"{1}\"", DynamicColumn, column)).Compile();
        //        dictFunc[column] = func;
        //    }
        //    //获取实体所有属性
        //    Dictionary<string, PropertyInfo> PropertyInfoDict = new Dictionary<string, PropertyInfo>();
        //    Type type = typeof(T);
        //    var propertyInfos = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        //    //数值列
        //    List<string> AllNumberField = new List<string>() { NumColumn };
        //    foreach (var item in propertyInfos)
        //    {
        //        PropertyInfoDict[item.Name] = item;
        //        //if (item.PropertyType == typeof(int) || item.PropertyType == typeof(double) || item.PropertyType == typeof(float) || item.PropertyType == typeof(decimal))
        //        //{
        //        //    AllNumberField.Add(item.Name);
        //        //}
        //    }
        //    //分组
        //    var dataGroup = list.GroupBy(string.Format("new ({0})", string.Join(",", DimensionList)), "new(it as Vm)") as IEnumerable<IGrouping<dynamic, dynamic>>;
        //    List<dynamic> listResult = new List<dynamic>();
        //    IDictionary<string, object> itemObj = null;
        //    T vm2 = default(T);
        //    foreach (var group in dataGroup)
        //    {
        //        itemObj = new ExpandoObject();
        //        var listVm = group.Select(e => e.Vm as T).ToList();
        //        //维度列赋值
        //        vm2 = listVm.FirstOrDefault();
        //        foreach (var key in DimensionList)
        //        {
        //            itemObj[key] = PropertyInfoDict[key].GetValue(vm2);
        //        }
        //        foreach (var column in AllColumnList)
        //        {
        //            vm2 = listVm.FirstOrDefault(dictFunc[column]);
        //            if (vm2 != null)
        //            {
        //                foreach (string name in AllNumberField)
        //                {
        //                    //itemObj[name + column] = PropertyInfoDict[name].GetValue(vm2);
        //                    itemObj[column] = PropertyInfoDict[name].GetValue(vm2);
        //                }
        //            }
        //        }
        //        listResult.Add(itemObj);
        //    }
        //    return listResult;
        //}

        ///// <summary>
        ///// 获取handsontable数据源
        ///// </summary>
        ///// <returns></returns>
        //public List<dynamic> GetData(Expression<Func<StaffPayItem, bool>> predicate)
        //{
        //    using (var view = new StaffPayItems())
        //    {
        //        var data = view.Where(predicate);

        //        List<string> dynColumns;
        //        if (!data.Any())
        //        {
        //            return null;
        //        }

        //        return DynamicLinq(data.ToList(), GetFixedColumns(), "Name", "Value", out dynColumns);
        //    }

        //}

        //private List<dynamic> GetDataByPayId(string payId)
        //{
        //    using (var view = new StaffPayItems())
        //    {
        //        var data = view.Where(item => item.PayID == payId);

        //        List<string> dynColumns;
        //        if (!data.Any())
        //        {
        //            return null;
        //        }

        //        return DynamicLinq(data.ToList(), GetFixedColumns(), "Name", "Value", out dynColumns);
        //    }

        //}

        ///// <summary>
        ///// json转DataTable
        ///// </summary>
        ///// <param name="strJson"></param>
        ///// <returns></returns>
        //public DataTable JsonToDataTable(string strJson)
        //{
        //    //转换json格式
        //    strJson = strJson.Replace(",\"", "*\"").Replace("\":", "\"#").ToString();
        //    //取出表名   
        //    var rg = new Regex(@"(?<={)[^:]+(?=:\[)", RegexOptions.IgnoreCase);
        //    string strName = rg.Match(strJson).Value;
        //    DataTable tb = null;
        //    //去除表名   
        //    strJson = strJson.Substring(strJson.IndexOf("[") + 1);
        //    strJson = strJson.Substring(0, strJson.IndexOf("]"));

        //    //获取数据   
        //    rg = new Regex(@"(?<={)[^}]+(?=})");
        //    MatchCollection mc = rg.Matches(strJson);
        //    for (int i = 0; i < mc.Count; i++)
        //    {
        //        string strRow = mc[i].Value;
        //        string[] strRows = strRow.Split('*');

        //        //创建表   
        //        if (tb == null)
        //        {
        //            tb = new DataTable();
        //            tb.TableName = strName;
        //            foreach (string str in strRows)
        //            {
        //                var dc = new DataColumn();
        //                string[] strCell = str.Split('#');

        //                if (strCell[0].Substring(0, 1) == "\"")
        //                {
        //                    int a = strCell[0].Length;
        //                    dc.ColumnName = strCell[0].Substring(1, a - 2);
        //                }
        //                else
        //                {
        //                    dc.ColumnName = strCell[0];
        //                }
        //                tb.Columns.Add(dc);
        //            }
        //            tb.AcceptChanges();
        //        }

        //        //增加内容   
        //        DataRow dr = tb.NewRow();
        //        for (int r = 0; r < strRows.Length; r++)
        //        {
        //            try
        //            {
        //                string columnName = strRows[r].Split('#')[0].Trim().Replace("\"", "");
        //                string a = strRows[r].Split('#')[1].Trim();
        //                if (a.Equals("null"))
        //                {
        //                    dr[columnName] = "";
        //                }
        //                else
        //                {
        //                    dr[columnName] = strRows[r].Split('#')[1].Trim().Replace("，", ",").Replace("：", ":").Replace("\"", "");
        //                }
        //            }
        //            catch (Exception e)
        //            {

        //                throw e;
        //            }
        //        }
        //        tb.Rows.Add(dr);
        //        tb.AcceptChanges();
        //    }

        //    try
        //    {
        //        if (tb != null)
        //        {
        //            return tb;
        //        }
        //        else
        //        {
        //            throw new Exception("解析错误");
        //        }
        //    }
        //    catch (Exception e)
        //    {

        //        throw e;
        //    }
        //}



        ///// <summary>
        ///// 获取动态列
        ///// </summary>
        ///// <returns></returns>
        //private string GetDynamicColumns()
        //{

        //    string dynColumns;

        //    using (var items = new StaffPayItems())
        //    {
        //        var myWageItems = new MyWageItemsRoll();

        //        var staffPayItems = items.Where(GetExpression());

        //        if (!staffPayItems.Any())
        //        {
        //            return null;
        //        }

        //        //获取我的工资项
        //        var result = from wageItem in myWageItems.ToList()
        //                     join staffPi in staffPayItems.ToList() on wageItem.ID equals staffPi.StaffID
        //                     where wageItem.ID != ""
        //                     group wageItem by wageItem.WageItemName;


        //        //获取工资项
        //        var wageitem = Alls.Current.WageItems.ToList();

        //        //排序
        //        var resultOrder = from r in result
        //                          join w in wageitem on r.Key equals w.Name
        //                          orderby w.OrderIndex
        //                          select new { name = r.Key, isCalc = w.IsCalc };

        //        dynColumns = resultOrder.Json();
        //    }

        //    return dynColumns;
        //}

        ///// <summary>
        ///// 查询条件
        ///// </summary>
        ///// <returns></returns>
        //public Expression<Func<StaffPayItem, bool>> GetExpression()
        //{
        //    Expression<Func<StaffPayItem, bool>> predicate = null;

        //    //工资日期
        //    if (!string.IsNullOrWhiteSpace(wageDate.Value))
        //    {
        //        string date = wageDate.Value.Replace("-", "");

        //        predicate = predicate.And(item => item.DateIndex == date);
        //    }

        //    //姓名、工号
        //    if (!string.IsNullOrWhiteSpace(txt_name.Value))
        //    {
        //        predicate =
        //            predicate.And(item => item.StaffName.Contains(txt_name.Value) || item.StaffSelCode == txt_name.Value);
        //    }

        //    //内部公司
        //    if (!string.IsNullOrWhiteSpace(CompanyId.Value))
        //    {
        //        predicate = predicate.And(item => item.CompanyName == CompanyId.Value);
        //    }

        //    //岗位
        //    if (!string.IsNullOrWhiteSpace(PostionId.Value))
        //    {
        //        predicate = predicate.And(item => item.PostionName == PostionId.Value);
        //    }

        //    return predicate;
        //}

        ///// <summary>
        ///// 获取固定列
        ///// </summary>
        ///// <returns></returns>
        //private List<string> GetFixedColumns()
        //{
        //    return new List<string>()
        //            {
        //                "PayID",
        //                "StaffID",
        //                "StaffName",
        //                "StaffCode",
        //                "DateIndex",
        //                "Status",
        //                "City",
        //                "CompanyName",
        //                "DyjCompanyCode",
        //                "DyjDepartmentCode",
        //                "PostionName",
        //                "IDCard",
        //                "StaffSelCode",
        //                "BankAccount",
        //            };
        //}

        ///// <summary>
        ///// 列名
        ///// </summary>
        ///// <returns></returns>
        //protected string GetColNames()
        //{
        //    var list = new List<dynamic>()
        //    {
        //        new {data="PayID",title="PayID",type="text"},
        //        new {data="City",title="地区",type="text"},
        //        new {data="CompanyName",title="所属公司",type="text"},
        //        new {data="DyjCompanyCode",title="分公司",type="text"},
        //        new {data="DyjDepartmentCode",title="部门",type="text"},
        //        new {data="StaffSelCode",title="ID（工号）",type="text"},
        //        new {data="StaffName",title="姓名",type="text"},
        //        new {data="PostionName",title="考核岗位",type="text"},
        //        new {data="IDCard",title="身份证号码",type="text"},
        //    };

        //    using (var view = new StaffPayItems())
        //    {
        //        var dynColumns = GetDynamicColumns();

        //        if (dynColumns != null)
        //        {
        //            var result = dynColumns.JsonTo<dynamic>();

        //            foreach (var d in result)
        //            {
        //                if (!Convert.ToBoolean(d.isCalc))
        //                {
        //                    list.Add(new { data = d.name, title = d.name, type = "text" });
        //                }
        //                else
        //                {
        //                    list.Add(new { data = d.name, title = d.name, type = "text", readOnly = true });
        //                }
        //            }
        //        }
        //    }

        //    return list.Json();
        //}

        ///// <summary>
        ///// 去重复
        ///// </summary>
        ///// <returns></returns>
        //private List<PayItem> RemoveDuplicate(PayItem[] oldItem)
        //{
        //    List<PayItem> list = new List<PayItem>();

        //    if (oldItem.Length <= 0) return null;

        //    //倒序去重复
        //    for (int i = oldItem.Length - 1; i >= 0; i--)
        //    {
        //        if (!list.Any(t => t.PayID == oldItem[i].PayID && t.Name == oldItem[i].Name))
        //        {
        //            list.Add(oldItem[i]);
        //        }
        //    }

        //    return list;
        //}

        ///// <summary>
        ///// 获取内部公司
        ///// </summary>
        ///// <returns></returns>
        //protected object getCompanies()
        //{
        //    var result = Alls.Current.Companies.Select(item => new { Value = item.Name, Text = item.Name }).ToList();
        //    result.Insert(0, new { Value = "", Text = "全部" });

        //    return result;
        //}

        ///// <summary>
        ///// 获取岗位
        ///// </summary>
        ///// <returns></returns>
        //protected object getPostions()
        //{
        //    var result = Alls.Current.Postions.Select(item => new { Value = item.Name, Text = item.Name }).ToList();
        //    result.Insert(0, new { Value = "", Text = "全部" });

        //    return result;
        //}

        ///// <summary>
        ///// 获取模板文件
        ///// </summary>
        //private string GetTemplate()
        //{
        //    return Server.MapPath("~/Upload/Template/template.xls");
        //}

        ///// <summary>
        ///// 获取固定样式
        ///// </summary>
        ///// <param name="workbook"></param>
        ///// <returns></returns>
        //private ICellStyle GetCellStyle(IWorkbook workbook)
        //{
        //    var cellstyle = workbook.CreateCellStyle();
        //    cellstyle.Alignment = HorizontalAlignment.Center;
        //    cellstyle.VerticalAlignment = VerticalAlignment.Center;

        //    //字体
        //    IFont font = workbook.CreateFont();
        //    font.FontName = "宋体";
        //    font.FontHeightInPoints = 9;

        //    cellstyle.SetFont(font);

        //    //有边框
        //    cellstyle.BorderBottom = BorderStyle.Thin;
        //    cellstyle.BorderLeft = BorderStyle.Thin;
        //    cellstyle.BorderRight = BorderStyle.Thin;
        //    cellstyle.BorderTop = BorderStyle.Thin;

        //    return cellstyle;
        //}

        ///// <summary>
        ///// 下载文件
        ///// </summary>
        ///// <param name="fileName"></param>
        //private void DownLoadFile(string fileName)
        //{
        //    if (string.IsNullOrWhiteSpace(fileName))
        //    {
        //        return;
        //    }

        //    FileInfo fileInfo = new FileInfo(fileName);
        //    Response.Clear();
        //    Response.ClearContent();
        //    Response.ClearHeaders();
        //    Response.AddHeader("Content-Disposition", "attachment;filename=" + DateTime.Now.ToString("yyyyMMddHHmmss") + "." + Path.GetExtension(fileName).ToLower());
        //    Response.AddHeader("Content-Length", fileInfo.Length.ToString());
        //    Response.AddHeader("Content-Transfer-Encoding", "binary");
        //    Response.ContentType = "application/octet-stream";
        //    Response.ContentEncoding = System.Text.Encoding.GetEncoding("gb2312");
        //    Response.WriteFile(fileInfo.FullName);
        //    Response.Flush();
        //    Response.End();
        //}


        ///// <summary>
        ///// 生成会计Excel
        ///// </summary>
        ///// <param name="tempFile">模板文件</param>
        ///// <param name="exportName">导出文件名称</param>
        ///// <param name="dataRows">源数据</param>
        ///// <param name="dataColumns">列集合</param>
        ///// <returns>excel文件路径</returns>
        //private string MakeFinanceExcel(string tempFile, string exportName, DataRow[] dataRows, DataColumnCollection dataColumns)
        //{
        //    string fileName = string.Empty;

        //    FileStream fs = null;
        //    FileStream fsExport = null;

        //    try
        //    {
        //        string fileExt = Path.GetExtension(tempFile).ToLower(); //文件后缀
        //        using (fs = new FileStream(tempFile, FileMode.Open, FileAccess.Read))
        //        {
        //            IWorkbook workbook;
        //            //XSSFWorkbook 适用XLSX格式，HSSFWorkbook 适用XLS格式
        //            if (fileExt == ".xlsx")
        //            {
        //                workbook = new XSSFWorkbook(fs);
        //            }
        //            else if (fileExt == ".xls")
        //            {
        //                workbook = new HSSFWorkbook(fs);
        //            }
        //            else
        //            {
        //                workbook = null;
        //            }

        //            ISheet sheet = workbook.GetSheetAt(0);

        //            //设置标题 2019年4月员工工资表明细
        //            IRow header1 = sheet.GetRow(0); //获取标头
        //            string date = wageDate.Value.Replace("-", "年") + "月";
        //            header1.Cells[0].SetCellValue(date + GetValue(header1.GetCell(0)));

        //            //遍历循环数据源
        //            int headRowCount = 3; //标题行数
        //            IRow headRow2 = sheet.GetRow(headRowCount - 2); //标题行
        //            IRow headRow3 = sheet.GetRow(headRowCount - 1); //标题行
        //            IRow curRow; //当前行
        //            ICell curCell; //当前单元
        //            DataRow dr;
        //            string head = string.Empty; //标题

        //            //样式
        //            var cellstyle = GetCellStyle(workbook);

        //            for (int i = 0; i < dataRows.Length; i++)
        //            {
        //                dr = dataRows[i];
        //                curRow = sheet.CreateRow(i + headRowCount);

        //                for (int m = 0; m < headRow3.LastCellNum; m++)
        //                {
        //                    head = (GetValue(headRow3.GetCell(m)) ?? GetValue(headRow2.GetCell(m)))?.ToString();
        //                    curCell = curRow.CreateCell(m);
        //                    curCell.CellStyle = cellstyle;

        //                    if (head == "序号")
        //                    {
        //                        curCell.SetCellValue((i + 1));
        //                        continue;
        //                    }

        //                    if (head == "地区")
        //                    {
        //                        curCell.SetCellValue(dr["City"].ToString());
        //                        continue;
        //                    }

        //                    if (head == "所属公司")
        //                    {
        //                        curCell.SetCellValue(dr["CompanyName"].ToString());
        //                        continue;
        //                    }

        //                    if (head == "分公司")
        //                    {
        //                        curCell.SetCellValue(dr["DyjCompanyCode"].ToString());
        //                        continue;
        //                    }

        //                    if (head == "部门")
        //                    {
        //                        curCell.SetCellValue(dr["DyjDepartmentCode"].ToString());
        //                        continue;
        //                    }

        //                    if (head == "ID（工号）")
        //                    {
        //                        curCell.SetCellValue(dr["StaffSelCode"].ToString());
        //                        continue;
        //                    }

        //                    if (head == "姓名")
        //                    {
        //                        curCell.SetCellValue(dr["StaffName"].ToString());
        //                        continue;
        //                    }

        //                    if (head == "考核岗位")
        //                    {
        //                        curCell.SetCellValue(dr["PostionName"].ToString());
        //                        continue;
        //                    }

        //                    if (head == "身份证号码")
        //                    {
        //                        curCell.SetCellValue(dr["IDCard"].ToString());
        //                        continue;
        //                    }


        //                    if (head == "银行卡号")
        //                    {
        //                        curCell.SetCellValue(dr["BankAccount"].ToString());
        //                        continue;
        //                    }


        //                    foreach (var column in dataColumns)
        //                    {
        //                        if (head == column.ToString().Trim())
        //                        {
        //                            if (dr[column.ToString()].ToString().IsNumber())
        //                            {
        //                                curCell
        //                                    .SetCellValue(Convert.ToDouble(dr[column.ToString()].ToString()));
        //                            }
        //                            else
        //                            {
        //                                curCell
        //                                    .SetCellValue(dr[column.ToString()].ToString());
        //                            }

        //                            break;
        //                        }
        //                    }
        //                }

        //            }


        //            fileName = Server.MapPath("~/Upload/") + exportName;
        //            using (fsExport = new FileStream(fileName, FileMode.Create))
        //            {
        //                workbook.Write(fsExport);
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }
        //    finally
        //    {
        //        fs?.Close();
        //        fsExport?.Close();
        //    }

        //    return fileName;
        //}

        //#region 压缩文件
        ///// <summary>
        /////  压缩多个文件
        ///// </summary>
        ///// <param name="files">文件名</param>
        ///// <param name="ZipedFileName">压缩包文件名</param>
        ///// <param name="Password">解压码</param>
        ///// <returns></returns>
        //public void Zip(string[] files, string ZipedFileName, string Password)
        //{
        //    files = files.Where(f => File.Exists(f)).ToArray();
        //    if (files.Length == 0) throw new FileNotFoundException("未找到指定打包的文件");
        //    ZipOutputStream s = new ZipOutputStream(File.Create(ZipedFileName));
        //    s.SetLevel(6);
        //    if (!string.IsNullOrEmpty(Password.Trim())) s.Password = Password.Trim();
        //    ZipFileDictory(files, s);
        //    s.Finish();
        //    s.Close();
        //}

        ///// <summary>
        /////  压缩多个文件
        ///// </summary>
        ///// <param name="files">文件名</param>
        ///// <param name="ZipedFileName">压缩包文件名</param>
        ///// <returns></returns>
        //public void Zip(string[] files, string ZipedFileName)
        //{
        //    Zip(files, ZipedFileName, string.Empty);
        //}

        //private static void ZipFileDictory(string[] files, ZipOutputStream s)
        //{
        //    ZipEntry entry = null;
        //    FileStream fs = null;
        //    Crc32 crc = new Crc32();
        //    try
        //    {
        //        //创建当前文件夹
        //        entry = new ZipEntry("/");  //加上 “/” 才会当成是文件夹创建
        //        s.PutNextEntry(entry);
        //        s.Flush();
        //        foreach (string file in files)
        //        {
        //            //打开压缩文件
        //            fs = File.OpenRead(file);

        //            byte[] buffer = new byte[fs.Length];
        //            fs.Read(buffer, 0, buffer.Length);
        //            entry = new ZipEntry("/" + Path.GetFileName(file));
        //            entry.DateTime = DateTime.Now;
        //            entry.Size = fs.Length;
        //            fs.Close();
        //            crc.Reset();
        //            crc.Update(buffer);
        //            entry.Crc = crc.Value;
        //            s.PutNextEntry(entry);
        //            s.Write(buffer, 0, buffer.Length);
        //        }
        //    }
        //    finally
        //    {
        //        if (fs != null)
        //        {
        //            fs.Close();
        //            fs = null;
        //        }
        //        if (entry != null)
        //            entry = null;
        //        GC.Collect();
        //    }
        //}
        //#endregion
        //#endregion
    }
}