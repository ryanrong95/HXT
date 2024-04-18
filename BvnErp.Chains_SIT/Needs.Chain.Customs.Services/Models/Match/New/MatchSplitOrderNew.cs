using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class MatchSplitOrderNew
    {
        /// <summary>
        /// 页面勾选信息
        /// </summary>
        public List<MatchViewModel> SelectedItems { get; set; }
        /// <summary>
        /// 拆分之前的订单
        /// </summary>
        public Order OriginOrder { get; set; }
        /// <summary>
        /// 新拆出来的订单
        /// </summary>
        public Order CurrentOrder { get; set; }
        public MatchSplitOrderNew(List<MatchViewModel> selectedItems, Order originOrder, Order currentOrder)
        {
            SelectedItems = selectedItems;
            OriginOrder = originOrder;
            CurrentOrder = currentOrder;
        }
        
        public void Handle()
        {
            try
            {
                SCPersistentHandler SCPersistentHandler = new SCPersistentHandler(SelectedItems, OriginOrder, CurrentOrder);
                SCPersistentHandler.handleRequest();
                var items = SCPersistentHandler.GetMatchViewModels();

                SCAutoClassifyHandler SCAutoClassifyHandler = new SCAutoClassifyHandler(items, CurrentOrder);
                SCAutoClassifyHandler.handleRequest();

                SCHandler SCOrderChangeHandler = new SCOrderChangeHandler(items, CurrentOrder);
                SCPostOrderInfo SCPostOrderInfo = new SCPostOrderInfo(CurrentOrder);
                SCSyncClassifyResult SCSyncClassifyResult = new SCSyncClassifyResult(items);
                SCSyncAutoClassifyResult SCSyncAutoClassifyResult = new SCSyncAutoClassifyResult(SCAutoClassifyHandler.ClassifyProducts);
                SC2ClientConfirm SC2ClientConfirm = new SC2ClientConfirm(SelectedItems, CurrentOrder);
                SCAdjustOrderVoyages SCAdjustOrderVoyages = new SCAdjustOrderVoyages(OriginOrder, CurrentOrder);
                SCAdjustOrderFee SCAdjustOrderFee = new SCAdjustOrderFee(CurrentOrder);
                SCAdjustOrderChangeNotices SCAdjustOrderChangeNotices = new SCAdjustOrderChangeNotices(SelectedItems, CurrentOrder);
                SCReOrderBillHandler SCReOrderBill = new SCReOrderBillHandler(items, CurrentOrder);


                SCOrderChangeHandler.next = SCPostOrderInfo;
                SCPostOrderInfo.next = SCSyncClassifyResult;
                SCSyncClassifyResult.next = SCSyncAutoClassifyResult;
                SCSyncAutoClassifyResult.next = SC2ClientConfirm;
                SC2ClientConfirm.next = SCAdjustOrderVoyages;
                SCAdjustOrderVoyages.next = SCAdjustOrderFee;
                SCAdjustOrderFee.next = SCAdjustOrderChangeNotices;
                SCAdjustOrderChangeNotices.next = SCReOrderBill;

                SCOrderChangeHandler.handleRequest();

            }
            catch (Exception ex)
            {
                ex.CcsLog("订单拆分:CurrentOrderID:" + CurrentOrder.ID + " OriginOrder" + OriginOrder.ID);
            }
        }
    }
}
