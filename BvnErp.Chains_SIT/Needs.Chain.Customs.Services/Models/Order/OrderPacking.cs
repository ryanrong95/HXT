using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Linq;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using Spire.Pdf.Tables;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 订单的裝箱信息
    /// </summary>
    public sealed  class OrderPacking : Order
    {
        /// <summary>
        /// 是否有装箱信息
        /// </summary>
        public bool HasPacking { get; set; }

        /// <summary>
        /// 封箱状态
        /// </summary>

        public Enums.PackingStatus PackingStatus { get; set; }
    }

}