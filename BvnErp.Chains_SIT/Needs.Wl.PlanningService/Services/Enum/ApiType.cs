using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.PlanningService
{
    public enum ApiType
    {

        /// <summary>
        /// 获取预归类产品列表接口
        /// </summary>
        PreProduct = 1,

        /// <summary>
        /// 获取预处理二产品信息接口
        /// </summary>
        PreProduct2 = 11,

        /// <summary>
        /// 推送归类结果接口
        /// </summary>
        CResult = 2,

        /// <summary>
        /// 推送申报要素接口
        /// </summary>
        CElements = 22,

        /// <summary>
        /// 推送完税价格推送接口
        /// </summary>
        DutiablePrice = 3,

        /// <summary>
        /// 获取订单列表接口
        /// </summary>
        Order = 4,

        /// <summary>
        /// 获取订单信息接口
        /// </summary>
        Order2 = 44,

        /// <summary>
        /// ICGOOPI接口
        /// </summary>
        ICGOOPI = 5,

        /// <summary>
        /// DYJPI接口
        /// </summary>
        DYJPI = 6,

        /// <summary>
        /// 获取咨询产品
        /// </summary>
        Consult = 13,

        /// <summary>
        /// 特殊渠道，Icgoo会返回多个PI文件
        /// </summary>
        ICGOOMutliPI = 51,

    }
}
