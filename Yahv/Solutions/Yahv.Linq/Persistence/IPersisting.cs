namespace Yahv.Linq.Persistence
{
    /// <summary>
    /// 持久化
    /// </summary>
    public interface IPersisting
    {
        /// <summary>
        /// 持久化
        /// </summary>
        void Enter();

        /// <summary>
        /// 废弃（不持久化）
        /// </summary>
        void Abandon();
    }
}
