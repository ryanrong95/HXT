using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yahv.PsWms.SzMvc.Models
{
    public class StorageListModel
    {
        /// <summary>
        /// StorageID
        /// </summary>
        public string StorageID { get; set; }

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
        /// 包装类型名称
        /// </summary>
        public string StocktakingTypeName { get; set; }

        /// <summary>
        /// 最小包装量
        /// </summary>
        public int Mpq { get; set; }

        /// <summary>
        /// 数量(件数)
        /// </summary>
        public int PackageNumber { get; set; }

        /// <summary>
        /// 库位号
        /// </summary>
        public string LocationNo { get; set; }

        /// <summary>
        /// 型号总数
        /// </summary>
        public int ItemTotal { get; set; }

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsCheck { get; set; }

        /// <summary>
        /// 需要的数量
        /// </summary>
        public int NeedPackageNumber { get; set; }

        /// <summary>
        /// 库存数量(件数)
        /// </summary>
        public int StoragePackageNumber { get; set; }
    }
}