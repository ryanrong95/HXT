using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yahv.PsWms.SzMvc.Models
{
    public class GetStorageDetailInModel
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
        /// 总数量(总件数)
        /// </summary>
        public int AllPackageNumber { get; set; }

        /// <summary>
        /// 全部的总数
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
        /// 快递公司名称
        /// </summary>
        public string ExpressCompanyName { get; set; }

        /// <summary>
        /// 快递单号
        /// </summary>
        public string ExpressNumber { get; set; }

        /// <summary>
        /// 提货时间
        /// </summary>
        public string TakingDate { get; set; }

        /// <summary>
        /// 交货联系人姓名
        /// </summary>
        public string ConsigneeManName { get; set; }

        /// <summary>
        /// 交货联系人电话
        /// </summary>
        public string ConsigneeManTel { get; set; }

        /// <summary>
        /// 交货联系人地址
        /// </summary>
        public string ConsigneeManAddress { get; set; }

        /// <summary>
        /// 提货单文件
        /// </summary>
        public File[] TakingFiles { get; set; }

        /// <summary>
        /// 装箱单文件
        /// </summary>
        public File[] PackingFiles { get; set; }

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
        }

        public class File
        {
            /// <summary>
            /// 文件名
            /// </summary>
            public string name { get; set; }

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
        }
    }
}