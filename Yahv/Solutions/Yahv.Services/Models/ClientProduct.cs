using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Services.Models
{
    /// <summary>
    /// 客户端我的产品
    /// </summary>
    public class ClientProduct : IUnique
    {
        #region 属性
        /// <summary>
        /// ID
        /// </summary>
        public string ID{ set; get; }

        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID { set; get; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// 型号
        /// </summary>
        public string Model { set; get; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Manufacturer { set; get; }

        /// <summary>
        /// 批号
        /// </summary>
        public string Batch { set; get; }

        /// <summary>
        /// 状态
        /// </summary>
        public GeneralStatus Status { set; get; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime UpdateDate { get; set; }

        #endregion
    }
}
