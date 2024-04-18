using Needs.Ccs.Services;
using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Receipt.Order
{
    /// <summary>
    /// 订单收款明细列表界面
    /// </summary>
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void LoadData()
        {
            //收款通知的ID
            string financeReceiptId = Request.QueryString["ID"];
            var notice = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.MyReceiptNotices[financeReceiptId];

            var unReceiveListClientName = Server.UrlDecode(Request.QueryString["UnReceiveListClientName"]);

            //var vouchers = new Needs.Ccs.Services.Views.FinanceVoucherView()
            //    .Where(t => t.FinanceReceiptID == financeReceiptId
            //             && t.Status == Needs.Ccs.Services.Enums.Status.Normal).ToList();

            //decimal voucherAmount = 0;
            //if (vouchers != null && vouchers.Any())
            //{
            //    voucherAmount = vouchers.Sum(t => t.Amount);
            //}

            this.Model.NoticeData = new
            {
                ClienName = notice.Client.Company.Name,
                Amount = notice.AvailableAmount.ToString("#0.00"),
                //VoucherAmount = voucherAmount.ToString("#0.00"),
                //AllAmount = (notice.Amount + voucherAmount).ToString("#0.00"),
                ClearAmount = notice.ClearAmount.ToString("#0.00"),
                ReceiptDate = notice.ReceiptDate.ToString("yyyy-MM-dd"),

                UnReceiveListClientName = unReceiveListClientName,
            }.Json();
        }

        protected void data()
        {
            string subAction = Request.QueryString["SubAction"];

            //收款弹框 可收款的 OrderReceipt
            if ("ReceivableOrderReceipt" == subAction)
            {
                string orderId = Request.QueryString["OrderID"];

                //var receivableOrderReceipt = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.ReceivableOrderReceipt.Where(i => i.OrderID == orderId);

                var receivableOrderReceipt = new Needs.Ccs.Services.Views.ReceivableOrderReceiptView().GetOrderReceiptForMerchandiser(orderId);

                Func<Needs.Ccs.Services.Models.ReceivableOrderReceiptModel, object> convertReceivableOrderReceipt = item => new
                {
                    OrderReceiptId = item.ID,
                    item.OrderID,
                    FeeTypeValue = (int)item.FeeType,
                    FeeSourceID = item.FeeSourceID,
                    item.FeeTypeShowName,  //FeeType = item.FeeType.GetDescription(),
                    ReceivableAmount = item.ReceivableAmount.ToString("#0.00"),
                    IsFooter = false,
                };

                var resultRows = receivableOrderReceipt.Select(convertReceivableOrderReceipt).ToList();
                //resultRows.Add(new
                //{
                //    OrderReceiptId = "",
                //    OrderID = "",
                //    FeeType = "总计",
                //    ReceivableAmount = receivableOrderReceipt.Sum(i => i.ReceivableAmount),
                //    IsFooter = true,
                //});

                decimal sum = 0;
                if (receivableOrderReceipt != null && receivableOrderReceipt.Any())
                {
                    sum = receivableOrderReceipt.Sum(i => decimal.Round(i.ReceivableAmount, 2));
                }

                List<dynamic> listFooter = new List<dynamic>();
                listFooter.Add(new
                {
                    OrderReceiptId = "",
                    OrderID = "",
                    FeeTypeShowName = "总计",
                    ReceivableAmount = sum,
                    IsFooter = true,
                });

                Response.Write(new
                {
                    rows = resultRows,
                    footer = listFooter.ToArray(),
                }.Json());
            }
            else
            {
                //页面的列表
                string clientId = Request.QueryString["ClientID"];
                string DecMainOrderID = Request.QueryString["DecMainOrderID"];
                string DecOrderStartDate = Request.QueryString["DecOrderStartDate"];
                string DecOrderEndDate = Request.QueryString["DecOrderEndDate"];

                var orders = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyOrders1;
                List<LambdaExpression> lambdas = new List<LambdaExpression>();
                Expression<Func<Needs.Ccs.Services.Models.Order, bool>> expression = item => true;

                #region 查询条件

                Expression<Func<Needs.Ccs.Services.Models.Order, bool>> lambda1 = item => item.OrderStatus >= Needs.Ccs.Services.Enums.OrderStatus.QuoteConfirmed && item.OrderStatus <= Needs.Ccs.Services.Enums.OrderStatus.Completed;
                lambdas.Add(lambda1);

                if (!string.IsNullOrEmpty(clientId))
                {
                    Expression<Func<Needs.Ccs.Services.Models.Order, bool>> lambda = item => item.ClientID == clientId;
                    lambdas.Add(lambda);
                }
                if (!string.IsNullOrEmpty(DecMainOrderID))
                {
                    Expression<Func<Needs.Ccs.Services.Models.Order, bool>> lambda = item => item.ID == DecMainOrderID;
                    lambdas.Add(lambda);
                }
                if (!string.IsNullOrEmpty(DecOrderStartDate))
                {
                    var from = DateTime.Parse(DecOrderStartDate);
                    Expression<Func<Needs.Ccs.Services.Models.Order, bool>> lambda = item => item.CreateDate >= from;
                    lambdas.Add(lambda);
                }
                if (!string.IsNullOrEmpty(DecOrderEndDate))
                {
                    var to = DateTime.Parse(DecOrderEndDate);
                    Expression<Func<Needs.Ccs.Services.Models.Order, bool>> lambda = item => item.CreateDate < to.AddDays(1);
                    lambdas.Add(lambda);
                }
                #endregion

                #region 页面需要数据
                int page, rows;
                int.TryParse(Request.QueryString["page"], out page);
                int.TryParse(Request.QueryString["rows"], out rows);
                var orderlist = orders.GetPageList(page, rows, expression, lambdas.ToArray());

                Response.Write(new
                {
                    rows = orderlist.Select(
                            order => new
                            {
                                order.MainOrderID,
                                order.ID,
                                order.Client.ClientCode,
                                ClientName = order.Client.Company.Name,
                                order.Currency,
                                DeclarePrice = order.DeclarePrice.ToRound(2).ToString("0.00"),
                                TotalDeclarePrice = (order.DeclarePrice * order.ProductFeeExchangeRate).ToString("#0.00"),
                                CreateDate = order.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                                OrderStatus = order.OrderStatus.GetDescription(),
                                ClientTypeInt = (int)order.Client.ClientType,

                            }
                         ).ToArray(),
                    total = orderlist.Total,
                }.Json());
                #endregion
            }
        }

        /// <summary>
        /// 收款记录(一笔款项的)
        /// </summary>
        protected void GetReceiptRecordFinance()
        {
            string financeReceiptId = Request.QueryString["FinanceReceiptId"];

            var receiptRecordFinance = from orderReceipt in Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.OrderReceiptsAllStatus
                                       where orderReceipt.FinanceReceiptID == financeReceiptId
                                                && orderReceipt.Type == Needs.Ccs.Services.Enums.OrderReceiptType.Received
                                       orderby orderReceipt.CreateDate descending
                                       select orderReceipt;

            Func<Needs.Ccs.Services.Models.OrderReceipt, object> convertReceiptRecordFinance = item => new
            {
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                AdminName = item.Admin.RealName,
                item.OrderID,
                FeeType = item.FeeTypeShowName,  //item.FeeType.GetDescription(),
                ReceiptType = (0 - item.Amount) > 0 ? "收款" : "冲正",
                Amount = (0 - item.Amount).ToString("#0.00"),
            };

            this.Paging(receiptRecordFinance, convertReceiptRecordFinance);
        }

        protected void Submit()
        {
            try
            {
                var admin = new Needs.Ccs.Services.Views.AdminsTopView2().Where(t => t.OriginID == Needs.Wl.Admin.Plat.AdminPlat.Current.ID).FirstOrDefault();

                string financeReceiptId = Request.QueryString["FinanceReceiptId"];
                var sources = Request.Form["ReceiveData"].Replace("&quot;", "\'").Replace("amp;", "");
                var receiveDataModel = sources.JsonTo<dynamic>();
                string OrderID = Request.QueryString["OrderID"];

                //是否收货款
                //bool isHasProductValue = false;
                //var payExchangeID = string.Empty;
                ////foreach (var item in receiveDataModel)
                ////{
                ////    int feeTypeValue = item.FeeTypeValue;
                ////    if (feeTypeValue == (int)Needs.Ccs.Services.Enums.OrderFeeType.Product)
                ////    {
                ////        isHasProductValue = true;
                ////        payExchangeID = item.FeeSourceID;
                ////        break;
                ////    }
                ////}
                ////如果收货款, 检验是否有审核、审批、完成的付汇申请
                //if (isHasProductValue)
                //{
                //    bool isRightPayExchangeApply = new Needs.Ccs.Services.Views.CheckProductValueStatusView().CheckIsRightPayExchangeApply(OrderID);
                //    if (isRightPayExchangeApply == false)
                //    {
                //        throw new Exception("客户还未以该订单进行付汇申请，或付汇申请还未审核，暂不能对货款进行核销！");
                //    }

                //    //if (!new Needs.Ccs.Services.Views.UserPayExchangeAppliesView().Any(t => t.ID == payExchangeID))
                //    //{
                //    //    throw new Exception("付汇申请ID不存在！");
                //    //}
                //}


                foreach (var item in receiveDataModel)
                {
                    string strReceiveAmount = item.ReceiveAmount;
                    decimal decReceiveAmount = 0;
                    if (!decimal.TryParse(strReceiveAmount, out decReceiveAmount))
                    {
                        throw new Exception("提交了非数字");
                    }
                    if (decReceiveAmount < 0)
                    {
                        throw new Exception("提交了小于0的数字");
                    }
                }

                foreach (var item in receiveDataModel)
                {
                    int feeTypeValue = item.FeeTypeValue;
                    decimal decReceivableAmount = item.ReceivableAmount;
                    decimal decReceiveAmount = item.ReceiveAmount;

                    if (feeTypeValue != (int)Needs.Ccs.Services.Enums.OrderFeeType.Product)
                    {
                        if (decReceivableAmount < decReceiveAmount)
                        {
                            throw new Exception("非货款类别，实收金额不能大于应收金额");
                        }
                    }
                }
                //by yess 2020-12-29  收款页面，核销货款时，判断订单是否有垫款:有垫款，归还垫资额度（修改垫资记录表、垫资申请.已使用金额）
                foreach (var item in receiveDataModel)
                {
                    int feeTypeValue = item.FeeTypeValue;
                    decimal decReceivableAmount = item.ReceivableAmount;
                    decimal decReceiveAmount = item.ReceiveAmount;
                    string payExchangeID = item.FeeSourceID;
                    if (feeTypeValue == (int)Needs.Ccs.Services.Enums.OrderFeeType.Product)
                    {
                        var payExchangeApplyView = new Needs.Ccs.Services.Views.PayExchangeApplyView().FirstOrDefault(t => t.ID == payExchangeID);
                        if (payExchangeApplyView.IsAdvanceMoney == 0)
                        {
                            AdvanceRecordModel advanceRecord = new AdvanceRecordModel();
                            advanceRecord.ClientID = payExchangeApplyView.ClientID;
                            advanceRecord.PayExchangeID = payExchangeID;
                            advanceRecord.AmountUsed = Convert.ToDecimal(decReceiveAmount);
                            advanceRecord.OrderID = OrderID;
                            advanceRecord.Update();
                        }
                    }
                }
                string oneTinyOrderID = string.Empty;

                foreach (var item in receiveDataModel)
                {
                    string orderReceiptId = item.OrderReceiptId;
                    decimal decReceiveAmount = item.ReceiveAmount;
                    if (0 == decReceiveAmount)
                    {
                        continue;
                    }

                    var orginOrderReceipt = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.OrderReceiptsAllStatus[orderReceiptId];

                    var received = new OrderReceived();
                    received.ReceiptNoticeID = financeReceiptId;
                    received.ClientID = orginOrderReceipt.ClientID;
                    received.OrderID = orginOrderReceipt.OrderID;
                    received.FeeSourceID = item.FeeSourceID;
                    received.FeeType = orginOrderReceipt.FeeType;
                    received.Amount = decReceiveAmount;
                    received.Admin = admin;
                    received.Enter();

                    oneTinyOrderID = orginOrderReceipt.OrderID;
                }

                var myFinanceReceipt = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.MyReceiptNotices[financeReceiptId];
                decimal clearAmount = myFinanceReceipt.ClearAmount;

                //判断是否要解除管控，进一步是否要取消订单挂起  Begin

                UnHangUpCheck unHangUpCheck = new UnHangUpCheck(OrderID,OrderID);
                unHangUpCheck.Execute();

                //判断是否要解除管控，进一步是否要取消订单挂起  End

                //此处调用 Yahv Begin
                System.Threading.Tasks.Task.Run(() =>
                {
                    List<ReceiptToYahvAmountModel> listReceiptToYahvAmountModel = new List<ReceiptToYahvAmountModel>();
                    foreach (var item in receiveDataModel)
                    {
                        decimal decReceiveAmount = item.ReceiveAmount;
                        if (0 == decReceiveAmount)
                        {
                            continue;
                        }

                        listReceiptToYahvAmountModel.Add(new ReceiptToYahvAmountModel()
                        {
                            Amount = item.ReceiveAmount,
                            Type = item.FeeTypeValue,
                            FeeSourceID = item.FeeSourceID,
                        });
                    }

                    ReceiptToYahv toYahv = new ReceiptToYahv(
                    oneTinyOrderID,
                    admin,
                    financeReceiptId,
                    listReceiptToYahvAmountModel.ToArray());
                    toYahv.Execute();

                });
                //此处调用 Yahv End

                //核销信息提交到中心财务
                System.Threading.Tasks.Task.Run(() =>
                {
                   
                    List<CenterReceipt> listReceiptToCenterModel = new List<CenterReceipt>();
                    foreach (var item in receiveDataModel)
                    {
                        decimal decReceiveAmount = item.ReceiveAmount;
                        if (0 == decReceiveAmount)
                        {
                            continue;
                        }

                        listReceiptToCenterModel.Add(new CenterReceipt()
                        {
                            SeqNo = myFinanceReceipt.SeqNo,
                            Amount = item.ReceiveAmount,
                            CreatorID = admin.ErmAdminID, 
                            AccountNo = myFinanceReceipt.Account==null?"":myFinanceReceipt.Account.BankAccount,
                            FeeType = OrderFeeTypeTransfer.Current.L2CInTransfer(item.FeeTypeValue),
                        });
                    }

                    Receipt2Center receipt2Center = new Receipt2Center(listReceiptToCenterModel);
                    receipt2Center.Send();

                    //收款 更新 订单逾期信息表
                    ExpireUpdate expire = new ExpireUpdate(OrderID);
                    expire.Update();
                });


                //NoticeLog noticeLog = new NoticeLog();
                //noticeLog.MainID = financeReceiptId;
                //noticeLog.NoticeType = Needs.Ccs.Services.Enums.SendNoticeType.ReceivingPending;
                //noticeLog.Readed = true;
                //noticeLog.SendNotice();

                Response.Write((new { success = "true", message = "提交成功", clearAmount = clearAmount }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = "false", message = "提交失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 收款记录(某个订单,某个类别的)
        /// 传入参数是 应收款项的 OrderReceiptId
        /// </summary>
        protected void ReceiptRecordOrder()
        {
            string receivableOrderReceiptId = Request.QueryString["OrderReceiptId"];

            var receivableOrderReceipt = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.OrderReceiptsAllStatus[receivableOrderReceiptId];

            IOrderedQueryable<OrderReceipt> receiptRecordOrder;

            if (receivableOrderReceipt.FeeType == Needs.Ccs.Services.Enums.OrderFeeType.Incidental && receivableOrderReceipt.FeeSourceID != null)
            {
                //杂费 并且 非商检费
                receiptRecordOrder = from orderReceipt in Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.OrderReceiptsAllStatus
                                     where orderReceipt.OrderID == receivableOrderReceipt.OrderID
                                        && orderReceipt.FeeType == receivableOrderReceipt.FeeType
                                        && orderReceipt.FeeSourceID == receivableOrderReceipt.FeeSourceID
                                        && orderReceipt.Type == Needs.Ccs.Services.Enums.OrderReceiptType.Received
                                     orderby orderReceipt.CreateDate descending
                                     select orderReceipt;
            }
            else if (receivableOrderReceipt.FeeType == Needs.Ccs.Services.Enums.OrderFeeType.Incidental && receivableOrderReceipt.FeeSourceID == null)
            {
                //杂费 并且 是商检费
                receiptRecordOrder = from orderReceipt in Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.OrderReceiptsAllStatus
                                     where orderReceipt.OrderID == receivableOrderReceipt.OrderID
                                        && orderReceipt.FeeType == receivableOrderReceipt.FeeType
                                        && orderReceipt.FeeSourceID == null
                                        && orderReceipt.Type == Needs.Ccs.Services.Enums.OrderReceiptType.Received
                                     orderby orderReceipt.CreateDate descending
                                     select orderReceipt;
            }
            else
            {
                //非杂费
                receiptRecordOrder = from orderReceipt in Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.OrderReceiptsAllStatus
                                     where orderReceipt.OrderID == receivableOrderReceipt.OrderID
                                        && orderReceipt.FeeType == receivableOrderReceipt.FeeType
                                        && orderReceipt.Type == Needs.Ccs.Services.Enums.OrderReceiptType.Received
                                     orderby orderReceipt.CreateDate descending
                                     select orderReceipt;
            }

            Func<Needs.Ccs.Services.Models.OrderReceipt, object> convert = item => new
            {
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                AdminName = item.Admin.RealName,
                ReceiptType = (0 - item.Amount) > 0 ? "收款" : "冲正",
                Amount = (0 - item.Amount).ToString("#0.00"),
                SeqNo = item.SeqNo,
                ReceiptDate = item.ReceiptDate?.ToString("yyyy-MM-dd"),
            };

            //this.Paging(receiptRecordOrder, convert);
            Response.Write(new
            {
                rows = receiptRecordOrder.Select(convert).ToList(),
                //total = orderReceiptDetail.Count()
            }.Json());
        }

        /// <summary>
        /// 香港代仓储订单数据
        /// </summary>
        protected void DataStore()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string ClienName = Request.QueryString["ClienName"];
            string StoreMainOrderID = Request.QueryString["StoreMainOrderID"];
            string StoreOrderStartDate = Request.QueryString["StoreOrderStartDate"];
            string StoreOrderEndDate = Request.QueryString["StoreOrderEndDate"];

            List<LambdaExpression> lamdas = new List<LambdaExpression>();
            Expression<Func<Needs.Ccs.Services.Views.WsOrderTopViewModel, bool>> lambda1 = item => item.ClientName == ClienName;
            lamdas.Add(lambda1);

            //Type 为 1, 2, 3 的是代仓储的订单
            Expression<Func<Needs.Ccs.Services.Views.WsOrderTopViewModel, bool>> lambda2 = item => new int[] { 1, 2, 3, }.Contains(item.Type);
            lamdas.Add(lambda2);

            if (!string.IsNullOrEmpty(StoreMainOrderID))
            {
                StoreMainOrderID = StoreMainOrderID.Trim();
                Expression<Func<Needs.Ccs.Services.Views.WsOrderTopViewModel, bool>> lambda = item => item.ID.Contains(StoreMainOrderID);
                lamdas.Add(lambda);
            }

            if (!string.IsNullOrEmpty(StoreOrderStartDate))
            {
                DateTime begin = DateTime.Parse(StoreOrderStartDate);
                Expression<Func<Needs.Ccs.Services.Views.WsOrderTopViewModel, bool>> lambda = item => item.CreateDate >= begin;
                lamdas.Add(lambda);
            }

            if (!string.IsNullOrEmpty(StoreOrderEndDate))
            {
                DateTime end = DateTime.Parse(StoreOrderEndDate);
                end = end.AddDays(1);
                Expression<Func<Needs.Ccs.Services.Views.WsOrderTopViewModel, bool>> lambda = item => item.CreateDate < end;
                lamdas.Add(lambda);
            }

            int totalCount = 0;
            var wsOrders = new Needs.Ccs.Services.Views.WsOrderTopView().GetList(out totalCount, page, rows, lamdas.ToArray());

            Func<Needs.Ccs.Services.Views.WsOrderTopViewModel, object> convert = item => new
            {
                ID = item.ID,
                ClientName = item.ClientName,
                SettlementCurrency = ((Needs.Ccs.Services.Enums.WsOrderCurrency)item.SettlementCurrency).ToString(),
                SupplierName = item.ChineseName,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                MainStatus = ((Needs.Ccs.Services.Enums.CgOrderStatus)item.MainStatus).GetDescription(),
            };

            Response.Write(new
            {
                rows = wsOrders.Select(convert).ToArray(),
                total = totalCount,
            }.Json());
        }

        /// <summary>
        /// 深圳代仓储账单数据
        /// </summary>
        protected void SzDataStore()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string ClienName = Request.QueryString["ClienName"];
            string CutDateIndex = Request.QueryString["CutDateIndex"];

            var query = new Needs.Ccs.Services.Views.PayeeLefts_Show_View();
            if (!string.IsNullOrEmpty(ClienName))
            {
                query = query.SearchByPayerName(ClienName);
            }
            if (!string.IsNullOrEmpty(CutDateIndex))
            {
                query = query.SearchByCutDateIndex(int.Parse(CutDateIndex));
            }
            
            var result = query.ToMyPage(page, rows);
            Response.Write(result.Json());
        }

        protected void GetCurrentClearAmount()
        {
            string FinanceReceiptId = Request.Form["FinanceReceiptId"];

            decimal clearAmount = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.MyReceiptNotices[FinanceReceiptId].ClearAmount;

            Response.Write((new { success = "true", clearAmount = clearAmount }).Json());
        }

    }
}