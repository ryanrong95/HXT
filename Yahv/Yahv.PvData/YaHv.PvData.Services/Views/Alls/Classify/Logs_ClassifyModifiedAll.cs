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
    /// 产品归类变更日志
    /// </summary>
    public class Logs_ClassifyModifiedAll : UniqueView<Models.Log_ClassifyModified, PvDataReponsitory>
    {
        public Logs_ClassifyModifiedAll()
        {
        }

        internal Logs_ClassifyModifiedAll(PvDataReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Log_ClassifyModified> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvData.Logs_ClassifyModified>()
                   orderby entity.CreateDate descending
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
