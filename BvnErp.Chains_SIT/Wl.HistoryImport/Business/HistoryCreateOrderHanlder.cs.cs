using Needs.Ccs.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wl.HistoryImport
{
    public class HistoryCreateOrderHanlder
    {
        /// <summary>
        /// Icgoo接口下单时发生,生成对账单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void HistoryGenerateOrderBillHanlder(object sender, HistoryCreateOrderEventArgs e);
        public class HistoryCreateOrderEventArgs : EventArgs
        {
            public IcgooOrder order { get; private set; }
            public List<InsideOrderItem> partno { get; set; }

            public HistoryUseOnly historyUseOnly{ get; set; }
            public List<PackHistoryOnly> Packs { get; set; }

            public HistoryCreateOrderEventArgs(IcgooOrder order, List<InsideOrderItem> partno, HistoryUseOnly HistoryUseOnly, List<PackHistoryOnly> packs)
            {
                this.order = order;
                this.partno = partno;
                this.historyUseOnly = HistoryUseOnly;
                this.Packs = packs;
            }

            public HistoryCreateOrderEventArgs() { }
        }
    }
}
