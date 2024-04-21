using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace YaHv.Csrm.Services.Extends
{
    public static class ToLinqExtends
    {
        /// <summary>
        /// Enterprise To Linq
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layers.Data.Sqls.PvbCrm.Enterprises ToLinq(this Models.Origins.Enterprise entity)
        {
            return new Layers.Data.Sqls.PvbCrm.Enterprises
            {
                ID = entity.ID,
                Name = entity.Name,
                AdminCode = entity.AdminCode == null ? "" : entity.AdminCode,
                Status = (int)entity.Status,
                District = entity.District,
                RegAddress = entity.RegAddress,
                Corporation = entity.Corporation,
                Uscc = entity.Uscc,
                Place = entity.Place

            };
        }
        /// <summary>
        /// WsClient To Linq
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layers.Data.Sqls.PvbCrm.Clients ToLinq(this Models.Origins.Client entity)
        {
            return new Layers.Data.Sqls.PvbCrm.Clients
            {
                ID = entity.ID,
                // Origin=(int)entity.Origin,
                Nature = (int)entity.Nature,
                AreaType = (int)entity.AreaType,
                Grade = (int?)entity.Grade,
                DyjCode = entity.DyjCode,
                TaxperNumber = entity.TaxperNumber,
                Vip = (int)entity.Vip,
                Status = (int)entity.ClientStatus,
                AdminID = entity.CreatorID,
                CreateDate = entity.CreateDate,
                UpdateDate = DateTime.Now,
                Place = entity.Place,
                Major = entity.Major
            };
        }
        /// <summary>
        /// Company To Linq
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layers.Data.Sqls.PvbCrm.Companies ToLinq(this Models.Origins.Company entity)
        {
            return new Layers.Data.Sqls.PvbCrm.Companies
            {
                ID = entity.ID,
                Type = (int)entity.Type,
                Range = (int)entity.Range,
                Status = (int)entity.CompanyStatus,
                AdminID = entity.CreatorID,
                CreateDate = entity.CreateDate,
                UpdateDate = DateTime.Now
            };
        }

        /// <summary>
        /// Supplier To Linq
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layers.Data.Sqls.PvbCrm.Suppliers ToLinq(this Models.Origins.Supplier entity)
        {
            return new Layers.Data.Sqls.PvbCrm.Suppliers
            {
                ID = entity.ID,
                DyjCode = entity.DyjCode,
                TaxperNumber = entity.TaxperNumber,
                Type = (int)entity.Type,
                Nature = (int)entity.Nature,
                Grade = (int?)entity.Grade,
                AreaType = (int)entity.AreaType,
                InvoiceType = (int?)entity.InvoiceType,
                IsFactory = entity.IsFactory,
                AgentCompany = entity.AgentCompany,
                Status = (int)entity.SupplierStatus,
                RepayCycle = (int)entity.RepayCycle,
                Currency = (int)entity.Currency,
                Price = entity.Price,
                AdminID = entity.CreatorID,
                CreateDate = entity.CreateDate,
                UpdateDate = DateTime.Now,
                Place = entity.Place,
                IsForwarder = entity.IsForwarder
            };
        }
        /// <summary>
        /// Beneficiary To Linq
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layers.Data.Sqls.PvbCrm.Beneficiaries ToLinq(this Models.Origins.Beneficiary entity)
        {
            return new Layers.Data.Sqls.PvbCrm.Beneficiaries
            {
                ID = entity.ID,
                InvoiceType = (int?)entity.InvoiceType,
                EnterpriseID = entity.EnterpriseID,
                RealID = entity.RealID,
                RealName = entity.RealName,
                Bank = entity.Bank,
                BankAddress = entity.BankAddress,
                Account = entity.Account,
                SwiftCode = entity.SwiftCode,
                Methord = (int)entity.Methord,
                Currency = (int)entity.Currency,
                District = (int)entity.District,
                Name = entity.Name,
                Tel = entity.Tel,
                Mobile = entity.Mobile,
                Email = entity.Email,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = DateTime.Now,
                AdminID = entity.CreatorID,
                BankCode = entity.BankCode
            };
        }
        /// <summary>
        /// Contact To Linq
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layers.Data.Sqls.PvbCrm.Contacts ToLinq(this Models.Origins.Contact entity)
        {
            return new Layers.Data.Sqls.PvbCrm.Contacts
            {
                ID = entity.ID,
                EnterpriseID = entity.EnterpriseID,
                Type = (int)entity.Type,
                Name = entity.Name,
                Tel = entity.Tel,
                Mobile = entity.Mobile,
                Email = entity.Email,
                Fax = entity.Fax,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = DateTime.Now,
                AdminID = entity.CreatorID
            };
        }
        /// <summary>
        /// Consignee To Linq
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layers.Data.Sqls.PvbCrm.Consignees ToLinq(this Models.Origins.Consignee entity)
        {
            return new Layers.Data.Sqls.PvbCrm.Consignees
            {
                ID = entity.ID,
                Title = entity.Title,
                EnterpriseID = entity.EnterpriseID,
                DyjCode = entity.DyjCode,
                District = (int)entity.District,
                Address = entity.Address,
                Postzip = entity.Postzip,
                Name = entity.Name,
                Tel = entity.Tel,
                Mobile = entity.Mobile,
                Email = entity.Email,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = DateTime.Now,
                AdminID = entity.CreatorID,
                Province = entity.Province,
                City = entity.City,
                Land = entity.Land,
                PlateCode = entity.PlateCode,
                Place = entity.Place

            };
        }
        /// <summary>
        /// Consignor To Linq
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layers.Data.Sqls.PvbCrm.Consignors ToLinq(this Models.Origins.Consignor entity)
        {
            return new Layers.Data.Sqls.PvbCrm.Consignors
            {
                ID = entity.ID,
                Title = entity.Title,
                EnterpriseID = entity.EnterpriseID,
                DyjCode = entity.DyjCode,
                Province = entity.Province,
                City = entity.City,
                Land = entity.Land,
                Address = entity.Address,
                Postzip = entity.Postzip,
                Name = entity.Name,
                Tel = entity.Tel,
                Mobile = entity.Mobile,
                Email = entity.Email,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = DateTime.Now,
                AdminID = entity.CreatorID,
                IsDefault = entity.IsDefault,
                Place = entity.Place
            };
        }
        /// <summary>
        /// Invoice To Linq
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layers.Data.Sqls.PvbCrm.Invoices ToLinq(this Models.Origins.Invoice entity)
        {
            return new Layers.Data.Sqls.PvbCrm.Invoices
            {
                ID = entity.ID,
                EnterpriseID = entity.EnterpriseID,
                CompanyTel = entity.CompanyTel,
                Bank = entity.Bank,
                BankAddress = entity.BankAddress,
                Type = (int)entity.Type,
                Account = entity.Account,
                TaxperNumber = entity.TaxperNumber,
                District = (int)entity.District,
                Address = entity.Address,
                Postzip = entity.Postzip,
                Name = entity.Name,
                Tel = entity.Tel,
                Mobile = entity.Mobile,
                Email = entity.Email,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = DateTime.Now,
                AdminID = entity.CreatorID,
                Province = entity.Province,
                City = entity.City,
                Land = entity.Land,
                DeliveryType = (int)entity.DeliveryType,
                InvoiceAddress = entity.InvoiceAddress
            };
        }

        /// <summary>
        /// WareHouse To Linq
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layers.Data.Sqls.PvbCrm.WareHouses ToLinq(this Models.Origins.WareHouse entity)
        {
            return new Layers.Data.Sqls.PvbCrm.WareHouses
            {
                ID = entity.ID,
                DyjCode = entity.DyjCode,
                District = (int)entity.District,
                Grade = (int)entity.Grade,
                Address = entity.Address,
                AdminID = entity.CreatorID,
                CreateDate = entity.CreateDate,
                UpdateDate = DateTime.Now,
                Status = (int)entity.Status,
                WsCode = entity.WsCode
            };
        }
        /// <summary>
        /// Customsbroker To Linq
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        //public static layers.data.sqls.pvbcrm.customsbrokers tolinq(this models.origins.customsbroker entity)
        //{
        //    return new layers.data.sqls.pvbcrm.customsbrokers
        //    {
        //        id = entity.id,
        //        admincode = entity.admincode,
        //        name = entity.name,
        //        dyjcode = entity.dyjcode,
        //        grade = (int)entity.grade
        //    };
        //}
        public static Layers.Data.Sqls.PvbCrm.WsClients ToLinq(this Models.Origins.WsClient entity)
        {
            return new Layers.Data.Sqls.PvbCrm.WsClients
            {
                ID = entity.ID,
                Grade = (int)entity.Grade,
                Vip = entity.Vip,
                EnterCode = entity.EnterCode,
                CustomsCode = entity.CustomsCode,
                AdminID = entity.CreatorID,
                CreateDate = entity.CreateDate,
                UpdateDate = DateTime.Now,
                Summary = entity.Summary,
                Status = (int)entity.WsClientStatus,
                Nature = (int)entity.Nature,
                Place = entity.Place,
                ServiceType = (int)entity.ServiceType,
                IsDeclaretion = entity.IsDeclaretion,
                IsStorageService = entity.IsStorageService,
                StorageType = (int)entity.StorageType,
                ChargeWH=(int)entity.ChargeWHType
            };
        }
        public static Layers.Data.Sqls.PvbCrm.WsSuppliers ToLinq(this Models.Origins.WsSupplier entity)
        {
            return new Layers.Data.Sqls.PvbCrm.WsSuppliers
            {
                ID = entity.ID,
                Grade = (int)entity.Grade,
                AdminID = entity.CreatorID,
                Summary = entity.Summary,
                ChineseName = entity.ChineseName,
                EnglishName = entity.EnglishName,
                CreateDate = entity.CreateDate,
                UpdateDate = DateTime.Now,
                Status = (int)entity.WsSupplierStatus,
                Place = entity.Place
            };
        }

        public static Layers.Data.Sqls.PvbCrm.nSuppliers ToLinq(this Models.Origins.nSupplier entity)
        {
            return new Layers.Data.Sqls.PvbCrm.nSuppliers
            {
                ID = entity.ID,
                EnterpriseID = entity.EnterpriseID,
                RealID = entity.RealEnterprise.ID,
                FromID = null,
                ChineseName = entity.ChineseName,
                EnglishName = entity.EnglishName,
                Grade = (int)entity.Grade,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Summary = entity.Summary,
                Creator = entity.Creator,
                Status = (int)entity.Status,
                Conduct = (int)entity.Conduct,
                CHNabbreviation = entity.CHNabbreviation
            };
        }
        public static Layers.Data.Sqls.PvbCrm.FilesDescription ToLinq(this Models.Origins.FileDescription entity)
        {
            return new Layers.Data.Sqls.PvbCrm.FilesDescription
            {
                ID = entity.ID,
                EnterpriseID = entity.EnterpriseID,
                Name = entity.Name,
                Type = (int)entity.Type,
                Url = entity.Url,
                FileFormat = entity.FileFormat,
                Status = (int)entity.Status,
                AdminID = entity.CreatorID,
                Summary = entity.Summary,
                CreateDate = entity.CreateDate,
                PaysID = entity.CompanyID
            };
        }
        public static Layers.Data.Sqls.PvbCrm.Contracts ToLinq(this Models.Origins.Contract entity)
        {
            return new Layers.Data.Sqls.PvbCrm.Contracts
            {
                ID = entity.ID,
                EnterpriseID = entity.Enterprise.ID,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                AgencyRate = entity.AgencyRate,
                MinAgencyFee = entity.MinAgencyFee,
                InvoiceTaxRate = entity.InvoiceTaxRate,
                InvoiceType = (int)entity.InvoiceType,
                ExchangeMode = (int)entity.ExchangeMode,
                Status = (int)entity.Status,
                Creator = entity.CreatorID,
                Summary = entity.Summary,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
            };
        }
        //public static Layers.Data.Sqls.PvbCrm.SiteUsers ToLinq(this Models.Origins.SiteUser entity)
        //{
        //    return new Layers.Data.Sqls.PvbCrm.SiteUsers
        //    {
        //        ID = entity.ID,
        //        EnterpriseID = entity.EnterpriseID,
        //        UserName = entity.UserName,
        //        RealName = entity.RealName,
        //        Password = entity.Password,
        //        //From = entity.From,
        //        Mobile = entity.Mobile,
        //        Email = entity.Email,
        //        QQ = entity.QQ,
        //        Wx = entity.Wx,
        //        IsMain = entity.IsMain,
        //        Summary = entity.Summary,
        //        //AdminID = entity.CreatorID,
        //        CreateDate = entity.CreateDate,
        //        UpdateDate =DateTime.Now,
        //        Status = (int)entity.Status
        //    };
        //}

        /// <summary>
        //
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layers.Data.Sqls.PvbCrm.Carriers ToLinq(this Models.Origins.Carrier entity)
        {
            return new Layers.Data.Sqls.PvbCrm.Carriers
            {
                ID = entity.Enterprise.ID,
                Icon = entity.Icon,
                Code = entity.Code,
                Type = (int)entity.Type,
                Status = (int)entity.Status,
                Creator = entity.CreatorID,
                Summary = entity.Summary,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                IsInternational = entity.IsInternational
            };
        }
        /// <summary>
        /// 运输工具
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layers.Data.Sqls.PvbCrm.Transports ToLinq(this Models.Origins.Transport entity)
        {
            return new Layers.Data.Sqls.PvbCrm.Transports
            {
                ID = entity.ID,
                EnterpriseID = entity.Enterprise.ID,
                Type = (int)entity.Type,
                CarNumber1 = entity.CarNumber1,
                CarNumber2 = entity.CarNumber2,
                Weight = entity.Weight,
                Status = (int)entity.Status,
                Creator = entity.CreatorID,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
            };
        }
        public static Layers.Data.Sqls.PvbCrm.Drivers ToLinq(this Models.Origins.Driver entity)
        {
            return new Layers.Data.Sqls.PvbCrm.Drivers
            {
                ID = entity.ID,
                EnterpriseID = entity.Enterprise.ID,
                Name = entity.Name,
                IDCard = entity.IDCard,
                Mobile = entity.Mobile,
                Status = (int)entity.Status,
                Creator = entity.CreatorID,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Mobile2 = entity.Mobile2,
                CustomsCode = entity.CustomsCode,
                CardCode = entity.CardCode,
                PortCode = entity.PortCode,
                LBPassword = entity.LBPassword,
                IsChcd = entity.IsChcd
            };
        }
        public static Layers.Data.Sqls.PvbCrm.WsContracts ToLinq(this Models.Origins.WsContract entity)
        {
            return new Layers.Data.Sqls.PvbCrm.WsContracts
            {
                ID = entity.ID,
                Trustee = entity.Trustee,
                WsClientID = entity.WsClient.ID,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                Currency = (int)entity.Currency,
                ContainerNum = entity.ContainerNum,
                Charges = entity.Charges,
                Status = (int)entity.Status,
                CreatorID = entity.CreatorID,
                CreateDate = entity.CreateDate,
                Summary = entity.Summary
            };
        }


        #region 付款人
        public static Layers.Data.Sqls.PvbCrm.Payers ToLinq(this Models.Origins.Payer entity)
        {
            return new Layers.Data.Sqls.PvbCrm.Payers
            {
                ID = entity.ID,
                EnterpriseID = entity.EnterpriseID,
                RealID = entity.RealID,
                Bank = entity.Bank,
                BankAddress = entity.BankAddress,
                Account = entity.Account,
                SwiftCode = entity.SwiftCode,
                Methord = (int)entity.Methord,
                Currency = (int)entity.Currency,
                Contact = entity.Contact,
                Tel = entity.Tel,
                Mobile = entity.Mobile,
                Email = entity.Email,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Creator = entity.Creator,
                Place = entity.Place
            };
        }
        #endregion

        #region 收款人
        public static Layers.Data.Sqls.PvbCrm.nPayees ToLinq(this Models.Origins.nPayee entity)
        {
            return new Layers.Data.Sqls.PvbCrm.nPayees
            {
                ID = entity.ID,
                nSupplierID = entity.nSupplierID,
                EnterpriseID = entity.EnterpriseID,
                RealID = entity.RealID,
                Bank = entity.Bank,
                BankAddress = entity.BankAddress,
                Account = entity.Account,
                SwiftCode = entity.SwiftCode,
                Methord = (int)entity.Methord,
                Currency = (int)entity.Currency,
                Contact = entity.Contact,
                Tel = entity.Tel,
                Mobile = entity.Mobile,
                Email = entity.Email,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Creator = entity.Creator,
                IsDefault = entity.IsDefault,
                Place = entity.Place
            };
        }
        public static Layers.Data.Sqls.PvbCrm.nPayers ToLinq(this Models.Origins.nPayer entity)
        {
            return new Layers.Data.Sqls.PvbCrm.nPayers
            {
                ID = entity.ID,
                EnterpriseID = entity.EnterpriseID,
                RealID = entity.RealID,
                Bank = entity.Bank,
                BankAddress = entity.BankAddress,
                Account = entity.Account,
                SwiftCode = entity.SwiftCode,
                Methord = (int)entity.Methord,
                Currency = (int)entity.Currency,
                Contact = entity.Contact,
                Tel = entity.Tel,
                Mobile = entity.Mobile,
                Email = entity.Email,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Creator = entity.Creator
            };
        }
        #endregion

        #region 收款人
        public static Layers.Data.Sqls.PvbCrm.Payees ToLinq(this Models.Origins.Payee entity)
        {
            return new Layers.Data.Sqls.PvbCrm.Payees
            {
                ID = entity.ID,
                EnterpriseID = entity.EnterpriseID,
                RealID = entity.RealID,
                Bank = entity.Bank,
                BankAddress = entity.BankAddress,
                Account = entity.Account,
                SwiftCode = entity.SwiftCode,
                Methord = (int)entity.Methord,
                Currency = (int)entity.Currency,
                Contact = entity.Contact,
                Tel = entity.Tel,
                Mobile = entity.Mobile,
                Email = entity.Email,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Creator = entity.Creator,
                Place = entity.Place
            };
        }
        #endregion

        #region 私有交货地址
        /// <summary>
        /// Consignor To Linq
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layers.Data.Sqls.PvbCrm.nConsignors ToLinq(this Models.Origins.nConsignor entity)
        {
            return new Layers.Data.Sqls.PvbCrm.nConsignors
            {
                ID = entity.ID,
                nSupplierID = entity.nSupplierID,
                Title = entity.Title,
                EnterpriseID = entity.EnterpriseID,
                RealID = entity.RealID,
                Province = entity.Province,
                City = entity.City,
                Land = entity.Land,
                Address = entity.Address,
                Postzip = entity.Postzip,
                Contact = entity.Contact,
                Tel = entity.Tel,
                Mobile = entity.Mobile,
                Email = entity.Email,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = DateTime.Now,
                AdminID = entity.Creator,
                IsDefault = entity.IsDefault,
                Place = entity.Place
            };
        }
        #endregion

        #region 私有联系人
        public static Layers.Data.Sqls.PvbCrm.nContacts ToLinq(this Models.Origins.nContact entity)
        {
            return new Layers.Data.Sqls.PvbCrm.nContacts
            {
                ID = entity.ID,
                nSupplierID = entity.nSupplierID,
                EnterpriseID = entity.EnterpriseID,
                RealID = entity.RealID,
                Name = entity.Name,
                Tel = entity.Tel,
                Mobile = entity.Mobile,
                Email = entity.Email,
                Fax = entity.Fax,
                QQ = entity.QQ,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Creator = entity.Creator
            };
        }
        #endregion

        #region 品牌同义词
        public static Layers.Data.Sqls.PvbCrm.BrandDictionary ToLinq(this Models.Origins.BrandDictionary entity)
        {
            return new Layers.Data.Sqls.PvbCrm.BrandDictionary
            {
                ID = entity.ID,
                Name = entity.Name,
                OtherName = entity.OtherName,
                CreateDate = entity.CreateDate,
                Source = entity.Source,
                IsShort = entity.IsShort
            };
        }
        #endregion

        #region 品牌
        public static Layers.Data.Sqls.PvbCrm.Brands ToLinq(this Models.Origins.Brand entity)
        {
            return new Layers.Data.Sqls.PvbCrm.Brands
            {
                ID = entity.ID,
                Name = entity.Name,
                ShortName = entity.ShortName,
                CreateDate = entity.CreateDate,
                Status = (int)entity.Status
            };
        }
        #endregion
    }
}
