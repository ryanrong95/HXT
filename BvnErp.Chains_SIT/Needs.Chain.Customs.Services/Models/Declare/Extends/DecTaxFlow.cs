using Needs.Ccs.Services.Enums;
using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 缴税报关单
    /// </summary>
    public class DecTax : DecHead
    {
        //开票类型
        public Enums.InvoiceType InvoiceType { get; set; }

        public Enums.DecTaxStatus DecTaxStatus { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public int IsUpload { get; set; }

        public Status Status { get; set; }

        public string ClientID { get; set; }

        public string UserID { get; set; }

        public string Summary { get; set; }

        //成交币种
        public string Currency { get; set; }

        public decimal? DecAmount { get; set; }

        /// <summary>
        /// 订单中的委托报关价格(外币)
        /// </summary>
        public decimal? OrderAgentAmount { get; set; }

        /// <summary>
        /// 报关单关税值
        /// </summary>
        public decimal? TariffValue { get; set; }

        /// <summary>
        /// 报关单消费税值
        /// </summary>
        public decimal? ExciseTaxValue { get; set; }

        /// <summary>
        /// 报关单增值税值
        /// </summary>
        public decimal? AddedValue { get; set; }


        public Enums.UploadStatus UploadStatus { get; set; }

        /// <summary>
        /// 处理类型(复合枚举)
        /// </summary>
        public int? HandledType { get; set; }

        /// <summary>
        /// 消费使用单位
        /// </summary>
        public string ConsumeName { get; set; }

        /// <summary>
        ///入库数据是否已经推送到大赢家（新增字段 addby jss 2022-10-08）
        /// </summary>
        public int IsPutInSto { get; set; }

        /// <summary>
        /// 报关单文件
        /// </summary>
        public IEnumerable<DecHeadFile> files { get; set; }

        public string StatusName
        {
            get
            {
                return MultiEnumUtils.ToText<Enums.CusDecStatus>(this.CusDecStatus);
            }
        }

        /// <summary>
        /// 关税发票
        /// </summary>
        public DecHeadFile TariffFile
        {
            get { return this.files.Where(item => item.FileType == Enums.FileType.DecHeadTariffFile).FirstOrDefault(); }
        }

        /// <summary>
        /// 消费税发票
        /// </summary>
        public DecHeadFile ExciseTaxFile
        {
            get { return this.files.Where(item => item.FileType == Enums.FileType.DecHeadExciseTaxFile).FirstOrDefault(); }
        }

        /// <summary>
        /// 增值税发票
        /// </summary>
        public DecHeadFile VatFile
        {
            get { return this.files.Where(item => item.FileType == Enums.FileType.DecHeadVatFile).FirstOrDefault(); }
        }

        /// <summary>
        /// 报关单文件
        /// </summary>
        public DecHeadFile DecFile
        {
            get { return this.files.Where(item => item.FileType == Enums.FileType.DecHeadFile).FirstOrDefault(); }
        }

        /// <summary>
        /// 报关单缴税流水
        /// </summary>
        public IEnumerable<DecTaxFlow> flows { get; set; }
        /// <summary>
        /// 关税流水
        /// </summary>
        public DecTaxFlow TariffFlow
        {
            get
            {
                return this.flows.Where(item => item.TaxType == Enums.DecTaxType.Tariff).FirstOrDefault();
            }
        }
        /// <summary>
        /// 消费税流水
        /// </summary>
        public DecTaxFlow ExciseTaxFlow
        {
            get
            {
                return this.flows.Where(item => item.TaxType == Enums.DecTaxType.ExciseTax).FirstOrDefault();
            }
        }
        /// <summary>
        /// 增值税流水
        /// </summary>
        public DecTaxFlow VatfFlow
        {
            get
            {
                return this.flows.Where(item => item.TaxType == Enums.DecTaxType.AddedValueTax).FirstOrDefault();
            }
        }

        public DecTax()
        {
            this.DecTaxStatus = Enums.DecTaxStatus.Unpaid;
            this.UploadStatus = Enums.UploadStatus.NotUpload;
            this.CreateDate = this.UpdateDate = DateTime.Now;
            this.IsPutInSto = 0;
        }

        public new void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxs>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.DecTaxs
                    {
                        ID = this.ID,
                        InvoiceType = (int)this.InvoiceType,
                        Status = (int)this.DecTaxStatus,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        IsUpload = (int)this.UploadStatus,
                        Summary = this.Summary,
                        IsPutInSto = this.IsPutInSto
                    });
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecTaxs>(new
                    {
                        InvoiceType = (int)this.InvoiceType,
                        Status = (int)this.DecTaxStatus,
                        IsUpload = (int)this.UploadStatus,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary,
                    }, item => item.ID == this.ID);
                }
                this.OnEnter();
            }
        }

        /// <summary>
        /// 设置为已上传缴费流水
        /// 输入参数
        /// 1. DecHeadID(ID) 2. OrderID
        /// </summary>
        public void SetNoDecTaxFlow(int targetHandledType)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //1. 修改 DecTaxs 中 HandledType 字段，使用或
                //2. 设置 DecTaxs 中 IsUpload 字段为已上传
                var decTax = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxs>().Where(t => t.ID == this.ID).FirstOrDefault();
                if (decTax != null)
                {
                    if ((decTax.HandledType | targetHandledType).HasValue)//判断是否为空
                    {

                        int resultHandledType = (int)(decTax.HandledType | targetHandledType);
                        if (targetHandledType == (int)Needs.Ccs.Services.Enums.HandledType.AddedValueTax)
                        {
                            reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecTaxs>(new
                            {
                                Status = DecTaxStatus.NoTax,
                                HandledType = resultHandledType,
                                IsUpload = (int)Enums.UploadStatus.Uploaded,
                            }, item => item.ID == this.ID);
                        }
                        else
                        {
                            reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecTaxs>(new
                            {
                                IsUpload = (int)Enums.UploadStatus.Uploaded,
                                HandledType = resultHandledType,
                            }, item => item.ID == this.ID);

                        }



                        //5. 如果  既无关税、也无增值流水，向 APInotices表插一条记录
                        NoFlowsInsertAPInotice(reponsitory, resultHandledType, decTax.ID, this.ID);
                    }
                }

                //3. 查出该 OrderID 所对应的所有的 OrderItemID
                string[] orderItemIDs = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>()
                    .Where(t => t.OrderID == this.OrderID && t.Status == (int)Enums.Status.Normal)
                    .Select(t => t.ID).ToArray();

                //4. 设置该订单下对应类型的 OrderItemTaxes 中的 ReceiptRate 字段为 0

                int targetOrderItemTaxesType = 0;
                if (targetHandledType == Enums.HandledType.Tariff.GetHashCode())
                {
                    targetOrderItemTaxesType = Enums.CustomsRateType.ImportTax.GetHashCode();
                    //如果没有关税，更新declist中的完税价格
                    DTaxedPrice(reponsitory);
                }
                else if (targetHandledType == Enums.HandledType.AddedValueTax.GetHashCode())
                {
                    targetOrderItemTaxesType = Enums.CustomsRateType.AddedValueTax.GetHashCode();
                }
                else if (targetHandledType == Enums.HandledType.ExciseTax.GetHashCode())
                {
                    targetOrderItemTaxesType = Enums.CustomsRateType.ConsumeTax.GetHashCode();
                }

                reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>(new { ReceiptRate = (decimal)0, },
                    item => orderItemIDs.Contains(item.OrderItemID) && item.Type == targetOrderItemTaxesType);
            }
        }

        /// <summary>
        /// 5. 如果  既无关税、也无增值流水，向 APInotices表插一条记录
        /// </summary>
        private void NoFlowsInsertAPInotice(
            Layer.Data.Sqls.ScCustomsReponsitory reponsitory,
            int resultHandledType,
            string decTaxID,
            string decHeadID)
        {
            if (resultHandledType == (Needs.Ccs.Services.Enums.HandledType.Tariff.GetHashCode()
                                    | Needs.Ccs.Services.Enums.HandledType.AddedValueTax.GetHashCode()
                                    | Needs.Ccs.Services.Enums.HandledType.ExciseTax.GetHashCode()))
            {
                int decTaxFlowCount = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxFlows>()
                    .Where(t => t.DecTaxID == decTaxID).Count();
                if (decTaxFlowCount <= 0)
                {
                    var order = new Needs.Ccs.Services.Views.Origins.OrdersOrigin()[this.OrderID];
                    var orderChangeCount = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderChangeNotices>().Count(x => x.OderID == this.OrderID);
                    if (order.Type != Enums.OrderType.Outside && orderChangeCount == 0)
                    {
                        int apiNoticeCount = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ApiNotices>().Count(x => x.ItemID == decHeadID);
                        if (apiNoticeCount == 0)
                        {
                            reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ApiNotices
                            {
                                ID = ChainsGuid.NewGuidUp(),
                                PushType = (int)Enums.PushType.DutiablePrice,
                                PushStatus = (int)Enums.PushStatus.Unpush,
                                ItemID = decHeadID,
                                ClientID = order.ClientID,
                                CreateDate = DateTime.Now,
                                UpdateDate = DateTime.Now
                            });

                        }
                    }
                }
            }

        }

        /// <summary>
        /// 如果没有关税，设置declist的完税价格
        /// </summary>
        public void DTaxedPrice(Layer.Data.Sqls.ScCustomsReponsitory reponsitory)
        {
            var DecHead = new Ccs.Services.Views.DecHeadsView().Where(t => t.ID == this.ID).FirstOrDefault();
            var DecLists = new Ccs.Services.Views.DecOriginListsView().Where(t => t.DeclarationID == this.ID).ToList();
            var orderCus = new Ccs.Services.Views.Orders2View().Where(t => t.ID == DecHead.OrderID).FirstOrDefault();
            var orderitems = new Needs.Ccs.Services.Views.OrdersView()[DecHead.OrderID].Items.ToList();
            var remainDeci = DecHead.isTwoStep ? 2 : 0;
            foreach (var item in DecLists)
            {
                decimal taxedPrice = Math.Round(item.DeclTotal * orderCus.CustomsExchangeRate.Value, remainDeci, MidpointRounding.AwayFromZero);
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecLists>(new { TaxedPrice = taxedPrice }, t => t.ID == item.ID);
            }
        }

    }

    /// <summary>
    /// 已缴税未抵扣的报关单
    /// </summary>
    public class UnDeductionDecTax : DecTax
    {
        private DateTime DeductionTime;

        private string[] IdList;

        public UnDeductionDecTax(string[] IdList, DateTime DeductionTime)
        {
            this.IdList = IdList;
            this.DeductionTime = DeductionTime;
        }

        //抵扣
        public void Deduction()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecTaxs>(new
                {
                    Status = (int)Enums.DecTaxStatus.Deducted,
                    UpdateDate = DateTime.Now,
                }, item => this.IdList.Contains(item.ID));

                reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecTaxFlows>(new
                {
                    Status = (int)Enums.DecTaxStatus.Deducted,
                    DeductionTime = this.DeductionTime,
                    UpdateDate = DateTime.Now,
                }, item => this.IdList.Contains(item.DecTaxID) && item.TaxType == (int)Enums.DecTaxType.AddedValueTax);
            }
        }
    }

    public class UnDeductionDecTax2
    {
        private string _taxNumber;

        private decimal _vaildAmount;

        private DateTime _deductionTime;

        private DateTime _deductionMonth;

        public UnDeductionDecTax2(string taxNumber, decimal vaildAmount, DateTime deductionTime, DateTime deductionMonth)
        {
            this._taxNumber = taxNumber;
            this._vaildAmount = vaildAmount;
            this._deductionTime = deductionTime;
            this._deductionMonth = deductionMonth;
        }

        public System.Tuple<bool, string> Deduction()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var decTaxFlows = reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.DecTaxFlows>();
                var theDecTaxFlow = decTaxFlows.Where(t => t.TaxNumber == this._taxNumber
                                                        && t.TaxType == (int)Enums.DecTaxType.AddedValueTax).FirstOrDefault();
                if (theDecTaxFlow == null)
                {
                    return new Tuple<bool, string>(false, "根据海关缴款书号码 " + this._taxNumber + " 查询不到增值税流水记录");
                }

                reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecTaxFlows>(new
                {
                    VaildAmount = this._vaildAmount,
                    DeductionTime = this._deductionTime,
                    DeductionMonth = this._deductionMonth,
                    Status = (int)Enums.DecTaxStatus.Deducted,
                    UpdateDate = DateTime.Now,
                }, item => item.ID == theDecTaxFlow.ID);

                reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecTaxs>(new
                {
                    Status = (int)Enums.DecTaxStatus.Deducted,
                    UpdateDate = DateTime.Now,
                }, item => item.ID == theDecTaxFlow.DecTaxID);

                return new Tuple<bool, string>(true, "");
            }
        }
    }

    /// <summary>
    /// 报关单缴税流水
    /// </summary>
    public class DecTaxFlow : IUnique, IPersist
    {
        #region 属性

        private string id;
        public string ID
        {
            get
            {
                return this.id ?? string.Concat(this.DecheadID, this.TaxType).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        public string DecTaxID { get; set; }

        public string DecheadID { get; set; }

        /// <summary>
        /// 合同编号
        /// </summary>
        public string ContrNo { get; set; }

        //税费单号
        public string TaxNumber { get; set; }

        public Enums.DecTaxType TaxType { get; set; }
        //支付金额
        public decimal Amount { get; set; }
        //银行扣税时间
        public DateTime? PayDate { get; set; }

        public string BankName { get; set; }

        public DateTime? DeductionTime { get; set; }

        //填发日期
        public DateTime? FillinDate { get; set; }

        public Enums.DecTaxStatus Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        public string OrderID { get; set; }

        /// <summary>
        /// 报关是否上传流水
        /// </summary>
        //public Enums.UploadStatus IsUpload { get; set; }
        /// <summary>
        /// 报关单号
        /// </summary>

        public string EntryID { get; set; }

        /// <summary>
        /// DecTax 的 HandledType
        /// </summary>
        public int? DecTaxHandledType { get; set; }

        #endregion

        #region 事件

        public event SuccessHanlder EnterSuccess;

        private void DecTaxFlow_EnterSuccess(object sender, SuccessEventArgs e)
        {
            var flow = (DecTaxFlow)e.Object;
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //财务上传流水 ，修改付款日期和缴税状态；
                if (flow.PayDate != null)
                {
                    var head = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>().FirstOrDefault(t => t.ID == flow.DecheadID);
                    var gua = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxGuarantees>().FirstOrDefault(t => t.ID == head.GuaranteeNo);

                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecTaxs>(new
                    {
                        Status = flow.Status,
                        IsUpload = Enums.UploadStatus.Uploaded,
                        UpdateDate = DateTime.Now,
                    }, item => item.ID == flow.DecheadID);

                    //财务上传缴税流水后，海关税费额度预警表， 缴费状态改为1；
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecTaxQuotas>(new
                    {
                        PayStatus = TaxStatus.Paid,
                        UpdateDate = DateTime.Now,
                    }, item => item.DeclarationID == flow.DecheadID);

                    //保函额度增加
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecTaxGuarantees>(new
                    {
                        RemainAmount = gua.RemainAmount + flow.Amount
                    }, item => item.ID == gua.ID);
                }
                else
                {
                    var currentDecTax = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxs>().Where(t => t.ID == flow.DecheadID).FirstOrDefault();
                    if ((currentDecTax.HandledType).HasValue)
                    {
                        int targetDexTaxHandledType = (int)currentDecTax.HandledType;

                        if (flow.TaxType == Enums.DecTaxType.AddedValueTax)
                        {
                            targetDexTaxHandledType = targetDexTaxHandledType | Enums.HandledType.AddedValueTax.GetHashCode();
                        }
                        else if (flow.TaxType == Enums.DecTaxType.ExciseTax)
                        {
                            targetDexTaxHandledType = targetDexTaxHandledType | Enums.HandledType.ExciseTax.GetHashCode();
                        }
                        else if (flow.TaxType == Enums.DecTaxType.Tariff)
                        {
                            targetDexTaxHandledType = targetDexTaxHandledType | Enums.HandledType.Tariff.GetHashCode();
                        }


                        //报关员上传流水 ，产生订单变更通知
                        //流水上传成功之后 ，先更新decTax 状态，将单子置为已上传；
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecTaxs>(new
                        {
                            Status = flow.Status,
                            IsUpload = Enums.UploadStatus.Uploaded,
                            UpdateDate = DateTime.Now,
                            HandledType = targetDexTaxHandledType,
                        }, item => item.ID == flow.DecheadID);


                        var order = new Needs.Ccs.Services.Views.Origins.OrdersOrigin()[this.OrderID];

                        //客户账单汇率使用实时汇率的，跳过税费异常 20230628 ryan 
                        var agreement = new Views.ClientAgreementsView().FirstOrDefault(t => t.ClientID == order.ClientID && t.Status == Enums.Status.Normal);
                        if (agreement != null && agreement.TaxFeeClause.ExchangeRateType != ExchangeRateType.Custom)
                        {
                            return;
                        }


                        //订单变更的插入逻辑
                        int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderChangeNotices>().Count(item => item.OderID == this.OrderID && item.Type == (int)Enums.OrderChangeType.TaxChange);
                        // 增值税
                        if (flow.TaxType == Enums.DecTaxType.AddedValueTax && flow.EntryID != null)
                        {

                            var AddedValueTax = new Needs.Ccs.Services.Views.OrdersView()[this.OrderID].Items.Sum(x => x.AddedValueTax.Value);
                            //流水中实际缴纳的增值税是否与订单一致
                            if (AddedValueTax != flow.Amount)
                            {
                                if (count == 0)
                                {
                                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.OrderChangeNotices
                                    {
                                        ID = ChainsGuid.NewGuidUp(),
                                        OderID = this.OrderID,
                                        Type = (int)Enums.OrderChangeType.TaxChange,
                                        ProcessState = (int)Enums.ProcessState.UnProcess,
                                        Status = (int)Enums.Status.Normal,
                                        CreateDate = DateTime.Now,
                                        UpdateDate = DateTime.Now,
                                    });
                                }
                                else
                                {
                                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderChangeNotices>(new { UpdateDate = DateTime.Now }, item => item.OderID == this.OrderID && item.Type == (int)Enums.OrderChangeType.TaxChange);

                                }
                                NoticeLog noticeLog = new NoticeLog();
                                noticeLog.MainID = this.OrderID;
                                noticeLog.NoticeType = SendNoticeType.TaxError;
                                noticeLog.SendNotice();
                            }
                        }
                        //关税 
                        if (flow.TaxType == Enums.DecTaxType.Tariff && flow.EntryID != null)
                        {
                            // 流水中实际缴纳的关税是否与订单一致
                            var ImportTax = new Needs.Ccs.Services.Views.OrdersView()[this.OrderID].Items.Sum(x => x.ImportTax.Value);
                            if (ImportTax != flow.Amount)
                            {
                                if (count == 0)
                                {
                                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.OrderChangeNotices
                                    {
                                        ID = ChainsGuid.NewGuidUp(),
                                        OderID = this.OrderID,
                                        Type = (int)Enums.OrderChangeType.TaxChange,
                                        ProcessState = (int)Enums.ProcessState.UnProcess,
                                        Status = (int)Enums.Status.Normal,
                                        CreateDate = DateTime.Now,
                                        UpdateDate = DateTime.Now,
                                    });
                                }
                                else
                                {
                                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderChangeNotices>(new { UpdateDate = DateTime.Now }, item => item.OderID == this.OrderID && item.Type == (int)Enums.OrderChangeType.TaxChange);
                                }
                                NoticeLog noticeLog = new NoticeLog();
                                noticeLog.MainID = this.OrderID;
                                noticeLog.NoticeType = SendNoticeType.TaxError;
                                noticeLog.SendNotice();
                            }
                            else
                            {
                                Task.Run(() =>
                                {
                                    //上传无关税异常，更新DecList里的完税价格。
                                    var DecHead = new Ccs.Services.Views.DecHeadsView().Where(t => t.ID == flow.DecheadID).FirstOrDefault();
                                    var DecLists = new Ccs.Services.Views.DecOriginListsView().Where(t => t.DeclarationID == flow.DecheadID).ToList();
                                    var orderCus = new Ccs.Services.Views.Orders2View().Where(t => t.ID == DecHead.OrderID).FirstOrDefault();
                                    var orderitems = new Needs.Ccs.Services.Views.OrdersView()[DecHead.OrderID].Items.ToList();
                                    var remainDeci = DecHead.isTwoStep ? 2 : 0;
                                    foreach (var item in DecLists)
                                    {
                                        decimal taxedPrice = Math.Round(item.DeclTotal * orderCus.CustomsExchangeRate.Value, remainDeci, MidpointRounding.AwayFromZero);
                                        decimal tariff = orderitems.Where(t => t.ID == item.OrderItemID).FirstOrDefault().ImportTax.Value.Value;
                                        taxedPrice += tariff;
                                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecLists>(new { TaxedPrice = taxedPrice }, t => t.ID == item.ID);
                                    }
                                });
                            }
                        }

                        
                        var orderChangeCount = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderChangeNotices>().Count(x => x.OderID == this.OrderID && x.Type == (int)Enums.OrderChangeType.TaxChange);
                        if (order.Type != Enums.OrderType.Outside && orderChangeCount == 0)
                        {
                            int apiNoticeCount = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ApiNotices>().Count(x => x.ItemID == flow.DecheadID);
                            if (apiNoticeCount == 0)
                            {
                                reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ApiNotices
                                {
                                    ID = ChainsGuid.NewGuidUp(),
                                    PushType = (int)Enums.PushType.DutiablePrice,
                                    PushStatus = (int)Enums.PushStatus.Unpush,
                                    ItemID = flow.DecheadID,
                                    ClientID = order.ClientID,
                                    CreateDate = DateTime.Now,
                                    UpdateDate = DateTime.Now
                                });
                            }
                        }

                        //外部公司插入apinotice,推送进价
                        //if (order.Type != Enums.OrderType.Inside)
                        //{
                        //    int apiNoticeCount = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ApiNotices>().Count(x => x.ItemID == flow.DecheadID && x.PushType == (int)Enums.PushType.PurchasePrice);
                        //    if (apiNoticeCount == 0)
                        //    {
                        //        reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ApiNotices
                        //        {
                        //            ID = ChainsGuid.NewGuidUp(),
                        //            PushType = (int)Enums.PushType.PurchasePrice,
                        //            PushStatus = (int)Enums.PushStatus.Unpush,
                        //            ItemID = flow.DecheadID,
                        //            ClientID = "",
                        //            CreateDate = DateTime.Now,
                        //            UpdateDate = DateTime.Now
                        //        });
                        //    }
                        //}


                    }
                }
            }

        }

        #endregion

        public DecTaxFlow()
        {
            this.Status = Enums.DecTaxStatus.Unpaid;
            this.CreateDate = this.UpdateDate = DateTime.Now;
            //this.IsUpload = Enums.UploadStatus.NotUpload;
            this.EnterSuccess += DecTaxFlow_EnterSuccess;
        }

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxFlows>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecTaxFlows>(new
                    {
                        Status = this.Status,
                        PayDate = this.PayDate,
                        BankName = this.BankName

                    }, item => item.ID == this.ID);

                    //reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecLists>(new
                    //{
                    //    UpdateDate = DateTime.Now,
                    //}, item => item.ID == this.ID);
                }

                this.OnEnter();

            }
        }

        /// <summary>
        /// Enter后发生
        /// </summary>
        virtual protected void OnEnter()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }
    }

    /// <summary>
    /// 缴费流水扩展属性（用于客户端缴费流水）
    /// </summary>
    public class DecTaxFlowForUser : DecTaxFlow
    {
        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 开票类型
        /// </summary>
        public InvoiceType InvoiceType { get; set; }

        /// <summary>
        /// 报关单文件
        /// </summary>
        public IEnumerable<DecHeadFile> files { get; set; }

        /// <summary>
        /// 关税发票/增值税发票
        /// </summary>
        public DecHeadFile InvoiceFile
        {
            get
            {
                if (this.TaxType == DecTaxType.AddedValueTax)
                {
                    return this.files.Where(item => item.FileType == Enums.FileType.DecHeadVatFile).FirstOrDefault();
                }
                else
                {
                    return this.files.Where(item => item.FileType == Enums.FileType.DecHeadTariffFile).FirstOrDefault();
                }
            }
        }
    }
}
