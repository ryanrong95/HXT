using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;

namespace YaHv.PvData.Services.Views.Alls
{
    /// <summary>
    /// 产地加征税率
    /// </summary>
    public class OriginsATRateAll : UniqueView<Models.OriginATRate, PvDataReponsitory>
    {
        public OriginsATRateAll()
        {
        }

        internal OriginsATRateAll(PvDataReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.OriginATRate> GetIQueryable()
        {
            var originsATRateView = new Origins.OriginsATRateOrigin(this.Reponsitory).Where(or => or.Status == GeneralStatus.Normal);
            var tariffsView = new Origins.TariffsOrigin(this.Reponsitory).Where(t => t.Status == GeneralStatus.Normal);

            return from entity in originsATRateView
                   join tariff in tariffsView on entity.TariffID equals tariff.ID
                   select new Models.OriginATRate
                   {
                       ID = entity.ID,
                       TariffID = entity.TariffID,
                       Tariff = tariff,
                       Origin = entity.Origin,
                       Rate = entity.Rate,
                       StartDate = entity.StartDate,
                       EndDate = entity.EndDate,
                       Status = entity.Status,
                       CreateDate = entity.CreateDate,
                       ModifyDate = entity.ModifyDate
                   };
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="hsCode">海关编码</param>
        /// <param name="origin">原产地</param>
        /// <returns></returns>
        public Models.OriginATRate this[string hsCode, string origin]
        {
            get
            {
                var curDate = DateTime.Parse(DateTime.Now.ToShortDateString());
                return this.FirstOrDefault(item => item.TariffID == hsCode && item.Origin == origin &&
                                                   item.StartDate <= curDate && (item.EndDate == null || item.EndDate >= curDate));
            }
        }
    }
}
