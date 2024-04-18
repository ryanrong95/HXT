using Needs.Ccs.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wl.HistoryImport
{
    public class CreateTax
    {
        public void Create(TaxHistoryUseOnly only)
        {
            var OrderView = new Needs.Ccs.Services.Views.OrdersView();
            string orderid = only.InvoiceItems.Select(t => t.OrderNO).Distinct().FirstOrDefault();
            var order = OrderView.Where(t => t.ID == orderid).FirstOrDefault();
         
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                string InvoiceNoticeID = Needs.Overall.PKeySigner.Pick(PKeyType.InvoiceNotice);
                //生成开票通知
                var invoiceItem = new Layer.Data.Sqls.ScCustoms.InvoiceNotices
                {
                    ID = InvoiceNoticeID,
                    ApplyID = "XDTAdmin",
                    AdminID = "XDTAdmin",
                    ClientID = order.Client.ID,
                    ClientInvoiceID = order.Client.Invoice.ID,
                    InvoiceType = (int)only.InvoiceType,
                    InvoiceTaxRate = only.InvoiceTaxRate,
                    Address = only.Adress,
                    Tel = only.Tel,
                    BankName = only.BankName,
                    BankAccount = only.BankAccount,
                    DeliveryType = (int)Needs.Ccs.Services.Enums.InvoiceDeliveryType.SendByPost,
                    MailName = order.Client.InvoiceConsignee.Name,
                    MailMobile = order.Client.InvoiceConsignee.Mobile,
                    MailAddress = order.Client.InvoiceConsignee.Address,
                    Status = (int)Needs.Ccs.Services.Enums.InvoiceNoticeStatus.Confirmed,
                    CreateDate = only.CreateDate,
                    UpdateDate = only.CreateDate,
                    Summary = "",
                };
                reponsitory.Insert(invoiceItem);

                //开票类型赛选
                if (only.InvoiceType == Needs.Ccs.Services.Enums.InvoiceType.Full)
                {
                    //生成开票通知项
                    foreach (var noticeItem in only.InvoiceItems)
                    {
                        var invoiceNoticeItem = new Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems
                        {
                            ID = Needs.Overall.PKeySigner.Pick(PKeyType.InvoiceNoticeItem),
                            InvoiceNoticeID = InvoiceNoticeID,
                            OrderID = noticeItem.OrderNO,
                            OrderItemID = noticeItem.OrderItemID,
                            UnitPrice = noticeItem.UnitPrice,
                            Amount = noticeItem.Amount,
                            Difference = noticeItem.Difference,
                            InvoiceNo = only.InvoiceNo,
                            Status = (int)Needs.Ccs.Services.Enums.Status.Normal,
                            CreateDate = only.CreateDate,
                            UpdateDate = only.CreateDate,
                            Summary = "",
                        };
                        reponsitory.Insert(invoiceNoticeItem);
                    }
                }
                else
                {
                    string orderIDs = "";
                    foreach (var p in only.InvoiceItems)
                    {
                        orderIDs = orderIDs + p.OrderNO + ",";
                    }
                    orderIDs.TrimEnd(',');
                    
                    foreach (var noticeItem in only.InvoiceItems)
                    {
                        var invoiceNoticeItem = new Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems
                        {
                            ID = Needs.Overall.PKeySigner.Pick(PKeyType.InvoiceNoticeItem),
                            InvoiceNoticeID = InvoiceNoticeID,
                            OrderID = orderIDs,
                            //OrderItemID = noticeItem.OrderItemID,
                            UnitPrice = noticeItem.UnitPrice,
                            Amount = noticeItem.Amount,
                            Difference = noticeItem.Difference,
                            InvoiceNo = only.InvoiceNo,
                            Status = (int)Needs.Ccs.Services.Enums.Status.Normal,
                            CreateDate = only.CreateDate,
                            UpdateDate = only.CreateDate,
                            Summary = "",
                        };
                        reponsitory.Insert(invoiceNoticeItem);
                    }
                }


                //更改订单状态
                foreach (var noticeItem in only.InvoiceItems)
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(
                          new
                          {
                              UpdateDate = order.UpdateDate,
                              InvoiceStatus = (int)Needs.Ccs.Services.Enums.InvoiceStatus.Invoiced
                          }, item => item.ID == noticeItem.OrderNO);
                }
                  
            }

           

        }
    }
}
