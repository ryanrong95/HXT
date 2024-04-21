using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Services.Enums;

namespace Wms.Services.chonggous.Models
{
    /// <summary>
    /// 库存流水
    /// </summary>
    public class CgForm
    {
        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 库存ID
        /// </summary>
        public string StorageID { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal Quantity { get; set; }
        /// <summary>
        /// 通知ID
        /// </summary>
        public string NoticeID { get; set; }
        /// <summary>
        /// 流水状态: 冻结，真实的(真正执行的)
        /// </summary>
        public FormStatus Status { get; set; }
        /// <summary>
        /// 按照约定: InputID  OutputID  AdminID
        /// </summary>
        public String SessoinID { get; set; }
    }
}
