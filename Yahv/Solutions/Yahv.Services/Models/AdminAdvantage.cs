using System.Collections.Generic;
using Yahv.Utils.Serializers;

namespace Yahv.Services.Models
{
    /// <summary>
    /// 优势品牌型号
    /// </summary>
    public class AdminAdvantage
    {
        /// <summary>
        /// 管理员
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Manufacturers { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string PartNumbers { get; set; }

        IEnumerable<Item> manufacturersObj;
        /// <summary>
        /// 品牌
        /// </summary>
        public IEnumerable<Item> ManufacturersObj
        {
            get
            {
                if (this.manufacturersObj == null)
                {
                    this.manufacturersObj = this.Manufacturers.JsonTo<IEnumerable<Item>>();
                }
                return this.manufacturersObj;
            }
        }

        IEnumerable<Item> partNumbersObj;
        /// <summary>
        /// 型号
        /// </summary>
        public IEnumerable<Item> PartNumbersObj
        {
            get
            {
                if (this.partNumbersObj == null)
                {
                    this.partNumbersObj = this.PartNumbers.JsonTo<IEnumerable<Item>>();
                }
                return this.partNumbersObj;
            }
        }

        /// <summary>
        /// json类
        /// </summary>
        public class Item
        {
            /// <summary>
            /// 名称
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 是否代理
            /// </summary>
            public bool Agent { get; set; }
        }
    }
}
