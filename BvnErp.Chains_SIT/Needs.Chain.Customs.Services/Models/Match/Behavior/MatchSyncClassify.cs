using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class MatchSyncClassify
    {
        public List<Models.OrderItemAssitant> OrderItems { get; private set; }

        public MatchSyncClassify(List<Models.OrderItemAssitant> orderItems)
        {
            this.OrderItems = orderItems;
        }

        public void SyncResult()
        {
            var itemIDs = this.OrderItems.Select(item => item.ID).Distinct().ToArray();
            var cps = new Views.Alls.ClassifyProductsAll().GetTop(itemIDs.Length, item => itemIDs.Contains(item.ID), null, null, null);
            foreach (var cp in cps)
            {
                cp.ImportTax.ImportPreferentialTaxRate = cp.ImportTax.Rate;
                cp.ImportTax.OriginRate = 0m;
            }
            SyncManager.Current.Classify.For(cps).DoSync();
        }
    }
}
