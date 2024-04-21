using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Underly.Enums;
using Yahv.Web.Controls.Easyui;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.Erm.WebApp.Erm_KQ.Attends
{
    public partial class ChangeScheduling : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitData();
            }
        }

        #region 加载数据
        private void InitData()
        {
            this.Model.Schedulings = Erp.Current.Erm.Schedulings.Select(item => new { text = item.Name, value = item.ID });
        }
        #endregion

        #region 功能函数
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string id = Request.QueryString["id"];
                string dates = Request.QueryString["dates"];
                string SchedulingID = Request.Form["SchedulingID"];
                var staff = Alls.Current.Staffs.Single(item => item.Admin.ID == id);

                List<DateTime> datetimes = new List<DateTime>();
                foreach (var date in dates.Split(','))
                {
                    datetimes.Add(Convert.ToDateTime(date));
                }

                Services.Common.AttendHelper.UpdatePastsAttendScheduling(staff.ID, datetimes.ToArray(), SchedulingID);

                Easyui.Dialog.Close("操作成功!", Web.Controls.Easyui.AutoSign.Success);
            }
            catch (Exception ex)
            {
                Easyui.Alert("操作提示", $"提交异常!{ex.Message}", Sign.Error);
            }
        }
        #endregion
    }
}