using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using NtErp.Crm.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.MyApprove
{
    public partial class ClientView : Uc.PageBase
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
                        ImportantLevel = clientExtends.ImportantLevel == null ? string.Empty : clientExtends.ImportantLevel.GetDescription(),
                    }.Json();
                }
                else
                {
                    this.Model.CustomerData = string.Empty.Json();
                }
                var files = new NtErp.Crm.Services.Views.FileAlls().Where(item => item.ClientID == ClientId && item.Status == Status.Normal);
                this.Model.Files = files.Select(item => new { item.ID, item.Name, item.Url }).Json();
                this.Model.AreaData = new NtErp.Crm.Services.Views.AreaTree().tree;
            }
        }

        /// <summary>
        /// 审批记录展示
        /// </summary>
        protected void data()
        {
            var applyid = Request.QueryString["ID"];
            var applysteps = new NtErp.Crm.Services.Views.ApplyStepAlls().Where(item => item.ApplyID == applyid);
            Func<NtErp.Crm.Services.Models.ApplyStep, object> convert = item => new
            {
                item.AdminName,
                StatusName = item.Status.GetDescription(),
                AprDate = item.AprDate.ToString(),
                item.Comment,
            };

            this.Paging(applysteps, convert);
        }
    }
}