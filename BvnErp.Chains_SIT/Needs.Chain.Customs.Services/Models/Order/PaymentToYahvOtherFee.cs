using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class PaymentToYahvOtherFee
    {
        private string TinyOrderID { get; set; }

        private string OriginAdminID { get; set; }

        public PaymentToYahvOtherFee(string tinyOrderID, string originAdminID)
        {
            this.TinyOrderID = tinyOrderID;
            this.OriginAdminID = originAdminID;
        }

        public void Execute()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    List<Yahv.Payments.Models.XdtFee> listXdtFee = new List<Yahv.Payments.Models.XdtFee>();

                    //查询型号信息
                    var orderItems = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>().Where(t => t.OrderID == this.TinyOrderID)
                        .Select(item => new
                        {
                            OrderItemID = item.ID,
                        }).ToList();

                    if (orderItems == null || orderItems.Count <= 0)
                    {
                        return;
                    }

                    int orderItemCount = orderItems.Count();

                    //查询 ermAdminID
                    var adminsTopView2 = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView2>();

                    string ermAdminID = "";
                    var theAdmin = (from admin in adminsTopView2
                                    where admin.OriginID == this.OriginAdminID
                                    select admin).FirstOrDefault();

                    if (theAdmin != null)
                    {
                        ermAdminID = theAdmin.ID;
                    }

                    //查询订单客户等信息
                    string payerName = "";
                    string mainOrderID = "";
                    string clientID = "";

                    var orders = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
                    var clients = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
                    var companies = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>();

                    var info = (from order in orders
                                join client in clients on order.ClientID equals client.ID
                                join company in companies on client.CompanyID equals company.ID
                                where order.ID == this.TinyOrderID
                                   && order.Status == (int)Enums.Status.Normal
                                   && client.Status == (int)Enums.Status.Normal
                                   && company.Status == (int)Enums.Status.Normal
                                select new { order, company }).FirstOrDefault();

                    if (info != null)
                    {
                        mainOrderID = info.order.MainOrderId;
                        payerName = info.company.Name;
                        clientID = info.order.ClientID;
                        //customsExchangeRate = info.order.CustomsExchangeRate != null ? (decimal)info.order.CustomsExchangeRate : 0;
                    }
                    else
                    {
                        throw new Exception("通过 OrderID：" + this.TinyOrderID + " 查询不到 ClientCompany");
                    }

                    //查出杂费
                    var orderPremiums = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderPremiums>();
                    List<Models.OrderPremium> listOrderPremiumsOtherFee = (from orderPremium in orderPremiums
                                                                           where orderPremium.OrderID == this.TinyOrderID
                                                                              && orderPremium.Type != (int)Enums.OrderPremiumType.AgencyFee
                                                                              && orderPremium.Type != (int)Enums.OrderPremiumType.InspectionFee
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

                    if (listOrderPremiumsOtherFee != null && listOrderPremiumsOtherFee.Count > 0)
                    {
                        //查询 InvoiceTaxRate
                        var clientAgreements = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAgreements>();
                        var clientAgreementModel = (from clientAgreement in clientAgreements
                                                    where clientAgreement.Status == (int)Enums.Status.Normal
                                                       && clientAgreement.ClientID == clientID
                                                    select new
                                                    {
                                                        InvoiceTaxRate = clientAgreement.InvoiceTaxRate,
                                                    }).FirstOrDefault();
                        if (clientAgreementModel == null)
                        {
                            throw new Exception("通过 ClientID：" + clientID + " 查询不到 ClientAgreement");
                        }

                        //税点
                        var taxpoint = 1 + clientAgreementModel.InvoiceTaxRate;

                        decimal the其他杂费_All = listOrderPremiumsOtherFee.Sum(t => t.UnitPrice * t.Count * t.Rate) * taxpoint;
                        decimal the其他杂费 = the其他杂费_All / orderItemCount;
                        decimal the其他杂费_R2 = the其他杂费.ToRound(2);

                        //装载 listXdtFee
                        foreach (var orderItem in orderItems)
                        {
                            int index = orderItems.IndexOf(orderItem);

                            decimal use其他杂费 = the其他杂费;

                            //修正 其它杂费
                            if (index != orderItemCount - 1) //如果不是最后一个型号 
                            {
                                use其他杂费 = the其他杂费_R2;
                            }
                            else //最后一个型号
                            {
                                use其他杂费 = the其他杂费_All - (the其他杂费_R2 * (orderItemCount - 1));
                            }

                            listXdtFee.Add(new Yahv.Payments.Models.XdtFee(
                                Yahv.Payments.CatalogConsts.杂费, Yahv.Payments.SubjectConsts.其他, Yahv.Underly.Currency.CNY, use其他杂费, orderItem.OrderItemID));
                        }
                    }

                    //传杂费给 Yahv
                    Yahv.Payments.PaymentManager.Erp(ermAdminID)[payerName, PurchaserContext.Current.CompanyName][Yahv.Payments.ConductConsts.代报关]
                                .Receivable.XdtRecord(
                                        vastOrderID: mainOrderID,
                                        tinyOrderID: this.TinyOrderID,
                                        rate: 1, //customsExchangeRate,
                                        itemID: null,
                                        applicationID: null,
                                        isOnlyClearExtras: true,
                                        array: listXdtFee.ToArray());

                    string logGuid = Guid.NewGuid().ToString("N");

                    new Models.DeliveryNoticeApiLog()
                    {
                        ID = logGuid,
                        BatchID = logGuid,
                        OrderID = mainOrderID,
                        TinyOrderID = this.TinyOrderID,
                        RequestContent = listXdtFee.Json(),
                        Status = Enums.Status.Normal,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        Summary = "杂费应收调用Yahv的dll",
                    }.Enter();

                }
            }
            catch (Exception ex)
            {
                string tinyOrderID = this.TinyOrderID;
                ex.CcsLog("PaymentToYahvOtherFee|" + tinyOrderID + "|");
            }
        }

    }
}
