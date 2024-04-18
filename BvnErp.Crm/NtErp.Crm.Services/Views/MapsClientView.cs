using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Linq;
using Layer.Data.Sqls;

namespace NtErp.Crm.Services.Views
{
    public class MapsClientView : QueryView<object, BvCrmReponsitory>
    {
        public MapsClientView()
        {
        }

        public MapsClientView(BvCrmReponsitory reponsitory):base(reponsitory)
        {
        }

        protected override IQueryable<object> GetIQueryable()
        {
            var linq = from map in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsClient>()
                       select new MapClient
                       {
                           ID = map.ID,
                           ClientID = map.ClientID,
                           AdminID = map.AdminID,
                           IndustryID = map.IndustryID,
                           ManufacturerID = map.ManufacturerID
                       };
            return linq;
        }

        public IQueryable<Models.Admin> GetClientOwner(string clientid)
        {
            var view = this.IQueryable.Cast<MapClient>();
            var linq = from map in view
                       join admin in Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.AdminTopView>() on map.AdminID equals admin.ID
                       where map.ClientID == clientid && map.AdminID != null
                       select new Models.Admin
                       {
                           ID = admin.ID,
                           UserName = admin.UserName,
                           RealName = admin.RealName,
                       };
            return linq;
        }

        private class MapClient
        {
            public string ID { get; set; }

            public string ClientID { get; set; }

            public string AdminID { get; set; }

            public string IndustryID { get; set; }

            public string ManufacturerID { get; set; }
        }
    }
}
