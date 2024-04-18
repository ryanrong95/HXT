using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Permissions.Admin
{
    public partial class SetDepartment : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadDate();
        }

        /// <summary>
        /// 初始化数据加载
        /// </summary>
        protected void LoadDate()
        {
            this.Model.DepartmentData = new Needs.Ccs.Services.Views.DepartmentView().Where(t => t.FatherID == "A85A9D761ADB49D8B24E29EFBCBEBCD9").Select(t => new { Key = t.ID, Value = t.Name }).Json();
            string id = Request.QueryString["ID"];
            var adminDepart = new Needs.Ccs.Services.Views.AdminsTopView().Where(item => item.ID == id).FirstOrDefault();
            string isleadid = string.Empty;
            if (!string.IsNullOrEmpty(adminDepart?.DepartmentID))
            {
                isleadid = new Needs.Ccs.Services.Views.DepartmentView()[adminDepart.DepartmentID].LeaderID;
            }

            this.Model.AdminData = new
            {
                ID = id,
                DepartmentID = adminDepart?.DepartmentID,
                IsLeader = string.IsNullOrEmpty(isleadid) ? false : true
            }.Json();
        }

        protected void Save()
        {
            var id = Request.Form["ID"];
            var departid = Request.Form["Set"];
            var isleader = Request.Form["IsLeader"] != null;

            var admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(id);

            admin.DepartmentID = departid;
            admin.EnterError += Admin_EnterError;
            admin.EnterSuccess += Admin_EnterSuccess;
            admin.Enter();
            //设置部门leader
            if (isleader)
            {
                var department = new Needs.Ccs.Services.Views.DepartmentView()[departid];
                department.LeaderID = id;
                department.SetDepartmentLeader();
            }
        }

        /// <summary>
        /// 保存异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Admin_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功" }).Json());
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Admin_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write((new { success = false, message = "保存失败" }).Json());
        }
    }
}
