using Yahv.Usually;

namespace Yahv.Linq.Persistence
{
    /// <summary>
    /// 水单方式持久化[只能录入不能删除]
    /// </summary>
    public interface IFormings
    {
        /// <summary>
        /// 录入成功
        /// </summary>
        event SuccessHanlder EnterSuccess;

        /// <summary>
        /// 持久化
        /// </summary>
        void Enter();
    }
}
