using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Crm.Client.BlackLists
{
    public partial class Detail : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var ID = Request.QueryString["ID"];
                var file = new FilesDescriptionRoll().Where(x => x.EnterpriseID == ID).ToArray();
                this.Model.Licenses = file.Where(x => x.Type == CrmFileType.License).Select(x => new { FileName = x.CustomName, Url = x.Url });
                this.Model.LogoFile = file.FirstOrDefault(x => x.Type == CrmFileType.Logo)?.Url;
                var client = Erp.Current.CrmPlus.Clients[ID];
                //所有人
               // this.Model.Top10N = Erp.Current.CrmPlus.MyTops.FirstOrDefault(x => x.ClientID == ID && x.OwnerID == Erp.Current.ID);
                this.Model.Entity = new
                {
                    client.ID,
                    client.Name,
                    Source = client.SourceDes,
                    ClientTypeValue = client.ClientType,
                    ClientType = client.ClientType.GetDescription(),
                    Status = client.Status.GetDescription(),
                    Vip = client?.Vip == null ? VIPLevel.NonVIP.GetDescription() : client.Vip.GetDescription(),
                    ClientGrade = client?.Grade == null ? ClientGrade.None.GetDescription() : client.ClientGrade.GetDescription(),
                    ProfitRate = client.ProfitRate == null ? 0M : client.ProfitRate,
                    Protector = client.Owner != null ? client.Admin.RealName : client.Owner,
                    IsSpecial = client.IsSpecial ? "是" : "否",
                    IsMajor = client.IsMajor ? "是" : "否",
                    product = client.Industry??"",
                    Place = string.IsNullOrEmpty(client.Place) ? Origin.Unknown.GetDescription() : ((Origin)int.Parse(client.Place)).GetDescription(),
                    District = client?.DistrictDes,
                    EnterPriseStatus = client.Status,
                    Grade = client?.Grade,
                    client.EnterpriseRegister.IsSecret,
                    client.EnterpriseRegister.IsInternational,
                    IsInternationDes = client.EnterpriseRegister.IsInternational ? "是" : "否",
                    client.EnterpriseRegister.Corperation,
                    client.EnterpriseRegister.RegAddress,
                    client.EnterpriseRegister.Uscc,
                    Currency = client.EnterpriseRegister?.Currency == null ? Currency.Unknown.GetDescription() : client.EnterpriseRegister.Currency.GetDescription(),
                    RegistCurrency = client.EnterpriseRegister?.RegistCurrency == null ? Currency.Unknown.GetDescription() : client.EnterpriseRegister.RegistCurrency.GetDescription(),
                    client.EnterpriseRegister.RegistFund,
                    client.EnterpriseRegister.Industry,
                    RegistDate = client.EnterpriseRegister.RegistDate?.ToShortDateString(),
                    client.EnterpriseRegister.BusinessState,
                    client.EnterpriseRegister.WebSite,
                    Nature = client.EnterpriseRegister.Nature,
                    client.EnterpriseRegister.Employees
                };

            }
        }
    }
}