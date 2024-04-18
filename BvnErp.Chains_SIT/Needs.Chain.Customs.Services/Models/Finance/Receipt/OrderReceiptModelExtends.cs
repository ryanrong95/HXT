using Needs.Linq;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 订单实收费用统计
    /// </summary>
    public class OrderReceivedFee : IUnique
    {
        /// <summary>
        /// 主键ID/订单ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public Enums.OrderStatus OrderStatus { get; set; }

        /// <summary>
        /// 下单日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 已收货款
        /// </summary>
        public decimal? ProductFee { get; set; }

        /// <summary>
        /// 已收关税
        /// </summary>
        public decimal? Tariff { get; set; }

        /// <summary>
        /// 已收增值税
        /// </summary>
        public decimal? AddedValueTax { get; set; }

        /// <summary>
        /// 已收代理费
        /// </summary>
        public decimal? AgencyFee { get; set; }
        /// <summary>
        /// 已收杂费
        /// </summary>
        public decimal? IncidentalFee { get; set; }

        /// <summary>
        /// 订单是否已经完成收款
        /// </summary>
        public bool IsCompleted { get; set; }
    }

    /// <summary>
    /// 订单费用明细维护
    /// </summary>
    public class OrderReceivedDetail : IUnique
    {
        /// <summary>
        /// 主键ID/订单ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 订单已收款
        /// </summary>
        public IEnumerable<OrderFeeModel> ReceivedFees { get; set; }

        /// <summary>
        /// 订单未收款
        /// </summary>
        public IEnumerable<OrderFeeModel> UnReceiveFees { get; set; }
    }

    public class OrderFeeModel
    {
        /// <summary>
        /// 费用来源
        /// </summary>
        public string FeeSourceID { get; set; }

        /// <summary>
        /// 费用名称
        /// </summary>
        public string Name
        {
            get
            {
                if (this.Type == Enums.OrderFeeType.Incidental)
                {
                    //商检费
                    if (this.FeeSourceID == null)
                    {
                        return Enums.OrderPremiumType.InspectionFee.GetDescription();
                    }
                    //其他杂费
                    using (var view = new Views.OrderPremiumsView())
                    {
                        var premium = view.Where(item => item.ID == this.FeeSourceID).FirstOrDefault();
                        if (premium.Type == Enums.OrderPremiumType.OtherFee)
                        {
                            return premium.Name;
                        }
                        else
                        {
                            return premium.Type.GetDescription();
                        }
                    }
                }
                else
                {
                    //货款、关税、增值税、代理费
                    return this.Type.GetDescription();
                }
            }
        }

        /// <summary>
        /// 费用类型
        /// </summary>
        public Enums.OrderFeeType Type { get; set; }

        /// <summary>
        /// 人民币金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 收款日期
        /// </summary>
        public DateTime ReceiptDate { get; set; }

        /// <summary>
        /// 是否已付款
        /// </summary>
        public bool Ispaid { get; set; }
    }

    #region 待收款中弹框使用

    /// <summary>
    /// 可收款的 OrderReceipt
    /// </summary>
    public class ReceivableOrderReceiptModel : OrderReceipt
    {
        /// <summary>
        /// 仍可收的金额
        /// </summary>
        public decimal ReceivableAmount { get; set; }

        ///// <summary>
        ///// 费用类型显示名称
        ///// </summary>
        //public string FeeTypeShowName
        //{
        //    get
        //    {
        //        if (this.FeeType == Enums.OrderFeeType.Incidental)
        //        {
        //            //商检费
        //            if (this.FeeSourceID == null)
        //            {
        //                return Enums.OrderPremiumType.InspectionFee.GetDescription();
        //            }
        //            //其他杂费
        //            using (var view = new Views.OrderPremiumsView())
        //            {
        //                var premium = view.Where(item => item.ID == this.FeeSourceID).FirstOrDefault();
        //                if (premium.Type == Enums.OrderPremiumType.OtherFee)
        //                {
        //                    return premium.Name;
        //                }
        //                else
        //                {
        //                    return premium.Type.GetDescription();
        //                }
        //            }
        //        }
        //        else
        //        {
        //            //货款、关税、增值税、代理费
        //            return this.FeeType.GetDescription();
        //        }
        //    }
        //}
    }

    #endregion
}
