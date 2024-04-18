using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Underly;
using Needs.Utils.Serializers;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models;
using NtErp.Crm.Services.Views;

namespace NtErp.Crm.Services.Extends
{
    public static class ClientExtends
    {
        /// <summary>
        /// model数据映射数据库对象
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        static internal Layer.Data.Sqls.BvCrm.Clients ToLinq(this Models.Client entity)
        {
            var arrAreaID = (entity.AreaID ?? string.Empty).Split(' ');
            return new Layer.Data.Sqls.BvCrm.Clients
            {
                ID = entity.ID,
                Name = entity.Name,
                IsSafe = Convert.ToBoolean(entity.IsSafe),
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Summary = entity.Summary,
                AdminCode = entity.AdminCode,
                CUSCC = entity.CUSCC,
                IndustryInvolved = entity.IndustryInvolved,
                AreaID = arrAreaID[arrAreaID.Count() - 1] == string.Empty ? null : arrAreaID[arrAreaID.Count() - 1],
            };
        }

        static internal Layer.Data.Sqls.BvCrm.ClientShowers ToShower(this Models.Client entity)
        {
            entity.NTextString = "";
            return new Layer.Data.Sqls.BvCrm.ClientShowers
            {
                ClientID = entity.ID,
                NTextString = entity.Json(),
                //NTextString = (new {
                //    Name = entity.Name,
                //    EnterpriseProperty=entity.EnterpriseProperty,
                //    Area=entity.Area,
                //    RegisteredCapital=entity.RegisteredCapital,                   
                //    EstablishmentDate = entity.EstablishmentData,
                //    OperatingPeriod = entity.OperatingPeriod,
                //    RegisteredAddress = entity.RegisteredAddress,
                //    OfficeAddress = entity.OfficeAddress,
                //    Site = entity.Site,
                //    BusinessScope = entity.BusinessScope,
                //    CustomerType = entity.CustomerType,
                //    CustomerLevel = entity.CustomerLevel,
                //    IndustryInvolved = entity.IndustryInvolved,
                //    Possessor = entity.Possessor,
                //    CompanyID = entity.CompanyID,
                //    BusinessType = entity.BusinessType,
                //    ProtectLevel = entity.ProtectLevel,
                //    ProtectionScope = entity.ProtectionScope,
                //    AgentBrand = entity.AgentBrand,
                //    CreditLimit = entity.CreditLimit,
                //    CreditPayment = entity.CreditPayment,
                //    CustomerStatus = entity.CustomerStatus,
                //    BillingInformation = entity.BillingInformation,
                //    ExtraPacking = entity.ExtraPacking,
                //    SpecialSupplier = entity.SpecialSupplier,
                //    ShippingAddress = entity.ShippingAddress,
                //    ShippingAddress2 = entity.ShippingAddress2,
                //    InformationSource = entity.InformationSource,
                //    Summary = entity.Summary
                //}).Json()
            };

        }
    }
}
