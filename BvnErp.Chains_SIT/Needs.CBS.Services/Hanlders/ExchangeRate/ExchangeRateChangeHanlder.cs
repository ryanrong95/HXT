using Needs.Cbs.Services.Models.Origins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Cbs.Services.Hanlders
{
    /// <summary>
    /// 汇率操作前
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">汇率操作前汇率各个参数</param>
    public delegate void ExchangeRateChangingHanlder(object sender, ExchangeRateChangingEventArgs e);

    /// <summary>
    /// 汇率操作前事件参数
    /// </summary>
    public class ExchangeRateChangingEventArgs : EventArgs
    {
        public ExchangeRate ExchangeRate { get; private set; }

        public ExchangeRateChangingEventArgs()
        {

        }

        public ExchangeRateChangingEventArgs(ExchangeRate exchangeRate)
        {
            this.ExchangeRate = exchangeRate;
        }
    }
}
