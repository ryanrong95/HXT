using System;

namespace Needs.Wl.Models.Hanlders
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
        public Models.ExchangeRate ExchangeRate { get; private set; }

        public ExchangeRateChangingEventArgs()
        {

        }

        public ExchangeRateChangingEventArgs(Models.ExchangeRate exchangeRate)
        {
            this.ExchangeRate = exchangeRate;
        }
    }
}
