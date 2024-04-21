using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Origins
{
    public class WsContractsOrigin : Yahv.Linq.UniqueView<WsContract, PvbCrmReponsitory>
    {
        internal WsContractsOrigin()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal WsContractsOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<WsContract> GetIQueryable()
        {
            var enterprisesView = new EnterprisesOrigin(this.Reponsitory);
            var adminsView = new Rolls.AdminsAllRoll(this.Reponsitory);
            var linq = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.WsContracts>()
                       join client in enterprisesView on entity.WsClientID equals client.ID
                       join admin in adminsView on entity.CreatorID equals admin.ID into _admin
                       from admin in _admin.DefaultIfEmpty()
                       select new WsContract
                       {
                           ID = entity.ID,
                           WsClient = client,
                           Trustee = entity.Trustee,
                           StartDate = entity.StartDate,
                           EndDate = entity.EndDate,
                           ContainerNum = entity.ContainerNum,
                           Currency = (Currency)entity.Currency,
                           Charges = entity.Charges,
                           CreatorID = entity.CreatorID,
                           Creator = admin,
                           CreateDate = entity.CreateDate,
                           Status = (GeneralStatus)entity.Status,
                           Summary = entity.Summary,
                       };
            return linq;
        }
    }
}
