using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 确认订单，只有无通知产品录入合并到现有订单中这一种情况
    /// </summary>
    public class MatchConfirmOrderNew
    {
        /// <summary>
        /// 页面勾选信息
        /// </summary>
        public List<MatchViewModel> SelectedItems { get; set; }
       
        public Order CurrentOrder { get; set; }
        public MatchConfirmOrderNew(List<MatchViewModel> selectedItems, Order currentOrder)
        {
            SelectedItems = selectedItems;           
            CurrentOrder = currentOrder;
        }

        public void Handle()
        {
            try
            {
                SCPersistentHandlerCon SCPersistentHandler = new SCPersistentHandlerCon(SelectedItems, CurrentOrder);
                SCPersistentHandler.handleRequest();
                var items = SCPersistentHandler.GetMatchViewModels();

                SCAutoClassifyHandler SCAutoClassifyHandler = new SCAutoClassifyHandler(SelectedItems, CurrentOrder);
                SCAutoClassifyHandler.handleRequest();

                SCHandler SCOrderChangeHandler = new SCOrderChangeHandler(items, CurrentOrder);                
                SCPostOrderInfo SCPostOrderInfo = new SCPostOrderInfo(CurrentOrder);
                SCSyncClassifyResult SCSyncClassifyResult = new SCSyncClassifyResult(items);
                SCSyncAutoClassifyResult SCSyncAutoClassifyResult = new SCSyncAutoClassifyResult(SCAutoClassifyHandler.ClassifyProducts);
                SC2ClientConfirm SC2ClientConfirm = new SC2ClientConfirm(SelectedItems, CurrentOrder);
                SCAdjustOrderVoyages SCAdjustOrderVoyages = new SCAdjustOrderVoyages(null, CurrentOrder);               
                
                SCOrderChangeHandler.next = SCPostOrderInfo;         
                SCPostOrderInfo.next = SCSyncClassifyResult;
                SCSyncClassifyResult.next = SCSyncAutoClassifyResult;
                SCSyncAutoClassifyResult.next = SC2ClientConfirm;
                SC2ClientConfirm.next = SCAdjustOrderVoyages;

                SCOrderChangeHandler.handleRequest();
            }
            catch (Exception ex)
            {
                ex.CcsLog("订单确认:CurrentOrderID:" + CurrentOrder.ID );
            }
        }
    }
}
