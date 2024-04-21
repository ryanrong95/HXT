using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YaHv.VcCsrm.Service.Views.Rolls
{
    public class WsSuppliersRoll : Origins.WsSuppliersOrigin
    {
        string shipid;
        /// <summary>
        /// 默认构造器
        /// </summary>
        public WsSuppliersRoll(string shipid)
        {
            this.shipid = shipid;
        }
        protected override IQueryable<Models.WsSupplier> GetIQueryable()
        {
            return from entity in base.GetIQueryable() where entity.ShipID == this.shipid select entity;
        }
    }
}
