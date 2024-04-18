using Needs.Underly;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using Newtonsoft.Json.Linq;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models;
using NtErp.Crm.Services.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.MyClients
{
    /// <summary>
    /// 客户详细数据展示页面
    /// </summary>
    public partial class Show : Needs.Web.Sso.Forms.ErpPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.PageInit();
                this.Model.AreaData = new AreaTree().tree;
            }
        }

        private void PageInit()
        {
            string id = Request.QueryString["id"];
            var clientdossier = Needs.Erp.ErpPlot.Current.ClientSolutions.MyClients.GetTop(1, item => item.Client.ID == id).SingleOrDefault();
            if (clientdossier != null)
            {
                var clientExtends = JsonSerializerExtend.JsonTo<ClientExtends>(clientdossier.Client.NTextString);
                var industries = Needs.Erp.ErpPlot.Current.ClientSolutions.MyIndustries;
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
                    ImportantLevel = clientExtends.ImportantLevel == null ? string.Empty : clientExtends.ImportantLevel.GetDescription(),
                }.Json();
            }
            else
            {
                this.Model.CustomerData = string.Empty.Json();
            }
            var files = new FileAlls().Where(item => item.ClientID == id && item.Status == Status.Normal);
            this.Model.Files = files.Select(item => new { item.ID, item.Name, item.Url }).Json();
        }

        /// <summary>
        /// 获取审批意见
        /// </summary>
        protected void GetComment()
        {
            string clientid = Request.Form["ClientID"];
            var apply = new ApplyAlls().Where(item => item.MainID == clientid && item.Status == ApplyStatus.NotApproval).
                OrderByDescending(item => item.CreateDate).FirstOrDefault();
            var applyid = apply?.ID;
            var Comment = new ApplyStepAlls().Where(item => item.ApplyID == applyid).FirstOrDefault()?.Comment;
            Response.Write(Comment);
        }
    }
}