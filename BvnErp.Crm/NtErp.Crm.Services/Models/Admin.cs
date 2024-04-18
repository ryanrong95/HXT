using System;
using Needs.Linq;
using System.Linq;
using NtErp.Crm.Services.Extends;
using NtErp.Crm.Services.Enums;

namespace NtErp.Crm.Services.Models
{
    public partial class Admin : IUnique
    {
        #region 属性
        /// <summary>
        /// 主键ID
        /// </summary>
        public string ID
        {
            get;set;
        }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName
        {
            get; set;
        }

        /// <summary>
        /// 真实名称
        /// </summary>
        public string RealName
        {
            get; set;
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate
        {
            get; set;
        }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateDate
        {
            get; set;
        }

        /// <summary>
        /// 状态
        /// </summary>
        public Status Status
        {
            get; set;
        }

        /// <summary>
        /// 描述
        /// </summary>
        public string Summary
        {
            get; set;
        }
        #endregion
    }
}
