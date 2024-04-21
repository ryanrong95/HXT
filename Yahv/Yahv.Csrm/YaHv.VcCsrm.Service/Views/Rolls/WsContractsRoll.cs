using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YaHv.VcCsrm.Service.Views.Rolls
{
    public class WsContractsRoll : Origins.WsContractsOrigin
    {
        string WsClientID;
        string Trustee;
        /// <summary>
        /// 默认构造器
        /// </summary>
        public WsContractsRoll(string wsClientID, string trustee = "HK LIANCHUANG ELECTRONICS., LIMITED")
        {
            this.WsClientID = wsClientID;
            this.Trustee = trustee;
        }
        protected override IQueryable<Models.WsContract> GetIQueryable()
        {
            var linq = from entity in base.GetIQueryable()
                       where entity.WsClientID == WsClientID && entity.TrusteeID == this.Trustee
                       select entity;
            return linq;
        }
    }
}
