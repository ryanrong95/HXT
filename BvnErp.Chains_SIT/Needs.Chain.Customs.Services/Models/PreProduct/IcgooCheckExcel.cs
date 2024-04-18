using Needs.Ccs.Services.Enums;
using Needs.Linq;
using Needs.Utils.Npoi;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class IcgooCheckExcel 
    {      
        public string ContactNo { get; set; }
     
        public string customcode { get; set; }

        /// <summary>
        /// 交货单号，没有，为保持与icgoo的格式一致
        /// </summary>
        public string GoodsNO { get; set; }
        public string Model { get; set; }
        public string ProductName { get; set; }
        public string Brand { get; set; }
        public string Origin { get; set; }
        public decimal? Qty { get; set; }

        /// <summary>
        /// 外币单价
        /// </summary>
        public decimal? UnitPrice { get; set; }

        /// <summary>
        /// 总货值
        /// </summary>
        public decimal? TotalPrice { get; set; }
        public decimal? CustomExchangeRate { get; set; }
        public decimal? RealExchangeRate { get; set; }
        public decimal? TotalRMBPrice { get; set; }
        public decimal? Tariff { get; set; }
        public decimal? TairffRate { get; set; }
        public decimal? ExciseTax { get; set; }
        public decimal? ExciseTaxRate { get; set; }
        public decimal? ValueAdded { get; set; }
        public decimal? ValueAddedRate { get; set; }

        /// <summary>
        /// 海关编码 归类时候的海关编码
        /// </summary>
        public string HS_Code { get; set; }
        public DateTime? CreateTime { get; set; }
        public string CompanyName { get; set; }

    }
}
