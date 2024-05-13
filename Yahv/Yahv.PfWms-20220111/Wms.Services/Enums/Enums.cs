using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Wms.Services.Enums
{
    /// <summary>
    /// 地区简码
    /// </summary>
    public enum RegionCode
    {
        [Description("北京")]
        BJ = 1,
        [Description("上海")]
        SH = 2,
        [Description("深圳")]
        SZ = 3,
        [Description("香港")]
        HK = 4
    }

    /// <summary>
    /// 库房能力
    /// </summary>
    public enum WarehouseAbilities
    {
        [Description("检测")]
        Testing = 10,
        [Description("报关")]
        Customs = 20,
        [Description("恒温")]
        ConstantTemperature = 30,
        [Description("抽真空")]
        Vacuum = 40,
    }

    /// <summary>
    /// Shelves类型
    /// </summary>
    public enum ShelvesType
    {
        /// <summary>
        /// 库房
        /// </summary>
        [Description("库房")]
        Warehouse = 10,

        /// <summary>
        /// 库区
        /// </summary>
        [Description("库区")]
        Region = 20,

        /// <summary>
        /// 货架
        /// </summary>
        [Description("货架")]
        Shelve = 30,

        /// <summary>
        /// 卡板
        /// </summary>
        [Description("卡板")]
        Board = 40,

        /// <summary>
        /// 库位
        /// </summary>
        [Description("库位")]
        Position = 50
    }

    /// <summary>
    /// Shelves状态
    /// </summary>
    public enum ShelvesStatus
    {
        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        Normal = 200,

        /// <summary>
        /// 删除
        /// </summary>
        [Description("删除")]
        Deleted = 400,

        /// <summary>
        /// 停用
        /// </summary>
        [Description("停用")]
        StopUsing = 500,
    }

    /// <summary>
    /// 货架用途
    /// </summary>
    public enum ShelvesPurpose
    {
        /// <summary>
        /// 租赁业务
        /// </summary>
        [Description("租赁业务")]
        Rent = 10,

        /// <summary>
        /// 代仓储业务
        /// </summary>
        [Description("代仓储业务")]
        WarehouseServicing = 20,

        /// <summary>
        /// 代报关业务
        /// </summary>
        [Description("代报关业务")]
        DeclareCustoms = 30,

        /// <summary>
        /// 贸易业务
        /// </summary>
        [Description("贸易业务")]
        Trade = 40

    }

    /// <summary>
    /// 库存状态
    /// </summary>
    public enum StoragesStatus
    {
        [Description("正常")]
        Normal = 200,
        [Description("删除")]
        Deleted = 400,
        [Description("停用")]
        StopUsing = 500
    }

    /// <summary>
    /// 装箱单状态
    /// </summary>
    [Obsolete("与小辉商定：弃用")]
    public enum BoxesStatus
    {

        [Description("待申请")]
        WaitingApply = 200,
        [Description("待出库")]
        WaitingOUt = 300,
        [Description("删除")]
        Deleted = 400,
        [Description("已报关")]
        Declared = 500
    }

    /// <summary>
    /// (新的)装箱单状态
    /// </summary>
    public enum BoxingStatus
    {
        [Description("已装箱")]
        Boxed = 20,

        /// <remarks>
        /// 香港点击申报后
        /// </remarks>
        [Description("申报中")]
        Declaring = 30,

        /// <remarks>
        /// 收到报关的出库通知后
        /// </remarks>
        [Description("待装运")]
        Shiping = 40,

        /// <remarks>
        /// 香港装运后
        /// </remarks>
        [Description("已装运")]
        Shiped = 50,
    }

    /// <summary>
    /// (新的)装箱单状态
    /// </summary>
    public enum TinyOrderDeclareStatus
    {
        [Description("已装箱")]
        Boxed = 20,

        /// <remarks>
        /// 香港点击申报后
        /// </remarks>
        [Description("申报中")]
        Declaring = 30,

        /// <remarks>
        /// 收到报关的出库通知后
        /// </remarks>
        [Description("待装运")]
        Shiping = 40,

        /// <remarks>
        /// 香港装运后
        /// </remarks>
        [Description("已装运")]
        Shiped = 50,
    }
}
