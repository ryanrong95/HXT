using Layers.Data.Sqls.PvbCrm;
using System;
using System.Linq;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.Services.Views
{

    /// <summary>
    /// 受益人视图
    /// </summary>
    public class BeneficiariesTopView<TReponsitory> : UniqueView<Beneficiary, TReponsitory>
            where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BeneficiariesTopView()
        {

        }

        protected Business? business;


        public BeneficiariesTopView(Business business)
        {
            this.business = business;
        }
        public BeneficiariesTopView(Business business, TReponsitory reponsitory) : base(reponsitory)
        {
            this.business = business;
        }
        public BeneficiariesTopView(TReponsitory reponsitory) : base(reponsitory)
        {

        }

        virtual protected IQueryable<Layers.Data.Sqls.PvbCrm.MapsBEnterTopView> GetMapIQueryable()
        {
            if (business.HasValue)
            {
                return from map in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnterTopView>()
                       where map.Bussiness == (int)business && map.Type == (int)MapsType.Beneficiary
                       select map;
            }
            return null;
        }


        IQueryable<Beneficiary> getIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<BeneficiariesTopView>()
                   select new Beneficiary
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       Account = entity.Account,
                       Bank = entity.Bank,
                       BankAddress = entity.BankAddress,
                       Currency = (Currency)entity.Currency,
                       District = (District)entity.District,
                       Email = entity.Email,
                       EnterpriseID = entity.EnterpriseID,
                       Methord = (Methord)entity.Methord,
                       Mobile = entity.Mobile,
                       RealName = entity.RealName,
                       SwiftCode = entity.SwiftCode,
                       Tel = entity.Tel,
                       InvoiceType = (InvoiceType)entity.InvoiceType
                   };
        }


        protected override IQueryable<Beneficiary> GetIQueryable()
        {

            var mapView = this.GetMapIQueryable();

            if (mapView == null)
            {
                return this.getIQueryable();
            }
            else
            {
                var linq = from entity in this.getIQueryable()
                           join m in this.GetMapIQueryable() on entity.ID equals m.SubID
                           select new Models.Beneficiary
                           {
                               ID = entity.ID,
                               EnterpriseID = entity.EnterpriseID,
                               Name = entity.Name,
                               Account = entity.Account,
                               Bank = entity.Bank,
                               BankAddress = entity.BankAddress,
                               Currency = entity.Currency,
                               District = entity.District,
                               Email = entity.Email,
                               Methord = entity.Methord,
                               Mobile = entity.Mobile,
                               RealName = entity.RealName,
                               SwiftCode = entity.SwiftCode,
                               Tel = entity.Tel,
                               InvoiceType = entity.InvoiceType,
                               ClientID = m.EnterpriseID,//关系表中获取
                               IsDefault = m.IsDefault
                           };
                return linq;
            }
        }
    }
}
