using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Newtonsoft.Json;
using Yahv.Erm.Services.Models.Rolls;
using Yahv.Linq;
using Yahv.Services.Views;
using Yahv.Underly.Erps;
using System.Linq.Dynamic;

namespace Yahv.Erm.Services.Views.Rolls
{
    public class MyWareHouseRoll : QueryView<AdminWareHouse, PvbErmReponsitory>
    {
        private IErpAdmin _admin;

        public MyWareHouseRoll(IErpAdmin admin)
        {
            this._admin = admin;
        }

        protected override IQueryable<AdminWareHouse> GetIQueryable()
        {
            var maps = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.MapsWareHouse>().ToList();
            var wareHouses = new Yahv.Services.Views.WarehousePlatesTopView<PvbCrmReponsitory>().ToList();
            var result = from entity in maps
                         join wms in wareHouses on entity.ConsigneeID equals wms.ID into joinWms
                         from wms in joinWms.DefaultIfEmpty()
                         join bwms in wareHouses on entity.ConsigneeID equals bwms.EnterpriseID into joinBwms
                         from bwms in joinBwms.DefaultIfEmpty()
                         where entity.AdminID == _admin.ID
                         select new AdminWareHouse()
                         {
                             AdminID = entity.AdminID,
                             ID = entity.ConsigneeID,
                             Code = wms == null ? bwms.WsCode : wms.Code,
                             Name = wms == null ? bwms.WarehouseName : wms.Title,
                             ParentID = wms == null ? bwms.EnterpriseID : wms.EnterpriseID,
                             ParentName = wms == null ? bwms.WarehouseName : wms.WarehouseName,
                             ParentCode = wms?.WsCode ?? bwms.WsCode,
                         };

            return result.Distinct(new DataRowComparer()).AsQueryable();
        }

        public class DataRowComparer : IEqualityComparer<AdminWareHouse>
        {
            public bool Equals(AdminWareHouse t1, AdminWareHouse t2)
            {
                return (t1.AdminID == t2.AdminID
                        && t1.ID == t2.ID
                        && t1.Code == t2.Code
                        && t1.Name == t2.Name
                        && t1.ParentID == t2.ParentID
                        && t1.ParentCode == t2.ParentCode
                        && t1.ParentName == t2.ParentName);
            }
            public int GetHashCode(AdminWareHouse t)
            {
                return t.ToString().GetHashCode();
            }
        }
    }
}
