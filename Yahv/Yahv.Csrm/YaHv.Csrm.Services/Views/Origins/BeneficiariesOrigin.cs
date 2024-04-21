using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Origins
{
    public class BeneficiariesOrigin : Yahv.Linq.UniqueView<Models.Origins.Beneficiary, PvbCrmReponsitory>
    {
        /// <summary>
        /// 默认构造器
        /// </summary>
        internal BeneficiariesOrigin()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal BeneficiariesOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Beneficiary> GetIQueryable()
        {
            var enterpriseView = new Origins.EnterprisesOrigin(this.Reponsitory);
            var adminsView = new Views.Rolls.AdminsAllRoll(this.Reponsitory);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Beneficiaries>()
                   join enterprise in enterpriseView on entity.EnterpriseID equals enterprise.ID
                   join admin in adminsView on entity.AdminID equals admin.ID
                   select new Beneficiary()
                   {
                       ID = entity.ID,
                       InvoiceType = (InvoiceType)entity.InvoiceType,
                       EnterpriseID = entity.EnterpriseID,
                       RealName = entity.RealName,
                       Bank = entity.Bank,
                       BankAddress = entity.BankAddress,
                       Account = entity.Account,
                       SwiftCode = entity.SwiftCode,
                       Methord = (Methord)entity.Methord,
                       Currency = (Currency)entity.Currency,
                       District = (District)entity.District,
                       Name = entity.Name,
                       Tel = entity.Tel,
                       Mobile = entity.Mobile,
                       Email = entity.Email,
                       Status = (ApprovalStatus)entity.Status,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       Enterprise = enterprise,
                       CreatorID = admin.ID,
                       Creator = admin,
                       BankCode = entity.BankCode
                   };

        }
    }

    /// <summary>
    /// 关系表中的受益人（代仓储供应商）
    /// </summary>
    public class WsBeneficiariesOrigin : Yahv.Linq.UniqueView<Models.Origins.WsBeneficiary, PvbCrmReponsitory>
    {
        /// <summary>
        /// 默认构造器
        /// </summary>
        internal WsBeneficiariesOrigin()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal WsBeneficiariesOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<WsBeneficiary> GetIQueryable()
        {
            var enterpriseView = new Origins.EnterprisesOrigin(this.Reponsitory);
            var adminsView = new Views.Rolls.AdminsAllRoll(this.Reponsitory);

            var mapsView = Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Where(item => item.Type == (int)MapsType.Beneficiary && item.Bussiness == (int)Business.WarehouseServicing);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Beneficiaries>()

                   join map in mapsView on entity.ID equals map.SubID

                   join SupplierEnterprise in enterpriseView on entity.EnterpriseID equals SupplierEnterprise.ID

                   join clientEnterprise in enterpriseView on map.EnterpriseID equals clientEnterprise.ID

                   join admin in adminsView on entity.AdminID equals admin.ID into _admin
                   from admin in _admin.DefaultIfEmpty()
                   select new WsBeneficiary()
                   {
                       ID = entity.ID,
                       InvoiceType = (InvoiceType)entity.InvoiceType,
                       EnterpriseID = entity.EnterpriseID,
                       RealName = entity.RealName,
                       Bank = entity.Bank,
                       BankAddress = entity.BankAddress,
                       Account = entity.Account,
                       SwiftCode = entity.SwiftCode,
                       Methord = (Methord)entity.Methord,
                       Currency = (Currency)entity.Currency,
                       District = (District)entity.District,
                       Name = entity.Name,
                       Tel = entity.Tel,
                       Mobile = entity.Mobile,
                       Email = entity.Email,
                       Status = (ApprovalStatus)entity.Status,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       Enterprise = SupplierEnterprise,
                       CreatorID = entity.AdminID,
                       Creator = admin,
                       IsDefault = map.IsDefault,
                       WsClient = clientEnterprise
                   };

        }
    }

    public class TradingBeneficiariesOrigin : Yahv.Linq.UniqueView<Models.Origins.TradingBeneficiary, PvbCrmReponsitory>
    {
        /// <summary>
        /// 默认构造器
        /// </summary>
        internal TradingBeneficiariesOrigin()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal TradingBeneficiariesOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<TradingBeneficiary> GetIQueryable()
        {
            var enterpriseView = new Origins.EnterprisesOrigin(this.Reponsitory);
            var adminsView = new Views.Rolls.AdminsAllRoll(this.Reponsitory);
            var mapsView = Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Where(item => item.Type == (int)MapsType.Beneficiary && item.Bussiness == (int)Business.Trading);
            var linq = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Beneficiaries>()
                       join enterprise in enterpriseView on entity.EnterpriseID equals enterprise.ID

                       join map in mapsView on entity.ID equals map.SubID

                       join admin in adminsView on map.CtreatorID equals admin.ID
                       select new TradingBeneficiary()
                       {
                           ID = entity.ID,
                           InvoiceType = (InvoiceType)entity.InvoiceType,
                           EnterpriseID = entity.EnterpriseID,
                           RealName = entity.RealName,
                           Bank = entity.Bank,
                           BankAddress = entity.BankAddress,
                           Account = entity.Account,
                           SwiftCode = entity.SwiftCode,
                           Methord = (Methord)entity.Methord,
                           Currency = (Currency)entity.Currency,
                           District = (District)entity.District,
                           Name = entity.Name,
                           Tel = entity.Tel,
                           Mobile = entity.Mobile,
                           Email = entity.Email,
                           Status = (ApprovalStatus)entity.Status,
                           CreateDate = entity.CreateDate,
                           UpdateDate = entity.UpdateDate,
                           Enterprise = enterprise,
                           CreatorID = entity.AdminID,
                           Creator = admin,
                           IsDefault = map.IsDefault,
                           BankCode = entity.BankCode
                       };
            return linq;

        }
    }
}
