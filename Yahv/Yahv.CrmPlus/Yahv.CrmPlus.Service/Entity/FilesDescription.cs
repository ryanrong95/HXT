using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.CrmPlus.Service.Entity
{
    public class FileDescription : Linq.IEntity
    {
        public string ID { get; set; }

        public string EnterpriseID { get; set; }
        public string SubID { set; get; }
        /// <summary>
        /// 文件类型
        /// </summary>

        public CrmFileType Type { set; get; }
        /// <summary>
        /// 文件名称
        /// </summary>
        public string CustomName { set; get; }
        /// <summary>
        /// url
        /// </summary>
        public string Url { set; get; }
        /// <summary>
        /// 创建人ID
        /// </summary>
        public string CreatorID { set; get; }
        /// <summary>
        /// 创建人真实姓名
        /// </summary>
        public string CreatorName { internal set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { internal set; get; }
        /// <summary>
        /// 状态
        /// </summary>
        public DataStatus Status { set; get; }
    }
}
