using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Underly
{
    /// <summary>
    /// 成功录入事件句柄
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">成功事件参数</param>
    public delegate void EnterSuccessHanlder(object sender, EnterSuccessEventArgs e);

    /// <summary>
    /// 成功录入事件参数
    /// </summary>
    public class EnterSuccessEventArgs : EventArgs
    {
        public string ID { get; set; }

        public EnterSuccessEventArgs(string ID)
        {
            this.ID = ID;
        }
        public EnterSuccessEventArgs()
        {
            this.ID = ID;
        }
    }
}
