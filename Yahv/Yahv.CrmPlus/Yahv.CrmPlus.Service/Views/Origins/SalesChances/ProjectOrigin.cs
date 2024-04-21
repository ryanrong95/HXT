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

    /// <summary>
    /// 项目
    /// </summary>
    public class ProjectOrigin : Yahv.Linq.UniqueView<Project, PvdCrmReponsitory>
    {

        internal ProjectOrigin()
        {
        }
        public ProjectOrigin(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {


        }

        protected override IQueryable<Project> GetIQueryable()
        {
            var adminTopview = new AdminsAllRoll(this.Reponsitory);
            var contactView = new ContactsOrigin(this.Reponsitory);
            var clientView = new ClientsOrigin(this.Reponsitory).Where(x => x.IsDraft == false && x.Status == Underly.AuditStatus.Normal);
            var OrderclientView = new ClientsOrigin(this.Reponsitory).Where(x => x.IsDraft == false && x.Status == Underly.AuditStatus.Normal);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Projects>()
                   join client in clientView on entity.EndClientID equals client.ID
                   join contact in contactView on entity.ClientContactID equals contact.ID
                   join admin in adminTopview on entity.OwnerID equals admin.ID
                   join orderClient in OrderclientView on entity.AssignClientID equals orderClient.ID into g
                   from orderClient in g.DefaultIfEmpty()

                   select new Project
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       EstablishDate = entity.EstablishDate,
                       RDDate = entity.RDDate,
                       ProductDate = entity.ProductDate,
                       CompanyID = entity.CompanyID,
                       OwnerID = admin.ID,
                       CreateDate = entity.CreateDate,
                       ModifyDate = entity.ModifyDate,
                       Summary = entity.Summary,
                       Contact = contact,
                       ClientContactID = contact.ID,
                       Client = client,
                       EndClientID = client.ID,
                       AssignClientID=entity.AssignClientID,
                       OrderClient = orderClient,
                       Status = (DataStatus)entity.Status,

                   };
        }

    }


    public class ProjectExtendOrigin : Yahv.Linq.UniqueView<Project, PvdCrmReponsitory>
    {

        internal ProjectExtendOrigin()
        {
        }
        public ProjectExtendOrigin(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {


        }

        protected override IQueryable<Project> GetIQueryable()
        {
            var clientView = new MyClientsOrigin(this.Reponsitory).Where(x => x.IsDraft == false && x.Status == Underly.AuditStatus.Normal);

            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Projects>()
                   join client in clientView on entity.EndClientID equals client.ID
                   select new Project
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       Client=client,
                       EstablishDate = entity.EstablishDate,
                       RDDate = entity.RDDate,
                       ProductDate = entity.ProductDate,
                       CompanyID = entity.CompanyID,
                       CreateDate = entity.CreateDate,
                       ModifyDate = entity.ModifyDate,
                       Summary = entity.Summary,
                       ClientContactID = entity.ClientContactID,
                       EndClientID = entity.EndClientID,
                       AssignClientID = entity.AssignClientID,
                       Status = (DataStatus)entity.Status,

                   };
        }

    }
}
