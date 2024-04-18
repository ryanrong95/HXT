using Needs.Ccs.Services.Views;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class PaymentToYahv_Temp
    {
        OrderPremium Agency { get; set; }

        Enums.OrderBillType OrderBillType { get; set; }

        ClientAgreement Agreement { get; set; }

        public PaymentToYahv_Temp(OrderPremium agency, Enums.OrderBillType orderBillType, ClientAgreement agreement)
        {
            this.Agency = agency;
            this.OrderBillType = orderBillType;
            this.Agreement = agreement;
        }

        public void Execute()
        {
            try
            {
                string tinyOrderID = this.Agency.OrderID;
                string mainOrderID = string.Empty;
                string payerName = string.Empty;
                string ermAdminID = string.Empty;
                decimal customsExchangeRate = 0;
                DateTime dateTime = DateTime.Now;

                //查 ErmAdminID
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    var adminsTopView2 = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView2>();

                    var theAdmin = (from admin in adminsTopView2
                                    where admin.OriginID == this.Agency.Admin.ID
                                    select admin).FirstOrDefault();

                    if (theAdmin != null)
                    {
                        ermAdminID = theAdmin.ID;
                    }
                }

                //查 mainOrderID、payerName
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    var orders = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
                    var clients = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
                    var companies = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>();

                    var info = (from order in orders
                                join client in clients on order.ClientID equals client.ID
                                join company in companies on client.CompanyID equals company.ID
                                where order.ID == this.Agency.OrderID
                                   && order.Status == (int)Enums.Status.Normal
                                   && client.Status == (int)Enums.Status.Normal
                                   && company.Status == (int)Enums.Status.Normal
                                select new { order, company }).FirstOrDefault();

                    if (info != null)
                    {
                        mainOrderID = info.order.MainOrderId;
                        payerName = info.company.Name;
                        customsExchangeRate = info.order.CustomsExchangeRate != null ? (decimal)info.order.CustomsExchangeRate : 0;
                        dateTime = info.order.CreateDate;
                    }
                    else
                    {
                        throw new Exception("通过 OrderID：" + this.Agency.OrderID + " 查询不到 ClientCompany");
                    }
                }

                //将代理费计算到每个订单项上
                var bill = new Needs.Ccs.Services.Views.OrderBillsView2(tinyOrderID).FirstOrDefault();

                decimal agencyRate = bill.AgencyFeeExchangeRate * this.Agreement.AgencyRate;
                bool isAverage = false;
                decimal minAgencyFee = this.Agreement.MinAgencyFee;
                switch (this.OrderBillType)
                {
                    case Enums.OrderBillType.Normal:
                        isAverage = bill.DeclarePrice * agencyRate < minAgencyFee ? true : false;
                        break;

                    case Enums.OrderBillType.MinAgencyFee:
                        isAverage = false;
                        break;

                    case Enums.OrderBillType.Pointed:
                        isAverage = true;
                        break;
                }

                //税点
                var taxpoint = 1 + bill.Agreement.InvoiceTaxRate;

                //平摊代理费、其他杂费
                decimal AgencyFee = bill.AgencyFee * taxpoint;
                int 型号数量 = bill.Items.Count();
                decimal aveAgencyFee = AgencyFee / 型号数量;

                List<XdtFeeInfo> listXdtFeeInfo = bill.Items.Select(item => new XdtFeeInfo
                {
                    OrderItemID = item.ID,
                    代理费 = isAverage ? aveAgencyFee : (item.TotalPrice * agencyRate * taxpoint),
                    关税 = item.ImportTax.Value,
                    增值税 = item.AddedValueTax.Value,
                    消费税 = item.ExciseTax?.Value
                }).ToList();

                //查出每个订单项的应收商检费
                List<Models.OrderPremium> listOrderPremiums = new List<OrderPremium>();
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    var orderPremiums = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderPremiums>();

                    listOrderPremiums = (from orderPremium in orderPremiums
                                         where orderPremium.OrderID == tinyOrderID
                                            && orderPremium.Type != (int)Enums.OrderPremiumType.AgencyFee
                                            && orderPremium.Status == (int)Enums.Status.Normal
                                         select new Models.OrderPremium()
                                         {
                                             OrderID = orderPremium.OrderID,
                                             OrderItemID = orderPremium.OrderItemID,
                                             UnitPrice = orderPremium.UnitPrice,
                                             Count = orderPremium.Count,
                                             Rate = orderPremium.Rate,
                                             Type = (Enums.OrderPremiumType)orderPremium.Type,
                                         }).ToList();
                }


                if (listXdtFeeInfo != null && listXdtFeeInfo.Any())
                {
                    string batchID = Guid.NewGuid().ToString("N");

                    List<Yahv.Payments.Models.XdtFee> listXdtFee = new List<Yahv.Payments.Models.XdtFee>();

                    foreach (var xdtFeeInfo in listXdtFeeInfo)
                    {
                        if (xdtFeeInfo.关税 != null)
                        {
                            listXdtFee.Add(new Yahv.Payments.Models.XdtFee(
                                Yahv.Payments.CatalogConsts.税款, Yahv.Payments.SubjectConsts.关税, Yahv.Underly.Currency.CNY, (decimal)xdtFeeInfo.关税, xdtFeeInfo.OrderItemID));
                        }
                        if (xdtFeeInfo.增值税 != null)
                        {
                            listXdtFee.Add(new Yahv.Payments.Models.XdtFee(
                                Yahv.Payments.CatalogConsts.税款, Yahv.Payments.SubjectConsts.销售增值税, Yahv.Underly.Currency.CNY, (decimal)xdtFeeInfo.增值税, xdtFeeInfo.OrderItemID));
                        }
                        if (xdtFeeInfo.消费税 != null)
                        {
                            listXdtFee.Add(new Yahv.Payments.Models.XdtFee(
                                Yahv.Payments.CatalogConsts.税款, Yahv.Payments.SubjectConsts.消费税, Yahv.Underly.Currency.CNY, (decimal)xdtFeeInfo.消费税, xdtFeeInfo.OrderItemID));
                        }

                        listXdtFee.Add(new Yahv.Payments.Models.XdtFee(
                                Yahv.Payments.CatalogConsts.代理费, Yahv.Payments.SubjectConsts.代理费, Yahv.Underly.Currency.CNY, (decimal)xdtFeeInfo.代理费, xdtFeeInfo.OrderItemID));

                        //商检费
                        Models.OrderPremium 商检费OrderPremium = listOrderPremiums
                            .Where(t => t.OrderItemID == xdtFeeInfo.OrderItemID
                                     && t.Type == Enums.OrderPremiumType.InspectionFee).FirstOrDefault();
                        if (商检费OrderPremium != null)
                        {
                            decimal the商检费 = 商检费OrderPremium.UnitPrice * 商检费OrderPremium.Count * 商检费OrderPremium.Rate * taxpoint;
                            listXdtFee.Add(new Yahv.Payments.Models.XdtFee(
                                Yahv.Payments.CatalogConsts.杂费, Yahv.Payments.SubjectConsts.商检费, Yahv.Underly.Currency.CNY, the商检费, xdtFeeInfo.OrderItemID));
                        }

                        //其它杂费
                        List<Models.OrderPremium> list其它杂费OrderPremium = listOrderPremiums
                            .Where(t => t.Type != Enums.OrderPremiumType.InspectionFee).ToList();
                        if (list其它杂费OrderPremium != null && list其它杂费OrderPremium.Any())
                        {
                            decimal the其他杂费 = list其它杂费OrderPremium.Sum(t => t.UnitPrice * t.Count * t.Rate) * taxpoint / 型号数量;
                            listXdtFee.Add(new Yahv.Payments.Models.XdtFee(
                                Yahv.Payments.CatalogConsts.杂费, Yahv.Payments.SubjectConsts.其他, Yahv.Underly.Currency.CNY, the其他杂费, xdtFeeInfo.OrderItemID));
                        }
                    }

                    Yahv.Payments.PaymentManager.Erp(ermAdminID)[payerName, PurchaserContext.Current.CompanyName][Yahv.Payments.ConductConsts.代报关]
                        .Receivable.XdtRecord_Temp(
                                vastOrderID: mainOrderID,
                                tinyOrderID: tinyOrderID,
                                dateTime: dateTime,
                                rate: 1, //customsExchangeRate,
                                itemID: null,
                                applicationID: null,
                                array: listXdtFee.ToArray());

                }

            }
            catch (Exception ex)
            {
                ex.CcsLog("GenerateBill中PaymentToYahv");
            }
        }
    }

    public class Order_Temp : Order
    {
        public new void GenerateBill(Enums.OrderBillType orderBillType = Enums.OrderBillType.Normal, decimal PointedAgencyFee = 0)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var order = this;

                //获取海关汇率和实时汇率
                decimal customsExchangeRate = order.CustomsExchangeRate.Value;
                decimal realExchangeRate = order.RealExchangeRate.Value;

                //代理费汇率类型
                decimal agentExchangeRate = 0;
                switch (order.ClientAgreement.AgencyFeeClause.ExchangeRateType)
                {
                    case Enums.ExchangeRateType.RealTime:
                        agentExchangeRate = realExchangeRate;
                        break;
                    case Enums.ExchangeRateType.Custom:
                        agentExchangeRate = customsExchangeRate;
                        break;
                    case Enums.ExchangeRateType.Agreed:
                        agentExchangeRate = order.ClientAgreement.AgencyFeeClause.ExchangeRateValue.HasValue ? order.ClientAgreement.AgencyFeeClause.ExchangeRateValue.Value : 0;
                        break;
                    default:
                        break;
                }

                //计算代理费
                decimal agencyRate = agentExchangeRate * order.ClientAgreement.AgencyRate;
                //重新计算订单总价格
                order.DeclarePrice = order.Items.Sum(item => item.TotalPrice);
                decimal orderAgencyFee = order.DeclarePrice * agencyRate;
                decimal minAgencyFee = order.ClientAgreement.MinAgencyFee;
                OrderPremium agency = new OrderPremium();
                //agency.ID = Needs.Overall.PKeySigner.Pick(PKeyType.OrderPremium);
                agency.OrderID = order.ID;
                agency.Type = Enums.OrderPremiumType.AgencyFee;
                agency.Admin = order.Client.Merchandiser;
                agency.Count = 1;
                agency.Currency = MultiEnumUtils.ToCode<Enums.Currency>(Enums.Currency.CNY);
                agency.Rate = 1;
                switch (orderBillType)
                {
                    case Enums.OrderBillType.Normal:
                        agency.UnitPrice = orderAgencyFee < minAgencyFee ? minAgencyFee : orderAgencyFee.ToRound(4);
                        break;

                    case Enums.OrderBillType.MinAgencyFee:
                        agency.UnitPrice = orderAgencyFee.ToRound(4);
                        break;

                    case Enums.OrderBillType.Pointed:
                        agency.UnitPrice = PointedAgencyFee;
                        break;
                }

                //计算关税、增值税
                foreach (var item in order.Items)
                {
                    //2020-09-09 更新
                    //完税价格计算公式：Round(Round(报关总价 * 运保杂, 2) * 海关汇率, 0)
                    var topPrice = (item.TotalPrice * ConstConfig.TransPremiumInsurance).ToRound(2);
                    var decHead = new DecHeadsView().Where(t => t.OrderID == order.ID).FirstOrDefault();
                    int decimalForTotalPrice = 0;
                    if (decHead != null && decHead.isTwoStep)
                    {
                        decimalForTotalPrice = 2;
                    }

                    var total = (topPrice * customsExchangeRate).ToRound(decimalForTotalPrice);

                    if (item.ImportTax != null)
                    {
                        //关税计算公式：Round(完税价格 * 关税率, 2)
                        var importTaxValue = (total * item.ImportTax.Rate).ToRound(2);
                        item.ImportTax.Value = importTaxValue;

                        if (item.ExciseTax != null)
                        {
                            //消费税计算公式：Round((完税价格＋关税)÷(1－消费税税率)×消费税税率, 2）
                            var exciseTaxValue = ((total + importTaxValue) / (1 - item.ExciseTax.Rate) * item.ExciseTax.Rate).ToRound(2);
                            item.ExciseTax.Value = exciseTaxValue;
                        }

                        if (item.AddedValueTax != null)
                        {
                            var exciseTaxRate = item.ExciseTax?.Rate ?? 0m;
                            //增值税计算公式：Round(((完税价 + 关税) + (完税价 + 关税) / (1-消费税税率) * 消费税税率) * 增值税率, 2)
                            var addedValueTaxValue = (((total + importTaxValue) + (total + importTaxValue) / (1 - exciseTaxRate) * exciseTaxRate) * item.AddedValueTax.Rate).ToRound(2);
                            item.AddedValueTax.Value = addedValueTaxValue;
                        }
                    }
                }

                //此处调用一下 Yahv 的 dll ，使用 Task 执行, 并记录异常日志 Begin

                PaymentToYahv_Temp paymentToYahv = new PaymentToYahv_Temp(agency, orderBillType, order.ClientAgreement);
                paymentToYahv.Execute();

                //此处调用了一下 Yahv 的 dll ，其中用了 Task, 并会记录异常日志 End

            }
        }
    }
}
