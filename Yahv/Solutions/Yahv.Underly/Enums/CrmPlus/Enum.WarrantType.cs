using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    public enum WarrantType
    {
        /// <summary>
        /// 授信附件
        /// </summary>
        [Description("授信附件")]
        Credit = 1,
        /// <summary>
        /// 地址附件
        /// </summary>
        [Description("地址附件")]
        Address = 2,

        /// <summary>
        /// 关联关系
        /// </summary>
        [Description("关联关系")]
        Relation = 3,
    }
}
