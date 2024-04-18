using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
   public  enum SampleType
    {
        /// <summary>
        /// 原厂赠送
        /// </summary>
        [Description("原厂赠送")]
        Factory = 1,
        /// <summary>
        /// 公司赠送
        /// </summary>
        [Description("公司赠送")]
        Company = 2,
        /// <summary>
        /// 付费样品
        /// </summary>
        [Description("付费样品")]
        Pay = 3,

    }
}
