using Needs.Ccs.Services.Models;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Swap.Bank
{
    public partial class SetLimit1 : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadDropdownList();
            PageInit();
        }

        private void PageInit()
        {
            string ID = Request.QueryString["ID"];
            this.Model.IDdate = ID;
        }

        private void LoadDropdownList()
        {
            this.Model.BankData = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.SwapBanks.Select(item => new { value = item.ID, text = item.Name }).Json();
            this.Model.HscodeData = Needs.Wl.Admin.Plat.AdminPlat.Countries.Select(item => new { value = item.Code, text = item.Code + "-" + item.Name }).Json();
        }

        protected void getCountryName()
        {
            string name = "noResult";

            string Code = Request.Form["Code"];
            var country = Needs.Wl.Admin.Plat.AdminPlat.Countries.Where(item => item.Code == Code).FirstOrDefault();
            if (country != null)
            {
                name = country.Name;
            }
            Response.Write(name);
        }


        protected void Save()
        {
            var BankID = Request.Form["ID"];
            var Name = Request.Form["Name"];
            var Code = Request.Form["HSCode"];
            var Summary = Request.Form["Summary"];
            var limit = new Needs.Ccs.Services.Models.SwapLimitCountry();
            limit.Name = Name;
            limit.BankID = BankID;
            limit.Code = Code;
            limit.Summary = Summary;
            limit.SetAdmin(Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID));
            limit.EnterSuccess += Limit_EnterSuccess;
            limit.EnterError += Limit_EnterError;
            limit.Enter();
        }

        private void Limit_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功" }).Json());
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Limit_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write((new { success = false, message = "保存失败" }).Json());
        }
    }
}