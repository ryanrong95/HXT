using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Underly;
using YaHv.CrmPlus.Services.Views.Rolls;

namespace Yahv.CrmPlus.Service.Views.Origins
{
    public class SuppliersOrigin : Linq.UniqueView<Supplier, PvdCrmReponsitory>
    {
        internal SuppliersOrigin()
        {

        }
        internal SuppliersOrigin(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Supplier> GetIQueryable()
        {
            var enterprisesView = new EnterprisesOrigin(this.Reponsitory);
            //var registersView = new EnterpriseRegistersOrigin(this.Reponsitory);
            var adminsView = new AdminsAllRoll(this.Reponsitory);
            var fixedsupplier = new FixedSuppliersOrigin(this.Reponsitory);
            return from supplier in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Suppliers>()
                   join enterprise in enterprisesView on supplier.ID equals enterprise.ID
                   //join register in registersView on supplier.EnterpriseID equals register.ID
                   //固定渠道
                   join fixeds in fixedsupplier on supplier.ID equals fixeds.ID into fxs
                   from fx in fxs.DefaultIfEmpty()
                       //保护人
                   join own_admin in adminsView on supplier.OwnerID equals own_admin.ID into ownerAdmin
                   from owner in ownerAdmin.DefaultIfEmpty()
                       //创建人
                   join creat_admin in adminsView on supplier.CreatorID equals creat_admin.ID into creatAdmin
                   from creator in creatAdmin.DefaultIfEmpty()
                       //下单公司
                   join orderenterprise in enterprisesView on supplier.OrderCompanyID equals orderenterprise.ID into OrderEnterprise
                   from orderenterprise in OrderEnterprise.DefaultIfEmpty()
                   select new Supplier
                   {
                      
                       ID = supplier.ID,
                       Name = enterprise.Name,
                       IsDraft = enterprise.IsDraft,
                       Status = enterprise.Status,
                       District = enterprise.District,
                       Place = enterprise.Place,
                       Grade = supplier.Grade,
                       Summary = enterprise.Summary,
                       CreateDate = enterprise.CreateDate,
                       ModifyDate = enterprise.ModifyDate,
                       DyjCode = enterprise.DyjCode,
                       SupplierGrade = supplier.Grade,
                       Products = supplier.Products,
                       Source = supplier.Source,
                       EnterpriseRegister = enterprise.EnterpriseRegister,

                       Type = (Underly.CrmPlus.SupplierType)supplier.Type,
                       SettlementType = (Underly.CrmPlus.SettlementType)supplier.SettlementType,
                       OrderType = (Underly.CrmPlus.OrderType)supplier.OrderType,
                       InvoiceType = (InvoiceType)supplier.InvoiceType,
                       IsSpecial = supplier.IsSpecial,
                       IsClient = supplier.IsClient,
                       IsProtected = supplier.IsProtected,
                       IsAgent = supplier.IsAgent,
                       IsAccount = supplier.IsAccount,
                       WorkTime = supplier.WorkTime,
                       IsFixed = supplier.IsFixed,
                       OwnerAdmin = owner,
                       CreatorID = creator.ID,
                       CreatorAdmin = creator,
                       SupplierStatus = (AuditStatus)supplier.Status,
                       FiexedSupplier = fx,
                       OrderCompanyID = supplier.OrderCompanyID,
                       OrderCompany = orderenterprise
                   };
        }
    }
}
