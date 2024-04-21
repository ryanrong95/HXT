using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.Enums;
using Yahv.Underly;

namespace WebApp.Services
{
    public class File
    {
        /// <summary>
        /// 运单ID
        /// </summary>
        public string WaybillID { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string CustomName { get; set; }
        /// <summary>
        /// 文件类型
        /// </summary>
        public FileType Type { get; set; }
        /// <summary>
        /// 文件地址(自动变更名称)
        /// </summary>
        public string Url { get; set; } 
 
        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID { get; set; }
        /// <summary>
        /// 添加人
        /// </summary>
        public string AdminID { get; set; }


    }
}
