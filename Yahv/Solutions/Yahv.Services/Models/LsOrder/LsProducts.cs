using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;

namespace Yahv.Services.Models.LsOrder
{
    /// <summary>
    /// 租赁产品表
    /// </summary>
    public class LsProducts : Yahv.Linq.IUnique
    {
        #region 属性
        /// <summary>
        /// 主键ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 库位ID
        /// </summary>
        public string SpecID { get; set; }

        /// <summary>
        /// 库位承重
        /// </summary>
        public int Load { get; set; }

        /// <summary>
        /// 库位容积
        /// </summary>
        public string Volume { get; set; }

        /// <summary>
        /// 可用数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyDate { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { get; set; }
        #endregion

        /// <summary>
        /// 价格
        /// </summary>
        public LsProductPrices[] LsProductPrice { get; set; }
    }

    /// <summary>
    /// 租赁产品价格指导表
    /// </summary>
    public class LsProductPrices : Yahv.Linq.IUnique
    {
        #region 属性
        private string id;
        public string ID
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.id))
                {
                    this.id = string.Concat(this.ProductID, this.Month, this.Currency).MD5();
                }
                return this.id;
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 产品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 时长
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 操作人
        /// </summary>
        public string Creator { get; set; }
        public string Summary { get; set; }
        #endregion

        #region 扩展属性
        public LsProducts LsProduct { get; set; }
        /// <summary>
        /// 操作人姓名
        /// </summary>
        public string CreatorName
        {
            get
            {
                if (this.Creator==null)
                {
                    return "";
                }
                return new Yahv.Services.Views.AdminsAll<Layers.Data.Sqls.PvbErmReponsitory>().FirstOrDefault(t=>t.ID==this.Creator).RealName;
            }
        }
        #endregion
    }
}
