using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class InStoreViewModel:IUnique
    {
        /// <summary>
        /// 报关单-编号
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 没有入库日期显示:在途，有入库日期显示入库日期
        /// </summary>
        public DateTime? InStoreDate { get; set; }
        public string InStoreDateShow { get; set; }
        /// <summary>
        /// 只有两个状态（在途，已入库）  
        /// </summary>
        public string GoodsStatus
        {
            get
            {
                if (string.IsNullOrEmpty(this.OperatorID))
                {
                    return "在途";

                }
                else
                {
                    return "已入库";
                }
            }
        }
        /// <summary>
        /// 报关单-报关品名
        /// </summary>
        public string GName { get; set; }
        /// <summary>
        /// 报关单-品牌
        /// </summary>
        public string GoodsBrand { get; set; }
        /// <summary>
        /// 报关单-型号
        /// </summary>
        public string GoodsModel { get; set; }
        /// <summary>
        /// 报关单-数量
        /// </summary>
        public decimal GQty { get; set; }
        /// <summary>
        /// 报关单-单位
        /// </summary>
        public string Gunit { get; set; }
        /// <summary>
        /// 报关单-单位名称
        /// </summary>
        public string GunitName { get; set; }

        /// <summary>
        /// 报关单-单价
        /// </summary>
        public decimal DeclPrice { get; set; }
        /// <summary>
        /// 报关单-金额
        /// </summary>
        public decimal DeclTotal { get; set; }
        /// <summary>
        /// 进价：完税价格/数量
        /// </summary>
        public decimal PurchasingPrice
        {
            get
            {
                return Math.Round(this.TaxedPrice==null?0:this.TaxedPrice.Value / this.GQty, 2, MidpointRounding.AwayFromZero);
            }
        }
        /// <summary>
        /// 完税价格：Round(Round(报关总价*运保杂,2)*海关汇率,0)+关税
        /// DeclTotal = Round(报关总价*运保杂,2)
        /// 持久化到表中
        /// </summary>
        public decimal? TaxedPrice { get; set; }
        /// <summary>
        /// 报关单-币值
        /// </summary>
        public string TradeCurr { get; set; }
        /// <summary>
        /// 报关单-合同号
        /// </summary>
        public string ContrNo { get; set; }
        /// <summary>
        /// 报关单-报关日期
        /// </summary>
        public DateTime? DDate { get; set; }
        /// <summary>
        /// 客户名称：报关单-消费使用单位
        /// </summary>
        public string OwnerName { get; set; }
        /// <summary>
        /// 入库人id
        /// </summary>
        public string OperatorID { get; set; }
        /// <summary>
        /// 入库人姓名
        /// </summary>
        public string OperatorName { get; set; }

        /// <summary>
        /// 关税税费单号
        /// </summary>
        public string TariffTaxNumber { get; set; }
        /// <summary>
        /// 增值税税费单号
        /// </summary>
        public string ValueAddedTaxNumber { get; set; }
        /// <summary>
        /// 六联单号
        /// </summary>
        public string VoyNo { get; set; }
        public string LotNumber { get; set; }
        public string OrderItemID { get; set; }
        public string DeclarationID { get; set; }
        public string OrderID { get; set; }
        public string InputID { get; set; }
    }
}
