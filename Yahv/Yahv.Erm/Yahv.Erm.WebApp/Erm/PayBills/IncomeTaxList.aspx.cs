using System;
using System.Collections.Generic;
using System.Data;
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
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Utils;
using Yahv.Utils.Serializers;
using Yahv.Web.Controls.Easyui;
using Yahv.Web.Erp;
using Yahv.Web.Forms;
using Staff = Yahv.Erm.Services.Models.Origins.Staff;

namespace Yahv.Erm.WebApp.Erm.PayBills
{
    public partial class IncomeTaxList : ErpParticlePage
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

            var wageItems = new List<string>()
            {
                "累计收入",
                "累计免税收入",
                "累计专项扣除",
                "累计专项附加扣除",
                "累计专项附加调整列",
                "累计个税起征点调整",
                "累计预扣预缴应纳税所得额",
                "累计已预扣预缴税额",
                "本月个税",
            };

            string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            string files = ExportWages.Current.MakeExportExcel(fileName, dataTable.Select(), dataTable.Columns, wageItems,new List<string>() { "身份证号码", "员工状态" });

            Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "累计个税列表",
                    $"Excel导出：{GetDateIndex()}", string.Empty);

            //下载文件
            DownLoadFile(files);
        }

        #endregion

        #region 私有函数

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
            var result = ExportWages.Current.DynamicLinq(data.ToList(), GetFixedColumns(), "Name", "Value", out dynColumns);
            return result;

        }

        /// <summary>
        /// 获取动态列
        /// </summary>
        /// <returns></returns>
        private string GetDynamicColumns()
        {
            return null;
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
            //获取工资项
            var wageitem = Alls.Current.WageItems.ToList();
            //排序
            var resultOrder = from r in result
                              join w in wageitem on r equals w.Name
                              orderby w.OrderIndex
                              select new { name = r, Type = w.Type };
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

            ////员工状态
            //string status = Status?.Value ?? Request.Form["status"];
            //if (!string.IsNullOrWhiteSpace(status))
            //{
            //    int s = int.Parse(status);
            //    predicate = predicate.And(item => item.StaffStatus == s);
            //}

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
                new {data="累计收入",title="累计收入",type="text", readOnly = true},
                new {data="累计免税收入",title="累计免税收入",type="text", readOnly = true},
                new {data="累计专项扣除",title="累计专项扣除",type="text", readOnly = true},
                new {data="累计专项附加扣除",title="累计专项附加扣除",type="text", readOnly = true},
                new {data="累计专项附加调整列",title="累计专项附加调整列",type="text", readOnly = true},
                new {data="累计个税起征点调整",title="累计个税起征点调整",type="text", readOnly = true},
                new {data="累计预扣预缴应纳税所得额",title="累计预扣预缴应纳税所得额",type="text", readOnly = true},
                new {data="累计已预扣预缴税额",title="累计已预扣预缴税额",type="text", readOnly = true},
                new {data="本月个税",title="本月个税",type="text", readOnly = true},
            };

            return list.Json();
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
            var result = ExtendsEnum.ToDictionary<Services.StaffStatus>().Select(item => new { Value = item.Key, Text = item.Value }).ToList();
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
            string enterpriseId = String.Empty;

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
                msg = CheckStaffInfo(dt.Rows[i], staffs, companies, out staffId, out enterpriseId);
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
                    IsInsert = payBills.All(item => item.ID != (staffId.Replace("Staff", "") + '-' + dateIndex)),       //是否新增
                    IsUpdate = payBills.Any(item => item.ID == (staffId.Replace("Staff", "") + '-' + dateIndex) && item.EnterpriseID != enterpriseId)       //是否修改企业
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
        private string CheckStaffInfo(DataRow dr, List<Staff> staffs, List<Company> companies, out string staffId, out string enterpriseId)
        {
            string result = string.Empty;
            staffId = string.Empty;
            enterpriseId = String.Empty;

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

                //姓名
                if (string.IsNullOrWhiteSpace(staff.Name))
                {
                    result += "姓名不能为空! ";
                }

                if (staff.Status == StaffStatus.Departure)
                {
                    result += $"{staff.Name}已离职!";
                }
                else if (staff.Status == StaffStatus.Delete)
                {
                    result += $"{staff.Name}已废弃!";
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

        #endregion
    }
}