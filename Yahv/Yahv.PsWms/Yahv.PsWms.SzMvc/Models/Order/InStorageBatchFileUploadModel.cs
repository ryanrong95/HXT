using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yahv.PsWms.SzMvc.Models
{
    public class InStorageBatchFileUploadReturnModel
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 型号列表
        /// </summary>
        public OrderItem[] list { get; set; }

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
            /// 包装类型 Int(这里类型要 string)
            /// </summary>
            public string StocktakingTypeInt { get; set; }

            /// <summary>
            /// 最小包装量
            /// </summary>
            public int Mpq { get; set; }

            /// <summary>
            /// 数量(件数)
            /// </summary>
            public int PackageNumber { get; set; }
        }
    }
}