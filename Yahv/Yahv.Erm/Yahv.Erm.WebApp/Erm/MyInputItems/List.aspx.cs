using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Erm.Services;
using Yahv.Erm.Services.Extends;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Erm.Services.Models.Rolls;
using Yahv.Erm.Services.Views.Rolls;
using Yahv.Linq.Extends;
using Yahv.Models;
using Yahv.Underly;
using Yahv.Utils;
using Yahv.Utils.Serializers;
using Yahv.Utils.Validates;
using Yahv.Web.Controls.Easyui;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.Erm.WebApp.Erm.MyInputItems
{
    public partial class List : ErpParticlePage
    {
        #region 常量

        private const string Id_Column = "ID（工号）";
        private const string Name_Column = "姓名";
        private const string Role_Name = "Erm管理员";
        #endregion

        #region Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
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
            string dateIndex = wageDate.Value.Replace("-", "");

            var payBillStatus = Alls.Current.PayBills.GetStatus(dateIndex);

            if (payBillStatus == PayBillStatus.Closed)
            {
                Easyui.Alert("操作提示", $"{dateIndex}已经封账!", Sign.Error);
                return;
            }

            //检查是否有基础数据
            if (!IsHaveData(dateIndex))
            {
                Easyui.Alert("操作提示", $"{dateIndex}没有数据，请您先导入基本工资!", Sign.Error);
                return;
            }

            if (isOnlyRead() && !IsManager())
            {
                Easyui.Alert("操作提示", $"{dateIndex}已经提交，不能再导入!", Sign.Error);
                return;
            }

            string fileFullName = UploadFile();     //上传文件

            if (string.IsNullOrWhiteSpace(fileFullName))
            {
                Easyui.Alert("操作提示", "上传文件失败!", Sign.Error);
                return;
            }

            var dt = UploadStaffs.Current.ExcelToTable(fileFullName);

            if (dt == null || dt.Rows.Count <= 0)
            {
                Easyui.Alert("操作提示", "数据行不能为空!", Sign.Error);
                return;
            }


            if (!dt.Columns.Contains(Id_Column) || !dt.Columns.Contains(Name_Column))
            {
                Easyui.Alert("操作提示", $"数据行必须包含[{Id_Column}]和[{Name_Column}]!", Sign.Error);
                return;
            }

            if (!IsManager())
            {
                var myWageItems = Erp.Current.Erm.MyInputItems.Select(item => item.Name).ToArray();        //我负责的工资项
                                                                                                           //数据列是否有我的工资项
                if (!myWageItems.Any(item => dt.Columns.Contains(item)))
                {
                    Easyui.Alert("操作提示", "数据行不包含您导入的工资项!", Sign.Error);
                    return;
                }

                if (myWageItems.Length <= 0)
                {
                    Easyui.Alert("操作提示", "您没有要导入的工资项!", Sign.Error);
                    return;
                }
            }

            List<PayItem> list = new List<PayItem>();
            //数据检验
            Dictionary<int, string> errorMsgs = CheckData(dt, out list);
            //导出错误excel
            if (errorMsgs.Count > 0)
            {
                DownLoadFile(ExportWages.Current.MakeErrorExcel(fileFullName, errorMsgs, 0));
            }
            else
            {
                list.Enter();
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "奖金提成项导入",
                    $"Excel导入：{dateIndex}", string.Empty);
                Easyui.Alert("操作提示", $"导入成功!");
            }
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

            string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            string files = ExportWages.Current.MakeExportExcel(fileName, dataTable.Select(), dataTable.Columns, GetMyWageItems(), new List<string>() { "身份证号码", "员工状态" });

            Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "累计个税列表",
                    $"Excel导出：{GetDateIndex()}", string.Empty);

            //下载文件
            DownLoadFile(files);
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


                //判断是否已经封账
                string dateIndex = Request["dateIndex"].Replace("-", "");
                var payBillStatus = Alls.Current.PayBills.GetStatus(dateIndex);
                if (payBillStatus == PayBillStatus.Closed)
                {
                    return new
                    {
                        code = 400,
                        message = $"{dateIndex}已经封账!"
                    };
                }

                if (isOnlyRead())
                {
                    return new
                    {
                        code = 400,
                        message = $"{dateIndex}已经提交，不能再导入!"
                    };
                }

                var list = RemoveDuplicate(data);
                if (list.Count > 0)
                {
                    var payItems = Alls.Current.PayItems.Where(item => item.DateIndex == dateIndex).ToList();
                    var myStaffIds = Erp.Current.Erm.MyInputItems.GetMyStaffIds();       //我的工资项下的所有员工
                    var listRemove = new List<PayItem>();
                    string staffId = string.Empty;

                    foreach (var l in list)
                    {
                        staffId = "Staff" + l.PayID.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries)[0];
                        //判断当前人是否有权更改次工资项
                        if (!IsManager() && !myStaffIds[l.Name].Contains(staffId))
                        {
                            listRemove.Add(l);
                            continue;
                        }

                        l.UpdateAdminID = Erp.Current.ID;
                        if (!payItems.Any(item => item.PayID == l.PayID && item.Name == l.Name))
                        {
                            l.DateIndex = dateIndex;
                            l.AdminID = Erp.Current.ID;
                            l.Status = PayItemStatus.Save;
                            l.IsInsert = true;
                        }
                    }

                    if (listRemove.Count > 0)
                    {
                        list = list.Where(item => !listRemove.Contains(item)).ToList();
                    }
                }

                list.Enter();

                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "奖金提成项导入",
                    $"保存：{dateIndex}", list.Json());
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
        /// 提交
        /// </summary>
        /// <returns></returns>
        protected object submit()
        {
            var result = new
            {
                code = 200,
                message = "操作成功!"
            };

            //获取我的工资项状态
            var items = Erp.Current.Erm.MyInputItems.Select(item => item.Name).ToArray();
            if (items.Length <= 0)
            {
                return new { code = 400, message = "未找到我的工资项!" };
            }

            string dateIndex = Request["dateIndex"].Replace("-", "");
            if (string.IsNullOrWhiteSpace(dateIndex))
            {
                return new { code = 400, message = "工资日期不能为空!" };
            }

            Alls.Current.PayItems.SubmitPayItems(items, dateIndex);
            Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "奖金提成项导入",
                    $"提交：{dateIndex}", string.Empty);
            return result;
        }

        /// <summary>
        /// 下载模板
        /// </summary>
        protected void btnDownload_Click(object sender, EventArgs e)
        {
            //获取我的工资项
            var myItems = IsManager() ? Alls.Current.WageItems.Where(item => item.InputerId != "").Select(item => item.Name).ToList()
                : Erp.Current.Erm.MyInputItems.Select(item => item.Name).ToList();        //我的工资项

            //添加固定列
            myItems.Insert(0, "ID（工号）");
            myItems.Insert(1, "姓名");

            var fileName = ExportWages.Current.MakeTemplateExcel(myItems.ToArray());
            DownLoadFile(fileName);
        }

        #endregion

        #region 自定义函数
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
            var result = ExtendsEnum.ToDictionary<Services.StaffStatus>().Select(item => new { Value = item.Key, Text = item.Value }).ToList();
            result.Insert(0, new { Value = "", Text = "全部" });
            return result;
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
                //new {data="StaffStatusName",title="员工状态",type="text", readOnly = true},
            };

            var dynColumns = GetDynamicColumns();
            if (dynColumns != null)
            {
                var result = dynColumns.JsonTo<dynamic>();
                string fontStyle = "<label style='color:{2}' title='{1}'>{0}</label>";

                //根据是否提交修改标题颜色
                var importItems = Alls.Current.PayItems.GetImportPayItemsStatus(GetDateIndex());
                var wageItems = Alls.Current.WageItems.Where(item => item.InputerId != "").ToList();
                PayItemInputer model = null;

                foreach (var d in result)
                {
                    if (d.type == WageItemType.Normal)
                    {
                        //管理员显示提交状态
                        if (IsManager())
                        {
                            if (importItems.Any(t => t.PayItemName == (string)d.name))
                            {
                                model = importItems.FirstOrDefault(t => t.PayItemName == (string)d.name);
                                list.Add(
                                    new
                                    {
                                        data = d.name,
                                        title =
                                            string.Format(fontStyle, d.name,
                                                model.IsImport
                                                    ? $"已提交({model.InputerName})"
                                                    : $"待提交({model.InputerName})", model.IsImport ? "#EE6363" : "#8DB6CD"),
                                        type = "text"
                                    });
                            }
                            else
                            {
                                list.Add(
                                   new
                                   {
                                       data = d.name,
                                       title = string.Format(fontStyle, d.name, $"待提交({wageItems.FirstOrDefault(item => item.Name == (string)d.name)?.InputerName}", "#8DB6CD"),
                                       type = "text"
                                   });
                            }
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

            return list.Json();
        }


        /// <summary>
        /// 获取动态列
        /// </summary>
        /// <returns></returns>
        private string GetDynamicColumns()
        {
            string result = string.Empty;

            //管理员可以看到所有录入列
            if (IsManager())
            {
                result = Alls.Current.WageItems.Where(item => item.InputerId != "")
                    .OrderBy(item => item.OrderIndex)
                    .Select(item => new
                    {
                        name = item.Name,
                        type = item.Type,
                    }).Json();
            }
            else
            {
                result = Erp.Current.Erm.MyInputItems.OrderBy(item => item.OrderIndex).Select(item => new
                {
                    name = item.Name,
                    type = item.Type,
                }).Json();
            }

            return result;
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

        protected object getData()
        {
            var data = GetData(GetExpression());
            return data;
        }

        /// <summary>
        /// 查询条件
        /// </summary>
        /// <returns></returns>
        public Expression<Func<StaffPayItem, bool>> GetExpression()
        {
            Expression<Func<StaffPayItem, bool>> predicate = item => true;

            //工资日期
            string date = wageDate?.Value ?? Request.Form["dateIndex"];
            if (!string.IsNullOrWhiteSpace(date))
            {
                date = date.Replace("-", "");
                predicate = predicate.And(item => item.DateIndex == date);
            }

            //只显示我的工资项下的员工
            var myStaffIds = Erp.Current.Erm.MyInputItems.GetMyAllStaffIds();
            if (myStaffIds.Length <= 0)
            {
                //管理员看到所有员工
                if (IsManager())
                {
                    myStaffIds =
                        Alls.Current.PayBills.Where(item => item.DateIndex == date).Select(item => item.StaffID).ToArray();
                }
                else
                    predicate = predicate.And(item => false);
            }
            else
            {
                predicate = predicate.And(item => myStaffIds.Contains(item.StaffID));
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

            return predicate;
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
        /// 是否为只读
        /// </summary>
        /// <returns></returns>
        protected bool isOnlyRead()
        {
            string dateIndex = GetDateIndex();

            if (IsManager() && !IsClosed(dateIndex))
                return false;

            var status = Alls.Current.PayItems.GetImportPayItemsStatus(dateIndex, Erp.Current.ID);
            return status.All(t => t.IsImport);
        }

        /// <summary>
        /// 判断是否有数据
        /// </summary>
        /// <param name="dateIndex">工资日期</param>
        /// <returns></returns>
        private bool IsHaveData(string dateIndex)
        {
            bool result = false;

            using (var view = new PayBillsRoll())
            {
                if (view.Any(item => item.DateIndex == dateIndex))
                {
                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <returns></returns>
        private string UploadFile()
        {
            string fileFullName = string.Empty; //上传文件地址

            try
            {
                if (fileUpload.HasFile)
                {
                    string filePath = Server.MapPath("~/Upload/");
                    string fileName = DateTime.Now.ToString("yyyyMMddHHmmss");
                    string extension = Path.GetExtension(fileUpload.PostedFile.FileName); //获取扩展名
                    fileFullName = filePath + fileName + extension;

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
        /// 数据验证
        /// </summary>
        /// <returns></returns>
        private Dictionary<int, string> CheckData(DataTable dt, out List<PayItem> list)
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            list = new List<PayItem>();
            string dateIndex = wageDate.Value.Replace("-", "");

            var myWageItems = IsManager() ? Alls.Current.WageItems.Where(item => item.InputerId != "").Select(item => item.Name).ToArray()
                : Erp.Current.Erm.MyInputItems.Select(item => item.Name).ToArray();        //我的工资项
            var myStaffIds = Erp.Current.Erm.MyInputItems.GetMyStaffIds();       //我的工资项下的所有员工
            var payItems = Alls.Current.PayItems.Where(item => item.DateIndex == dateIndex).ToList();        //当前月所有工资项
            var payBills = Alls.Current.PayBills.Where(item => item.DateIndex == dateIndex).ToList();       //当前月所有工资单
            var staffs = Alls.Current.Staffs.Where(item => item.Status == StaffStatus.Period || item.Status == StaffStatus.Normal || item.Status == StaffStatus.Departure).ToList();

            string msg = string.Empty;
            string staffId = string.Empty;
            string enterpriseId = String.Empty;

            //重复人员（根据工号和名称）
            var repeatStaffs = from data in dt.AsEnumerable()
                               group data by new { id = data.Field<string>(Id_Column), name = data.Field<string>(Name_Column) } into grp
                               where grp.Count() > 1
                               select grp.Key.id + "_" + grp.Key.name;


            string payId_temp;
            Staff staff = null;
            PayBillItem payBillItem = null;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //判断是否有重复身份证
                if (repeatStaffs.Contains(dt.Rows[i][Id_Column] + "_" + dt.Rows[i][Name_Column]))
                {
                    dic.Add(i + 1, $"{dt.Rows[i][Id_Column] + "_" + dt.Rows[i][Name_Column]}员工信息重复!");
                    continue;
                }

                staff =
                staffs.FirstOrDefault(
                    item =>
                        item.DyjCode == dt.Rows[i][Id_Column].ToString() &&
                        item.Name == dt.Rows[i][Name_Column].ToString());

                //判断员工是否存在
                if (string.IsNullOrWhiteSpace(staff?.ID))
                {
                    dic.Add(i + 1, $"[{dt.Rows[i][Name_Column]}]不存在，请检查{Id_Column}和{Name_Column}是否填写正确!");
                    continue;
                }

                payId_temp = staff.ID.Replace("Staff", "") + "-" + dateIndex;

                //判断工资单是否存在
                if (payBills.All(item => item.ID != payId_temp))
                {
                    dic.Add(i + 1, $"该员工工资单不存在!");
                    continue;
                }

                //循环我的工资项
                foreach (var wageItem in myWageItems)
                {
                    if (dt.Columns.Contains(wageItem))
                    {
                        //判断当前人是否有权更改次工资项
                        if (!IsManager() && !myStaffIds[wageItem].Contains(staff.ID))
                        {
                            dic.Add(i + 1, $"您没有权限在[{wageItem}]填入值!");
                            break;
                        }

                        try
                        {
                            //如果非数字，直接跳过
                            Convert.ToDecimal(dt.Rows[i][wageItem].ToString());

                            if (!string.IsNullOrWhiteSpace(dt.Rows[i][wageItem].ToString()))
                            {
                                if (payItems.FirstOrDefault(t => t.PayID == payId_temp && t.Name == wageItem) != null)
                                {
                                    if (decimal.Parse(dt.Rows[i][wageItem].ToString()) ==
                                        payItems.FirstOrDefault(t => t.PayID == payId_temp && t.Name == wageItem).Value)
                                    {
                                        continue;
                                    }
                                }
                                list.Add(new PayItem()
                                {
                                    DateIndex = dateIndex,
                                    Name = wageItem,
                                    PayID = payId_temp,
                                    IsInsert = !payItems.Any(item => item.PayID == payId_temp && item.Name == wageItem),     //判断添加还是更新
                                    Value = decimal.Parse(dt.Rows[i][wageItem].ToString()),
                                    AdminID = Erp.Current.ID,
                                    UpdateAdminID = Erp.Current.ID,
                                    Status = PayItemStatus.Save,
                                });
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }

            return dic;
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
        /// 是否为管理员(SA或Erm管理员)
        /// </summary>
        /// <returns></returns>
        private bool IsManager()
        {
            return Erp.Current.ID == "SA01" || Erp.Current.Role.Name == Role_Name;
        }

        /// <summary>
        /// 获取我的工资列
        /// </summary>
        /// <returns></returns>
        private List<string> GetMyWageItems()
        {
            var wageItems = new List<string>();
            var dynColumns = GetDynamicColumns();
            if (dynColumns != null)
            {
                var result = dynColumns.JsonTo<dynamic>();
                foreach (var d in result)
                {
                    wageItems.Add(d.name.ToString());
                }
            }

            return wageItems;
        }
        #endregion
    }
}