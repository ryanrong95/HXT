using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 收款记录列表视图
    /// </summary>
    public class ReceiveRecordListView : QueryView<ReceiveRecordListViewModel, ScCustomsReponsitory>
    {
        public ReceiveRecordListView()
        {
        }

        protected ReceiveRecordListView(ScCustomsReponsitory reponsitory, IQueryable<ReceiveRecordListViewModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<ReceiveRecordListViewModel> GetIQueryable()
        {
            var orderReceipts = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>();
            var adminsView = new Views.AdminsTopView(this.Reponsitory);
            var financeReceipts = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceReceipts>();

            var iQuery = from orderReceipt in orderReceipts
                         join admin in adminsView on orderReceipt.AdminID equals admin.ID
                         join financeReceipt in financeReceipts on orderReceipt.FinanceReceiptID equals financeReceipt.ID into FinanceReceipts2
                         from financeReceipt in FinanceReceipts2.DefaultIfEmpty()
                         where orderReceipt.Type == (int)Enums.OrderReceiptType.Received
                            && orderReceipt.Status == (int)Enums.Status.Normal
                         group new { orderReceipt, admin, financeReceipt, } by new { orderReceipt.OrderID, orderReceipt.FeeType, } into g
                         let sameGroupFirst = g.Where(t => t.orderReceipt.OrderID == g.Key.OrderID && t.orderReceipt.FeeType == g.Key.FeeType)
                                                            .OrderByDescending(t => t.orderReceipt.CreateDate)
                                                            .FirstOrDefault()
                         select new ReceiveRecordListViewModel
                         {
                             //OrderReceiptID = orderReceipt.ID,
                             //CreateDate = orderReceipt.CreateDate,
                             //OrderID = orderReceipt.OrderID,
                             //AdminName = admin.RealName,
                             //FeeType = (Enums.OrderFeeType)orderReceipt.FeeType,
                             //FeeSourceID = orderReceipt.FeeSourceID,
                             //Type = (Enums.OrderPremiumType)orderReceipt.Type,
                             //Amount = orderReceipt.Amount,
                             //SeqNo = financeReceipt != null ? financeReceipt.SeqNo : "",
                             //ReceiptDate = financeReceipt != null ? (DateTime?)financeReceipt.ReceiptDate : null,


                             OrderReceiptID = sameGroupFirst.orderReceipt.ID,
                             CreateDate = sameGroupFirst.orderReceipt.CreateDate,
                             OrderID = g.Key.OrderID,
                             AdminName = sameGroupFirst.admin.RealName,
                             FeeType = (Enums.OrderFeeType)g.Key.FeeType,
                             FeeSourceID = sameGroupFirst.orderReceipt.FeeSourceID,
                             Type = (Enums.OrderPremiumType)sameGroupFirst.orderReceipt.Type,
                             Amount = sameGroupFirst.orderReceipt.Amount,
                             SeqNo = sameGroupFirst.financeReceipt != null ? sameGroupFirst.financeReceipt.SeqNo : "",
                             ReceiptDate = sameGroupFirst.financeReceipt != null ? (DateTime?)sameGroupFirst.financeReceipt.ReceiptDate : null,
                             // 2020-09-27 by yeshuangshuang
                             ReceiptAmount = sameGroupFirst.financeReceipt.Amount,
                             Payer = sameGroupFirst.financeReceipt.Payer,
                             FinanceReceiptID = sameGroupFirst.financeReceipt.ID
                         };

            return iQuery;
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<ReceiveRecordListViewModel> iquery = this.IQueryable.Cast<ReceiveRecordListViewModel>().OrderByDescending(item => item.CreateDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_myOrderReceipts = iquery.ToArray();

            var theBatchReceiptIDs = ienum_myOrderReceipts.Select(t => t.OrderReceiptID).ToArray();
            var theBatchOrderIDs = ienum_myOrderReceipts.Select(t => t.OrderID).ToArray();

            var ienums_linq = from orderReceipt in ienum_myOrderReceipts
                              select new ReceiveRecordListViewModel
                              {
                                  OrderReceiptID = orderReceipt.OrderReceiptID,
                                  CreateDate = orderReceipt.CreateDate,
                                  OrderID = orderReceipt.OrderID,
                                  AdminName = orderReceipt.AdminName,
                                  FeeType = orderReceipt.FeeType,
                                  FeeSourceID = orderReceipt.FeeSourceID,
                                  Type = orderReceipt.Type,
                                  Amount = orderReceipt.Amount,
                                  SeqNo = orderReceipt.SeqNo,
                                  ReceiptDate = orderReceipt.ReceiptDate,
                                  // 2020-09-27 by yeshuangshuang
                                  ReceiptAmount = orderReceipt.ReceiptAmount,
                                  Payer = orderReceipt.Payer,
                                  FinanceReceiptID = orderReceipt.FinanceReceiptID
                              };

            var results = ienums_linq.ToList();

            #region 带上这一页数据中，同订单号、同费用类型的其它数据

            var orderReceipts = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>();
            var adminsView = new Views.AdminsTopView(this.Reponsitory);
            var financeReceipts = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceReceipts>();

            var linq_otherDatas = from orderReceipt in orderReceipts
                                  join admin in adminsView on orderReceipt.AdminID equals admin.ID
                                  join financeReceipt in financeReceipts on orderReceipt.FinanceReceiptID equals financeReceipt.ID into FinanceReceipts2
                                  from financeReceipt in FinanceReceipts2.DefaultIfEmpty()
                                  where orderReceipt.Type == (int)Enums.OrderReceiptType.Received
                                     && orderReceipt.Status == (int)Enums.Status.Normal
                                     && theBatchOrderIDs.Contains(orderReceipt.OrderID)
                                     && !theBatchReceiptIDs.Contains(orderReceipt.ID)
                                  select new ReceiveRecordListViewModel
                                  {
                                      OrderReceiptID = orderReceipt.ID,
                                      CreateDate = orderReceipt.CreateDate,
                                      OrderID = orderReceipt.OrderID,
                                      AdminName = admin.RealName,
                                      FeeType = (Enums.OrderFeeType)orderReceipt.FeeType,
                                      FeeSourceID = orderReceipt.FeeSourceID,
                                      Type = (Enums.OrderPremiumType)orderReceipt.Type,
                                      Amount = orderReceipt.Amount,
                                      SeqNo = financeReceipt != null ? financeReceipt.SeqNo : "",
                                      ReceiptDate = financeReceipt != null ? (DateTime?)financeReceipt.ReceiptDate : null,
                                      // 2020-09-27 by yeshuangshuang
                                      ReceiptAmount = financeReceipt.Amount,
                                      Payer = financeReceipt.Payer
                                  };

            var ienums_otherDatas = linq_otherDatas.ToArray();

            List<ReceiveRecordListViewModel> realResults = new List<ReceiveRecordListViewModel>();

            for (int i = 0; i < results.Count; i++)
            {
                realResults.Add(results[i]);

                var relationDatas = ienums_otherDatas.Where(t => t.OrderID == results[i].OrderID
                                                              && t.FeeType == results[i].FeeType)
                                                     .OrderByDescending(t => t.CreateDate).ToList();

                if (relationDatas != null && relationDatas.Any())
                {
                    realResults.AddRange(relationDatas);
                }
            }

            #endregion

            Func<ReceiveRecordListViewModel, object> convert = item => new
            {
                OrderReceiptID = item.OrderReceiptID,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                AdminName = item.AdminName,
                item.OrderID,
                FeeTypeInt = (int)item.FeeType,
                FeeTypeName = item.FeeTypeShowName,
                ReceiptTypeName = (0 - item.Amount) > 0 ? "收款" : "冲正",
                Amount = (0 - item.Amount).ToString("#0.00"),
                SeqNo = item.SeqNo,
                ReceiptDate = item.ReceiptDate?.ToString("yyyy-MM-dd"),
                // 2020-09-27 by yeshuangshuang
                ReceiptAmount = item.ReceiptAmount,
                Payer = item.Payer,
                FinanceReceiptID = item.FinanceReceiptID
            };

            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                return realResults.Select(convert).ToArray();
            }
                       

            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = realResults.Select(convert).ToArray(),
            };
        }

        /// <summary>
        /// 根据收款日期开始时间查询
        /// </summary>
        /// <param name="begin"></param>
        /// <returns></returns>
        public ReceiveRecordListView SearchByCreateDateBegin(DateTime begin)
        {
            var linq = from query in this.IQueryable
                       where query.CreateDate >= begin
                       select query;

            var view = new ReceiveRecordListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据收款日期结束时间查询
        /// </summary>
        /// <param name="end"></param>
        /// <returns></returns>
        public ReceiveRecordListView SearchByCreateDateEnd(DateTime end)
        {
            var linq = from query in this.IQueryable
                       where query.CreateDate < end
                       select query;

            var view = new ReceiveRecordListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据订单编号查询
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public ReceiveRecordListView SearchByOrderID(string orderID)
        {
            var linq = from query in this.IQueryable
                       where query.OrderID.Contains(orderID)
                       select query;

            var view = new ReceiveRecordListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据费用类型查询
        /// </summary>
        /// <param name="orderFeeType"></param>
        /// <returns></returns>
        public ReceiveRecordListView SearchByFeeType(Enums.OrderFeeType orderFeeType)
        {
            var linq = from query in this.IQueryable
                       where query.FeeType == orderFeeType
                       select query;

            var view = new ReceiveRecordListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据银行流水收款日期开始时间查询
        /// </summary>
        /// <param name="begin"></param>
        /// <returns></returns>
        public ReceiveRecordListView SearchByReceiptDateBegin(DateTime begin)
        {
            var linq = from query in this.IQueryable
                       where query.ReceiptDate >= begin
                       select query;

            var view = new ReceiveRecordListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据银行流水收款日期结束时间查询
        /// </summary>
        /// <param name="end"></param>
        /// <returns></returns>
        public ReceiveRecordListView SearchByReceiptDateEnd(DateTime end)
        {
            var linq = from query in this.IQueryable
                       where query.ReceiptDate < end
                       select query;

            var view = new ReceiveRecordListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据银行流水号查询
        /// </summary>
        /// <param name="seqNo"></param>
        /// <returns></returns>
        public ReceiveRecordListView SearchBySeqNo(string seqNo)
        {
            var linq = from query in this.IQueryable
                       where query.SeqNo.Contains(seqNo)
                       select query;

            var view = new ReceiveRecordListView(this.Reponsitory, linq);
            return view;
        }

    }

    public class ReceiveRecordListViewModel
    {
        /// <summary>
        /// OrderReceiptID
        /// </summary>
        public string OrderReceiptID { get; set; }

        /// <summary>
        /// 收款时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 收款人
        /// </summary>
        public string AdminName { get; set; }

        /// <summary>
        /// 订单收款费用类型
        /// </summary>
        public Enums.OrderFeeType FeeType { get; set; }

        /// <summary>
        /// 费用来源
        /// </summary>
        public string FeeSourceID { get; set; }

        /// <summary>
        /// 费用类型
        /// 代理费、商检费、送货费、快递费、清关费、提货费 、停车费... ... 杂费
        /// </summary>
        public Enums.OrderPremiumType Type { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 对应的一比收款的银行流水号
        /// </summary>
        public string SeqNo { get; set; } = string.Empty;

        /// <summary>
        /// 对应的一比收款的银行收款日期
        /// </summary>
        public DateTime? ReceiptDate { get; set; }


        /// <summary>
        /// 新增显示财务收款记录ID by ryan 20220418  luxiaoling
        /// </summary>
        public string FinanceReceiptID { get; set; }



        /// <summary>
        /// 费用类型显示名称
        /// </summary>
        public string FeeTypeShowName
        {
            get
            {
                if (this.FeeType == Enums.OrderFeeType.Incidental)
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
                    return this.FeeType.GetDescription();
                }
            }
        }

        #region 增加列 2020-09-27 by  yeshuangshuang

        /// <summary>
        /// 收款金额 
        /// </summary>
        public decimal ReceiptAmount { get; set; }

        /// <summary>
        /// 客户名称（收款人）
        /// </summary>
        public string Payer { get; set; }

        #endregion


    }

}
