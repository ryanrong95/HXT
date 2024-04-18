using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class VoyagePackNo
    {
        public List<string> DecHeads { get; set; }

        public VoyagePackNo(List<string> decHeads)
        {
            this.DecHeads = decHeads;
        }

        public int Calculate()
        {
            string[] innerCompanies = DyjInnerCompanies.Current.Companies.Split(',');
            List<string> innerBoxes = new List<string>();
            List<string> outBoxes = new List<string>();

            var declists = new DecOriginListsView().Where(t=>this.DecHeads.Contains(t.DeclarationID)).ToList();
            foreach(var item in declists)
            {
                string clientcode = item.OrderID.Substring(0, 5);               
                if (innerCompanies.Contains(clientcode))
                {
                    if (!innerBoxes.Contains(item.CaseNo))
                    {
                        innerBoxes.Add(item.CaseNo);
                    }                    
                }
                else
                {
                    if (!outBoxes.Contains(item.CaseNo))
                    {
                        outBoxes.Add(item.CaseNo);
                    }                        
                }
            }

            int innerCount = new CalculateContext(CompanyTypeEnums.Inside, innerBoxes).CalculatePacks();
            int outCount = new CalculateContext(CompanyTypeEnums.Icgoo, outBoxes).CalculatePacks();

            return innerCount+ outCount;
        }
    }
}
