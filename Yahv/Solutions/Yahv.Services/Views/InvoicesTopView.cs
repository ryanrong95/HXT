using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.Services.Views
{

    /// <summary>
    /// 发票通用视图
    /// </summary>
    public class InvoicesTopView<TReponsitory> : UniqueView<Invoice, TReponsitory>
            where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public InvoicesTopView()
        {

        }
        public InvoicesTopView(TReponsitory reponsitory) : base(reponsitory)
        {

        }
        protected Business? business;
        public InvoicesTopView(Business business)
        {
            this.business = business;
        }
        public InvoicesTopView(Business business, TReponsitory reponsitory) : base(reponsitory)
        {
            this.business = business;
        }
        virtual protected IQueryable<Layers.Data.Sqls.PvbCrm.MapsBEnterTopView> GetMapIQueryable()
        {
            if (business.HasValue)
            {
                return from map in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnterTopView>()
                       where map.Bussiness == (int)business && map.Type == (int)MapsType.Invoice
                       select map;
            }
            return null;
        }


        IQueryable<Invoice> getIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.InvoicesTopView>()
                   select new Invoice
                   {
                       ID = entity.ID,
                       EnterpriseID = entity.EnterpriseID,
                       Bank = entity.Bank,
                       BankAddress = entity.BankAddress,
                       Account = entity.Account,
                       Name = entity.Name,
                       Tel = entity.Tel,
                       Mobile = entity.Mobile,
                       Type = (InvoiceType)entity.Type,//发票类型
                       Email = entity.Email,
                       District = (District)entity.District,
                       Postzip = entity.Postzip,
                       Address = entity.Address,
                       Status = (ApprovalStatus)entity.Status,
                       CompanyName = entity.CompanyName,
                       CompanyTel = entity.CompanyTel,
                       DeliveryType = (InvoiceDeliveryType)entity.DeliveryType,//开票类型
                       Uscc = entity.Uscc,
                       TaxperNumber = entity.TaxperNumber,
                       RegAddress = entity.RegAddress
                   };
        }
        protected override IQueryable<Invoice> GetIQueryable()
        {
            var mapsView = this.GetMapIQueryable();
            if (mapsView == null)
            {
                return this.getIQueryable();
            }
            else
            {
                return from entity in this.getIQueryable()
                       join m in mapsView on entity.ID equals m.SubID
                       select new Invoice
                       {
                           ID = entity.ID,
                           EnterpriseID = entity.EnterpriseID,
                           Bank = entity.Bank,
                           BankAddress = entity.BankAddress,
                           Account = entity.Account,
                           Name = entity.Name,
                           Tel = entity.Tel,
                           Mobile = entity.Mobile,
                           Type = entity.Type,//发票类型
                           Email = entity.Email,
                           District = entity.District,
                           Postzip = entity.Postzip,
                           Address = entity.Address,
                           Status = entity.Status,
                           CompanyName = entity.CompanyName,
                           CompanyTel = entity.CompanyTel,
                           DeliveryType = entity.DeliveryType,//开票类型
                           Uscc = entity.Uscc,
                           TaxperNumber = entity.TaxperNumber,
                           RegAddress = entity.RegAddress,
                           IsDefault = m.IsDefault,//关系表中获取
                       };
            }

        }

    }
}
