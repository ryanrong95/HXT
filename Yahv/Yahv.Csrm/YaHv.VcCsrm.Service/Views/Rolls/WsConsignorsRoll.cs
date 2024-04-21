using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YaHv.VcCsrm.Service.Views.Rolls
{
    public class WsConsignorsRoll : Origins.WsConsignorsOrigin
    {
        string wssupplierid;
        /// <summary>
        /// 默认构造器
        /// </summary>
        public WsConsignorsRoll(string wssupplierid)
        {
            this.wssupplierid = wssupplierid;
        }
        protected override IQueryable<Models.WsConsignor> GetIQueryable()
        {
            return from entity in base.GetIQueryable() where entity.WsSupplierID == this.wssupplierid select entity;
        }

    }
}
