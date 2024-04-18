using Needs.Ccs.Services.Enums;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Client.View
{
    /// <summary>
    /// 会员（客户）-客服查看页面
    /// Add by Merlin@20181103
    /// </summary>
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadComboBoxData();
        }

        /// <summary>
        /// 初始化下拉框数据
        /// </summary>
        protected void LoadComboBoxData()
        {
            var status = EnumUtils.ToDictionary<ClientStatus>().Select(item => new { item.Key, item.Value });
            this.Model.Status = status.Json();

            this.Model.PvWsOrderUrl = System.Configuration.ConfigurationManager.AppSettings["PvWsOrder"];
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        protected void data()
        {
            string CompanyName = Request.QueryString["CompanyName"];
            string ClientCode = Request.QueryString["ClientCode"];
            string UnPayExchangeAmount = Request.QueryString["UnPayExchangeAmount"];
            string CreateDateFrom = Request.QueryString["CreateDateFrom"];
            string CreateDateTo = Request.QueryString["CreateDateTo"];
            string Status = Request.QueryString["Status"];
            string NormalType = Request.QueryString["NormalType"];

            var clients = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyClients.AsQueryable();
            if (!string.IsNullOrEmpty(CompanyName))
            {
                clients = clients.Where(t => t.Company.Name.Contains(CompanyName.Trim()));
            }
            if (!string.IsNullOrEmpty(ClientCode))
            {
                clients = clients.Where(t => t.ClientCode.Contains(ClientCode.Trim()));
            }
            if (!string.IsNullOrEmpty(UnPayExchangeAmount))
            {
                var amount = decimal.Parse(UnPayExchangeAmount);
                clients = clients.Where(t => t.UnPayExchangeAmount >= amount);
            }
            if (!string.IsNullOrEmpty(CreateDateFrom))
            {
                var from = DateTime.Parse(CreateDateFrom);
                clients = clients.Where(t => t.CreateDate >= from);
            }
            if (!string.IsNullOrEmpty(CreateDateTo))
            {
                var to = DateTime.Parse(CreateDateTo).AddDays(1);
                clients = clients.Where(t => t.CreateDate < to);
            }
            if (!string.IsNullOrEmpty(Status))
            {
                int status = Int32.Parse(Status);
                clients = clients.Where(t => t.ClientStatus == (ClientStatus)status);
            }
            if (!string.IsNullOrEmpty(NormalType))
            {
                NormalType = NormalType.Trim();
                if (NormalType == "1")
                {
                    clients = clients.Where(t => t.IsNormal == true);
                }
                else if (NormalType == "0")
                {
                    clients = clients.Where(t => t.IsNormal == false);
                }
            }
            var adminid = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
            Func<Needs.Ccs.Services.Models.Client, object> convert = client => new
            {
                client.ID,
                ClientAdminID = adminid,
                CompanyCode = client.Company.Code,
                CompanyName = client.Company.Name,
                CustomsCode = client.Company.CustomsCode,
                ClientCode = client.ClientCode,
                ClientRankValue = client.ClientRank,
                ClientRank = client.ClientRank.GetDescription(),
                ContactName = client.Company.Contact.Name,
                ContactTel = client.Company.Contact.Mobile,
                CreateDate = client.CreateDate.ToShortDateString(),
                StatusValue = client.ClientStatus,
                Status = client.ClientStatus.GetDescription(),
                ServiceManagerName = client.ServiceManager?.RealName,
                MerchandiserName = client.Merchandiser?.RealName,
                //StorageServiceManagerName = client.StorageServiceManager?.RealName,
                //StorageMerchandiserName = client.StorageMerchandiser?.RealName,
                RegisterYear = client.RegisterYear,
                ClientNature = ((ClientNature)client.ClientNature).GetDescription(),
                ServiceType = client.ServiceType.GetDescription(),
                isNormal = client.IsNormal,

                UnPayExchangeAmount = client.UnPayExchangeAmount.HasValue ? client.UnPayExchangeAmount.Value : 0,
                DeclareAmount = client.DeclareAmount.HasValue ? client.DeclareAmount.Value : 0,
                PayExchangeAmount = client.PayExchangeAmount.HasValue ? client.PayExchangeAmount.Value : 0,
            };

            this.Paging(clients, convert);
        }
    }
}
