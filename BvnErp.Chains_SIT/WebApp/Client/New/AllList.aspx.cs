using Needs.Ccs.Services.Enums;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace WebApp.Client.New
{
    public partial class AllList : Uc.PageBase
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

            //var s = new Needs.Ccs.Services.Models.OrderBill();
            //s.ToPDF();
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        protected void data()
        {
            // private string isModify = System.Configuration.ConfigurationManager.AppSettings["AEO"];
            string CompanyName = Request.QueryString["CompanyName"];
            string ClientCode = Request.QueryString["ClientCode"];
            string CreateDateFrom = Request.QueryString["CreateDateFrom"];
            string CreateDateTo = Request.QueryString["CreateDateTo"];
            string NormalType = Request.QueryString["NormalType"];

            var clients = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.PayExControlListView.AsQueryable();            
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

            Func<Needs.Ccs.Services.Models.Client, object> convert = client => new
            {
                client.ID,
                CompanyCode = client.Company.Code,
                CompanyName = client.Company.Name,
                ClientRankValue = client.ClientRank,
                ClientRank = client.ClientRank.GetDescription(),
                ClientCode = client.ClientCode,
                CreateDate = client.CreateDate.ToShortDateString(),
                SalesName = client.ServiceManager?.RealName,
                MerchandiserName = client.Merchandiser?.RealName,
                ClientStatus = client.ClientStatus.GetDescription(),
                statusValue =client.ClientStatus,
                isModify =client.IsModify,
                client.Summary,
                isNormal = client.IsNormal,
                IsDownloadDecTax = client.IsDownloadDecTax,
                DecTaxExtendDate = string.IsNullOrEmpty(client.DecTaxExtendDate) ? "" : DateTime.Parse(client.DecTaxExtendDate).ToShortDateString(),
                IsApplyInvoice = client.IsApplyInvoice,
                InvoiceExtendDate = string.IsNullOrEmpty(client.InvoiceExtendDate) ? "" : DateTime.Parse(client.InvoiceExtendDate).ToShortDateString(),

            };

            this.Paging(clients, convert);
        }

        protected void SetNormal()
        {
            string ClientID = Request.Form["ClientID"];
            Boolean IsNormal = Convert.ToBoolean(Request.Form["IsNormal"]);

            try
            {
                var client = new  Needs.Ccs.Services.Views.ClientsView().Where(t => t.ID == ClientID).FirstOrDefault();
                client.SetNormal(!IsNormal);
                Response.Write((new { success = true, message = "更改成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = ex.ToString() }).Json());
            }
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