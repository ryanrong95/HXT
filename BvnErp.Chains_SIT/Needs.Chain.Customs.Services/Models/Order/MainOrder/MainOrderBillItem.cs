using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
   
    public class MainOrderBillItem 
    {
        public string OrderID { get; set; }
        public string ContrNo { get; set; }
        public bool IsSuccess { get; set; }
        public DateTime? DDate { get; set; }
        public decimal CustomsExchangeRate { get; set; }
        public decimal RealExchangeRate { get; set; }
        public Enums.OrderType OrderType { get; set; }
        public decimal AgencyFee { get; set; }
        public List<MainOrderBillItemProduct> Products { get; set; }

        public List<MainOrderBillItemProduct> PartProducts { get; set; }

        //合计 客户端页面展示用
        public decimal totalQty { get; set; }
        public decimal totalPrice { get; set; }
        public decimal totalCNYPrice { get; set; }
        public decimal? totalTraiff { get; set; }
        public decimal? totalExciseTax { get; set; }
        public decimal? totalAddedValueTax { get; set; }
        public decimal totalAgencyFee { get; set; }
        public decimal totalIncidentalFee { get; set; }
    }
}
