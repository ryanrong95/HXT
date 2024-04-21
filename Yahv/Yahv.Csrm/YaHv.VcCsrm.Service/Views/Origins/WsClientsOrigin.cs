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
    public class WsClientsOrigin : Yahv.Linq.UniqueView<Models.WsClient, PvcCrmReponsitory>
    {
        internal WsClientsOrigin()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal WsClientsOrigin(PvcCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Models.WsClient> GetIQueryable()
        {
            var enterprisesView = new EnterprisesOrigin(this.Reponsitory);
            var adminsView = new Views.Rolls.AdminsAllRoll(this.Reponsitory);

            var partnerView = Reponsitory.ReadTable<Layers.Data.Sqls.PvcCrm.ShipsPartner>();
            var linq = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvcCrm.WsClients>()
                       join partner in partnerView on entity.ID equals partner.ID
                       join client in enterprisesView on partner.MainID equals client.ID
                       join company in enterprisesView on partner.SubID equals company.ID
                       select new Models.WsClient
                       {
                           ID = entity.ID,
                           ClientID = client.ID,
                           Name = client.Name,
                           CompanyID = company.ID,
                           CompanyName = company.Name,
                           Type = (Business)partner.Type,
                           Vip = entity.Vip,
                           EnterCode = entity.EnterCode,
                           CustomsCode = entity.CustomsCode,
                           Grade = entity.Grade,
                           RegAddress = client.RegAddress,
                           Corperation = client.Corporation,
                           Uscc = client.Uscc,
                           Nature = (ClientType)entity.Nature,
                           Origin = entity.Origin,
                           Status = (ApprovalStatus)entity.Status,
                           CreatorID = entity.AdminID,
                           CreateDate = entity.CreateDate,
                           UpdateDate = entity.UpdateDate,
                       };
            return linq;

        }
    }
}
