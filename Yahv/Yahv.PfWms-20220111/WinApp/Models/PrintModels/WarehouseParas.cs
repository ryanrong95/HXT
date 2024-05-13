using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinApp.Models
{
    public class WarehouseParas /*: Obj*/
    {
        /// <summary>
        /// 库位编号（条码）
        /// </summary>
        public string StorehouseID { get; set; }

        /// <summary>
        /// 库区
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// 位置
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// 负责人
        /// </summary>
        public string ManagerID { get; set; }

        /// <summary>
        /// 所有人
        /// </summary>
        public string OwnerID { get; set; }

    }
}
