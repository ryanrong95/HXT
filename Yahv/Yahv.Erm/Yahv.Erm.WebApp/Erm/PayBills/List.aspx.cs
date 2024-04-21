using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Layers.Data.Sqls.PvbErm;
using Yahv.Erm.Services;
using Yahv.Erm.Services.Extends;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Erm.Services.Models.Rolls;
using Yahv.Erm.Services.Views.Rolls;
using Yahv.Linq.Extends;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Utils;
using Yahv.Utils.Http;
using Yahv.Utils.Serializers;
using Yahv.Utils.Validates;
using Yahv.Web.Controls.Easyui;
using Yahv.Web.Erp;
using Yahv.Web.Forms;
using Staff = Yahv.Erm.Services.Models.Origins.Staff;

namespace Yahv.Erm.WebApp.Erm.PayBills
{
    public partial class List : ErpParticlePage
    {
        #region 常量

        private const string IdCard_Column = "身份证号码";
        #endregion

        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //UploadWages.Current.Execute();      //工资计算
                wageDate.Value = DateTime.Now.ToString("yyyy-MM");
            }
        }
        #endregion

        #region 功能按钮
        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnImport_Click(object sender, EventArgs e)
        {
            //判断是否已经封账
            string dateIndex = GetDateIndex();
            if (IsClosed(dateIndex))
            {
                Easyui.Alert("操作提示", $"{dateIndex}已经封账!", Sign.Error);
                return;
            }

            string fileFullName = UploadFile();     //上传文件
            string fileName = fileFullName.Substring(fileFullName.LastIndexOf('\\') + 1,
                fileFullName.Length - fileFullName.LastIndexOf('\\') - 1);          //文件名称

            if (string.IsNullOrWhiteSpace(fileFullName))
            {
                Easyui.Alert("操作提示", "上传文件失败!", Sign.Error);
                return;
            }

            var dt = ExportWages.Current.ExcelToTable(fileFullName);

            if (dt == null || dt.Rows.Count <= 0)
            {
                Easyui.Alert("操作提示", "导入数据不能为空!", Sign.Error);
                FileHelper.Rename(fileFullName, fileName.Replace(".check.", ".delete."));
                return;
            }

            List<PayBillItem> list = new List<PayBillItem>();

            //数据检验
            Dictionary<int, string> errorMsgs = CheckData(dt, out list);

            //导出错误excel
            if (errorMsgs.Count > 0)
            {
                FileHelper.Rename(fileFullName, fileName.Replace(".check.", ".delete."));
                DownLoadFile(ExportWages.Current.MakeErrorExcel(fileFullName.Replace(".check.", ".delete."), errorMsgs));
            }
            else
            {
                //更新文件名称，后台开始计算
                //FileHelper.Rename(fileFullName, fileName.Replace(".check.", ".wages."));

                //计算工资项
                Formula(list);
                list.Enter();

                Easyui.Alert("操作提示", $"导入成功!");
            }

            Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "工资录入",
                    $"工资录入：{dateIndex}", string.Empty);

            btnSearch_Click(null, null);
        }

        /// <summary>
        /// 搜索
        /// </summary>
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(wageDate.Value))
            {
                Easyui.Alert("操作提示", "请您选择工资日期", Sign.Error);
                return;
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        protected object save()
        {
            try
            {
                var data = Request["source"].Replace("&quot;", "\"").JsonTo<PayItem[]>();

                if (data.Length <= 0)
                {
                    return new
                    {
                        code = 400,
                        message = "未修改数据!"
                    };
                }

                var list = RemoveDuplicate(data);

                //判断是否已经封账
                string dateIndex = Request["dateIndex"].Replace("-", "");
                if (IsClosed(dateIndex))
                {
                    return new
                    {
                        code = 400,
                        message = $"{dateIndex}已经封账!"
                    };
                }

                var errorMsg = CheckDataByPayItem(data);
                if (!string.IsNullOrWhiteSpace(errorMsg))
                {
                    return new
                    {
                        code = 400,
                        message = errorMsg
                    };
                }

                //普通工资项计算
                foreach (var payItem in data)
                {
                    UpdatePayItemByHandstable(payItem, dateIndex);      //工资项修改
                }

                //公式计算
                foreach (var payId in data.ToList().GroupBy(t => t.PayID).Select(t => t.Key))
                {
                    //Alls.Current.PayItems.CalcWageByPayId(payId, Erp.Current.ID);
                    Calc(dateIndex, payId);
                }

                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "工资录入",
                    $"保存", string.Empty);
            }
            catch (Exception ex)
            {
                return new
                {
                    code = 400,
                    message = "异常错误：" + ex.Message
                };
            }

            return new
            {
                code = 200,
            };
        }

        /// <summary>
        /// 封账
        /// </summary>
        /// <returns></returns>
        protected object checkClosed()
        {
            try
            {
                //#if DEBUG
                return new
                {
                    code = 200
                };
                //#endif

                //判断是否已经封账
                string dateIndex = Request["dateIndex"].Replace("-", "");
                if (IsClosed(dateIndex))
                {
                    return new
                    {
                        code = 400,
                        message = $"{dateIndex}已经封账!"
                    };
                }

                int staffCount = 0;
                int payCount = 0;


                using (var staffView = new StaffsRoll())
                using (var payView = new PayBillsRoll())
                {
                    //比较导入的数据和在职员工个数是否一致 状态：试用期、在职、离职
                    staffCount = staffView.Where(item => item.Status == StaffStatus.Period
                    || item.Status == StaffStatus.Normal || item.Status == StaffStatus.Departure).ToList().Count;
                    payCount = payView.Where(item => item.DateIndex == dateIndex).ToList().Count;


                    if (staffCount != payCount && staffCount != 0)
                    {
                        return new
                        {
                            code = 400,
                            message = $"导入的数据和员工在职人员个数不一致，不能封账!"
                        };
                    }

                    //导入的人员和在职员工是否匹配
                    var ids = payView.Where(t => t.DateIndex == dateIndex).Select(t => t.StaffID).ToList();
                    if (!staffView.Any(item => item.Status == StaffStatus.Normal && ids.Contains(item.ID)))
                    {
                        return new
                        {
                            code = 400,
                            message = $"部分人员未导入工资!"
                        };
                    }
                }


                return new
                {
                    code = 200
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    code = 400,
                    message = "异常错误：" + ex.Message
                };
            }
        }

        protected object closed()
        {
            try
            {
                Alls.Current.PayBills.UpdateStatus(Request["dateIndex"].Replace("-", ""), PayBillStatus.Closed);
            }
            catch (Exception ex)
            {
                return new
                {
                    code = 400,
                    message = "异常错误：" + ex.Message
                };
            }

            return new
            {
                code = 200,
            };
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        protected object delete()
        {
            try
            {
                //判断是否已经封账
                string dateIndex = Request["dateIndex"].Replace("-", "");
                if (IsClosed(dateIndex))
                {
                    return new
                    {
                        code = 400,
                        message = $"{dateIndex}已经封账!"
                    };
                }

                Alls.Current.PayBills.DeleteAll(dateIndex);

                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "工资录入",
                    $"清空数据{dateIndex}", string.Empty);

                return new
                {
                    code = 200
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    code = 400,
                    message = "异常错误：" + ex.Message
                };
            }
        }

        /// <summary>
        /// 根据PayIds删除数据
        /// </summary>
        /// <returns></returns>
        protected object deleteByPayIds()
        {
            try
            {
                //判断是否已经封账
                string dateIndex = Request["dateIndex"].Replace("-", "");
                if (IsClosed(dateIndex))
                {
                    return new
                    {
                        code = 400,
                        message = $"{dateIndex}已经封账!"
                    };
                }

                Alls.Current.PayBills.DeleteByPayIds(Request["payIds"]);

                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "工资录入",
                    $"删除", Request["payIds"]);

                return new
                {
                    code = 200
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    code = 400,
                    message = "异常错误：" + ex.Message
                };
            }
        }

        /// <summary>
        /// 进度
        /// </summary>
        /// <returns></returns>
        protected object progress()
        {
            var status = UploadWages.Current.Status;

            if (status != null)
            {
                return new
                {
                    totalData = status.TotalData,
                    readData = status.ReadData,
                    //message = $"文件个数{status.FileCount},已完成{status.Completed},当前文件有{status.TotalData}行数据，已经处理{status.ReadData}行..."
                    message = $"一共有{status.TotalData}行数据，已经处理{status.ReadData}行..."
                };
            }
            else
            {
                return null;
            }
        }

        protected object getData()
        {
            var data = GetData(GetExpression());
            return data;
        }

        /// <summary>
        /// Excel导出
        /// </summary>
        protected void btnExport_Click(object sender, EventArgs e)
        {
            var data = GetData(GetExpression());
            if (data == null || !data.Any())
            {
                Easyui.Alert("操作提示", "未找到数据!", Sign.Error);
                return;
            }
            var dataTable = ExportWages.Current.JsonToDataTable(data.Json());
            if (dataTable == null || dataTable.Rows.Count <= 0)
            {
                Easyui.Alert("操作提示", "未找到数据!", Sign.Error);
                return;
            }

            //根据模板文件，过滤一下标头
            var wageItems = Alls.Current.WageItems.OrderBy(item => item.OrderIndex).Select(item => item.Name).ToList();
            var tempHeads = ExportWages.Current.GetExcelHead(GetTemplate(), "1,2");
            wageItems = wageItems.Where(item => tempHeads.Contains(item)).ToList();

            string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            string files = ExportWages.Current.MakeExportExcel(fileName, dataTable.Select(), dataTable.Columns, wageItems);

            Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "工资录入",
                    $"Excel导出：{GetDateIndex()}", string.Empty);

            //下载文件
            DownLoadFile(files);
        }

        /// <summary>
        /// 导出文件（会计）
        /// </summary>
        protected void btnExport_Finance_Click(object sender, EventArgs e)
        {
            if (!IsClosed(wageDate.Value.Replace("-", "")))
            {
                Easyui.Alert("操作提示", "本月未封账，不可导出!", Sign.Error);
                return;
            }

            string file = GetTemplate();       //复制模板文件
            if (string.IsNullOrWhiteSpace(file))
            {
                Easyui.Alert("操作提示", "未找到模板文件!", Sign.Error);
                return;
            }

            var data = GetData(GetExpression());
            if (data == null || !data.Any())
            {
                Easyui.Alert("操作提示", "未找到数据!", Sign.Error);
                return;
            }

            var dataTable = ExportWages.Current.JsonToDataTable(data.Json());
            if (dataTable == null || dataTable.Rows.Count <= 0)
            {
                Easyui.Alert("操作提示", "未找到数据!", Sign.Error);
                return;
            }


            var companyies = from DataRow myRow in dataTable.Rows
                             group myRow by myRow["CompanyName"];

            string date = wageDate.Value.Replace("-", "年") + "月";
            string fileExt = Path.GetExtension(file).ToLower(); //文件后缀

            List<string> files = companyies.Select(company => ExportWages.Current.MakeFinanceExcel(file, date + company.Key + "--会计" + fileExt, dataTable.Select("CompanyName='" + company.Key + "'"), dataTable.Columns)).ToList();

            string fileName = Server.MapPath("~/Upload/") + DateTime.Now.ToString("yyyyMMddHHmmss") + "--会计.zip";
            ZipHelper.Zip(files.ToArray(), fileName);

            Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "工资录入",
                    $"会计导出：{GetDateIndex()}", string.Empty);

            //下载文件
            DownLoadFile(fileName);
        }

        /// <summary>
        /// 导出文件（出纳）
        /// </summary>
        protected void btnExport_Cashier_Click(object sender, EventArgs e)
        {
            if (!IsClosed(wageDate.Value.Replace("-", "")))
            {
                Easyui.Alert("操作提示", "本月未封账，不可导出!", Sign.Error);
                return;
            }

            var data = GetData(GetExpression());
            if (data == null || !data.Any())
            {
                Easyui.Alert("操作提示", "未找到数据!", Sign.Error);
                return;
            }

            var dataTable = ExportWages.Current.JsonToDataTable(data.Json());
            if (dataTable == null || dataTable.Rows.Count <= 0)
            {
                Easyui.Alert("操作提示", "未找到数据!", Sign.Error);
                return;
            }


            var companyies = from DataRow myRow in dataTable.Rows
                             group myRow by myRow["CompanyName"];

            string date = wageDate.Value.Replace("-", "年") + "月";
            List<string> files = companyies.Select(company => ExportWages.Current.MakeCashierExcel(date + company.Key + "--出纳.xls", dataTable.Select("CompanyName='" + company.Key + "'"))).ToList();
            string fileName = Server.MapPath("~/Upload/") + DateTime.Now.ToString("yyyyMMddHHmmss") + "--出纳.zip";
            ZipHelper.Zip(files.ToArray(), fileName);

            Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "工资录入",
                    $"出纳导出：{GetDateIndex()}", string.Empty);

            //下载文件
            DownLoadFile(fileName);
        }

        /// <summary>
        /// 导出文件（大赢家）
        /// </summary>
        protected void btnExport_DYJ_Click(object sender, EventArgs e)
        {
            if (!IsClosed(wageDate.Value.Replace("-", "")))
            {
                Easyui.Alert("操作提示", "本月未封账，不可导出!", Sign.Error);
                return;
            }

            var data = GetData(GetExpression());
            if (data == null || !data.Any())
            {
                Easyui.Alert("操作提示", "未找到数据!", Sign.Error);
                return;
            }

            var dataTable = ExportWages.Current.JsonToDataTable(data.Json());
            if (dataTable == null || dataTable.Rows.Count <= 0)
            {
                Easyui.Alert("操作提示", "未找到数据!", Sign.Error);
                return;
            }


            var companyies = from DataRow myRow in dataTable.Rows
                             group myRow by myRow["CompanyName"];

            string date = wageDate.Value.Replace("-", "年") + "月";

            List<string> files = companyies.Select(company => ExportWages.Current.MakeDYJExcel(date + company.Key + "--大赢家.xls", dataTable.Select("CompanyName='" + company.Key + "'"), date)).ToList();

            string fileName = Server.MapPath("~/Upload/") + DateTime.Now.ToString("yyyyMMddHHmmss") + "--大赢家.zip";
            ZipHelper.Zip(files.ToArray(), fileName);

            Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "工资录入",
                    $"大赢家导出：{GetDateIndex()}", string.Empty);

            //下载文件
            DownLoadFile(fileName);
        }

        /// <summary>
        /// 获取工资单状态
        /// </summary>
        /// <returns></returns>
        protected object getStatus()
        {
            //工资日期
            string date = Request.Form["dateIndex"].Replace("-", "");

            var payBill = Alls.Current.PayBills.Where(item => item.DateIndex == date).FirstOrDefault();
            if (payBill == null)
            {
                return -1;
            }

            return payBill.Status;
        }

        /// <summary>
        /// 计算汇总
        /// </summary>
        protected object recalculation()
        {
            var result = new { code = "200", message = "操作成功!" };

            string dateIndex = Request.Form["dateIndex"].Replace("-", "");

            Calc(dateIndex);        //计算并保存

            Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "工资录入",
                    $"计算汇总：{dateIndex}", string.Empty);

            return result;
        }

        #endregion

        #region 私有函数
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <returns></returns>
        private string UploadFile()
        {
            string fileFullName = string.Empty; //上传文件地址

            string sign = ".check";     //添加特殊字符

            try
            {
                if (fileUpload.HasFile)
                {
                    string filePath = Server.MapPath("~/Upload/");
                    //string fileName = Path.GetFileNameWithoutExtension(fileUpload.PostedFile.FileName); //获取文件名（不包括扩展名）
                    //string fileName = DateTime.Now.ToString("yyyyMMddHHmmss");
                    string fileName = wageDate.Value.Replace("-", "") + '-' + Erp.Current.ID + '-' +
                                      DateTime.Now.ToString("MMddHHmmss");
                    string extension = Path.GetExtension(fileUpload.PostedFile.FileName); //获取扩展名
                    fileFullName = filePath + fileName + sign + extension;

                    fileUpload.SaveAs(fileFullName);
                }
            }
            catch (Exception)
            {
                fileFullName = string.Empty;
            }

            return fileFullName;
        }

        /// <summary>
        /// 修改工资项（通过handsontable）
        /// </summary>
        /// <param name="payItem"></param>
        /// <param name="dateIndex"></param>
        /// <returns></returns>
        private string UpdatePayItemByHandstable(PayItem payItem, string dateIndex)
        {
            string result = string.Empty;

            try
            {
                using (var view = new PayItemsRoll())
                {
                    var model = view.FirstOrDefault(item => item.PayID == payItem.PayID && item.Name == payItem.Name);
                    if (model != null)
                    {
                        model.Value = payItem.Value;
                        model.Enter();
                    }
                    else
                    {
                        var entity = new PayItem()
                        {
                            PayID = payItem.PayID,
                            Name = payItem.Name,
                            Value = payItem.Value,
                            DateIndex = dateIndex,
                            AdminID = Erp.Current.ID,
                        };
                        entity.Enter();
                    }
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 判断工资月是否已经封账
        /// </summary>
        /// <param name="dateIndex"></param>
        /// <returns></returns>
        private bool IsClosed(string dateIndex)
        {
            bool result = false;

            using (var view = new PayBillsRoll())
            {
                if (view.Any(item => item.DateIndex == dateIndex && item.Status == PayBillStatus.Closed))
                {
                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// 是否封账
        /// </summary>
        /// <returns></returns>
        protected object isClosed()
        {
            try
            {
                //判断是否已经封账
                string dateIndex = Request["dateIndex"].Replace("-", "");
                if (!IsClosed(dateIndex))
                {
                    return new
                    {
                        code = 400,
                    };
                }

                return new
                {
                    code = 200
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    code = 400,
                    message = "异常错误：" + ex.Message
                };
            }
        }

        /// <summary>
        /// 获取handsontable数据源
        /// </summary>
        /// <returns></returns>
        public List<dynamic> GetData(Expression<Func<StaffPayItem, bool>> predicate)
        {
            var data = Alls.Current.PayItems.Where(predicate);
            List<string> dynColumns;
            if (!data.Any())
            {
                return null;
            }
            var result = ExportWages.Current.DynamicLinq(data.OrderBy(item => item.CompanyName).ThenBy(item => item.StaffName).ToList(), GetFixedColumns(), "Name", "Value", out dynColumns);
            return result;

        }

        /// <summary>
        /// 获取动态列
        /// </summary>
        /// <returns></returns>
        private string GetDynamicColumns()
        {
            string dynColumns;
            var staffPayItems = Alls.Current.PayItems.Where(GetExpression());

            if (!staffPayItems.Any())
            {
                //return Alls.Current.WageItems.OrderBy(item => item.OrderIndex).Select(item =>
                //new
                //{
                //    name = item.Name,
                //    isCalc = item.IsCalc
                //}).Json();
                return null;
            }
            //获取我的工资项
            var result = staffPayItems.GroupBy(item => item.Name).Select(item => item.Key).ToList();
            //根据模板去掉其他工资项
            var heads = ExportWages.Current.GetExcelHead(GetTemplate(), "1,2");

            result = result.Where(item => heads.Contains(item)).ToList();

            //获取工资项
            var wageitem = Alls.Current.WageItems.ToList();
            //排序
            var resultOrder = from r in result
                              join w in wageitem on r equals w.Name
                              orderby w.OrderIndex
                              select new { name = r, type = w.Type };
            dynColumns = resultOrder.Json();
            return dynColumns;
        }

        /// <summary>
        /// 查询条件
        /// </summary>
        /// <returns></returns>
        public Expression<Func<StaffPayItem, bool>> GetExpression()
        {
            Expression<Func<StaffPayItem, bool>> predicate = null;

            //工资日期
            string date = wageDate?.Value ?? Request.Form["dateIndex"];
            if (!string.IsNullOrWhiteSpace(date))
            {
                date = date.Replace("-", "");
                predicate = predicate.And(item => item.DateIndex == date);
            }

            //姓名、工号
            string name = txt_name?.Value ?? Request.Form["name"];
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate =
                    predicate.And(item => item.StaffName.Contains(name) || item.DyjCode == name);
            }

            //内部公司
            string company = CompanyId?.Value ?? Request.Form["company"];
            if (!string.IsNullOrWhiteSpace(company))
            {
                predicate = predicate.And(item => item.CompanyName == company);
            }

            //岗位
            string postion = PostionId?.Value ?? Request.Form["postion"];
            if (!string.IsNullOrWhiteSpace(postion))
            {
                predicate = predicate.And(item => item.PostionName == postion);
            }

            //员工状态
            string status = Status?.Value ?? Request.Form["status"];
            if (!string.IsNullOrWhiteSpace(status))
            {
                int s = int.Parse(status);
                predicate = predicate.And(item => item.StaffStatus == s);
            }

            //地区
            string wrokCity = WorkCity?.Value ?? Request.Form["workCity"];
            if (!string.IsNullOrWhiteSpace(wrokCity))
            {
                predicate = predicate.And(item => item.City == wrokCity);
            }

            return predicate;
        }

        /// <summary>
        /// 获取固定列
        /// </summary>
        /// <returns></returns>
        private List<string> GetFixedColumns()
        {
            return new List<string>()
                    {
                        "PayID",
                        "StaffID",
                        "StaffName",
                        //"StaffCode",
                        "DateIndex",
                        //"Status",
                        "City",
                        "CompanyName",
                        "DyjCompanyCode",
                        "DyjDepartmentCode",
                        "PostionName",
                        "IDCard",
                        "DyjCode",
                        "BankAccount",
                        "StaffStatus",
                        "StaffStatusName",
                    };
        }

        /// <summary>
        /// 动态列名
        /// </summary>
        /// <returns></returns>
        protected object getColNames()
        {
            var list = new List<dynamic>()
            {
                new {data="PayID",title="PayID",type="text", readOnly = true},
                new {data="StaffID",title="StaffID",type="text", readOnly = true},
                new {data="City",title="地区",type="text", readOnly = true},
                new {data="CompanyName",title="所属公司",type="text", readOnly = true},
                new {data="DyjCompanyCode",title="分公司",type="text", readOnly = true},
                new {data="DyjDepartmentCode",title="部门",type="text", readOnly = true},
                new {data="DyjCode",title="ID（工号）",type="text", readOnly = true},
                new {data="StaffName",title="姓名",type="text", readOnly = true},
                new {data="PostionName",title="考核岗位",type="text", readOnly = true},
                new {data="IDCard",title="身份证号码",type="text", readOnly = true},
                new {data="StaffStatusName",title="员工状态",type="text", readOnly = true},
            };
            var dynColumns = GetDynamicColumns();
            if (dynColumns != null)
            {
                var result = dynColumns.JsonTo<dynamic>();
                string fontStyle = "<label style='color:{2}' title='{1}'>{0}</label>";

                //根据是否提交修改标题颜色
                var importItems = Alls.Current.PayItems.GetImportPayItemsStatus(GetDateIndex());
                PayItemInputer model = null;

                foreach (var d in result)
                {
                    if (d.type == WageItemType.Normal)
                    {
                        if (importItems.Any(t => t.PayItemName == (string)d.name))
                        {
                            model = importItems.FirstOrDefault(t => t.PayItemName == (string)d.name);

                            list.Add(new { data = d.name, title = string.Format(fontStyle, d.name, model.IsImport ? $"已提交({model.InputerName})" : $"待提交({model.InputerName})", model.IsImport ? "#EE6363" : "#8DB6CD"), type = "text" });
                        }
                        else
                        {
                            list.Add(new { data = d.name, title = d.name, type = "text" });
                        }
                    }
                    else
                    {
                        list.Add(new { data = d.name, title = d.name, type = "text", readOnly = true });
                    }
                }
            }
            //else
            //{
            //    return "[]";
            //}
            return list.Json();
        }

        /// <summary>
        /// 去重复
        /// </summary>
        /// <returns></returns>
        private List<PayItem> RemoveDuplicate(PayItem[] oldItem)
        {
            List<PayItem> list = new List<PayItem>();

            if (oldItem.Length <= 0) return null;

            //倒序去重复
            for (int i = oldItem.Length - 1; i >= 0; i--)
            {
                if (!list.Any(t => t.PayID == oldItem[i].PayID && t.Name == oldItem[i].Name))
                {
                    list.Add(oldItem[i]);
                }
            }

            return list;
        }

        /// <summary>
        /// 获取内部公司
        /// </summary>
        /// <returns></returns>
        protected object getCompanies()
        {
            var result = Alls.Current.Companies.Select(item => new { Value = item.Name, Text = item.Name }).ToList();
            result.Insert(0, new { Value = "", Text = "全部" });

            return result;
        }

        /// <summary>
        /// 获取岗位
        /// </summary>
        /// <returns></returns>
        protected object getPostions()
        {
            var result = Alls.Current.Postions.Select(item => new { Value = item.Name, Text = item.Name }).ToList();
            result.Insert(0, new { Value = "", Text = "全部" });

            return result;
        }

        /// <summary>
        /// 获取员工状态
        /// </summary>
        /// <returns></returns>
        protected object getStaffStatus()
        {
            string[] enums = new string[]
            {
               ((int)StaffStatus.Period).ToString(),
               ((int)StaffStatus.Normal).ToString(),
               ((int)StaffStatus.Departure).ToString(),
               ((int)StaffStatus.Cancel).ToString(),
            };

            var result = ExtendsEnum.ToDictionary<Services.StaffStatus>()
                .Where(item => enums.Contains(item.Key))
                .Select(item => new { Value = item.Key, Text = item.Value }).ToList();
            result.Insert(0, new { Value = "", Text = "全部" });
            return result;
        }

        /// <summary>
        /// 获取地区
        /// </summary>
        /// <returns></returns>
        protected object getWorkCities()
        {
            var result = Alls.Current.LeaguesRolls.Where(item => item.Category == Category.StaffInCity && item.FatherID != "").Select(item => new { Value = item.Name, Text = item.Name }).ToList();
            result.Insert(0, new { Value = "", Text = "全部" });
            return result;
        }

        /// <summary>
        /// 获取模板文件
        /// </summary>
        private string GetTemplate()
        {
            return Server.MapPath("~/Upload/Template/template.xls");
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="fileName"></param>
        private void DownLoadFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return;
            }

            string name = fileName.Substring(fileName.LastIndexOf('\\') + 1, fileName.Length - Path.GetExtension(fileName).Length - fileName.LastIndexOf('\\') - 1);

            FileInfo fileInfo = new FileInfo(fileName);
            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.AddHeader("Content-Disposition", "attachment;filename=" + name + Path.GetExtension(fileName).ToLower());
            Response.AddHeader("Content-Length", fileInfo.Length.ToString());
            Response.AddHeader("Content-Transfer-Encoding", "binary");
            Response.ContentType = "application/octet-stream";
            Response.ContentEncoding = System.Text.Encoding.GetEncoding("gb2312");
            Response.WriteFile(fileInfo.FullName);
            Response.Flush();
            Response.End();
        }

        /// <summary>
        /// 数据验证
        /// </summary>
        /// <returns></returns>
        private Dictionary<int, string> CheckData(DataTable dt, out List<PayBillItem> list)
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            list = new List<PayBillItem>();
            string dateIndex = wageDate.Value.Replace("-", "");

            var staffs = Alls.Current.Staffs.ToList();       //所有员工
            var staffItems = new MyWageItemsRoll().ToList();        //所有员工工资项默认值
            var wageItems = Alls.Current.WageItems.ToList();        //工资项
            var companies = Alls.Current.Companies.ToList();        //所有内部公司
            var payBills = Alls.Current.PayBills.Where(item => item.DateIndex == dateIndex).ToList();
            var payItems = Alls.Current.PayItems.Where(item => item.DateIndex == dateIndex).ToList();        //当前月所有工资项

            string msg = string.Empty;
            string staffId = string.Empty;
            string enterpriseId = string.Empty;
            string postionId = string.Empty;

            //重复身份证号
            var repeatIdCards = from idCard in dt.AsEnumerable()
                                group idCard by idCard.Field<string>(IdCard_Column) into grp
                                where grp.Count() > 1
                                select grp.Key;


            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //判断是否包含所属公司
                //if (!companies.Contains(dt.Rows[i]["所属公司"]))
                //{
                //    dic.Add(i + 2, $"【{dt.Rows[i]["所属公司"]}】不存在，请检查公司名称是否一致!");
                //    continue;
                //}

                //判断是否有重复身份证
                if (repeatIdCards.Contains(dt.Rows[i][IdCard_Column]))
                {
                    dic.Add(i + 2, "身份证号码重复!");
                    continue;
                }

                //员工信息校验
                msg = CheckStaffInfo(dt.Rows[i], staffs, companies, out staffId, out enterpriseId, out postionId);
                if (!string.IsNullOrWhiteSpace(msg))
                {
                    dic.Add(i + 2, msg);
                    continue;
                }

                list.Add(new PayBillItem()
                {
                    Status = PayBillStatus.Check,
                    ID = staffId.Replace("Staff", "") + '-' + dateIndex,
                    StaffID = staffId,
                    CreaetDate = DateTime.Now,
                    DateIndex = dateIndex,
                    EnterpriseID = enterpriseId,
                    PostionID = postionId,
                    IsInsert = payBills.All(item => item.ID != (staffId.Replace("Staff", "") + '-' + dateIndex)),       //是否新增
                    IsUpdate = payBills.Any(item => item.ID == (staffId.Replace("Staff", "") + '-' + dateIndex) && (item.EnterpriseID != enterpriseId || item.PostionID != postionId))       //是否修改企业
                });

                //工资列数据校验
                msg = CheckWageItemData(dt.Rows[i], dt.Columns, staffItems.Where(item => item.ID == staffId).ToList(), wageItems, payItems.Where(item => item.PayID == (staffId.Replace("Staff", "") + '-' + dateIndex)).ToList(), staffId, enterpriseId, list);
                if (!string.IsNullOrWhiteSpace(msg))
                {
                    dic.Add(i + 2, msg);
                    continue;
                }
            }

            return dic;
        }

        /// <summary>
        /// 检查数据
        /// </summary>
        /// <param name="payItems"></param>
        /// <returns></returns>
        private string CheckDataByPayItem(PayItem[] payItems)
        {
            string result = string.Empty;

            if (payItems.Length <= 0)
            {
                return "数据不能为空!";
            }

            var wageItems = Alls.Current.WageItems.ToList();        //工资项
            var staffItems = new MyWageItemsRoll().ToList();        //所有员工工资项默认值

            foreach (var payItem in payItems)
            {
                //判断该项是否属于工资项
                if (wageItems.All(t => t.Name != payItem.Name))
                {
                    continue;
                }

                //判断我的工资项是否包含该项
                if (!staffItems.Any(t => t.ID == payItem.StaffID && t.WageItemName == payItem.Name))
                {
                    return $"修改的人不包含[{payItem.Name}]";
                }

                //判断该项是否可变更
                if (!wageItems.FirstOrDefault(t => t.Name == payItem.Name).IsImport)
                {
                    return $"[{payItem.Name}]不可更改!";
                }
            }

            return result;
        }

        /// <summary>
        /// 检查员工信息
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="staffs"></param>
        /// <returns></returns>
        private string CheckStaffInfo(DataRow dr, List<Staff> staffs, List<Company> companies, out string staffId, out string enterpriseId, out string postionId)
        {
            string result = string.Empty;
            staffId = string.Empty;
            enterpriseId = string.Empty;
            postionId = string.Empty;

            try
            {
                //身份证号码
                if (string.IsNullOrWhiteSpace(dr[IdCard_Column].ToString()))
                {
                    result += "身份证号码不能为空! ";
                }


                //判断员工是否存在
                var staff = staffs.FirstOrDefault(item => item.IDCard == dr[IdCard_Column].ToString());
                if (string.IsNullOrWhiteSpace(staff?.ID))
                {
                    result += $"该员工[{dr["姓名"]}]不存在! ";
                    return result;
                }
                staffId = staff.ID;
                enterpriseId = staff.EnterpriseID;
                postionId = staff.PostionID;

                //姓名
                if (string.IsNullOrWhiteSpace(staff.Name))
                {
                    result += "姓名不能为空! ";
                }

                //if (staff.Status == StaffStatus.Departure)
                //{
                //    result += $"{staff.Name}已离职!";
                //}
                if (staff.Status == StaffStatus.Delete)
                {
                    result += $"{staff.Name}已废弃!";
                }
                if (staff.Status == StaffStatus.Cancel)
                {
                    result += $"{staff.Name}已注销!";
                }
                if (staff.Status == StaffStatus.UnApplied
                    || staff.Status == StaffStatus.InterviewFail
                    || staff.Status == StaffStatus.InterviewPass
                    || staff.Status == StaffStatus.Applied)
                {
                    result += $"{staff.Name}还未办理入职!";
                }

                //所属公司
                if (string.IsNullOrWhiteSpace(staff.EnterpriseID))
                {
                    result += "所属公司不能为空! ";
                }

                if (companies.All(item => item.ID != staff.EnterpriseID))
                {
                    result += "请检查员工信息的所属公司是否正确! ";
                }

                //excel的公司和staff公司是否一致
                if (staff.EnterpriseCompany != dr["所属公司"].ToString())
                {
                    result += "所属公司和员工信息公司不一致! ";
                }

                //地区
                if (string.IsNullOrWhiteSpace(staff.CityName))
                {
                    result += "地区不能为空! ";
                }


                //大赢家 分公司编码
                if (string.IsNullOrWhiteSpace(staff.DyjCompanyCode))
                {
                    result += $"分公司不能为空! ";
                }

                //大赢家 部门编码
                if (string.IsNullOrWhiteSpace(staff.DyjDepartmentCode))
                {
                    result += "部门不能为空! ";
                }

                //ID（工号）
                if (string.IsNullOrWhiteSpace(staff.DyjCode))
                {
                    result += "ID（工号）不能为空! ";
                }

                //考核岗位
                if (string.IsNullOrWhiteSpace(staff.PostionID))
                {
                    result += "考核岗位不能为空! ";
                }

                //银行卡号
                //if (string.IsNullOrWhiteSpace(staff.BankAccount))
                //{
                //    result += "银行卡号不能为空! ";
                //}

                if (!string.IsNullOrWhiteSpace(result))
                {
                    result = staff?.Name + result;
                    result = result.Replace(staff?.Name + staff?.Name, staff?.Name);
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 校验员工的工资项
        /// </summary>
        /// <param name="dr">数据</param>
        /// <param name="columns">数据列的集合</param>
        /// <param name="staffWageItems">我的工资项</param>
        /// <param name="wageItems">工资项</param>
        /// <returns></returns>
        private string CheckWageItemData(DataRow dr, DataColumnCollection columns, List<StaffWageItem> staffWageItems, List<WageItem> wageItems, List<StaffPayItem> payItems, string staffId, string enterpriseId, List<PayBillItem> list)
        {
            string result = string.Empty;

            if (staffWageItems == null || staffWageItems.Count <= 0)
            {
                return "没有设置我的工资项!";
            }

            decimal? defaultVal = null;     //默认值
            decimal? excelVal = null;       //工资项的值
            StaffWageItem wageItem = null;
            string colName = string.Empty;      //列名
            string payId_temp = string.Empty;
            List<PayItem> list_temp = new List<PayItem>(); ;

            foreach (var column in columns)
            {
                colName = column.ToString();

                //判断该项是否属于工资项
                if (wageItems.All(item => item.Name != colName))
                {
                    continue;
                }

                //获取该工资项的值
                if (!string.IsNullOrWhiteSpace(dr[colName].ToString()))
                {
                    try
                    {
                        excelVal = decimal.Parse(dr[colName].ToString());
                    }
                    catch (Exception e)
                    {
                        excelVal = null;
                    }
                }
                else
                {
                    excelVal = null;
                }


                //判断我的工资项列表是否包含该工资项
                if (staffWageItems.All(item => item.WageItemName != colName))
                {
                    //如果不包含
                    //判断该工资是不是有值，如果有值提示错误信息
                    if (excelVal != null && excelVal != 0)
                    {
                        result += $"您不包含[{column}]，不可以填入值! ";
                    }
                }
                else
                {
                    wageItem = staffWageItems.FirstOrDefault(item => item.WageItemName == colName);
                    defaultVal = wageItem?.DefaultValue;

                    //if (wageItem?.Status != Status.Normal && excelVal != null && excelVal != 0)
                    //{
                    //    result += $"[{column}]已经删除，您不可以填入值! ";
                    //}

                    //如果包含
                    //判断该工资项和默认值是否一致
                    if (excelVal != defaultVal && excelVal != null)
                    {
                        //如果不一致 判断该工资是否可变更
                        if (!wageItem.IsImport)
                        {
                            //如果不可变更，提示错误信息
                            result += $"[{column}]是不可变更项，您不可以填入值! ";
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(result) && staffWageItems.Any(item => item.WageItemName == colName))
                {
                    payId_temp = staffId.Replace("Staff", "") + "-" + wageDate.Value.Replace("-", "");
                    if (payItems.Any(item => item.PayID == payId_temp && item.Name == colName && item.Value == (excelVal ?? 0)))
                    {
                        continue;
                    }

                    list_temp.Add(new PayItem()
                    {
                        DateIndex = wageDate.Value.Replace("-", ""),
                        Name = colName,
                        PayID = payId_temp,
                        IsInsert = !payItems.Any(item => item.PayID == payId_temp && item.Name == colName),     //判断添加还是更新
                        //Value = wageItems.FirstOrDefault(item => item.Name == colName).IsCalc ? 0 : (excelVal ?? 0),
                        Value = (excelVal ?? 0),
                        AdminID = Erp.Current.ID,
                        ID = enterpriseId,
                        WageItemFormula = wageItems.FirstOrDefault(item => item.Name == colName).Formula,
                        Status = PayItemStatus.Save,
                    });
                }
            }

            //如果没有更新，计算列不需要重新计算
            //if (list_temp.Any())
            {
                //补充计算列和数据列
                foreach (var wItem in wageItems.Where(t => t.Type != WageItemType.Normal))
                {
                    if (list_temp.All(t => t.Name != wItem.Name))
                    {
                        list_temp.Add(new PayItem()
                        {
                            DateIndex = wageDate.Value.Replace("-", ""),
                            Name = wItem.Name,
                            PayID = payId_temp,
                            IsInsert = !payItems.Any(item => item.PayID == payId_temp && item.Name == wItem.Name),     //判断添加还是更新
                            AdminID = Erp.Current.ID,
                            ID = enterpriseId,
                            WageItemFormula = wageItems.FirstOrDefault(item => item.Name == wItem.Name)?.Formula,
                            Status = PayItemStatus.Save,
                        });
                    }
                }
            }


            if (list_temp.Any() && list.FirstOrDefault(item => item.ID == payId_temp) != null)
            {
                list.FirstOrDefault(item => item.ID == payId_temp).PayItems = list_temp;
            }

            if (!string.IsNullOrWhiteSpace(result))
            {
                result = $"身份证号为[{dr[IdCard_Column]}] 出错!" + result;
            }



            return result;
        }

        /// <summary>
        /// 初始化公式模板
        /// </summary>
        private void InitFormulaTemplate()
        {
            //if (Formulas.Current.data.Count == 0)
            {
                foreach (var item in Alls.Current.WageItems.Where(item => item.Type == WageItemType.Calc))
                {
                    Formulas.Create(item.Name, item.Formula);
                }
            }
        }

        /// <summary>
        /// 计算
        /// </summary>
        /// <param name="allList"></param>
        private void Formula(List<PayBillItem> allList, List<StaffPayItem> payItems = null)
        {
            InitFormulaTemplate();

            using (var view = new PayWageItemRoll())
            using (var paybillsView = new PayBillsRoll())
            {
                string dateIndex = GetDateIndex();
                string lastYear = (int.Parse(dateIndex.Substring(0, 4)) - 1).ToString();
                var wageItems = Alls.Current.WageItems.ToList();        //所有工资项
                payItems = payItems == null ? Alls.Current.PayItems.Where(item => item.DateIndex == dateIndex).ToList() : payItems;       //当前月份工资项
                var pastItems = Erp.Current.Erm.PastItems.Where(item => item.DateIndex == (int.Parse(dateIndex) - 1)).ToList();     //历史累计值
                var pastLastYearItems = Erp.Current.Erm.PastItems.Where(item => item.Type == WageItemType.AccumIncome &&
                    (item.DateIndex >= int.Parse(lastYear + "01") && item.DateIndex <= int.Parse(lastYear + "12"))).ToArray();      //去年历史累计收入
                var personalRates = Alls.Current.PersonalRates.ToList();        //个税税率
                var payBills = paybillsView.Where(item => item.DateIndex.Contains(DateTime.Now.AddYears(-1).Year.ToString())).ToArray();

                foreach (var item in allList)
                {
                    if (item.PayItems == null || !item.PayItems.Any())
                    {
                        continue;
                    }

                    //整理工资项
                    var list = item.PayItems.Select(t => new PayWageItem()
                    {
                        Name = t.Name,
                        Value = t.Value,
                        Type = wageItems.FirstOrDefault(tt => tt.Name == t.Name) != null ? wageItems.FirstOrDefault(tt => tt.Name == t.Name).Type : WageItemType.Normal,
                    }).ToList();

                    //关联工资项 把没有填入的值从数据库获取数来，用来计算
                    list = (from pi in payItems.Where(t => t.StaffID == item.StaffID)
                            join pp in item.PayItems on pi.Name equals pp.Name into pp
                            join wi in wageItems on pi.Name equals wi.Name
                            from view_pp in pp.DefaultIfEmpty()
                            select new PayWageItem()
                            {
                                Name = pi.Name,
                                Value = view_pp?.Value ?? pi.Value,
                                Type = wi.Type
                            }).ToList();


                    //新增
                    if (!payItems.Any(t => t.StaffID == item.StaffID))
                    {
                        list = (from pp in item.PayItems
                                join wi in wageItems on pp.Name equals wi.Name
                                select new PayWageItem()
                                {
                                    Name = wi.Name,
                                    Value = pp.Value,
                                    Type = wi.Type
                                }).ToList();
                    }

                    if (list.Any())
                    {
                        //计算并保存至List中
                        var payWageItems = new PayWageItems(list,
                                pastItems.Where(t => t.StaffID == item.StaffID && t.EnterpriseID == item.EnterpriseID),
                                personalRates, dateIndex, IsFixed(payBills.Where(p => p.StaffID == item.StaffID).ToArray(), pastLastYearItems.Where(t => t.StaffID == item.StaffID).ToArray()))
                            .Where(t => t.Type != WageItemType.Normal);
                        foreach (var v in payWageItems)
                        {
                            if (item.PayItems.FirstOrDefault(t => t.Name == v.Name) != null)
                            {
                                item.PayItems.FirstOrDefault(t => t.Name == v.Name).Value = v.Value;
                            }

                        }
                    }
                }
            }
        }

        /// <summary>
        /// 计算个税
        /// </summary>
        /// <param name="dateIndex"></param>
        /// <param name="payId"></param>
        private void Calc(string dateIndex, string payId = "")
        {
            var payBills = string.IsNullOrWhiteSpace(payId) ? Alls.Current.PayBills.Where(item => item.DateIndex == dateIndex).ToList() : Alls.Current.PayBills.Where(item => item.DateIndex == dateIndex && item.ID == payId).ToList();
            var payItems = Alls.Current.PayItems.Where(item => item.DateIndex == dateIndex).ToList();
            var wageItems = Alls.Current.WageItems.ToList();

            //整理数据
            var list = (from pb in payBills
                        select new PayBillItem
                        {
                            ID = pb.StaffID.Replace("Staff", "") + "-" + pb.DateIndex,
                            DateIndex = pb.DateIndex,
                            StaffID = pb.StaffID,
                            EnterpriseID = pb.EnterpriseID,
                            IsInsert = false,
                            IsUpdate = false,
                            PayItems = (from pi in payItems
                                        join wi in wageItems on pi.Name equals wi.Name
                                        where wi.Type != WageItemType.Normal && pi.PayID == pb.ID
                                        select new PayItem
                                        {
                                            DateIndex = pi.DateIndex,
                                            Name = pi.Name,
                                            PayID = pi.PayID,
                                            IsInsert = false,
                                            Value = 0,
                                            ID = pb.EnterpriseID,
                                            WageItemFormula = wi.Formula,
                                        }).ToList()
                        }).ToList();

            Formula(list, payItems);
            list.Enter();
        }

        /// <summary>
        /// 计算个税
        /// </summary>
        /// <param name="dateIndex"></param>
        /// <param name="payId"></param>
        //private void CalcV1(string dateIndex, List<PastsItem> pastItems, string payId = "")
        //{
        //    InitFormulaTemplate();

        //    var payBills = string.IsNullOrWhiteSpace(payId) ? Alls.Current.PayBills.Where(item => item.DateIndex == dateIndex).ToList() : Alls.Current.PayBills.Where(item => item.DateIndex == dateIndex && item.ID == payId).ToList();
        //    var payItems = Alls.Current.PayItems.Where(item => item.DateIndex == dateIndex).ToList();
        //    pastItems = pastItems ?? Erp.Current?.Erm?.PastItems.Where(item => item.DateIndex == (int.Parse(dateIndex) - 1)).ToList();     //历史累计值
        //    var personalRates = Alls.Current.PersonalRates.ToList();        //个税税率

        //    //整理数据
        //    var list = (from pb in payBills
        //                select new PayBillItem
        //                {
        //                    ID = pb.ID,
        //                    DateIndex = pb.DateIndex,
        //                    StaffID = pb.StaffID,
        //                    EnterpriseID = pb.EnterpriseID,
        //                    IsInsert = false,
        //                    IsUpdate = false,
        //                    PayItems = (from pi in payItems
        //                                join r in (new PayWageItems(payItems.Where(item => item.PayID == pb.ID).Select(t => new PayWageItem()
        //                                {
        //                                    Name = t.Name,
        //                                    Type = t.WageType,
        //                                    Value = t.Value,
        //                                }), pastItems, personalRates)).ToList() on pi.Name equals r.Name
        //                                where pi.WageType != WageItemType.Normal && pi.PayID == pb.ID
        //                                select new PayItem
        //                                {
        //                                    DateIndex = pi.DateIndex,
        //                                    Name = pi.Name,
        //                                    PayID = pi.PayID,
        //                                    IsInsert = false,
        //                                    Value = r.Value,
        //                                    ID = pb.EnterpriseID,
        //                                }).ToList()
        //                }).ToList();

        //    list.Enter();
        //}

        /// <summary>
        /// 获取当前工资日期
        /// </summary>
        /// <returns></returns>
        private string GetDateIndex()
        {
            string dateIndex = string.Empty;

            if (wageDate != null)
            {
                dateIndex = wageDate.Value.Replace("-", "");
            }
            else if (!string.IsNullOrWhiteSpace(Request["dateIndex"]))
            {
                dateIndex = Request["dateIndex"].Replace("-", "");
            }

            return dateIndex;
        }

        /// <summary>
        /// 根据日期初始化数据列
        /// </summary>
        /// <returns></returns>
        protected object initDataColumn()
        {
            var json = new { code = 200, msg = "操作成功!" };

            try
            {
                string dateIndex = GetDateIndex();

                //根据月份获取数据
                var payBills = Alls.Current.PayBills.Where(item => item.DateIndex == dateIndex).ToList();
                var payItems = Alls.Current.PayItems.Where(item => item.DateIndex == dateIndex).ToList();
                var wageItems = Alls.Current.WageItems.ToList();
                if (!payItems.Any())
                {
                    return new { code = 400, msg = "当前月份没有数据!" };
                }

                var list = new List<PayBillItem>();
                var temp = new PayBillItem();
                var temp_PayItems = new List<PayItem>();

                foreach (var payBill in payBills)
                {
                    temp = new PayBillItem()
                    {
                        DateIndex = dateIndex,
                        EnterpriseID = payBill.EnterpriseID,
                        ID = payBill.ID,
                        IsInsert = false,
                        IsUpdate = false,
                        StaffID = payBill.StaffID
                    };

                    temp_PayItems = new List<PayItem>();
                    //补充计算列和数据列
                    foreach (var wItem in wageItems.Where(t => t.Type != WageItemType.Normal))
                    {
                        temp_PayItems.Add(new PayItem()
                        {
                            DateIndex = dateIndex,
                            Name = wItem.Name,
                            PayID = payBill.ID,
                            IsInsert = !payItems.Any(item => item.PayID == payBill.ID && item.Name == wItem.Name),
                            AdminID = Erp.Current.ID,
                            ID = payBill.EnterpriseID,
                            WageItemFormula = wageItems.FirstOrDefault(item => item.Name == wItem.Name)?.Formula,
                            Status = PayItemStatus.Save,
                        });
                    }

                    temp.PayItems = temp_PayItems;

                    list.Add(temp);
                }

                Formula(list);
                list.Enter();
                Erp.Current.Erm.PastItems.Rebuild(dateIndex);
            }
            catch (Exception ex)
            {
                return new { code = 500, msg = "操作异常!" + ex.Message };
            }

            return json;
        }

        /// <summary>
        /// 是否是固定 累计免税收入
        /// </summary>
        /// <param name="payBills">某个员工去年的工资单</param>
        /// <returns></returns>
        private bool IsFixed(PayBill[] payBills, PastsItem[] pastsLastYear)
        {
            bool result = true;

            //是否为特殊人员
            if (payBills.Length > 0)
            {
                if (ConfigurationManager.AppSettings["FixedStaffIdsFreeIncomeSpec"].Contains(payBills?.FirstOrDefault()?.StaffID))
                {
                    return result;
                }
            }

            //如果去年不是12个月的工资，按照5000累计计算
            if (payBills.Length < 12)
            {
                result = false;
            }

            //去年换过公司的
            if (payBills.GroupBy(item => item.EnterpriseID).Count() > 1)
            {
                result = false;
            }

            //去年累计收入 大于60000的，按照5000累计计算
            if (pastsLastYear.Length > 0)
            {
                var val = pastsLastYear?.OrderByDescending(item => item.DateIndex)
                    ?.FirstOrDefault(item => item.Type == WageItemType.AccumIncome)?.Value;

                //var val1 = pastsLastYear.LastOrDefault(item => item.Type == WageItemType.AccumIncome)?.Value;
                if (val > 60000)
                {
                    result = false;
                }
            }

            return result;
        }
        #endregion
    }
}