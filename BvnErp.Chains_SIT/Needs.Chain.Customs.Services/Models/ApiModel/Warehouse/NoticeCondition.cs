using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class NoticeCondition
    {
        /// <summary>
        /// 是否拆箱检测
        /// </summary>
        public bool DevanningCheck { get; set; }

        /// <summary>
        /// 是否称重
        /// </summary>
        public bool Weigh { get; set; }

        /// <summary>
        /// 是否点数
        /// </summary>
        public bool CheckNumber { get; set; }

        /// <summary>
        /// 是否上机检测
        /// </summary>
        public bool OnlineDetection { get; set; }

        /// <summary>
        /// 是否贴标签
        /// </summary>
        public bool AttachLabel { get; set; }

        /// <summary>
        /// 是否涂抹标签
        /// </summary>
        public bool PaintLabel { get; set; }

        /// <summary>
        /// 是否重新标签
        /// </summary>
        public bool Repacking { get; set; }

        /// <summary>
        /// 是否按高价值来进行处理
        /// </summary>
        public bool PickByValue { get; set; }
    }
}
