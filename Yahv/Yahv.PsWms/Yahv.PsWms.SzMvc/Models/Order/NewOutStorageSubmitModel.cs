using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yahv.PsWms.SzMvc.Models
{
    public class NewOutStorageSubmitModel
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
        /// 发货单文件信息
        /// </summary>
        public FileInfo FileInfoFaHuoDan { get; set; }

        /// <summary>
        /// 客户标签文件信息
        /// </summary>
        public FileInfo FileInfoKeHuBiaoQian { get; set; }

        /// <summary>
        /// 货运类型 Int
        /// </summary>
        public int TransportModeInt { get; set; }

        /// <summary>
        /// 送货地址的值(快递)
        /// </summary>
        public string DeliverTargetValue { get; set; }

        /// <summary>
        /// 快递公司的值
        /// </summary>
        public string ExpressCompanyValue { get; set; }

        /// <summary>
        /// 快递方式的值
        /// </summary>
        public string ExpressMethodValue { get; set; }

        /// <summary>
        /// 运费支付的值
        /// </summary>
        public string FreightPayValue { get; set; }

        /// <summary>
        /// 月结账号的值
        /// </summary>
        public string ThirdParty { get; set; }

        /// <summary>
        /// 送货地址的值(送货上门)
        /// </summary>
        public string DeliverTargetValue2 { get; set; }

        /// <summary>
        /// 提货时间的值
        /// </summary>
        public string TakingDate { get; set; }

        /// <summary>
        /// 提货人信息 ID
        /// </summary>
        public string TakingID { get; set; }

        /// <summary>
        /// 提货人的值
        /// </summary>
        public string TakingMan { get; set; }

        /// <summary>
        /// 提货人电话的值
        /// </summary>
        public string TakingTel { get; set; }

        /// <summary>
        /// 证件类型的值
        /// </summary>
        public string ProofTypeValue { get; set; }

        /// <summary>
        /// 证件号码的值
        /// </summary>
        public string ProofNumber { get; set; }

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

            /// <summary>
            /// StorageID
            /// </summary>
            public string StorageID { get; set; }
        }


        public class FileInfo
        {
            /// <summary>
            /// 文件名(ya3.jpg)
            /// </summary>
            public string FileName { get; set; }

            /// <summary>
            /// 以 http 开头的文件地址(http://localhost:6662/Files/FaHuoDan/2021/01/10/d2d805261610284276614.jpg)
            /// </summary>
            public string FileUrl { get; set; }

            /// <summary>
            /// 是否上传(true)
            /// </summary>
            public bool IsUploaded { get; set; }

            /// <summary>
            /// 要保存的文件地址(FaHuoDan/2021/01/10/d2d805261610284276614.jpg)
            /// </summary>
            public string URL { get; set; }

            /// <summary>
            /// 文件格式(image/jpeg)
            /// </summary>
            public string fileFormat { get; set; }
        }
    }
}