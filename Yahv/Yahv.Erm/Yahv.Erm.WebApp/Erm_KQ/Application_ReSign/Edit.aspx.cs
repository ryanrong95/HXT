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

namespace Yahv.Erm.WebApp.Erm_KQ.Application_ReSign
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
                    Date = application.ReSignContext.Date,
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
                    code = staff?.Code,
                };
                Response.Write((new { success = true, message = "保存成功", data = data }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }

        protected void DateChange()
        {
            try
            {
                //员工ID
                string AdminID = Request.Form["Name"];
                DateTime Date = Convert.ToDateTime(Request.Form["Date"]);
                var result = AttendHelper.GetReSignContext(AdminID, Date);
                Response.Write((new { success = true, message = "", data = result }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = ex.Message }).Json());
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
                            FileDirectory dic = new FileDirectory(file.FileName, FileType.ResignApplication);
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
            var files = new Services.Views.ApplicationFileAlls(ApplicationID).Where(item => item.Type == (int)FileType.ResignApplication).AsEnumerable();
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
                string ID = Request.Form["ID"];
                string AdminID = Request.Form["Staff"];
                string Manager = Request.Form["Manager"];
                string ManagerName = Request.Form["ManagerName"];
                DateTime Date = Convert.ToDateTime(Request.Form["Date"]);
                bool AmOn = Request.Form["AmOn"] == "true" ? true : false;
                bool AmOff = Request.Form["AmOff"] == "true" ? true : false;
                bool PmOn = Request.Form["PmOn"] == "true" ? true : false;
                bool PmOff = Request.Form["PmOff"] == "true" ? true : false;
                int ReSignTimes = int.Parse(Request.Form["ReSignTimes"]);
                //文件信息
                var files = Request.Form["files"].Replace("&quot;", "'").Replace("amp;", "");
                var fileList = files.JsonTo<List<Yahv.Services.Models.CenterFileDescription>>();
                #endregion

                #region 验证是否可以申请补签
                var applications = Erp.Current.Erm.Applications
                   .Where(item => item.ApplicantID == AdminID && item.ApplicationType == Services.ApplicationType.ReSign);
                var applicationDate = applications
                    .Where(item => item.Context.Contains(Date.ToString("yyyy-MM-dd")))
                    .Where(item => item.ID != ID);
                if (applicationDate.Count() > 0)
                {
                    throw new Exception("已经申请了补签，不能重复申请!");
                }
                if (fileList.Count == 0)
                {
                    throw new Exception("请上传补签附件，用于证明在公司上班!");
                }
                int count = 0;
                count = AmOn ? count + 1 : count;
                count = AmOff ? count + 1 : count;
                count = PmOn ? count + 1 : count;
                count = PmOff ? count + 1 : count;
                if (count == 0)
                {
                    throw new Exception("没有需要补签的时间");
                }
                if (ReSignTimes + count > 3)
                {
                    throw new Exception("每月补签次数不能超过3次");
                }
                #endregion
                var Staff = Alls.Current.Staffs.SingleOrDefault(item => item.Admin.ID == AdminID);

                Application application = Erp.Current.Erm.Applications[ID] ?? new Application();
                application.Title = "补签申请";
                application.Context = new ReSignContext()
                {
                    Date = Date,
                    ApproveID = Manager,
                    ApproveName = ManagerName,
                    DepartmentCode = Staff.DepartmentCode,
                    AmOn = AmOn,
                    AmOff = AmOff,
                    PmOn = PmOn,
                    PmOff = PmOff,
                }.Json();
                application.ApplicantID = AdminID;
                application.CreatorID = Erp.Current.ID;
                application.ApplicationStatus = Services.ApplicationStatus.UnderApproval;
                application.Fileitems = fileList;
                application.ApplicationType = Services.ApplicationType.ReSign;

                application.Enter();

                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
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
                string AdminID = Request.Form["Staff"];
                string Manager = Request.Form["Manager"];
                string ManagerName = Request.Form["ManagerName"];
                DateTime Date = Convert.ToDateTime(Request.Form["Date"]);
                bool AmOn = Request.Form["AmOn"] == "true" ? true : false;
                bool AmOff = Request.Form["AmOff"] == "true" ? true : false;
                bool PmOn = Request.Form["PmOn"] == "true" ? true : false;
                bool PmOff = Request.Form["PmOff"] == "true" ? true : false;
                //文件信息
                var files = Request.Form["files"].Replace("&quot;", "'").Replace("amp;", "");
                var fileList = files.JsonTo<List<Yahv.Services.Models.CenterFileDescription>>();
                #endregion

                #region 验证是否可以申请补签
                int count = 0;
                count = AmOn ? count + 1 : count;
                count = AmOff ? count + 1 : count;
                count = PmOn ? count + 1 : count;
                count = PmOff ? count + 1 : count;
                if (count == 0)
                {
                    throw new Exception("没有需要补签的时间");
                }
                #endregion
                var Staff = Alls.Current.Staffs.SingleOrDefault(item => item.Admin.ID == AdminID);

                Application application = new Application();
                application.Title = "补签申请";
                application.Context = new ReSignContext()
                {
                    Date = Date,
                    ApproveID = Manager,
                    ApproveName = ManagerName,
                    DepartmentCode = Staff.DepartmentCode,
                    AmOn = AmOn,
                    AmOff = AmOff,
                    PmOn = PmOn,
                    PmOff = PmOff,
                }.Json();
                application.ApplicantID = AdminID;
                application.CreatorID = Erp.Current.ID;
                application.ApplicationStatus = Services.ApplicationStatus.Draft;
                application.Fileitems = fileList;
                application.ApplicationType = Services.ApplicationType.ReSign;

                application.Enter();

                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                string message = "空间名：" + ex.Source + "；" + '\n' +
                    "方法名：" + ex.TargetSite + '\n' +
                    "故障点：" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf("\\") + 1, ex.StackTrace.Length - ex.StackTrace.LastIndexOf("\\") - 1) + '\n' +
                    "错误提示：" + ex.Message;
                Response.Write((new { success = false, message = "保存失败：" + message }).Json());
            }
        }

    }
}