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
    /// 子系统的归类结果
    /// </summary>
    public class SubSystemClassifiedResultsAll : UniqueView<Models.SubSystemClassifiedResult, PvDataReponsitory>
    {
        public SubSystemClassifiedResultsAll()
        {
        }

        internal SubSystemClassifiedResultsAll(PvDataReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.SubSystemClassifiedResult> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvData.MapsSystem>()
                   orderby entity.ModifyDate descending
                   select new Models.SubSystemClassifiedResult
                   {
                       ID = entity.ID,
                       MainID = entity.MainID,
                       ItemID = entity.ItemID,
                       Step = (ClassifyStep)entity.Step,
                       CpnID = entity.CpnID,
                       ClientName = entity.ClientName,
                       ClientCode = entity.ClientCode,
                       OrderedDate = entity.OrderedDate,
                       PIs = entity.PIs,
                       CallBackUrl = entity.CallBackUrl,
                       Unit = entity.Unit,
                       CustomName = entity.CustomName,
                       CreateDate = entity.CreateDate,
                       ModifyDate = entity.ModifyDate,
                       Summary = entity.Summary
                   };
        }
    }
}
