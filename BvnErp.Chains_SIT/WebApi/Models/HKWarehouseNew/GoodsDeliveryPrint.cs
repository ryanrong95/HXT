using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    /// <summary>
    /// 货物流转确认单
    /// </summary>
    public class GoodsDeliveryPrint : PrintFile
    {
        public GoodsDeliveryPrint(string voyageNo, int totalPackNo, decimal totalWeight)
        {
            this.VoyageNo = voyageNo;
            this.TotalPackNo = totalPackNo;
            this.TotalWeight = totalWeight;
            this.TemplateFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content\\templates\\", "货物流转书模板.xlsx");
        }
    }
}