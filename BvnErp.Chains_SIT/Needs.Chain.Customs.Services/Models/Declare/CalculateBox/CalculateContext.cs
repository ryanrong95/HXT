using Needs.Ccs.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class CalculateContext
    {
        private BaseCalculate calculate { get; set; }

        public CalculateContext(CompanyTypeEnums companyType, List<string> packNos)
        {
            switch (companyType)
            {
                case CompanyTypeEnums.Icgoo:
                case CompanyTypeEnums.OutSide:
                case CompanyTypeEnums.FastBuy:
                    this.calculate = new OuterCalculate(packNos);
                    break;

                case CompanyTypeEnums.Inside:
                    this.calculate = new InnerCalculate(packNos);
                    break;
            }
                

        }

        public int CalculatePacks()
        {
            return this.calculate.Calculate();
        }
    }
}
