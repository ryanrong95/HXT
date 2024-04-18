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
    /// 产品归类特殊类型
    /// </summary>
    public class OthersAll : UniqueView<Models.Other, PvDataReponsitory>
    {
        public OthersAll()
        {
        }

        internal OthersAll(PvDataReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Other> GetIQueryable()
        {
            return new Origins.OthersOrigin(this.Reponsitory);
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="partNumber">型号</param>
        /// <param name="manufacturer">品牌/制造商</param>
        /// <returns></returns>
        public Models.Other this[string partNumber, string manufacturer]
        {
            get
            {
                return this.FirstOrDefault(cpn => cpn.PartNumber == partNumber && cpn.Manufacturer == manufacturer);
            }
        }
    }
}
