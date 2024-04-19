using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Underly
{
    /// <summary>
    /// 成功废除事件句柄
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">成功事件参数</param>
    public delegate void AbandonSuccessHanlder(object sender, AbandonSuccessEventArgs e);

    /// <summary>
    /// 成功废除事件参数
    /// </summary>
    public class AbandonSuccessEventArgs : EventArgs
    {
        public string ID { get; set; }

        public AbandonSuccessEventArgs(string ID = null)
        {
            this.ID = ID;
        }
    }
}
