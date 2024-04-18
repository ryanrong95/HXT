using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.User.Plat.Models
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
        public ApprovalStatus Status { set; get; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { set; get; }

        /// <summary>
        /// 管理员编码
        /// </summary>
        public string AdminCode { get; set; }

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
        #endregion
    }
}
