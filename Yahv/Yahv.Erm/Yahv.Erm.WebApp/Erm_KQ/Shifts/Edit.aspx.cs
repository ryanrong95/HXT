using System;
using System.Linq;
using System.Threading.Tasks;
using Yahv.Erm.Services;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Utils.Serializers;
using Yahv.Web.Controls.Easyui;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.Erm.WebApp.Erm_KQ.Shifts
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadComboBoxData();
                InitData();
            }
        }

        #region 加载数据

        protected void LoadComboBoxData()
        {
            //员工
            var staffs = Erp.Current.Erm.XdtStaffs.Where(item => item.Status == StaffStatus.Normal || item.Status == StaffStatus.Period);
            this.Model.StaffData = staffs.Select(item => new
            {
                Value = item.ID,
                Text = item.Name,
            });
            //班别
            var schedulings = Erp.Current.Erm.Schedulings.Where(item => item.IsMain == true);
            this.Model.SchedulingData = schedulings.Select(item => new
            {
                Value = item.ID,
                Text = item.Name,
            });
        }

        private void InitData()
        {
            string id = Request.QueryString["id"];

            if (!string.IsNullOrWhiteSpace(id))
            {
                var shift = Erp.Current.Erm.ShiftStaffs[id];
                this.Model.Data = new
                {
                    ID = shift.ID,
                    Shifts = shift.ShiftSchedule,
                    nextSch = shift.NextSchedulingID,
                };
            }
        }

        protected void StaffChange()
        {
            try
            {
                string ID = Request.Form["ID"];
                var staff = Erp.Current.Erm.XdtStaffs.SingleOrDefault(item => item.ID == ID);
                var data = new
                {
                    selcode = staff?.SelCode,
                    scheduling = staff.SchedulingID,
                };
                Response.Write((new { success = true, message = "保存成功", data = data }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }

        #endregion

        #region 功能按钮

        protected void Save()
        {
            try
            {
                string ID = Request.Form["ID"];
                string Staff = Request.Form["Staff"];
                string Shifts = Request.Form["Shifts"];
                string NextSch = Request.Form["NextSch"];

                ShiftStaff entity = Erp.Current.Erm.ShiftStaffs[ID] ?? new ShiftStaff() { CreatorID = Erp.Current.ID };
                entity.ID = Staff;
                entity.ShiftSchedule = Shifts;
                entity.NextSchedulingID = NextSch;
                entity.ModifyID = Erp.Current.ID;
                
                entity.Enter();
                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }

        #endregion
    }
}