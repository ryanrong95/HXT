using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.CostApply
{
    public partial class PayEditView : Uc.PageBase
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
            string CostApplyID = Request.QueryString["CostApplyID"];
            this.Model.PaperNotesStatus = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.CheckPaperNotesEnum>().Select(item => new { item.Key, item.Value }).Json();
          //  string id = Request.QueryString["ID"];
            var data = new Needs.Ccs.Services.Views.PayApproverListView().GetResults(CostApplyID);
            if (data != null)
            {
                this.Model.AllData = new
                {
                    CostApplyID = CostApplyID,
                    PaperNotesStatus =data.PaperNotesStatusInt
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
            var apernotesstatus = Request.Form["PaperNotesStatus"];
            var ID = Request.Form["CostApplyID"];
            var admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

            var commissionproportion = new Needs.Ccs.Services.Views.PayApproverListView().GetResults(ID);
            commissionproportion.ID = ID;
            commissionproportion.CreateDate = DateTime.Now;
            commissionproportion.PaperNotesStatusInt = Convert.ToInt32(apernotesstatus);
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