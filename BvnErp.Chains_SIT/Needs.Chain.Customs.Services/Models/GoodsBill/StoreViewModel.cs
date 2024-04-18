using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class StoreViewModel
    {
        public string ID { get; set; }
        /// <summary>
        /// 报关单-报关品名
        /// </summary>
        public string GName { get; set; }
        /// <summary>
        /// 报关单-型号
        /// </summary>
        public string GoodsModel { get; set; }
        /// <summary>
        /// 库存数量（入库-出库）
        /// </summary>
        public decimal StockQty
        {
            get
            {
                return this.InStoreQty - this.OutStoreQty;
            }
        }
        /// <summary>
        /// 入库数量
        /// </summary>
        public decimal InStoreQty { get; set; }
        /// <summary>
        /// 出库数量
        /// </summary>
        public decimal OutStoreQty { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string Gunit { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string GunitName { get; set; }
        /// <summary>
        /// 报关数量
        /// </summary>
        public decimal GQty { get; set; }
        /// <summary>
        /// 进价：完税价格/数量
        /// </summary>
        public decimal PurchasingPrice
        {
            get
            {
                return Math.Round(this.TaxedPrice == null ? 0 : this.TaxedPrice.Value / this.GQty, 2, MidpointRounding.AwayFromZero);
            }
        }
        /// <summary>
        /// 完税价格：Round(Round(报关总价*运保杂,2)*海关汇率,0)+关税       
        /// </summary>
        public decimal? TaxedPrice { get; set; }
        /// <summary>
        /// 合同号
        /// </summary>
        public string ContrNo { get; set; }
        /// <summary>
        /// 入库日期
        /// </summary>
        public DateTime InStoreDate { get; set; }
        /// <summary>
        /// 入库人
        /// </summary>
        public string InStoreAdminID { get; set; }
        /// <summary>
        /// 入库人
        /// </summary>
        public string InStoreAdminName { get; set; }
        /// <summary>
        /// 关税税费单号
        /// </summary>
        public string TariffTaxNumber { get; set; }
        /// <summary>
        /// 增值税税费单号
        /// </summary>
        public string ValueAddedTaxNumber { get; set; }
        public string DeclarationID { get; set; }
        public string GoodsBrand { get; set; }
        public decimal DeclPrice { get; set; }
        public decimal DeclTotal { get; set; }
        public string TradeCurr { get; set; }
        public string OrderItemID { get; set; }
        public DateTime? DDate { get; set; }
        public string OrderID { get; set; }
        public string OperatorID { get; set; }
        public string OperatorName { get; set; }
        public string OwnerName { get; set; }
        public string InputID { get; set; }
    }
}
