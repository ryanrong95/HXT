using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Yahv.Erm.Services;
using Yahv.Erm.Services.Common;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Erm.Services.Views.Rolls;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.Erm.WebApp.Erm_KQ.Application_RewardAndPunish
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadComboBoxData();
                LoadData();
            }
        }

        protected void LoadComboBoxData()
        {
            var staffs = Erp.Current.Erm.XdtStaffs
                .Where(item => item.Status == StaffStatus.Normal || item.Status == StaffStatus.Period);
            var staff = staffs.SingleOrDefault(item => item.ID == Erp.Current.StaffID);
            if (staff != null && staff.PostionCode != ((int)Services.Common.PostType.President).ToString())
            {
                staffs = staffs.Where(item => item.DepartmentCode == staff.DepartmentCode);
            }
            //员工
            this.Model.StaffData = staffs.Select(item => new
            {
                Value = item.Admin.ID,
                Text = item.Name,
            });
            //员工岗位
            this.Model.PositionData = Erp.Current.Erm.XdtPostions.Select(item => new
            {
                Value = item.ID,
                Text = item.Name,
            });
            //部门类型
            this.Model.DepartmentType = ExtendsEnum.ToDictionary<DepartmentType>().Select(item => new { Value = item.Key, Text = item.Value });
            //奖惩类别
            this.Model.RewardOrPunish = ExtendsEnum.ToDictionary<RewardOrPunish>().Select(item => new { Value = item.Value, Text = item.Value });
            //奖励形式
            this.Model.RewardType = ExtendsEnum.ToDictionary<RewardType>().Select(item => new { Value = item.Value, Text = item.Value });
            //惩罚形式
            this.Model.PunishType = ExtendsEnum.ToDictionary<PunishType>().Select(item => new { Value = item.Value, Text = item.Value });
        }

        protected void LoadData()
        {
            //申请信息
            string ApplicationID = Request.QueryString["ID"];
            this.Model.ApplicationData = null;
            if (!string.IsNullOrEmpty(ApplicationID))
            {
                var application = Erp.Current.Erm.Applications.Single(item => item.ID == ApplicationID);
                this.Model.ApplicationData = new
                {
                    ApplicantID = application.ApplicantID,
                    StaffID = application.RewardAndPunishContext.StaffID,
                    RewardOrPunish = application.RewardAndPunishContext.RewardOrPunish,
                    RewardType = application.RewardAndPunishContext.RewardType,
                    RewardDec = application.RewardAndPunishContext.RewardDec,
                    PunishType = application.RewardAndPunishContext.PunishType,
                    PunishDec = application.RewardAndPunishContext.PunishDec,
                    Reason = application.RewardAndPunishContext.Reason,
                };
            }
        }

        protected void StaffChange()
        {
            try
            {
                var staffs = Erp.Current.Erm.XdtStaffs
                    .Where(item => item.Status == StaffStatus.Normal || item.Status == StaffStatus.Period);
                //员工姓名
                string Name = Request.Form["Name"];
                var staff = staffs.Single(item => item.Admin.ID == Name);
                var data = new
                {
                    department = (int)((DepartmentType)Enum.Parse(typeof(DepartmentType), staff.DepartmentCode)),
                    staff.PostionID,
                };
                Response.Write((new { success = true, message = "保存成功", data = data }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        protected void UploadFile()
        {
            try
            {
                List<dynamic> fileList = new List<dynamic>();
                IList<HttpPostedFile> files = HttpContext.Current.Request.Files.GetMultiple("uploadFile");
                if (files.Count > 0)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        //处理附件
                        HttpPostedFile file = files[i];
                        if (file.ContentLength != 0)
                        {
                            FileDirectory dic = new FileDirectory(file.FileName, FileType.RewardAndPunishApplication);
                            dic.AdminID = Erp.Current.ID;
                            dic.Save(file);
                            fileList.Add(new
                            {
                                ID = dic.uploadResult.FileID,
                                CustomName = dic.FileName,
                                FileName = dic.uploadResult.FileName,
                                FileType = dic.FileType,
                                FileTypeDec = dic.FileType.GetDescription(),
                                Url = dic.uploadResult.Url,
                            });
                        }
                    }
                }
                Response.Write((new { success = true, data = fileList }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导入失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 加载文件
        /// </summary>
        /// <returns></returns>
        protected object LoadFile()
        {
            string ApplicationID = Request.QueryString["ID"];
            var files = new Services.Views.ApplicationFileAlls(ApplicationID).AsEnumerable();
            var linq = files.Select(t => new
            {
                ID = t.ID,
                CustomName = t.CustomName,
                FileType = ((FileType)Enum.Parse(typeof(FileType), t.Type.ToString())).GetDescription(),
                CreateDate = t.CreateDate?.ToString("yyyy-MM-dd"),
                Creater = t.AdminID,
                Url = FileDirectory.ServiceRoot + t.Url,
            });
            return linq;
        }

        /// <summary>
        /// 审批日志
        /// </summary>
        /// <returns></returns>
        protected object LoadLogs()
        {
            var id = Request.QueryString["ID"];
            var logs = Erp.Current.Erm.Logs_ApplyVoteSteps.Where(item => item.ApplicationID == id);

            var data = logs.OrderByDescending(item => item.CreateDate).ToArray().Select(item => new
            {
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                AdminID = item.Admin.RealName,
                Status = item.Status.GetDescription(),
                Summary = item.Summary,
                item.VoteStepName,
            });
            return new
            {
                rows = data,
                total = data.Count(),
            };
        }

        /// <summary>
        /// 提交
        /// </summary>
        protected void Submit()
        {
            try
            {
                #region 界面数据
                string id = Request.Form["ID"];
                string Applicant = Request.Form["Applicant"];
                string Staff = Request.Form["Staff"];
                string Department = Request.Form["Department"];
                string Position = Request.Form["Position"];
                string PositionName = Request.Form["PositionName"];
                string StaffName = Request.Form["StaffName"];

                string RewardOrPunish = Request.Form["RewardOrPunish"];
                string RewardType = Request.Form["RewardType"];
                string PunishType = Request.Form["PunishType"];
                string RewardDec = Request.Form["RewardDec"];
                string PunishDec = Request.Form["PunishDec"];

                string Reason = Request.Form["Reason"];
                //文件信息
                var files = Request.Form["files"].Replace("&quot;", "'").Replace("amp;", "");
                var fileList = files.JsonTo<List<Yahv.Services.Models.CenterFileDescription>>();
                #endregion

                Application application = Erp.Current.Erm.Applications[id] ?? new Application();
                application.Title = "员工奖惩申请";
                application.Context = new RewardAndPunishContext()
                {
                    StaffID = Staff,
                    StaffName = StaffName,
                    Department = Department,
                    PositionID = Position,
                    PositionName = PositionName,
                    RewardOrPunish = RewardOrPunish,
                    RewardType = RewardType,
                    PunishType = PunishType,
                    RewardDec = RewardDec,
                    PunishDec = PunishDec,
                    Reason = Reason,
                }.Json();
                application.ApplicantID = Applicant;
                application.CreatorID = Erp.Current.ID;
                application.ApplicationStatus = Services.ApplicationStatus.UnderApproval;
                application.Fileitems = fileList;
                application.ApplicationType = Services.ApplicationType.RewardAndPunish;

                application.Enter();

                //申请人
                var staff = Alls.Current.Staffs.Single(item => item.Admin.ID == Applicant);
                //如果是总经理则直接到行政部执行
                if (staff.PostionCode == PostType.President.GetHashCode().ToString())
                {
                    var apply = Erp.Current.Erm.ApplicationsRoll[application.ID];
                    apply.Approval(true);
                    apply.Approval(true);
                }
                //如果是行政部提交则直接到总经理审批
                if (staff.PostionCode == PostType.Manager.GetHashCode().ToString() && staff.DepartmentCode == DepartmentType.行政部.GetHashCode().ToString())
                {
                    var apply = Erp.Current.Erm.ApplicationsRoll[application.ID];
                    apply.Approval(true);
                }

                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new
                {
                    success = false,
                    message = "保存失败：" + ex.Message
                }).Json());
            }
        }

        /// <summary>
        /// 保存草稿
        /// </summary>
        protected void SaveDraft()
        {
            try
            {
                #region 界面数据
                string Applicant = Request.Form["Applicant"];
                string Staff = Request.Form["Staff"];
                string Department = Request.Form["Department"];
                string Position = Request.Form["Position"];
                string PositionName = Request.Form["PositionName"];
                string StaffName = Request.Form["StaffName"];

                string RewardOrPunish = Request.Form["RewardOrPunish"];
                string RewardType = Request.Form["RewardType"];
                string PunishType = Request.Form["PunishType"];
                string RewardDec = Request.Form["RewardDec"];
                string PunishDec = Request.Form["PunishDec"];

                string Reason = Request.Form["Reason"];
                //文件信息
                var files = Request.Form["files"].Replace("&quot;", "'").Replace("amp;", "");
                var fileList = files.JsonTo<List<Yahv.Services.Models.CenterFileDescription>>();
                #endregion

                var staff = Alls.Current.Staffs.Single(item => item.Admin.ID == Applicant);

                Application application = new Application();
                application.Title = "员工奖惩申请";
                application.Context = new RewardAndPunishContext()
                {
                    StaffID = Staff,
                    StaffName = StaffName,
                    Department = Department,
                    PositionID = Position,
                    PositionName = PositionName,
                    RewardOrPunish = RewardOrPunish,
                    RewardType = RewardType,
                    PunishType = PunishType,
                    RewardDec = RewardDec,
                    PunishDec = PunishDec,
                    Reason = Reason,
                }.Json();
                application.ApplicantID = Applicant;
                application.CreatorID = Erp.Current.ID;
                application.ApplicationStatus = Services.ApplicationStatus.Draft;
                application.Fileitems = fileList;
                application.ApplicationType = Services.ApplicationType.RewardAndPunish;

                application.Enter();

                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }

    }
}