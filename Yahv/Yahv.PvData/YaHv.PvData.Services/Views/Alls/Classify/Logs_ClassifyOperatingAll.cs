using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace YaHv.PvData.Services.Views.Alls
{
    public class Logs_ClassifyOperatingAll : UniqueView<Models.Log_ClassifyOperating, PvDataReponsitory>
    {
        public Logs_ClassifyOperatingAll()
        {
        }

        internal Logs_ClassifyOperatingAll(PvDataReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Log_ClassifyOperating> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvData.Logs_ClassifyOperating>()
                   orderby entity.CreateDate descending
                   select new Models.Log_ClassifyOperating
                   {
                       ID = entity.ID,
                       MainID = entity.MainID,
                       CreatorID = entity.CreatorID,
                       LogType = (LogType)entity.LogType,
                       CreateDate = entity.CreateDate,
                       Summary = entity.Summary
                   };
        }
    }
}
