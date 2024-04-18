using Yahv.Usually;

namespace Yahv.Linq.Persistence
{
    /// <summary>
    /// 持久化
    /// </summary>
    public interface IPersistence : IFormings, IPersisting
    {
        /// <summary>
        /// 废弃成功
        /// </summary>
        event SuccessHanlder AbandonSuccess;
    }
}
