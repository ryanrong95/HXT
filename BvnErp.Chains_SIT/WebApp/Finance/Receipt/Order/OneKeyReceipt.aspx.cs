using Needs.Ccs.Services;
using Needs.Ccs.Services.Models;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Receipt.Order
{
    public partial class OneKeyReceipt : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string OrderIDs = Request.QueryString["OrderIDs"];
            string financeReceiptId = Request.QueryString["financeReceiptId"];

            this.Model.OrderIDs = OrderIDs;
            this.Model.financeReceiptId = financeReceiptId;
        }

        /// <summary>
        /// 执行一键收款
        /// </summary>
        protected void DoOneKeyReceipt()
        {
            try
            {
                string FinanceReceiptId = Request.Form["FinanceReceiptId"];
                string ReceiptTypeArrayStr = HttpUtility.UrlDecode(Request.Form["ReceiptTypeArray"]).Replace("&quot;", "\'").Replace("amp;", "");
                string OrderIDArrayStr = HttpUtility.UrlDecode(Request.Form["OrderIDArray"]).Replace("&quot;", "\'").Replace("amp;", "");

                string[] ReceiptTypeArray = ReceiptTypeArrayStr.JsonTo<string[]>();
                string[] OrderIDArray = OrderIDArrayStr.JsonTo<string[]>();

                var admin = new Needs.Ccs.Services.Views.AdminsTopView2().Where(t => t.OriginID == Needs.Wl.Admin.Plat.AdminPlat.Current.ID).FirstOrDefault();

                var receivableOrderReceipts = new Needs.Ccs.Services.Views.ReceivableOrderReceiptView().GetForOneKeyReceipt(OrderIDArray);

                List<Needs.Ccs.Services.Models.ReceiptToYahvAmountModel> listReceiptToYahvAmountModel = new List<Needs.Ccs.Services.Models.ReceiptToYahvAmountModel>();
                List<Needs.Ccs.Services.Models.OrderReceived> listReceived = new List<Needs.Ccs.Services.Models.OrderReceived>();


                CalcTotalReceipt(
                    admin,
                    FinanceReceiptId,
                    OrderIDArray,
                    ReceiptTypeArray,
                    receivableOrderReceipts,

                    out listReceiptToYahvAmountModel,
                    out listReceived);

                //检查本次收款金额是否会大于这笔款中剩下的金额 Begin

                decimal receiptTotalAmount = listReceiptToYahvAmountModel.Sum(t => t.Amount);
                var notice = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.MyReceiptNotices[FinanceReceiptId];
                decimal daiShouAll = (notice.Amount - notice.ClearAmount).ToRound(2);

                if (daiShouAll < receiptTotalAmount)
                {
                    Response.Write((new { success = "false", message = "所勾选要收费内容的总金额大于当前款项中剩余金额！" }).Json());
                    return;
                }

                //检查本次收款金额是否会大于这笔款中剩下的金额 End

                if (listReceived == null || !listReceived.Any())
                {
                    Response.Write((new { success = "true", message = "执行完成，未收取任何费用", }).Json());
                    return;
                }

                //华芯通 本地收款 Begin

                //foreach (var received in listReceived)
                //{
                //    received.Enter();
                //}

                foreach (var received in listReceived)
                {
                    received.ID = Needs.Overall.PKeySigner.Pick(PKeyType.OrderReceipt);
                }

                Needs.Ccs.Services.Models.OrderReceivedBatch orderReceivedBatch = new Needs.Ccs.Services.Models.OrderReceivedBatch(listReceived);
                orderReceivedBatch.Insert();

                //华芯通 本地收款 End

                //货款收完后进行美金权益相关的处理 Begin

                foreach (var orderID in OrderIDArray)
                {
                    var pruductFeeHandle = new Needs.Ccs.Services.Models.PruductFeeHandle(orderID);
                    pruductFeeHandle.Execute();
                }

                //货款收完后进行美金权益相关的处理 End

                //其它金额收完后进行美金权益相关的处理 Begin

                foreach (var received in listReceived)
                {
                    if (received.FeeType != Needs.Ccs.Services.Enums.OrderFeeType.Product)
                    {
                        var otherFeeHandle = new Needs.Ccs.Services.Models.OtherFeeHandle(received.OrderID, received.Amount);
                        otherFeeHandle.Execute();
                    }
                }

                //其它金额收完后进行美金权益相关的处理 End



                Needs.Ccs.Services.Models.IcgooOrderMapSet icgooOrderMapSet = new Needs.Ccs.Services.Models.IcgooOrderMapSet(OrderIDArray);
                icgooOrderMapSet.Execute();



                //此处调用 Yahv Begin

                string[] toYahvOrderIDs = listReceiptToYahvAmountModel.Select(t => t.OrderID).Distinct().ToArray();
                foreach (var toYahvOrderID in toYahvOrderIDs)
                {
                    var thisTimeReceiptToYahvAmountModels = listReceiptToYahvAmountModel.Where(t => t.OrderID == toYahvOrderID).ToList();

                    Needs.Ccs.Services.Models.ReceiptToYahv toYahv = new Needs.Ccs.Services.Models.ReceiptToYahv(
                        toYahvOrderID,
                        admin,
                        FinanceReceiptId,
                        thisTimeReceiptToYahvAmountModels.ToArray());
                    toYahv.Execute();
                }

                //一键收款核销信息提交到中心财务
                System.Threading.Tasks.Task.Run(() =>
                {

                    List<CenterReceipt> listReceiptToCenterModel = new List<CenterReceipt>();
                    foreach (var item in listReceiptToYahvAmountModel)
                    {
                        listReceiptToCenterModel.Add(new CenterReceipt()
                        {
                            SeqNo = notice.SeqNo,
                            Amount = item.Amount,
                            CreatorID = admin.ErmAdminID,
                            AccountNo = notice.Account == null ? "" : notice.Account.BankAccount,
                            FeeType = OrderFeeTypeTransfer.Current.L2CInTransfer(item.Type),
                        });
                    }

                    Receipt2Center receipt2Center = new Receipt2Center(listReceiptToCenterModel);
                    receipt2Center.Send();

                    //一键收款 更新 订单逾期信息表
                    foreach (var orderID in OrderIDArray)
                    {                        
                        ExpireUpdate expire = new ExpireUpdate(orderID);
                        expire.Update();
                    }
                });

                //此处调用 Yahv End

                Response.Write((new { success = "true", message = "一键收款成功", }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = "false", message = "一键收款发生错误：" + ex.Message, }).Json());
            }
        }


        private void CalcTotalReceipt(
            Needs.Ccs.Services.Models.Admin admin,
            string FinanceReceiptId,
            string[] OrderIDArray,
            string[] ReceiptTypeArray,
            List<Needs.Ccs.Services.Models.ReceivableOrderReceiptModel> receivableOrderReceiptModels,
            
            out List<Needs.Ccs.Services.Models.ReceiptToYahvAmountModel> outListReceiptToYahvAmountModel,
            out List<Needs.Ccs.Services.Models.OrderReceived> outListReceived)
        {
            List<Needs.Ccs.Services.Models.ReceiptToYahvAmountModel> listReceiptToYahvAmountModel = new List<Needs.Ccs.Services.Models.ReceiptToYahvAmountModel>();
            List<Needs.Ccs.Services.Models.OrderReceived> listReceived = new List<Needs.Ccs.Services.Models.OrderReceived>();


            var receivableOrderReceipts = receivableOrderReceiptModels.Select(item => new
            {
                OrderReceiptId = item.ID,
                item.OrderID,
                item.ClientID,
                FeeTypeValue = (int)item.FeeType,
                FeeSourceID = item.FeeSourceID,
                item.FeeTypeShowName,
                ReceivableAmount = item.ReceivableAmount.ToString("#0.00"),
                //item.Admin,
            });

            //要收税杂费, 直接按照应收进行收款
            if (ReceiptTypeArray.Contains("2"))
            {
                var receivableOrderReceiptsShuiDaiFee = receivableOrderReceipts.Where(t => t.FeeTypeValue != (int)Needs.Ccs.Services.Enums.OrderFeeType.Product).ToList();

                foreach (var shuiDaiFee in receivableOrderReceiptsShuiDaiFee)
                {
                    decimal receivableAmountDecimal = Convert.ToDecimal(shuiDaiFee.ReceivableAmount);
                    if (0 == receivableAmountDecimal)
                    {
                        continue;
                    }

                    var received = new Needs.Ccs.Services.Models.OrderReceived();
                    received.ReceiptNoticeID = FinanceReceiptId;
                    received.ClientID = shuiDaiFee.ClientID;
                    received.OrderID = shuiDaiFee.OrderID;
                    received.FeeSourceID = shuiDaiFee.FeeSourceID;
                    received.FeeType = (Needs.Ccs.Services.Enums.OrderFeeType)shuiDaiFee.FeeTypeValue;
                    received.Amount = receivableAmountDecimal;
                    received.Admin = admin;
                    //received.Enter();
                    listReceived.Add(received);


                    listReceiptToYahvAmountModel.Add(new Needs.Ccs.Services.Models.ReceiptToYahvAmountModel()
                    {
                        OrderID = shuiDaiFee.OrderID,

                        Amount = receivableAmountDecimal,
                        Type = shuiDaiFee.FeeTypeValue,
                        FeeSourceID = shuiDaiFee.FeeSourceID,
                    });
                }
            }

            //要收货款, 找出对该订单通过审核的付汇申请, 对该数值进行收款
            if (ReceiptTypeArray.Contains("1"))
            {
                //var receivableOrderReceiptsProductFee = receivableOrderReceipts.Where(t => t.FeeTypeValue == (int)Needs.Ccs.Services.Enums.OrderFeeType.Product).ToList();

                var rightPayExchangeApplyItemsRMBAmounts = new Needs.Ccs.Services.Views.CheckProductValueStatusView().GetRightPayExchangeApplyItemsRMBAmount(OrderIDArray);
                var receivedProductFeeAmounts = new Needs.Ccs.Services.Views.CheckProductValueStatusView().GetReceivedProductFeeAmount(OrderIDArray);
                var orders = new Needs.Ccs.Services.Views.Origins.OrdersOrigin().Where(t => OrderIDArray.Contains(t.ID) && t.Status == Needs.Ccs.Services.Enums.Status.Normal).ToList();

                //有效的付汇申请
                var rightPayExchangeItems = new Needs.Ccs.Services.Views.CheckProductValueStatusView().GetRightPayExchangeApplyItems(OrderIDArray);

                if (rightPayExchangeItems != null)
                {
                    foreach (var orderID in OrderIDArray)
                    {
                        #region 修改一键收货款，需要匹配付汇申请进行收款 ryan 

                        //decimal payExchangeAmount = 0;
                        decimal receivedAmount = 0;
                        string clientID = "";


                        //订单应收货款总金额
                        //var shouldRecive = rightPayExchangeItems.Where(t => t.OrderID == orderID).Sum(t => t.RMBAmount);

                        //订单实收货款金额
                        var receivedAmountModel = receivedProductFeeAmounts.Where(t => t.OrderID == orderID).FirstOrDefault();
                        if (receivedAmountModel != null)
                        {
                            receivedAmount = receivedAmountModel.RMBAmount;
                        }

                        //货款暂时不允许混合：要么一键收款、要么手动收款
                        if (receivedAmount == 0)
                        {
                            //查询客户ID
                            var theOrder = orders.Where(t => t.ID == orderID).FirstOrDefault();
                            if (theOrder != null)
                            {
                                clientID = theOrder.ClientID;
                            }

                            //没有手动收款
                            foreach (var payitem in rightPayExchangeItems.Where(t=>t.OrderID == orderID))
                            {
                                var received = new Needs.Ccs.Services.Models.OrderReceived();
                                received.ReceiptNoticeID = FinanceReceiptId;
                                received.ClientID = clientID;
                                received.OrderID = orderID;
                                received.FeeSourceID = payitem.PayExchangeApplyID;
                                received.FeeType = Needs.Ccs.Services.Enums.OrderFeeType.Product;
                                received.Amount = payitem.RMBAmount.ToRound(2);
                                received.Admin = admin;
                                listReceived.Add(received);

                                receivedAmount += payitem.RMBAmount.ToRound(2);
                            }

                            listReceiptToYahvAmountModel.Add(new Needs.Ccs.Services.Models.ReceiptToYahvAmountModel()
                            {
                                OrderID = orderID,
                                Amount = receivedAmount,
                                Type = (int)Needs.Ccs.Services.Enums.OrderFeeType.Product,
                                FeeSourceID = null,
                            });
                        }

                        #endregion

                        #region old

                        //var payExchangeAmountModel = rightPayExchangeApplyItemsRMBAmounts.Where(t => t.OrderID == orderID).FirstOrDefault();
                        //if (payExchangeAmountModel != null)
                        //{
                        //    payExchangeAmount = payExchangeAmountModel.RMBAmount;
                        //}

                        //var receivedAmountModel = receivedProductFeeAmounts.Where(t => t.OrderID == orderID).FirstOrDefault();
                        //if (receivedAmountModel != null)
                        //{
                        //    receivedAmount = receivedAmountModel.RMBAmount;
                        //}

                        //var theOrder = orders.Where(t => t.ID == orderID).FirstOrDefault();
                        //if (theOrder != null)
                        //{
                        //    clientID = theOrder.ClientID;
                        //}

                        //if (payExchangeAmount > receivedAmount)
                        //{
                        //    var received = new Needs.Ccs.Services.Models.OrderReceived();
                        //    received.ReceiptNoticeID = FinanceReceiptId;
                        //    received.ClientID = clientID;
                        //    received.OrderID = orderID;
                        //    received.FeeSourceID = null;
                        //    received.FeeType = Needs.Ccs.Services.Enums.OrderFeeType.Product;
                        //    received.Amount = (payExchangeAmount - receivedAmount).ToRound(2);
                        //    received.Admin = admin;
                        //    //received.Enter();
                        //    listReceived.Add(received);


                        //    listReceiptToYahvAmountModel.Add(new Needs.Ccs.Services.Models.ReceiptToYahvAmountModel()
                        //    {
                        //        OrderID = orderID,

                        //        Amount = (payExchangeAmount - receivedAmount).ToRound(2),
                        //        Type = (int)Needs.Ccs.Services.Enums.OrderFeeType.Product,
                        //        FeeSourceID = null,
                        //    });
                        //}

                        #endregion
                    }
                }
            }


            outListReceiptToYahvAmountModel = listReceiptToYahvAmountModel;
            outListReceived = listReceived;
        }


    }
}