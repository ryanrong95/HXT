using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Underly.Erps;
using Yahv.Utils.Converters.Contents;

namespace Yahv.Services.Views
{
    public class WsClientsTopView<TReponsitory> : UniqueView<WsClient, TReponsitory>
          where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {

        public WsClientsTopView()
        {

        }
        public WsClientsTopView(TReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<WsClient> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.WsClientsTopView>()
                   select new WsClient
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       RegAddress = entity.RegAddress,
                       Corporation = entity.Corporation,
                       CustomsCode = entity.CustomsCode,
                       EnterCode = entity.EnterCode,
                       Uscc = entity.Uscc,
                       Grade = (Underly.ClientGrade)entity.Grade,
                       Vip = entity.Vip,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       AdminID = entity.AdminID,
                       AdminCode = entity.AdminCode,
                       Status = (Underly.ApprovalStatus)entity.Status,
                       ClientNatue = (ClientType)entity.ClientNature,
                       ServiceType = (ServiceType)entity.ServiceType,
                       IsDeclaretion = entity.IsDeclaretion,
                       IsStorageService = entity.IsStorageService,
                       StorageType = (WsIdentity)entity.StorageType
                   };
        }

        public IEnumerable<WsClientManager> getManagers()
        {
            var linq_clients = this.GetIQueryable();
            var allclients = linq_clients.ToArray();
            var realids = allclients.Select(item => new
            {
                ClientID = item.ID,
                RealID = string.Join("", "DBAEAB43B47EB4299DD1D62F764E6B6A", item.ID).MD5(),
            }).ToArray();

            var linq_managers = from maps in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MapsTrackerTopView>()
                                join admins in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.AdminsTopView>() on maps.AdminID equals admins.ID
                                select new
                                {
                                    maps.RealID,
                                    ServiceManagerID = admins.ID,
                                    ServiceManagerName = admins.RealName
                                };
            var servicemanages = linq_managers.ToArray();

            var linq = from client in allclients
                       join reals in realids on client.ID equals reals.ClientID
                       join managers in servicemanages on reals.RealID equals managers.RealID
                       select new WsClientManager
                       {
                           ID = client.ID,
                           Name = client.Name,
                           RegAddress = client.RegAddress,
                           Corporation = client.Corporation,
                           CustomsCode = client.CustomsCode,
                           EnterCode = client.EnterCode,
                           Uscc = client.Uscc,
                           Grade = client.Grade,
                           Vip = client.Vip,
                           CreateDate = client.CreateDate,
                           UpdateDate = client.UpdateDate,
                           AdminID = client.AdminID,
                           AdminCode = client.AdminCode,
                           Status = client.Status,
                           ClientNatue = client.ClientNatue,
                           ServiceType = client.ServiceType,
                           IsDeclaretion = client.IsDeclaretion,
                           IsStorageService = client.IsStorageService,
                           StorageType = client.StorageType,
                           ServiceManagerID = managers.ServiceManagerID,
                           ServiceManagerName = managers.ServiceManagerName
                       };
            return linq;
        }
    }
}
