using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class CenterVault
    {
        /// <summary>
        /// 金库名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 金库原来名称 修改的时候用
        /// </summary>
        public string OriginName { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public string OwnerID { get; set; }
        /// <summary>
        /// 操作ID
        /// </summary>
        public string CreatorID { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Summary { get; set; }
    }
}
