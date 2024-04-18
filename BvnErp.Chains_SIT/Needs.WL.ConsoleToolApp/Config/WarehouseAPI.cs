using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.WL.ConsoleToolApp
{
    public static class WarehouseAPI
    {
        public static string ApiName = "http://erp8.wapi.for-ic.net:60077";

        /// <summary>
        /// 未装箱产品历史数据
        /// </summary>
        public static string UnSortingList = "/wmsapi/cgsortings/show";

        /// <summary>
        /// 装箱记录
        /// </summary>
        public static string SortingInfo = "/wmsapi/cgInputReport/TinyOrderIDReportGroup";
    }
}
