using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.Alls
{
    /// <summary>
    /// 产品归类变更日志
    /// </summary>
    public class Logs_ClassifyModifiedAll : UniqueView<Models.Log_ClassifyModified, ScCustomsReponsitory>
    {
        public Logs_ClassifyModifiedAll()
        {
        }

        internal Logs_ClassifyModifiedAll(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Log_ClassifyModified> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Logs_ClassifyModifiedSynonymTopView>()
                   select new Models.Log_ClassifyModified
                   {
                       ID = entity.ID,
                       PartNumber = entity.PartNumber,
                       Manufacturer = entity.Manufacturer,
                       CreatorID = entity.CreatorID,
                       CreateDate = entity.CreateDate,
                       Summary = entity.Summary
                   };
        }
    }
}
