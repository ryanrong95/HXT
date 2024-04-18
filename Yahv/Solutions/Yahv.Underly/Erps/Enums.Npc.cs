using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// Npc
    /// </summary>
    public enum Npc
    {
        /// <summary>
        /// 库存数据报价
        /// </summary>
        [Description("库存数据报价机器人")]
        Inventory = 1,
        /// <summary>
        /// 历史数据报价
        /// </summary>
        [Description("历史数据报价机器人")]
        Historical = 2,
        /// <summary>
        /// 系统机器人
        /// </summary>
        [Description("系统机器人")]
        Robot = 3,
        /// <summary>
        /// 自动指派
        /// </summary>
        [Description("系统指派")]
        AURA = 5,
        /// <summary>
        /// 自动预判
        /// </summary>
        [Description("系统预判")]
        Prejudger = 6,
        /// <summary>
        /// 自动审批
        /// </summary>
        [Description("系统审批")]
        Approver = 7
        //WanghuiRot
    }
}
