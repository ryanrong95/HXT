using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PsWms.PdaApi.Services.Enums;

namespace Yahv.PsWms.PdaApi.Services.Models
{
    public class NoticeTransport : IUnique
    {
        #region 属性
        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 货运类型
        /// </summary>
        public TransportMode TransportMode { get; set; }

        /// <summary>
        /// 承运商
        /// </summary>
        public string Carrier { get; set; }

        /// <summary>
        /// 运单号
        /// </summary>
        public string WaybillCode { get; set; }

        /// <summary>
        /// 运费负担方,枚举
        /// </summary>
        public FreightPayer ExpressPayer { get; set; }

        /// <summary>
        /// 快递货运类型
        /// </summary>
        public string ExpressTransport { get; set; }

        /// <summary>
        /// 运费账户, 只在 FreightPayer.ThirdParty时候起作用
        /// </summary>
        public string ExpressEscrow { get; set; }

        /// <summary>
        /// 快递运费
        /// </summary>
        public decimal ExpressFreight { get; set; }

        /// <summary>
        /// 提送货时间
        /// </summary>
        public DateTime? TakingTime { get; set; }

        /// <summary>
        /// 提送货人名称
        /// </summary>
        public string TakerName { get; set; }

        /// <summary>
        /// 提送货人车牌
        /// </summary>
        public string TakerLicense { get; set; }

        /// <summary>
        /// 提送货人联系电话
        /// </summary>
        public string TakerPhone { get; set; }

        /// <summary>
        /// 证件类型
        /// </summary>
        public IDType TakerIDType { get; set; }

        /// <summary>
        /// 证件号码
        /// </summary>
        public string TakerIDCode { get; set; }

        /// <summary>
        /// 提送货地址 格式:”省\s市\s县\sAddress”
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 提送货联系人
        /// </summary>
        public string Contact { get; set; }

        /// <summary>
        /// 提送货联系人电话
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 提送货联系人邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyDate { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { get; set; }
        #endregion
    }
}
