using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class ClientsView : UniqueView<Models.Client, ScCustomsReponsitory>//, Needs.Underly.IFkoView<Models.Client>
    {
        public ClientsView()
        {
        }

        public ClientsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Client> GetIQueryable()
        {
            var companyView = new CompaniesView(this.Reponsitory);
            var adminsView = new AdminsTopView(this.Reponsitory);
            var clientAdminsView = new ClientAdminsView(this.Reponsitory);
            var separtmentView = new DepartmentView(this.Reponsitory);

            return from client in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>()
                   join company in companyView on client.CompanyID equals company.ID
                   join admin in adminsView on client.AdminID equals admin.ID into g
                   from admin in g.DefaultIfEmpty()
                   join clientAdmin in clientAdminsView.Where(t => t.Type == Enums.ClientAdminType.Merchandiser && t.Status == Enums.Status.Normal) on client.ID equals clientAdmin.ClientID into clientAdmins
                   from cadmin in clientAdmins.DefaultIfEmpty()

                   join saleAdmin in clientAdminsView.Where(t => t.Type == Enums.ClientAdminType.ServiceManager && t.Status == Enums.Status.Normal) on client.ID equals saleAdmin.ClientID into saleAdmins
                   from sale in saleAdmins.DefaultIfEmpty()

                       //引荐人 by 2020-12-07 yeshuangshuang
                       //join referrerAdmin in clientAdminsView.Where(t => t.Type == Enums.ClientAdminType.Referrer && t.Status == Enums.Status.Normal) on client.ID equals referrerAdmin.ClientID into referrerAdmins
                       //from referrer in referrerAdmins.DefaultIfEmpty()

                   join depart in separtmentView on sale.Admin.DepartmentID equals depart.ID into depart_temp
                   from depart in depart_temp.DefaultIfEmpty()


                   orderby client.CreateDate descending
                   select new Models.Client
                   {
                       ID = client.ID,
                       Company = company,
                       ClientType = (Enums.ClientType)client.ClientType,
                       ClientCode = client.ClientCode,
                       ClientRank = (Enums.ClientRank)client.ClientRank,
                       Admin = admin,
                       ClientStatus = (Enums.ClientStatus)client.ClientStatus,
                       Status = (Enums.Status)client.Status,
                       CreateDate = client.CreateDate,
                       UpdateDate = client.UpdateDate,
                       Summary = client.Summary,
                       IsSpecified = client.IsSpecified,
                       ClientNature = client.ClientNature == null ? (int)ClientNature.Trade : (int)client.ClientNature,
                       ServiceType = client.ServiceType == null ? ServiceType.Unknown : (ServiceType)client.ServiceType,
                       StorageType = client.StorageType == null ? StorageType.Unknown : (StorageType)client.StorageType,
                       IsStorageValid = client.IsStorageValid,
                       IsValid = client.IsValid,
                       ChargeWH = (ChargeWHType)client.ChargeWH,
                       ChargeType = client.ChargeType != null ? (ChargeType?)client.ChargeType : null,
                       AmountWH = client.AmountWH,
                       Merchandiser = new Models.Admin
                       {
                           ID = cadmin.Admin.ID,
                           RealName = cadmin.Admin.RealName,
                           Tel = cadmin.Admin.Tel,
                           Email = cadmin.Admin.Email,
                           Mobile = cadmin.Admin.Mobile
                       },
                       ServiceManager = new Models.Admin
                       {
                           ID = sale.Admin.ID,
                           RealName = sale.Admin.RealName,
                           Tel = sale.Admin.Tel,
                           Email = sale.Admin.Email,
                           Mobile = sale.Admin.Mobile
                       },
                       //Referrer = new Models.Admin
                       //{
                       //    ID = referrer.Admin.ID,
                       //    RealName = referrer.Admin.RealName,
                       //    Tel = referrer.Admin.Tel,
                       //    Email = referrer.Admin.Email,
                       //    Mobile = referrer.Admin.Mobile
                       //},
                       Referrer = client.Referrer,
                       IsNormal = client.IsNormal,
                       DepartmentCode = depart != null ? depart.Name : null,
                       IsQualified = client.IsQualified,
                       Department = depart,
                       AssessDate = client.AssessDate,

                       IsDownloadDecTax = client.IsDownloadDecTax,
                       DecTaxExtendDate = client.DecTaxExtendDate,
                       IsApplyInvoice = client.IsApplyInvoice,
                       InvoiceExtendDate = client.InvoiceExtendDate,

                       UnPayExchangeAmount = client.UnPayExchangeAmount,
                       UnPayExchangeAmount4M = client.UnPayExchangeAmount4M,
                       DeclareAmount = client.DeclareAmount,
                       PayExchangeAmount = client.PayExchangeAmount,
                       DeclareAmountMonth = client.DeclareAmountMonth,
                       PayExchangeAmountMonth = client.PayExchangeAmountMonth
                   };
        }
    }

    public class SuperAdminClientsView : ClientsView
    {
        public SuperAdminClientsView()
        {

        }

        protected override IQueryable<Client> GetIQueryable()
        {
            return base.GetIQueryable();
        }

        /// <summary>
        /// 增加部门 Code 查询，并且 DepartmentCode 是查询条件
        /// </summary>
        /// <param name="clients"></param>
        /// <param name="departmentCode"></param>
        /// <returns></returns>
        public IQueryable<Client> SearchAddServiceManagerDepart(IQueryable<Client> clients, string departmentCode)
        {
            var XDTStaffsTopView = new XDTStaffsTopView(this.Reponsitory);
            var clientAdminsView = new ClientAdminsView(this.Reponsitory);
            var separtmentView = new DepartmentView(this.Reponsitory);

            var linq = from client in clients
                       join saleAdmin in clientAdminsView.Where(t => t.Type == Enums.ClientAdminType.ServiceManager && t.Status == Enums.Status.Normal)
                          on client.ID equals saleAdmin.ClientID into saleAdmins
                       from sale in saleAdmins.DefaultIfEmpty()

                       join depart in separtmentView on sale.Admin.DepartmentID equals depart.ID into depart_temp
                       from depart in depart_temp.DefaultIfEmpty()

                           //join StoragesaleAdmin in clientAdminsView.Where(t => t.Type == Enums.ClientAdminType.StorageServiceManager && t.Status == Enums.Status.Normal)
                           //    on client.ID equals StoragesaleAdmin.ClientID into StoragesaleAdmins
                           // from Storagesale in StoragesaleAdmins.DefaultIfEmpty()
                           // join Storagedepart in separtmentView on Storagesale.Admin.DepartmentID equals Storagedepart.ID into Storagedepart_temp
                           // from Storagedepart in Storagedepart_temp.DefaultIfEmpty()
                       select new Client
                       {
                           ID = client.ID,
                           Company = client.Company,
                           ClientType = client.ClientType,
                           ClientCode = client.ClientCode,
                           ClientRank = client.ClientRank,
                           Admin = client.Admin,
                           ClientStatus = client.ClientStatus,
                           Status = client.Status,
                           CreateDate = client.CreateDate,
                           UpdateDate = client.UpdateDate,
                           Summary = client.Summary,
                           IsSpecified = client.IsSpecified,
                           ClientNature = client.ClientNature,
                           ServiceType = client.ServiceType,
                           StorageType = client.StorageType,
                           IsStorageValid = client.IsStorageValid,
                           IsValid = client.IsValid,
                           ChargeWH = client.ChargeWH,
                           Merchandiser = client.Merchandiser,
                           ServiceManager = client.ServiceManager,
                           Referrer = client.Referrer,
                           DepartmentCode = depart != null ? depart.Name : null,
                           //StorageDepartmentCode = Storagedepart != null ? Storagedepart.Name : null,
                           // DepartmentCode = staff != null ? staff.DepartmentCode : null,
                           IsNormal = client.IsNormal,
                           IsQualified = client.IsQualified,
                           AssessDate = client.AssessDate,
                           UnPayExchangeAmount = client.UnPayExchangeAmount,
                           DeclareAmount = client.DeclareAmount,
                           PayExchangeAmount = client.PayExchangeAmount,
                           DeclareAmountMonth = client.DeclareAmountMonth,
                           PayExchangeAmountMonth = client.PayExchangeAmountMonth
                       };

            if (!string.IsNullOrEmpty(departmentCode))
            {
                //通过 departmentCode 过滤
                linq = linq.Where(t => t.DepartmentCode == departmentCode);
            }

            return linq;
        }

        public IQueryable<Client> SearchByReturned(IQueryable<Client> clients)
        {

            var clientLosView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientLogs>();
            var returnedID = clientLosView.Where(t => t.Summary.Contains("不通过")).Select(t => t.ClientID).ToArray();

            return clients.Where(t => returnedID.Contains(t.ID));
        }

        /// <summary>
        /// 根据电子版是否上传查询
        /// </summary>
        /// <param name="isSAEleUpload"></param>
        /// <returns></returns>
        public IQueryable<Client> SearchByIsSAEleUpload(IQueryable<Client> clients, bool isSAEleUpload)
        {
            var filesDescriptionTopView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FilesDescriptionTopView>();

            var allUploadedClientIDs = filesDescriptionTopView
                .Where(t => t.Status == (int)Enums.Status.Normal && t.Type == (int)Enums.FileType.ServiceAgreement && !t.CustomName.Contains(".docx"))
                .Select(t => t.ClientID);

            if (isSAEleUpload)
            {
                clients = clients.Where(t => allUploadedClientIDs.Contains(t.ID));
            }
            else
            {
                clients = clients.Where(t => !allUploadedClientIDs.Contains(t.ID));
            }

            return clients;
        }
    }


    public class APIClientsView : UniqueView<Models.Client, ScCustomsReponsitory>
    {
        public APIClientsView()
        {
        }

        public APIClientsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Client> GetIQueryable()
        {
            var companyView = new CompaniesView(this.Reponsitory);
            var adminsView = new AdminsTopView(this.Reponsitory);
            var clientAdminsView = new ClientAdminsView(this.Reponsitory);

            return from client in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>()
                   join company in companyView on client.CompanyID equals company.ID
                   join clientFile in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientFiles>() on client.ID equals clientFile.ClientID
                   join admin in adminsView on client.AdminID equals admin.ID
                   join clientAdmin in clientAdminsView.Where(t => t.Type == Enums.ClientAdminType.Merchandiser && t.Status == Enums.Status.Normal) on client.ID equals clientAdmin.ClientID into clientAdmins
                   from cadmin in clientAdmins.DefaultIfEmpty()
                   join saleAdmin in clientAdminsView.Where(t => t.Type == Enums.ClientAdminType.ServiceManager && t.Status == Enums.Status.Normal) on client.ID equals saleAdmin.ClientID into saleAdmins
                   from sale in saleAdmins.DefaultIfEmpty()
                   orderby client.CreateDate descending
                   select new Models.Client
                   {
                       ID = client.ID,
                       Company = company,
                       ClientType = (Enums.ClientType)client.ClientType,
                       ClientCode = client.ClientCode,
                       ClientRank = (Enums.ClientRank)client.ClientRank,
                       Admin = admin,
                       ClientStatus = (Enums.ClientStatus)client.ClientStatus,
                       Status = (Enums.Status)client.Status,
                       CreateDate = client.CreateDate,
                       UpdateDate = client.UpdateDate,
                       IsSpecified = client.IsSpecified,

                       Summary = client.Summary,
                       ClientFile = new Models.ClientFile
                       {
                           ClientID = clientFile.ClientID,
                           FileFormat = clientFile.FileFormat,
                           FileType = (Enums.FileType)clientFile.FileType,
                           Name = clientFile.Name,
                           Url = clientFile.Url
                       },
                       Merchandiser = new Models.Admin
                       {
                           ID = cadmin.Admin.ID,
                           RealName = cadmin.Admin.RealName,
                           Tel = cadmin.Admin.Tel,
                           Email = cadmin.Admin.Email,
                           Mobile = cadmin.Admin.Mobile
                       },
                       ServiceManager = new Models.Admin
                       {
                           ID = sale.Admin.ID,
                           RealName = sale.Admin.RealName,
                           Tel = sale.Admin.Tel,
                           Email = sale.Admin.Email,
                           Mobile = sale.Admin.Mobile
                       },
                   };
        }
    }


    public class ClientControlsView : UniqueView<Models.ClientControl, ScCustomsReponsitory>
    {
        public ClientControlsView()
        {
        }

        public ClientControlsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ClientControl> GetIQueryable()
        {
            var companyView = new CompaniesView(this.Reponsitory);
            var adminsView = new AdminsTopView(this.Reponsitory);
            var clientAdminsView = new ClientAdminsView(this.Reponsitory);

            return from control in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientControls>()
                   join company in companyView on control.CompanyID equals company.ID
                   join admin in adminsView on control.AdminID equals admin.ID into g
                   from admin in g.DefaultIfEmpty()
                   join clientAdmin in clientAdminsView.Where(t => t.Type == Enums.ClientAdminType.Merchandiser && t.Status == Enums.Status.Normal) on control.ClientID equals clientAdmin.ClientID into clientAdmins
                   from cadmin in clientAdmins.DefaultIfEmpty()
                   join saleAdmin in clientAdminsView.Where(t => t.Type == Enums.ClientAdminType.ServiceManager && t.Status == Enums.Status.Normal) on control.ClientID equals saleAdmin.ClientID into saleAdmins
                   from sale in saleAdmins.DefaultIfEmpty()

                   join StoragesaleAdmin in clientAdminsView.Where(t => t.Type == Enums.ClientAdminType.StorageServiceManager && t.Status == Enums.Status.Normal) on control.ClientID equals StoragesaleAdmin.ClientID into StoragesaleAdmins
                   from Storagesale in StoragesaleAdmins.DefaultIfEmpty()
                   orderby control.CreateDate descending
                   select new Models.ClientControl
                   {
                       ID = control.ID,
                       ClientID = control.ClientID,
                       Company = company,
                       ClientType = (Enums.ClientType)control.ClientType,
                       ClientCode = control.ClientCode,
                       ClientRank = (Enums.ClientRank)control.ClientRank,
                       Admin = admin,
                       ClientControlStatus = (Enums.ClientControlStatus)control.ClientStatus,
                       Status = (Enums.Status)control.Status,
                       CreateDate = control.CreateDate,
                       UpdateDate = control.UpdateDate,
                       Summary = control.Summary,
                       Merchandiser = new Models.Admin
                       {
                           ID = cadmin.Admin.ID,
                           RealName = cadmin.Admin.RealName,
                           Tel = cadmin.Admin.Tel,
                           Email = cadmin.Admin.Email,
                           Mobile = cadmin.Admin.Mobile
                       },
                       ServiceManager = new Models.Admin
                       {
                           ID = sale.Admin.ID,
                           RealName = sale.Admin.RealName,
                           Tel = sale.Admin.Tel,
                           Email = sale.Admin.Email,
                           Mobile = sale.Admin.Mobile
                       },
                       StorageServiceManager = new Models.Admin
                       {
                           ID = Storagesale.Admin.ID,
                           RealName = Storagesale.Admin.RealName,
                           Tel = Storagesale.Admin.Tel,
                           Email = Storagesale.Admin.Email,
                           Mobile = Storagesale.Admin.Mobile
                       },
                   };
        }
    }


    /// <summary>
    /// 风控管理超期未付汇客户列表
    /// </summary>
    public class PayExControlListView : UniqueView<Models.Client, ScCustomsReponsitory>
    {
        public PayExControlListView()
        {
        }

        public PayExControlListView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Client> GetIQueryable()
        {
            var companyView = new CompaniesView(this.Reponsitory);
            var adminsView = new AdminsTopView(this.Reponsitory);
            var clientAdminsView = new ClientAdminsView(this.Reponsitory);

            return from client in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>()
                   join company in companyView on client.CompanyID equals company.ID

                   join clientAdmin in clientAdminsView.Where(t => t.Type == Enums.ClientAdminType.Merchandiser && t.Status == Enums.Status.Normal) on client.ID equals clientAdmin.ClientID into clientAdmins
                   from cadmin in clientAdmins.DefaultIfEmpty()
                   join saleAdmin in clientAdminsView.Where(t => t.Type == Enums.ClientAdminType.ServiceManager && t.Status == Enums.Status.Normal) on client.ID equals saleAdmin.ClientID into saleAdmins
                   from sale in saleAdmins.DefaultIfEmpty()
                   where client.IsValid == true
                   select new Models.Client
                   {
                       ID = client.ID,
                       Company = company,
                       ClientType = (Enums.ClientType)client.ClientType,
                       ClientCode = client.ClientCode,
                       ClientRank = (Enums.ClientRank)client.ClientRank,
                       //Status = (Enums.Status)client.Status,
                       ClientStatus = (Enums.ClientStatus)client.ClientStatus,
                       CreateDate = client.CreateDate,
                       UpdateDate = client.UpdateDate,
                       Summary = client.Summary,
                       Merchandiser = new Models.Admin
                       {
                           ID = cadmin.Admin.ID,
                           RealName = cadmin.Admin.RealName,
                           Tel = cadmin.Admin.Tel,
                           Email = cadmin.Admin.Email,
                           Mobile = cadmin.Admin.Mobile
                       },
                       ServiceManager = new Models.Admin
                       {
                           ID = sale.Admin.ID,
                           RealName = sale.Admin.RealName,
                           Tel = sale.Admin.Tel,
                           Email = sale.Admin.Email,
                           Mobile = sale.Admin.Mobile
                       },
                       IsNormal = client.IsNormal,
                       IsDownloadDecTax = client.IsDownloadDecTax,
                       DecTaxExtendDate = client.DecTaxExtendDate,
                       IsApplyInvoice = client.IsApplyInvoice,
                       InvoiceExtendDate = client.InvoiceExtendDate
                   };
        }
    }


    /// <summary>
    /// 业务员列表，弹框提示超期未付汇客户
    /// </summary>
    public class PayExchangeExceedView : UniqueView<Models.Client, ScCustomsReponsitory>
    {
        public PayExchangeExceedView()
        {
        }

        public PayExchangeExceedView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Client> GetIQueryable()
        {
            var companyView = new CompaniesView(this.Reponsitory);
            var adminsView = new AdminsTopView(this.Reponsitory);
            var clientAdminsView = new ClientAdminsView(this.Reponsitory);

            return from client in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>()
                   join company in companyView on client.CompanyID equals company.ID

                   join clientAdmin in clientAdminsView.Where(t => t.Type == Enums.ClientAdminType.Merchandiser && t.Status == Enums.Status.Normal) on client.ID equals clientAdmin.ClientID into clientAdmins
                   from cadmin in clientAdmins.DefaultIfEmpty()
                   join saleAdmin in clientAdminsView.Where(t => t.Type == Enums.ClientAdminType.ServiceManager && t.Status == Enums.Status.Normal) on client.ID equals saleAdmin.ClientID into saleAdmins
                   from sale in saleAdmins.DefaultIfEmpty()
                   where client.IsValid == true
                   select new Models.Client
                   {
                       ID = client.ID,
                       Company = company,
                       ClientType = (Enums.ClientType)client.ClientType,
                       ClientCode = client.ClientCode,
                       ClientRank = (Enums.ClientRank)client.ClientRank,
                       //Status = (Enums.Status)client.Status,
                       ClientStatus = (Enums.ClientStatus)client.ClientStatus,
                       CreateDate = client.CreateDate,
                       UpdateDate = client.UpdateDate,
                       Summary = client.Summary,
                       Merchandiser = new Models.Admin
                       {
                           ID = cadmin.Admin.ID,
                           RealName = cadmin.Admin.RealName,
                           Tel = cadmin.Admin.Tel,
                           Email = cadmin.Admin.Email,
                           Mobile = cadmin.Admin.Mobile
                       },
                       ServiceManager = new Models.Admin
                       {
                           ID = sale.Admin.ID,
                           RealName = sale.Admin.RealName,
                           Tel = sale.Admin.Tel,
                           Email = sale.Admin.Email,
                           Mobile = sale.Admin.Mobile
                       },
                       IsNormal = client.IsNormal,
                       IsDownloadDecTax = client.IsDownloadDecTax,
                       DecTaxExtendDate = client.DecTaxExtendDate,
                       IsApplyInvoice = client.IsApplyInvoice,
                       InvoiceExtendDate = client.InvoiceExtendDate,
                       UnPayExchangeAmount = client.UnPayExchangeAmount,
                       DeclareAmount = client.DeclareAmount,
                       PayExchangeAmount = client.PayExchangeAmount,
                       DeclareAmountMonth = client.DeclareAmountMonth,
                       PayExchangeAmountMonth = client.PayExchangeAmountMonth
                   };
        }
    }
}
