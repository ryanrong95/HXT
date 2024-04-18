using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Models.Statistics
{
    /// <summary>
    /// 客户新增销售机会数
    /// </summary>
    public class ClientProject : IUnique
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 销售机会名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 销售机会创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}
