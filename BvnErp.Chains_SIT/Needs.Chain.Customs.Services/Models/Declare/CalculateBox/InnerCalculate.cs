using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class InnerCalculate:BaseCalculate
    {
        public InnerCalculate(List<string> packNos) : base(packNos) { }

        public override int Calculate()
        {
            try
            {
                int count = 0;
                List<string> pres = new List<string>();
                foreach (var code in this.PackNos)
                {
                    string[] codes = code.Split('-');
                    if (!pres.Contains(codes[0].ToLower()+"-"+ codes[1]))
                    {
                        pres.Add(codes[0].ToLower() + "-" + codes[1]);
                    }
                }
              
                foreach (var pre in pres)
                {
                    List<string> cases = new List<string>();                   
                    var preRelated = this.PackNos.Where(t => t.ToLower().StartsWith(pre)).ToList();
                    foreach (var caseno in preRelated)
                    {
                        string[] casenos = caseno.Split('-');
                        if (casenos.Length == 4) 
                        {
                            int start = Convert.ToInt32(casenos[2]);
                            int end = Convert.ToInt32(casenos[3]);
                            for(int i = start; i <= end; i++) 
                            {
                                string newBoxCode = i.ToString().PadLeft(3, '0');
                                if (!cases.Contains(newBoxCode)) 
                                {
                                    cases.Add(newBoxCode);
                                }
                            }
                        }
                        else if (!cases.Contains(casenos[2]))
                        {
                            cases.Add(casenos[2]);
                        }                                                
                    }
                    count += cases.Count();
                }
                return count;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
