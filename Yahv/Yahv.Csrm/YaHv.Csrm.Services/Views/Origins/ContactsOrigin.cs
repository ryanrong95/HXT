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
    public class ContactsOrigin : Yahv.Linq.UniqueView<Models.Origins.Contact, PvbCrmReponsitory>
    {
        /// <summary>
        /// 默认构造器
        /// </summary>
        internal ContactsOrigin()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal ContactsOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Contact> GetIQueryable()
        {
            var enterpriseView = new Origins.EnterprisesOrigin(this.Reponsitory);
            var adminsView = new Rolls.AdminsAllRoll(this.Reponsitory);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Contacts>()
                   join enterprises in enterpriseView on entity.EnterpriseID equals enterprises.ID
                   join admin in adminsView on entity.AdminID equals admin.ID into _admin
                   from admin in _admin.DefaultIfEmpty()
                   select new Contact()
                   {
                       ID = entity.ID,
                       EnterpriseID = entity.EnterpriseID,
                       Name = entity.Name,
                       Type = (ContactType)entity.Type,
                       Tel = entity.Tel,
                       Mobile = entity.Mobile,
                       Email = entity.Email,
                       Fax = entity.Fax,
                       Status = (Status)entity.Status,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       Enterprise = enterprises,
                       CreatorID = entity.AdminID,
                       Creator = admin
                   };
        }
    }
    /// <summary>
    /// 代仓储联系人
    /// </summary>
    public class WsContactsOrigin : Yahv.Linq.UniqueView<Models.Origins.WsContact, PvbCrmReponsitory>
    {
        /// <summary>
        /// 默认构造器
        /// </summary>
        internal WsContactsOrigin()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal WsContactsOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<WsContact> GetIQueryable()
        {
            var enterpriseView = new Origins.EnterprisesOrigin(this.Reponsitory);
            var adminsView = new Rolls.AdminsAllRoll(this.Reponsitory);
            var linq = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Contacts>()
                       join enterprises in enterpriseView on entity.EnterpriseID equals enterprises.ID
                       join admin in adminsView on entity.AdminID equals admin.ID into _admin
                       from admin in _admin.DefaultIfEmpty()
                       join map in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>() on entity.ID equals map.SubID
                       where map.Type == (int)MapsType.Contact && map.Bussiness == (int)Business.WarehouseServicing
                       select new WsContact()
                       {
                           ID = entity.ID,
                           EnterpriseID = entity.EnterpriseID,
                           Name = entity.Name,
                           Type = (ContactType)entity.Type,
                           Tel = entity.Tel,
                           Mobile = entity.Mobile,
                           Email = entity.Email,
                           Fax = entity.Fax,
                           Status = (Status)entity.Status,
                           CreateDate = entity.CreateDate,
                           UpdateDate = entity.UpdateDate,
                           Enterprise = enterprises,
                           CreatorID = admin.ID,
                           Creator = admin,
                           IsDefault = map.IsDefault,
                       };
            return linq;
        }
    }

    /// <summary>
    /// 不同业务线的联系人
    /// </summary>
    public class ServiceContactsOrigin : Yahv.Linq.UniqueView<Models.Origins.TradingContact, PvbCrmReponsitory>
    {
        /// <summary>
        /// 默认构造器
        /// </summary>
        public ServiceContactsOrigin()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal ServiceContactsOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<TradingContact> GetIQueryable()
        {
            var adminsView = new Rolls.AdminsAllRoll(this.Reponsitory);
            var enterpriseView = new Origins.EnterprisesOrigin(this.Reponsitory);
            var mapsView = Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Where(item => item.Bussiness == (int)Business.Trading && item.Type == (int)MapsType.Contact);
            var linq = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Contacts>()

                       join maps in mapsView on entity.ID equals maps.SubID

                       join enterprise in enterpriseView on entity.EnterpriseID equals enterprise.ID

                       join admin in adminsView on maps.CtreatorID equals admin.ID

                       select new TradingContact()
                       {
                           ID = entity.ID,
                           EnterpriseID = entity.EnterpriseID,
                           Enterprise = enterprise,
                           Name = entity.Name,
                           Type = (ContactType)entity.Type,
                           Tel = entity.Tel,
                           Mobile = entity.Mobile,
                           Email = entity.Email,
                           Fax = entity.Fax,
                           Status = (Status)entity.Status,
                           CreateDate = entity.CreateDate,
                           UpdateDate = entity.UpdateDate,
                           CreatorID = admin.ID,
                           Creator = admin,
                       };
            return linq;
        }
    }
}
