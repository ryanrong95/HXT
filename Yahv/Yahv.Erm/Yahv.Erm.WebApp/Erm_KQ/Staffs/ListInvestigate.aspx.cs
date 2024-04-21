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
using System.Linq.Expressions;
using Yahv.Erm.Services.Common;

namespace Yahv.Erm.WebApp.Erm_KQ.Staffs
{
    public partial class ListInvestigate : ErpParticlePage
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
            this.Model.ApprovalStatus = ExtendsEnum.ToDictionary<StaffApprovalStatus>()
                .Where(item => item.Key != StaffApprovalStatus.Applied.GetHashCode().ToString())
                .Select(item => new { Value = item.Key, Text = item.Value });
        }

        /// <summary>
        /// 数据查询
        /// </summary>
        /// <returns></returns>
        protected object data()
        {
            Expression<Func<Logs_StaffApproval, bool>> expression = Predicate();
            int page = int.Parse(Request.QueryString["page"]);
            int rows = int.Parse(Request.QueryString["rows"]);

            var staffs = Erp.Current.Erm.Logs_StaffApprovalAll.Where(item => item.ApprovalStep == StaffApprovalStep.BackgroundInvestigate).Where(expression);

            var data = staffs.OrderByDescending(item => item.CreateDate).ToArray().Select(item => new
            {
                ID = item.StaffID,
                Name = item.Staff.Name,
                Code = item.Staff.Code,
                Gender = item.Staff.Gender.GetDescription(),
                Age = item.Staff.Personal.Age,
                Education = item.Staff.Personal.Education,
                GraduatInstitutions = item.Staff.Personal.GraduatInstitutions,
                Mobile = item.Staff.Personal.Mobile,
                Email = item.Staff.Personal.Email,
                Status = item.ApprovalStatus,
                StatusDec = item.ApprovalStatus.GetDescription(),
                CreateDate = item.UpdateDate.ToString("yyyy-MM-dd"),
                AdminName = item.Admin?.RealName,
            });

            return new
            {
                rows = data.Skip((page - 1) * rows).Take(rows),
                total = data.Count(),
            };
        }
        Expression<Func<Logs_StaffApproval, bool>> Predicate()
        {
            Expression<Func<Logs_StaffApproval, bool>> predicate = item => true;

            //查询参数
            var Name = Request.QueryString["Name"];
            var ApprovalStatus = Request.QueryString["ApprovalStatus"];

            if (!string.IsNullOrWhiteSpace(Name))
            {
                predicate = predicate.And(item => item.Staff.Name.Contains(Name) || item.Staff.Code.Contains(Name));
            }
            if (!string.IsNullOrWhiteSpace(ApprovalStatus))
            {
                var status = (StaffApprovalStatus)Enum.Parse(typeof(StaffApprovalStatus), ApprovalStatus);
                predicate = predicate.And(item => item.ApprovalStatus == status);
            }
            return predicate;
        }

        /// <summary>
        /// 调查通过
        /// </summary>
        protected void Pass()
        {
            try
            {
                string id = Request.Form["ID"];
                var staff = Alls.Current.Staffs[id];
                var files = new Services.Views.StaffFileAlls(id).AsEnumerable();
                if (staff != null)
                {
                    string exception = string.Empty;
                    //验证文件：应聘人员登记表、面试情况评估表
                    if (files.Where(item => item.Type == (int)FileType.EmploymentForm).Count() == 0)
                    {
                        exception += "应聘人员登记表,";
                    }
                    if (files.Where(item => item.Type == (int)FileType.InterviewEvaluationForm).Count() == 0)
                    {
                        exception += "面试情况评估表,";
                    }
                    if (files.Where(item => item.Type == (int)FileType.BackgroundInvestigation).Count() == 0)
                    {
                        exception += "员工背景调查报告,";
                    }
                    if (!string.IsNullOrEmpty(exception))
                    {
                        throw new Exception("请上传文件(" + exception + ")");
                    }

                    //调查通过日志
                    var log = new Logs_StaffApproval();
                    log.StaffID = staff.ID;
                    log.ApprovalStep = StaffApprovalStep.BackgroundInvestigate;
                    log.ApprovalStatus = StaffApprovalStatus.Pass;
                    log.AdminID = Erp.Current.ID;
                    log.Enter();

                    //等待审批日志
                    var log2 = new Logs_StaffApproval();
                    log2.StaffID = staff.ID;
                    log2.ApprovalStep = StaffApprovalStep.Manager;
                    log2.ApprovalStatus = StaffApprovalStatus.Waiting;
                    log2.Enter();

                    Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "员工信息",
                        $"面试通过", staff.Json());
                }
                Response.Write((new { success = true, message = "成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 调查未通过
        /// </summary>
        protected void Fail()
        {
            try
            {
                string id = Request.Form["ID"];
                var staff = Alls.Current.Staffs[id];
                var files = new Services.Views.StaffFileAlls(id).AsEnumerable();
                if (staff != null)
                {
                    string exception = string.Empty;
                    //验证文件：员工背景调查报告
                    if (files.Where(item => item.Type == (int)FileType.BackgroundInvestigation).Count() == 0)
                    {
                        exception += "员工背景调查报告,";
                    }
                    if (!string.IsNullOrEmpty(exception))
                    {
                        throw new Exception("请上传文件(" + exception + ")");
                    }

                    var log = new Logs_StaffApproval();
                    log.StaffID = staff.ID;
                    log.ApprovalStep = StaffApprovalStep.BackgroundInvestigate;
                    log.ApprovalStatus = StaffApprovalStatus.Fail;
                    log.AdminID = Erp.Current.ID;
                    log.Enter();
                    Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "员工信息",
                        $"面试未通过", staff.Json());
                }
                Response.Write((new { success = true, message = "成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 员工背景调查报告
        /// </summary>
        protected void ExportBackgroundInvestigation()
        {
            try
            {
                string StaffID = Request.Form["ID"];
                var staff = Alls.Current.Staffs[StaffID];

                var fileName = FileType.BackgroundInvestigation.GetDescription() + "-" + staff.Name + DateTime.Now.Ticks + ".xlsx";
                FileDirectory fileDir = new FileDirectory(fileName, FileType.BackgroundInvestigation);
                fileDir.CreateDirectory();
                string filePath = fileDir.DownLoadRoot + fileName;
                string TempletePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content\\Template\\files\\员工背景调查报告.xlsx");
                staff.ExportBackgroundInvestigation(filePath, TempletePath);

                var fileUrl = @"../../Files/Download/" + fileName;
                Response.Write((new { success = true, message = "导出成功", fileUrl }).Json());
            }
            catch (Exception ex)
            {

                Response.Write((new { success = false, message = "导出失败:" + ex.Message }).Json());
            }
        }

    }
}