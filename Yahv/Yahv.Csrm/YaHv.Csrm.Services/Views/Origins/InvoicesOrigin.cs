using Layers.Data.Sqls;
using System.Linq;
using Yahv.Underly;
using YaHv.Csrm.Services.Models.Origins;
using System;

namespace YaHv.Csrm.Services.Views.Origins
{
    public class InvoicesOrigin : Yahv.Linq.UniqueView<Models.Origins.Invoice, PvbCrmReponsitory>
    {
        /// <summary>
        /// 默认构造器
        /// </summary>
        public InvoicesOrigin()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal InvoicesOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Invoice> GetIQueryable()
        {
            var enterpriseView = new Origins.EnterprisesOrigin(this.Reponsitory);
            var adminsView = new Rolls.AdminsAllRoll(this.Reponsitory);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Invoices>()
                   where entity.Status == (int)GeneralStatus.Normal
                   join enterprises in enterpriseView on entity.EnterpriseID equals enterprises.ID
                   join admin in adminsView on entity.AdminID equals admin.ID into _admin
                   from admin in _admin.DefaultIfEmpty()
                   select new Invoice()
                   {
                       ID = entity.ID,
                       EnterpriseID = entity.EnterpriseID,
                       Type = (InvoiceType)entity.Type,
                       Bank = entity.Bank,
                       BankAddress = entity.BankAddress,
                       Account = entity.Account,
                       TaxperNumber = entity.TaxperNumber,
                       Name = entity.Name,
                       Tel = entity.Tel,
                       Mobile = entity.Mobile,
                       Email = entity.Email,
                       District = (District)entity.District,
                       Address = entity.Address,
                       Postzip = entity.Postzip,
                       Status = (ApprovalStatus)entity.Status,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       Enterprise = enterprises,
                       CreatorID = entity.AdminID,
                       Creator = admin,
                       Province = entity.Province,
                       City = entity.City,
                       Land = entity.Land,
                       DeliveryType = (InvoiceDeliveryType)entity.DeliveryType,
                       CompanyTel = entity.CompanyTel,
                       InvoiceAddress = entity.InvoiceAddress
                   };
        }
    }

    /// <summary>
    /// 代仓储业务发票
    /// </summary>
    public class WsInvoicesOrigin : Yahv.Linq.UniqueView<Models.Origins.WsInvoice, PvbCrmReponsitory>
    {
        /// <summary>
        /// 默认构造器
        /// </summary>
        public WsInvoicesOrigin()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal WsInvoicesOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<WsInvoice> GetIQueryable()
        {
            var enterpriseView = new Origins.EnterprisesOrigin(this.Reponsitory);
            var adminsView = new Rolls.AdminsAllRoll(this.Reponsitory);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Invoices>()
                   join enterprises in enterpriseView on entity.EnterpriseID equals enterprises.ID
                   join admin in adminsView on entity.AdminID equals admin.ID into _admin
                   from admin in _admin.DefaultIfEmpty()

                   join map in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>() on entity.ID equals map.SubID
                   where map.Type == (int)MapsType.Invoice && map.Bussiness == (int)Business.WarehouseServicing
                   select new WsInvoice()
                   {
                       ID = entity.ID,
                       EnterpriseID = entity.EnterpriseID,
                       CompanyTel = entity.CompanyTel,
                       Type = (InvoiceType)entity.Type,
                       Bank = entity.Bank,
                       BankAddress = entity.BankAddress,
                       Account = entity.Account,
                       TaxperNumber = entity.TaxperNumber,
                       Name = entity.Name,
                       Tel = entity.Tel,
                       Mobile = entity.Mobile,
                       Email = entity.Email,
                       District = (District)entity.District,
                       Address = entity.Address,
                       Postzip = entity.Postzip,
                       Status = (ApprovalStatus)entity.Status,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       Enterprise = enterprises,
                       CreatorID = admin.ID,
                       Creator = admin,
                       Province = entity.Province,
                       City = entity.City,
                       Land = entity.Land,
                       DeliveryType = (InvoiceDeliveryType)entity.DeliveryType,
                       IsDefault = map.IsDefault,
                       InvoiceAddress = entity.InvoiceAddress
                   };
        }
    }

    /// <summary>
    /// 创痛贸易业务的企业发票
    /// </summary>
    public class TradingInvoiceOrigin : Yahv.Linq.UniqueView<Models.Origins.TradingInvoice, PvbCrmReponsitory>
    {
        /// <summary>
        /// 默认构造器
        /// </summary>
        public TradingInvoiceOrigin()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal TradingInvoiceOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<TradingInvoice> GetIQueryable()
        {
            var adminsView = new Rolls.AdminsAllRoll(this.Reponsitory);
            var enterpriseView = new Origins.EnterprisesOrigin(this.Reponsitory);
            var mapsView = Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Where(item => item.Bussiness == (int)Business.Trading && item.Type == (int)MapsType.Invoice);
            var linq = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Invoices>()

                       join maps in mapsView on entity.ID equals maps.SubID

                       join enterprise in enterpriseView on entity.EnterpriseID equals enterprise.ID

                       join admin in adminsView on entity.AdminID equals admin.ID
                       select new TradingInvoice()
                       {
                           ID = entity.ID,
                           EnterpriseID = entity.EnterpriseID,
                           Type = (InvoiceType)entity.Type,
                           Bank = entity.Bank,
                           BankAddress = entity.BankAddress,
                           Account = entity.Account,
                           TaxperNumber = entity.TaxperNumber,
                           Name = entity.Name,
                           Tel = entity.Tel,
                           Mobile = entity.Mobile,
                           Email = entity.Email,
                           District = (District)entity.District,
                           Address = entity.Address,
                           Postzip = entity.Postzip,
                           Status = (ApprovalStatus)entity.Status,
                           CreateDate = entity.CreateDate,
                           UpdateDate = entity.UpdateDate,
                           Enterprise = enterprise,
                           CreatorID = admin.ID,
                           Creator = admin,
                           Province = entity.Province,
                           City = entity.City,
                           Land = entity.Land,
                           CompanyTel = entity.CompanyTel,
                           DeliveryType = (InvoiceDeliveryType)entity.DeliveryType
                       };
            return linq;
        }
    }
}
