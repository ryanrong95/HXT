using Spire.Pdf;
using Spire.Pdf.Graphics;
using Spire.Pdf.Grid;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 报关单的运单
    /// 销售的运单
    /// 香港出口运单
    /// </summary>
    [Serializable]
    public class OutputWayBill : ManifestConsignment
    {
        /// <summary>
        ///舱单 航次号
        /// </summary>
        public Voyage Voyage { get; set; }

        /// <summary>
        /// 报关单
        /// </summary>
        public DecHead DecHead { get; set; }

        /// <summary>
        /// 运输条款
        /// </summary>
        public Enums.TransportTermType TransportTermType
        {
            get
            {
                if (base.ConditionCode.HasValue)
                {
                    return (Enums.TransportTermType)base.ConditionCode.Value;
                }

                return Enums.TransportTermType.DoorToDoor;
            }
        }

        /// <summary>
        /// 受益人
        /// 报关公司
        /// 代理报关关系中的买方
        /// 收货人
        /// </summary>
        public Beneficiary Beneficiary { get; set; }

        public OutputWayBill()
        {

        }
    }
}