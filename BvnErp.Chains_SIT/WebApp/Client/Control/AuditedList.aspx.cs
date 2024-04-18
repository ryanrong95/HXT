using Needs.Ccs.Services.Enums;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Client.Control
{
    /// <summary>
    /// 会员（客户）-客服人员/业务员分配-查询界面
    /// Add by Merlin@20181103
    /// </summary>
    public partial class AuditedList : Uc.PageBase
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
            var status = EnumUtils.ToDictionary<ClientControlStatus>().Select(item => new { item.Key, item.Value });
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

            var clients = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientControlsView.AsQueryable().Where(x=> x.Status == Needs.Ccs.Services.Enums.Status.Normal);
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
                clients = clients.Where(t => t.ClientControlStatus == (Needs.Ccs.Services.Enums.ClientControlStatus)status);
            }

            Func<Needs.Ccs.Services.Models.ClientControl, object> convert = client => new
            {
                client.ID,
                client.ClientID,
                CompanyCode = client.Company.Code,
                CompanyName = client.Company.Name,
                CustomsCode = client.Company.CustomsCode,
                ClientCode = client.ClientCode,
                ClientRankValue = client.ClientRank,
                ClientRank = client.ClientRank.GetDescription(),
                ContactName = client.Company.Contact.Name,
                ContactTel = client.Company.Contact.Mobile,
                CreateDate = client.CreateDate.ToShortDateString(),
                Status = client.ClientControlStatus.GetDescription(),
                StatusValue= client.ClientControlStatus,
                ServiceManagerName = client.ServiceManager?.RealName,
                StorageServiceManagerName=client.StorageServiceManager?.RealName,
                MerchandiserName = client.Merchandiser?.RealName,
            };

            this.Paging(clients, convert);
        }
    }
}