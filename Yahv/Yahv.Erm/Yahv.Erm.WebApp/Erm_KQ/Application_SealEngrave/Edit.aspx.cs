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

namespace Yahv.Erm.WebApp.Erm_KQ.Application_SealEngrave
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
            //员工
            this.Model.StaffData = staffs.Select(item => new
            {
                Value = item.Admin.ID,
                Text = item.Name,
            });
            //负责人员
            this.Model.ManageData = staffs.Where(item => item.PostionCode == ((int)PostType.Manager).ToString() || item.PostionCode == ((int)PostType.President).ToString())
                .Select(item => new
                {
                    Value = item.Admin.ID,
                    Text = item.Name
                });
            //部门类型
            this.Model.DepartmentType = ExtendsEnum.ToDictionary<DepartmentType>().Select(item => new { Value = item.Key, Text = item.Value });
            //印章形状
            this.Model.SealShape = ExtendsEnum.ToDictionary<SealShape>().Select(item => new { Value = item.Value, Text = item.Value });
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
                    SealName = application.SealEngraveContext.SealName,
                    SealShape = application.SealEngraveContext.SealShape,
                    SealShapeDec = application.SealEngraveContext.SealShapeDec,
                    SealSize = application.SealEngraveContext.SealSize,
                    Reason = application.SealEngraveContext.Reason,
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
                            FileDirectory dic = new FileDirectory(file.FileName, FileType.SealEngraveApplication);
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
                string Staff = Request.Form["Staff"];
                string Manager = Request.Form["Manager"];
                string ManagerName = Request.Form["ManagerName"];
                string SealName = Request.Form["SealName"];
                string SealShape = Request.Form["SealShape"];
                string SealShapeDec = Request.Form["SealShapeDec"];
                string SealSize = Request.Form["SealSize"];
                string Reason = Request.Form["Reason"];
                //文件信息
                var files = Request.Form["files"].Replace("&quot;", "'").Replace("amp;", "");
                var fileList = files.JsonTo<List<Yahv.Services.Models.CenterFileDescription>>();
                #endregion

                var staff = Alls.Current.Staffs.Single(item => item.Admin.ID == Staff);

                Application application = Erp.Current.Erm.Applications[id] ?? new Application();
                application.Title = "印章刻制申请";
                application.Context = new SealEngraveContext()
                {
                    ApproveID = Manager,
                    ApproveName = ManagerName,
                    DepartmentCode = staff.DepartmentCode,
                    SealName = SealName,
                    SealShape = SealShape,
                    SealShapeDec = SealShapeDec,
                    SealSize = SealSize,
                    Reason = Reason,
                }.Json();
                application.ApplicantID = Staff;
                application.CreatorID = Erp.Current.ID;
                application.ApplicationStatus = Services.ApplicationStatus.UnderApproval;
                application.Fileitems = fileList;
                application.ApplicationType = Services.ApplicationType.SealEngrave;

                application.Enter();

                //如果是总经理则直接到行政安排刻制
                if (staff.PostionCode == ((int)Services.Common.PostType.President).ToString())
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
                string Staff = Request.Form["Staff"];
                string Manager = Request.Form["Manager"];
                string ManagerName = Request.Form["ManagerName"];
                string SealName = Request.Form["SealName"];
                string SealShape = Request.Form["SealShape"];
                string SealShapeDec = Request.Form["SealShapeDec"];
                string SealSize = Request.Form["SealSize"];
                string Reason = Request.Form["Reason"];
                //文件信息
                var files = Request.Form["files"].Replace("&quot;", "'").Replace("amp;", "");
                var fileList = files.JsonTo<List<Yahv.Services.Models.CenterFileDescription>>();
                #endregion

                var staff = Alls.Current.Staffs.Single(item => item.Admin.ID == Staff);

                Application application = new Application();
                application.Title = "印章刻制申请";
                application.Context = new SealEngraveContext()
                {
                    ApproveID = Manager,
                    ApproveName = ManagerName,
                    DepartmentCode = staff.DepartmentCode,
                    SealName = SealName,
                    SealShape = SealShape,
                    SealShapeDec = SealShapeDec,
                    SealSize = SealSize,
                    Reason = Reason,
                }.Json();
                application.ApplicantID = Staff;
                application.CreatorID = Erp.Current.ID;
                application.ApplicationStatus = Services.ApplicationStatus.Draft;
                application.Fileitems = fileList;
                application.ApplicationType = Services.ApplicationType.SealEngrave;

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