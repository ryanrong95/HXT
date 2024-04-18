using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PsWms.PvRoute.Services.Models
{
    /// <summary>
    /// 货物信息
    /// </summary>
    public class Commodity
    {
        public string GoodsName { get; set; }
        public string GoodsCode { get; set; }
        public long Goodsquantity { get; set; }
        public double GoodsPrice { get; set; }
        public double GoodsWeight { get; set; }
        public string GoodsDesc { get; set; }
        public double GoodsVol { get; set; }
    }
}
