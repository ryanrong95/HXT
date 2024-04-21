using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using YaHv.VcCsrm.Service.Models;

namespace YaHv.VcCsrm.Service.Views.Origins
{
    public class WsContractsOrigin : Yahv.Linq.UniqueView<WsContract, PvcCrmReponsitory>
    {
        internal WsContractsOrigin()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal WsContractsOrigin(PvcCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<WsContract> GetIQueryable()
        {
            var enterprisesView = new EnterprisesOrigin(this.Reponsitory);
           // var adminsView = new Rolls.AdminsAllRoll(this.Reponsitory);
            var linq = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvcCrm.WsContracts>()
                       //join admin in adminsView on entity.CreatorID equals admin.ID into _admin
                       //from admin in _admin.DefaultIfEmpty()
                       select new WsContract
                       {
                           ID = entity.ID,
                           WsClientID = entity.WsClientID,
                           TrusteeID = entity.Trustee,
                           StartDate = entity.StartDate,
                           EndDate = entity.EndDate,
                           ContainerNum = entity.ContainerNum,
                           Currency = (Currency)entity.Currency,
                           Charges = entity.Charges,
                           CreatorID = entity.CreatorID,
                           //Creator = admin,
                           CreateDate = entity.CreateDate,
                           Status = (GeneralStatus)entity.Status,
                           Summary = entity.Summary,
                       };
            return linq;
        }
    }
}
