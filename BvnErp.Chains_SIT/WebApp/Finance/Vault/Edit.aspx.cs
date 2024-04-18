using Needs.Ccs.Services.ApiSettings;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.App_Utils;

namespace WebApp.Finance.Vault
{
    /// <summary>
    /// 财务金库编辑界面
    /// </summary>
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
            var ServiceIDs = Needs.Wl.Admin.Plat.AdminPlat.Current.Permissions.AdminRoles.Where(manager => manager.Role.Name == "创新恒远财务" || manager.Role.Name == "出纳员").Select(item => item.Admin.ID).ToArray();
            this.Model.AllAdmin = Needs.Wl.Admin.Plat.AdminPlat.Admins.Where(item => ServiceIDs.Contains(item.ID)).Select(item => new { Key = item.ID, Value = item.ByName }).ToArray().Json();

            string id = Request.QueryString["ID"];
            var financeVault = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceVault[id];
            if (financeVault != null)
            {
                this.Model.AllData = new
                {
                    ID = id,
                    Name = financeVault.Name,
                    Leader = financeVault.Leader,
                    Summary = financeVault.Summary
                }.Json();
            }
            else
            {
                this.Model.AllData = new { }.Json();
            }
        }

        /// <summary>
        /// 校验金库名称是否可用，金库名称不允许重复
        /// 不管新增，修改都要校验
        /// </summary>
        protected void CheckValutName()
        {
            var id = Request.Form["ID"];
            var Name = Request.Form["Name"];

            var OriginFinanceVault = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceVault[id];
            if (OriginFinanceVault != null && OriginFinanceVault.Name.Equals(Name))
            {
                Response.Write((new { success = true, message = "" }).Json());
            }
            else
            {
                var financeVault = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceVault.Where(t => t.Name == Name).FirstOrDefault();

                if (financeVault != null)
                {
                    Response.Write((new { success = false, message = "金库名称不能重复" }).Json());
                }
                else
                {
                    Response.Write((new { success = true, message = "" }).Json());
                }
            }
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        protected void Save()
        {
            var Name = Request.Form["Name"];
            var Leader = Request.Form["Leader"];
            var Summary = Request.Form["Summary"];
            var id = Request.Form["ID"];

            var ErmLeaderID = new AdminsTopView2().FirstOrDefault(t => t.OriginID == Leader)?.ID;
            var ErmCreatorID = new AdminsTopView2().FirstOrDefault(t => t.OriginID == Needs.Wl.Admin.Plat.AdminPlat.Current.ID)?.ID;

            CenterVault centerVault = new CenterVault();
            centerVault.OriginName = Name;
            centerVault.Name = Name;
            centerVault.OwnerID = ErmLeaderID;
            centerVault.CreatorID = ErmCreatorID;
            centerVault.Summary = Summary;

            SendStrcut sendStrcut = new SendStrcut();
            sendStrcut.sender = "FSender001";


            var financeVault = new Needs.Ccs.Services.Models.FinanceVault();

            var oldfinanceVault = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceVault[id];
            if (oldfinanceVault != null)
            {
                sendStrcut.option = CenterConstant.Update;
                financeVault.ID = oldfinanceVault.ID;
                if (!oldfinanceVault.Name.Equals(Name))
                {
                    centerVault.OriginName = oldfinanceVault.Name;
                }
            }
            else
            {
                sendStrcut.option = CenterConstant.Enter;
            }

            sendStrcut.model = centerVault;

            financeVault.Name = Name;
            financeVault.Leader = Leader;
            financeVault.Summary = Summary;
            financeVault.Admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
            financeVault.EnterSuccess += FinanceVault_EnterSuccess;
            financeVault.EnterError += FinanceVault_EnterError;


            //提交中心
            string URL = System.Configuration.ConfigurationManager.AppSettings[FinanceApiSetting.ApiName];
            string requestUrl = URL + FinanceApiSetting.VaultUrl;
            string apiclient = JsonConvert.SerializeObject(sendStrcut);

            HttpResponseMessage response = new HttpResponseMessage();
            response = new HttpClientHelp().HttpClient("POST", requestUrl, apiclient);


            if (response == null || response.StatusCode != HttpStatusCode.OK)
            {
                Response.Write((new { success = false, message = "请求中心接口失败：" }).Json());
            }
            else
            {
                financeVault.Enter();
            }
        }
        /// <summary>
        /// 保存异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FinanceVault_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write((new { success = false, message = e.Message }).Json());
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FinanceVault_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功", ID = e.Object }).Json());
        }
    }
}
