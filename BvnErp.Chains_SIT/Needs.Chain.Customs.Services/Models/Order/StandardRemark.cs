using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class StandardRemark
    {
        public SelectedStd SelectedStd { get; set; }

        public ChargeInputs[] ChargeInputs { get; set; }
    }

    public class SelectedStd
    {
        public string ID { get; set; }

        public string FatherID { get; set; }

        public string Type { get; set; }

        public string IsMenuLeaf { get; set; }

        public string Name { get; set; }

        public string Unit1 { get; set; }

        public string Unit2 { get; set; }

        public string Price { get; set; }

        public string Remark1 { get; set; }

        public string Remark2 { get; set; }

        public SelectedStd[] Children { get; set; }
    }

    public class ChargeInputs
    {
        public string StdID { get; set; }

        public decimal Price { get; set; }

        public string Currency { get; set; }

        public string CurrencyCN { get; set; }

        public ChargeInputValue[] Values { get; set; }
    }

    public class ChargeInputValue
    {
        public string Unit { get; set; }

        public decimal Value { get; set; }
    }
}
