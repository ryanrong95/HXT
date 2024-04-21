using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Rolls
{
    public class WsContractsRoll : Origins.WsContractsOrigin
    {
        string WsClientID;
        string Trustee;
        /// <summary>
        /// 默认构造器
        /// </summary>
        public WsContractsRoll(string wsClientID, string trustee= "HK LIANCHUANG ELECTRONICS., LIMITED")
        {
            this.WsClientID = wsClientID;
            this.Trustee = trustee;
        }
        protected override IQueryable<Models.Origins.WsContract> GetIQueryable()
        {
            var linq = from entity in base.GetIQueryable()
                       where entity.WsClient.ID == WsClientID && entity.Trustee == this.Trustee
                       select entity;
            return linq;
        }
    }
}
