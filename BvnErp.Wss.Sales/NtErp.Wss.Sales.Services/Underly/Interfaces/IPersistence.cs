using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Underly
{
    /// <summary>
    /// 持久化
    /// </summary>
    public interface IPersistence : IForEnter
    {
        /// <summary>
        /// 出现错误
        /// </summary>
        event ErrorHanlder Error;
        /// <summary>
        /// 录入成功
        /// </summary>
        event EnterSuccessHanlder EnterSuccess;
        /// <summary>
        /// 废弃成功
        /// </summary>
        event AbandonSuccessHanlder AbandonSuccess;

        /// <summary>
        /// 错误消息
        /// </summary>
        //event ErrorMsg ErrorMessage;

        ///// <summary>
        ///// 持久化
        ///// </summary>
        //void Enter();

        ///// <summary>
        ///// 废弃（不持久化）
        ///// </summary>
        //void Abandon();
    }


    public interface IForEnter
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
