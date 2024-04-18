using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yahv.PsWms.SzMvc.Models
{
    public class GetStorageDetailOutModel
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 订单项
        /// </summary>
        public OrderItem[] OrderItems { get; set; }

        /// <summary>
        /// 数量总和
        /// </summary>
        public int AllPackageNumber { get; set; }

        /// <summary>
        /// 总数总和
        /// </summary>
        public int AllItemTotal { get; set; }

        /// <summary>
        /// 特殊要求
        /// </summary>
        public SpecialRequire[] SpecialRequires { get; set; }

        /// <summary>
        /// 货运类型 Int
        /// </summary>
        public int TransportModeInt { get; set; }

        /// <summary>
        /// 货运类型名称
        /// </summary>
        public string TransportModeName { get; set; }

        /// <summary>
        /// 送货地址联系人显示(快递)
        /// </summary>
        public string DeliverTargetManShow { get; set; }

        /// <summary>
        /// 送货地址电话显示(快递)
        /// </summary>
        public string DeliverTargetTelShow { get; set; }

        /// <summary>
        /// 送货地址地址显示(快递)
        /// </summary>
        public string DeliverTargetAddressShow { get; set; }

        /// <summary>
        /// 送货地址联系人显示(送货上门)
        /// </summary>
        public string DeliverTargetMan2Show { get; set; }

        /// <summary>
        /// 送货地址电话显示(送货上门)
        /// </summary>
        public string DeliverTargetTel2Show { get; set; }

        /// <summary>
        /// 送货地址地址显示(送货上门)
        /// </summary>
        public string DeliverTargetAddress2Show { get; set; }

        /// <summary>
        /// 快递公司显示
        /// </summary>
        public string ExpressCompanyShow { get; set; }

        /// <summary>
        /// 快递方式显示
        /// </summary>
        public string ExpressMethodShow { get; set; }

        /// <summary>
        /// 运费支付 Int
        /// </summary>
        public string FreightPayInt { get; set; }

        /// <summary>
        /// 运费支付显示
        /// </summary>
        public string FreightPayShow { get; set; }

        /// <summary>
        /// 月结账号显示
        /// </summary>
        public string ThirdParty { get; set; }

        /// <summary>
        /// 提货时间
        /// </summary>
        public string TakingDate { get; set; }

        /// <summary>
        /// 提货人
        /// </summary>
        public string TakingMan { get; set; }

        /// <summary>
        /// 提货人电话
        /// </summary>
        public string TakingTel { get; set; }

        /// <summary>
        /// 证件类型显示
        /// </summary>
        public string ProofTypeShow { get; set; }

        /// <summary>
        /// 证件号码
        /// </summary>
        public string ProofNumber { get; set; }

        /// <summary>
        /// 库房修改的货运信息
        /// </summary>
        public TransportInfoFromStorage StorageTransportInfo { get; set; }

        public class OrderItem
        {
            /// <summary>
            /// 自定义编号
            /// </summary>
            public string CustomCode { get; set; }

            /// <summary>
            /// 型号
            /// </summary>
            public string PartNumber { get; set; }

            /// <summary>
            /// 品牌
            /// </summary>
            public string Brand { get; set; }

            /// <summary>
            /// 封装
            /// </summary>
            public string Package { get; set; }

            /// <summary>
            /// 批次
            /// </summary>
            public string DateCode { get; set; }

            /// <summary>
            /// 包装类型显示
            /// </summary>
            public string StocktakingTypeDes { get; set; }

            /// <summary>
            /// 最小包装量
            /// </summary>
            public int Mpq { get; set; }

            /// <summary>
            /// 数量
            /// </summary>
            public int PackageNumber { get; set; }

            /// <summary>
            /// 总数
            /// </summary>
            public int ItemTotal { get; set; }
        }

        public class SpecialRequire
        {
            /// <summary>
            /// 特殊要求名称
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 是否是其他要求
            /// </summary>
            public bool IsOtherRequire { get; set; }

            /// <summary>
            /// 其他要求
            /// </summary>
            public string OtherRequire { get; set; }

            /// <summary>
            /// 是否有文件
            /// </summary>
            public bool IsHasFile { get; set; }

            /// <summary>
            /// 文件名
            /// </summary>
            public string filename { get; set; }

            /// <summary>
            /// 全路径(http 开头)
            /// </summary>
            public string fullURL { get; set; }
        }

        public class TransportInfoFromStorage
        {
            /// <summary>
            /// 货运方式 Int 1-自提 2-快递 3-送货上门
            /// </summary>
            public int TransportModeInt { get; set; }

            /// <summary>
            /// 承运商名称
            /// </summary>
            public string CarrierName { get; set; }

            /// <summary>
            /// 运单号
            /// </summary>
            public string WaybillCode { get; set; }

            /// <summary>
            /// 支付方式 Int
            /// </summary>
            public string ExpressPayerInt { get; set; }

            /// <summary>
            /// 支付方式显示
            /// </summary>
            public string ExpressPayerShow { get; set; }

            /// <summary>
            /// 承运类型
            /// </summary>
            public string ExpressTransportShow { get; set; }
        }
    }
}