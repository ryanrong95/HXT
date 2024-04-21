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
using Yahv.Underly.Enums;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.Erm.WebApp.Erm_KQ.Application_Offtime
{
    public partial class Details : ErpParticlePage
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
            var staffs = Erp.Current.Erm.XdtStaffs.ToArray();
            //员工
            this.Model.StaffData = staffs.Select(item => new
            {
                Value = item.Admin?.ID,
                Text = item.Name,
                SelCode = item.SelCode,
                YearsDay = item.VacationItems.Where(t => t.Type == VacationType.YearsDay).Sum(t => t.Lefts),
                OffDay = item.VacationItems.Where(t => t.Type == VacationType.OffDay).Sum(t => t.Lefts)
            });
            //部门类型
            this.Model.DepartmentType = ExtendsEnum.ToDictionary<DepartmentType>().Select(item => new { Value = item.Key, Text = item.Value });
            //假期类型
            this.Model.SchedulePrivateType = ExtendsEnum.ToDictionary<Underly.Enums.LeaveType>().Select(item => new
            {
                Value = item.Key,
                Text = item.Value
            });
            //请假时长
            this.Model.DateType = ExtendsEnum.ToDictionary<DateLengthType>().Select(item => new { Value = item.Key, Text = item.Value });
            //出差原因
            this.Model.BusinessTripReason = ExtendsEnum.ToArray<BusinessTripReason>().Select(item => new
            {
                Value = (int)item,
                Text = item.GetDescription()
            });
            //是否借款
            this.Model.LoanOrNot = ExtendsEnum.ToArray<LoanOrNot>().Select(item => new
            {
                Value = (int)item,
                Text = item.GetDescription()
            });
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
                    Type = (int)application.OffTimeContext.LeaveType,
                    SwapStaff = application.OffTimeContext.SwapStaff,
                    WorkContext = application.OffTimeContext.WorkContext,
                    Reason = application.OffTimeContext.Reason,
                    BusinessReason = application.OffTimeContext.BusinessReason,
                    Entourage = application.OffTimeContext.Entourage,
                    LoanOrNot = application.OffTimeContext.LoanOrNot,

                    ApproveID = application.OffTimeContext.ApproveID,
                    DepartmentName = application.OffTimeContext.DepartmentName,
                };
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
                            FileDirectory dic = new FileDirectory(file.FileName, FileType.OfftimeApplication);
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
            var files = new Services.Views.ApplicationFileAlls(ApplicationID).Where(item => item.Type == (int)FileType.OfftimeApplication).AsEnumerable();
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
        /// 请假日期
        /// </summary>
        /// <returns></returns>
        protected object LoadDate()
        {
            var id = Request.QueryString["ID"];
            var dates = Erp.Current.Erm.Applications[id]?.OffTimeContext?.DateItems;
            var data = dates.Select(item => new
            {
                Date = item.Date.ToString("yyyy-MM-dd"),
                Type = item.Type,
            });
            return new
            {
                rows = data,
                total = data.Count(),
            };
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

        protected object LoadSchedule()
        {
            var id = Request.QueryString["ID"];
            var dates = Erp.Current.Erm.Applications[id]?.OffTimeContext?.ScheduleItems;
            var data = dates.Select(item => new
            {
                StartDate = item.StartDate.ToString("yyyy-MM-dd"),
                StartPlace = item.StartPlace,
                EndDate = item.EndDate.ToString("yyyy-MM-dd"),
                EndPlace = item.EndPlace,
                Vehicle = item.Vehicle,
                VehicleCost = item.VehicleCost,
                StayDay = item.StayDay,
                CompanyName = item.CompanyName,
                Person = item.Person,
                Department = item.Department,
                Position = item.Position,
                Phone = item.Phone,
                BusinessReason = item.BusinessReason,
            });
            return new
            {
                rows = data,
                total = data.Count(),
            };
        }
    }
}