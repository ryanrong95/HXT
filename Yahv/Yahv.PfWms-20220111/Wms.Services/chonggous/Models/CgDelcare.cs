using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Wms.Services.chonggous.Models
{
    /// <summary>
    /// 库房申请报关后，将数据通到芯达通
    /// </summary>
    public class CgDeclareApply
    {
        /// <summary>
        /// 报关申请内容
        /// </summary>
        public List<CgDeclareApplyItem> Items { get; set; }
    }
    public class CgDeclareApplyItem
    {
        /// <summary>
        /// 小订单ID
        /// </summary>
        public string TinyOrderID { get; set; }

        /// <summary>
        /// 操作人ID
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 分拣/拣货ID
        /// </summary>
        public string DeclareID { get; set; }
    }

    /// <summary>
    /// 芯达通申报成功后
    /// 将数据通到库房
    /// </summary>
    public class CgDelcare
    {
        public CgWaybill HkExitWaybill { get; set; }

        public List<CgNotice> Notices { get; set; }
    }

    /// <summary>
    /// 芯达通截单后 将数据通到库房
    /// </summary>
    public class CgDelcareCutting
    {
        /// <summary>
        /// ID（运输批次号）
        /// </summary>
        public string ID { get; set; } = string.Empty;

        /// <summary>
        /// 【车辆】香港车牌号
        /// </summary>
        public string HKLicense { get; set; } = string.Empty;

        /// <summary>
        /// 【司机】司机姓名
        /// </summary>
        public string DriverName { get; set; } = string.Empty;

        /// <summary>
        /// 【司机】司机证件编码 Drivers.Licence
        /// </summary>
        public string DriverCode { get; set; } = string.Empty;

        /// <summary>
        /// 【承运商】承运商简称
        /// </summary>
        public string CarrierCode { get; set; } = string.Empty;

        /// <summary>
        /// 【Voyage】运输时间
        /// </summary>
        public DateTime? TransportTime { get; set; }

        /// <summary>
        /// 【Voyage】运输类型
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 【Voyage】截单状态
        /// </summary>
        public int CutStatus { get; set; }

        /// <summary>
        /// 【Voyage】香港清关状态
        /// </summary>
        public bool HKDeclareStatus { get; set; }

        /// <summary>
        /// 【Voyage】Status
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 【Voyage】CreateTime
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 【Voyage】UpdateDate
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 【Voyage】Summary
        /// </summary>
        public string Summary { get; set; } = string.Empty;

        /// <summary>
        /// 【承运商】承运商类型
        /// </summary>
        public int? CarrierType { get; set; }

        /// <summary>
        /// 【承运商】名称
        /// </summary>
        public string CarrierName { get; set; } = string.Empty;

        /// <summary>
        /// 【承运商】查询标记
        /// </summary>
        public string CarrierQueryMark { get; set; } = string.Empty;

        /// <summary>
        /// 【承运商】联系电话
        /// </summary>
        public string ContactMobile { get; set; } = string.Empty;

        /// <summary>
        /// 【承运商】承运商地址
        /// </summary>
        public string CarrierAddress { get; set; } = string.Empty;

        /// <summary>
        /// 【承运商】联系人
        /// </summary>
        public string ContactName { get; set; } = string.Empty;

        /// <summary>
        /// 【承运商】传真
        /// </summary>
        public string ContactFax { get; set; } = string.Empty;

        /// <summary>
        /// 【车辆】车辆类型
        /// </summary>
        public int? VehicleType { get; set; }

        /// <summary>
        /// 【车辆】车牌号
        /// </summary>
        public string VehicleLicence { get; set; } = string.Empty;

        /// <summary>
        /// 【车辆】车重
        /// </summary>
        public string VehicleWeight { get; set; }

        /// <summary>
        /// 【司机】大陆手机号
        /// </summary>
        public string DriverMobile { get; set; } = string.Empty;

        /// <summary>
        /// 【司机】司机海关编号
        /// </summary>
        public string DriverHSCode { get; set; } = string.Empty;

        /// <summary>
        /// 【司机】香港手机号
        /// </summary>
        public string DriverHKMobile { get; set; } = string.Empty;

        /// <summary>
        /// 【司机】司机卡号
        /// </summary>
        public string DriverCardNo { get; set; } = string.Empty;

        /// <summary>
        /// 【司机】口岸电子编号
        /// </summary>
        public string DriverPortElecNo { get; set; } = string.Empty;

        /// <summary>
        /// 【司机】寮步密码
        /// </summary>
        public string DriverLaoPaoCode { get; set; } = string.Empty;

        /// <summary>
        /// 总件数
        /// </summary>
        public int TotalPacks { get; set; }

        /// <summary>
        /// 总重量
        /// </summary>
        public decimal TotalWeight { get; set; }

        /// <summary>
        /// 口岸
        /// </summary>
        public string CustomsPort { get; set; }

        /// <summary>
        /// 车辆尺寸
        /// </summary>
        public string VehicleSize { get; set; }
    }

    public class CgDelcareSZPrice
    {
        public List<CgDelcareSZPriceItem> Items { get; set; }
    }

    public class CgDelcareSZPriceItem
    {
        public string OrderID { get; set; }

        public string TinyOrderID { get; set; }

        public string OrderItemID { get; set; }

        public decimal InUnitPrice { get; set; }

        public decimal OutUnitPrice { get; set; }
    }
}
