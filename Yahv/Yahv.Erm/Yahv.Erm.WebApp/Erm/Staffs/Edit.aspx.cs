using System;
using System.Linq;
using System.Transactions;
using Yahv.Erm.Services;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;


namespace Yahv.Erm.WebApp.Erm.Staffs
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadComboBoxData();
                var staffs = Alls.Current.Staffs[Request.QueryString["ID"]];

                if (staffs != null)
                {
                    
                }
            }
        }
        protected void LoadComboBoxData()
        {
            this.Model.PostionData = Alls.Current.Postions.Select(item => new { Value = item.ID, Text = item.Name }).Json();
            this.Model.CityData = Alls.Current.LeaguesRolls.Select(item => new { Value = item.ID, Text = item.Name }).Json();
            this.Model.Status = ExtendsEnum.ToDictionary<Services.StaffStatus>().Select(item => new { Value = item.Key, Text = item.Value }).Json();
            this.Model.CompaniesDate = Alls.Current.Companies.Select(item => new { Value = item.ID, Text = item.Name }).Json();
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var id = Request.QueryString["id"];
                var staff = Alls.Current.Staffs[id] ?? new Staff();
                staff.Name = Request.Form["Name"].Trim();
                staff.Code = Request.Form["Code"].Trim();
                staff.SelCode = Request.Form["SelCode"].Trim();
                staff.DyjCompanyCode = Request.Form["DyjCompanyCode"].Trim();
                staff.DyjDepartmentCode = Request.Form["DyjDepartmentCode"].Trim();
                staff.Gender = (Gender)Enum.Parse(typeof(Gender), Request.Form["Gender"]);
                staff.IDCard= Request.Form["IDCard"].Trim();
                staff.PostionID = Request.Form["PostionID"].Trim();
                staff.WorkCity = Request.Form["WorkCity"].Trim();
                staff.Status = (StaffStatus)Enum.Parse(typeof(StaffStatus), Request.Form["Status"]);
                staff.EnterpriseID= Request.Form["EnterpriseID"].Trim();
                staff.EntryDate = Convert.ToDateTime(Request.Form["EntryDate"]);
                staff.LeaveDate = Convert.ToDateTime(Request.Form["LeaveDate"]);
                staff.EnterError += Staff_EnterError;
                staff.EnterSuccess += Staff_EnterSuccess;
                #region personal
                var personal = Alls.Current.Personals[id] ?? new Personal();
                personal.IDCard = staff.IDCard;
                //personal.Image= Request.Form["Image"].Trim();
                personal.Enter();
                #endregion
                #region labour
                var labour = Alls.Current.Labours[id] ?? new Labour();
                labour.EnterpriseID = staff.EnterpriseID;
                labour.LeaveDate = staff.LeaveDate;
                labour.Enter();
                #endregion
                #region Bankcard
                //TODO:后期维护增加bankcard
                //var bankcard = Alls.Current.BankCards[id] ?? new BankCard();
                #endregion
                staff.Enter();
                scope.Complete();
            }
        }
        /// <summary>
        /// 保存成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Staff_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }

        /// <summary>
        /// 保存失败触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Staff_EnterError(object sender, Usually.ErrorEventArgs e)
        {
            Easyui.Alert("操作提示", e.Message, Web.Controls.Easyui.Sign.Error);
        }
    }
}