using Needs.Ccs.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class SCSyncAutoClassifyResult : SCHandler
    {
        public List<ClassifyProduct> ClassifyProducts { get; private set; }

        public SCSyncAutoClassifyResult(List<ClassifyProduct> classifyProducts)
        {
            this.ClassifyProducts = classifyProducts;
        }
        public override void handleRequest()
        {
            try
            {
                var cps = ClassifyProducts.Where(cp => cp.ClassifyStatus == ClassifyStatus.Done).ToArray();
                if (cps.Count() > 0)
                {
                    SyncManager.Current.Classify.For(cps).DoSync();
                }

                if (next != null)
                {
                    next.handleRequest();
                }
            }
            catch(Exception ex)
            {

            }           
        }
    }
}
