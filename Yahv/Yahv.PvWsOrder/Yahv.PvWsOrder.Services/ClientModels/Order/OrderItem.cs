using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Usually;
using Yahv.Utils.Serializers;

namespace Yahv.PvWsOrder.Services.ClientModels
{
    /// <summary>
    /// 订单项
    /// </summary>
    public class OrderItem : Linq.IUnique
    {
        public OrderItem()
        {
            this.CreateDate = this.ModifyDate = DateTime.Now;
            this.Status = OrderItemStatus.Normal;
            this.Type = OrderItemType.Normal;
            this.IsAuto = false;
        }

        #region 属性

        public string ID { get; set; }

        /// <summary>
        /// 品名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public OrderItemType Type { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 小订单ID
        /// </summary>
        public string TinyOrderID { get; set; }

        /// <summary>
        /// 可以与ID保持一致，但是未来一定要考虑拆分。因此建议使用新的规则生成:Ipt+yyyyMMdd+count
        /// </summary>
        public string InputID { get; set; }

        /// <summary>
        /// 出库ID
        /// </summary>
        public string OutputID { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 原产地
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// 批次号
        /// </summary>
        public string DateCode { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public LegalUnit Unit { get; set; }

        /// <summary>
        /// 总价
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// 毛重
        /// </summary>
        public decimal? GrossWeight { get; set; }

        /// <summary>
        /// 体积
        /// </summary>
        public decimal? Volume { get; set; }

        /// <summary>
        /// 入库条件
        /// </summary>
        public string Conditions { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public OrderItemStatus Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyDate { get; set; }

        /// <summary>
        /// 是否自动归类
        /// </summary>
        public bool IsAuto { get; set; }

        /// <summary>
        /// 库房反填的运单号
        /// </summary>
        public string WayBillID { get; set; }

        /// <summary>
        /// 海关品名
        /// </summary>
        public string CustomName { get; set; }
        #endregion

        #region 扩展属性
        /// <summary>
        /// 物料编号
        /// </summary>
        public string ProductUnicode { get; set; }

        /// <summary>
        /// 报关品名
        /// </summary>
        public string ClassfiedName { get; set; }

        /// <summary>
        /// 归类产品ID
        /// </summary>
        public string PreProductID { get; set; }

        /// <summary>
        /// 归类出的管控信息
        /// </summary>
        public OrderItemsTerm ItemsTerm { get; set; }

        /// <summary>
        /// 归类信息
        /// </summary>
        public OrderItemsChcd ItemsChcd { get; set; }

        /// <summary>
        /// 产品对象
        /// </summary>
        public CenterProduct Product { get; set; }

        /// <summary>
        /// 入库条件
        /// </summary>
        public OrderItemCondition OrderItemCondition
        {
            get
            {
                return this.Conditions.JsonTo<OrderItemCondition>();
            }
        }

        /// <summary>
        /// 库存ID
        /// </summary>
        public string StorageID { get; set; }

        /// <summary>
        /// 库存编号
        /// </summary>
        public string WareHouseID { get; set; }

        /// <summary>
        /// 关税
        /// </summary>
        public decimal Traiff { get; set; }

        /// <summary>
        /// 关税率
        /// </summary>
        public decimal TraiffRate { get; set; }

        /// <summary>
        /// 消费税
        /// </summary>
        public decimal ExcisePrice { get; set; }
        /// <summary>
        /// 消费税率
        /// </summary>
        public decimal ExciseTaxRate { get; set; }

        /// <summary>
        /// 增值税率
        /// </summary>
        public decimal AddTaxRate { get; set; }

        /// <summary>
        /// 实时汇率
        /// </summary>
        public decimal RealExchangeRate { get; set; }

        /// <summary>
        /// 海关汇率
        /// </summary>
        public decimal CustomsExchangeRate { get; set; }

        /// <summary>
        /// 增值税
        /// </summary>
        public decimal AddTax { get; set; }

        /// <summary>
        /// 代理费
        /// </summary>
        public decimal AgencyFee { get; set; }

        /// <summary>
        /// 商检费，杂费
        /// </summary>
        public decimal InspectionFee { get; set; }

        /// <summary>
        /// 报关货值
        /// </summary>
        public decimal DeclareTotalPrice { get; set; }
        #endregion
    }
}
