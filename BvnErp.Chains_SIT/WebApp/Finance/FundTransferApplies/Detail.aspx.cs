using Needs.Ccs.Services.Views;
using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Needs.Ccs.Services.ApiSettings;
using Newtonsoft.Json;
using System.Net.Http;
using WebApp.App_Utils;
using System.Net;

namespace WebApp.Finance.FundTransferApplies
{
    /// <summary>
    /// 金库账户编辑界面
    /// </summary>
    public partial class Detail : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PageInit();
                LoadComboBoxData();
            }
        }

        protected void LoadComboBoxData()
        {
            //金库
            //this.Model.FinanceVaultData = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceVault.Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal).Select(item => new { Value = item.ID, Text = item.Name }).Json();
            var accountData = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceAccounts;
            this.Model.FinanceVaultData = accountData.Select(item => new { Value = item.FinanceVaultID, Text = item.FinanceVaultName }).Distinct().Json();
            this.Model.FundTransferType = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.FundTransferType>().Select(item => new { Value = item.Key, Text = item.Value }).Json();
        }

        protected void getAccounts()
        {
            string VaultID = Request.Form["VaultID"];
            //string Currency = Request.Form["Currency"];
            //string IsCash = Request.Form["IsCash"];

            var result = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceAccounts
                .Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal)
                .Where(t => t.FinanceVaultID == VaultID);

            //if (!string.IsNullOrEmpty(IsCash) && IsCash == "true")
            //{
            //    result = result.Where(t => t.IsCash == true);
            //}
            //else
            //{
            //    result = result.Where(t => t.IsCash == false);
            //}

            if (result != null)
            {
                Response.Write(result.Select(item => new { Value = item.ID, Text = item.AccountName }).Json());
            }
        }

        /// <summary>
        /// 页面数据初始化
        /// </summary>
        void PageInit()
        {
            this.Model.AllData = "".Json();
            string id = Request.QueryString["ID"];
            Needs.Ccs.Services.Models.FundTransferApplies apply = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FundTransferApplies[id];
            if (apply != null)
            {
                this.Model.AllData = new
                {
                    ID = id,
                    OutVault = apply.OutAccount.FinanceVaultID,
                    OutAccount = apply.OutAccount.ID,
                    OutMoney = apply.OutAmount,
                    FromSeqNo = apply.FromSeqNo,
                    InVault = apply.InAccount.FinanceVaultID,
                    InAccount = apply.InAccount.ID,
                    InMoney = apply.InAmount,
                    Summary = apply.Summary,
                    FeeType = apply.FeeType,
                }.Json();
            }
        }

      

        /// <summary>
        /// 保存数据
        /// </summary>
        protected void Save()
        {
            var id = Request.Form["ID"];
            var OutVault = Request.Form["OutVault"];
            var OutAccount = Request.Form["OutAccount"];
            var OutMoney = Request.Form["OutMoney"];
            var FromSeqNo = Request.Form["FromSeqNo"];
            var InVault = Request.Form["InVault"];
            var InAccount = Request.Form["InAccount"];
            var InMoney = Request.Form["InMoney"];
            var Summary = Request.Form["Summary"];

            Needs.Ccs.Services.Models.FundTransferApplies apply = new Needs.Ccs.Services.Models.FundTransferApplies();
            
            apply.OutAccount = new FinanceAccount { ID = OutAccount };
            apply.OutAmount = Convert.ToDecimal(OutMoney);
            apply.FromSeqNo = FromSeqNo;
            apply.InAccount = new FinanceAccount { ID = InAccount };
            apply.InAmount = Convert.ToDecimal(InMoney);
            apply.Admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
            apply.Summary = Summary;

            apply.EnterSuccess += FinanceVault_EnterSuccess;
            apply.EnterError += FinanceVault_EnterError;

            apply.Enter();

            apply.Log("财务【"+ Needs.Wl.Admin.Plat.AdminPlat.Current.RealName+"】提交了资金调拨申请，调拨金额："+apply.OutAmount);
                 
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

        /// <summary>
        /// 审批
        /// </summary>
        protected void Approve()
        {
            var id = Request.Form["ID"];
            var Summary = Request.Form["Summary"];
            Needs.Ccs.Services.Models.FundTransferApplies apply = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FundTransferApplies[id];
            if (apply != null)
            {
                string CostApplyFinanceStaffName = System.Configuration.ConfigurationManager.AppSettings["CostApplyFinanceStaffName"];
                try
                {
                    apply.Approve(Needs.Ccs.Services.Enums.FundTransferApplyStatus.Paying);
                    apply.Log("财务负责人【" + CostApplyFinanceStaffName + "】审批通过了资金调拨申请,备注:"+ Summary);
                    Response.Write((new { success = true, info = "审批成功"}).Json());
                }
                catch(Exception ex)
                {
                    apply.Log("财务负责人【" + CostApplyFinanceStaffName + "】审批出错:"+ex.Message);
                    Response.Write((new { success = false, info = ex.Message }).Json());
                }
            }
            else
            {
                Response.Write((new { success = false, info = "查询该条资金调拨记录出错，请联系管理员!"}).Json());
            }
        }

        /// <summary>
        /// 审批
        /// </summary>
        protected void Deny()
        {
            var id = Request.Form["ID"];
            var Summary = Request.Form["Summary"];
            Needs.Ccs.Services.Models.FundTransferApplies apply = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FundTransferApplies[id];
            if (apply != null)
            {
                string CostApplyFinanceStaffName = System.Configuration.ConfigurationManager.AppSettings["CostApplyFinanceStaffName"];
                try
                {
                    apply.Approve(Needs.Ccs.Services.Enums.FundTransferApplyStatus.Denied);
                    apply.Log("财务负责人【" + CostApplyFinanceStaffName + "】拒绝资金调拨申请,备注:" + Summary);
                    Response.Write((new { success = true, info = "审批成功" }).Json());
                }
                catch (Exception ex)
                {
                    apply.Log("财务负责人【" + CostApplyFinanceStaffName + "】审批出错:" + ex.Message);
                    Response.Write((new { success = false, info = ex.Message }).Json());
                }
            }
            else
            {
                Response.Write((new { success = false, info = "查询该条资金调拨记录出错，请联系管理员!" }).Json());
            }
        }

        /// <summary>
        /// 读取日志
        /// </summary>
        protected void LoadLogs()
        {
            string ID = Request.Form["ID"];
            var list = new Needs.Ccs.Services.Views.LogsView().Where(t => t.MainID == ID).OrderByDescending(t => t.CreateDate).AsQueryable();
            Func<Needs.Ccs.Services.Models.Logs, object> convert = item => new
            {
                ID = item.ID,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Summary = item.Summary
            };
            Response.Write(new { rows = list.Select(convert).ToArray(), }.Json());
        }

    }
}