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
using Yahv.Utils.Serializers;
using Yahv.Web.Controls.Easyui;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.Erm.WebApp.Erm.Attendance
{
    public partial class List : ErpParticlePage
    {
        #region 常量
        /// <summary>
        /// 考勤扣款
        /// </summary>
        private const string KQ_Column = "考勤扣款";

        /// <summary>
        /// 新基本工资
        /// </summary>
        private const string XJBGZ_Column = "新基本工资";

        /// <summary>
        /// 岗位工资
        /// </summary>
        private const string GWGZ_Column = "岗位工资";

        /// <summary>
        /// 岗位津贴
        /// </summary>
        private const string GWJT_Column = "岗位津贴";

        /// <summary>
        /// 浮动工资
        /// </summary>
        private const string FDGZ_Column = "浮动工资";

        /// <summary>
        /// 特殊岗位ID
        /// </summary>
        private const string GW_Id = "0318465CCB21F214C0440481C8DCDE66";
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
            if (IsClosed(dateIndex))
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

            string fileFullName = UploadFile();     //上传文件

            if (string.IsNullOrWhiteSpace(fileFullName))
            {
                Easyui.Alert("操作提示", "上传文件失败!", Sign.Error);
                return;
            }

            var dt = UploadStaffs.Current.ExcelToTable(fileFullName);

            //判断是否包含关键列
            if (!dt.Columns.Contains("ID") ||
                !dt.Columns.Contains("员工") ||
                !dt.Columns.Contains("迟到") ||
                !dt.Columns.Contains("早退") ||
                !dt.Columns.Contains("旷工") ||
                !dt.Columns.Contains("事假") ||
                !dt.Columns.Contains("病假"))
            {
                Easyui.Alert("操作提示", "导入文件格式不正确!", Sign.Error);
                return;
            }
            var list = GetList(dt);
            list.Enter();

            Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "大赢家考勤导入",
                    $"Excel导入{dateIndex}", string.Empty);

            //Easyui.Alert("操作提示", "操作成功!", Sign.Info);
            Easyui.Dialog.Close("操作成功!", Web.Controls.Easyui.AutoSign.Success);

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
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "大赢家考勤导入",
                    $"保存", list.Json());
                list.Enter();
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
            Expression<Func<StaffPayItem, bool>> predicate = item => item.Name == KQ_Column;

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
        private Dictionary<int, string> CheckData(DataTable dt)
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();

            var staffs = Alls.Current.Staffs.ToList();       //所有员工
            var staffItems = new MyWageItemsRoll().ToList();        //所有员工工资项默认值
            var wageItems = Alls.Current.WageItems.ToList();        //工资项
            var companies = Alls.Current.Companies.Select(t => t.Name).ToList();        //所有内部公司

            string msg = string.Empty;
            string staffId = string.Empty;

            //重复身份证号
            //var repeatIdCards = from idCard in dt.AsEnumerable()
            //                    group idCard by idCard.Field<string>(IdCard_Column) into grp
            //                    where grp.Count() > 1
            //                    select grp.Key;


            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //#region 历史数据导入时验证
                ////判断所属公司是否存在
                //if (String.CompareOrdinal(wageDate.Value.Replace("-", ""), "201907") < 0)
                //{
                //    //判断是否包含所属公司
                //    if (!companies.Contains(dt.Rows[i]["所属公司"]))
                //    {
                //        dic.Add(i + 2, $"【{dt.Rows[i]["所属公司"]}】不存在，请检查公司名称是否一致!");
                //        continue;
                //    }
                //}
                //#endregion

                ////判断是否有重复身份证
                //if (repeatIdCards.Contains(dt.Rows[i][IdCard_Column]))
                //{
                //    dic.Add(i + 2, "身份证号码重复!");
                //    continue;
                //}

                ////员工信息校验
                //msg = CheckStaffInfo(dt.Rows[i], staffs, out staffId);
                //if (!string.IsNullOrWhiteSpace(msg))
                //{
                //    dic.Add(i + 2, msg);
                //    continue;
                //}


                ////工资列数据校验
                //msg = CheckWageItemData(dt.Rows[i], dt.Columns, staffItems.Where(item => item.ID == staffId).ToList(), wageItems);
                //if (!string.IsNullOrWhiteSpace(msg))
                //{
                //    dic.Add(i + 2, msg);
                //    continue;
                //}
            }

            return dic;
        }

        /// <summary>
        /// 获取修改的数据
        /// </summary>
        /// <returns></returns>
        private List<PayItem> GetList(DataTable dt)
        {
            List<PayItem> list = new List<PayItem>();

            if (dt == null || dt.Rows.Count <= 0)
            {
                return list;
            }

            try
            {
                string dateIndex = wageDate.Value.Replace("-", "");
                //只查 新基本工资、岗位津贴、岗位工资、浮动工资
                List<string> itemsName = new List<string>()
                {
                    XJBGZ_Column,
                    GWGZ_Column,
                    FDGZ_Column,
                    GWJT_Column,
                    KQ_Column
                };

                var staffs = Alls.Current.Staffs.Where(item => item.PostionID != GW_Id).Where(item=> item.Status == StaffStatus.Period || item.Status == StaffStatus.Normal || item.Status == StaffStatus.Departure).ToList();       //所有员工(特殊岗位除外)
                var myItems = new MyWageItemsRoll().Where(item => item.WageItemName == KQ_Column).ToList();        //所有员工工资项默认值
                var payBills = Alls.Current.PayBills.Where(item => item.DateIndex == dateIndex).ToList();        //当前月所有工资账单
                var payItems = Alls.Current.PayItems.Where(item => item.DateIndex == dateIndex && itemsName.Contains(item.Name)).ToList();        //当前月所有工资项


                string staffId_temp = string.Empty;
                string payId_temp = string.Empty;


                //遍历循环数据集
                foreach (DataRow dr in dt.Rows)
                {
                    //判断是否需要扣款
                    if (!IsDeductions(dr))
                    {
                        continue;
                    }

                    staffId_temp =
                        staffs.FirstOrDefault(
                            item => item.Name == dr["员工"].ToString() && item.DyjCode == dr["ID"].ToString())?.ID;

                    //员工不存在
                    if (string.IsNullOrWhiteSpace(staffId_temp))
                    {
                        continue;
                    }

                    //判断这个人是否有考勤扣款工资项
                    if (myItems.All(item => item.ID != staffId_temp))
                    {
                        continue;
                    }

                    payId_temp = staffId_temp.Replace("Staff", "") + "-" + dateIndex;

                    //判断是否已经创建工资单
                    if (!payBills.Any(item => item.ID == payId_temp))
                    {
                        continue;
                    }

                    list.Add(new PayItem()
                    {
                        DateIndex = dateIndex,
                        Name = KQ_Column,
                        PayID = payId_temp,
                        StaffID = payItems.Any(item => item.PayID == payId_temp && item.Name == KQ_Column) ? "1" : "0",     //判断添加还是更新
                        Value = GetDeductions(dr, payItems.Where(item => item.PayID == payId_temp && item.Name != KQ_Column).Sum(item => item.Value)),
                        AdminID = Erp.Current.ID,
                    });
                }
            }
            catch (Exception)
            {

                throw;
            }

            return list;
        }

        /// <summary>
        /// 是否扣款
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        private bool IsDeductions(DataRow dr)
        {

            //迟到+早退 超过5次进行扣款
            if (GetValueByRow(dr, "迟到") + GetValueByRow(dr, "早退") > 5)
            {
                return true;
            }

            if (GetValueByRow(dr, "旷工") > 0 || GetValueByRow(dr, "事假") > 0 || GetValueByRow(dr, "病假") > 0)
            {
                return true;
            }


            return false;
        }

        /// <summary>
        /// 获取扣款金额
        /// </summary>
        /// <param name="dr">数据行</param>
        /// <param name="items">员工的工资项数据</param>
        /// <returns></returns>
        private decimal GetDeductions(DataRow dr, decimal wage)
        {
            decimal result = 0;

            //迟到+早退
            //5-10次	（次数-5）*20
            //10次以上	（10-5）*20 +（次数-10）*50
            decimal lateOrEarly = GetValueByRow(dr, "迟到") + GetValueByRow(dr, "早退");

            if (lateOrEarly > 5 && lateOrEarly <= 10)
            {
                result += (lateOrEarly - 5) * 20;
            }
            else if (lateOrEarly > 10)
            {
                result += (lateOrEarly - 10) * 50 + (10 - 5) * 20;
            }

            //日工资：（新基本工资+岗位工资+岗位津贴+浮动工资）/ 21.5
            decimal dailyWage = wage / Convert.ToDecimal("21.5");



            //旷工/事假     日工资*旷工天数
            decimal leave = GetValueByRow(dr, "旷工") + GetValueByRow(dr, "事假");
            if (leave > 0)
            {
                result += leave * dailyWage;
            }

            //病假    日工资*0.5*病假天数
            decimal sickLeave = GetValueByRow(dr, "病假");
            if (sickLeave > 0)
            {
                result += sickLeave * decimal.Parse("0.5") * dailyWage;
            }

            result = Round(result, 2);

            return result;
        }

        /// <summary>
        /// 获取行的值（绝对值）
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private decimal GetValueByRow(DataRow dr, string name)
        {
            try
            {
                return Math.Abs(dr[name].ToString() == "" ? 0 : Convert.ToDecimal(dr[name].ToString()));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 四舍五入
        /// </summary>
        /// <param name="value"></param>
        /// <param name="decimals"></param>
        /// <returns></returns>
        private decimal Round(decimal value, int decimals)
        {
            decimal result = value;

            if (value < 0)
            {
                result = Math.Abs(result);
            }

            result = Math.Round(result, decimals, MidpointRounding.AwayFromZero);

            if (value < 0)
            {
                result = 0 - result;
            }

            return result;
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
        #endregion
    }
}