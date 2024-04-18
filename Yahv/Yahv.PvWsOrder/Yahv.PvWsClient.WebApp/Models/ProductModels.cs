using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yahv.PvWsClient.WebApp.Models
{
    /// <summary>
    /// 我的产品
    /// </summary>
    public class MyProductsViewModel
    {
        public string ID { get; set; }
        /// <summary>
        /// 批次
        /// </summary>
        public string Batch { get; set; }

        /// <summary>
        /// 品名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string Models { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
      //  public decimal Price { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }
    }

    /// <summary>
    /// 产品预归类
    /// </summary>
    public class PreProductInfoViewModel
    {
        public string ID { get; set; }
        /// <summary>
        /// 品牌
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string Models { get; set; }

        /// <summary>
        /// 货币
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public string Supplier { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// 物流号
        /// </summary>
        public string ProductUnionCode { get; set; }

        /// <summary>
        /// 品名
        /// </summary>
        public string ProductName { get; set; }
    }

    /// <summary>
    /// 预归类产品导入数据模型
    /// </summary>
    public class PreProcuctsData
    {
        //产品名称
        public string Name { get; set; }

        //批次号
        public string Batch { get; set; }

        //型号
        public string Model { get; set; }

        //单价
        public string UnitPrice { get; set; }

        //品牌
        public string Manufacturer { get; set; }

        //币种
        public string Currency { get; set; }

        //物流号
        public string UniqueCode { get; set; }
        /// <summary>
        /// 预计到货日期
        /// </summary>
        public string DueDate { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public string Qty { get; set; }
    }
}