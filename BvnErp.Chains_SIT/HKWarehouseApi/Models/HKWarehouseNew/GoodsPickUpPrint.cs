using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HKWarehouseApi.Models
{
    /// <summary>
    /// 货物提货委托书
    /// </summary>
    public class GoodsPickUpPrint : PrintFile
    {
        public GoodsPickUpPrint(string voyageNo, int totalPackNo, decimal totalWeight)
        {
            this.VoyageNo = voyageNo;
            this.TotalPackNo = totalPackNo;
            this.TotalWeight = totalWeight;
            this.TemplateFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content\\templates\\", "提货委托书模板.xlsx");
        }
    }
}