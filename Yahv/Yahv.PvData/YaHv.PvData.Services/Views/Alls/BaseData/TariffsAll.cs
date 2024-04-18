using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;
using YaHv.PvData.Services.Models;

namespace YaHv.PvData.Services.Views.Alls
{
    /// <summary>
    /// 海关税则
    /// </summary>
    public class TariffsAll : UniqueView<Models.Tariff, PvDataReponsitory>
    {
        public TariffsAll()
        {
        }

        internal TariffsAll(PvDataReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Tariff> GetIQueryable()
        {
            return new Origins.TariffsOrigin(this.Reponsitory).Where(t => t.Status == GeneralStatus.Normal);
        }

        public override Tariff this[string hsCode]
        {
            get
            {
                return this.SingleOrDefault(item => item.HSCode == hsCode.Trim());
            }
        }
    }
}
