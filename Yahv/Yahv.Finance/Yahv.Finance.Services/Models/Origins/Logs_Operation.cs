using System;
using Yahv.Finance.Services.Enums;
using Yahv.Linq;

namespace Yahv.Finance.Services.Models.Origins
{
    /// <summary>
    /// 操作日志
    /// </summary>
    public class Logs_Operation : IUnique
    {
        #region 属性
        public string ID { get; set; }

        /// <summary>
        /// 模块
        /// </summary>
        public string Modular { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 操作
        /// </summary>
        public string Operation { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        public string CreatorID { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }
        #endregion

        #region 拓展属性
        /// <summary>
        /// 创建人名称
        /// </summary>
        public string CreatorName { get; set; }
        #endregion
    }
}