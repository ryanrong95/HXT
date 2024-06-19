using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Services.Models
{
    public class WsClient : Enterprise
    {
        #region 属性

        /// <summary>
        /// 等级
        /// </summary>
        public ClientGrade Grade { set; get; }
       
        /// <summary>
        /// 是否Vip
        /// </summary>
        public bool Vip { set; get; }
        
        /// <summary>
        /// 入仓号
        /// </summary>
        public string EnterCode { set; get; }
        
        /// <summary>
        /// 海关编码
        /// </summary>
        public string CustomsCode { set; get; }
        
        /// <summary>
        /// 状态
        /// </summary>
        public new ApprovalStatus Status { set; get; }
        
        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { set; get; }

        /// <summary>
        /// 管理员编码
        /// </summary>
        public new string AdminCode { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string AdminID { get; set; }
        /// <summary>
        /// 性质
        /// </summary>
        public ClientType ClientNatue { set; get; }
        /// <summary>
        /// 业务（服务类型）
        /// </summary>
        public ServiceType ServiceType { set; get; }
        /// <summary>
        /// 是否已报关审批
        /// </summary>

        public bool IsDeclaretion { set; get; }
        /// <summary>
        /// 是否已仓储审批
        /// </summary>

        public bool IsStorageService { set; get; }
        /// <summary>
        /// 仓储类型/客户身份
        /// </summary>
        public WsIdentity StorageType { set; get; }

        /// <summary>
        /// 是否含有出口业务
        /// </summary>
        public bool? HasExport { set; get; }

        #endregion
    }

    public class WsClientManager: WsClient
    {
        /// <summary>
        /// 业务员ID
        /// </summary>
        public string ServiceManagerID { set; get; }
        /// <summary>
        /// 业务员姓名
        /// </summary>
        public string ServiceManagerName { set; get; }
    }
}
