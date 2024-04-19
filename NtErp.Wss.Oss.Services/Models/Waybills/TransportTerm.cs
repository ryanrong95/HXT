using Needs.Linq;
using Needs.Utils.Converters;
using NtErp.Wss.Oss.Services.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Oss.Services.Models
{
    /// <summary>
    /// 运输条款
    /// </summary>
    public class TransportTerm : IUnique, IPersist
    {
        public TransportTerm()
        {

        }

        #region 属性
        public string ID { get; set; }
        /// <summary>
        /// 运输方式
        /// </summary>
        public TransportMode TransportMode { get; set; }
        /// <summary>
        /// 运费支付方式
        /// </summary>
        public FreightMode FreightMode { get; set; }
        /// <summary>
        /// 价格条款
        /// </summary>
        public PriceClause PriceClause { get; set; }
        /// <summary>
        /// 承运商
        /// </summary>
        public string Carrier { get; set; }

        /// <summary>
        /// 自提地址
        /// </summary>
        public string Address { get; set; }


        #endregion

        #region 持久化

        public void Enter()
        {
            using (var reponsitory = new Layer.Data.Sqls.CvOssReponsitory())
            {
                if (!reponsitory.ReadTable<Layer.Data.Sqls.CvOss.TransportTerms>().Any(item => item.ID == this.ID))
                {
                    reponsitory.Insert(this.ToLinq());
                }
            }
        }
        #endregion
    }

    public enum ShipmentType
    {
        /// <summary>
        /// 自提
        /// </summary>
        SelfPickUp = 1,
        /// <summary>
        /// UPS
        /// </summary>
        UPS = 2,
        /// <summary>
        /// FedEx
        /// </summary>
        FedEx = 3,
        /// <summary>
        /// DHL
        /// </summary>
        DHL = 4,
        /// <summary>
        /// 顺丰
        /// </summary>
        SF = 5,
        /// <summary>
        /// 其他
        /// </summary>
        Other = 6,
    }
}
