using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
  
        /// <summary>
        /// 产品香港出库
        /// </summary>
        /// <param name="sender">发出者</param>
        /// <param name="e">状态改变事件参数</param>
        public delegate void WarehouseExitedEventHanlder(object sender, WarehouseExitedEventArgs e);

        /// <summary>
        /// 事件参数
        /// </summary>
        public class WarehouseExitedEventArgs : EventArgs
        {
            public Models.ExitNotice ExitNotice { get; private set; }

            public WarehouseExitedEventArgs(Models.ExitNotice item)
            {
                this.ExitNotice = item;
            }

            public WarehouseExitedEventArgs() { }
        }
    
}
