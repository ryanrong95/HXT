using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Underly;
using Layers.Data.Sqls;
using Yahv.Underly.Erps;

namespace Yahv.PvWsOrder.Services.Views
{
    public class WsClientsAlls : UniqueView<WsClient, PvWsOrderReponsitory>
    {
        public WsClientsAlls()
        {

        }

        internal WsClientsAlls(PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<WsClient> GetIQueryable()
        {
            var wsClientTopView = new Yahv.Services.Views.WsClientsTopView<PvWsOrderReponsitory>(this.Reponsitory)
                .Where(item => item.Status == ApprovalStatus.Normal); 
            var contactTopView = new Yahv.Services.Views.ContactsTopView<PvWsOrderReponsitory>(Business.WarehouseServicing, this.Reponsitory)
                .Where(contact => contact.Status == ApprovalStatus.Normal && contact.IsDefault == true);

            var linq = from entity in wsClientTopView
                       join contact in contactTopView on entity.ID equals contact.EnterpriseID into contacts
                       from contact in contacts.DefaultIfEmpty()
                       select new WsClient
                       {
                           ID = entity.ID,
                           Name = entity.Name,
                           RegAddress = entity.RegAddress,
                           Corporation = entity.Corporation,
                           CustomsCode = entity.CustomsCode,
                           EnterCode = entity.EnterCode,
                           Uscc = entity.Uscc,
                           Grade = entity.Grade,
                           Vip = entity.Vip,
                           CreateDate = entity.CreateDate,
                           UpdateDate = entity.UpdateDate,
                           AdminID = entity.AdminID,
                           AdminCode = entity.AdminCode,
                           Status = entity.Status,
                           ServiceType = entity.ServiceType,
                           IsDeclaretion = entity.IsDeclaretion,
                           IsStorageService = entity.IsStorageService,
                           StorageType = entity.StorageType,

                           Contact = contact,
                       };
            return linq;
        }
    }

    public class MyWsClientsAlls : UniqueView<WsClient, PvWsOrderReponsitory>
    {
        IErpAdmin admin;
        public MyWsClientsAlls(IErpAdmin Admin)
        {
            this.admin = Admin;
        }

        internal MyWsClientsAlls(PvWsOrderReponsitory reponsitory, IErpAdmin Admin) : base(reponsitory)
        {
            this.admin = Admin;
        }

        protected override IQueryable<WsClient> GetIQueryable()
        {
            if (admin.IsSuper)
            {
                return new WsClientsAlls();
            }

            var wsClientTopView = new Yahv.Services.Views.TrackerWsClients<PvWsOrderReponsitory>(Reponsitory, admin)
                .Where(item => item.Status == ApprovalStatus.Normal);
            var contactTopView = new Yahv.Services.Views.ContactsTopView<PvWsOrderReponsitory>(Business.WarehouseServicing, this.Reponsitory)
                .Where(contact => contact.Status == ApprovalStatus.Normal && contact.IsDefault == true);
            var linq = from entity in wsClientTopView
                       join contact in contactTopView on entity.ID equals contact.EnterpriseID into contacts
                       from contact in contacts.DefaultIfEmpty()
                       select new WsClient
                       {
                           ID = entity.ID,
                           Name = entity.Name,
                           RegAddress = entity.RegAddress,
                           Corporation = entity.Corporation,
                           CustomsCode = entity.CustomsCode,
                           EnterCode = entity.EnterCode,
                           Uscc = entity.Uscc,
                           Grade = entity.Grade,
                           Vip = entity.Vip,
                           CreateDate = entity.CreateDate,
                           UpdateDate = entity.UpdateDate,
                           AdminID = entity.AdminID,
                           AdminCode = entity.AdminCode,
                           Status = entity.Status,
                           ServiceType = entity.ServiceType,
                           IsDeclaretion = entity.IsDeclaretion,
                           IsStorageService = entity.IsStorageService,
                           StorageType = entity.StorageType,

                           Contact = contact,
                       };
            return linq;
        }
    }
}
