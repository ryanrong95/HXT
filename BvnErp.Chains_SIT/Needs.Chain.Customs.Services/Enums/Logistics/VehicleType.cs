using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
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

    /// <summary>
    /// 运输条款类型
    /// </summary>
    public enum TransportTermType
    {
        /// <summary>
        /// 港到港
        /// </summary>
        PortToPort = 10,

        /// <summary>
        /// 门到门
        /// </summary>
        DoorToDoor = 27,

        /// <summary>
        /// 门到点
        /// </summary>
        DoorToPier = 28,

        /// <summary>
        /// 点到门
        /// </summary>
        PierToDoor = 29,

        /// <summary>
        /// 点到点
        /// </summary>
        PierToPier = 30,
    }
}
