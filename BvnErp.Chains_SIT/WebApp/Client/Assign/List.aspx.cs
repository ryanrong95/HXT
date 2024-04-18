using Needs.Ccs.Services.Enums;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Client.Assign
{
    /// <summary>
    /// 会员（客户）-客服人员/业务员分配-查询界面
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
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        protected void data()
        {
            string CompanyName = Request.QueryString["CompanyName"];
            string ClientCode = Request.QueryString["ClientCode"];
            string CreateDateFrom = Request.QueryString["CreateDateFrom"];
            string CreateDateTo = Request.QueryString["CreateDateTo"];
            string Status = Request.QueryString["Status"];

            var clients = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.SuperAdminClientsView.AsQueryable().Where(x=>x.Status== Needs.Ccs.Services.Enums.Status.Normal);
            if (!string.IsNullOrEmpty(CompanyName))
            {
                clients = clients.Where(t => t.Company.Name.Contains(CompanyName.Trim()));
            }
            if (!string.IsNullOrEmpty(ClientCode))
            {
                clients = clients.Where(t => t.ClientCode.Contains(ClientCode.Trim()));
            }
            if (!string.IsNullOrEmpty(CreateDateFrom))
            {
                var from = DateTime.Parse(CreateDateFrom);
                clients = clients.Where(t => t.CreateDate >= from);
            }
            if (!string.IsNullOrEmpty(CreateDateTo))
            {
                var to = DateTime.Parse(CreateDateTo).AddDays(1); ;
                clients = clients.Where(t => t.CreateDate < to);
            }
            if (!string.IsNullOrEmpty(Status))
            {
                int status = Int32.Parse(Status);
                clients = clients.Where(t => t.ClientStatus == (Needs.Ccs.Services.Enums.ClientStatus)status);
            }

            Func<Needs.Ccs.Services.Models.Client, object> convert = client => new
            {
                client.ID,
                CompanyCode = client.Company.Code,
                CompanyName = client.Company.Name,
                CustomsCode = client.Company.CustomsCode,
                ClientCode = client.ClientCode,
                ClientRankValue = client.ClientRank,
                ClientRank = client.ClientRank.GetDescription(),
                ContactName = client.Company.Contact.Name,
                ContactTel = client.Company.Contact.Mobile,
                CreateDate = client.CreateDate.ToShortDateString(),
                Status = client.ClientStatus.GetDescription(),
                ServiceManagerName = client.ServiceManager?.RealName,
                MerchandiserName = client.Merchandiser?.RealName,
                RegisterYear = client.RegisterYear,
            };

            this.Paging(clients, convert);
        }
    }
}