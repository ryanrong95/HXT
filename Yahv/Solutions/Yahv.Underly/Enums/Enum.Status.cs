namespace Yahv.Underly
{
    /// <summary>
    /// 通用状态
    /// </summary>
    public enum GeneralStatus 
    {
        /// <summary>
        /// 待处理
        /// </summary>
        [Attributes.Description("待处理")]
        Waiting = 100,

        /// <summary>
        /// 正常
        /// </summary>
        [Attributes.Description("正常")]
        Normal = 200,

        /// <summary>
        /// 关闭
        /// </summary>
        [Attributes.Description("关闭")]
        Closed = 400,

        /// <summary>
        /// 删除
        /// </summary>
        [Attributes.Description("删除")]
        Deleted = 500,

        /// <summary>
        /// 退回
        /// </summary>
        [Attributes.Description("退回")]
        Returned = 600,

    }


  

    /// <summary>
    /// 官网注册申请, 是否被处理
    /// </summary>
    public enum ServiceApplyHandleStatus
    {
        [Attributes.Description("待处理")]
        Pending = 0,

        [Attributes.Description("已处理")]
        Processed = 1,
    }

}
