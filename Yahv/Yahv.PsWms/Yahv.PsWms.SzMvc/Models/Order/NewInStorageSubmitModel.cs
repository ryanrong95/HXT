using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yahv.PsWms.SzMvc.Models
{
    public class NewInStorageSubmitModel
    {
        /// <summary>
        /// 订单项
        /// </summary>
        public OrderItem[] OrderItems { get; set; }

        /// <summary>
        /// 特殊要求的值
        /// </summary>
        public int[] SpecialRequireValues { get; set; }

        /// <summary>
        /// 其他要求的值
        /// </summary>
        public string OtherRequire { get; set; }

        /// <summary>
        /// 货运类型 Int
        /// </summary>
        public int TransportModeInt { get; set; }

        /// <summary>
        /// 快递公司的值
        /// </summary>
        public string ExpressCompanyValue { get; set; }

        /// <summary>
        /// 快递单号
        /// </summary>
        public string ExpressNumber { get; set; }

        /// <summary>
        /// 提货时间
        /// </summary>
        public string TakingDate { get; set; }

        /// <summary>
        /// 交货联系人信息的值
        /// </summary>
        public string ConsigneeManValue { get; set; }

        /// <summary>
        /// 提货单文件的值
        /// </summary>
        public FileInfo[] TakingFiles { get; set; }

        /// <summary>
        /// 装箱单文件的值
        /// </summary>
        public FileInfo[] PackingFiles { get; set; }


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
            /// 包装类型 Int
            /// </summary>
            public int StocktakingTypeInt { get; set; }

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

        public class FileInfo
        {
            public string name { get; set; }

            public string URL { get; set; }

            public string fullURL { get; set; }

            public string fileFormat { get; set; }
        }
    }
}