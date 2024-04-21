using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.Underly;
using YaHv.CrmPlus.Service.Views.Origins;
using YaHv.CrmPlus.Services.Views.Rolls;

namespace Yahv.CrmPlus.Service.Views.Origins
{

    public class SampleOrigin : Yahv.Linq.UniqueView<Sample, PvdCrmReponsitory>
    {

        internal SampleOrigin()
        {
        }
        public SampleOrigin(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {


        }

        protected override IQueryable<Sample> GetIQueryable()
        {
            var projectView = new ProjectExtendOrigin(this.Reponsitory);
            var adminTopview = new AdminsAllRoll(this.Reponsitory);
            var contactView = new  ContactsOrigin(this.Reponsitory);
            var addressView = new AddressesOrigin(this.Reponsitory);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Samples>()
                   join project in projectView on entity.ProjectID equals project.ID
                   join contact in contactView on  entity.ContactID equals contact.ID
                   join address in addressView on entity.AddressID equals address.ID
                   join applier in adminTopview on entity.ApplierID equals applier.ID into g
                   from applier in g.DefaultIfEmpty()
                   select new Sample
                   {
                       ID = entity.ID,
                       ProjectID = entity.ProjectID,
                       Project = project,
                       ApplierID = entity.ApplierID,
                       ApproverID = entity.ApproverID,
                       ContactID = entity.ContactID,
                       WaybillCode = entity.WaybillCode,
                       Freight = entity.Freight,
                       DeliveryDate = entity.DeliveryDate,
                       CreateDate = entity.CreateDate,
                       ModifyDate = entity.ModifyDate,
                       AuditStatus = (AuditStatus)entity.Status,
                       Applier = applier,
                       Contact=contact,
                       Address=address,
                       Summary=entity.Summary
                   };
        }

    }
}
