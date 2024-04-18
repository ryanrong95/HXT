using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace YaHv.PvData.Services.Views.Alls
{
    /// <summary>
    /// 归类历史数据变更日志
    /// </summary>
    public class Pasts_ClassifiedModifiedAll : UniqueView<Models.Past_ClassifiedModified, PvDataReponsitory>
    {
        public Pasts_ClassifiedModifiedAll()
        {
        }

        internal Pasts_ClassifiedModifiedAll(PvDataReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Past_ClassifiedModified> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvData.Pasts_ClassifiedModified>()
                   orderby entity.CreateDate descending
                   select new Models.Past_ClassifiedModified
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
