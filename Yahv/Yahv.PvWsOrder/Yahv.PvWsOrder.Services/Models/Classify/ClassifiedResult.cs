using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.PvWsOrder.Services.Models
{
    /// <summary>
    /// 手动归类结果
    /// </summary>

    public class ClassifiedResult
    {
        public string ItemID { get; set; }

        public string MainID { get; set; }

        public string ProductID { get; set; }

        public string HSCodeID { get; set; }

        public string CreatorID { get; set; }

        public string Step { get; set; }

        public decimal OriginRate { get; set; }

        public decimal FVARate { get; set; }

        public bool Ccc { get; set; }

        public bool Embargo { get; set; }

        public bool HkControl { get; set; }

        public bool Coo { get; set; }

        public bool CIQ { get; set; }

        public decimal CIQprice { get; set; }

        public bool IsHighPrice { get; set; }

        public bool IsDisinfected { get; set; }

        public bool IsSysCcc { get; set; }

        public bool IsSysEmbargo { get; set; }
    }
}
