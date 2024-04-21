using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace YaHv.Csrm.Services.Models.Origins
{
    /// <summary>
    /// 代理线企业（客户或供应商）
    /// </summary>
    public class AgentEnterprise : Yahv.Linq.IUnique
    {
        public string ID { set; get; }
        /// <summary>
        /// 企业名称
        /// </summary>
        public string Name { internal set; get; }
        public apiClient Client { set; get; }
        public apiSupplier Supplier { set; get; }

    }
    public class apiClient
    {
        /// <summary>
        /// 客户等级
        /// </summary>
        public int? Grade { internal set; get; }
        /// <summary>
        /// 客户类型
        /// </summary>
        public ClientType Type { internal set; get; }
    }
    public class apiSupplier
    {
        /// <summary>
        /// 供应商是否原厂
        /// </summary>
        public bool? IsFactory { internal set; get; }
        /// <summary>
        /// 供应商等级
        /// </summary>
        public int? Grade { internal set; get; }
    }
}
