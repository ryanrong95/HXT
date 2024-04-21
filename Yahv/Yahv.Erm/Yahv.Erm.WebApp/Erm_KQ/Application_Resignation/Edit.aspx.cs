using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Yahv.Erm.Services;
using Yahv.Erm.Services.Common;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Erm.Services.Models.Rolls;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using ApplicationStatus = Yahv.Erm.Services.ApplicationStatus;
using ApplicationType = Yahv.Erm.Services.ApplicationType;

namespace Yahv.Erm.WebApp.Erm_KQ.Application_Resignation
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitData();
            }
        }

        #region 加载数据
        /// <summary>
        /// 加载数据
        /// </summary>
        private void InitData()
        {
            string id = Request.QueryString["id"];
            Resignation entity = new Resignation();

            this.Model.Postions = Alls.Current.Postions.Select(item => new { value = item.ID, text = item.Name });
            this.Model.FileType = ExtendsEnum.ToArray<FileType>().Select(item => new { value = (int)item, text = item.GetDescription() }).Where(item => item.value >= 2200 && item.value < 2300 || item.value == 2003);

            if (string.IsNullOrWhiteSpace(id))
            {
                if (!string.IsNullOrWhiteSpace(Erp.Current.StaffID))
                {
                    var staffs = Erp.Current.Erm.XdtStaffs
                        .Where(item => item.Status == StaffStatus.Normal || item.Status == StaffStatus.Period);
                    var staff = staffs.Single(item => item.Admin.ID == Erp.Current.ID);
                    var manager = staffs.SingleOrDefault(item => item.DepartmentCode == staff.DepartmentCode && item.PostionCode == ((int)PostType.Manager).ToString());
                    var president = staffs.Single(item => item.PostionCode == ((int)PostType.President).ToString());

                    entity = new Resignation()
                    {
                        Applicant = Erp.Current.RealName,
                        ApplicantID = Erp.Current.ID,
                        CreatorID = Erp.Current.ID,
                        CreateDate = DateTime.Now,
                        DeptLeader = manager?.Name,
                        DeptLeaderID = manager?.Admin?.ID,
                        DeptName = string.IsNullOrEmpty(staff.DepartmentCode) ? "" : ((DepartmentType)Enum.Parse(typeof(DepartmentType), staff.DepartmentCode)).GetDescription(),
                        GeneralManager = president?.Name,
                        GeneralManagerID = president?.Admin?.ID,
                        PostionID = Alls.Current.Staffs[Erp.Current.StaffID]?.PostionID,
                        ResignationDate = null,
                    };
                }

                this.Model.Data = entity;
            }
            else
            {
                entity = JsonConvert.DeserializeObject<Resignation>(Erp.Current.Erm.Applications[id].Context);
                entity.ID = id;
                this.Model.Data = entity;
            }
        }

        /// <summary>
        /// 加载文件
        /// </summary>
        /// <returns></returns>
        protected object LoadFile()
        {
            string ApplicationID = Request.QueryString["id"];
            var files = new Services.Views.ApplicationFileAlls(ApplicationID).AsEnumerable();
            var linq = files.Select(t => new
            {
                ID = t.ID,
                CustomName = t.CustomName,
                FileTypeDec = ((FileType)t.Type).GetDescription(),
                CreateDate = t.CreateDate?.ToString("yyyy-MM-dd"),
                Creater = t.AdminID,
                Url = FileDirectory.ServiceRoot + t.Url,
            });
            return linq;
        }

        /// <summary>
        /// admins
        /// </summary>
        /// <returns></returns>
        protected object GetAdmins()
        {
            var staffs = Erp.Current.Erm.XdtStaffs
                .Where(item => item.Status == StaffStatus.Normal || item.Status == StaffStatus.Period).ToArray()
                .Select(item => new
                {
                    id = item.Admin.ID,
                    name = item.Name,
                    department = string.IsNullOrEmpty(item.DepartmentCode) ? "" : ((DepartmentType)Enum.Parse(typeof(DepartmentType), item.DepartmentCode)).GetDescription(),
                });
            return staffs;
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
        #endregion

        #region 功能函数

        /// <summary>
        /// 提交
        /// </summary>
        /// <returns></returns>
        protected object Submit()
        {
            JMessage json = new JMessage() { success = true, data = "提交成功!" };

            try
            {
                string id = Request.Form["id"];

                string applicant = Request.Form["Applicant"];       //姓名
                string deptName = Request.Form["DeptName"];     //部门
                string postionID = Request.Form["PostionID"];       //岗位
                DateTime? resignationDate = null;       //离职日期
                if (!string.IsNullOrWhiteSpace(Request.Form["ResignationDate"]))
                {
                    resignationDate = DateTime.Parse(Request.Form["ResignationDate"]);
                }
                string deptLeader = Request.Form["DeptLeader"];     //部门负责人
                string deptLeaderID = Request.Form["DeptLeaderID"];     //部门负责人ID
                string generalManager = Request.Form["GeneralManager"];     //总经理
                string generalManagerID = Request.Form["GeneralManagerID"];     //总经理ID
                string handoverID = Request.Form["HandoverID"];     //承接人
                string reasonDescription = Request.Form["ReasonDescription"];       //离职原因
                string jobDescription = Request.Form["JobDescription"];     //工作内容

                //文件信息
                var files = Request.Form["files"].Replace("&quot;", "'").Replace("amp;", "");
                var fileList = files.JsonTo<List<Yahv.Services.Models.CenterFileDescription>>();

                var entity = Erp.Current.Erm.Applications[id] ?? new Application();
                entity.CreateDate = DateTime.Now;
                entity.CreatorID = Erp.Current.ID;
                entity.ApplicationStatus = ApplicationStatus.UnderApproval;
                entity.ApplicationType = ApplicationType.Leave;
                entity.Title = $"{applicant}的离职申请";
                entity.ApplicantID = Erp.Current.ID;
                entity.Fileitems = fileList;
                entity.Context = new Resignation()
                {
                    Applicant = applicant,
                    ApplicantID = Erp.Current.ID,
                    CreateDate = DateTime.Now,
                    CreatorID = Erp.Current.ID,
                    Creator = Erp.Current.RealName,
                    DeptLeader = deptLeader,
                    DeptLeaderID = deptLeaderID,
                    DeptName = deptName,
                    GeneralManager = generalManager,
                    GeneralManagerID = generalManagerID,
                    Handover = Alls.Current.Admins[handoverID]?.RealName,
                    HandoverID = handoverID,
                    PostionID = postionID,
                    PostionName = Alls.Current.Postions[postionID]?.Name,
                    ResignationDate = resignationDate,

                    JobDescription = jobDescription.Trim().Replace("\\r\\n", "\r\n"),
                    ReasonDescription = reasonDescription.Trim().Replace("\\r\\n", "\r\n"),
                }.Json();

                entity.Enter();

                //如果是部门负责人
                var staff = Alls.Current.Staffs[Erp.Current.StaffID];
                if (staff.PostionCode == ((int)Services.Common.PostType.Manager).ToString())
                {
                    var apply = Erp.Current.Erm.ApplicationsRoll[entity.ID];
                    apply.Approval(true);
                }
                //如果是总经理
                if (staff.PostionCode == ((int)Services.Common.PostType.President).ToString())
                {
                    var apply = Erp.Current.Erm.ApplicationsRoll[entity.ID];
                    apply.Approval(true);
                    apply.Approval(true);
                }
            }
            catch (Exception ex)
            {
                json.success = false;
                json.data = $"提交异常!{ex.Message}";
            }
            return json;
        }

        /// <summary>
        /// 保存草稿
        /// </summary>
        /// <returns></returns>
        protected object Save()
        {
            JMessage json = new JMessage() { success = true, data = "保存成功!" };

            try
            {
                string id = Request.Form["id"];

                string applicant = Request.Form["Applicant"];       //姓名
                string deptName = Request.Form["DeptName"];     //部门
                string postionID = Request.Form["PostionID"];       //岗位
                DateTime resignationDate;       //离职日期
                DateTime.TryParse(Request.Form["ResignationDate"], out resignationDate);
                string deptLeader = Request.Form["DeptLeader"];     //部门负责人
                string deptLeaderID = Request.Form["DeptLeaderID"];     //部门负责人ID
                string generalManager = Request.Form["GeneralManager"];     //总经理
                string generalManagerID = Request.Form["GeneralManagerID"];     //总经理ID
                string handoverID = Request.Form["HandoverID"];     //承接人
                string reasonDescription = Request.Form["ReasonDescription"];       //离职原因
                string jobDescription = Request.Form["JobDescription"];     //工作内容

                //文件信息
                var files = Request.Form["files"].Replace("&quot;", "'").Replace("amp;", "");
                var fileList = files.JsonTo<List<Yahv.Services.Models.CenterFileDescription>>();

                if (resignationDate < DateTime.Now.Date)
                {
                    json.success = false;
                    json.data = "离职日期不能早于当前日期!";
                    return json;
                }

                var entity = Erp.Current.Erm.Applications[id] ?? new Application();
                entity.CreateDate = DateTime.Now;
                entity.CreatorID = Erp.Current.ID;
                entity.ApplicationType = ApplicationType.Leave;
                entity.Title = $"{applicant}的离职申请";
                entity.ApplicantID = Erp.Current.ID;
                entity.Fileitems = fileList;
                entity.Context = new Resignation()
                {
                    Applicant = applicant,
                    ApplicantID = Erp.Current.ID,
                    CreateDate = DateTime.Now,
                    CreatorID = Erp.Current.ID,
                    Creator = Erp.Current.RealName,
                    DeptLeader = deptLeader,
                    DeptLeaderID = deptLeaderID,
                    DeptName = deptName,
                    GeneralManager = generalManager,
                    GeneralManagerID = generalManagerID,
                    Handover = Alls.Current.Admins[handoverID]?.RealName,
                    HandoverID = handoverID,
                    PostionID = postionID,
                    PostionName = Alls.Current.Postions[postionID]?.Name,
                    ResignationDate = resignationDate,

                    JobDescription = jobDescription.Trim().Replace("\\r\\n", "\r\n"),
                    ReasonDescription = reasonDescription.Trim().Replace("\\r\\n", "\r\n"),
                }.Json();

                entity.Enter();
            }
            catch (Exception ex)
            {
                json.success = false;
                json.data = $"提交异常!{ex.Message}";
            }
            return json;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        protected void UploadFile()
        {
            try
            {
                string fileType = Request.Form["fileType"];

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
                            FileDirectory dic = new FileDirectory(file.FileName, (FileType)Enum.Parse(typeof(FileType), fileType));
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
        #endregion

    }
}