using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Underly;

namespace Yahv.CrmPlus.Service.Views.Origins
{
    public class SampleItemOrigin : Yahv.Linq.UniqueView<SampleItem, PvdCrmReponsitory>
    {

        internal SampleItemOrigin()
        {
        }
        public SampleItemOrigin(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {


        }

        protected override IQueryable<SampleItem> GetIQueryable()
        {

            var standardPartNumberView = new StandardPartNumbersOrigin(this.Reponsitory);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.SampleItems>()
                   join standardPartNumber in standardPartNumberView on entity.SpnID equals standardPartNumber.ID
                   select new SampleItem
                   {
                       ID = entity.ID,
                       SampleID = entity.SampleID,
                       SpnID = entity.SpnID,
                       SpnName=standardPartNumber.PartNumber,
                       Brand=standardPartNumber.Brand,
                       SampleType = (SampleType)entity.Type,
                       Quantity = entity.Quantity,
                       Price = entity.Price,
                     //  Total = (entity.Quantity) * (entity.Price),
                       CreateDate = entity.CreateDate,
                       ModifyDate = entity.ModifyDate,
                       AuditStatus = (AuditStatus)entity.Status,
                       Summary = entity.Summary,

                   };
        }

    }
}
