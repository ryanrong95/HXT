using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Oss.Services.Interfaces
{
    /// <summary>
    /// 订单
    /// </summary>
    public interface IOrderAction
    {

        //event Handlers.CreateSuccessHanlder CreateSuccess;
        //event Handlers.CreateErrorHanlder CreateError;

        //event Handlers.CompleteSuccessHanlder CompleteSuccess;
        //event Handlers.CompleteErrorHanlder CompleteError;

        //event Handlers.CloseSuccessHanlder CloseSuccess;
        //event Handlers.CloseErrorHanlder CloseError;

        //event Handlers.PaySuccessHanlder PaySuccess;
        //event Handlers.PayErrorHanlder PayError;

        /// <summary>
        /// 创建订单
        /// </summary>
        void Create();
        /// <summary>
        /// 完成订单
        /// </summary>
        void Complete();
        /// <summary>
        /// 关闭订单
        /// </summary>
        void Close();
        /// <summary>
        /// 支付订单
        /// </summary>
        void Pay(decimal amount);
    }
}
