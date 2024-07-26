using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    /// <summary>
    /// 信息推送类型
    /// </summary>
    public enum PushType
    {
        /// <summary>
        /// 归类结果推送
        /// </summary>
        [Description("归类结果")]
        ClassifyResult = 1,

        /// <summary>
        /// 完税价格推送
        /// </summary>
        [Description("完税价格")]
        DutiablePrice = 2,

        /// <summary>
        /// Icgoo预归类产品归类的关税 和 下单时候归类的关税 差异
        /// </summary>
        [Description("关税差异")]
        TariffDiff =3,

        /// <summary>
        /// 外部公司(Icgoo和外单)进价推送华芯通出入库系统
        /// </summary>
        [Description("进价")]
        PurchasePrice = 4,
    }
}
