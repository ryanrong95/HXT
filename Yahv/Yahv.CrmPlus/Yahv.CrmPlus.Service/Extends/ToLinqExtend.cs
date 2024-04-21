using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.CrmPlus.Service.Extends
{
    public static class ToLinqExtend
    {
        #region 标准型号
        /// <summary>
        /// StandardPartNumber To Linq
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layers.Data.Sqls.PvdCrm.StandardPartNumbers ToLinq(this Models.Origins.StandardPartNumber entity)
        {
            return new Layers.Data.Sqls.PvdCrm.StandardPartNumbers()
            {
                ID = entity.ID,
                PartNumber = entity.PartNumber,
                BrandID = entity.BrandID,
                Brand = entity.Brand,
                ProductName = entity.ProductName,
                PackageCase = entity.PackageCase,
                Packaging = entity.Packaging,
                Moq = entity.Moq,
                Mpq = entity.Mpq,
                TaxCode = entity.TaxCode,
                Eccn = entity.Eccn,
                TariffRate = entity.TariffRate,
                Ccc = entity.Ccc,
                CreateDate = entity.CreateDate,
                ModifyDate = DateTime.Now,
                Status = (int)entity.Status,
                Summary = entity.Summary,
                CreatorID = entity.CreatorID,
                Catalog = entity.Catalog
            };
        }
        #endregion

        #region 品牌或型号管控
        public static Layers.Data.Sqls.PvdCrm.Controls ToLinq(this Models.Origins.Control entity)
        {
            return new Layers.Data.Sqls.PvdCrm.Controls()
            {
                ID = entity.ID,
                MainID = entity.MainID,
                Type = (int)entity.Type,
                Context = entity.Context,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                CreatorID = entity.CreatorID
            };
        }
        #endregion

        #region 供应商
        public static Layers.Data.Sqls.PvdCrm.Suppliers ToLinq(this Models.Origins.Supplier entity)
        {
            return new Layers.Data.Sqls.PvdCrm.Suppliers
            {
                ID = entity.ID,
                Grade = entity.SupplierGrade,
                Products = entity.Products,
                Source = entity.Source,
                Type = (int)entity.Type,
                SettlementType = (int)entity.SettlementType,
                OrderType = (int)entity.OrderType,
                InvoiceType = (int)entity.InvoiceType,
                IsSpecial = entity.IsSpecial,
                IsClient = entity.IsClient,
                IsProtected = entity.IsProtected,
                IsAccount = entity.IsAccount,
                WorkTime = entity.WorkTime,
                IsFixed = entity.IsFixed,
                Status = (int)entity.SupplierStatus,
                CreateDate = entity.CreateDate,
                CreatorID = entity.CreatorID,
                OrderCompanyID = entity.OrderCompanyID
            };
        }
        #endregion

        #region 企业
        public static Layers.Data.Sqls.PvdCrm.Enterprises ToLinq(this Models.Origins.Enterprise entity)
        {
            return new Layers.Data.Sqls.PvdCrm.Enterprises
            {
                ID = entity.ID,
                Name = entity.Name,
                IsDraft = entity.IsDraft,
                Status = (int)entity.Status,
                District = entity.District,
                Grade = entity.Grade,
                Summary = entity.Summary,
                CreateDate = entity.CreateDate,
                DyjCode = entity.DyjCode,
                ModifyDate = entity.ModifyDate
            };
        }
        #endregion
        #region 企业信息
        public static Layers.Data.Sqls.PvdCrm.EnterpriseRegisters ToLinq(this Models.Origins.EnterpriseRegister entity)
        {
            return new Layers.Data.Sqls.PvdCrm.EnterpriseRegisters
            {
                ID = entity.ID,
                IsSecret = entity.IsSecret,
                IsInternational = entity.IsInternational,
                Corperation = entity.Corperation,
                RegAddress = entity.RegAddress,
                Uscc = entity.Uscc,
                Currency = (int?)entity.Currency,
                RegistFund = entity.RegistFund,
                RegistCurrency = (int?)entity.RegistCurrency,
                Industry = entity.Industry,
                RegistDate = entity.RegistDate,
                Summary = entity.Summary,
                BusinessState = entity.BusinessState,
                Employees = entity.Employees,
                WebSite = entity.WebSite,
                Nature = entity.Nature
            };
        }
        #endregion


        #region  客户
        public static Layers.Data.Sqls.PvdCrm.Clients ToLinq(this Models.Origins.Client entity)
        {
            return new Layers.Data.Sqls.PvdCrm.Clients
            {
                ID = entity.ID,
                Grade = (int)entity.ClientGrade,
                Type = (int)entity.ClientType,
                Vip = (int)entity.Vip,
                Source = entity.Source,
                IsMajor = entity.IsMajor,
                IsSpecial = entity.IsSpecial,
                IsSupplier = entity.IsSupplier,
                Industry = entity.Industry,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                ModifyDate = entity.ModifyDate,
                ProfitRate = entity.ProfitRate,
                Owner = entity.Owner,

            };


        }

        #endregion
    }
}
