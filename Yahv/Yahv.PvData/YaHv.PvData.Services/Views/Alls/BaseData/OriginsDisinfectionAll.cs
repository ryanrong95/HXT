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
    /// 产地消毒/检疫
    /// </summary>
    public class OriginsDisinfectionAll : UniqueView<Models.OriginDisinfection, PvDataReponsitory>
    {
        public OriginsDisinfectionAll()
        {
        }

        internal OriginsDisinfectionAll(PvDataReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.OriginDisinfection> GetIQueryable()
        {
            return new Origins.OriginsDisinfectionOrigin(this.Reponsitory).Where(od => od.Status == GeneralStatus.Normal);
        }
    }
}
