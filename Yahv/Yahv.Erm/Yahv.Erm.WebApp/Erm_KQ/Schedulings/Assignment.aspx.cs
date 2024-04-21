using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Yahv.Erm.Services;
using Yahv.Erm.Services.Common;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Controls.Easyui;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.Erm.WebApp.Erm_KQ.Schedulings
{
    public partial class Assignment : ErpParticlePage
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
            this.Model.PostionData = Erp.Current.Erm.XdtPostions.Select(item => new { Value = item.ID, Text = item.Name });
            this.Model.DepartmentType = ExtendsEnum.ToDictionary<DepartmentType>().Select(item => new { Value = item.Key, Text = item.Value });
            this.Model.PostType = ExtendsEnum.ToDictionary<PostType>().Select(item => new { Value = item.Key, Text = item.Value });
            this.Model.Schedulings = Erp.Current.Erm.Schedulings.Where(item => item.IsMain == true).Select(item => new { Value = item.ID, Text = item.Name });
        }

        #region 加载数据
        protected object data()
        {
            Expression<Func<Staff, bool>> expression = Predicate();

            var staffs = Erp.Current.Erm.XdtStaffs
                .Where(item => item.Status == StaffStatus.Normal || item.Status == StaffStatus.Period).Where(expression);

            var data = staffs.OrderByDescending(item => item.CreateDate).ToArray().Select(item => new
            {
                item.ID,
                item.Code,
                item.Name,
                SchedulingOld = item.SchedulingID,
                SchedulingNew = item.SchedulingID,
                DepartmentCode = string.IsNullOrEmpty(item.DepartmentCode) ? "" : ((DepartmentType)Enum.Parse(typeof(DepartmentType), item.DepartmentCode)).GetDescription(),
                PostionName = item.Postion?.Name,
                EntryDate = item.Labour.EntryDate.ToShortDateString(),
            });

            return new
            {
                rows = data.ToArray(),
                total = data.Count(),
            };
        }

        Expression<Func<Staff, bool>> Predicate()
        {
            Expression<Func<Staff, bool>> predicate = item => true;

            //查询参数
            var Name = Request.QueryString["Name"];
            var PositionID = Request.QueryString["Position"];
            var DepartmentType = Request.QueryString["DepartmentType"];
            var PostType = Request.QueryString["PostType"];

            if (!string.IsNullOrWhiteSpace(Name))
            {
                predicate = predicate.And(item => item.Name.Contains(Name) || item.Code.Contains(Name));
            }
            if (!string.IsNullOrWhiteSpace(PositionID))
            {
                predicate = predicate.And(item => item.PostionID == PositionID);
            }
            if (!string.IsNullOrWhiteSpace(DepartmentType))
            {
                predicate = predicate.And(item => item.DepartmentCode == DepartmentType);
            }
            if (!string.IsNullOrWhiteSpace(PostType))
            {
                predicate = predicate.And(item => item.PostionCode == PostType);
            }
            return predicate;
        }
        #endregion

        protected void Submit()
        {
            try
            {
                #region 界面数据

                var schedules = Request.Form["schedules"].Replace("&quot;", "'").Replace("amp;", "");
                var scheduleList = schedules.JsonTo<List<dynamic>>();

                #endregion

                foreach (var schedule in scheduleList)
                {
                    string scheduleId = schedule["SchedulingNew"];
                    string staffId = schedule["ID"];
                    StaffHelper.UpdateScheduling(scheduleId, staffId);
                }
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "员工班别",
                    $"员工班别", scheduleList.Json());
                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }
    }
}