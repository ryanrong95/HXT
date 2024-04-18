using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Linq;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.SpirePdf;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using Spire.Pdf.Grid;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PdfDocument = Needs.Utils.SpirePdf.PdfDocument;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 订单的对账单
    /// </summary>
    public sealed class OrderBill : IUnique
    {
        #region 属性

        /// <summary>
        /// 订单ID\订单编号
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 合同号
        /// </summary>
        private string contractNO;
        public string ContractNO
        {
            get
            {
                if (this.contractNO == null)
                {
                    using (var view = new Views.DecHeadsView())
                    {
                        this.ContractNO = view.FirstOrDefault(item => item.OrderID == this.ID)?.ContrNo;
                    }
                }
                return this.contractNO;
            }
            set
            {
                this.contractNO = value;
            }
        }

        public bool IsSuccess { get; set; }
        /// <summary>
        /// 报关日期
        /// </summary>
        public DateTime DDate { get; set; }

        /// <summary>
        /// 订单
        /// </summary>
        public Order Order { get; set; }

        /// <summary>
        /// 客户
        /// </summary>
        public Client Client { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        public Models.User User { get; set; }

        /// <summary>
        /// 受益人
        /// 这里是指报关公司
        /// </summary>
        public Beneficiary Beneficiary { get; set; }

        /// <summary>
        /// 订单对应合同协议
        /// </summary>
        public ClientAgreement Agreement { get; set; }

        /// <summary>
        /// 交易币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 海关汇率
        /// </summary>
        public decimal CustomsExchangeRate { get; set; }

        /// <summary>
        /// 实时汇率
        /// </summary>
        public decimal RealExchangeRate { get; set; }

        #region 修改汇率时，传递旧的信息 Begin

        /// <summary>
        /// 修改汇率时，传递旧的海关汇率
        /// </summary>
        public decimal OldCustomsExchangeRate { get; set; }

        /// <summary>
        /// 修改汇率时，传递旧的实时汇率
        /// </summary>
        public decimal OldRealExchangeRate { get; set; }

        /// <summary>
        /// 修改汇率时，传递旧的代理费类型
        /// </summary>
        public Enums.OrderBillType OldOrderBillType { get; set; }

        /// <summary>
        /// 修改汇率时，传递旧的代理费单价
        /// </summary>
        public decimal OldAgencyFeeUnitPrice { get; set; }

        #endregion

        /// <summary>
        /// 报关总货值（外币）
        /// </summary>
        private decimal declarePrice;
        public decimal DeclarePrice
        {
            get
            {
                if (this.declarePrice == 0)
                {
                    this.DeclarePrice = this.Items.Sum(item => item.TotalPrice);
                }
                return this.declarePrice;
            }
            set
            {
                this.declarePrice = value;
            }
        }

        /// <summary>
        /// 是否代垫货款
        /// </summary>
        public bool IsLoan { get; set; }

        /// <summary>
        /// 下单日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 订单类型
        /// </summary>
        public Enums.OrderType OrderType { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public Enums.OrderStatus OrderStatus { get; set; }

        /// <summary>
        /// 对账单文件
        /// </summary>
        public OrderFile File { get; set; }

        public OrderFileStatus MainOrderFileStatus { get; set; }

        /// <summary>
        /// 产品明细
        /// </summary>
        OrderItems items;
        public OrderItems Items
        {
            get
            {
                if (items == null)
                {
                    using (var view = new Views.OrderItemsView())
                    {
                        var query = view.Where(item => item.OrderID == this.ID && item.Status == Enums.Status.Normal);
                        this.Items = new OrderItems(query);
                    }
                }
                return this.items;
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                this.items = new OrderItems(value, new Action<OrderItem>(delegate (OrderItem item)
                {
                    item.OrderID = this.ID;
                }));
            }
        }

        #endregion

        Purchaser purchaser = PurchaserContext.Current;


        #region 费用

        /// <summary>
        /// 费用明细
        /// </summary>
        OrderPremiums premiums;
        public OrderPremiums Premiums
        {
            get
            {
                if (premiums == null)
                {
                    using (var view = new Views.OrderPremiumsView())
                    {
                        var query = view.Where(item => item.OrderID == this.ID && item.Status == Enums.Status.Normal);
                        this.Premiums = new OrderPremiums(query);
                    }
                }
                return this.premiums;
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                this.premiums = new OrderPremiums(value, new Action<OrderPremium>(delegate (OrderPremium item)
                {
                    item.OrderID = this.ID;
                }));
            }
        }

        /// <summary>
        /// 代理费合计
        /// </summary>
        public decimal AgencyFee
        {
            get
            {
                return this.Premiums.Where(item => item.Type == OrderPremiumType.AgencyFee)
                                    .Sum(item => item.Count * item.UnitPrice * item.Rate);
            }
        }

        /// <summary>
        /// 商检费
        /// </summary>
        public OrderPremiums InspFees
        {
            get
            {
                var view = this.Premiums.Where(item => item.Type == OrderPremiumType.InspectionFee);
                return new OrderPremiums(view);
            }
        }

        /// <summary>
        /// 杂费合计(不含商检费)
        /// </summary>
        public decimal OtherFee
        {
            get
            {
                return this.Premiums.Where(item => item.Type != OrderPremiumType.AgencyFee && item.Type != OrderPremiumType.InspectionFee)
                                    .Sum(item => item.Count * item.UnitPrice * item.Rate);
            }
        }

        /// <summary>
        /// 费用实收
        /// </summary>
        OrderReceiveds receiveds;
        public OrderReceiveds Receiveds
        {
            get
            {
                if (receiveds == null)
                {
                    using (var view = new Views.OrderReceivedsView())
                    {
                        var query = view.Where(item => item.OrderID == this.ID && item.Status == Enums.Status.Normal);
                        this.Receiveds = new OrderReceiveds(query);
                    }
                }
                return this.receiveds;
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                this.receiveds = new OrderReceiveds(value, new Action<OrderReceived>(delegate (OrderReceived item)
                {
                    item.OrderID = this.ID;
                }));
            }
        }

        #endregion

        #region 汇率

        /// <summary>
        /// 货款汇率
        /// </summary>
        private decimal productFeeExchangeRate;
        public decimal ProductFeeExchangeRate
        {
            get
            {
                var exchangeRateType = this.Agreement.ProductFeeClause.ExchangeRateType;
                switch (exchangeRateType)
                {
                    case Enums.ExchangeRateType.RealTime:
                        productFeeExchangeRate = this.RealExchangeRate;
                        break;
                    case Enums.ExchangeRateType.Custom:
                        productFeeExchangeRate = this.CustomsExchangeRate;
                        break;
                    case Enums.ExchangeRateType.Agreed:
                        productFeeExchangeRate = this.Agreement.ProductFeeClause.ExchangeRateValue.HasValue ? this.Agreement.ProductFeeClause.ExchangeRateValue.Value : 0;
                        break;
                    default:
                        productFeeExchangeRate = 0;
                        break;
                }

                return this.productFeeExchangeRate;
            }
        }

        /// <summary>
        /// 代理费汇率
        /// </summary>
        private decimal agencyFeeExchangeRate;
        public decimal AgencyFeeExchangeRate
        {
            get
            {
                var exchangeRateType = this.Agreement.AgencyFeeClause.ExchangeRateType;
                switch (exchangeRateType)
                {
                    case Enums.ExchangeRateType.RealTime:
                        agencyFeeExchangeRate = this.RealExchangeRate;
                        break;
                    case Enums.ExchangeRateType.Custom:
                        agencyFeeExchangeRate = this.CustomsExchangeRate;
                        break;
                    case Enums.ExchangeRateType.Agreed:
                        agencyFeeExchangeRate = this.Agreement.AgencyFeeClause.ExchangeRateValue.HasValue ? this.Agreement.AgencyFeeClause.ExchangeRateValue.Value : 0;
                        break;
                    default:
                        agencyFeeExchangeRate = 0;
                        break;
                }

                return this.agencyFeeExchangeRate;
            }
        }

        /// <summary>
        /// 杂费汇率
        /// </summary>
        private decimal incidentalFeeExchangeRate;
        public decimal IncidentalFeeExchangeRate
        {
            get
            {
                var exchangeRateType = this.Agreement.IncidentalFeeClause.ExchangeRateType;
                switch (exchangeRateType)
                {
                    case Enums.ExchangeRateType.RealTime:
                        incidentalFeeExchangeRate = this.RealExchangeRate;
                        break;
                    case Enums.ExchangeRateType.Custom:
                        incidentalFeeExchangeRate = this.CustomsExchangeRate;
                        break;
                    case Enums.ExchangeRateType.Agreed:
                        incidentalFeeExchangeRate = this.Agreement.IncidentalFeeClause.ExchangeRateValue.HasValue ? this.Agreement.IncidentalFeeClause.ExchangeRateValue.Value : 0;
                        break;
                    default:
                        incidentalFeeExchangeRate = 0;
                        break;
                }

                return this.incidentalFeeExchangeRate;
            }
        }

        #endregion

        #region 应付款日期

        /// <summary>
        /// 货款应付款日期
        /// </summary>
        private DateTime productFeeDueDate;
        public DateTime ProductFeeDueDate
        {
            get
            {
                var periodType = this.Agreement.ProductFeeClause.PeriodType;
                switch (periodType)
                {
                    case Enums.PeriodType.PrePaid:
                        productFeeDueDate = DateTime.Now;
                        break;
                    case Enums.PeriodType.AgreedPeriod:
                        var daysLimit = this.Agreement.ProductFeeClause.DaysLimit.Value;
                        productFeeDueDate = DateTime.Now.AddDays(daysLimit);
                        break;
                    case Enums.PeriodType.Monthly:
                        var monthlyDay = this.Agreement.ProductFeeClause.MonthlyDay.Value;
                        var days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.AddMonths(1).Month);
                        if (days < monthlyDay)
                        {
                            monthlyDay = days;
                        }
                        productFeeDueDate = new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(1).Month, monthlyDay);
                        break;
                    default:
                        productFeeDueDate = DateTime.Now;
                        break;
                }

                return this.productFeeDueDate;
            }
        }

        /// <summary>
        /// 税款应付款日期
        /// </summary>
        private DateTime taxFeeDueDate;
        public DateTime TaxFeeDueDate
        {
            get
            {
                var periodType = this.Agreement.TaxFeeClause.PeriodType;
                switch (periodType)
                {
                    case Enums.PeriodType.PrePaid:
                        taxFeeDueDate = DateTime.Now;
                        break;
                    case Enums.PeriodType.AgreedPeriod:
                        var daysLimit = this.Agreement.TaxFeeClause.DaysLimit.Value;
                        taxFeeDueDate = DateTime.Now.AddDays(daysLimit);
                        break;
                    case Enums.PeriodType.Monthly:
                        var monthlyDay = this.Agreement.TaxFeeClause.MonthlyDay.Value;
                        var days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.AddMonths(1).Month);
                        if (days < monthlyDay)
                        {
                            monthlyDay = days;
                        }
                        taxFeeDueDate = new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(1).Month, monthlyDay);
                        break;
                    default:
                        taxFeeDueDate = DateTime.Now;
                        break;
                }

                return this.taxFeeDueDate;
            }
        }

        /// <summary>
        /// 代理费应付款日期
        /// </summary>
        private DateTime agencyFeeDueDate;
        public DateTime AgencyFeeDueDate
        {
            get
            {
                var periodType = this.Agreement.AgencyFeeClause.PeriodType;
                switch (periodType)
                {
                    case Enums.PeriodType.PrePaid:
                        agencyFeeDueDate = DateTime.Now;
                        break;
                    case Enums.PeriodType.AgreedPeriod:
                        var daysLimit = this.Agreement.AgencyFeeClause.DaysLimit.Value;
                        agencyFeeDueDate = DateTime.Now.AddDays(daysLimit);
                        break;
                    case Enums.PeriodType.Monthly:
                        var monthlyDay = this.Agreement.AgencyFeeClause.MonthlyDay.Value;
                        var days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.AddMonths(1).Month);
                        if (days < monthlyDay)
                        {
                            monthlyDay = days;
                        }
                        agencyFeeDueDate = new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(1).Month, monthlyDay);
                        break;
                    default:
                        agencyFeeDueDate = DateTime.Now;
                        break;
                }

                return this.agencyFeeDueDate;
            }
        }

        /// <summary>
        /// 杂费应付款日期
        /// </summary>
        private DateTime incidentalFeeDueDate;
        public DateTime IncidentalFeeDueDate
        {
            get
            {
                var periodType = this.Agreement.IncidentalFeeClause.PeriodType;
                switch (periodType)
                {
                    case Enums.PeriodType.PrePaid:
                        incidentalFeeDueDate = DateTime.Now;
                        break;
                    case Enums.PeriodType.AgreedPeriod:
                        var daysLimit = this.Agreement.IncidentalFeeClause.DaysLimit.Value;
                        incidentalFeeDueDate = DateTime.Now.AddDays(daysLimit);
                        break;
                    case Enums.PeriodType.Monthly:
                        var monthlyDay = this.Agreement.IncidentalFeeClause.MonthlyDay.Value;
                        var days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.AddMonths(1).Month);
                        if (days < monthlyDay)
                        {
                            monthlyDay = days;
                        }
                        incidentalFeeDueDate = new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(1).Month, monthlyDay);
                        break;
                    default:
                        incidentalFeeDueDate = DateTime.Now;
                        break;
                }

                return this.incidentalFeeDueDate;
            }
        }

        #region 1.已报关：根据报关日期 与 协议(税费、代理费、杂费) 计算  2. 未报关：根据下单日期 与 协议(税费、代理费、杂费) 计算

        /// <summary>
        /// 税款应付款日期
        /// </summary>
        private DateTime taxFeeDueDate1;
        public DateTime TaxFeeDueDate1
        {
            get
            {
                var periodType = this.Agreement.TaxFeeClause.PeriodType;

                if (this.IsSuccess)
                {
                    switch (periodType)
                    {
                        case Enums.PeriodType.PrePaid:
                            taxFeeDueDate = this.DDate;
                            break;
                        case Enums.PeriodType.AgreedPeriod:
                            var daysLimit = this.Agreement.TaxFeeClause.DaysLimit.Value;
                            taxFeeDueDate = this.DDate.AddDays(daysLimit);
                            break;
                        case Enums.PeriodType.Monthly:
                            var monthlyDay = this.Agreement.TaxFeeClause.MonthlyDay.Value;
                            //当前月的天数
                            var days = DateTime.DaysInMonth(this.DDate.AddMonths(1).Year, this.DDate.AddMonths(1).Month);
                            if (days < monthlyDay)
                            {
                                monthlyDay = days;
                            }
                            taxFeeDueDate = new DateTime(this.DDate.AddMonths(1).Year, this.DDate.AddMonths(1).Month, monthlyDay);
                            break;
                        default:
                            taxFeeDueDate = this.DDate;
                            break;
                    }
                }
                else
                {
                    switch (periodType)
                    {
                        case Enums.PeriodType.PrePaid:
                            taxFeeDueDate = this.CreateDate;
                            break;
                        case Enums.PeriodType.AgreedPeriod:
                            var daysLimit = this.Agreement.TaxFeeClause.DaysLimit.Value;
                            taxFeeDueDate = this.CreateDate.AddDays(daysLimit);
                            break;
                        case Enums.PeriodType.Monthly:
                            var monthlyDay = this.Agreement.TaxFeeClause.MonthlyDay.Value;
                            var days = DateTime.DaysInMonth(this.CreateDate.AddMonths(1).Year, this.CreateDate.AddMonths(1).Month);
                            if (days < monthlyDay)
                            {
                                monthlyDay = days;
                            }
                            taxFeeDueDate = new DateTime(this.CreateDate.AddMonths(1).Year, this.CreateDate.AddMonths(1).Month, monthlyDay);
                            break;
                        default:
                            taxFeeDueDate = this.CreateDate;
                            break;
                    }
                }

                return this.taxFeeDueDate;
            }
        }

        /// <summary>
        /// 代理费应付款日期
        /// </summary>
        private DateTime agencyFeeDueDate1;
        public DateTime AgencyFeeDueDate1
        {
            get
            {
                var periodType = this.Agreement.AgencyFeeClause.PeriodType;
                if (this.IsSuccess)
                {
                    switch (periodType)
                    {
                        case Enums.PeriodType.PrePaid:
                            agencyFeeDueDate = this.DDate;
                            break;
                        case Enums.PeriodType.AgreedPeriod:
                            var daysLimit = this.Agreement.AgencyFeeClause.DaysLimit.Value;
                            agencyFeeDueDate = this.DDate.AddDays(daysLimit);
                            break;
                        case Enums.PeriodType.Monthly:
                            var monthlyDay = this.Agreement.AgencyFeeClause.MonthlyDay.Value;
                            var days = DateTime.DaysInMonth(this.DDate.AddMonths(1).Year, this.DDate.AddMonths(1).Month);
                            if (days < monthlyDay)
                            {
                                monthlyDay = days;
                            }
                            agencyFeeDueDate = new DateTime(this.DDate.AddMonths(1).Year, this.DDate.AddMonths(1).Month, monthlyDay);
                            break;
                        default:
                            agencyFeeDueDate = this.DDate;
                            break;
                    }
                }
                else
                {
                    switch (periodType)
                    {
                        case Enums.PeriodType.PrePaid:
                            agencyFeeDueDate = this.CreateDate;
                            break;
                        case Enums.PeriodType.AgreedPeriod:
                            var daysLimit = this.Agreement.AgencyFeeClause.DaysLimit.Value;
                            agencyFeeDueDate = this.CreateDate.AddDays(daysLimit);
                            break;
                        case Enums.PeriodType.Monthly:
                            var monthlyDay = this.Agreement.AgencyFeeClause.MonthlyDay.Value;
                            var days = DateTime.DaysInMonth(this.CreateDate.AddMonths(1).Year, this.CreateDate.AddMonths(1).Month);
                            if (days < monthlyDay)
                            {
                                monthlyDay = days;
                            }
                            agencyFeeDueDate = new DateTime(this.CreateDate.AddMonths(1).Year, this.CreateDate.AddMonths(1).Month, monthlyDay);
                            break;
                        default:
                            agencyFeeDueDate = this.CreateDate;
                            break;
                    }
                }
                return this.agencyFeeDueDate;
            }
        }

        /// <summary>
        /// 杂费应付款日期
        /// </summary>
        private DateTime incidentalFeeDueDate1;
        public DateTime IncidentalFeeDueDate1
        {
            get
            {
                var periodType = this.Agreement.IncidentalFeeClause.PeriodType;
                if (this.IsSuccess)
                {
                    switch (periodType)
                    {
                        case Enums.PeriodType.PrePaid:
                            incidentalFeeDueDate = this.DDate;
                            break;
                        case Enums.PeriodType.AgreedPeriod:
                            var daysLimit = this.Agreement.IncidentalFeeClause.DaysLimit.Value;
                            incidentalFeeDueDate = this.DDate.AddDays(daysLimit);
                            break;
                        case Enums.PeriodType.Monthly:
                            var monthlyDay = this.Agreement.IncidentalFeeClause.MonthlyDay.Value;
                            var days = DateTime.DaysInMonth(this.DDate.AddMonths(1).Year, this.DDate.AddMonths(1).Month);
                            if (days < monthlyDay)
                            {
                                monthlyDay = days;
                            }
                            incidentalFeeDueDate = new DateTime(this.DDate.AddMonths(1).Year, this.DDate.AddMonths(1).Month, monthlyDay);
                            break;
                        default:
                            incidentalFeeDueDate = this.DDate;
                            break;
                    }
                }
                else
                {
                    switch (periodType)
                    {
                        case Enums.PeriodType.PrePaid:
                            incidentalFeeDueDate = this.CreateDate;
                            break;
                        case Enums.PeriodType.AgreedPeriod:
                            var daysLimit = this.Agreement.IncidentalFeeClause.DaysLimit.Value;
                            incidentalFeeDueDate = this.CreateDate.AddDays(daysLimit);
                            break;
                        case Enums.PeriodType.Monthly:
                            var monthlyDay = this.Agreement.IncidentalFeeClause.MonthlyDay.Value;
                            var days = DateTime.DaysInMonth(this.CreateDate.AddMonths(1).Year, this.CreateDate.AddMonths(1).Month);
                            if (days < monthlyDay)
                            {
                                monthlyDay = days;
                            }
                            incidentalFeeDueDate = new DateTime(this.CreateDate.AddMonths(1).Year, this.CreateDate.AddMonths(1).Month, monthlyDay);
                            break;
                        default:
                            incidentalFeeDueDate = this.CreateDate;
                            break;
                    }
                }
                return this.incidentalFeeDueDate;
            }
        }
        #endregion
        #endregion

        /// <summary>
        /// 当对账单汇率变更时发生
        /// </summary>
        public event ExchangeRateChangedHanlder ExchangeRateChanged;

        /// <summary>
        /// 当对账单海关汇率变更时发生
        /// </summary>
        public event CustomsExchangeRateChangedHanlder CustomsExchangeRateChanged;

        /// <summary>
        /// 当对账单实时汇率变更时发生
        /// </summary>
        public event RealTimeExchangeRateChangedHanlder RealTimeExchangeRateChanged;

        /// <summary>
        /// 当跟单员审核对账单通过时发生
        /// </summary>
        public event OrderFileAuditedHanlder Approved;
        /// <summary>
        /// 主订单的对账单汇率发送改变时发生
        /// </summary>
        public event MainExchangeRateChangedHanlder MainExchangeRateChanged;
        public OrderBill()
        {
            this.ExchangeRateChanged += OrderBill_ExchangeRateChanged;
            this.CustomsExchangeRateChanged += OrderBill_CustomsExchangeRateChanged;
            this.RealTimeExchangeRateChanged += OrderBill_RealTimeExchangeRateChanged;
            this.Approved += OrderBill_Approved;
            this.MainExchangeRateChanged += MainOrderBill_ExchangeRateChanged;
        }

        private void OrderBill_ExchangeRateChanged(object sender, ExchangeRateChangedEventArgs e)
        {
            var order = e.Bill.Order;
            order.CustomsExchangeRate = e.Bill.CustomsExchangeRate;
            order.RealExchangeRate = e.Bill.RealExchangeRate;
            order.GenerateBill(order.OrderBillType, order.PointedAgencyFee);

            if (order.Admin != null)
            {
                order.Log(order.Admin, "跟单员[" + order.Admin.RealName + "]变更了对账单汇率");
            }

            //
            if (order.Type != OrderType.Outside)
            {
                #region 自动上传对账单

                //保存文件
                string fileName = DateTime.Now.Ticks + ".pdf";
                Utils.FileDirectory file = new Utils.FileDirectory(fileName);
                file.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                file.CreateDataDirectory();
                e.Bill.SaveAs(file.FilePath);

                //先删除之前上传的对账单
                var origFiles = order.Files.Where(f => f.FileType == FileType.OrderBill && f.Status == Status.Normal);
                foreach (var origFile in origFiles)
                {
                    Needs.Ccs.Services.Models.MainOrderFile orderBill = new Needs.Ccs.Services.Models.MainOrderFile();
                    orderBill.ID = origFile.ID;
                    orderBill.Abandon();
                    new CenterFilesTopView().Modify(new { Status = FileDescriptionStatus.Delete }, origFile.ID);
                }

                var ermAdmin = new Needs.Ccs.Services.Views.AdminsTopView2().FirstOrDefault(x => x.OriginID == order.AdminID);
                var ErmAdminID = ermAdmin?.ID ?? "";
                var dic = new { CustomName = fileName, WsOrderID = order.MainOrderID, AdminID = ErmAdminID };

                var centerType = Needs.Ccs.Services.Models.ApiModels.Files.FileType.OrderBill;
                //本地文件上传到服务器
                var tempPath = file.FilePath;
                var result = Needs.Ccs.Services.Models.CenterFilesTopView.Upload(tempPath, centerType, dic);
                string[] ID = { result[0].FileID };
                new CenterFilesTopView().Modify(new { Status = FileDescriptionStatus.Approved }, ID);

                #endregion
            }
        }

        private void OrderBill_RealTimeExchangeRateChanged(object sender, RealTimeExchangeRateChangedEventArgs e)
        {
            var order = e.Bill.Order;
            order.RealExchangeRate = e.Bill.RealExchangeRate;
            order.GenerateBill(order.OrderBillType, order.PointedAgencyFee);

            if (order.Admin != null)
            {
                order.Log(order.Admin, "跟单员[" + order.Admin.RealName + "]修改了对账单的实时汇率");
            }
        }

        private void OrderBill_CustomsExchangeRateChanged(object sender, CustomsExchangeRateChangedEventArgs e)
        {
            var order = e.Bill.Order;
            order.CustomsExchangeRate = e.Bill.CustomsExchangeRate;
            order.GenerateBill(order.OrderBillType, order.PointedAgencyFee);

            if (order.Admin != null)
            {
                order.Log(order.Admin, "跟单员[" + order.Admin.RealName + "]修改了对账单的海关汇率");
            }
        }

        private void OrderBill_Approved(object sender, OrderFileAuditedEventArgs e)
        {
            var order = e.Bill.Order;
            if (order.Admin != null)
            {
                order.Log(order.Admin, "跟单员[" + order.Admin.RealName + "]审核通过了客户上传的对账单");
            }
        }

        /// <summary>
        /// 修改对账单的汇率
        /// </summary>
        public void ChangeExchangeRate()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new
                {
                    CustomsExchangeRate = this.CustomsExchangeRate,
                    RealExchangeRate = this.RealExchangeRate
                }, item => item.ID == this.ID);
            }

            this.OnExchangeRateChanged();
        }

        void OnExchangeRateChanged()
        {
            if (this.ExchangeRateChanged != null)
            {
                this.ExchangeRateChanged(this, new ExchangeRateChangedEventArgs(this));
            }
        }

        /// <summary>
        /// 修改对账单的海关汇率
        /// </summary>
        public void ChangeCustomsExchangeRate()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new { CustomsExchangeRate = this.CustomsExchangeRate }, item => item.ID == this.ID);
            }

            this.OnCustomsExchangeRateChanged();
        }

        void OnCustomsExchangeRateChanged()
        {
            if (this.CustomsExchangeRateChanged != null)
            {
                this.CustomsExchangeRateChanged(this, new CustomsExchangeRateChangedEventArgs(this));
            }
        }

        /// <summary>
        /// 修改对账单的实时汇率
        /// </summary>
        public void ChangeRealTimeExchangeRate()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new { RealExchangeRate = this.RealExchangeRate }, item => item.ID == this.ID);
            }

            this.OnRealTimeExchangeRateChanged();
        }

        void OnRealTimeExchangeRateChanged()
        {
            if (this.RealTimeExchangeRateChanged != null)
            {
                this.RealTimeExchangeRateChanged(this, new RealTimeExchangeRateChangedEventArgs(this));
            }
        }

        /// <summary>
        /// 审核通过
        /// </summary>
        public void Approve()
        {
            this.File.FileStatus = Enums.OrderFileStatus.Audited;
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderFiles>(new { FileStatus = Enums.OrderFileStatus.Audited }, item => item.ID == this.File.ID);
            }

            this.OnApproved();
        }

        void OnApproved()
        {
            if (this.Approved != null)
            {
                this.Approved(this, new OrderFileAuditedEventArgs(this));
            }
        }

        /// <summary>
        /// 导出PDF
        /// </summary>
        public PdfDocument ToPdf()
        {

            #region pdf对象声明

            //创建一个PdfDocument类对象
            PdfDocument pdf = new PdfDocument();
            pdf.PageSettings.Margins = new PdfMargins(10, 60, 10, 10);

            //添加一页到PDF文档
            PdfPageBase page = pdf.Pages.Add(PdfPageSize.A4);
            int pageCount = pdf.Pages.Count;

            //画笔
            PdfBrush brush = PdfBrushes.Black;
            //字体
            PdfTrueTypeFont font1 = new PdfTrueTypeFont(new Font("SimSun", 14f, FontStyle.Bold), true);
            PdfTrueTypeFont font2 = new PdfTrueTypeFont(new Font("SimSun", 7f, FontStyle.Regular), true);
            //字体对齐方式
            PdfStringFormat formatCenter = new PdfStringFormat(PdfTextAlignment.Center);
            PdfStringFormat formatRight = new PdfStringFormat(PdfTextAlignment.Right);
            PdfStringFormat formatLeft = new PdfStringFormat(PdfTextAlignment.Left);

            #endregion

            #region 头
            float x = 0, y = 5f;

            float width = page.Canvas.ClientSize.Width;
            string message = "委托进口货物报关对帐单";
            page.Canvas.DrawString(message, font1, brush, width / 2, y, formatCenter);

            y += font1.MeasureString(message, formatCenter).Height + 8;

            //创建一个PdfGrid对象
            PdfGrid grid = new PdfGrid();
            grid.Style.Font = font2;

            //设置列宽
            grid.Columns.Add(2);
            grid.Columns[0].Width = width / 2;
            grid.Columns[1].Width = width / 2;

            //表头信息
            PdfGridRow row = grid.Rows.Add();
            row.Cells[0].Value = "委托方: " + this.Client.Company.Name;
            row.Cells[1].Value = "被委托方: " + purchaser.CompanyName;
            row = grid.Rows.Add();
            row.Cells[0].Value = "电话：" + this.Client.Company.Contact.Tel;
            row.Cells[1].Value = "电话：" + purchaser.Tel;
            row = grid.Rows.Add();
            row.Cells[0].Value = "订单编号：" + this.ID + (this.ContractNO == null ? "" : (" 合同号：" + this.ContractNO));
            row.Cells[1].Value = "实时汇率：" + this.RealExchangeRate + " 海关汇率：" + this.CustomsExchangeRate;

            //设置边框
            SetCellBorder(grid);

            PdfLayoutResult result = grid.Draw(page, new PointF(x, y));
            y += result.Bounds.Height + 5;

            #endregion

            #region 报关商品明细

            //创建一个PdfGrid对象
            grid = new PdfGrid();
            grid.Style.Font = font2;

            //设置列宽
            grid.Columns.Add(14);
            grid.Columns[0].Width = width * 3f / 100;
            grid.Columns[1].Width = width * 13f / 100;
            grid.Columns[2].Width = width * 13f / 100;
            grid.Columns[3].Width = width * 6f / 100;
            grid.Columns[4].Width = width * 7f / 100;
            grid.Columns[5].Width = width * 7f / 100;
            grid.Columns[6].Width = width * 7f / 100;
            grid.Columns[7].Width = width * 7f / 100;
            grid.Columns[8].Width = width * 5f / 100;
            grid.Columns[9].Width = width * 6f / 100;
            grid.Columns[10].Width = width * 6f / 100;
            grid.Columns[11].Width = width * 5f / 100;
            grid.Columns[12].Width = width * 7f / 100;
            grid.Columns[13].Width = width * 8f / 100;

            //产品信息
            row = grid.Rows.Add();
            row.Cells[0].Value = "序号";
            row.Cells[1].Value = "报关品名";
            row.Cells[2].Value = "规格型号";
            row.Cells[3].Value = "数量";
            row.Cells[4].Value = "报关单价" + "(" + this.Currency + ")";
            row.Cells[5].Value = "报关总价" + "(" + this.Currency + ")";
            row.Cells[6].Value = "关税率";
            row.Cells[7].Value = "报关货值(CNY)";
            row.Cells[8].Value = "关税(CNY)";
            row.Cells[9].Value = "增值税(CNY)";
            row.Cells[10].Value = "代理费(CNY)";
            row.Cells[11].Value = "杂费(CNY)";
            row.Cells[12].Value = "税费合计(CNY)";
            row.Cells[13].Value = "报关总金额(CNY)";

            //税点
            var taxpoint = 1 + this.Agreement.InvoiceTaxRate;
            //代理费率、最低代理费
            decimal agencyRate = this.AgencyFeeExchangeRate * this.Agreement.AgencyRate;
            decimal minAgencyFee = this.Agreement.MinAgencyFee;
            bool isAverage = this.DeclarePrice * agencyRate < minAgencyFee ? true : false;
            //合计
            int sn = 0;
            decimal totalQty = this.Items.Sum(item => item.Quantity);
            decimal totalPrice = this.Items.Sum(item => item.TotalPrice);
            decimal totalCNYPrice = totalPrice * this.ProductFeeExchangeRate;
            decimal totalAgencyFee = this.AgencyFee * taxpoint;
            decimal totalInspFee = this.InspFees.Sum(item => item.Count * item.UnitPrice * item.Rate) * taxpoint;
            decimal totalOtherFee = this.OtherFee * taxpoint;
            decimal totalTraiff = 0, totalAddedValueTax = 0;
            //平摊代理费、杂费
            decimal aveAgencyFee = totalAgencyFee / this.Items.Count();
            decimal aveOtherFee = totalOtherFee / this.Items.Count();

            foreach (var item in this.Items)
            {
                //产品项的关税、增值税、商检费
                decimal itemTariff = item.ImportTax.Value.Value;
                decimal itemAddedValueTax = item.AddedValueTax.Value.Value;
                decimal itemInspFee = this.InspFees.Where(fee => fee.OrderItemID == item.ID)
                                                   .Select(fee => fee.Count * fee.UnitPrice * fee.Rate).FirstOrDefault() * taxpoint;
                decimal itemAgencyFee = isAverage ? aveAgencyFee : item.TotalPrice * agencyRate * taxpoint;

                sn++;
                row = grid.Rows.Add();
                row.Cells[0].Value = sn.ToString();
                row.Cells[1].Value = item.Category.Name;
                row.Cells[2].Value = item.Model;
                row.Cells[3].Value = item.Quantity.ToString("0.####");
                row.Cells[4].Value = item.UnitPrice.ToString("0.0000");
                row.Cells[5].Value = item.TotalPrice.ToRound(2).ToString("0.00");
                row.Cells[6].Value = item.ImportTax.Rate.ToString("0.0000");
                row.Cells[7].Value = (item.TotalPrice * this.ProductFeeExchangeRate).ToRound(2).ToString("0.00");
                row.Cells[8].Value = itemTariff.ToRound(2).ToString("0.00");
                totalTraiff += itemTariff;
                row.Cells[9].Value = itemAddedValueTax.ToRound(2).ToString("0.00");
                totalAddedValueTax += itemAddedValueTax;
                row.Cells[10].Value = itemAgencyFee.ToRound(2).ToString("0.00");
                row.Cells[11].Value = (itemInspFee + aveOtherFee).ToRound(2).ToString("0.00");
                row.Cells[12].Value = (itemTariff + itemAddedValueTax + itemAgencyFee + itemInspFee + aveOtherFee).ToRound(2).ToString("0.00");
                row.Cells[13].Value = (item.TotalPrice * this.ProductFeeExchangeRate + itemTariff + itemAddedValueTax + itemAgencyFee + itemInspFee + aveOtherFee).ToRound(2).ToString("0.00");
            }

            //内单和Icgoo的对账单如果关税总和小于50，则显示0
            //ryan 20210113 外单税费小于50不收 钟苑平
            //if (this.OrderType != OrderType.Outside && totalTraiff < 50)
            if (totalTraiff < 50)
            {
                totalTraiff = 0;
            }
            //内单和Icgoo的对账单如果增值税总和小于50，则显示0
            if (totalAddedValueTax < 50)
            {
                totalAddedValueTax = 0;
            }

            //合计行
            row = grid.Rows.Add();
            row.Cells[0].ColumnSpan = 3;
            row.Cells[0].Value = "合计:";
            row.Cells[3].Value = totalQty.ToString("0.####");
            row.Cells[5].Value = totalPrice.ToRound(2).ToString("0.00");
            row.Cells[7].Value = totalCNYPrice.ToRound(2).ToString("0.00");
            row.Cells[8].Value = totalTraiff.ToRound(2).ToString("0.00");
            row.Cells[9].Value = totalAddedValueTax.ToRound(2).ToString("0.00");
            row.Cells[10].Value = totalAgencyFee.ToRound(2).ToString("0.00");
            row.Cells[11].Value = (totalInspFee + totalOtherFee).ToRound(2).ToString("0.00");
            row.Cells[12].Value = (totalTraiff + totalAddedValueTax + totalAgencyFee + totalInspFee + totalOtherFee).ToRound(2).ToString("0.00");
            row.Cells[13].Value = (totalCNYPrice + totalTraiff + totalAddedValueTax + totalAgencyFee + totalInspFee + totalOtherFee).ToRound(2).ToString("0.00");

            foreach (var pgr in grid.Rows)
            {
                for (int i = 0; i < pgr.Cells.Count; i++)
                {
                    if (i == 1 || i == 2)
                    {
                        pgr.Cells[i].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
                        continue;
                    }
                    pgr.Cells[i].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                }
            }

            //设置边框
            SetCellBorder(grid);

            result = grid.Draw(page, new PointF(x, y));
            if (pdf.Pages.Count > pageCount)
                UpdateIfNewPageCreated(pdf, out pageCount, out page, out x, out y);
            y += result.Bounds.Height + 5;

            #endregion

            #region 费用合计明细

            //创建一个PdfGrid对象
            grid = new PdfGrid();
            grid.Style.Font = font2;

            //设置列宽
            grid.Columns.Add(2);
            grid.Columns[0].Width = width * 0.8f;
            grid.Columns[1].Width = width * 0.2f;

            row = grid.Rows.Add();
            row.Cells[0].Value = "货值小计：";
            row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Right, PdfVerticalAlignment.Middle);
            row.Cells[1].Value = this.Currency + " " + totalPrice.ToRound(2).ToString("0.00") + "\r\nCNY " + totalCNYPrice.ToRound(2).ToString("0.00");
            row = grid.Rows.Add();
            row.Cells[0].Value = "税代费小计：";
            row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Right, PdfVerticalAlignment.Middle);
            var totalTaxFee = totalTraiff + totalAddedValueTax + totalAgencyFee + totalInspFee + totalOtherFee;
            row.Cells[1].Value = "CNY " + totalTaxFee.ToRound(2).ToString("0.00");
            row = grid.Rows.Add();
            row.Cells[0].Value = "应收总金额合计：";
            row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Right, PdfVerticalAlignment.Middle);
            var totalAmount = totalCNYPrice + totalTraiff + totalAddedValueTax + totalAgencyFee + totalInspFee + totalOtherFee;
            row.Cells[1].Value = "CNY " + totalAmount.ToRound(2).ToString("0.00");

            //设置边框
            SetCellBorder(grid);

            result = grid.Draw(page, new PointF(x, y));
            if (pdf.Pages.Count > pageCount)
                UpdateIfNewPageCreated(pdf, out pageCount, out page, out x, out y);
            y += result.Bounds.Height + 5;

            #endregion

            #region 尾

            //创建一个PdfGrid对象
            grid = new PdfGrid();
            grid.Style.Font = font2;

            //设置列宽
            grid.Columns.Add(1);
            grid.Columns[0].Width = width;

            row = grid.Rows.Add();
            row.Cells[0].Value = purchaser.CompanyName + " " + purchaser.Address + " 电话：" + purchaser.Tel + " 传真：" + purchaser.UseOrgPersonTel;
            row = grid.Rows.Add();
            row.Cells[0].Value = $"开户行:{purchaser.BankName} 开户名：{purchaser.AccountName} 账户：{purchaser.AccountId}";

            //设置边框
            SetCellBorder(grid);

            result = grid.Draw(page, new PointF(x, y));
            if (pdf.Pages.Count > pageCount)
                UpdateIfNewPageCreated(pdf, out pageCount, out page, out x, out y);
            y += result.Bounds.Height + 5;

            //创建一个PdfGrid对象
            grid = new PdfGrid();
            grid.Style.Font = font2;

            //设置列宽
            grid.Columns.Add(2);
            grid.Columns[0].Width = width / 2;
            grid.Columns[1].Width = width / 2;

            //应付款结算日期
            var dueDate = GetDueDate();

            row = grid.Rows.Add();
            row.Cells[0].ColumnSpan = 2;
            row.Cells[0].Value = "备注:\r\n" +
                                 //"1.我司" + purchaser.CompanyName + "为委托方代垫" +
                                 //"本金(" + (this.IsLoan ? totalCNYPrice.ToRound(2).ToString("0.00") : "0.00") + "元)" +
                                 //"+关税(" + totalTraiff.ToRound(2).ToString("0.00") + "元)" +
                                 //"+增值税(" + totalAddedValueTax.ToRound(2).ToString("0.00") + "元)" +
                                 //"+代理费(" + totalAgencyFee.ToRound(2).ToString("0.00") + "元)" +
                                 //"+杂费(" + (totalInspFee + totalOtherFee).ToRound(2).ToString("0.00") + "元)," +
                                 //"共计应收人民币(" + (this.IsLoan ? totalAmount.ToRound(2).ToString("0.00") : totalTaxFee.ToRound(2).ToString("0.00")) + "元)，" +
                                 //"委托方需在(" + dueDate.ToString("yyyy年MM月dd日") + ")前与我方结清所有欠款，逾期未结款的，按日加收万分之五的滞纳金。\r\n" +
                                 //"2.客户在90天内完成付汇手续，付汇汇率为实际付汇当天的中国银行上午十点后的第一个现汇卖出价。\r\n" +
                                 "1.委托方需在报关协议约定的付款日前与我方结清所有欠款，逾期未结款的，按日加收万分之五的滞纳金。\r\n" +
                                "2.委托方在90天内完成付汇，付汇汇率为报关协议约定的实际付汇当天的汇率。\r\n" +
                                "3.委托方应在收到帐单之日起二个工作日内签字和盖章确认回传。\r\n" +
                                "4.此传真件、扫描件、复印件与原件具有同等法律效力。\r\n" +
                                "5.如若对此帐单有发生争议的，双方应友好协商解决，如协商不成的，可通过被委托方所在地人民法院提起诉讼解决。";
            row = grid.Rows.Add();
            row.Height = 30f;
            row.Cells[0].Value = "委托方确认签字或盖章：";
            row.Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
            row.Cells[1].Value = "被委托方签字或盖章：";
            row.Cells[1].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);

            //设置边框
            SetCellBorder(grid);

            result = grid.Draw(page, new PointF(x, y));
            if (pdf.Pages.Count > pageCount)
                UpdateIfNewPageCreated(pdf, out pageCount, out page, out x, out y);
            y += result.Bounds.Height + 5;

            //大赢家加上委托方章
            if (this.Order.Client.ClientType == Enums.ClientType.Internal)
            {
                PdfImage imageInternal = PdfImage.FromFile(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content\\images\\", this.Order.Client.Company.Name + ".png"));
                page.Canvas.DrawImage(imageInternal, 100, y - 80);
            }

            PdfImage image = PdfImage.FromFile(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, purchaser.SealUrl));
            page.Canvas.DrawImage(image, 350, y - 80);

            #endregion

            #region 公共组件

            //页眉、页脚、二维码、水印
            pdf.PdfMargins = new PdfMargins(10, 10);
            PdfDocumentHandle pdfDocumentHandle = new PdfDocumentHandle(pdf);
            string imageUrl = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PurchaserContext.Current.HeaderImg);

            pdfDocumentHandle.HeaderFooter.GenerateHeader(imageUrl, PurchaserContext.Current.OfficalWebsite);
            pdfDocumentHandle.HeaderFooter.GenerateFooter(PurchaserContext.Current.CompanyName);
            pdfDocumentHandle.Barcode.GenerateQRCode(this.ID, imageUrl);
            pdfDocumentHandle.Watermark.DrawWatermark(PurchaserContext.Current.CompanyName);

            #endregion

            return pdf;
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public void SaveAs(string filePath)
        {
            PdfDocument pdf = this.ToPdf();
            pdf.SaveToFile(filePath);
            pdf.Close();
        }

        /// <summary>
        /// 保存内存流
        /// </summary>
        public MemoryStream SaveAs()
        {
            var pdf = this.ToPdf();
            MemoryStream mStream = new MemoryStream();
            pdf.SaveToStream(mStream);
            pdf.Close();
            return mStream;
        }

        #region 使用ItextSharp生成PDF，可超过10页

        //public Document ToPdfIText()
        //{
        //    //中文字体
        //    BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
        //    Rectangle rec = new iTextSharp.text.Rectangle(PageSize.A4);
        //    //创建一个文档实例。 去除边距
        //    Document document = new Document(rec);
        //}


        #endregion

        /// <summary>
        /// 对账单应付款结算日期
        /// </summary>
        /// <returns></returns>
        public DateTime GetDueDate()
        {
            var dueDate = this.ProductFeeDueDate > this.TaxFeeDueDate ? this.TaxFeeDueDate : this.ProductFeeDueDate;
            dueDate = dueDate > this.AgencyFeeDueDate ? this.AgencyFeeDueDate : dueDate;
            dueDate = dueDate > this.IncidentalFeeDueDate ? this.IncidentalFeeDueDate : dueDate;
            return dueDate;
        }
        /// <summary>
        /// 对账单应付款结算日期
        /// </summary>
        /// <returns></returns>
        public DateTime GetDueDate1()
        {
            DateTime dueDate;
            //if (this.IsSuccess) //1 已报关
            //{
            //    dueDate = this.TaxFeeDueDate1;//this.DDate > this.TaxFeeDueDate ? this.TaxFeeDueDate : this.DDate;
            //    dueDate = dueDate > this.AgencyFeeDueDate1 ? this.AgencyFeeDueDate1 : dueDate;
            //    dueDate = dueDate > this.IncidentalFeeDueDate1 ? this.IncidentalFeeDueDate1 : dueDate;
            //}
            //else
            //{
            dueDate = this.TaxFeeDueDate1;//this.CreateDate > this.TaxFeeDueDate ? this.TaxFeeDueDate : this.CreateDate;
            dueDate = dueDate > this.AgencyFeeDueDate1 ? this.AgencyFeeDueDate1 : dueDate;
            dueDate = dueDate > this.IncidentalFeeDueDate1 ? this.IncidentalFeeDueDate1 : dueDate;
            // }

            return dueDate;
        }
        /// <summary>
        /// 获取费用明细
        /// </summary>
        /// <returns></returns>
        [Obsolete("废弃，该方法保留以防后续业务变更")]
        public List<dynamic> GetFeeDetails()
        {
            //税点
            var taxpoint = 1 + this.Agreement.InvoiceTaxRate;

            #region 应收、实收
            //费用明细
            var agencyFees = this.Premiums.Where(item => item.Type == OrderPremiumType.AgencyFee);
            var inspFees = this.Premiums.Where(item => item.Type == OrderPremiumType.InspectionFee);
            var incidentalFees = this.Premiums.Where(item => item.Type != OrderPremiumType.AgencyFee &&
                                                             item.Type != OrderPremiumType.InspectionFee);
            //实收
            var productReceiveds = this.Receiveds.Where(item => item.FeeType == OrderFeeType.Product);
            var tariffReceiveds = this.Receiveds.Where(item => item.FeeType == OrderFeeType.Tariff);
            var addedValueTaxReceiveds = this.Receiveds.Where(item => item.FeeType == OrderFeeType.AddedValueTax);
            var agencyReceiveds = this.Receiveds.Where(item => item.FeeType == OrderFeeType.AgencyFee);
            var inspReceiveds = this.Receiveds.Where(item => item.FeeType == OrderFeeType.Incidental && item.FeeSourceID == null);
            #endregion

            //费用列表
            var feeList = new List<dynamic>();

            #region 货款
            feeList.Add(new
            {
                Type = Enums.OrderFeeType.Product.GetDescription(),
                ExchangeRate = this.ProductFeeExchangeRate.ToString("0.0000"),
                ExchangeRateType = this.Agreement.ProductFeeClause.ExchangeRateType.GetDescription(),
                AccountingDate = DateTime.Now.ToShortDateString(),
                DueDate = this.ProductFeeDueDate.ToShortDateString(),
                Receivable = (this.DeclarePrice * this.ProductFeeExchangeRate).ToRound(2).ToString("0.00"),
                Received = productReceiveds.Sum(item => item.Amount * item.Rate).ToRound(2).ToString("0.00"),
                PaymentDate = productReceiveds.Count() > 0 ? productReceiveds.First().CreateDate.ToShortDateString() : ""
            });
            #endregion

            #region 关税
            feeList.Add(new
            {
                Type = Enums.OrderFeeType.Tariff.GetDescription(),
                ExchangeRate = this.CustomsExchangeRate.ToString("0.0000"),
                ExchangeRateType = this.Agreement.TaxFeeClause.ExchangeRateType.GetDescription(),
                AccountingDate = DateTime.Now.ToShortDateString(),
                DueDate = this.TaxFeeDueDate.ToShortDateString(),
                Receivable = this.Items.Sum(item => item.ImportTax.Value)?.ToRound(2).ToString("0.00"),
                Received = tariffReceiveds.Sum(item => item.Amount * item.Rate).ToRound(2).ToString("0.00"),
                PaymentDate = tariffReceiveds.Count() > 0 ? tariffReceiveds.First().CreateDate.ToShortDateString() : ""
            });
            #endregion

            #region 增值税
            feeList.Add(new
            {
                Type = Enums.OrderFeeType.AddedValueTax.GetDescription(),
                ExchangeRate = this.CustomsExchangeRate.ToString("0.0000"),
                ExchangeRateType = this.Agreement.TaxFeeClause.ExchangeRateType.GetDescription(),
                AccountingDate = DateTime.Now.ToShortDateString(),
                DueDate = this.TaxFeeDueDate.ToShortDateString(),
                Receivable = this.Items.Sum(item => item.AddedValueTax.Value)?.ToRound(2).ToString("0.00"),
                Received = addedValueTaxReceiveds.Sum(item => item.Amount * item.Rate).ToRound(2).ToString("0.00"),
                PaymentDate = addedValueTaxReceiveds.Count() > 0 ? addedValueTaxReceiveds.First().CreateDate.ToShortDateString() : ""
            });
            #endregion

            #region 代理费
            feeList.Add(new
            {
                Type = OrderPremiumType.AgencyFee.GetDescription(),
                ExchangeRate = this.AgencyFeeExchangeRate.ToString("0.0000"),
                ExchangeRateType = this.Agreement.AgencyFeeClause.ExchangeRateType.GetDescription(),
                AccountingDate = DateTime.Now.ToShortDateString(),
                DueDate = this.AgencyFeeDueDate.ToShortDateString(),
                Receivable = (agencyFees.Sum(item => item.Count * item.UnitPrice * item.Rate) * taxpoint).ToRound(2).ToString("0.00"),
                Received = agencyReceiveds.Sum(item => item.Amount * item.Rate).ToRound(2).ToString("0.00"),
                PaymentDate = agencyReceiveds.Count() > 0 ? agencyReceiveds.First().CreateDate.ToShortDateString() : ""
            });
            #endregion

            #region 商检费
            if (inspFees.Count() > 0)
            {
                feeList.Add(new
                {
                    Type = OrderPremiumType.InspectionFee.GetDescription(),
                    ExchangeRate = inspFees.First().Rate.ToString("0.0000"),
                    ExchangeRateType = this.Agreement.IncidentalFeeClause.ExchangeRateType.GetDescription(),
                    AccountingDate = DateTime.Now.ToShortDateString(),
                    DueDate = this.IncidentalFeeDueDate.ToShortDateString(),
                    Receivable = (inspFees.Sum(item => item.Count * item.UnitPrice * item.Rate) * taxpoint).ToRound(2).ToString("0.00"),
                    Received = inspReceiveds.Sum(item => item.Amount * item.Rate).ToRound(2).ToString("0.00"),
                    PaymentDate = inspReceiveds.Count() > 0 ? inspReceiveds.First().CreateDate.ToShortDateString() : ""
                });
            }
            #endregion

            #region 其他杂费
            if (incidentalFees.Count() > 0)
            {
                feeList.AddRange(incidentalFees.Select(item => new
                {
                    Type = item.Type == OrderPremiumType.OtherFee ? item.Name : item.Type.GetDescription(),
                    ExchangeRate = item.Rate.ToString("0.0000"),
                    ExchangeRateType = this.Agreement.IncidentalFeeClause.ExchangeRateType.GetDescription(),
                    AccountingDate = DateTime.Now.ToShortDateString(),
                    DueDate = this.IncidentalFeeDueDate.ToShortDateString(),
                    Receivable = ((item.Count * item.UnitPrice * item.Rate) * taxpoint).ToRound(2).ToString("0.00"),
                    Received = item.OrderReceiveds.Sum(fee => fee.Amount * fee.Rate).ToRound(2).ToString("0.00"),
                    PaymentDate = item.OrderReceiveds.Count() > 0 ? item.OrderReceiveds.First().CreateDate.ToShortDateString() : ""
                }));
            }
            #endregion

            return feeList;
        }

        /// <summary>
        /// 设置pdfgrid单元格边框样式
        /// </summary>
        /// <param name="grid"></param>
        private void SetCellBorder(PdfGrid grid)
        {
            foreach (PdfGridRow pgr in grid.Rows)
            {
                foreach (PdfGridCell pgc in pgr.Cells)
                {
                    pgc.Style.Borders.All = new PdfPen(Color.Black, 0.01f);
                }
            }
        }

        /// <summary>
        /// 更新页数、页面、偏移量
        /// </summary>
        /// <param name="pdf">PDF文档</param>
        /// <param name="pageCount">新增页面前的页数</param>
        /// <param name="page">PDF页面</param>
        /// <param name="x">X轴偏移量</param>
        /// <param name="y">Y轴偏移量</param>
        private void UpdateIfNewPageCreated(PdfDocument pdf, out int pageCount, out PdfPageBase page, out float x, out float y)
        {
            pageCount = pdf.Pages.Count;
            page = pdf.Pages[pdf.Pages.Count - 1];
            x = pdf.PageSettings.Margins.Left;
            y = pdf.PageSettings.Margins.Top;
        }

        /// <summary>
        /// 修改主对账单的汇率
        /// </summary>
        public void MainChangeExchangeRate(Enums.OrderBillType orderBillType, decimal RealAgencyFee)
        {
            DoChangeRate(this.Order, this.Order.ID, this.Order.Admin.ID,
                    this.CustomsExchangeRate, this.RealExchangeRate, orderBillType);
            //修改汇率后的重新生成对账单操作
            //包括“如果是内单，自动上传对账单”操作
            GenerateBillAfterChangeRate(this, orderBillType, RealAgencyFee);
        }

        private void MainOrderBill_ExchangeRateChanged(object sender, MainExchangeRateChangedEventArgs e)
        {
            var order = e.Bill.Order;
            order.CustomsExchangeRate = e.Bill.CustomsExchangeRate;
            order.RealExchangeRate = e.Bill.RealExchangeRate;
            //order.GenerateBill(e.OrderBillType,e.RealAgencyFee);

            if (order.Admin != null)
            {
                order.Log(order.Admin, "跟单员[" + order.Admin.RealName + "]变更了对账单汇率");
            }
        }

        /// <summary>
        /// 修改汇率操作
        /// </summary>
        /// <param name="order"></param>
        /// <param name="orderID"></param>
        /// <param name="adminID"></param>
        /// <param name="customsExchangeRate"></param>
        /// <param name="realExchangeRate"></param>
        /// <param name="orderBillType"></param>
        private void DoChangeRate(
            Order order,
            string orderID,
            string adminID,
            decimal customsExchangeRate,
            decimal realExchangeRate,
            Enums.OrderBillType orderBillType)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new
                {
                    CustomsExchangeRate = customsExchangeRate,
                    RealExchangeRate = realExchangeRate,
                    OrderBillType = (int)orderBillType,
                }, item => item.ID == orderID);
            }

            Task.Run(() =>
            {
                if (order != null && order.Admin != null)
                {
                    order.Log(order.Admin, "跟单员[" + order.Admin.RealName + "]变更了对账单汇率");
                }
            });

        }

        /// <summary>
        /// 修改汇率后的重新生成对账单操作
        /// </summary>
        private void GenerateBillAfterChangeRate(
            Models.OrderBill orderBill, Enums.OrderBillType orderBillType, decimal RealAgencyFee)
        {
            var theOrder = orderBill.Order;
            theOrder.CustomsExchangeRate = orderBill.CustomsExchangeRate;
            theOrder.RealExchangeRate = orderBill.RealExchangeRate;
            theOrder.GenerateBill(orderBillType, RealAgencyFee);


            //如果是内单，自动上传对账单
            Task.Run(() =>
            {
                string orderid = theOrder.ID;
                var order = new Needs.Ccs.Services.Views.OrdersView()[orderid];
                if (order.Type != OrderType.Outside)
                {
                    var Orders = new Needs.Ccs.Services.Views.Orders2View().Where(item => item.MainOrderID == order.MainOrderID
                                                                 && item.OrderStatus != OrderStatus.Canceled
                                                                 && item.OrderStatus != OrderStatus.Returned
                                                                 && item.Status == Status.Normal).ToList();
                    var orderIds = Orders.Select(t => t.ID).ToList();
                    var decheads = new Needs.Ccs.Services.Views.DecHeadsView().Where(item => orderIds.Contains(item.OrderID));
                    //如果这个主订单下的所有的子订单都已经制单了，才自动上传对账单，委托书
                    if (orderIds.Count() == decheads.Count())
                    {
                        #region 自动上传对账单
                        var bill = getModel(order.MainOrderID);

                        //保存文件
                        string fileName = DateTime.Now.Ticks + ".pdf";
                        FileDirectory fileDic = new FileDirectory(fileName);
                        fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                        fileDic.CreateDataDirectory();
                        if (bill.ClientType == ClientType.Internal)
                        {
                            bill.ProductsForIcgoo = new List<MainOrderBillItemProduct>();
                            bill.PartProductsForIcgoo = new List<MainOrderBillItemProduct>();
                            foreach (var t in bill.Bills)
                            {
                                bill.ProductsForIcgoo.AddRange(t.Products);
                                bill.PartProductsForIcgoo.AddRange(t.PartProducts);
                            }
                            //bill.SaveASIcgoo(fileDic.FilePath);
                            var orderbill = new OrderBillToPdf(bill);
                            orderbill.SaveAs(fileDic.FilePath);
                        }
                        else
                        {
                            bill.SaveAs(fileDic.FilePath);
                        }

                        //先删除之前上传的委托书
                        var origFiles = order.MainOrderFiles.Where(f => f.FileType == FileType.OrderBill && f.Status == Status.Normal);
                        foreach (var aorigFile in origFiles)
                        {
                            Needs.Ccs.Services.Models.MainOrderFile orderBillfile = new Needs.Ccs.Services.Models.MainOrderFile();
                            orderBillfile.ID = aorigFile.ID;
                            orderBillfile.Abandon();
                            new CenterFilesTopView().Modify(new { Status = FileDescriptionStatus.Delete }, aorigFile.ID);
                        }

                        var ermAdmin = new Needs.Ccs.Services.Views.AdminsTopView2().FirstOrDefault(x => x.OriginID == order.AdminID);
                        var ErmAdminID = ermAdmin?.ID ?? "";
                        var dic = new { CustomName = fileName, WsOrderID = order.MainOrderID, AdminID = ErmAdminID };

                        var centerType = Needs.Ccs.Services.Models.ApiModels.Files.FileType.OrderBill;
                        //本地文件上传到服务器
                        var tempPath = fileDic.FilePath;
                        //if (bill.ClientType == ClientType.Internal)
                        //{
                        //    tempPath = fileDic.FilePath.Replace(".pdf", "_1.pdf");
                        //}     
                        var result = Needs.Ccs.Services.Models.CenterFilesTopView.Upload(tempPath, centerType, dic);
                        string[] ID = { result[0].FileID };
                        new CenterFilesTopView().Modify(new { Status = FileDescriptionStatus.Approved }, ID);
                        #endregion
                    }

                }
            });
        }

        private MainOrderBillViewModel getModel(string id)
        {
            var viewModel = new MainOrderBillViewModel();
            var model = getModelStander(id);
            if (model == null)
            {
                return null;
            }
            else
            {
                #region 两个Model 转换              
                viewModel.MainOrderID = id;

                viewModel.Bills = model.Bills;

                var purchaser = PurchaserContext.Current;
                viewModel.AgentName = purchaser.CompanyName;
                viewModel.AgentAddress = purchaser.Address;
                viewModel.AgentTel = purchaser.Tel;
                viewModel.AgentFax = purchaser.UseOrgPersonTel;
                viewModel.Purchaser = purchaser.CompanyName;
                viewModel.Bank = purchaser.BankName;
                viewModel.Account = purchaser.AccountName;
                viewModel.AccountId = purchaser.AccountId;
                viewModel.SealUrl = PurchaserContext.Current.SealUrl.ToUrl();

                viewModel.ClientName = model.OrderBill.Client.Company.Name;
                viewModel.ClientTel = model.OrderBill.Client.Company.Contact.Tel;
                viewModel.Currency = model.OrderBill.Currency;
                viewModel.IsLoan = model.OrderBill.IsLoan;
                viewModel.DueDate = model.OrderBill.GetDueDate().ToString("yyyy年MM月dd日");
                viewModel.CreateDate = model.OrderBill.CreateDate.ToString();
                viewModel.ClientType = model.OrderBill.Client.ClientType;

                //var OrderBillFile = model.OrderBillFile;
                //viewModel.FileID = OrderBillFile?.ID;
                //viewModel.FileStatus = OrderBillFile == null ? OrderFileStatus.NotUpload.GetDescription() :
                //                        OrderBillFile.FileStatus.GetDescription();
                //viewModel.FileName = OrderBillFile == null ? "" : OrderBillFile.Name;
                //viewModel.Url = OrderBillFile == null ? "" : OrderBillFile.Url;             
                //viewModel.Url = FileDirectory.Current.FileServerUrl + "/" + OrderBillFile?.Url.ToUrl();

                viewModel.summaryTotalPrice = model.BillTotalPrice;
                viewModel.summaryTotalCNYPrice = model.BillTotalCNYPrice;
                viewModel.summaryTotalTariff = model.BillTotalTariff;
                viewModel.summaryTotalExciseTax = model.BillTotalExciseTax;
                viewModel.summaryTotalAddedValueTax = model.BillTotalAddedValueTax;
                viewModel.summaryTotalAgencyFee = model.BillTotalAgencyFee;
                viewModel.summaryTotalIncidentalFee = model.BillTotalIncidentalFee;

                viewModel.summaryPay = model.BillTotalTaxAndFee;
                viewModel.summaryPayAmount = model.BillTotalDeclarePrice;


                viewModel.CreateDate = model.MainOrder.CreateDate.ToString("yyyy-MM-dd HH:mm");
                #endregion

                return viewModel;
            }


        }

        private MainOrderBillStander getModelStander(string id)
        {
            var Orders = new Views.Orders2View().Where(item => item.MainOrderID == id && item.OrderStatus >= Needs.Ccs.Services.Enums.OrderStatus.Quoted
                                                  && item.OrderStatus != Needs.Ccs.Services.Enums.OrderStatus.Canceled && item.OrderStatus != Needs.Ccs.Services.Enums.OrderStatus.Returned)
                         .ToList();

            var purchaser = PurchaserContext.Current;
            if (Orders.Count == 0)
            {
                return null;
            }
            else
            {
                MainOrderBillStander mainOrderBillStander = new MainOrderBillStander(purchaser, Orders);

                return mainOrderBillStander;
            }
        }

    }
}
