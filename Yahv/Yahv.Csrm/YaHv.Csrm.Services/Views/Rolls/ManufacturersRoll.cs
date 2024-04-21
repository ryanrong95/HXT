using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Rolls
{
    /// <summary>
    /// 供应商，客户，Admin的优势品牌
    /// </summary>
    public class ManufacturersRoll : Origins.ManufacturersOrigin
    {
        public ManufacturersRoll()
        {

        }
        protected override IQueryable<Manufacturer> GetIQueryable()
        {
            return base.GetIQueryable();
        }
    }
}
