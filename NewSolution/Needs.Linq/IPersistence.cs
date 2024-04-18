using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Linq
{
    /// <summary>
    /// 单持久化
    /// </summary>
    public interface IPersist
    {
        /// <summary>
        /// 持久化
        /// </summary>
        void Enter();
    }

    /// <summary>
    /// 持久化
    /// </summary>
    public interface IPersistence : IPersist
    {
        /// <summary>
        /// 废弃（不持久化）
        /// </summary>
        void Abandon();
    }

    /// <summary>
    /// 成功接口
    /// </summary>
    public interface IEnterSuccess
    {
        /// <summary>
        /// 录入成功
        /// </summary>
        event SuccessHanlder EnterSuccess;
    }


    /// <summary>
    /// 成功接口
    /// </summary>
    public interface IFulSuccess : IEnterSuccess
    {
        /// <summary>
        /// 废弃成功
        /// </summary>
        event SuccessHanlder AbandonSuccess;
    }

    public interface IFulError
    {
        /// <summary>
        /// 出现错误
        /// </summary>
        event ErrorHanlder EnterError;

        /// <summary>
        /// 出现错误
        /// </summary>
        event ErrorHanlder AbandonError;
    }
}
