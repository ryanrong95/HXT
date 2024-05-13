using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.vTaskers.Services
{
    /// <summary>
    /// 任务设置
    /// </summary>
    public class TaskSettgins
    {

        /// <summary>
        /// 私有构造器
        /// </summary>
        TaskSettgins()
        {
            throw new NotSupportedException("不能这样调用");
        }

        /// <summary>
        /// 中心库任务
        /// </summary>
        public class PvCenter
        {
            public const string 香港报关出库 = nameof(香港报关出库);

            public const string 深圳报关内单出库 = nameof(深圳报关内单出库);

            public const string 单一窗口申报后深圳出库 = nameof(单一窗口申报后深圳出库);

            public const string 芯达通费用同步 = nameof(芯达通费用同步);

            /// <summary>
            /// 私有构造器
            /// </summary>
            PvCenter()
            {
                throw new NotSupportedException("不能这样调用");
            }

        }
    }
}
