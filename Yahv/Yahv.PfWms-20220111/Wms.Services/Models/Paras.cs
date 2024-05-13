using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms.Services.Models
{
    /// <summary>
    /// 根据进项编号更新信
    /// </summary>
    public class InfoByInput
    {
        public string InputID { get; set; }
        public decimal Weight { get; set; }

    }

    public class SZToXDT
    {

        public string AdminID { get; set; }
        public string WaybillID { get; set; }


        /// <summary>
        /// 通知的OrderID
        /// </summary>
        /// <remarks>
        /// 陈翰与苏亮商议增加这个数据
        /// </remarks>
        public string[] OrdersID { get; set; }

    }


}
