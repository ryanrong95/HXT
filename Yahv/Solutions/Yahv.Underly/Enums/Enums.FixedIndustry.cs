using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    //[EnumNaming("行业")]
    public enum FixedIndustry
    {
        #region Industry
        /// <summary>
        /// 电力
        /// </summary>
        [Fixed("FIndustry00")]
        [Description("其他")]
        Other = 0,
        /// <summary>
        /// 工业控制
        /// </summary>
        [Fixed("FIndustry01")]
        [Description("工业控制")]
        IndustrialControl = 1,
        /// <summary>
        /// 工业控制
        /// </summary>
        [Fixed("FIndustry02")]
        [Description("自动化")]
        Automate = 2,
        /// <summary>
        /// 物联网
        /// </summary>
        [Fixed("FIndustry03")]
        [Description("物联网")]
        LOT = 3,
        /// <summary>
        /// 航空航天
        /// </summary>
        [Fixed("FIndustry04")]
        [Description("航空航天")]
        Aerospace = 4,
        /// <summary>
        /// 医疗
        /// </summary>
        [Fixed("FIndustry05")]
        [Description("医疗")]
        Medical = 5,
        /// <summary>
        /// 通讯
        /// </summary>
        [Fixed("FIndustry06")]
        [Description("通讯")]
        Communication = 6,
        /// <summary>
        /// 电力
        /// </summary>
        [Fixed("FIndustry07")]
        [Description("电力")]
        ElectricPower = 7

        #endregion
    }
}
