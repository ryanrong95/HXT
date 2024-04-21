using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace YaHv.Csrm.Services.Models.Origins
{
    public class BusinessAdmin : Models.Origins.Admin
    {
        /// <summary>
        /// 客户ID，供应商ID
        /// </summary>
        public string RealID { set; get; }
        /// <summary>
        /// 类型
        /// </summary>
        public MapsType Type { set; get; }
        /// <summary>
        /// 默认人
        /// </summary>
        public bool IsDefault { set; get; }
    }

    public class TradingAdmin : Models.Origins.Admin
    {
        public string EnterpriseID { set; get; }
        /// <summary>
        /// 合作公司ID
        /// </summary>
        public Enterprise Company { set; get; }
        /// <summary>
        /// 默认人
        /// </summary>
        public bool IsDefault { set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { set; get; }
    }
}
