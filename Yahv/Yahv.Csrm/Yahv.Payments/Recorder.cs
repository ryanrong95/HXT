using System;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Services;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;

namespace Yahv.Payments
{
    /// <summary>
    /// 记录员基类
    /// </summary>
    abstract public class Recorder : ReceivedBase
    {
        public PayInfo PayInfo { get; internal set; }

        /// <summary>
        /// 深圳市芯达通供应链管理有限公司
        /// </summary>
        /// <summary>
        /// 深圳市万路通电子有限公司
        /// </summary>
        /// <summary>
        /// 深圳海关
        /// </summary>
        /// <summary>
        /// 记账
        /// </summary>
        /// <param name="currency">币种</param>
        /// <param name="unitPrice">单价</param>
        /// <param name="quantity">数量</param>
        /// <param name="orderID">订单ID</param>
        /// <param name="waybillID">运单ID</param>
        /// <param name="id">生成ID</param>
        /// <param name="tinyID">小订单ID</param>
        /// <param name="supplierSign">供应商标识</param>
        /// <param name="applicationID">付汇申请ID</param>
        /// <param name="AgentID">代理人ID</param>
        /// <param name="source">来源</param>
        /// <param name="trackingNum">快递单号</param>
        /// <param name="data">json内容</param>
        /// <returns>返回记账ID</returns>
        abstract public string Record(Currency currency, decimal? price, string orderID = null, string waybillID = null, string id = null, string tinyID = null, decimal? rightPrice = null, string itemID = null, string applicationID = null, string AgentID = null, int? quantity = null, string source = "", string trackingNum = "", string data = "", decimal? originPrice = null);

        /// <summary>
        /// 芯达通记账
        /// </summary>
        /// <remarks>添加或者修改</remarks>
        /// <param name="currency">币种</param>
        /// <param name="price">金额</param>
        /// <param name="orderID">大订单ID</param>
        /// <param name="tinyID">小订单ID</param>
        /// <param name="supplierSign">供应商标识</param>
        /// <returns></returns>
        //abstract public string Record_XinDaTong(Currency currency, decimal price, string orderID, string tinyID, string supplierSign = null, string itemID = null, string applicationID = null);

        /// <summary>
        /// 仓储费
        /// </summary>
        /// <param name="currency">币种</param>
        /// <param name="price">金额</param>
        /// <param name="lsOrderID">租赁订单ID</param>
        virtual public string RecordStorage(Currency currency, decimal price)
        {
            return string.Empty;
        }

        /// <summary>
        /// 内部公司账
        /// 芯达通应付万路通
        /// 万路通应收芯达通
        /// </summary>
        //protected void InsideFee(Currency currency, decimal? price, string orderID, string waybillID = null)
        //{
        //    var rate = ExchangeRates.Floating[currency, Currency.CNY];

        //    using (var reponsitory = LinqFactory<PvbCrmReponsitory>.Create())
        //    {
        //        //应收
        //        reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.Receivables()
        //        {
        //            ID = PKeySigner.Pick(PKeyType.Receivables),
        //            Payer = XinDaTong.MD5(),
        //            Payee = WanLuTong.MD5(),
        //            Business = this.PayInfo.Conduct,
        //            Catalog = this.PayInfo.Catalog,
        //            Subject = this.PayInfo.Subject,

        //            Currency = (int)currency,
        //            Price = price ?? 0m,

        //            Currency1 = (int)Currency.CNY,
        //            Rate1 = rate,
        //            Price1 = (price * rate) ?? 0m,

        //            SettlementCurrency = (int)currency,
        //            SettlementPrice = price ?? 0m,
        //            SettlementRate = rate,

        //            OrderID = orderID,
        //            WaybillID = waybillID,
        //            CreateDate = DateTime.Now,
        //            AdminID = this.PayInfo.Inputer.ID,
        //            Summay = string.Empty,
        //            PayeeBeneficiaryID = null, //收款账户
        //        });

        //        //应付
        //        reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.Payables()
        //        {
        //            ID = PKeySigner.Pick(PKeyType.Payables),
        //            Payee = WanLuTong.MD5(),
        //            Payer = XinDaTong.MD5(),
        //            Business = this.PayInfo.Conduct,
        //            Subject = this.PayInfo.Subject,
        //            Currency = (int)currency,
        //            Price = price,
        //            OrderID = orderID,
        //            CreateDate = DateTime.Now,
        //            AdminID = this.PayInfo.Inputer.ID,
        //            Summay = string.Empty,
        //            PayerBeneficiaryID = null,
        //            PayeeBeneficiaryID = null,
        //            WaybillID = waybillID,
        //            Catalog = this.PayInfo.Catalog,
        //        });
        //    }
        //}

        /// <summary>
        /// 四舍五入
        /// </summary>
        /// <param name="value"></param>
        /// <param name="decimals"></param>
        /// <returns></returns>
        public decimal Round(decimal value, int decimals = 2)
        {
            decimal result = value;

            if (value < 0)
            {
                result = Math.Abs(result);
            }

            result = Math.Round(result, decimals, MidpointRounding.AwayFromZero);

            if (value < 0)
            {
                result = 0 - result;
            }

            return result;
        }
    }
}
