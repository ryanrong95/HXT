using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Services.Models
{
    public class FileDescription : IUnique
    {
        public string ID { set; get; }
        /// <summary>
        /// 企业ID
        /// </summary>
        public string EnterpriseID { set; get; }
        /// <summary>
        /// 文件名称
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 文件类型：营业执照、委托合同
        /// </summary>
        public FileType Type { set; get; }
        /// <summary>
        /// 文件格式
        /// </summary>
        public string FileFormat { set; get; }
        /// <summary>
        /// 路径
        /// </summary>
        public string Url { set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { set; get; }
        /// <summary>
        /// 状态
        /// </summary>
        public ApprovalStatus Status { set; get; }
        /// <summary>
        /// 录入人
        /// </summary>
        public string AdminID { set; get; }
    }
}
