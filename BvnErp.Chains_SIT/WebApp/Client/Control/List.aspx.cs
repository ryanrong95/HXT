using Needs.Ccs.Services.Enums;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.App_Utils;

namespace WebApp.Client.Control
{
    /// <summary>
    /// 会员（客户）-客服人员/业务员分配-查询界面
    /// </summary>
    public partial class List : Uc.PageBase
    {
        private string URL = System.Configuration.ConfigurationManager.AppSettings["CrmUrl"];
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

            //客户等级
            this.Model.ClientRanks = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.ClientRank>().Select(item => new { item.Key, item.Value }).Json();

            this.Model.RealName = Needs.Wl.Admin.Plat.AdminPlat.Current.RealName;
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

            var clients = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.SuperAdminClientsView.AsQueryable().Where(x => (x.ClientStatus == ClientStatus.Verifying) && x.Status == Needs.Ccs.Services.Enums.Status.Normal);
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
                //StorageServiceManagerName = client.StorageServiceManager?.RealName,
                MerchandiserName = client.Merchandiser?.RealName,
                DepartmentCode = client.DepartmentCode,
                RegisterYear = client.RegisterYear,
            };

            this.Paging(clients, convert);
        }

        /// <summary>
        /// 风控修改会员等级
        /// </summary>
        protected void EditRank()
        {
            try
            {
                var id = Request.Form["ID"];
                var rank = Convert.ToInt16(Request.Form["Rank"]);
                var client = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView[id];
                client.ChangeRank(client.ClientRank, (ClientRank)rank);
                //调用CRM接口
                string requestUrl = URL + "/CrmUnify/SetGrade?EnterpriseName=" + client.Company.Name + "&Grade=" + rank;
                HttpResponseMessage response = new HttpResponseMessage();
                response = new HttpClientHelp().HttpClient("POST", requestUrl);
                if (response == null || response.StatusCode != HttpStatusCode.OK)
                {
                    Response.Write((new { success = false, message = "请求CRM会员接口SetGrade失败：" }).Json());
                    return;
                }
                Response.Write((new { success = true, message = "提交成功" }).Json());
            }
            catch (Exception ex)
            {

                Response.Write((new { success = false, message = ex.Message }).Json());
            }


        }
    }
}
