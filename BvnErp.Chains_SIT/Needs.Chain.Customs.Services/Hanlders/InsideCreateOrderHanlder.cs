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
    public delegate void InsideGenerateOrderBillHanlder(object sender, InsideCreateOrderEventArgs e);

    /// <summary>
    /// Icgoo接口下单时发生,报价
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void InsideQuoteHandler(object sender, InsideCreateOrderEventArgs e);

    /// <summary>
    /// Icgoo接口下单时发生，确认订单
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void InsideQuoteConfirmHandler(object sender, InsideCreateOrderEventArgs e);

    /// <summary>
    /// Icgoo接口下单时发生，装箱
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void InsidePackingHandler(object sender, InsideCreateOrderEventArgs e);

    /// <summary>
    /// Icgoo接口下单时发生，封箱
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void InsideSealHandler(object sender, InsideCreateOrderEventArgs e);
    /// <summary>
    /// 内单绑定产品和付汇供应商的关系
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>

    public delegate void InsideProductSupplierHandler(object sender, InsideCreateOrderEventArgs e);

    public delegate void InsidePostForWayBillHandler(object sender, InsideCreateOrderEventArgs e);

    public delegate void InsidePostToWareHouse(object sender, CenterInsideCreateOrderEventArgs e);

    /// <summary>
    /// Icgoo请求后的参数
    /// </summary>
    public class InsideCreateOrderEventArgs : EventArgs
    {
        public IcgooOrder order { get; private set; }

        public List<InsideOrderItem> partno { get; set; }

        public List<PvOrderItems> pvOrderItems { get; set; }      

        public InsideCreateOrderEventArgs(IcgooOrder order, List<InsideOrderItem> partno, List<PvOrderItems> pvOrderItems)
        {
            this.order = order;
            this.partno = partno;
            this.pvOrderItems = pvOrderItems;          
        }

        public InsideCreateOrderEventArgs() { }
    }

    public class CenterInsideCreateOrderEventArgs : EventArgs
    {
        public PvWsOrderInsApiModel PvWsOrderInsApiModel { get; set; }

        public CenterInsideCreateOrderEventArgs(PvWsOrderInsApiModel pvWsOrderInsApiModel)
        {
            this.PvWsOrderInsApiModel = pvWsOrderInsApiModel;
        }
    }
}
