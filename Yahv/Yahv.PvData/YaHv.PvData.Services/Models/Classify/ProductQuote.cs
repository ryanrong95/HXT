using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Usually;

namespace YaHv.PvData.Services.Models
{
    /// <summary>
    /// 产品报价
    /// </summary>
    public class ProductQuote : IUnique
    {
        #region 属性

        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string PartNumber { get; set; }

        /// <summary>
        /// 品牌/制造商
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 产地
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// 商检费
        /// </summary>
        public decimal CIQprice { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        #endregion

        #region 事件

        public event SuccessHanlder EnterSuccess;

        #endregion

        #region 持久化

        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvDataReponsitory>.Create())
            {
                this.ID = Layers.Data.PKeySigner.Pick(PKeyType.ProductQuote);
                reponsitory.Insert(new Layers.Data.Sqls.PvData.ProductQuotes()
                {
                    ID = this.ID,
                    PartNumber = this.PartNumber,
                    Manufacturer = this.Manufacturer,
                    Origin = this.Origin,
                    Currency = this.Currency,
                    UnitPrice = this.UnitPrice,
                    Quantity = this.Quantity,
                    CIQprice = this.CIQprice,
                    CreateDate = DateTime.Now
                });
            }

            if (this != null && this.EnterSuccess != null)
            {
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

        #endregion
    }
}
