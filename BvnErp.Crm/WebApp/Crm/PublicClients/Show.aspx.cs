using Needs.Underly;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using NtErp.Crm.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.PublicClients
{
    /// <summary>
    /// 公海客户详情页面
    /// </summary>
    public partial class Show : Needs.Web.Sso.Forms.ErpPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.PageInit();
            }
        }

        /// <summary>
        /// 页面数据初始化
        /// </summary>
        void PageInit()
        {
            string id = Request.QueryString["id"];
            var clientdossier = Needs.Erp.ErpPlot.Current.ClientSolutions.PublicClients.GetTop(1, item => item.Client.ID == id).SingleOrDefault();
            if (clientdossier != null)
            {
                var clientExtends = JsonSerializerExtend.JsonTo<NtErp.Crm.Services.Models.ClientExtends>(clientdossier.Client.NTextString);
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
                }.Json();
            }
            else
            {
                this.Model.CustomerData = string.Empty.Json();
            }

        }

    }
}