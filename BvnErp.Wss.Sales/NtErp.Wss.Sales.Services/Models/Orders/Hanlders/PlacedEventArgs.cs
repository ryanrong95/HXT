using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Model.Orders.Hanlders
{
    /// <summary>
    /// 下单成功事件句柄
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">下单事件参数</param>
    public delegate void PlacedSuccessHanlder(object sender, PlacedEventArgs e);

    /// <summary>
    /// 下单事件参数
    /// </summary>
    public class PlacedEventArgs : EventArgs
    {

    }
}
