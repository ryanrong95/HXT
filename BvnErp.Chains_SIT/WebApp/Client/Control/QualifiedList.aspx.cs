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
    public partial class Qualified : Uc.PageBase
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadComboBoxData();
        }

        protected void LoadComboBoxData()
        {

            var ServiceIDs = Needs.Wl.Admin.Plat.AdminPlat.Current.Permissions.AdminRoles.Where(manager => manager.Role.Name == "业务员").Select(item => item.Admin.ID).ToArray();
            this.Model.ServiceManager = Needs.Wl.Admin.Plat.AdminPlat.Admins.Where(x => ServiceIDs.Contains(x.ID)).Select(item => new { Key = item.ID, Value = item.RealName }).ToArray().Json();

            this.Model.CurrentName = Needs.Wl.Admin.Plat.AdminPlat.Current.RealName;
            this.Model.DepartmentType = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.DepartmentType>()
                                            .Where(t => t.Value.Contains("业务"))
                                            .Select(item => new { Key = item.Value, item.Value }).Json();
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
            // string Status = Request.QueryString["Status"];
            //  var ClaimStatus = Request.QueryString["ClaimStatus"];
            string Servicemanager = Request.QueryString["Servicemanager"];
            //var ReturnedStatus = Request.QueryString["ReturnedStatus"];
            //var IsSAEleUpload = Request.QueryString["IsSAEleUpload"];
            var DepartmentType = Request.QueryString["DepartmentType"];

            var clientsView = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.SuperAdminClientsView;

            var clients = clientsView.AsQueryable().Where(x => x.Status == Needs.Ccs.Services.Enums.Status.Normal
            && x.ClientStatus == ClientStatus.Confirmed
            && x.ServiceType != ServiceType.Warehouse
            && x.IsQualified == true);

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
            //if (!string.IsNullOrEmpty(Status))
            //{
            //    int status = Int32.Parse(Status);
            //    clients = clients.Where(t => t.ClientStatus == (Needs.Ccs.Services.Enums.ClientStatus)status);
            //}
            //if (Convert.ToBoolean(ClaimStatus))
            //{
            //    clients = clients.Where(x => x.ServiceManager.RealName == null && x.ClientStatus == ClientStatus.Auditing);
            //}

            if (!string.IsNullOrEmpty(Servicemanager))
            {
                clients = clients.Where(x => x.ServiceManager.ID == Servicemanager);
            }
            //if (Convert.ToBoolean(ReturnedStatus))
            //{
            //    clients = clientsView.SearchByReturned(clients);
            //}

            //if (Convert.ToBoolean(IsSAEleUpload))
            //{
            //    clients = clientsView.SearchByIsSAEleUpload(clients, !Convert.ToBoolean(IsSAEleUpload));
            //}

            clients = clientsView.SearchAddServiceManagerDepart(clients, DepartmentType);

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
                MerchandiserName = client.Merchandiser?.RealName,
                ReferrerName = client.Referrer,
                ServiceManagerName = client.ServiceManager?.RealName,
                RegisterYear = client.RegisterYear,
                RegisterDays = (DateTime.Now - client.CreateDate).Days,
                IsSpecified = client.IsSpecified,
                IsValid = client.IsValid,
                IsStorageValid = client.IsStorageValid,
                client.Summary,
                ServiceType = client.ServiceType.GetDescription(),
                DepartmentCode = client.DepartmentCode,
                client.IsQualified,
                AssessDate = client.AssessDate.HasValue? client.AssessDate.Value.ToString("yyyy-MM-dd") : ""
            };

            this.Paging(clients, convert);
        }




        protected void Delete()
        {
            string id = Request.Form["ID"];
            var entity = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.SuperAdminClientsView[id];
            entity.IsQualified = false;
            if (entity != null)
            {
                entity.AbandonSuccess += Express_AbandonSuccess;
                entity.SetQualified();
            }
        }

        /// <summary>
        /// 删除成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Express_AbandonSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Alert("移除成功!");
        }

        /// <summary>
        /// 修改评估日期
        /// </summary>
        protected void SaveAssessDate()
        {
            try
            {
                string ClientID = Request.Form["ClientID"];
                string AssessDate = Request.Form["AssessDate"];

                var entity = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.SuperAdminClientsView[ClientID];
                entity.AssessDate = DateTime.Parse(AssessDate);
                if (entity != null)
                {
                    entity.SetAssessDate();
                }

                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "修改评估日期失败：" + ex.Message }).Json());
            }

        }

    }
}
