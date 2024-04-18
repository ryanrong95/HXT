using Needs.Ccs.Services.Models;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.GeneralManage.CommissionProportion
{
    public partial class Edit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PageInit();
            }
        }
        /// <summary>
        /// 页面数据初始化
        /// </summary>
        void PageInit()
        {
            string id = Request.QueryString["ID"];
            var commissionproportion = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.CommissionProportions[id];
            if (commissionproportion != null)
            {
                this.Model.AllData = new
                {
                    ID = id,
                    RegeisterMonth = commissionproportion.RegeisterMonth,
                    CommissionProportion = commissionproportion.Proportion,
                    Summary = commissionproportion.Summary
                }.Json();
            }
            else
            {
                this.Model.AllData = new { }.Json();
            }

        }
        /// <summary>
        /// 保存数据
        /// </summary>
        protected void Save()
        {
            var RegeisterMonth = Request.Form["RegeisterMonth"];
            var CommissionProportion = Request.Form["CommissionProportion"];
            var Summary = Request.Form["Summary"];
            var id = Request.Form["ID"];
            var commissionproportion = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.CommissionProportions[id] as
             Needs.Ccs.Services.Models.CommissionProportion ?? new Needs.Ccs.Services.Models.CommissionProportion();
            commissionproportion.Proportion = decimal.Parse(CommissionProportion);
            commissionproportion.RegeisterMonth = int.Parse(RegeisterMonth);
            commissionproportion.Summary = Summary;
            commissionproportion.EnterSuccess += CommissionProportion_EnterSuccess;
            commissionproportion.EnterError += CommissionProportion_EnterError;
            commissionproportion.Enter();
        }
        /// <summary>
        /// 保存异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommissionProportion_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write(new { success = false, message = e.Message });
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommissionProportion_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功", ID = e.Object }).Json());
        }
    }
}