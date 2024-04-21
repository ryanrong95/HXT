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
    public class CopWsClientsOrigin : Yahv.Linq.UniqueView<CopWsClient, PvbCrmReponsitory>
    {
        internal CopWsClientsOrigin()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal CopWsClientsOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<CopWsClient> GetIQueryable()
        {
            var enterprisesView = new EnterprisesOrigin(this.Reponsitory);
            var adminsView = new Rolls.AdminsAllRoll(this.Reponsitory);

            var mapsView = Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MapsCoperation>();
            var linq = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.CopWsClients>()

                       join maps in mapsView on entity.CoperID equals maps.ID

                       join company in enterprisesView on maps.SubID equals company.ID
                       join client in enterprisesView on maps.MainID equals client.ID

                       // join _admin in adminsView on entity. equals _admin.ID into _admin
                       // from admin in _admin.DefaultIfEmpty()
                       select new CopWsClient
                       {
                           ID = entity.ID,
                           ClientID = client.ID,
                           Vip = entity.Vip,
                           EnterCode = entity.EnterCode,
                           CustomsCode = entity.CustomsCode,
                           Grade = entity.Grade,
                           Status = (ApprovalStatus)entity.Status,
                           // WsClientStatus = (Yahv.Underly.ApprovalStatus)entity.Status,
                           // Summary = entity.Summary,
                           //Enterprise = client,
                           //Company = company,
                           //CreatorID = admin.ID,
                           //Admin = admin,
                           Name = client.Name,
                           RegAddress = client.RegAddress,
                           Corperation = client.Corporation,
                           Uscc = client.Uscc,
                           CompanyID = company.ID,
                           CompanyName = company.Name,
                           CreateDate = entity.CreateDate,
                           UpdateDate = entity.UpdateDate,
                           Nature = (ClientType)entity.Nature,
                           CreatorID = entity.AdminID,
                           //Origin =(Origin) entity.Origin
                       };
            return linq;

        }
    }
}
