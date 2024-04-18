using System;
using System.Linq;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.Services.Views
{

    /// <summary>
    /// 企业联系人通用视图
    /// </summary>
    public class ContactsTopView<TReponsitory> : UniqueView<Contact, TReponsitory>
            where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ContactsTopView()
        {

        }

        public ContactsTopView(TReponsitory reponsitory) : base(reponsitory)
        {

        }

        public ContactsTopView(Business business)
        {
            this.business = business;
        }


        protected Business? business;

        public ContactsTopView(Business business, TReponsitory reponsitory) :base(reponsitory)
        {
            this.business = business;
        }

        virtual protected IQueryable<Layers.Data.Sqls.PvbCrm.MapsBEnterTopView> GetMapIQueryable()
        {
            if (business.HasValue)
            {
                return from map in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnterTopView>()
                       where map.Bussiness == (int)business.Value && map.Type == (int)MapsType.Contact
                       select map;
            }

            return null;
        }

        IQueryable<Contact> getIQueryable1()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.ContactsTopView>()
                   where entity.Status == (int)ApprovalStatus.Normal
                   select new Contact
                   {
                       ID = entity.ID,
                       EnterpriseID = entity.EnterpriseID,
                       Type = (ContactType)entity.Type,
                       Status = (ApprovalStatus)entity.Status,
                       Name = entity.Name,
                       Tel = entity.Tel,
                       Mobile = entity.Mobile,
                       Email = entity.Email,
                       Fax = entity.Fax
                   };
        }
        protected override IQueryable<Contact> GetIQueryable()
        {
            var mapsView = this.GetMapIQueryable();
            if (mapsView == null)
            {
                return this.getIQueryable1();
            }
            else
            {
                return from entity in this.getIQueryable1()
                       join m in mapsView on entity.ID equals m.SubID
                       select new Models.Contact
                       {
                           ID = entity.ID,
                           EnterpriseID = entity.EnterpriseID,
                           Type = entity.Type,
                           Status = entity.Status,
                           Name = entity.Name,
                           Tel = entity.Tel,
                           Mobile = entity.Mobile,
                           Email = entity.Email,
                           Fax = entity.Fax,
                           IsDefault = m.IsDefault  //关系表中获取
                       };
            }
        }

    }
}
