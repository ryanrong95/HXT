using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YaHv.PvData.Services.Handlers
{
    /// <summary>
    /// 申报要素品牌变更
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">事件参数</param>
    public delegate void ElementsMfrChangedHandler(object sender, ElementsMfrChangedEventArgs e);

    /// <summary>
    /// 状态改变事件参数
    /// </summary>
    public class ElementsMfrChangedEventArgs : EventArgs
    {
        /// <summary>
        /// 申报要素品牌
        /// </summary>
        public Models.ElementsManufacturer EM { get; private set; }

        /// <summary>
        /// 变更前的中文或外文品牌
        /// </summary>
        public string From { get; private set; }

        /// <summary>
        /// 变更后的中文或外文品牌
        /// </summary>
        public string To { get; private set; }

        public ElementsMfrChangedEventArgs(Models.ElementsManufacturer em, string from, string to)
        {
            this.EM = em;
            this.From = from;
            this.To = to;
        }

        public ElementsMfrChangedEventArgs() { }
    }
}
