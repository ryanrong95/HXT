using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Views;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 判断是否要解除管控，进一步是否要取消订单挂起
    /// </summary>
    public class UnHangUpCheck
    {
        private string TinyOrderID { get; set; }
        private string OrderID { get; set; }
        private string orderControlId { get; set; }
        public UnHangUpCheck(string tinyOrderID, string orderID)
        {
            this.TinyOrderID = tinyOrderID;
            this.OrderID = orderID;
        }
        public UnHangUpCheck() { }

        public void Execute()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    var order = new Needs.Ccs.Services.Views.OrdersView(reponsitory)[this.TinyOrderID];

                    //客户的未付款/欠款
                    var unpaidFees = new Views.OrderReceiptsAllsView(reponsitory).Where(item => item.ClientID == order.Client.ID).ToList();

                    var checkResultTaxFee = CheckTaxFeeOverLimit(reponsitory, order, unpaidFees);
                    var checkResultAgencyFee = CheckAgencyFeeOverLimit(reponsitory, order, unpaidFees);
                    var checkResultIncidentalFee = CheckIncidentalFeeOverLimit(reponsitory, order, unpaidFees);

                    //如果还有超出垫款上线的项，就不执行下面的操作
                    if (checkResultTaxFee || checkResultAgencyFee || checkResultIncidentalFee)
                    {
                        return;
                    }

                    //如果没有超出垫款上线的项，就检查是否有垫款上线的管控，如果有就解除
                    var orderControls = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControls>();
                    var orderControlSteps = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControlSteps>();

                    var unAuditedOverLimitControls = (from orderControl in orderControls
                                                      join orderControlStep in orderControlSteps on orderControl.ID equals orderControlStep.OrderControlID
                                                      where orderControl.Status == (int)Enums.Status.Normal
                                                         && orderControlStep.Status == (int)Enums.Status.Normal
                                                         && orderControl.OrderID == order.ID
                                                         && orderControlStep.ControlStatus == (int)Enums.OrderControlStatus.Auditing
                                                         && orderControl.ControlType == (int)Enums.OrderControlType.ExceedLimit
                                                      select new
                                                      {
                                                          OrderControlID = orderControl.ID,
                                                          OrderControlStepID = orderControlStep.ID,
                                                      }).ToList();

                    if (unAuditedOverLimitControls != null && unAuditedOverLimitControls.Any())
                    {
                        var targetOrderControlStepIDs = unAuditedOverLimitControls.Select(t => t.OrderControlStepID).ToArray();

                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderControlSteps>(new
                        {
                            ControlStatus = (int)Enums.OrderControlStatus.Approved,
                        }, item => targetOrderControlStepIDs.Contains(item.ID));
                    }

                    //通过当前 control、controlStep 信息判断 订单的 IsHangUp 应该是 True/False
                    int unAuditedControlsCount = (from orderControl in orderControls
                                                  join orderControlStep in orderControlSteps
                                                        on new
                                                        {
                                                            OrderControlID = orderControl.ID,
                                                            OrderControlStatus = orderControl.Status,
                                                            OrderControlStepStatus = (int)Enums.Status.Normal,
                                                            OrderID = orderControl.OrderID,

                                                            ControlStatus = (int)Enums.OrderControlStatus.Auditing,
                                                        }
                                                        equals new
                                                        {
                                                            OrderControlID = orderControlStep.OrderControlID,
                                                            OrderControlStatus = (int)Enums.Status.Normal,
                                                            OrderControlStepStatus = orderControlStep.Status,
                                                            OrderID = order.ID,

                                                            ControlStatus = orderControlStep.ControlStatus,
                                                        }
                                                  select new OrderControlData
                                                  {
                                                      ID = orderControl.ID,
                                                  }).Count();

                    if (unAuditedControlsCount <= 0)
                    {
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new
                        { IsHangUp = false }, item => item.ID == order.ID);
                    }
                    else
                    {
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new
                        { IsHangUp = true }, item => item.ID == order.ID);
                    }

                }
            }
            catch (Exception ex)
            {
                ex.CcsLog("判断是否要解除管控，进一步是否要取消订单挂起(UnHangUpCheck)|" + this.TinyOrderID + "|" + ex.Message);
            }
        }

        /// <summary>
        /// 检查 税款 是否超出垫款上线
        /// </summary>
        /// <param name="reponsitory"></param>
        /// <param name="order"></param>
        /// <param name="unpaidFees"></param>
        /// <returns></returns>
        private bool CheckTaxFeeOverLimit(Layer.Data.Sqls.ScCustomsReponsitory reponsitory, Order order, List<OrderReceipt> unpaidFees)
        {
            //垫款上限
            var taxFeeLimit = order.ClientAgreement.TaxFeeClause.UpperLimit.GetValueOrDefault();

            //客户的未付款/欠款
            var unpaidTaxFee = unpaidFees.Where(item => item.FeeType == OrderFeeType.Tariff || item.FeeType == OrderFeeType.AddedValueTax)
                                             .Sum(item => item.Amount * item.Rate);

            return unpaidTaxFee > taxFeeLimit;
        }

        /// <summary>
        /// 检查 代理费 是否超出垫款上线
        /// </summary>
        /// <param name="reponsitory"></param>
        /// <param name="order"></param>
        /// <param name="unpaidFees"></param>
        /// <returns></returns>
        private bool CheckAgencyFeeOverLimit(Layer.Data.Sqls.ScCustomsReponsitory reponsitory, Order order, List<OrderReceipt> unpaidFees)
        {
            //垫款上限
            var agencyFeeLimit = order.ClientAgreement.AgencyFeeClause.UpperLimit.GetValueOrDefault();

            //客户的未付款/欠款
            var unpaidAgencyFee = unpaidFees.Where(item => item.FeeType == OrderFeeType.AgencyFee).Sum(item => item.Amount * item.Rate);

            return unpaidAgencyFee > agencyFeeLimit;
        }

        /// <summary>
        /// 检查 杂费 是否超出垫款上线
        /// </summary>
        /// <param name="reponsitory"></param>
        /// <param name="order"></param>
        /// <param name="unpaidFees"></param>
        /// <returns></returns>
        private bool CheckIncidentalFeeOverLimit(Layer.Data.Sqls.ScCustomsReponsitory reponsitory, Order order, List<OrderReceipt> unpaidFees)
        {
            //垫款上限
            var incidentalFeeLimit = order.ClientAgreement.IncidentalFeeClause.UpperLimit.GetValueOrDefault();

            //客户的未付款/欠款
            var unpaidIncidentalFee = unpaidFees.Where(item => item.FeeType == OrderFeeType.Incidental).Sum(item => item.Amount * item.Rate);

            return unpaidIncidentalFee > incidentalFeeLimit;
        }

        public bool IsExceedLimit(string ClientID)
        {
            bool isExceed = false;
            //客户当前协议，用于当前做验证
            var agreement = new Views.ClientAgreementsView().Where(t => t.ClientID == ClientID && t.Status == Status.Normal).FirstOrDefault();

            try
            {
                //using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                //{
                //    var tinyOrders = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>().
                //                     Where(t => t.MainOrderId == this.TinyOrderID && t.Status == (int)Status.Normal).
                //                     Select(t => t.ID).ToList();

                //    foreach (var orderId in tinyOrders)
                //    {
                //        var order = new Needs.Ccs.Services.Views.OrdersView(reponsitory)[orderId];

                //        //客户的未付款/欠款
                //        var unpaidFees = new Views.OrderReceiptsAllsView(reponsitory).Where(item => item.ClientID == order.Client.ID && tinyOrders.Contains(item.OrderID)).ToList();

                //        var checkResultTaxFee = CheckTaxFeeOverLimitBy5(reponsitory, order, unpaidFees);
                //        var checkResultAgencyFee = CheckAgencyFeeOverLimitBy5(reponsitory, order, unpaidFees);
                //        var checkResultIncidentalFee = CheckIncidentalFeeOverLimitBy5(reponsitory, order, unpaidFees);

                //        if (checkResultTaxFee || checkResultAgencyFee || checkResultIncidentalFee)
                //        {
                //            isExceed = true;
                //        }
                //    }

                //    //验证是否有审批通过的记录（垫款超出被取消的记录）
                //    //超过垫款上限，验证是否有审批记录。有记录，允许出库

                //    if (isExceed == true)
                //    {
                //        var hasApproved = new Views.HQControlRecordsView().Any(t =>
                //        t.ControlType == OrderControlType.ExceedLimit
                //        && t.ControlStatus == OrderControlStatus.Approved
                //        && t.Step == Enums.OrderControlStep.Headquarters
                //        && tinyOrders.Contains(t.OrderID));
                //        if (hasApproved)
                //        {
                //            isExceed = false;
                //        }
                //    }
                //}

                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    //验证是否有这个订单的审批记录
                    var controlView = from control in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControls>()
                                      join step in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControlSteps>() on control.ID equals step.OrderControlID
                                      where control.OrderID.Contains(this.TinyOrderID)
                                      && control.ControlType == (int)OrderControlType.ExceedLimit
                                      //&& step.ControlStatus == (int)OrderControlStatus.Approved
                                      && step.Step == (int)Enums.OrderControlStep.Headquarters
                                      select new
                                      {
                                          ID = control.ID,
                                          ControlType = control.ControlType,
                                          OrderID = control.OrderID,
                                          ControlStatus = step.ControlStatus
                                      };

                    if (controlView.Any(t => t.ControlStatus == (int)OrderControlStatus.Approved))
                    {
                        isExceed = false;
                        return isExceed;
                    }
                    else
                    {
                        //判断该客户是否有超额挂起

                        //已报关有效订单的收款情况，统计这些订单的未付款是否超过垫款上限
                        var receiptInfos = (from order in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                                            join head in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>() on order.ID equals head.OrderID
                                            join receipt in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>() on order.ID equals receipt.OrderID
                                            where head.IsSuccess == true && order.ClientID == ClientID
                                            && receipt.Status == (int)Enums.Status.Normal
                                            select new Models.OrderReceipt
                                            {
                                                ID = receipt.ID,
                                                FinanceReceiptID = receipt.FinanceReceiptID,
                                                ClientID = receipt.ClientID,
                                                OrderID = receipt.OrderID,
                                                FeeSourceID = receipt.FeeSourceID,
                                                FeeType = (Enums.OrderFeeType)receipt.FeeType,
                                                Type = (Enums.OrderReceiptType)receipt.Type,
                                                Currency = receipt.Currency,
                                                Rate = receipt.Rate,
                                                Amount = receipt.Amount,
                                                Status = (Enums.Status)receipt.Status,
                                                CreateDate = receipt.CreateDate,
                                                UpdateDate = receipt.UpdateDate,
                                                Summary = receipt.Summary,
                                                OrderStatus = (Enums.OrderStatus)order.OrderStatus,
                                                IsLoan = order.IsLoan,
                                            }).ToList();


                        var checkResultTaxFee = CheckTaxFeeOverLimitBy5Client(reponsitory, agreement, receiptInfos);
                        var checkResultAgencyFee = CheckAgencyFeeOverLimitBy5Client(reponsitory, agreement, receiptInfos);
                        var checkResultIncidentalFee = CheckIncidentalFeeOverLimitBy5Client(reponsitory, agreement, receiptInfos);

                        //垫货款也要管控额度
                        var checkAdvance = CheckAdvanceMoneyByClient(reponsitory, agreement);

                        if (checkResultTaxFee || checkResultAgencyFee || checkResultIncidentalFee || checkAdvance)
                        {
                            isExceed = true;

                            //验证是否有超额挂起记录 ryan 20210721 很多订单是报关时未超，出库时超出
                            if (!controlView.Any())
                            {
                                var newOrderControlID = Guid.NewGuid().ToString("N");
                                //插入超额挂起记录
                                reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.OrderControls
                                {
                                    ID = newOrderControlID,
                                    OrderID = this.OrderID,
                                    OrderItemID = null,
                                    ControlType = (int)Enums.OrderControlType.ExceedLimit,
                                    Status = (int)Enums.Status.Normal,
                                    CreateDate = DateTime.Now,
                                    UpdateDate = DateTime.Now,
                                    EventInfo = null,
                                    Applicant = null,
                                    MainOrderID = null,
                                });
                                Enums.OrderControlStep step = Enums.OrderControlStep.Headquarters;
                                string newOrderControlStepID = string.Concat(newOrderControlID, step).MD5();

                                reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.OrderControlSteps
                                {
                                    ID = newOrderControlStepID,
                                    OrderControlID = newOrderControlID,
                                    Step = (int)step,
                                    ControlStatus = (int)Enums.OrderControlStatus.Auditing,
                                    Status = (int)Enums.Status.Normal,
                                    CreateDate = DateTime.Now,
                                    UpdateDate = DateTime.Now,
                                });
                            }
                        }
                    }
                }


                return isExceed;
            }
            catch (Exception ex)
            {
                ex.CcsLog("判断是否超出垫款上限，跟单是否可以录入出库通知出错|" + this.TinyOrderID + "|" + ex.Message);
                return true;
            }
        }
        public bool isAdvanceMoney(string id)
        {
            bool falg = false;
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    var tinyOrders = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>().
                                     Where(t => t.MainOrderId == this.TinyOrderID && t.Status == (int)Status.Normal).
                                     Select(t => t.ClientID).FirstOrDefault();

                    //判断是否存在垫资记录，如果存在垫资并没有还款则不可以出库，如果已经还款，则可以出库  by yeshuangshuang 2021-04-19 

                    var advanceMoneyApply = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdvanceMoneyApplies>().FirstOrDefault(t => t.ClientID == tinyOrders && t.Status == (int)Enums.AdvanceMoneyStatus.Effective);
                    if (advanceMoneyApply != null)
                    {
                        var advanceApprovedList = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdvanceRecords>().Where(t => t.Status == (int)Enums.Status.Normal && t.OrderID == id && t.AdvanceID == advanceMoneyApply.ID).ToList();
                        foreach (var item in advanceApprovedList)
                        {
                            var strStartDate = item.AdvanceTime.AddDays(item.LimitDays);
                            var strEndDate = DateTime.Now;

                            //判断风控审核页面是否存在已审核并填写缓冲天数的订单客户，如果存在则比对缓冲天数是否在日期之内:1.如果在日前之内，风控不需要审核；反之，则再次审核；2.循环判断
                            if (!string.IsNullOrEmpty(advanceMoneyApply.OverdueAdvancePayment.ToString()))
                            {
                                var OverdueAdvanceTime = advanceMoneyApply.OverdueAdvancePayment;
                                if (item.PaidAmount != item.Amount && strStartDate < strEndDate && OverdueAdvanceTime < strEndDate)
                                {
                                    falg = true;
                                }
                            }
                            else
                            {
                                if (item.PaidAmount != item.Amount && strStartDate < strEndDate)
                                {
                                    falg = true;
                                }

                            }
                        }
                        if (falg)
                        {
                            string newOrderControlID = Guid.NewGuid().ToString("N");
                            var orderControlList = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControls>().FirstOrDefault(t => t.Status == (int)Enums.Status.Normal && t.OrderID == id && t.ControlType == (int)Enums.OrderControlType.OverdueAdvancePayment);
                            if (orderControlList == null)
                            {
                                reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.OrderControls
                                {
                                    ID = newOrderControlID,
                                    OrderID = id,
                                    OrderItemID = null,
                                    ControlType = (int)Enums.OrderControlType.OverdueAdvancePayment,
                                    Status = (int)Enums.Status.Normal,
                                    CreateDate = DateTime.Now,
                                    UpdateDate = DateTime.Now,
                                    EventInfo = null,
                                    Applicant = null,
                                    MainOrderID = this.TinyOrderID,
                                });
                                Enums.OrderControlStep step = Enums.OrderControlStep.Headquarters;
                                string newOrderControlStepID = string.Concat(newOrderControlID, step).MD5();

                                reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.OrderControlSteps
                                {
                                    ID = newOrderControlStepID,
                                    OrderControlID = newOrderControlID,
                                    Step = (int)step,
                                    ControlStatus = (int)Enums.OrderControlStatus.Auditing,
                                    Status = (int)Enums.Status.Normal,
                                    CreateDate = DateTime.Now,
                                    UpdateDate = DateTime.Now,
                                });

                            }

                        }
                    }

                }
                return falg;
            }
            catch (Exception ex)
            {
                ex.CcsLog("判断是否存在垫资申请，跟单是否可以录入出库通知出错|" + this.TinyOrderID + "|" + ex.Message);
                return true;
            }
        }

        /// <summary>
        /// 垫资超期
        /// </summary>
        /// <returns></returns>
        public bool isOverDuePayment(string ClientID)
        {
            bool falg = false;
            string beforeMouthLastDay = "";
            try
            {
                //判断是否存在垫资超期  by yeshuangshuang 2021-04-19
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {

                    var clientAgreementsView = new ClientAgreementsView().Where(t => t.ClientID == ClientID && t.Status == Status.Normal).FirstOrDefault();

                    //判断垫资账期类型
                    var taxPeriodType = clientAgreementsView.TaxFeeClause.PeriodType;
                    var agencyPeriodType = clientAgreementsView.AgencyFeeClause.PeriodType;
                    var incidentalPeriodType = clientAgreementsView.IncidentalFeeClause.PeriodType;

                    var orders = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
                    var orderControls = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControls>();
                    var orderControlSteps = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControlSteps>();

                    var clientIds = orders.Where(t => t.ClientID == ClientID && t.Status == (int)Status.Normal).Select(t => t.ID).ToList();

                    //是否存在缓冲天数，如果存在多笔缓冲天数，取日期最大的一笔数据
                    var bufferDays = (from orderControl in orderControls
                                      join orderControlStep in orderControlSteps on orderControl.ID equals orderControlStep.OrderControlID
                                      where orderControl.BufferDays != null
                                         && orderControl.Status == (int)Enums.Status.Normal
                                         && orderControlStep.Status == (int)Enums.Status.Normal
                                         && orderControl.ControlType == (int)Enums.OrderControlType.OverdueAdvancePayment
                                         && clientIds.Contains(orderControl.OrderID)
                                      orderby orderControl.BufferDays descending
                                      select new
                                      {
                                          OrderID = orderControl.OrderID,
                                          BufferDays = orderControl.BufferDays,
                                      }).FirstOrDefault();

                    if (bufferDays != null)
                    {
                        if (DateTime.Parse(bufferDays.BufferDays?.ToString("yyyy-MM-dd")) < DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd")))
                        {
                            //税费（约定期限/月结）
                            //获取所有已报关订单，循环每笔订单是否在约定期限内，如果不在则返回数据，挂起待风控审批
                            if (falg == false)
                            {
                                var taxFeeType = (int)FeeType.Tax;
                                if (taxPeriodType == PeriodType.AgreedPeriod)
                                {
                                    var taxDaysLimit = Convert.ToDouble(clientAgreementsView.TaxFeeClause.DaysLimit);
                                    var checkResultTax = CheckDaysLimit(reponsitory, ClientID, taxDaysLimit, taxFeeType);
                                    if (checkResultTax)
                                    {
                                        falg = true;
                                    }
                                }
                                else if (taxPeriodType == PeriodType.Monthly)
                                {
                                    //var year = DateTime.Now.Year.ToString();
                                    // var month = DateTime.Now.Month < 10 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString();
                                    var monthlyDay = clientAgreementsView.TaxFeeClause.MonthlyDay < 10 ? "0" + clientAgreementsView.TaxFeeClause.MonthlyDay.ToString() : clientAgreementsView.TaxFeeClause.MonthlyDay.ToString();
                                    // var taxMonthlyDay = DateTime.Parse(Convert.ToDateTime(year + "-" + month + "-" + monthlyDay).ToString("yyyy-MM-dd"));

                                    //if (taxMonthlyDay < DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd")))//本月往前推1个月
                                    //{
                                    //    beforeMouthLastDay = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddDays(-1).ToString("yyyy-MM-dd"); //获取上个月最后一天日期
                                    //}
                                    //else//本月往前推2个月
                                    //{
                                    //    beforeMouthLastDay = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddDays(-2).ToString("yyyy-MM-dd"); //获取上个月最后一天日期
                                    //}

                                    var checkResultTax = CheckMonthlyDay(reponsitory, ClientID, monthlyDay, taxFeeType);
                                    if (checkResultTax)
                                    {
                                        falg = true;
                                    }

                                }
                            }

                            //代理费费（约定期限/月结）
                            if (falg == false)
                            {
                                var agencyFeeType = (int)FeeType.AgencyFee;
                                if (agencyPeriodType == PeriodType.AgreedPeriod)
                                {
                                    var agencyDaysLimit = Convert.ToDouble(clientAgreementsView.AgencyFeeClause.DaysLimit);
                                    var checkResultAgency = CheckDaysLimit(reponsitory, ClientID, agencyDaysLimit, agencyFeeType);
                                    if (checkResultAgency)
                                    {
                                        falg = true;
                                    }
                                }
                                else if (agencyPeriodType == PeriodType.Monthly)
                                {
                                    //var year = DateTime.Now.Year.ToString();
                                    //var month = DateTime.Now.Month < 10 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString();
                                    var monthlyDay = clientAgreementsView.AgencyFeeClause.MonthlyDay < 10 ? "0" + clientAgreementsView.AgencyFeeClause.MonthlyDay.ToString() : clientAgreementsView.AgencyFeeClause.MonthlyDay.ToString();
                                    //var agencyMonthlyDay = DateTime.Parse(Convert.ToDateTime(year + "-" + month + "-" + monthlyDay).ToString("yyyy-MM-dd"));

                                    //if (agencyMonthlyDay < DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd")))//本月往前推1个月
                                    //{
                                    //    beforeMouthLastDay = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddDays(-1).ToString("yyyy-MM-dd"); //获取上个月最后一天日期

                                    //}
                                    //else
                                    //{
                                    //    beforeMouthLastDay = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddDays(-2).ToString("yyyy-MM-dd"); //获取上个月最后一天日期
                                    //}

                                    var checkResultTax = CheckMonthlyDay(reponsitory, ClientID, monthlyDay, agencyFeeType);
                                    if (checkResultTax)
                                    {
                                        falg = true;
                                    }

                                }
                            }

                            //杂费（约定期限/月结）
                            if (falg == false)
                            {
                                var incidentalFeeType = (int)FeeType.Incidental;
                                if (incidentalPeriodType == PeriodType.AgreedPeriod)
                                {
                                    var incidentalDaysLimit = Convert.ToDouble(clientAgreementsView.IncidentalFeeClause.DaysLimit);
                                    var checkResultIncidental = CheckDaysLimit(reponsitory, ClientID, incidentalDaysLimit, incidentalFeeType);
                                    if (checkResultIncidental)
                                    {
                                        falg = true;
                                    }
                                }
                                else if (incidentalPeriodType == PeriodType.Monthly)
                                {
                                    // var year = DateTime.Now.Year.ToString();
                                    //var month = DateTime.Now.Month < 10 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString();
                                    var monthlyDay = clientAgreementsView.IncidentalFeeClause.MonthlyDay < 10 ? "0" + clientAgreementsView.IncidentalFeeClause.MonthlyDay.ToString() : clientAgreementsView.IncidentalFeeClause.MonthlyDay.ToString();
                                    // var incidentalMonthlyDay = DateTime.Parse(Convert.ToDateTime(year + "-" + month + "-" + monthlyDay).ToString("yyyy-MM-dd"));

                                    //if (incidentalMonthlyDay < DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd")))//本月往前推1个月
                                    //{
                                    //    beforeMouthLastDay = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddDays(-1).ToString("yyyy-MM-dd"); //获取上个月最后一天日期

                                    //}
                                    //else
                                    //{
                                    //    beforeMouthLastDay = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddDays(-2).ToString("yyyy-MM-dd"); //获取上个月最后一天日期
                                    //}
                                    var checkResultTax = CheckMonthlyDay(reponsitory, ClientID, monthlyDay, incidentalFeeType);
                                    if (checkResultTax)
                                    {
                                        falg = true;
                                    }
                                }
                            }

                            //20230704 货款垫资超期
                            if (falg == false) {
                                var advanceRecord = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdvanceRecords>().Where(t =>t.ClientID == ClientID && t.Amount > t.PaidAmount);
                                foreach (var ad in advanceRecord)
                                {
                                    var limitday = DateTime.Parse(ad.AdvanceTime.AddDays(ad.LimitDays).ToString("yyyy-MM-dd"));
                                    if (DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd")) > limitday)
                                    {
                                        falg = true;
                                        break;
                                    }
                                }
                            }

                        }
                    }
                    else
                    {
                        //税费（约定期限/月结）
                        //获取所有已报关订单，循环每笔订单是否在约定期限内，如果不在则返回数据，挂起待风控审批
                        if (falg == false)
                        {
                            var taxFeeType = (int)FeeType.Tax;
                            if (taxPeriodType == PeriodType.AgreedPeriod)
                            {
                                var taxDaysLimit = Convert.ToDouble(clientAgreementsView.TaxFeeClause.DaysLimit);
                                var checkResultTax = CheckDaysLimit(reponsitory, ClientID, taxDaysLimit, taxFeeType);
                                if (checkResultTax)
                                {
                                    falg = true;
                                }
                            }
                            else if (taxPeriodType == PeriodType.Monthly)
                            {
                                //var year = DateTime.Now.Year.ToString();
                                //var month = DateTime.Now.Month < 10 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString();
                                var monthlyDay = clientAgreementsView.TaxFeeClause.MonthlyDay < 10 ? "0" + clientAgreementsView.TaxFeeClause.MonthlyDay.ToString() : clientAgreementsView.TaxFeeClause.MonthlyDay.ToString();
                                //var taxMonthlyDay = DateTime.Parse(Convert.ToDateTime(year + "-" + month + "-" + monthlyDay).ToString("yyyy-MM-dd"));

                                //if (taxMonthlyDay < DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd")))//本月往前推1个月
                                //{
                                //    beforeMouthLastDay = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddDays(-1).ToString("yyyy-MM-dd"); //获取上个月最后一天日期
                                //}
                                //else//本月往前推2个月
                                //{
                                //    beforeMouthLastDay = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddDays(-2).ToString("yyyy-MM-dd"); //获取上个月最后一天日期
                                //}

                                var checkResultTax = CheckMonthlyDay(reponsitory, ClientID, monthlyDay, taxFeeType);
                                if (checkResultTax)
                                {
                                    falg = true;
                                }

                            }
                        }

                        //代理费费（约定期限/月结）
                        if (falg == false)
                        {
                            var agencyFeeType = (int)FeeType.AgencyFee;
                            if (agencyPeriodType == PeriodType.AgreedPeriod)
                            {
                                var agencyDaysLimit = Convert.ToDouble(clientAgreementsView.AgencyFeeClause.DaysLimit);
                                var checkResultAgency = CheckDaysLimit(reponsitory, ClientID, agencyDaysLimit, agencyFeeType);
                                if (checkResultAgency)
                                {
                                    falg = true;
                                }
                            }
                            else if (agencyPeriodType == PeriodType.Monthly)
                            {
                                // var year = DateTime.Now.Year.ToString();
                                //var month = DateTime.Now.Month < 10 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString();
                                var monthlyDay = clientAgreementsView.AgencyFeeClause.MonthlyDay < 10 ? "0" + clientAgreementsView.AgencyFeeClause.MonthlyDay.ToString() : clientAgreementsView.AgencyFeeClause.MonthlyDay.ToString();
                                // var agencyMonthlyDay = DateTime.Parse(Convert.ToDateTime(year + "-" + month + "-" + monthlyDay).ToString("yyyy-MM-dd"));

                                //if (agencyMonthlyDay < DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd")))//本月往前推1个月
                                //{
                                //    beforeMouthLastDay = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddDays(-1).ToString("yyyy-MM-dd"); //获取上个月最后一天日期

                                //}
                                //else
                                //{
                                //    beforeMouthLastDay = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddDays(-2).ToString("yyyy-MM-dd"); //获取上个月最后一天日期
                                //}

                                var checkResultTax = CheckMonthlyDay(reponsitory, ClientID, monthlyDay, agencyFeeType);
                                if (checkResultTax)
                                {
                                    falg = true;
                                }

                            }
                        }

                        //杂费（约定期限/月结）
                        if (falg == false)
                        {
                            var incidentalFeeType = (int)FeeType.Incidental;
                            if (incidentalPeriodType == PeriodType.AgreedPeriod)
                            {
                                var incidentalDaysLimit = Convert.ToDouble(clientAgreementsView.IncidentalFeeClause.DaysLimit);
                                var checkResultIncidental = CheckDaysLimit(reponsitory, ClientID, incidentalDaysLimit, incidentalFeeType);
                                if (checkResultIncidental)
                                {
                                    falg = true;
                                }
                            }
                            else if (incidentalPeriodType == PeriodType.Monthly)
                            {
                                //var year = DateTime.Now.Year.ToString();
                                // var month = DateTime.Now.Month < 10 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString();
                                var monthlyDay = clientAgreementsView.IncidentalFeeClause.MonthlyDay < 10 ? "0" + clientAgreementsView.IncidentalFeeClause.MonthlyDay.ToString() : clientAgreementsView.IncidentalFeeClause.MonthlyDay.ToString();
                                // var incidentalMonthlyDay = DateTime.Parse(Convert.ToDateTime(year + "-" + month + "-" + monthlyDay).ToString("yyyy-MM-dd"));

                                //if (incidentalMonthlyDay < DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd")))//本月往前推1个月
                                //{
                                //    beforeMouthLastDay = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddDays(-1).ToString("yyyy-MM-dd"); //获取上个月最后一天日期

                                //}
                                //else
                                //{
                                //    beforeMouthLastDay = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddDays(-2).ToString("yyyy-MM-dd"); //获取上个月最后一天日期
                                //}
                                var checkResultTax = CheckMonthlyDay(reponsitory, ClientID, monthlyDay, incidentalFeeType);
                                if (checkResultTax)
                                {
                                    falg = true;
                                }
                            }
                        }

                        //20230704 货款垫资超期
                        if (falg == false)
                        {
                            var advanceRecord = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdvanceRecords>().Where(t => t.ClientID == ClientID && t.Amount > t.PaidAmount);
                            foreach (var ad in advanceRecord)
                            {
                                var limitday = DateTime.Parse(ad.AdvanceTime.AddDays(ad.LimitDays).ToString("yyyy-MM-dd"));
                                if (DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd")) > limitday)
                                {
                                    falg = true;
                                    break;
                                }
                            }
                        }

                    }
                    if (falg)
                    {
                        var checkOrderId = (from orderControl in orderControls
                                            join orderControlStep in orderControlSteps on orderControl.ID equals orderControlStep.OrderControlID
                                            where orderControl.Status == (int)Enums.Status.Normal
                                               && orderControlStep.Status == (int)Enums.Status.Normal
                                               && orderControl.ControlType == (int)Enums.OrderControlType.OverdueAdvancePayment
                                               && orderControl.OrderID == this.OrderID
                                            select new
                                            {
                                                ControlId = orderControl.ID,
                                                OrderID = orderControl.OrderID,
                                                // MainOrderID = orderControlId
                                            }).FirstOrDefault();
                        if (checkOrderId != null)
                        {
                            reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderControls>(new
                            {
                                MainOrderID = orderControlId,

                            }, item => item.OrderID == checkOrderId.OrderID);

                            reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderControlSteps>(new
                            {
                                ControlStatus = (int)Enums.OrderControlStatus.Auditing,
                                Step = (int)Enums.OrderControlStep.Headquarters
                            }, item => item.OrderControlID == checkOrderId.ControlId);
                        }
                        else
                        {
                            string newOrderControlID = Guid.NewGuid().ToString("N");
                            var orderControlList = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControls>().FirstOrDefault(t => t.Status == (int)Enums.Status.Normal && t.OrderID == this.OrderID && t.ControlType == (int)Enums.OrderControlType.OverdueAdvancePayment);
                            if (orderControlList == null)
                            {
                                reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.OrderControls
                                {
                                    ID = newOrderControlID,
                                    OrderID = OrderID,
                                    OrderItemID = null,
                                    ControlType = (int)Enums.OrderControlType.OverdueAdvancePayment,
                                    Status = (int)Enums.Status.Normal,
                                    CreateDate = DateTime.Now,
                                    UpdateDate = DateTime.Now,
                                    EventInfo = null,
                                    Applicant = null,
                                    MainOrderID = orderControlId,
                                });
                                Enums.OrderControlStep step = Enums.OrderControlStep.Headquarters;
                                string newOrderControlStepID = string.Concat(newOrderControlID, step).MD5();

                                reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.OrderControlSteps
                                {
                                    ID = newOrderControlStepID,
                                    OrderControlID = newOrderControlID,
                                    Step = (int)step,
                                    ControlStatus = (int)Enums.OrderControlStatus.Auditing,
                                    Status = (int)Enums.Status.Normal,
                                    CreateDate = DateTime.Now,
                                    UpdateDate = DateTime.Now,
                                });

                            }
                        }
                    }

                }
                return falg;
            }
            catch (Exception ex)
            {
                ex.CcsLog("判断是否存在垫资申请，跟单是否可以录入出库通知出错|" + this.TinyOrderID + "|" + ex.Message);
                return true;
            }
        }

        public string GetOrderID(string ClientID)
        {
            string orderIdNew = "";
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var orders = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
                var orderControls = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControls>();
                var orderControlSteps = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControlSteps>();

                var orderIds = orders.Where(t => t.ClientID == ClientID).Select(t => t.ID).ToList();
                var GetOrderId = (from orderControl in orderControls
                                  join orderControlStep in orderControlSteps on orderControl.ID equals orderControlStep.OrderControlID
                                  where orderIds.Contains(orderControl.OrderID)
                                     && orderControl.Status == (int)Enums.Status.Normal
                                     && orderControlStep.Status == (int)Enums.Status.Normal
                                     && orderControlStep.ControlStatus == (int)Enums.OrderControlStatus.Auditing
                                     && orderControl.ControlType == (int)Enums.OrderControlType.OverdueAdvancePayment
                                     orderby orderControl.CreateDate descending
                                  select new
                                  {
                                      MainOrderID = orderControl.MainOrderID
                                  }).FirstOrDefault();

                if (GetOrderId != null)
                {
                    orderIdNew = GetOrderId.MainOrderID;
                }
            }
            return orderIdNew;
        }
        /// <summary>
        /// 检查 约定期限 是否垫资超期
        /// </summary>
        private bool CheckDaysLimit(Layer.Data.Sqls.ScCustomsReponsitory reponsitory, string clientId, double DaysLimit, int feeType)
        {
            bool checkDaysLimit = false;
            decimal amount = 0;
            var orders = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var decHeads = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();
            var orderReceipts = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>();

            var decHeadView = (from order in orders
                               join decHead in decHeads on order.ID equals decHead.OrderID
                               where order.Status == (int)Enums.Status.Normal
                                  && order.ClientID == clientId
                                  && decHead.IsSuccess == true
                               select new DecHead
                               {
                                   ID = order.ID,
                                   DDate = decHead.DDate
                               }).ToList();
            if (decHeadView != null && decHeadView.Any())
            {
                var orderIds = decHeadView.Select(t => t.ID).ToArray();
                foreach (var Id in orderIds)
                {
                    var orderReceipt = orderReceipts.Where(item => item.OrderID == Id && item.Status == (int)Enums.Status.Normal).ToArray();
                    if (feeType == 2)
                    {
                        amount = orderReceipt.Where(item => item.FeeType == (int)OrderFeeType.Tariff || item.FeeType == (int)OrderFeeType.AddedValueTax || item.FeeType == (int)OrderFeeType.ExciseTax)
                                                                 .Sum(item => item.Amount * item.Rate);
                    }
                    else if (feeType == 3)
                    {
                        amount = orderReceipt.Where(item => item.FeeType == (int)OrderFeeType.AgencyFee).Sum(item => item.Amount * item.Rate);
                    }
                    else
                    {
                        amount = orderReceipt.Where(item => item.FeeType == (int)OrderFeeType.Incidental).Sum(item => item.Amount * item.Rate);
                    }
                    //var amount = orderReceipts.Where(t => t.OrderID == Id && t.FeeType == feeType && t.Status == (int)Enums.Status.Normal).Sum(t => t.Amount);
                    if (amount > 5)//存在欠款
                    {
                        //判断报关日期，约定期限和当前日期是否超期
                        var orderDDate = decHeadView.Where(t => t.ID == Id).Select(t => t.DDate).FirstOrDefault();
                        DateTime daysLimit = Convert.ToDateTime(orderDDate).AddDays(DaysLimit);
                        if (DateTime.Parse(daysLimit.ToString("yyyy-MM-dd")) < DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd")))//daysLimit小于当前日期，存在垫资超期
                        {
                            checkDaysLimit = true;
                            orderControlId = Id;
                        }
                    }
                    if (checkDaysLimit)
                    {
                        break;
                    }
                }
            }

            return checkDaysLimit == true ? true : false;
        }

        /// <summary>
        /// 检查 月结 是否垫资超期
        /// </summary>
        private bool CheckMonthlyDay(Layer.Data.Sqls.ScCustomsReponsitory reponsitory, string clientId, string monthlyDay, int feeType)
        {
            bool checkMonthlyDay = false;
            decimal amount = 0;
            var orders = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var decHeads = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();
            var orderReceipts = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>();

            var decHeadView = (from order in orders
                               join decHead in decHeads on order.ID equals decHead.OrderID
                               where order.Status == (int)Enums.Status.Normal
                                  && order.ClientID == clientId
                                  && decHead.IsSuccess == true
                               //&& decHead.DDate < Convert.ToDateTime(beforeMouthLastDay)
                               select new DecHead
                               {
                                   ID = order.ID,
                                   DDate = decHead.DDate
                               }).ToList();
            if (decHeadView != null && decHeadView.Any())
            {
                var orderIds = decHeadView.Select(t => t.ID).ToArray();
                foreach (var Id in orderIds)
                {
                    var orderReceipt = orderReceipts.Where(item => item.OrderID == Id && item.Status == (int)Enums.Status.Normal).ToArray();

                    if (feeType == 2)
                    {
                        amount = orderReceipt.Where(item => item.FeeType == (int)OrderFeeType.Tariff || item.FeeType == (int)OrderFeeType.AddedValueTax || item.FeeType == (int)OrderFeeType.ExciseTax)
                                                                 .Sum(item => item.Amount * item.Rate);
                    }
                    else if (feeType == 3)
                    {
                        amount = orderReceipt.Where(item => item.FeeType == (int)OrderFeeType.AgencyFee).Sum(item => item.Amount * item.Rate);
                    }
                    else
                    {
                        amount = orderReceipt.Where(item => item.FeeType == (int)OrderFeeType.Incidental).Sum(item => item.Amount * item.Rate);
                    }
                    if (amount > 5)//存在欠款
                    {
                        //判断报关日期，月结和当前日期是否超期
                        var orderDDate = decHeadView.Where(t => t.ID == Id).Select(t => t.DDate).FirstOrDefault();
                        var orderDDateNew = orderDDate.HasValue ? orderDDate.Value.ToString("yyyy-MM-dd") : DateTime.Now.ToString("yyyy-MM-dd");

                        var orderDDateNext = Convert.ToDateTime(orderDDateNew).AddMonths(1).ToString("yyyy-MM");

                        //20220125 ryan 修改月份天数不足30天报错的问题
                        var days = DateTime.DaysInMonth(Convert.ToDateTime(orderDDateNew).AddMonths(1).Year, Convert.ToDateTime(orderDDateNew).AddMonths(1).Month);
                        if (days < Convert.ToInt32(monthlyDay))
                        {
                            monthlyDay = days.ToString();
                        }

                        var taxMonthlyDay = DateTime.Parse(Convert.ToDateTime(orderDDateNext + "-" + monthlyDay).ToString("yyyy-MM-dd"));//月结还款日期

                        // beforeMouthLastDay = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddMonths(1).ToString("yyyy-MM-dd");

                        if (taxMonthlyDay < DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd")))//daysLimit小于当前日期，存在垫资超期
                        {
                            checkMonthlyDay = true;
                            orderControlId = Id;
                        }
                    }
                    if (checkMonthlyDay)
                    {
                        break;
                    }
                }
            }

            return checkMonthlyDay == true ? true : false;
        }

        /// <summary>
        /// 税费返回日期
        /// </summary>
        /// <returns></returns>
        public string TaxFeeDateTime(PeriodType taxPeriodType, int DaysLimit, int MonthlyDay, int feeType, string overdueOrderID)
        {
            string daysLimitDateTime = "";

            var taxDaysLimit = Convert.ToDouble(DaysLimit);
            var checkResultTax = CheckDaysLimitDateTime(taxPeriodType, TinyOrderID, taxDaysLimit, feeType, MonthlyDay, overdueOrderID);
            if (!string.IsNullOrEmpty(checkResultTax))
            {
                daysLimitDateTime = checkResultTax;
            }
            return daysLimitDateTime;
        }
        /// <summary>
        /// 返回超期订单号
        /// </summary>
        /// <returns></returns>
        public string OverOrderID()
        {
            string overOrderID = "";
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var overdueOrderID = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControls>().Where(item => item.OrderID == OrderID).Select(item => item.MainOrderID).FirstOrDefault();

                if (overdueOrderID != null)
                {
                    overOrderID = overdueOrderID;
                }
            }
            return overOrderID;
        }
        /// <summary>
        /// 检查 约定期限返回日期
        /// </summary>
        private string CheckDaysLimitDateTime(PeriodType taxPeriodType, string OrderID, double DaysLimit, int feeType, int MonthlyDay, string overdueOrderID)
        {
            var daysLimitDateTime = "";
            decimal amount = 0;
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var orders = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
                var decHeads = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();
                var orderReceipts = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>().Where(item => item.OrderID == overdueOrderID && item.Status == (int)Enums.Status.Normal).ToArray();

                var decHeadView = (from order in orders
                                   join decHead in decHeads on order.ID equals decHead.OrderID
                                   where order.Status == (int)Enums.Status.Normal
                                      && order.ID == overdueOrderID
                                      && decHead.IsSuccess == true
                                   select new DecHead
                                   {
                                       DDate = decHead.DDate
                                   }).FirstOrDefault();
                if (decHeadView != null)
                {
                    if (feeType == 2)
                    {
                        amount = orderReceipts.Where(item => item.FeeType == (int)OrderFeeType.Tariff || item.FeeType == (int)OrderFeeType.AddedValueTax || item.FeeType == (int)OrderFeeType.ExciseTax)
                                                                 .Sum(item => item.Amount * item.Rate);
                    }
                    else if (feeType == 3)
                    {
                        amount = orderReceipts.Where(item => item.FeeType == (int)OrderFeeType.AgencyFee).Sum(item => item.Amount * item.Rate);
                    }
                    else
                    {
                        amount = orderReceipts.Where(item => item.FeeType == (int)OrderFeeType.Incidental).Sum(item => item.Amount * item.Rate);
                    }
                    if (amount > 5)//存在欠款
                    {
                        //判断报关日期，约定期限和当前日期是否超期
                        var orderDDate = decHeadView.DDate?.ToShortDateString();
                        if (taxPeriodType == PeriodType.AgreedPeriod)
                        {
                            daysLimitDateTime = Convert.ToDateTime(orderDDate).AddDays(DaysLimit).ToShortDateString();
                        }
                        else
                        {
                            // daysLimitDateTime = Convert.ToDateTime(orderDDate).ToShortDateString();
                            var orderDDateNext = Convert.ToDateTime(orderDDate).AddMonths(1).ToString("yyyy-MM");
                            var monthlyDay = MonthlyDay < 10 ? "0" + MonthlyDay.ToString() : MonthlyDay.ToString();
                            var taxMonthlyDay = DateTime.Parse(Convert.ToDateTime(orderDDateNext + "-" + monthlyDay).ToString("yyyy-MM-dd"));//月结还款日期
                            if (taxMonthlyDay < Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")))
                            {//
                                daysLimitDateTime = taxMonthlyDay.ToShortDateString();
                            }
                        }

                    }

                }
                return daysLimitDateTime;
            }
        }

        /// <summary>
        /// 检查 税款/代理费/杂费  是否超出垫款上线
        /// </summary>
        /// <param name="reponsitory"></param>
        /// <param name="order"></param>
        /// <param name="unpaidFees"></param>
        /// <returns></returns>
        private bool CheckTaxFeeOverLimitBy5(Layer.Data.Sqls.ScCustomsReponsitory reponsitory, Order order, List<OrderReceipt> unpaidFees)
        {
            //垫款上限
            var taxFeeLimit = order.ClientAgreement.TaxFeeClause.UpperLimit.GetValueOrDefault();

            //客户的未付款/欠款
            var unpaidTaxFee = unpaidFees.Where(item => item.FeeType == OrderFeeType.Tariff || item.FeeType == OrderFeeType.AddedValueTax)
                                             .Sum(item => item.Amount * item.Rate);

            return (unpaidTaxFee - taxFeeLimit) <= 5 ? false : true;
        }

        /// <summary>
        /// 检查 代理费 是否超出垫款上线
        /// </summary>
        /// <param name="reponsitory"></param>
        /// <param name="order"></param>
        /// <param name="unpaidFees"></param>
        /// <returns></returns>
        private bool CheckAgencyFeeOverLimitBy5(Layer.Data.Sqls.ScCustomsReponsitory reponsitory, Order order, List<OrderReceipt> unpaidFees)
        {
            //垫款上限
            var agencyFeeLimit = order.ClientAgreement.AgencyFeeClause.UpperLimit.GetValueOrDefault();

            //客户的未付款/欠款
            var unpaidAgencyFee = unpaidFees.Where(item => item.FeeType == OrderFeeType.AgencyFee).Sum(item => item.Amount * item.Rate);

            return (unpaidAgencyFee - agencyFeeLimit) <= 5 ? false : true;
        }

        /// <summary>
        /// 检查 杂费 是否超出垫款上线
        /// </summary>
        /// <param name="reponsitory"></param>
        /// <param name="order"></param>
        /// <param name="unpaidFees"></param>
        /// <returns></returns>
        private bool CheckIncidentalFeeOverLimitBy5(Layer.Data.Sqls.ScCustomsReponsitory reponsitory, Order order, List<OrderReceipt> unpaidFees)
        {
            //垫款上限
            var incidentalFeeLimit = order.ClientAgreement.IncidentalFeeClause.UpperLimit.GetValueOrDefault();

            //客户的未付款/欠款
            var unpaidIncidentalFee = unpaidFees.Where(item => item.FeeType == OrderFeeType.Incidental).Sum(item => item.Amount * item.Rate);

            return (unpaidIncidentalFee - incidentalFeeLimit) <= 5 ? false : true;
        }




        /// <summary>
        /// 客户当前的税款是否超上限
        /// </summary>
        /// <param name="reponsitory"></param>
        /// <param name="agreement"></param>
        /// <param name="orderReceipts"></param>
        /// <returns></returns>
        private bool CheckTaxFeeOverLimitBy5Client(Layer.Data.Sqls.ScCustomsReponsitory reponsitory, ClientAgreement agreement, List<OrderReceipt> orderReceipts)
        {
            //垫款上限
            var taxFeeLimit = agreement.TaxFeeClause.UpperLimit.GetValueOrDefault();

            //客户的未付款/欠款
            var unpaidTaxFee = orderReceipts.Where(item => item.FeeType == OrderFeeType.Tariff || item.FeeType == OrderFeeType.AddedValueTax || item.FeeType == OrderFeeType.ExciseTax)
                                             .Sum(item => item.Amount * item.Rate);

            return (unpaidTaxFee - taxFeeLimit) <= 5 ? false : true;
        }

        /// <summary>
        /// 客户当前的代理费是否超上限
        /// </summary>
        /// <param name="reponsitory"></param>
        /// <param name="agreement"></param>
        /// <param name="orderReceipts"></param>
        /// <returns></returns>
        private bool CheckAgencyFeeOverLimitBy5Client(Layer.Data.Sqls.ScCustomsReponsitory reponsitory, ClientAgreement agreement, List<OrderReceipt> orderReceipts)
        {
            //垫款上限
            var agentFeeLimit = agreement.AgencyFeeClause.UpperLimit.GetValueOrDefault();

            //客户的未付款/欠款
            var unpaidAgentFee = orderReceipts.Where(item => item.FeeType == OrderFeeType.AgencyFee)
                                             .Sum(item => item.Amount * item.Rate);

            return (unpaidAgentFee - agentFeeLimit) <= 5 ? false : true;
        }

        /// <summary>
        /// 客户当前的杂费是否超上限
        /// </summary>
        /// <param name="reponsitory"></param>
        /// <param name="agreement"></param>
        /// <param name="orderReceipts"></param>
        /// <returns></returns>
        private bool CheckIncidentalFeeOverLimitBy5Client(Layer.Data.Sqls.ScCustomsReponsitory reponsitory, ClientAgreement agreement, List<OrderReceipt> orderReceipts)
        {
            //垫款上限
            var incidentalFeeLimit = agreement.IncidentalFeeClause.UpperLimit.GetValueOrDefault();

            //客户的未付款/欠款
            var unpaidIncidentalFee = orderReceipts.Where(item => item.FeeType == OrderFeeType.Incidental)
                                             .Sum(item => item.Amount * item.Rate);

            return (unpaidIncidentalFee - incidentalFeeLimit) <= 5 ? false : true;
        }

        /// <summary>
        /// 客户当前的垫资是否超上限
        /// </summary>
        /// <param name="reponsitory"></param>
        /// <param name="agreement"></param>
        /// <returns></returns>
        private bool CheckAdvanceMoneyByClient(Layer.Data.Sqls.ScCustomsReponsitory reponsitory, ClientAgreement agreement)
        {
            //垫款上限
            var advance = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdvanceMoneyApplies>().FirstOrDefault(t=>t.ClientID == agreement.ClientID && t.Status == (int)Enums.AdvanceMoneyStatus.Effective );

            if (advance != null && (advance.Amount < advance.AmountUsed))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
