using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 同步归类结果
    /// </summary>
    public class SCSyncClassifyResult : SCHandler
    {
        public SCSyncClassifyResult(List<MatchViewModel> selectedItems)
        {
            SelectedItems = selectedItems;
        }
        public override void handleRequest()
        {
            try
            {
                var itemIDs = SelectedItems.Where(t => !string.IsNullOrEmpty(t.NewOrderItemID)).Select(t => t.NewOrderItemID).ToArray();
                var cps = new Views.Alls.ClassifyProductsAll().GetTop(itemIDs.Length, item => itemIDs.Contains(item.ID), null, null, null);
                foreach (var cp in cps)
                {
                    cp.ImportTax.ImportPreferentialTaxRate = cp.ImportTax.Rate;
                    cp.ImportTax.OriginRate = 0m;
                }
                SyncManager.Current.Classify.For(cps).DoSync();

                if (next != null)
                {
                    next.handleRequest();
                }
            }
            catch(Exception ex)
            {
                var itemIDs = SelectedItems.Where(t => !string.IsNullOrEmpty(t.NewOrderItemID)).Select(t => t.NewOrderItemID).ToArray();
                string firstID = itemIDs.Count()>0?itemIDs[0]:" ";
                ex.CcsLog("同步归类结果出错,CurrentOrder:" + firstID);

                if (next != null)
                {
                    next.handleRequest();
                }
            }
            
        }
    }
}
