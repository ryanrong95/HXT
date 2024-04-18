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
    /// 客户应付款统计
    /// </summary>
    public class ClientAccountPayable : IUnique
    {
        /// <summary>
        /// 主键ID/客户ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 会员协议
        /// </summary>
        internal ClientAgreement Agreement { get; set; }

        #region 货款

        /// <summary>
        /// 货款垫款上限
        /// </summary>
        public decimal ProductUpperLimit
        {
            get
            {
                return this.Agreement.ProductFeeClause.UpperLimit.GetValueOrDefault();
            }
        }

        /// <summary>
        /// 货款应付款
        /// </summary>
        public decimal ProductPayable { get; set; }

        /// <summary>
        /// 货款可用垫款
        /// </summary>
        public decimal ProductAvailable
        {
            get
            {
                return this.ProductUpperLimit - this.ProductPayable;
            }
        }

        #endregion

        #region 税款

        /// <summary>
        /// 税款垫款上限
        /// </summary>
        public decimal TaxUpperLimit
        {
            get
            {
                return this.Agreement.TaxFeeClause.UpperLimit.GetValueOrDefault();
            }
        }

        /// <summary>
        /// 税款应付款
        /// </summary>
        public decimal TaxPayable { get; set; }

        /// <summary>
        /// 税款可用垫款
        /// </summary>
        public decimal TaxAvailable
        {
            get
            {
                return this.TaxUpperLimit - this.TaxPayable;
            }
        }

        #endregion

        #region 代理费

        /// <summary>
        /// 代理费垫款上限
        /// </summary>
        public decimal AgencyUpperLimit
        {
            get
            {
                return this.Agreement.AgencyFeeClause.UpperLimit.GetValueOrDefault();
            }
        }

        /// <summary>
        /// 代理费应付款
        /// </summary>
        public decimal AgencyPayable { get; set; }

        /// <summary>
        /// 代理费可用垫款
        /// </summary>
        public decimal AgencyAvailable
        {
            get
            {
                return this.AgencyUpperLimit - this.AgencyPayable;
            }
        }

        #endregion

        #region 杂费

        /// <summary>
        /// 杂费垫款上限
        /// </summary>
        public decimal IncidentalUpperLimit
        {
            get
            {
                return this.Agreement.IncidentalFeeClause.UpperLimit.GetValueOrDefault();
            }
        }

        /// <summary>
        /// 杂费应付款
        /// </summary>
        public decimal IncidentalPayable { get; set; }

        /// <summary>
        /// 杂费可用垫款
        /// </summary>
        public decimal IncidentalAvailable
        {
            get
            {
                return this.IncidentalUpperLimit - this.IncidentalPayable;
            }
        }

        #endregion
    }

    /// <summary>
    /// 客户应付款详情
    /// </summary>
    public class ClientPayableDetail : IUnique
    {
        /// <summary>
        /// 主键ID/客户ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 应付货款明细
        /// </summary>
        IEnumerable<ClientFeeModel> ProductPayables
        {
            get
            {
                return this.Payables.Where(item => item.Type == Enums.OrderFeeType.Product).ToList();
            }
        }

        /// <summary>
        /// 应付税款明细
        /// </summary>
        IEnumerable<ClientFeeModel> TaxPayables
        {
            get
            {
                return this.Payables.Where(item => item.Type == Enums.OrderFeeType.Tariff || item.Type == Enums.OrderFeeType.AddedValueTax).ToList();
            }
        }

        /// <summary>
        /// 应付代理费明细
        /// </summary>
        IEnumerable<ClientFeeModel> AgencyPayables
        {
            get
            {
                return this.Payables.Where(item => item.Type == Enums.OrderFeeType.AgencyFee).ToList();
            }
        }

        /// <summary>
        /// 应付杂费明细
        /// </summary>
        IEnumerable<ClientFeeModel> IncidentalPayables
        {
            get
            {
                return this.Payables.Where(item => item.Type == Enums.OrderFeeType.Incidental).ToList();
            }
        }

        /// <summary>
        /// 应付款/欠款明细
        /// </summary>
        internal IEnumerable<ClientFeeModel> Payables { get; set; }
    }

    public class ClientFeeModel
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderID { get; set; }

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
        /// 费用类型：货款、关税、增值税、代理费、杂费
        /// </summary>
        public Enums.OrderFeeType Type { get; set; }

        /// <summary>
        /// 应付款/欠款金额
        /// </summary>
        public decimal Amount { get; set; }
    }
}
