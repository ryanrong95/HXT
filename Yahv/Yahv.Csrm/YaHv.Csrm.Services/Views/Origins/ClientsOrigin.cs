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
    public class ClientsOrigin : Yahv.Linq.UniqueView<Models.Origins.Client, PvbCrmReponsitory>
    {
        internal ClientsOrigin()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal ClientsOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Client> GetIQueryable()
        {
            var enterpriseView = new Origins.EnterprisesOrigin(this.Reponsitory);
            var adminsView = new Rolls.AdminsAllRoll(this.Reponsitory);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Clients>()
                   join enterprise in enterpriseView on entity.ID equals enterprise.ID
                   join admin in adminsView on entity.AdminID equals admin.ID
                   select new Client()
                   {
                       ID = entity.ID,
                       Nature = (ClientType)entity.Nature,//客户性质：终端、贸易
                       DyjCode = entity.DyjCode,
                       TaxperNumber = entity.TaxperNumber,
                       Grade = (ClientGrade)entity.Grade,
                       Vip = (VIPLevel)entity.Vip,
                       AreaType = (AreaType)entity.AreaType,//客户类型：国内、国际
                       ClientStatus = (ApprovalStatus)entity.Status,
                       Enterprise = enterprise,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       CreatorID=entity.AdminID,
                       Creator = admin,
                       Place = entity.Place,
                       Major = entity.Major//是否重点客户
                   };
        }
    }


    public class TradingClientsOrigin : Yahv.Linq.UniqueView<Models.Origins.TradingClient, PvbCrmReponsitory>
    {
        internal TradingClientsOrigin()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal TradingClientsOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<TradingClient> GetIQueryable()
        {
            var enterpriseView = new Origins.EnterprisesOrigin(this.Reponsitory);
            var adminsView = new Rolls.AdminsAllRoll(this.Reponsitory);
            //暂不获取合作公司的销售
            //var tradingSalesView = new Rolls.TradingAdminsRoll(this.Reponsitory, MapsType.Client, Business.Trading_Sale);//客户的销售人员

            var tradingSalesView = new Rolls.TradingAdminsRoll(this.Reponsitory, MapsType.Client);//客户的销售人员
            var linq = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Clients>()

                       join enterprise in enterpriseView on entity.ID equals enterprise.ID

                       join admin in adminsView on entity.AdminID equals admin.ID

                       join sale in tradingSalesView on entity.ID equals sale.EnterpriseID into sales

                       select new TradingClient
                       {
                           ID = entity.ID,
                           Nature = (ClientType)entity.Nature,//客户性质：终端、贸易
                           DyjCode = entity.DyjCode,
                           TaxperNumber = entity.TaxperNumber,
                           Grade = (ClientGrade)entity.Grade,
                           Vip = (VIPLevel)entity.Vip,
                           AreaType = (AreaType)entity.AreaType,//客户类型：国内、国际
                           ClientStatus = (ApprovalStatus)entity.Status,
                           Enterprise = enterprise,
                           CreateDate = entity.CreateDate,
                           UpdateDate = entity.UpdateDate,
                           CreatorID = entity.AdminID,
                           Creator = admin,
                           Sales = sales,
                           Place = entity.Place,
                           Major=entity.Major//是否重点客户
                       };
            return linq;
        }
    }
}
