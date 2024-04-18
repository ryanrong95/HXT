using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class OutStoreViewModel:IUnique
    {
        /// <summary>
        /// DeclistID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 出库日期
        /// </summary>
        public DateTime OutStoreDate { get; set; }
        /// <summary>
        /// 开票日期
        /// </summary>
        public DateTime? InvoiceDate { get; set; }
        public string InvoiceDateShow { get; set; }
        /// <summary>
        /// 报关品名
        /// </summary>
        public string GName { get; set; }
        /// <summary>
        /// 品牌
        /// </summary>
        public string GoodsBrand { get; set; }
        /// <summary>
        /// 型号
        /// </summary>
        public string GoodsModel { get; set; }
        /// <summary>
        /// 报关单数量
        /// </summary>
        public decimal GQty { get; set; }
        /// <summary>
        /// 发票数量
        /// </summary>
        public decimal InvoiceQty { get; set; }
        /// <summary>
        /// 已出库数量
        /// </summary>
        public decimal? OutQty { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string Gunit { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string GunitName { get; set; }
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
        public decimal DeclPrice { get; set; }
        /// <summary>
        /// 外币价格
        /// </summary>
        public decimal DeclTotal { get; set; }
        /// <summary>
        /// 客户名称：报关单-消费使用单位
        /// </summary>
        public string OwnerName { get; set; }
        /// <summary>
        /// 税务名称
        /// </summary>
        public string TaxName { get; set; }
        /// <summary>
        /// 税务编码
        /// </summary>
        public string TaxCode { get; set; }
        /// <summary>
        /// 出库人
        /// </summary>
        public string OperatorID { get; set; }
        /// <summary>
        /// 出库人
        /// </summary>
        public string OperatorName { get; set; }
        /// <summary>
        /// 售价,如果是服务费发票则不需要显示
        /// </summary>
        public decimal SalesPrice
        {
            get
            {
                return Math.Round(this.InvoicePrice / this.GQty, 4);
            }
        }
        /// <summary>
        /// 开票金额(不含税)：Round（外币价格*实时汇率，4），如果是服务费发票不需要显示
        /// </summary>
        public decimal InvoicePrice
        {
            get
            {
                return Math.Round(this.DeclTotal * this.RealExchangeRate.Value, 4);
            }
        }
        /// <summary>
        /// 发票号，如果是服务费发票不需要显示
        /// </summary>
        public string InvoiceNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string WaybillID { get; set; }
        /// <summary>
        /// 实时汇率
        /// </summary>
        public decimal? RealExchangeRate { get; set; }
        public int InvoiceType { get; set; }
        public string DeclarationID { get; set; }
        public string TradeCurr { get; set; }
        public string OrderItemID { get; set; }
        /// <summary>
        /// 报关单-报关日期
        /// </summary>
        public DateTime? DDate { get; set; }
        public string OrderID { get; set; }
        public string ContrNo { get; set; }
        public string ClientID { get; set; }
        public DateTime? StorageDate { get; set; }
        public string InvoiceNoticeID { get; set; }
        public string InputID { get; set; }
    }
}
