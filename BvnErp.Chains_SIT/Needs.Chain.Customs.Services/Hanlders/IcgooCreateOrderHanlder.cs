using Needs.Ccs.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    /// Icgoo接口下单时发生,生成对账单
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void IcgooGenerateOrderBillHanlder(object sender, IcgooCreateOrderEventArgs e);

    /// <summary>
    /// Icgoo接口下单时发生,报价
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void IcgooQuoteHandler(object sender, IcgooCreateOrderEventArgs e);

    /// <summary>
    /// Icgoo接口下单时发生，确认订单
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void IcgooQuoteConfirmHandler(object sender, IcgooCreateOrderEventArgs e);

    /// <summary>
    /// Icgoo接口下单时发生，装箱
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void IcgooPackingHandler(object sender, IcgooCreateOrderEventArgs e);

    /// <summary>
    /// Icgoo接口下单时发生，封箱
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void IcgooSealHandler(object sender, IcgooCreateOrderEventArgs e);

    /// <summary>
    /// Icgoo提交
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void IcgooPostForWayBillHandler(object sender, IcgooCreateOrderEventArgs e);

    /// <summary>
    /// Icgoo库房
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void IcgooPostToWareHouse(object sender, CenterIcgooCreateOrderEventArgs e);

    /// <summary>
    /// Icgoo请求后的参数
    /// </summary>
    public class IcgooCreateOrderEventArgs : EventArgs
    {
        public IcgooOrder order { get; private set; }

        public List<PartNoReceiveItem> partno { get; set; }
        public List<PvOrderItems> pvOrderItems  { get; set; }      
        public IcgooCreateOrderEventArgs(IcgooOrder order, List<PartNoReceiveItem> partno, List<PvOrderItems> pvOrderItems)
        {
            this.order = order;
            this.partno = partno;
            this.pvOrderItems = pvOrderItems;           
        }

        public IcgooCreateOrderEventArgs() { }
    }

    public class CenterIcgooCreateOrderEventArgs : EventArgs
    {
        public PvWsOrderInsApiModel PvWsOrderInsApiModel { get; set; }

        public CenterIcgooCreateOrderEventArgs(PvWsOrderInsApiModel pvWsOrderInsApiModel)
        {
            this.PvWsOrderInsApiModel = pvWsOrderInsApiModel;
        }
    }
}
