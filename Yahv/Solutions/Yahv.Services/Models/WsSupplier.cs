using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Services.Models
{
    public class WsSupplier : Enterprise
    {
        #region 属性
        /// <summary>
        /// 中文名称
        /// </summary>
        public string ChineseName { set; get; }
        /// <summary>
        /// 英文名称
        /// </summary>
        public string EnglishName { set; get; }
        /// <summary>
        /// 等级
        /// </summary>
        public SupplierGrade Grade { set; get; }
        /// <summary>
        /// 状态
        /// </summary>
        public GeneralStatus Status { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { set; get; }
        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID { set; get; }

        public DateTime CreateDate { get; set; }

        #endregion
    }
}
