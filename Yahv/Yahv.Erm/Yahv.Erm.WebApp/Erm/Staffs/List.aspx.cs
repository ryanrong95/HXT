#if true
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using Yahv.Erm.Services;
using Yahv.Web.Erp;
using Yahv.Underly;
using Yahv.Linq.Extends;
using Yahv.Web.Controls.Easyui;
using Yahv.Web.Forms;
using Yahv.Erm.Services.Models.Origins;
using Layers.Data;
using Yahv.Utils;
using System.Data;
using Yahv.Utils.Serializers;
using PKeyType = Yahv.Erm.Services.PKeyType;

namespace Yahv.Erm.WebApp.Erm.Staffs
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadComboBoxData();
            }
        }

        protected void LoadComboBoxData()
        {
            this.Model.PostionData = Alls.Current.Postions.Select(item => new { Value = item.ID, Text = item.Name });
            this.Model.CompaniesData = Alls.Current.Companies.Select(item => new { Value = item.ID, Text = item.Name });

            string[] enums = new string[]
            {
               ((int)StaffStatus.Period).ToString(),
               ((int)StaffStatus.Normal).ToString(),
               ((int)StaffStatus.Departure).ToString(),
               ((int)StaffStatus.Cancel).ToString(),
            };
            this.Model.Status = ExtendsEnum.ToDictionary<Services.StaffStatus>().Where(item => enums.Contains(item.Key))
                .Select(item => new { Value = item.Key, Text = item.Value });
        }

        /// <summary>
        /// 数据查询
        /// </summary>
        /// <returns></returns>
        protected object data()
        {
            var name = Request.QueryString["Name"];
            var PostionID = Request.QueryString["PostionID"];
            var Status = Request.QueryString["Status"];
            var CompanyID = Request.QueryString["CompanyID"];

            var staffs = Alls.Current.Staffs.Where(item => item.Status == StaffStatus.Period
             || item.Status == StaffStatus.Normal
             || item.Status == StaffStatus.Departure
             || item.Status == StaffStatus.Cancel);

            if (!string.IsNullOrWhiteSpace(name))
            {
                staffs = staffs.Where(item => item.Name.Contains(name) || item.Code.Contains(name) || item.DyjCode == name);
            }
            if (!string.IsNullOrWhiteSpace(PostionID))
            {
                staffs = staffs.Where(item => item.PostionID == PostionID);
            }
            if (!string.IsNullOrWhiteSpace(Status))
            {
                staffs = staffs.Where(item => item.Status.ToString() == Status);
            }
            if (!string.IsNullOrEmpty(CompanyID))
            {
                staffs = staffs.Where(item => item.EnterpriseID == CompanyID);
            }

            var data = staffs.OrderByDescending(item => item.CreateDate).ToArray().Select(item => new
            {
                item.ID,
                item.Name,
                item.Code,
                Gender = item.Gender.GetDescription(),
                WorkCity = item.CityName,
                PostionID = item.PostionName,
                Status = item.Status.GetDescription(),
                item.EnterpriseID,
                EnterpriseCompany = item.EnterpriseCompany,
                //EntryDate = item.EntryDate?.ToString("yyyy-MM-dd"),
                LeaveDate = item.LeaveDate?.ToString("yyyy-MM-dd"),
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
            });

            int page = int.Parse(Request.QueryString["page"]);
            int rows = int.Parse(Request.QueryString["rows"]);
            return new
            {
                rows = data.Skip((page - 1) * rows).Take(rows),
                total = data.Count(),
            };
        }
        /// <summary>
        /// 删除
        /// </summary>
        protected void Delete()
        {
            string id = Request.Form["ID"];
            var del = Alls.Current.Staffs[id];
            if (del != null)
            {
                del.Cancel();
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "员工信息",
                    $"删除", del.Json());
            }
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnImport_Click(object sender, EventArgs e)
        {
            string fileFullName = UploadFile();     //上传文件

            if (string.IsNullOrWhiteSpace(fileFullName))
            {
                Easyui.Alert("操作提示", "上传文件失败!", Sign.Error);
                return;
            }

            var dt = UploadStaffs.Current.ExcelToTable(fileFullName);
            Dictionary<int, string> errorMsgs = CheckData(dt);
            if (errorMsgs == null)
            {
                return;
            }
            //导出错误excel
            if (errorMsgs.Count > 0)
            {
                DownLoadFile(ExportWages.Current.MakeErrorExcel(fileFullName, errorMsgs));
            }
            else
            {
                int count = 0;
                int nub = 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    if (dt.Columns.Contains("状态"))
                    {
                        if (!string.IsNullOrEmpty(dt.Rows[i]["状态"].ToString()))
                        {
                            if (dt.Rows[i]["状态"].ToString() == "在职")
                            {
                                var staff = new Staff() as Staff;
                                var tempID = PKeySigner.Pick(PKeyType.Staff);
                                staff.ID = tempID;
                                staff.SelCode = tempID.Replace("Staff", "");
                                staff.Code = tempID.Replace("Staff", "");
                                staff.RoleID = "NRole000";
                                staff.Password = "123456";
                                staff.AdminID = Erp.Current.ID;
                                staff.Status = StaffStatus.Normal;
                                var labour = new Labour();
                                labour.ID = tempID;
                                var bankcard = new BankCard();
                                bankcard.ID = tempID;
                                if (dt.Columns.Contains("地区"))
                                {
                                    staff.WorkCity = Alls.Current.LeaguesRolls.Where(item => item.Category == Category.StaffInCity && item.Name == dt.Rows[i]["地区"].ToString()).FirstOrDefault().ID;
                                    if (staff.WorkCity == null)
                                    {
                                        continue;
                                    }
                                }
                                if (dt.Columns.Contains("所属公司"))
                                {
                                    labour.EntryCompany = dt.Rows[i]["所属公司"].ToString();
                                    labour.EnterpriseID = Alls.Current.Companies.Where(item => item.Name == dt.Rows[i]["所属公司"].ToString()).FirstOrDefault().ID;
                                    if (labour.EnterpriseID == null)
                                    {
                                        continue;
                                    }
                                }
                                if (dt.Columns.Contains("所属银行"))
                                {
                                    var bankName = dt.Rows[i]["所属银行"].ToString();

                                    if (!string.IsNullOrWhiteSpace(bankName))
                                    {
                                        if (ExtendsEnum.ToDictionary<Services.Bank>().Any(item => item.Value == dt.Rows[i]["所属银行"].ToString()))
                                            bankcard.Bank = ExtendsEnum.ToDictionary<Services.Bank>().Where(item => item.Value == dt.Rows[i]["所属银行"].ToString()).FirstOrDefault().Value;
                                    }
                                }
                                if (dt.Columns.Contains("入职时间"))
                                {
                                    if (!string.IsNullOrEmpty(dt.Rows[i]["入职时间"].ToString()))
                                    {
                                        labour.EntryDate = Convert.ToDateTime(dt.Rows[i]["入职时间"].ToString().Replace("/", "-"));
                                    }
                                }
                                if (dt.Columns.Contains("分公司"))
                                {
                                    staff.DyjCompanyCode = dt.Rows[i]["分公司"].ToString();
                                }
                                if (dt.Columns.Contains("手机号"))
                                {
                                    staff.Mobile = dt.Rows[i]["手机号"].ToString();
                                }
                                if (dt.Columns.Contains("部门"))
                                {
                                    staff.DyjDepartmentCode = dt.Rows[i]["部门"].ToString();
                                }
                                if (dt.Columns.Contains("ID（工号）"))
                                {
                                    staff.DyjCode = dt.Rows[i]["ID（工号）"].ToString();
                                }
                                if (dt.Columns.Contains("姓名"))
                                {
                                    staff.Name = dt.Rows[i]["姓名"].ToString();
                                    if (!string.IsNullOrEmpty(staff.Name))
                                    {
                                        staff.UserName = PinyinHelper.GetPinyin(staff.Name);
                                        int num = 1;
                                        while (Alls.Current.Admins.Where(item => item.UserName == staff.UserName).Count() >= 1)
                                        {
                                            if (num == 1)
                                            {
                                                staff.UserName = staff.UserName + num.ToString();
                                            }
                                            else
                                            {
                                                staff.UserName = staff.UserName.Replace((num - 1).ToString(), num.ToString());
                                            }
                                            num++;
                                        }
                                    }
                                }
                                if (dt.Columns.Contains("考核岗位"))
                                {
                                    staff.PostionID = Alls.Current.Postions.Where(item => item.Name == dt.Rows[i]["考核岗位"].ToString()).FirstOrDefault().ID;
                                    if (staff.PostionID == null)
                                    {
                                        continue;
                                    }
                                }
                                if (dt.Columns.Contains("银行卡号"))
                                {
                                    bankcard.Account = dt.Rows[i]["银行卡号"].ToString();
                                }
                                if (dt.Columns.Contains("性别"))
                                {
                                    if (!string.IsNullOrEmpty(dt.Rows[i]["性别"].ToString()))
                                    {
                                        if (dt.Rows[i]["性别"].ToString() == "男")
                                        {
                                            staff.Gender = Gender.Male;
                                        }
                                        else
                                        {
                                            staff.Gender = Gender.Female;
                                        }

                                    }
                                }
                                if (dt.Columns.Contains("身份证号码"))
                                {
                                    var newIDCard = dt.Rows[i]["身份证号码"].ToString();
                                    if (Alls.Current.Personals.Where(item => item.ID != tempID).Where(item => item.IDCard == newIDCard).Count() >= 1)
                                    {
                                        continue;
                                    }
                                    staff.IDCard = newIDCard;
                                }
                                if (staff.WorkCity == null || labour.EnterpriseID == null || staff.PostionID == null || staff.IDCard == null)
                                {
                                    continue;
                                }
                                var personal = new Personal() as Personal;
                                personal.ID = tempID;
                                personal.Mobile = staff.Mobile;
                                personal.IDCard = staff.IDCard;
                                var admin = new Admin() as Admin;
                                admin.RoleID = staff.RoleID;
                                admin.StaffID = tempID;
                                admin.RealName = staff.Name;
                                admin.UserName = staff.UserName;
                                admin.Password = staff.Password;
                                staff.Enter();
                                labour.Enter();
                                bankcard.Enter();
                                personal.Enter();
                                admin.Enter();
                                Alls.Current.MyWageItems.InitWageItems(staff.ID, staff.PostionID);      //根据岗位初始化默认工资项
                                count++;
                            }
                            else if (dt.Rows[i]["状态"].ToString() == "废弃")
                            {
                                if (dt.Columns.Contains("身份证号码"))
                                {
                                    var newIDCard = dt.Rows[i]["身份证号码"].ToString();
                                    if (Alls.Current.Personals.Where(item => item.IDCard == newIDCard).Count() == 1)
                                    {
                                        var id = Alls.Current.Personals.Where(item => item.IDCard == newIDCard).FirstOrDefault().ID;
                                        var del = Alls.Current.Staffs[id];
                                        if (del != null)
                                        {
                                            del.Abandon();
                                            nub++;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (dt.Columns.Contains("身份证号码"))
                                {
                                    var newIDCard = dt.Rows[i]["身份证号码"].ToString();
                                    if (Alls.Current.Personals.Where(item => item.IDCard == newIDCard).Count() == 1)
                                    {
                                        if (!string.IsNullOrEmpty(dt.Rows[i]["离职时间"].ToString()))
                                        {
                                            var date = Convert.ToDateTime(dt.Rows[i]["离职时间"].ToString().Replace("/", "-"));
                                            var staffid = Alls.Current.Personals.Where(item => item.IDCard == newIDCard).FirstOrDefault().ID;
                                            var del = Alls.Current.Staffs[staffid];
                                            if (del != null)
                                            {
                                                del.Departure(date);
                                                nub++;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "员工信息",
                    $"Excel导入", string.Empty);
                Easyui.Alert("操作提示", "上传文件成功，录入了" + count + "条数据，更改了" + nub + "条数据", Sign.Info);
                return;
            }

        }

        /// <summary>
        /// 下载模板
        /// </summary>
        protected void btnDownload_Click(object sender, EventArgs e)
        {
            //DownLoadFile("D:\\Projects_vs2015\\Yahv\\Yahv.Erm\\Yahv.Erm.WebApp\\Upload\\Template\\employeeTemp.xls");
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Upload\") + "Template\\employeeTemp.xls";
            DownLoadFile(fileName);
        }


        /// <summary>
        /// 导出Excel
        /// </summary>
        protected void btnExport_Click(object sender, EventArgs e)
        {
            var staffs = Alls.Current.Staffs.ToArray();
            if (staffs.Length <= 0) return;
            var fileName = ExportWages.Current.MakeStaffsExcel(DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls", staffs);

            Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "员工信息",
                    $"Excel导出", string.Empty);

            //下载文件
            DownLoadFile(fileName);
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

        private Dictionary<int, string> CheckData(DataTable dt)
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();

            //重复身份证号
            var repeatIdCards = from idCard in dt.AsEnumerable()
                                group idCard by idCard.Field<string>("身份证号码") into grp
                                where grp.Count() > 1
                                select grp.Key;

            var validColums = new List<string> { "地区", "所属公司", "分公司", "部门", "ID（工号）", "姓名", "考核岗位", "身份证号码", "性别", "入职时间", "手机号", "所属银行", "银行卡号", "离职时间", "状态" };
            var columns = dt.Columns;       //DataTable所有列
            var bank = ExtendsEnum.ToDictionary<Services.Bank>().Select(item => item.Value).ToList();
            var areas = Alls.Current.LeaguesRolls.Where(item => item.Category == Category.StaffInCity).Select(item => item.Name).ToList();        //所有地区
            var companies = Alls.Current.Companies.Select(item => item.Name).ToList();        //所有内部公司
            var positions = Alls.Current.Postions.Select(item => item.Name).ToList();
            var idcard = Alls.Current.Mypersonals.Select(item => item.IDCard).ToList();
            bool judge = true;
            //考核excel表格式是否正确
            foreach (var column in validColums)
            {
                if (!columns.Contains(column.ToString()))
                {
                    judge = false;
                }
            }
            if (!judge)
            {
                Easyui.Alert("操作提示", "excel文件格式错误!", Sign.Error);
                return null;
            }

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //判断是否有重复身份证
                if (repeatIdCards.Contains(dt.Rows[i]["身份证号码"]))
                {
                    dic.Add(i + 1, "身份证号码重复!");
                    continue;
                }

                if (dt.Rows[i]["状态"].ToString() == "在职")
                {
                    //地区是否存在
                    //if (!bank.Contains(dt.Rows[i]["所属银行"]))
                    //{
                    //    dic.Add(i + 1, "所属银行不存在!");
                    //    continue;
                    //}


                    //判断人员是否存在
                    if (idcard.Contains(dt.Rows[i]["身份证号码"]))
                    {
                        dic.Add(i + 1, "该人员已存在，请勿重复录入!");
                        continue;
                    }

                    //地区是否存在
                    if (!areas.Contains(dt.Rows[i]["地区"]))
                    {
                        dic.Add(i + 1, "地区不存在!");
                        continue;
                    }

                    //所属公司是否存在
                    if (!companies.Contains(dt.Rows[i]["所属公司"]))
                    {
                        dic.Add(i + 1, "所属公司不存在!");
                        continue;
                    }

                    //考核岗位是否存在
                    if (!positions.Contains(dt.Rows[i]["考核岗位"]))
                    {
                        dic.Add(i + 1, "考核岗位不存在!");
                        continue;
                    }
                }
                else if (dt.Rows[i]["状态"].ToString() == "离职" || dt.Rows[i]["状态"].ToString() == "废弃")
                {
                    //判断人员是否存在
                    if (!idcard.Contains(dt.Rows[i]["身份证号码"]))
                    {
                        dic.Add(i + 1, "不存在该人员，请核实后录入!");
                        continue;
                    }
                    if (dt.Rows[i]["状态"].ToString() == "离职")
                    {
                        if (string.IsNullOrEmpty(dt.Rows[i]["离职时间"].ToString()))
                        {
                            dic.Add(i + 1, "离职时间不可为空!");
                            continue;
                        }
                    }

                }
                else
                {
                    dic.Add(i + 1, "状态错误!");
                    continue;
                }

            }

            return dic;
        }
    }
}
#endif