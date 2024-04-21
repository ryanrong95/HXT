using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.WorkFlow.Services.Models.Origins
{
    /// <summary>
    /// 审批步骤
    /// </summary>
    internal class VoteStep : IUnique
    {
        #region 属性

        /// <summary>
        /// 主键
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 审批步骤名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 所属审批流
        /// </summary>
        public string VoteFlowID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int OrderIndex { get; set; }

        /// <summary>
        /// 固定审批人
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 岗位
        /// </summary>
        public string PositionID { get; set; }

        /// <summary>
        /// 审批页面
        /// </summary>
        public string Uri { get; set; }

        #endregion
    }
}
