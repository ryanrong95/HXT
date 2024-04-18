using Needs.Ccs.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.PlanningService
{
    public abstract class BasePreProduct
    {
        virtual public void Enter()
        {

        }
    }

    /// <summary>
    /// TODO:不使用的字段，不进行设计
    /// </summary>
    [Serializable]
    public class DyjPreProduct: BasePreProduct
    {
        public DyjPreProduct()
        {

        }

        public string ID { get; set; }

        public string 制单日期 { get; set; }
        public string 型号 { get; set; }
        public decimal 数量 { get; set; }
        public decimal 报关价格 { get; set; }
        public string 商检 { get; set; }
        public string 特殊包装 { get; set; }
        public string 报关公司 { get; set; }
        public string 来源 { get; set; }
        public string 状态 { get; set; }
        public string 厂家 { get; set; }
        public string 产地 { get; set; }
        public string 批号 { get; set; }
        public string 分公司 { get; set; }
        public string 部门 { get; set; }
        public string 业务员 { get; set; }
        public string 供应商 { get; set; }
        public string 备注 { get; set; }
        public string 打印 { get; set; }
        public string 品名 { get; set; }
        public string 用途 { get; set; }
        public string 商品编码 { get; set; }
        public string ArrivalTime { get; set; }
    }

    [Serializable]
    public class DyjSinglePreProduct
    {
        public string 单据号 { get; set; }
        public string 制单日期 { get; set; }
        public string 型号 { get; set; }
        public string 数量 { get; set; }
        public string 厂家 { get; set; }
        public string 产地 { get; set; }
        public string 批号 { get; set; }
        public string 封装 { get; set; }
        public string 描述 { get; set; }
        public string 开票品名 { get; set; }
        public string 用途 { get; set; }
        public string 报关品名 { get; set; }
        public string 商检 { get; set; }
        public string 特殊包装 { get; set; }
        public string 型号归类编码 { get; set; }
        public string 型号归类值 { get; set; }
        public string State { get; set; }
        public string 信息归类码 { get; set; }
        public string 信息归类值 { get; set; }
        public string 来源 { get; set; }
        public string 下单公司 { get; set; }
        public string 委托公司 { get; set; }
        public string 单位 { get; set; }
    }

    [Serializable]
    public class IcgooPreProduct
    {
        /// <summary>
        /// 唯一值
        /// </summary>
        public string sale_orderline_id { get; set; }

        public string partno { get; set; }

        public string mfr { get; set; }

        public decimal product_qty { get; set; }

        public decimal price { get; set; }

        public string currency_code { get; set; }
        public string supplier { get; set; }
        /// <summary>
        /// 预计到货日期
        /// </summary>
        public DateTime? arrival_date { get; set; }

        /// <summary>
        /// 报关公司,固定两个值XDT,HY
        /// </summary>
        public string customs_company { get; set; }
        /// <summary>
        /// 下单公司 北京创新在线电子产品销售有限公司 或者 北京创新在线电子产品销售有限公司深圳分公司
        /// </summary>
        public string order_company { get; set; }
    }

    [Serializable]
    public class KbPreProduct
    {
        /// <summary>
        /// 唯一值
        /// </summary>
        public string sale_orderline_id { get; set; }

        public string partno { get; set; }

        public string mfr { get; set; }

        public decimal product_qty { get; set; }

        public decimal price { get; set; }

        public string currency_code { get; set; }
        public string supplier { get; set; }
        /// <summary>
        /// 预计到货日期
        /// </summary>
        public DateTime? arrival_date { get; set; }
    }

    public class IcgooConsultProduct
    {
        public string supplier_partno { get; set; }
        public string state { get; set; }
        public string partno { get; set; }
        public string dc { get; set; }
        public decimal price { get; set; }
        public string currency_code { get; set; }
        public decimal qty { get; set; }
        public string mfr { get; set; }
        public string supplier { get; set; }
        public string sale_order_line_id { get; set; }
        public string customs_company { get; set; }
        public string inspection { get; set; }
        public string sales_name { get; set; }
    }
}