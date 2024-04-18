using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Swap.Bank
{
    public partial class Edit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            PageInit();
        }

        private void PageInit()
        {
            this.Model.AllData = "".Json();
            string id = Request.QueryString["ID"];
            var bank = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.SwapBanks[id];
            if (bank != null)
            {
                this.Model.AllData = new
                {
                    ID = id,
                    Name = bank.Name,
                    Code = bank.Code,
                    Summary = bank.Summary
                }.Json();
            }
        }

        protected void Save()
        {
            var ID = Request.Form["ID"];
            var Name = Request.Form["Name"];
            var Code = Request.Form["Code"];
            var Summary = Request.Form["Summary"];
            var bank = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.SwapBanks[ID] as
             Needs.Ccs.Services.Models.SwapBank ?? new Needs.Ccs.Services.Models.SwapBank();
            bank.Name = Name;
            bank.Code = Code;
            bank.Summary = Summary;
            bank.EnterSuccess += Bank_EnterSuccess;
            bank.EnterError += Bank_EnterError;
            bank.Enter();
        }

        private void Bank_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功" }).Json());
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Bank_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write((new { success = false, message = "保存失败" }).Json());
        }
    }
}