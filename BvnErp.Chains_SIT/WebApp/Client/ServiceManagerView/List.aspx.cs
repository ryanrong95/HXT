using Needs.Ccs.Services.Enums;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace WebApp.Client.ServiceManagerView
{
    public partial class List : Uc.PageBase
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            //LoadComboBoxData();
        }

        /// <summary>
        /// 初始化下拉框数据
        /// </summary>
        protected void LoadComboBoxData()
        {
            var status = EnumUtils.ToDictionary<ClientStatus>().Select(item => new { item.Key, item.Value });
            this.Model.Status = status.Json();

            //var s = new Needs.Ccs.Services.Models.OrderBill();
            //s.ToPDF();
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
            var adminid = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
            var isLeader = new Needs.Ccs.Services.Views.DepartmentView().Any(item => item.LeaderID.Contains(adminid));
            IQueryable<Needs.Ccs.Services.Models.Client> clients = null;
            if (isLeader)
            {
                clients = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView.Where(item => item.Department.LeaderID == adminid && item.ClientStatus == ClientStatus.Confirmed);
            }
            else
            {
                clients = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyClients.Where(item => item.ClientStatus == ClientStatus.Confirmed).AsQueryable();
            }
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

            Func<Needs.Ccs.Services.Models.Client, object> convert = client => new
            {
                client.ID,
                ClientAdminID = adminid,
                CompanyCode = client.Company.Code,
                CompanyName = client.Company.Name,
                ClientRankValue = client.ClientRank,
                ClientRank = client.ClientRank.GetDescription(),
                CustomsCode = client.Company.CustomsCode,
                ClientCode = client.ClientCode,
                ContactName = client.Company.Contact.Name,
                ContactTel = client.Company.Contact.Mobile,
                CreateDate = client.CreateDate.ToShortDateString(),
                ServiceManagerName = client.ServiceManager.RealName,
                MerchandiserName = client.Merchandiser?.RealName,

                //StorageMerchandiserName = client.StorageMerchandiser?.RealName,
                RegisterYear = client.RegisterYear,
                Status = client.ClientStatus.GetDescription(),
                statusValue = client.ClientStatus,
                isModify = client.IsModify,
                client.Summary,
                ServiceType = client.ServiceType,
                ClientNature = ((ClientNature)client.ClientNature).GetDescription(),
                ServiceTypeDes = client.ServiceType.GetDescription(),
                isNormal = client.IsNormal,
                client.Referrer
            };

            this.Paging(clients, convert);
        }



        protected void Submit()
        {
            try
            {
                string id = Request.Form["ID"];
                var entity = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyClients[id];
                var summary = string.Empty;
                if (entity != null)
                {
                    entity.EnterSuccess += ClientStatus_EnterSuccess;
                    entity.EnterError += ClientStatus_EnterError;
                    var ErmAdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ErmAdminID;
                    entity.Submit(ErmAdminID);
                }
            }
            catch (Exception ex)
            {
            }


        }

        protected void SetNormal()
        {
            string ClientID = Request.Form["ClientID"];
            Boolean IsNormal = Convert.ToBoolean(Request.Form["IsNormal"]);

            try
            {
                var client = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyClients.Where(t => t.ID == ClientID).FirstOrDefault();
                client.SetNormal(!IsNormal);
                Response.Write((new { success = true, message = "更改成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = ex.ToString() }).Json());
            }
        }
        /// <summary>
        /// 保存异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientStatus_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write((new { success = false, message = e.Message }).Json());
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientStatus_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "提交成功", ID = e.Object }).Json());
        }
    }
}
