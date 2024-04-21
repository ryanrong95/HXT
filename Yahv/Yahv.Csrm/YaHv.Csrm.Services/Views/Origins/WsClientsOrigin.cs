using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using Yahv.Underly.Enums;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Origins
{
    /// <summary>
    /// 代仓储客户
    /// </summary>
    public class WsClientsOrigin : Yahv.Linq.UniqueView<WsClient, PvbCrmReponsitory>
    {
        internal WsClientsOrigin()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal WsClientsOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<WsClient> GetIQueryable()
        {
            var enterprisesView = new EnterprisesOrigin(this.Reponsitory);
            var adminsView = new Rolls.AdminsAllRoll(this.Reponsitory);

            var mapsView = Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Where(item => item.Bussiness == (int)Business.WarehouseServicing && item.Type == (int)MapsType.WsClient);
            var linq = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.WsClients>()

                       join maps in mapsView on entity.ID equals maps.EnterpriseID

                       join company in enterprisesView on maps.SubID equals company.ID
                       join client in enterprisesView on maps.EnterpriseID equals client.ID

                       join _admin in adminsView on maps.CtreatorID equals _admin.ID into _admin
                       from admin in _admin.DefaultIfEmpty()
                       select new WsClient
                       {
                           ID = entity.ID,
                           Vip = entity.Vip,
                           EnterCode = entity.EnterCode,
                           CustomsCode = entity.CustomsCode,
                           Grade = (Yahv.Underly.ClientGrade)entity.Grade,
                           WsClientStatus = (Yahv.Underly.ApprovalStatus)entity.Status,
                           Summary = entity.Summary,
                           Enterprise = client,
                           Company = company,
                           CreatorID = admin.ID,
                           Admin = admin,
                           CreateDate = entity.CreateDate,
                           UpdateDate = entity.UpdateDate,
                           Nature = (ClientType)entity.Nature,
                           Place = entity.Place,
                           ServiceType = (ServiceType)entity.ServiceType,
                           IsDeclaretion=entity.IsDeclaretion,
                           IsStorageService=entity.IsStorageService,
                           StorageType = (WsIdentity)entity.StorageType,
                           ChargeWHType=(ChargeWHType)entity.ChargeWH
                       };
            return linq;
            //return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.WsClients>()
            //       join enterprises in enterprisesView on entity.ID equals enterprises.ID
            //       join _admin in adminsView on entity.AdminID equals _admin.ID into _admin
            //       from admin in _admin.DefaultIfEmpty()
            //       select new WsClient
            //       {
            //           ID = entity.ID,
            //           Vip = entity.Vip,
            //           EnterCode = entity.EnterCode,
            //           CustomsCode = entity.CustomsCode,
            //           Grade = (Yahv.Underly.ClientGrade)entity.Grade,
            //           WsClientStatus = (Yahv.Underly.ApprovalStatus)entity.Status,
            //           Summary = entity.Summary,
            //           Enterprise = enterprises,
            //           Admin = admin,
            //           CreateDate = entity.CreateDate,
            //           UpdateDate = entity.UpdateDate
            //       };
        }
    }
}

