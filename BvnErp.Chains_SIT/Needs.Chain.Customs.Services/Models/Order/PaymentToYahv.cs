using Needs.Utils.Serializers;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class PaymentToYahv
    {
        OrderPremium Agency { get; set; }

        Enums.OrderBillType OrderBillType { get; set; }

        ClientAgreement Agreement { get; set; }

        public PaymentToYahv(OrderPremium agency, Enums.OrderBillType orderBillType, ClientAgreement agreement)
        {
            this.Agency = agency;
            this.OrderBillType = orderBillType;
            this.Agreement = agreement;
        }

        public void Execute()
        {
            Task.Run(() =>
            {
                try
                {
                    string tinyOrderID = this.Agency.OrderID;
                    string mainOrderID = string.Empty;
                    string payerName = string.Empty;
                    string ermAdminID = string.Empty;
                    decimal customsExchangeRate = 0;

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
                    var preAgency = (bill.Agreement.PreAgency.HasValue && bill.Agreement.PreAgency > 0M) ?  bill.Agreement.PreAgency.Value : 0M;
                    switch (this.OrderBillType)
                    {
                        case Enums.OrderBillType.Normal:
                            isAverage = (bill.DeclarePrice * agencyRate + preAgency) < minAgencyFee ? true : false;
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

                    //基础收费平摊金额
                    var avePreAgency = preAgency / bill.Items.Count();

                    List<XdtFeeInfo> listXdtFeeInfo = bill.Items.Select(item => new XdtFeeInfo
                    {
                        OrderItemID = item.ID,
                        代理费 = isAverage ? aveAgencyFee : ((item.TotalPrice * agencyRate + avePreAgency) * taxpoint),
                        关税 = item.ImportTax.Value,
                        增值税 = item.AddedValueTax.Value,
                        消费税 = item.ExciseTax?.Value
                    }).ToList();

                    //修正 代理费
                    if (型号数量 > 1)
                    {
                        decimal 前N_1代理费 = 0;
                        for (int i = 0; i < listXdtFeeInfo.Count; i++)
                        {
                            if (i < listXdtFeeInfo.Count - 1) //前N-1个型号
                            {
                                listXdtFeeInfo[i].代理费 = listXdtFeeInfo[i].代理费.ToRound(2);
                                前N_1代理费 += listXdtFeeInfo[i].代理费;
                            }
                            else //最后一个型号
                            {
                                listXdtFeeInfo[i].代理费 = AgencyFee - 前N_1代理费;
                            }
                        }
                    }

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
                            int index = listXdtFeeInfo.IndexOf(xdtFeeInfo);

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
                                decimal the其他杂费_All = list其它杂费OrderPremium.Sum(t => t.UnitPrice * t.Count * t.Rate) * taxpoint;
                                decimal the其他杂费 = the其他杂费_All / 型号数量;
                                decimal the其他杂费_R2 = the其他杂费.ToRound(2);

                                decimal use其他杂费 = the其他杂费;

                                //修正 其它杂费
                                if (index != listXdtFeeInfo.Count - 1) //如果不是最后一个型号 
                                {
                                    use其他杂费 = the其他杂费_R2;
                                }
                                else //最后一个型号
                                {
                                    use其他杂费 = the其他杂费_All - (the其他杂费_R2 * (listXdtFeeInfo.Count - 1));
                                }

                                listXdtFee.Add(new Yahv.Payments.Models.XdtFee(
                                    Yahv.Payments.CatalogConsts.杂费, Yahv.Payments.SubjectConsts.其他, Yahv.Underly.Currency.CNY, use其他杂费, xdtFeeInfo.OrderItemID));
                            }
                        }

                        Yahv.Payments.PaymentManager.Erp(ermAdminID)[payerName, PurchaserContext.Current.CompanyName][Yahv.Payments.ConductConsts.代报关]
                            .Receivable.XdtRecord(
                                    vastOrderID: mainOrderID,
                                    tinyOrderID: tinyOrderID,
                                    rate: 1, //customsExchangeRate,
                                    itemID: null,
                                    applicationID: null,
                                    array: listXdtFee.ToArray());

                        new Models.DeliveryNoticeApiLog()
                        {
                            ID = Guid.NewGuid().ToString("N"),
                            BatchID = batchID,
                            OrderID = mainOrderID,
                            TinyOrderID = tinyOrderID,
                            RequestContent = listXdtFee.Json(),
                            Status = Enums.Status.Normal,
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now,
                            Summary = "对账单应收调用Yahv的dll",
                        }.Enter();

                    }

                }
                catch (Exception ex)
                {
                    string tinyOrderID = this.Agency.OrderID;
                    ex.CcsLog("GenerateBill中PaymentToYahv|" + tinyOrderID + "|");
                }
            });
        }

    }

    public class XdtFeeInfo
    {
        public string OrderItemID { get; set; } = string.Empty;

        public decimal 代理费 { get; set; }

        public decimal? 关税 { get; set; }

        public decimal? 增值税 { get; set; }

        public decimal? 消费税 { get; set; }
    }
}
