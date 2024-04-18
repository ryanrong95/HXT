using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvWsPortal.Services.Models
{
    public class StandardClassifyMaterialVO
    {
        public long id { get; set; }
        public string model { get; set; }
        public string brand { get; set; }
        public string hsCode { get; set; }
        public string name { get; set; }
        public string unit1 { get; set; }
        public string unit2 { get; set; }
        public decimal? addedTaxRate { get; set; }
        public decimal? exciseTaxRate { get; set; }
        public decimal? tariffRate { get; set; }
        public decimal? preferentialRate { get; set; }
        public decimal? levyRate { get; set; }
        public string elements { get; set; }
        //public TariffDefaultVO defaultVO{get;set;}
        public string superRequirements { get; set; }
        public decimal? importGeneralTaxRate { get; set; }
        public string ciqc { get; set; }
        public string ciqCode { get; set; }
        public string taxCode { get; set; }
        public string taxName { get; set; }
        public Boolean ccc { get; set; }
        public Boolean embargo { get; set; }
        public Boolean hkControl { get; set; }
        public Boolean coo { get; set; }
        public Boolean ciq { get; set; }
        public Boolean highValue { get; set; }
        public decimal? ciqPrice { get; set; }
        public string specialType { get; set; }
        public string eccn { get; set; }
        public DateTime? orderDate { get; set; }
        public string memo { get; set; }
        public DateTime dateUpdate { get; set; }
    }
}
