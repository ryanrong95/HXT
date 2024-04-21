using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YaHv.VcCsrm.Service.Extends
{
    public static class ToLinqExtends
    {
        /// <summary>
        /// Enterprise To Linq
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layers.Data.Sqls.PvcCrm.Enterprises ToLinq(this Models.Enterprise entity)
        {
            return new Layers.Data.Sqls.PvcCrm.Enterprises
            {
                ID = entity.ID,
                Name = entity.Name,
                AdminCode = entity.AdminCode == null ? "" : entity.AdminCode,
                Status = (int)entity.Status,
                District = entity.District,
                RegAddress = entity.RegAddress,
                Corporation = entity.Corporation,
                Uscc = entity.Uscc
            };
        }
        /// <summary>
        /// Enterprise To Linq
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layers.Data.Sqls.PvcCrm.WsClients ToLinq(this Models.WsClient entity)
        {
            return new Layers.Data.Sqls.PvcCrm.WsClients
            {
                ID = entity.ID,
                Grade = (int)entity.Grade,
                Vip = entity.Vip,
                EnterCode = entity.EnterCode,
                CustomsCode = entity.CustomsCode,
                AdminID = entity.CreatorID,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Summary = entity.Summary,
                Status = (int)entity.Status,
                Nature = (int)entity.Nature,
                Origin = entity.Origin
            };
        }
        /// <summary>
        /// Enterprise To Linq
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layers.Data.Sqls.PvcCrm.WsPayees ToLinq(this Models.WsPayee entity)
        {
            return new Layers.Data.Sqls.PvcCrm.WsPayees
            {
                ID = entity.ID,
                InvoiceType = (int)entity.InvoiceType,
                WsSupplierID = entity.WsSupplierID,
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
                CreatorID = entity.CreatorID,
                //IsDefault=entity.IsDefault
            };
        }

        /// <summary>
        /// Consignor To Linq
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layers.Data.Sqls.PvcCrm.WsConsignors ToLinq(this Models.WsConsignor entity)
        {
            return new Layers.Data.Sqls.PvcCrm.WsConsignors
            {
                ID = entity.ID,
                Title = entity.Title,
                WsSupplierID = entity.WsSupplierID,
                DyjCode = entity.DyjCode,
                Province = entity.Province,
                City = entity.City,
                Area = entity.Area,
                Address = entity.Address,
                Postzip = entity.Postzip,
                Name = entity.Name,
                Tel = entity.Tel,
                Mobile = entity.Mobile,
                Email = entity.Email,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = DateTime.Now,
                CreatorID = entity.CreatorID,
                IsDefault = entity.IsDefault
            };
        }
        /// <summary>
        /// Invoice To Linq
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layers.Data.Sqls.PvcCrm.Invoices ToLinq(this Models.Invoice entity)
        {
            return new Layers.Data.Sqls.PvcCrm.Invoices
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
                DeliveryType = (int)entity.DeliveryType


            };
        }
        /// <summary>
        /// 代仓储供应商
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layers.Data.Sqls.PvcCrm.WsSuppliers ToLinq(this Models.WsSupplier entity)
        {
            return new Layers.Data.Sqls.PvcCrm.WsSuppliers
            {
                ID = entity.ID,
                EnterpriseID = entity.EnterpriseID,
                ShipID = entity.ShipID,
                Grade = (int)entity.Grade,
                EnglishName = entity.EnglishName,
                ChineseName = entity.ChineseName,
                Status = (int)entity.Status,
                AdminID = entity.CreatorID,
                Summary = entity.Summary,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Origin = entity.Origin
            };
        }
        ///// <summary>
        ///// 代仓储客户新合同
        ///// </summary>
        ///// <param name="entity"></param>
        ///// <returns></returns>
        public static Layers.Data.Sqls.PvcCrm.WsContracts ToLinq(this Models.WsContract entity)
        {
            return new Layers.Data.Sqls.PvcCrm.WsContracts
            {
                ID = entity.ID,
                Trustee = entity.TrusteeID,
                WsClientID = entity.WsClientID,
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

        /// <summary>
        /// Consignee To Linq
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layers.Data.Sqls.PvcCrm.Consignees ToLinq(this Models.Consignee entity)
        {
            return new Layers.Data.Sqls.PvcCrm.Consignees
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
                PlateCode = entity.PlateCode
            };
        }
        public static Layers.Data.Sqls.PvcCrm.Contracts ToLinq(this Models.Contract entity)
        {
            return new Layers.Data.Sqls.PvcCrm.Contracts
            {
                ID = entity.ID,
                EnterpriseID = entity.WsClientID,
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
                // CompanyID=entity.CompanyID
            };
        }
    }
}
