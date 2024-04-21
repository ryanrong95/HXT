using System;
using System.Linq;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Erm.WebApp.Erm.Staffs
{
    public partial class LabourEdit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadComboBoxData();
                this.Model.AllData = Alls.Current.Labours[Request.QueryString["ID"]];
            }
        }
        protected void LoadComboBoxData()
        {
            this.Model.CompaniesDate = Alls.Current.Companies.Select(item => new { Value = item.ID, Text = item.Name });
        }
        protected void Save()
        {
            var id = Request.Form["ID"];
            #region labour
            var labour = Alls.Current.Labours[id] ?? new Labour();
            labour.ID = id;
            labour.EnterpriseID = Request.Form["EnterpriseID"].Trim();
            labour.EntryCompany = Request.Form["EntryCompany"].Trim();
            labour.EntryDate = Convert.ToDateTime(Request.Form["EntryDate"]);
            TimeSpan sp = labour.EntryDate.Subtract(DateTime.Now);
            int daysp = sp.Days + 1;
            if (daysp >= 200)
            {
                Response.Write((new { success = false, message = "入职时间错误，请核对后填入" }).Json());
                return;
            }
            if (Request.Form["LeaveDate"] != "")
            {
                labour.LeaveDate = Convert.ToDateTime(Request.Form["LeaveDate"]);
            }
            else
            {
                labour.LeaveDate = null;
            }
            labour.EnterError += Labour_EnterError;
            labour.EnterSuccess += Labour_EnterSuccess;
            Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "员工信息",
                   string.IsNullOrWhiteSpace(id) ? "新增劳资信息" : "修改劳资信息", labour.Json());
            labour.Enter();
            #endregion
        }

        /// <summary>
        /// 保存成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Labour_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {

            Response.Write((new { success = true, message = "保存成功" }).Json());
        }

        /// <summary>
        /// 保存失败触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Labour_EnterError(object sender, Usually.ErrorEventArgs e)
        {
            Response.Write((new { success = false, message = "保存失败" }).Json());
        }
    }
}