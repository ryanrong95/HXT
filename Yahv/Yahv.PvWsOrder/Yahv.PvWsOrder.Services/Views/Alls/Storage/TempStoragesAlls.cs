using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.Underly.Erps;

namespace Yahv.PvWsOrder.Services.Views
{
    public class TempStoragesAlls : UniqueView<Models.AdoptTmepStock, PvWmsRepository>                                    
    {
        public TempStoragesAlls()
        {

        }

        internal TempStoragesAlls(PvWmsRepository reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.AdoptTmepStock> GetIQueryable()
        {

            var unhandleTWaybills = new Views.Origins.TWaybillsOrigin<PvWmsRepository>().Where(t => t.Status == TempStorageStatus.Waiting);
            var wayBillIDs = unhandleTWaybills.Select(t => t.ID).ToList();

            var TStorages = new Views.Origins.TStoragesOrigin<PvWmsRepository>().Where(t => wayBillIDs.Contains(t.WaybillID));

            var tempStorages = from c in unhandleTWaybills
                               join d in TStorages
                               on c.ID equals d.WaybillID
                               select new Models.AdoptTmepStock
                               {
                                   ID = c.ID,
                                   EnterCode = c.EnterCode,
                                   ShelveID = c.ShelveID,
                                   WaybillCode = c.WaybillCode,
                                   Quantity = d.Quantity,
                                   CreateDate = c.CreateDate,
                                   TempStatus = (TempStorageStatus)c.Status
                               };

            return tempStorages;

            

        }
    }

    public class MyTempStoragesAlls : UniqueView<Models.AdoptTmepStock, PvWmsRepository>
    {
        IErpAdmin admin;
        public MyTempStoragesAlls(IErpAdmin Admin)
        {
            this.admin = Admin;
        }

        internal MyTempStoragesAlls(PvWmsRepository reponsitory, IErpAdmin Admin) : base(reponsitory)
        {
            this.admin = Admin;
        }

        protected override IQueryable<Models.AdoptTmepStock> GetIQueryable()
        {


            var wsClientTopView = new Yahv.PvWsOrder.Services.Views.TempTrackerWsClients<PvWmsRepository>(Reponsitory, admin);

            var unhandleTWaybills = new Views.Origins.TWaybillsOrigin<PvWmsRepository>(Reponsitory).Where(t => t.Status == TempStorageStatus.Waiting);            

            var TStorages = new Views.Origins.TStoragesOrigin<PvWmsRepository>(Reponsitory);


            var linq = from c in unhandleTWaybills
                       join d in TStorages
                       on c.ID equals d.WaybillID
                       join f in wsClientTopView on c.EnterCode equals f.EnterCode into clients
                       from client in clients.DefaultIfEmpty()
                       select new Models.AdoptTmepStock
                       {
                           ID = c.ID,
                           EnterCode = c.EnterCode,
                           CompanyName = client.Name,
                           ShelveID = c.ShelveID,
                           WaybillCode = c.WaybillCode,
                           Quantity = d.Quantity,
                           CreateDate = c.CreateDate,
                           TempStatus = (TempStorageStatus)c.Status,
                           Summary = c.Summary
                       };

            return linq;
        }
    }

    public class MyHandledTempStoragesAlls : UniqueView<Models.AdoptTmepStock, PvWmsRepository>
    {
        IErpAdmin admin;
        public MyHandledTempStoragesAlls(IErpAdmin Admin)
        {
            this.admin = Admin;
        }

        internal MyHandledTempStoragesAlls(PvWmsRepository reponsitory, IErpAdmin Admin) : base(reponsitory)
        {
            this.admin = Admin;
        }

        protected override IQueryable<Models.AdoptTmepStock> GetIQueryable()
        {


            var wsClientTopView = new Yahv.PvWsOrder.Services.Views.TempTrackerWsClients<PvWmsRepository>(Reponsitory, admin);

            var unhandleTWaybills = new Views.Origins.TWaybillsOrigin<PvWmsRepository>(Reponsitory).Where(t => t.Status == TempStorageStatus.Completed);

            var TStorages = new Views.Origins.TStoragesOrigin<PvWmsRepository>(Reponsitory);


            var linq = from c in unhandleTWaybills
                       join d in TStorages
                       on c.ID equals d.WaybillID
                       join f in wsClientTopView on c.EnterCode equals f.EnterCode into clients
                       from client in clients.DefaultIfEmpty()
                       select new Models.AdoptTmepStock
                       {
                           ID = c.ID,
                           EnterCode = c.EnterCode,
                           CompanyName = client.Name,
                           ShelveID = c.ShelveID,
                           WaybillCode = c.WaybillCode,
                           Quantity = d.Quantity,
                           CreateDate = c.CreateDate,
                           ForOrderID = c.ForOrderID,
                           TempStatus = (TempStorageStatus)c.Status
                       };

            return linq;
        }
    }
}
