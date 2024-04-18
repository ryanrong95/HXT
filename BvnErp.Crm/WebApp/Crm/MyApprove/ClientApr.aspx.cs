using Needs.Utils.Descriptions;
using Needs.Utils.Http;
using Needs.Utils.Serializers;
using NtErp.Crm.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Needs.Utils.Http.HttpHelper;

namespace WebApp.Crm.MyApprove
{
    /// <summary>
    /// 客户审核页面
    /// </summary>
    public partial class ClientApr : Uc.PageBase
    {
        /// <summary>
        /// 页面数据加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string ClientId = Request.QueryString["clientid"];
                var clientdossier = Needs.Erp.ErpPlot.Current.ClientSolutions.MyClients.GetTop(1, item => item.Client.ID == ClientId).SingleOrDefault();
                if (clientdossier != null)
                {
                    var clientExtends = JsonSerializerExtend.JsonTo<NtErp.Crm.Services.Models.ClientExtends>(clientdossier.Client.NTextString);
                    var industries = Needs.Erp.ErpPlot.Current.ClientSolutions.MyIndustries;
                    //主要产品
                    if (clientExtends.IndustryInvolved != null)
                    {
                        var IndustryInvolveds = industries.Where(item => clientExtends.IndustryInvolved.Split(',').Contains(item.ID)).
                            Select(item => item.Name).ToArray();
                        clientExtends.IndustryInvolved = string.Join(",", IndustryInvolveds);
                    }
                    this.Model.CustomerData = new
                    {
                        clientdossier.Client.Name,
                        EnterpriseProperty = clientExtends.EnterpriseProperty.GetDescription(),
                        Area = clientExtends.Area.GetDescription(),
                        clientExtends.RegisteredCapital,
                        Currency = clientExtends.Currency.GetDescription(),
                        clientExtends.EstablishmentDate,
                        clientExtends.OperatingPeriod,
                        clientExtends.RegisteredAddress,
                        clientExtends.OfficeAddress,
                        clientExtends.Site,
                        clientExtends.BusinessScope,
                        clientExtends.CUSCC,
                        CustomerType = clientExtends.CustomerType.GetDescription(),
                        CustomerLevel = clientExtends.CustomerLevel == null ? string.Empty : clientExtends.CustomerLevel.GetDescription(),
                        AgentBrand = string.Join(",", clientdossier.Manufactures.Select(item => item.Name).ToArray()),
                        CompanyName = string.IsNullOrWhiteSpace(clientExtends.CompanyID) ? string.Empty :
                            Needs.Erp.ErpPlot.Current.ClientSolutions.Companys[clientExtends.CompanyID].Name,
                        BusinessType = clientExtends.BusinessType.GetDescription(),
                        ReIndustry = string.Join(",", clientdossier.Industries.Select(item => item.Name).ToArray()),
                        clientExtends.IndustryInvolved,
                        ProtectLevel = clientExtends.ProtectLevel == null ? string.Empty : clientExtends.ProtectLevel.GetDescription(),
                        clientExtends.ProtectionScope,
                        clientExtends.CreditLimit,
                        clientExtends.CreditPayment,
                        CustomerStatus = clientExtends.CustomerStatus.GetDescription(),
                        clientExtends.ExtraPacking,
                        clientExtends.SpecialSupplier,
                        clientExtends.InformationSource,
                        clientExtends.AdminCode,
                        clientExtends.Summary,
                        clientExtends.AreaID,
                        IsProtected = CheckDYJProtected(clientdossier.Client.Name),
                        ImportantLevel = clientExtends.ImportantLevel == null ? string.Empty : clientExtends.ImportantLevel.GetDescription(),
                    }.Json();
                }
                else
                {
                    this.Model.CustomerData = string.Empty.Json();
                }
                var reportcount = new NtErp.Crm.Services.Views.ReportsAlls().Count(item => item.Client.ID == ClientId);
                var projectcount = new NtErp.Crm.Services.Views.ProjectAlls().Count(item => item.ClientID == ClientId);
                this.Model.IsShow = (reportcount + projectcount > 0).Json();
                var files = new NtErp.Crm.Services.Views.FileAlls().Where(item => item.ClientID == ClientId && item.Status == Status.Normal);
                this.Model.Files = files.Select(item => new { item.ID, item.Name, item.Url }).Json();
                SetDropDownList();
            }
        }


        /// <summary>
        /// 下拉框数据源初始化
        /// </summary>
        private void SetDropDownList()
        {
            this.Model.AreaData = new NtErp.Crm.Services.Views.AreaTree().tree;
        }

        /// <summary>
        /// 校验客户是否被大赢家保护
        /// </summary>
        /// <param name="ClientName"></param>
        /// <returns></returns>
        protected bool CheckDYJProtected(string ClientName)
        {
            var applyid = Request.QueryString["ID"];
            var admin = new NtErp.Crm.Services.Views.ApplyAlls()[applyid].Admin;

            var Url = "http://vpn3.t996.top:8800/users/views/page/" + "?page=1&Code=23788432ea2b4c93ba27f8538e410719&Queries=" 
                + ClientName + "&Query_field=FullName&Whether_the_fuzzy=false&format=json";
            
            CookieCollection responseCookies = new CookieCollection();
            var result = HttpHelper.Get(Url, responseCookies, Accept.json);
            var data = result.Data.JsonTo()["data"];
            int count = data.Count(item => item["ShortName"].ToString().Split('.')[0] == admin.DyjID);
            if (data.Count() > 0 && count == 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 同意
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAllow_Click(object sender, EventArgs e)
        {
            this.Appr(ApplyStep.Allow);
            this.UpdateStatus(ApplyStep.Allow);
            Alert("操作成功", Request.Url, true);
        }

        /// <summary>
        /// 否决
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnVote_Click(object sender, EventArgs e)
        {
            this.Appr(ApplyStep.Vote);
            this.UpdateStatus(ApplyStep.Vote);
            Alert("操作成功", Request.Url, true);
        }

        /// <summary>
        /// 审批记录
        /// </summary>
        /// <param name="applystep"></param>
        protected void Appr(ApplyStep applystep)
        {
            var appr = new NtErp.Crm.Services.Models.ApplyStep();
            appr.ApplyID = Request.Form["ApplyID"];
            appr.AdminID = Needs.Erp.ErpPlot.Current.ID;
            appr.Step = (int)applystep;
            appr.Status = applystep;
            appr.Comment = Request.Form["AprSummary"];
            appr.AprDate = DateTime.Now;
            appr.Enter();
        }

        /// <summary>
        /// 更新原数据状态
        /// </summary>
        /// <param name="applystep"></param>
        protected void UpdateStatus(ApplyStep applystep)
        {
            string applyid = Request.Form["ApplyID"];
            var apply = new NtErp.Crm.Services.Views.ApplyAlls()[applyid] as
                NtErp.Crm.Services.Models.Apply ?? new NtErp.Crm.Services.Models.Apply();
            apply.Status = (applystep == ApplyStep.Allow) ? ApplyStatus.Approval : ApplyStatus.NotApproval;

            //更新客户的装填
            var client = Needs.Erp.ErpPlot.Current.ClientSolutions.MyClientsBase[apply.MainID];
            client.Status = (applystep == ApplyStep.Allow) ? ActionStatus.Complete : ActionStatus.Reject;
            client.OnSingleUpdateEnter();

            apply.Enter();

        }
    }
}