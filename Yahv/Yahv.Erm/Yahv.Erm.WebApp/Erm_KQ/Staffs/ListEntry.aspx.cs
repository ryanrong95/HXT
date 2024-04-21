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
    public partial class ListEntry : ErpParticlePage
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
            this.Model.ApprovalStatus = ExtendsEnum.ToDictionary<StaffEntryReportStatus>()
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

            var staffs = Erp.Current.Erm.Logs_StaffApprovalAll.Where(item => item.ApprovalStep == StaffApprovalStep.Entry).Where(expression);

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
                Status = item.EntryReportStatus,
                StatusDec = item.EntryReportStatus.GetDescription(),
                CreateDate = item.UpdateDate.ToString("yyyy-MM-dd"),
                ReportDate = item.Logs_StaffApprovalContext.ReportDate,
                AdminName = item.Admin?.RealName,
                Summary = item.Summary,
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
                var status = (StaffEntryReportStatus)Enum.Parse(typeof(StaffEntryReportStatus), ApprovalStatus);
                predicate = predicate.And(item => item.EntryReportStatus == status);
            }
            return predicate;
        }

        /// <summary>
        /// 申请入职
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
                    if (files.Where(item => item.Type == (int)FileType.InductionRegistrationForm).Count() == 0)
                    {
                        exception += "入职登记表,";
                    }
                    if (!string.IsNullOrEmpty(exception))
                    {
                        throw new Exception("请上传文件(" + exception + ")");
                    }

                    //提交入职申请
                    staff.Apply(Erp.Current.ID);
                    //更新入职日志
                    var log = Erp.Current.Erm.Logs_StaffApprovalAll.Single(item => item.ApprovalStep == StaffApprovalStep.Entry && item.StaffID == staff.ID);
                    log.EntryReportStatus = StaffEntryReportStatus.Applied;
                    log.AdminID = Erp.Current.ID;
                    log.Enter();
                }
                Response.Write((new { success = true, message = "成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 员工入职登记表
        /// </summary>
        protected void ExportInductionRegistration()
        {
            try
            {
                string StaffID = Request.Form["ID"];
                var staff = Alls.Current.Staffs[StaffID];

                var fileName = FileType.InductionRegistrationForm.GetDescription() + "-" + staff.Name + DateTime.Now.Ticks + ".xlsx";
                FileDirectory fileDir = new FileDirectory(fileName, FileType.InductionRegistrationForm);
                fileDir.CreateDirectory();
                string filePath = fileDir.DownLoadRoot + fileName;
                string TempletePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content\\Template\\files\\入职登记表.xlsx");
                staff.ExportInductionRegistration(filePath, TempletePath);

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