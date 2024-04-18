using Needs.Ccs.Services.Enums;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Client.Claim
{
    /// <summary>
    /// 非会员认领
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

            var clients = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.SuperAdminClientsView.Where(x => x.ServiceManager.RealName == null && x.ClientStatus == ClientStatus.Auditing).AsQueryable();


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
                StatusValue = client.ClientStatus,
                ServiceManagerName = client.ServiceManager?.RealName,
                MerchandiserName = client.Merchandiser?.RealName,
                RegisterYear = client.RegisterYear,
            };

            this.Paging(clients, convert);
        }


        /// <summary>
        /// 业务员认领客户
        /// </summary>
        protected void ToDeal()
        {
            try
            {
                string id = Request.Form["ID"];
                var client = Needs.Wl.Admin.Plat.AdminPlat.Clients[id];
                var adminid = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
                client.ServiceManager = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create((string)adminid);
                //TODO：ryan 20211011 当前默认是报关业务，后期若需要，增加认领时选择业务的功能
                client.ServiceType = ServiceType.Customs;
                client.ClientSetServiceManager(client.ServiceManager, "");
                Response.Write((new { success = true, message = "操作成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "操作失败：" + ex.Message }).Json());
            }
        }

    }
}
