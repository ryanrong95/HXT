using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Model.Orders.Hanlders
{
    /// <summary>
    /// 订单关闭成功事件句柄
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">订单关闭事件参数</param>
    public delegate void CloseSuccessHanlder(object sender, CloseEventArgs e);

    /// <summary>
    /// 订单关闭事件参数
    /// </summary>
    public class CloseEventArgs : EventArgs
    {

    }
}
