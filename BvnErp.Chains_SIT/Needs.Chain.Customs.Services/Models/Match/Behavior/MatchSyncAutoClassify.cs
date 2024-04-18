using Needs.Ccs.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class MatchSyncAutoClassify
    {
        public List<ClassifyProduct> ClassifyProducts { get; private set; }

        public MatchSyncAutoClassify(List<ClassifyProduct> classifyProducts)
        {
            this.ClassifyProducts = classifyProducts;
        }

        public void SyncResult()
        {
            var cps = ClassifyProducts.Where(cp => cp.ClassifyStatus == ClassifyStatus.Done).ToArray();
            if (cps.Count() > 0)
            {
                SyncManager.Current.Classify.For(cps).DoSync();
            }            
        }
    }
}
