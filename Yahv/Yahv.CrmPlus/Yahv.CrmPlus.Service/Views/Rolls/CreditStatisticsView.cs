using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Models.Origins.Rolls;
using Yahv.CrmPlus.Service.Views.Origins;
using Yahv.Underly;
using YaHv.CrmPlus.Services.Models.Origins;

namespace YaHv.CrmPlus.Services.Views.Rolls
{

    /// <summary>
    /// 流水统计
    /// </summary>
    public class CreditStatisticsRoll : Yahv.Linq.QueryView<CreditStatistic, PvdCrmReponsitory>
    {
        public CreditStatisticsRoll()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public CreditStatisticsRoll(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<CreditStatistic> GetIQueryable()
        {
            var enterprisesView = new EnterprisesOrigin(this.Reponsitory);
            var linq = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.CreditsStatisticsView>()
                       join e1 in enterprisesView on entity.MakerID equals e1.ID
                       join e2 in enterprisesView on entity.TakerID equals e2.ID
                       select new CreditStatistic
                       {
                           Currency = (Currency)entity.Currency,
                           Maker = e1,
                           Taker = e2,
                           CreditType = (CreditType)entity.Type,
                           Total = entity.Total,
                           Cost = entity.Cost
                       };
            return linq;
        }
        public IQueryable<CreditStatistic> this[CreditType type]
        {
            get { return this.GetIQueryable().Where(item => item.CreditType == type); }
        }
    }

}
