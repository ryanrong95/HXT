using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 来源
    /// </summary>
    //[EnumNaming("企业来源")]
    public enum FixedSource
    {
        #region Source
        /// <summary>
        /// 展会
        /// </summary>
        [Fixed("FSource01")]
        [Description("展会")]
        Expo = 1,

        /// <summary>
        /// phone
        /// </summary>
        [Fixed("FSource02")]
        [Description("电话")]
        Phone = 2,
        /// <summary>
        /// 网络
        /// </summary>
        [Fixed("FSource03")]
        [Description("网络")]
        Network = 3,
        /// <summary>
        /// 原厂推荐
        /// </summary>
        [Fixed("FSource04")]
        [Description("原厂推荐")]
        OriginalFactory = 4,

        /// <summary>
        /// 销售经理
        /// </summary>
        [Fixed("FSource05")]
        [Description("方案导入")]
        PlanImport =5,

        #endregion
    }
}
