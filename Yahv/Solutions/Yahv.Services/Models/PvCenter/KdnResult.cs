using System;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;

namespace Yahv.Services.Models
{
    /// <summary>
    /// 快递鸟
    /// </summary>
    public class KdnResult : IUnique
    {
        public string ID { get; set; }
        public string OrderCode { get; set; }
        public string LogisticCode { get; set; }
        public DateTime CreateDate { get; set; }

        public string KDNOrderCode { get; set; }
        public string DestinatioCode { get; set; }
        public string OriginCode { get; set; }
        public string ShipperCode { get; set; }
        public string Html { get; set; }
    }
}
