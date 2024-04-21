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

namespace Yahv.Erm.WebApp.Erm_KQ.Application_ExternalTraining
{
    public partial class ApprovalStep : ErpParticlePage
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
                .Where(item => item.Status == StaffStatus.Normal || item.Status == StaffStatus.Period).ToArray();
            //员工
            this.Model.StaffData = staffs.OrderBy(item=>item.DepartmentCode).Select(item => new
            {
                Value = item.Admin.ID,
                Text = item.Name,
                SelCode = item.SelCode,
                DepartmentCode = string.IsNullOrEmpty(item.DepartmentCode) ? "" : ((DepartmentType)Enum.Parse(typeof(DepartmentType), item.DepartmentCode)).GetDescription(),
                PostionName = item.Postion?.Name,
                EntryDate = item.Labour?.EntryDate.ToString("yyyy-MM-dd"),
            });
            //负责人员
            this.Model.ManageData = staffs.Where(item => item.PostionCode == ((int)PostType.Manager).ToString() || item.PostionCode == ((int)PostType.President).ToString())
                .Select(item => new
                {
                    Value = item.Admin.ID,
                    Text = item.Name,
                });
            //部门类型
            this.Model.DepartmentType = ExtendsEnum.ToDictionary<DepartmentType>().Select(item => new { Value = item.Key, Text = item.Value });
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
                    TrainingTime = application.ExternalTrainingContext.TrainingTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    TrainingLocation = application.ExternalTrainingContext.TrainingLocation,
                    Trainees = application.ExternalTrainingContext.Trainees,
                    CourseName = application.ExternalTrainingContext.CourseName,
                    Tuition = application.ExternalTrainingContext.Tuition,
                    TravelExpenses = application.ExternalTrainingContext.TravelExpenses,
                    InstitutionName = application.ExternalTrainingContext.InstitutionName,
                    CourseValue = application.ExternalTrainingContext.CourseValue,
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
                //唯一负责人
                var manager = staffs.Where(item => item.DepartmentCode == staff.DepartmentCode)
                    .Where(item => item.PostionCode == ((int)PostType.Manager).ToString() || item.PostionCode == ((int)PostType.President).ToString()).FirstOrDefault();
                var data = new
                {
                    department = (int)((DepartmentType)Enum.Parse(typeof(DepartmentType), staff.DepartmentCode)),
                    manager = manager?.Admin.ID,
                    selcode = staff.SelCode,
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
                            FileDirectory dic = new FileDirectory(file.FileName, FileType.ExternalTrainingApplication);
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
        /// 通过
        /// </summary>
        protected void Submit()
        {
            try
            {
                #region 界面数据
                string id = Request.Form["ID"];
                string Argument = Request.Form["Argument"];
                //文件信息
                var files = Request.Form["files"].Replace("&quot;", "'").Replace("amp;", "");
                var fileList = files.JsonTo<List<Yahv.Services.Models.CenterFileDescription>>();
                #endregion

                Application application = Erp.Current.Erm.ApplicationsRoll[id];
                application.Fileitems = fileList;
                application.Enter();

                //审批通过
                application.CurrentVoteStep.Status = Services.ApprovalStatus.Agree;
                application.CurrentVoteStep.Summary = Argument;
                application.Approval(true);
                Response.Write((new { success = true, message = "审批成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "审批失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 驳回
        /// </summary>
        protected void Reject()
        {
            try
            {
                #region 界面数据
                string id = Request.Form["ID"];
                string Argument = Request.Form["Argument"];
                #endregion

                Application application = Erp.Current.Erm.ApplicationsRoll[id];
                //审批驳回
                application.CurrentVoteStep.Status = Services.ApprovalStatus.Reject;
                application.CurrentVoteStep.Summary = Argument;
                application.Approval(false);
                Response.Write((new { success = true, message = "驳回成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "驳回失败：" + ex.Message }).Json());
            }
        }

    }
}