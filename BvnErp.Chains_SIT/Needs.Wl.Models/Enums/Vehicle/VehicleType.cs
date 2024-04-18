using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Models.Enums
{
    public enum VehicleType
    {
        /// <summary>
        /// 车辆类型
        /// </summary>
        [Description("面包车")]
        TheVan = 1,

        [Description("厢式货车")]
        BoxTruck = 2,

        [Description("七座小客车")]
        Minibuses = 3,

        [Description("3吨车")]
        ThreeTonsCar = 4,

        [Description("5吨车")]
        FiveTonsCar = 5,

        [Description("8吨车")]
        EightTonsCar = 6,

        [Description("10吨车")]
        TenTonsCar = 7,

        [Description("12吨车")]
        TwelveTonsCar = 8,
    }
}
