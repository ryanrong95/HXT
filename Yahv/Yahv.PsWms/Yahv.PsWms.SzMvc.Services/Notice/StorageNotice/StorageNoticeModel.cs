using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PsWms.SzMvc.Services.Notice
{
    public class StorageNoticeModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string WarehouseID { get; set; }

        /// <summary>
        /// ClientID
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// CompanyID
        /// </summary>
        public string CompanyID { get; set; }

        /// <summary>
        /// 通知类型
        /// </summary>
        public string NoticeType { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public string FormID { get; set; }

        /// <summary>
        /// TrackerID
        /// </summary>
        public string TrackerID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 订单项
        /// </summary>
        public Item[] Items { get; set; }

        /// <summary>
        /// 特殊要求
        /// </summary>
        public Require[] Requires { get; set; }

        ///// <summary>
        ///// 文件
        ///// </summary>
        //public File[] Files { get; set; }

        /// <summary>
        /// 交货人信息(入库订单用)
        /// </summary>
        public Transport Consignor { get; set; }

        /// <summary>
        /// 收货人信息(出库订单用)
        /// </summary>
        public Transport Consignee { get; set; }


        public class Product
        {
            /// <summary>
            /// 型号
            /// </summary>
            public string Partnumber { get; set; }

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
            /// 最小包装量
            /// </summary>
            public string Mpq { get; set; }

            /// <summary>
            /// 最小起订量
            /// </summary>
            public string Moq { get; set; }
        }

        public class Item
        {
            /// <summary>
            /// 产品
            /// </summary>
            public Product Product { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string ID { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string Source { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string InputID { get; set; }

            /// <summary>
            /// 自定义编号
            /// </summary>
            public string CustomCode { get; set; }

            /// <summary>
            /// 包装类型 Int
            /// </summary>
            public int StocktakingType { get; set; }

            /// <summary>
            /// 最小包装量
            /// </summary>
            public int Mpq { get; set; }

            /// <summary>
            /// 数量(件数)
            /// </summary>
            public int PackageNumber { get; set; }

            /// <summary>
            /// 总数
            /// </summary>
            public int Total { get; set; }

            /// <summary>
            /// 币种 Int
            /// </summary>
            public int Currency { get; set; }

            /// <summary>
            /// 单价
            /// </summary>
            public decimal UnitPrice { get; set; }

            /// <summary>
            /// 供应商
            /// </summary>
            public string Supplier { get; set; }

            /// <summary>
            /// ClientID
            /// </summary>
            public string ClientID { get; set; }

            /// <summary>
            /// 订单号
            /// </summary>
            public string FormID { get; set; }

            /// <summary>
            /// 型号 ID
            /// </summary>
            public string FormItemID { get; set; }

            /// <summary>
            /// StorageID
            /// </summary>
            public string StorageID { get; set; }

            /// <summary>
            /// ShelveID
            /// </summary>
            public string ShelveID { get; set; }
        }

        public class Require
        {
            /// <summary>
            /// OrderID
            /// </summary>
            public string OrderID { get; set; }

            /// <summary>
            /// OrderTransportID
            /// </summary>
            public string OrderTransportID { get; set; }

            /// <summary>
            /// 特殊要求名称
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 其他要求的内容
            /// </summary>
            public string Contents { get; set; }
        }

        //public class File
        //{
        //    /// <summary>
        //    /// 文件类型 Int
        //    /// </summary>
        //    public string Type { get; set; }

        //    /// <summary>
        //    /// 原本的文件名
        //    /// </summary>
        //    public string CustomName { get; set; }

        //    /// <summary>
        //    /// Url
        //    /// </summary>
        //    public string Url { get; set; }

        //    /// <summary>
        //    /// AdminID
        //    /// </summary>
        //    public string AdminID { get; set; }

        //    /// <summary>
        //    /// SiteuserID
        //    /// </summary>
        //    public string SiteuserID { get; set; }
        //}

        public class Transport
        {
            /// <summary>
            /// ID
            /// </summary>
            public string ID { get; set; }

            /// <summary>
            /// 货运类型
            /// </summary>
            public string TransportMode { get; set; }

            /// <summary>
            /// 承运商
            /// </summary>
            public string Carrier { get; set; }

            /// <summary>
            /// 运单号
            /// </summary>
            public string WaybillCode { get; set; }

            /// <summary>
            /// 运费负担方
            /// </summary>
            public int ExpressPayer { get; set; }

            /// <summary>
            /// 快递运费
            /// </summary>
            public decimal ExpressFreight { get; set; }

            /// <summary>
            /// 月结账号
            /// </summary>
            public string ExpressEscrow { get; set; }

            /// <summary>
            /// 提送货时间
            /// </summary>
            public string TakingTime { get; set; }

            /// <summary>
            /// 提/送货联系人
            /// </summary>
            public string TakerName { get; set; }

            /// <summary>
            /// 提/送货人车牌
            /// </summary>
            public string TakerLicense { get; set; }

            /// <summary>
            /// 提/送货人联系电话
            /// </summary>
            public string TakerPhone { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string TakerIDCode { get; set; }

            /// <summary>
            /// 证件类型
            /// </summary>
            public string TakerIDType { get; set; }

            /// <summary>
            /// 地址
            /// </summary>
            public string Address { get; set; }

            /// <summary>
            /// 联系人
            /// </summary>
            public string Contact { get; set; }

            /// <summary>
            /// 联系电话
            /// </summary>
            public string Phone { get; set; }

            /// <summary>
            /// Email
            /// </summary>
            public string Email { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string Summary { get; set; }

            /// <summary>
            /// 快递方式
            /// </summary>
            public string ExpressTransport { get; set; }
        }
    }
}
