using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Rolls
{
    /// <summary>
    /// 某企业的交货地址
    /// </summary>
    public class WsConsignorsRoll : Origins.WsConsignorsOrigin
    {
        Enterprise WsClient;
        Enterprise WsSupplier;
        /// <summary>
        /// 默认构造器
        /// </summary>
        public WsConsignorsRoll(Enterprise wsclient, Enterprise wssupplier)
        {
            this.WsClient = wsclient;
            this.WsSupplier = wssupplier;
        }
        protected override IQueryable<WsConsignor> GetIQueryable()
        {
            return from item in base.GetIQueryable()
                   where item.EnterpriseID == this.WsSupplier.ID && item.WsClient.ID == this.WsClient.ID
                   select item;

        }
    }

    public class ConsignorsRoll : Origins.ConsignorsOrigin
    {
        /// <summary>
        /// 默认构造器
        /// </summary>
        public ConsignorsRoll()
        {
        }
        protected override IQueryable<Consignor> GetIQueryable()
        {
            return base.GetIQueryable();

        }
    }
}
