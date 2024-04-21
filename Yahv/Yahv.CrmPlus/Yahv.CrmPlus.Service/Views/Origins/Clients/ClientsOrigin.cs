using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Underly;
using YaHv.CrmPlus.Services.Views.Rolls;

namespace Yahv.CrmPlus.Service.Views.Origins
{
    public class ClientsOrigin : Yahv.Linq.UniqueView<Models.Origins.Client, PvdCrmReponsitory>
    {

        internal ClientsOrigin()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal ClientsOrigin(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Client> GetIQueryable()
        {
            var enterprisesView = new EnterprisesOrigin(this.Reponsitory);
            var adminsView = new AdminsAllRoll(this.Reponsitory);
          

            var registerView = new EnterpriseRegistersOrigin(this.Reponsitory);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Clients>()
                   join enterprise in enterprisesView on entity.ID equals enterprise.ID
                   join register in registerView on entity.ID equals register.ID
                   join admin in adminsView on entity.Owner equals admin.ID into g
                   from admin in g.DefaultIfEmpty()
                   select new Client
                   {
                       ID = entity.ID,
                       ClientGrade = (ClientGrade)entity.Grade,
                       ClientType = (Yahv.Underly.CrmPlus.ClientType)entity.Type,
                       Vip = (Yahv.Underly.VIPLevel)entity.Vip,
                       Source = entity.Source,
                       IsMajor = entity.IsMajor,
                       IsSpecial = entity.IsSpecial,
                       Industry = entity.Industry,
                       CreateDate = entity.CreateDate,
                       ModifyDate = entity.ModifyDate,
                       IsSupplier = entity.IsSupplier,
                       ProfitRate = entity.ProfitRate,
                       Name = enterprise.Name,
                       IsDraft=enterprise.IsDraft,
                       Place=enterprise.Place,
                       District=enterprise.District,
                       Grade=enterprise.Grade,
                       Summary=enterprise.Summary,
                      // Enterprise = enterprise,
                       EnterpriseRegister = register,
                       Owner = admin.ID,
                       Admin = admin,
                       Status = (AuditStatus)enterprise.Status,
                       ClientStatus= (AuditStatus)entity.Status,
                   };
        }

    }


    public class MyClientsOrigin : Yahv.Linq.UniqueView<Models.Origins.Client, PvdCrmReponsitory>
    {

        internal MyClientsOrigin(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {

        }
        protected override IQueryable<Client> GetIQueryable()
        {
            var enterprisesView = new EnterprisesOrigin(this.Reponsitory);
            var adminsView = new AdminsAllRoll(this.Reponsitory);
            var registerView = new EnterpriseRegistersOrigin(this.Reponsitory);
            var mapTopNView = new MapsTopNOrigin(this.Reponsitory);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Clients>()
                   join enterprise in enterprisesView on entity.ID equals enterprise.ID
                   join register in registerView on entity.ID equals register.ID
                   join mapTop in mapTopNView on entity.ID equals mapTop.ClientID into mapTops
                   from mapTop in  mapTops.DefaultIfEmpty()
                   join admin in adminsView on entity.Owner equals admin.ID into g
                   from admin in g.DefaultIfEmpty()
                   select new Client
                   {
                       ID = entity.ID,
                       ClientGrade = (ClientGrade)entity.Grade,
                       ClientType = (Yahv.Underly.CrmPlus.ClientType)entity.Type,
                       Vip = (Yahv.Underly.VIPLevel)entity.Vip,
                       Source = entity.Source,
                       IsMajor = entity.IsMajor,
                       IsSpecial = entity.IsSpecial,
                       Industry = entity.Industry,
                       CreateDate = entity.CreateDate,
                       ModifyDate = entity.ModifyDate,
                       IsSupplier = entity.IsSupplier,
                       ProfitRate = entity.ProfitRate,

                       //企业信息
                       Name = enterprise.Name,
                       Grade = enterprise.Grade,
                       IsDraft = enterprise.IsDraft,
                       Place = enterprise.Place,
                       District = enterprise.District,
                       Summary = enterprise.Summary,
                       Status = (AuditStatus)enterprise.Status,

                       EnterpriseRegister = register,
                       Owner = admin.ID,
                       Admin = admin,
                       MapsTopN=mapTop,
                       ClientStatus=(AuditStatus)entity.Status
                      
                   };
        }



    }
     
    /// <summary>
    /// 公海客户
    /// </summary>

    //public class PublicClientsOrigin : Yahv.Linq.UniqueView<Models.Origins.Client, PvdCrmReponsitory>
    //{

    //    public PublicClientsOrigin()
    //    {
    //    }
    //    protected override IQueryable<Models.Origins.Client> GetIQueryable()
    //    {
    //        var enterprisesView = new EnterprisesOrigin(this.Reponsitory);
    //         var clientsView = new ClientsOrigin(this.Reponsitory);
    //        var registerView = new EnterpriseRegistersOrigin(this.Reponsitory);
    //        var conductView = new ConductsOrigin(this.Reponsitory).Where(x=>x.IsPublic==true);
    //        var relationView = new RelationsOrigin(this.Reponsitory);
    //        return from conduct in conductView
    //               join relation in relationView on conduct.EnterpriseID equals relation.ClientID
    //               join entity in clientsView on conduct.EnterpriseID equals entity.ID
    //               select new Client
    //               {

    //                   ID = entity.ID,
    //                   Grade = (ClientGrade)entity.Grade,
    //                   ClientType = (Yahv.Underly.CrmPlus.ClientType)entity.ClientType,
    //                   Vip = (Yahv.Underly.CrmPlus.VIPLevel)entity.Vip,
    //                   Source = entity.Source,
    //                   IsMajor = entity.IsMajor,
    //                   IsSpecial = entity.IsSpecial,
    //                   Industry = entity.Industry,
    //                   CreateDate = entity.CreateDate,
    //                   ModifyDate = entity.ModifyDate,
    //                   IsSupplier = entity.IsSupplier,
    //                   ProfitRate = entity.ProfitRate,
    //                   Enterprise = entity.Enterprise,
    //                   EnterpriseRegister = entity.EnterpriseRegister,
    //                   Conduct = conduct,
    //                   Status = (AuditStatus)entity.Status,
    //                   Relation = relation,
    //               };



    //        //return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Clients>()
    //        //       join enterprise in enterprisesView on entity.ID equals enterprise.ID
    //        //       join register in registerView on entity.ID equals register.ID
    //        //       join conduct in conductView on entity.ID equals conduct.EnterpriseID into g
    //        //       from conduct in g.DefaultIfEmpty()
    //        //       join relation in relationView on conduct.EnterpriseID equals relation.ClientID into relations
    //        //       from relation in relations.DefaultIfEmpty()
    //        //       select new Client
    //        //       {
    //        //           ID = entity.ID,
    //        //           Grade = (ClientGrade)entity.Grade,
    //        //           ClientType = (Yahv.Underly.CrmPlus.ClientType)entity.Type,
    //        //           Vip = (Yahv.Underly.CrmPlus.VIPLevel)entity.Vip,
    //        //           Source = entity.Source,
    //        //           IsMajor = entity.IsMajor,
    //        //           IsSpecial = entity.IsSpecial,
    //        //           Industry = entity.Industry,
    //        //           CreateDate = entity.CreateDate,
    //        //           ModifyDate = entity.ModifyDate,
    //        //           IsSupplier = entity.IsSupplier,
    //        //           ProfitRate = entity.ProfitRate,
    //        //           Enterprise = enterprise,
    //        //           EnterpriseRegister = register,
    //        //           Conduct = conduct,
    //        //           Status = (AuditStatus)entity.Status,
    //        //           Relation=relation,
    //        //       };
    //    }

    //}

}
