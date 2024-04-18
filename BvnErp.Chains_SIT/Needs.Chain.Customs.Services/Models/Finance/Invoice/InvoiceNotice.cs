using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Views;
using Needs.Ccs.Services.Views.Origins;
using Needs.Linq;
using Needs.Utils.Converters;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 开票通知
    /// </summary>
    public class InvoiceNotice : IUnique, IPersistence, IFulError, IFulSuccess
    {
        #region 属性
        public string ID { get; set; }

        /// <summary>
        /// 申请人
        /// </summary>
        public string ApplyID { get; set; }
        public Admin Apply { get; set; }

        /// <summary>
        /// 开票人
        /// </summary>
        public string AdminID { get; set; }
        public Admin Admin { get; set; }

        /// <summary>
        /// 客户
        /// </summary>
        public string ClientID { get; set; }
        public Client Client { get; set; }

        /// <summary>
        /// 客户的发票信息
        /// </summary>
        public string ClientInvoiceID { get; set; }
        public ClientInvoice ClientInvoice { get; set; }

        /// <summary>
        /// 开票类型
        /// </summary>
        public Enums.InvoiceType InvoiceType { get; set; }

        /// <summary>
        /// 开票税率
        /// </summary>
        public decimal InvoiceTaxRate { get; set; }

        /// <summary>
        /// 客户地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 客户联系电话
        /// </summary>
        public string Tel { get; set; }

        /// <summary>
        /// 客户开户行名称
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// 客户的银行账号
        /// </summary>
        public string BankAccount { get; set; }

        /// <summary>
        /// 发票交付方式
        /// </summary>
        public Enums.InvoiceDeliveryType DeliveryType { get; set; }

        /// <summary>
        /// 发票收件人名称
        /// </summary>
        public string MailName { get; set; }

        /// <summary>
        /// 发票收件人电话
        /// </summary>
        public string MailMobile { get; set; }

        /// <summary>
        /// 发票收件人地址
        /// </summary>
        public string MailAddress { get; set; }

        /// <summary>
        /// 开票通知状态
        /// </summary>
        public Enums.InvoiceNoticeStatus InvoiceNoticeStatus { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        /// <summary>
        /// 含税总金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 差额总和
        /// </summary>
        public decimal Difference { get; set; }

        public decimal TotalQty
        {
            get
            {
                return this.items.Where(x => x.OrderItem != null).Sum(x => x.OrderItem.Quantity);
            }
        }
        /// <summary>
        /// 差额合计
        /// </summary>
        public decimal TotalDif
        {
            get
            {
                return this.items.Where(x => x.OrderItem != null).Sum(x => x.Difference);
            }
        }

        /// <summary>
        /// 发票运单
        /// </summary>
        public string WaybillCode { get; set; }

        /// <summary>
        /// 开票金额
        /// </summary>
        public decimal KaiPiaoJinE { get; set; }

        /// <summary>
        /// 发票号
        /// </summary>
        public string InvoiceNoSummary { get; set; }

        /// <summary>
        /// 开票限额
        /// </summary>
        public decimal? AmountLimit { get; set; }

        #endregion

        /// <summary>
        /// 开票通知明细项
        /// </summary>
        InvoiceNoticeItems items;
        public InvoiceNoticeItems InvoiceItems
        {
            get
            {
                if (items == null)
                {
                    using (var view = new Views.InvoiceNoticeItemView())
                    {
                        var query = view.Where(item => item.InvoiceNoticeID == this.ID);
                        this.InvoiceItems = new InvoiceNoticeItems(query);
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
                this.items = new InvoiceNoticeItems(value, new Action<InvoiceNoticeItem>(delegate (InvoiceNoticeItem item)
                {
                    item.InvoiceNoticeID = this.ID;
                }));
            }
        }

        public int InvoiceNoticeFileCount { get; set; }

        public InvoiceNotice()
        {
            this.UpdateDate = this.CreateDate = DateTime.Now;
            this.InvoiceNoticeStatus = Enums.InvoiceNoticeStatus.UnAudit;
        }

        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;

        /// <summary>
        /// 删除
        /// </summary>
        public void Abandon()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    //reponsitory.Update<Layer.Data.Sqls.ScCustoms.InvoiceNotices>(
                    //    new
                    //    {
                    //        UpdateDate = DateTime.Now,
                    //        Status = Enums.InvoiceNoticeStatus.Canceled
                    //    }, item => item.ID == this.ID);

                    //删除xml
                    var invoiceXmls = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeXmls>().Where(t => t.InvoiceNoticeID == this.ID).ToList();
                    foreach (var xml in invoiceXmls)
                    {
                        reponsitory.Delete<Layer.Data.Sqls.ScCustoms.InvoiceNoticeXmlItems>(item => item.InvoiceNoticeXmlID == xml.ID);
                        reponsitory.Delete<Layer.Data.Sqls.ScCustoms.InvoiceNoticeXmls>(item => item.ID == xml.ID);
                    }

                    //更改订单的开票状态
                    var invoiceNoticeItems = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems>().Where(item => item.InvoiceNoticeID == this.ID).ToList();
                    foreach (var noticeItem in invoiceNoticeItems)
                    {
                        if (this.InvoiceType == InvoiceType.Full)
                        {
                            reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new
                            {
                                InvoiceStatus = Enums.InvoiceStatus.UnInvoiced,
                                UpdateDate = DateTime.Now,
                            }, item => item.ID == noticeItem.OrderID);
                        }
                        else
                        {
                            string[] OrderIDs = noticeItem.OrderID.Split(',');
                            foreach (var orderID in OrderIDs)
                            {
                                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new
                                {
                                    InvoiceStatus = Enums.InvoiceStatus.UnInvoiced,
                                    UpdateDate = DateTime.Now,
                                }, item => item.ID == orderID);
                            }
                        }
                    }

                    //删除开票通知
                    reponsitory.Delete<Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems>(item => item.InvoiceNoticeID == this.ID);
                    reponsitory.Delete<Layer.Data.Sqls.ScCustoms.InvoiceNotices>(item => item.ID == this.ID);
                }
                this.OnAbandonSuccess(); ;
            }
            catch (Exception ex)
            {
                this.AbandonError(this, new ErrorEventArgs(ex.Message));
            }
        }

        /// <summary>
        /// 数据插入
        /// </summary>
        public void Enter()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNotices>().Count(item => item.ID == this.ID);
                    if (count == 0)
                    {
                        this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.InvoiceNotice);
                        reponsitory.Insert(this.ToLinq());
                    }
                    else
                    {
                        UpdateDate = DateTime.Now;
                        reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                    }
                }
                this.OnEnterSuccess();
            }
            catch (Exception ex)
            {
                this.EnterError(this, new ErrorEventArgs(ex.Message));
            }
        }

        /// <summary>
        /// 开票中
        /// </summary>
        public void UpdateAuditing()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.InvoiceNotices>(
                    new
                    {
                        Status = Enums.InvoiceNoticeStatus.Auditing,
                        UpdateDate = DateTime.Now,
                    }, item => item.ID == this.ID);
            }
        }

        virtual public void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

        virtual protected void OnAbandonSuccess()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this));
            }
        }
    }

    /// <summary>
    /// 开票上下文
    /// </summary>
    public class InvoiceContext
    {
        public string InvoiceNoticeID { get; set; }

        //开票订单
        public IEnumerable<UnInvoicedOrder> orders { get; set; }

        //开票申请人
        public Admin Apply { get; set; }

        //开票人
        public Admin Admin { get; set; }

        public Client Client { get; set; }

        //开票备注
        public string Summary { get; set; }

        //差额，发票号Model
        public List<InvoiceSubmitModel> InvoiceModelList { get; set; }

        //开票通知列表
        public IEnumerable<InvoiceNotice> notices { get; set; }

        public IEnumerable<InvoiceNoticeItem> noticeItems { get; set; }

        //快递公司
        public string CompanyName { get; set; }
        //发票运单号
        public string WaybillCode { get; set; }

        public Enums.InvoiceNoticeStatus InvoiceNoticeStatus { get; set; }
        public decimal AmountLimit { get; set; }

        public InvoiceContext()
        {
            
            //生成XML数据 暂时不加
            this.Applyed += InvoiceContext_Applyed;
            
            //最后记录申请开票的操作记录
            this.Applyed += InvoiceRecord_Applyed;

            //订单开票状态
            this.Applyed += InvoiceNotice_Applyed;

            this.Confirmed += InvoiceNotice_Confirmed;
            //如果时间来的及，继续优化：尽可能的在构造函数完成对象中属性值
            //进行InvoiceNotice对象的实例化、可以考虑通过构造函数传入某些对象进来

            //分配发票的出库数量
            this.Allocate += InvoiceContext_Allocate;
        }



        public event StatusChangedEventHanlder Applyed;
        public event StatusChangedEventHanlder Confirmed;
        public event StatusChangedEventHanlder Allocate;

        /// <summary>
        /// 提交开票申请
        /// </summary>
        public void SubmitApply()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var order = this.orders.First();
                //获取客户的开票、补充协议、邮寄信息
                var Invoice = order.Client.Invoice;
                var Agreement = order.ClientAgreement;
                var invoiceConsignee = order.Client.InvoiceConsignee;
                //生成开票通知
                var entity = new InvoiceNotice();
                entity.ID = Needs.Overall.PKeySigner.Pick(PKeyType.InvoiceNotice);
                this.InvoiceNoticeID = entity.ID;
                entity.Apply = this.Apply;
                entity.Client = order.Client;
                entity.ClientInvoice = Invoice;
                entity.Address = Invoice.Address;
                entity.Tel = Invoice.Tel;
                entity.BankAccount = Invoice.BankAccount;
                entity.BankName = Invoice.BankName;
                entity.DeliveryType = Invoice.DeliveryType;
                entity.InvoiceTaxRate = Agreement.InvoiceTaxRate;
                entity.InvoiceType = Agreement.InvoiceType;
                entity.MailName = invoiceConsignee.Name;
                entity.MailMobile = invoiceConsignee.Mobile;
                entity.MailAddress = invoiceConsignee.Address;
                entity.Summary = this.Summary;
                entity.AmountLimit = this.AmountLimit;
                reponsitory.Insert(entity.ToLinq());

                NoticeLog noticeLog = new NoticeLog();
                noticeLog.MainID = this.InvoiceNoticeID;
                noticeLog.NoticeType = Enums.SendNoticeType.InvoicePending;
                noticeLog.SendNotice();

                //开票类型赛选
                if (Agreement.InvoiceType == Enums.InvoiceType.Full)
                {
                    //生成开票通知项
                    foreach (var noticeItem in noticeItems)
                    {
                        noticeItem.ID = Needs.Overall.PKeySigner.Pick(PKeyType.InvoiceNoticeItem);
                        noticeItem.InvoiceNoticeID = entity.ID;
                        noticeItem.OrderItem = new OrderItem { ID = noticeItem.OrderItemID };
                        reponsitory.Insert(noticeItem.ToLinq());
                    }
                }
                else
                {
                    var orderIDs = GetOrdersID();
                    foreach (var noticeItem in noticeItems)
                    {
                        noticeItem.ID = Needs.Overall.PKeySigner.Pick(PKeyType.InvoiceNoticeItem);
                        noticeItem.InvoiceNoticeID = entity.ID;
                        noticeItem.OrderID = orderIDs;
                        reponsitory.Insert(noticeItem.ToLinq());
                    }
                }
            }
            this.OnSubmitApply();
        }

        #region 去除重复发票号使用

        public class InvoiceInfoForCalc
        {
            public string InvoiceCode { get; set; }

            public string InvoiceNo { get; set; }

            public string InvoiceDate { get; set; }

            private DateTime? _invoiceDateDt;

            public DateTime? InvoiceDateDt
            {
                get
                {
                    DateTime dt;
                    if (DateTime.TryParse(this.InvoiceDate, out dt))
                    {
                        return dt;
                    }
                    else
                    {
                        return null;
                    }
                }
                set { this._invoiceDateDt = value; }
            }
        }

        public class DistinctInvoiceInfoComparer : IEqualityComparer<InvoiceInfoForCalc>
        {

            public bool Equals(InvoiceInfoForCalc x, InvoiceInfoForCalc y)
            {
                return x.InvoiceCode == y.InvoiceCode &&
                    x.InvoiceNo == y.InvoiceNo &&
                    x.InvoiceDate == y.InvoiceDate;
            }

            public int GetHashCode(InvoiceInfoForCalc obj)
            {
                return obj.InvoiceCode.GetHashCode() ^
                    obj.InvoiceNo.GetHashCode() ^
                    obj.InvoiceDate.GetHashCode();
            }
        }

        #endregion

        /// <summary>
        /// 确认开票
        /// </summary>
        public void ConfirmInvoice()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //1.更行通知项的信息
                //foreach (var item in InvoiceModelList)
                //{
                //    reponsitory.Update<Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems>(
                //        new
                //        {
                //            UpdateDate = DateTime.Now,
                //            InvoiceNo = item.InvoiceNo,
                //            InvoiceCode = item.InvoiceCode,
                //            InvoiceDate = item.InvoiceDateDt,
                //            //Difference = item.Difference
                //        }, x => x.ID == item.ID);
                //}
                //2.更新通知状态,和开票人
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.InvoiceNotices>(
                    new
                    {
                        Status = Enums.InvoiceNoticeStatus.Confirmed,
                        AdminID = this.Admin.ID,
                        UpdateDate = DateTime.Now,
                    }, x => x.ID == this.InvoiceNoticeID);

                //#region 插入发票信息到 TaxMap

                //List<InvoiceInfoForCalc> invoiceForCalc = new List<InvoiceInfoForCalc>();
                //foreach (var invoiceModel in InvoiceModelList)
                //{
                //    foreach (var invoiceNo in invoiceModel.InvoiceNosReal)
                //    {
                //        invoiceForCalc.Add(new InvoiceContext.InvoiceInfoForCalc
                //        {
                //            InvoiceCode = invoiceModel.InvoiceCode,
                //            InvoiceNo = invoiceNo,
                //            InvoiceDate = invoiceModel.InvoiceDate,
                //        });
                //    }
                //}
                //invoiceForCalc = invoiceForCalc.Distinct(new InvoiceContext.DistinctInvoiceInfoComparer()).ToList();

                //var newTaxMaps = invoiceForCalc.Select(item => new Layer.Data.Sqls.ScCustoms.TaxMap
                //{
                //    ID = Guid.NewGuid().ToString("N"),
                //    InvoiceNoticeID = this.InvoiceNoticeID,
                //    InvoiceCode = item.InvoiceCode,
                //    InvoiceNo = item.InvoiceNo,
                //    InvoiceDate = (DateTime)item.InvoiceDateDt,
                //    IsMapped = false,
                //    ApiStatus = (int)Enums.TaxMapApiStatus.UnHandled,
                //    Status = (int)Enums.Status.Normal,
                //    CreateDate = DateTime.Now,
                //    UpdateDate = DateTime.Now,
                //}).ToArray();
                //reponsitory.Insert(newTaxMaps);

                //#endregion
            }

            this.OnConfiremd();
        }

        /// <summary>
        /// 更新发票号
        /// </summary>
        public void UpdateInvoiceNo()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //1.更行通知项的信息
                foreach (var item in InvoiceModelList)
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems>(
                        new
                        {
                            UpdateDate = DateTime.Now,
                            InvoiceNo = item.InvoiceNo,
                        }, x => x.ID == item.ID);
                }
            }
        }

        /// <summary>
        /// 保存运单数据到数据库
        /// </summary>
        public void GInvoiceWaybill()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                string orderIDs = "";
                foreach (var notice in notices)
                {
                    InvoiceWaybill waybill = new InvoiceWaybill();
                    waybill.InvoiceNotice = notice;
                    waybill.CompanyName = this.CompanyName;
                    waybill.WaybillCode = this.WaybillCode;
                    waybill.Enter();

                    var items = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems>().Where(t => t.InvoiceNoticeID == notice.ID).ToList();
                    foreach (var orderid in items)
                    {
                        orderIDs += orderid.OrderID.Trim();
                    }
                }

                System.Threading.Tasks.Task.Run(() =>
                {
                    PushMsg pushMsg = new PushMsg((int)SpotName.Invoiced, orderIDs);
                    pushMsg.push();
                });
            }
        }

        public virtual void OnSubmitApply()
        {
            if (this != null && this.Applyed != null)
            {
                this.Applyed(this, new StatusChangedEventArgs(this));
            }
        }

        public virtual void OnConfiremd()
        {
            if (this != null && this.Confirmed != null)
            {
                this.Confirmed(this, new StatusChangedEventArgs(this));
            }
        }

        /// <summary>
        /// 开票申请后，更新订单状态为待开票
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InvoiceNotice_Applyed(object sender, StatusChangedEventArgs e)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                foreach (var order in orders)
                {
                    //更新订单状态
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(
                        new
                        {
                            UpdateDate = DateTime.Now,
                            InvoiceStatus = Enums.InvoiceStatus.Applied
                        }, item => item.ID == order.ID);
                }

                var invoice = (InvoiceContext)e.Object;
                invoice.InvoiceNoticeID = this.InvoiceNoticeID;
                if (invoice.Apply != null)
                {
                    invoice.Log(invoice.Apply, "跟单员[" + invoice.Apply.RealName + "]申请了开票,等待开票");
                }
            }
        }


        private void InvoiceContext_Applyed(object sender, StatusChangedEventArgs e)
        {
            List<InvoiceNoticeXmlItem> xmlItems = new List<InvoiceNoticeXmlItem>();
            var invoiceNotice = new Views.InvoiceNoticeView().FirstOrDefault(t => t.ID == this.InvoiceNoticeID);
            if (invoiceNotice != null)
            {
                try
                {
                    var help = new InvoiceXmlHelp(invoiceNotice);
                    decimal alimit = invoiceNotice.AmountLimit == null ? InvoiceXmlConfig.XianEPerFp : invoiceNotice.AmountLimit.Value;
                    List<XmlFileUnit> xmlFileUnits = help.GenerateXmlFile(alimit);

                    foreach (var item in xmlFileUnits)
                    {
                        Fpxx fpxx = item.Kp.Fpxx;
                        foreach (var xml in fpxx.Fpsj)
                        {
                            string invoiceNoticeXmlID = Needs.Overall.PKeySigner.Pick(PKeyType.InvoiceNoticeXml);
                            InvoiceNoticeXml invoiceNoticeXml = new InvoiceNoticeXml();
                            invoiceNoticeXml.ID = invoiceNoticeXmlID;
                            invoiceNoticeXml.InvoiceNoticeID = this.InvoiceNoticeID;
                            invoiceNoticeXml.Djh = xml.Djh;
                            invoiceNoticeXml.Gfmc = xml.Gfmc.Trim();
                            invoiceNoticeXml.Gfsh = xml.Gfsh.Trim();
                            invoiceNoticeXml.Gfyhzh = xml.Gfyhzh.Trim();
                            invoiceNoticeXml.Gfdzdh = xml.Gfdzdh.Trim();
                            invoiceNoticeXml.Bz = xml.Bz.Length > 239 ? xml.Bz.Substring(0, 239) : xml.Bz;
                            invoiceNoticeXml.Fhr = xml.Fhr;
                            invoiceNoticeXml.Skr = xml.Skr;
                            invoiceNoticeXml.Spbmbbh = xml.Spbmbbh;
                            invoiceNoticeXml.Hsbz = xml.Hsbz;
                            invoiceNoticeXml.Admin = this.Apply;
                            invoiceNoticeXml.Enter();

                            int i = 1;
                            foreach (var xmlItem in xml.Spxx)
                            {
                                InvoiceNoticeXmlItem IXmlItem = new InvoiceNoticeXmlItem();
                                IXmlItem.ID = Needs.Overall.PKeySigner.Pick(PKeyType.InvoiceNoticeXmlItem);
                                IXmlItem.InvoiceNoticeXmlID = invoiceNoticeXmlID;
                                IXmlItem.Xh = i;
                                IXmlItem.Spmc = xmlItem.Spmc.Trim();
                                IXmlItem.Ggxh = xmlItem.Ggxh;
                                IXmlItem.Jldw = xmlItem.Jldw;
                                IXmlItem.Spbm = xmlItem.Spbm;
                                IXmlItem.Qyspbm = xmlItem.Qyspbm;
                                IXmlItem.Syyhzcbz = xmlItem.Syyhzcbz;
                                IXmlItem.Lslbz = xmlItem.Lslbz;
                                IXmlItem.Dj = xmlItem.Dj;
                                IXmlItem.Sl = xmlItem.Sl;
                                IXmlItem.Je = xmlItem.Je;
                                IXmlItem.Slv = xmlItem.Slv;
                                IXmlItem.Se = xmlItem.Se;
                                IXmlItem.InvoiceNoticeItemID = xmlItem.InvoiceNoticeItemID;
                                xmlItems.Add(IXmlItem);
                                IXmlItem.Enter();
                                i++;
                            }
                        }
                    }

                    if (this != null && this.Allocate != null)
                    {
                        this.Allocate(this, new StatusChangedEventArgs(xmlItems));
                    }
                }
                catch (Exception ex)
                {

                    ex.CcsLog("开票XML数据生成失败：" + this.InvoiceNoticeID);

                }
            }
            else
            {
                this.Log(this.Apply, "开票XML数据生成失败，未找到通知：" + this.InvoiceNoticeID);
            }

        }

        private void InvoiceRecord_Applyed(object sender, StatusChangedEventArgs e) 
        {

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var downLoadAmount = 0M;
                var summary = "";
                var currency = "";
                foreach (var order in orders)
                {
                    //累加开票金额
                    downLoadAmount += order.DeclarePrice;
                    summary += order.ID + ",";
                    currency = order.Currency;
                }

                var invoice = (InvoiceContext)e.Object;

                //记录开票申请时，付汇情况
                //插入记录表
                var record = new ClientUnPayExchangeRecord();
                record.ClientID = invoice.Client.ID;
                record.Type = 3;
                record.UnPayExchangeAmount = invoice.Client.UnPayExchangeAmount;
                record.DeclareAmount = invoice.Client.DeclareAmount;
                record.PayExchangeAmount = invoice.Client.PayExchangeAmount;
                record.DeclareAmountMonth = invoice.Client.DeclareAmountMonth;
                record.PayExchangeAmountMonth = invoice.Client.PayExchangeAmountMonth;
                record.Amount = downLoadAmount;//invoice.noticeItems.Sum(t => t.Amount);
                record.Currency = currency;
                record.Summary = summary;
                record.AdminID = invoice.Apply.ID;
                record.Enter();
            }
        }

        /// <summary>
        /// 开票确认后，更新订单状态为已开票
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InvoiceNotice_Confirmed(object sender, StatusChangedEventArgs e)
        {
            var orders = InvoiceModelList.DistinctBy(p => new { p.OrderID });

            string[] orderIds;
            string orderIDs = "";
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                foreach (var order in orders)
                {
                    //reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(
                    //    new
                    //    {
                    //        UpdateDate = DateTime.Now,
                    //        InvoiceStatus = Enums.InvoiceStatus.Invoiced
                    //    }, item => item.ID == order.OrderID);

                    orderIDs = orderIDs + order.OrderID + ",";
                }

                orderIDs = orderIDs.TrimEnd(',');
                orderIds = orderIDs.Split(',');
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(
                        new
                        {
                            UpdateDate = DateTime.Now,
                            InvoiceStatus = Enums.InvoiceStatus.Invoiced
                        }, item => orderIds.Contains(item.ID));
                var invoice = (InvoiceContext)e.Object;
                if (invoice.Admin != null)
                {
                    invoice.Log(invoice.Admin, "财务人员[" + invoice.Admin.ByName + "]确认了开票");
                }


                Task.Run(() =>
                {
                    try
                    {
                        #region 开票状态更新代仓储

                        var apiPvWsorder = new Needs.Ccs.Services.ApiSettings.PvWsOrderApiSetting();
                        string pvWsorderAddress = apiPvWsorder.InvoiceComplete;

                        var apipvurl = System.Configuration.ConfigurationManager.AppSettings[apiPvWsorder.ApiName] + pvWsorderAddress;

                        var ermAdminID = new Needs.Ccs.Services.Views.AdminsTopView2().FirstOrDefault(x => x.OriginID == this.Admin.ID)?.ID;

                        var mainOrderID = new OrdersOrigin().Where(t => orderIds.Contains(t.ID)).Select(t => t.MainOrderID).Distinct();


                        var PostData = new
                        {
                            VastOrderID = string.Join(",", mainOrderID),
                            AdminID = ermAdminID
                        };

                        Needs.Utils.Http.ApiHelper.Current.JPost(apipvurl, PostData);

                        #endregion


                        #region 将开票价格数据给到库房

                        //调用库房接口，将价格数据给到库房
                        var list = new CgDelcareSZPrice();
                        var lstItem = new List<CgDelcareSZPriceItem>();
                        //全额
                        if (orders.FirstOrDefault().TaxCode != "3040407040000000000")
                        {
                            var declist = GetAddTaxPrice(orderIds, this.InvoiceNoticeID);
                            foreach (var orderid in orderIds)
                            {
                                var decHead = new DecHeadsView().Where(t => t.OrderID == orderid).FirstOrDefault();
                                int decimalForTotalPrice = 0;
                                if (decHead != null && decHead.isTwoStep)
                                {
                                    decimalForTotalPrice = 2;
                                }
                                var declistSingle = declist.Where(t => t.OrderID == orderid).ToList();
                                foreach (var item in declistSingle)
                                {
                                    var entity = new CgDelcareSZPriceItem();
                                    entity.OrderID = item.MainOrderId;
                                    entity.TinyOrderID = item.OrderID;
                                    entity.OrderItemID = item.OrderItemID;
                                    entity.InUnitPrice = (((item.DeclTotal * item.CustomsExchangeRate) + ((item.DeclTotal * item.CustomsExchangeRate).ToRound(decimalForTotalPrice) * item.ReceiptRate)) / item.Qty).ToRound(7);
                                    entity.OutUnitPrice = (((item.TaxAmount + item.diffence)
                                        / (1 + orders.FirstOrDefault(x => x.OrderID == item.OrderID).InvoiceTaxRate)) / item.Qty).ToRound(7);
                                    lstItem.Add(entity);
                                }
                            }
                        }
                        else
                        {
                            //服务费发票
                            var declist = new CgDelcareSZPriceView().Where(x => orderIds.Contains(x.OrderID)).ToArray();
                            foreach (var orderid in orderIds)
                            {
                                var decHead = new DecHeadsView().Where(t => t.OrderID == orderid).FirstOrDefault();
                                int decimalForTotalPrice = 0;
                                if (decHead != null && decHead.isTwoStep)
                                {
                                    decimalForTotalPrice = 2;
                                }
                                var declistSingle = declist.Where(t => t.OrderID == orderid).ToList();
                                foreach (var item in declistSingle)
                                {
                                    var entity = new CgDelcareSZPriceItem();
                                    entity.OrderID = item.MainOrderId;
                                    entity.TinyOrderID = item.OrderID;
                                    entity.OrderItemID = item.OrderItemID;
                                    entity.InUnitPrice = (((item.DeclTotal * item.CustomsExchangeRate) + ((item.DeclTotal * item.CustomsExchangeRate).ToRound(decimalForTotalPrice) * item.ReceiptRate)) / item.Qty).ToRound(7);
                                    entity.OutUnitPrice = (((item.DeclTotal * item.CustomsExchangeRate) + ((item.DeclTotal * item.CustomsExchangeRate).ToRound(decimalForTotalPrice) * item.ReceiptRate)) / item.Qty).ToRound(7);
                                    lstItem.Add(entity);
                                }
                            }


                        }
                        list.Items = lstItem;
                        var apisetting = new ApiSettings.PfWmsApiSetting();
                        var apiurl = System.Configuration.ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.SZPriceCompute;
                        //var obj = list.Json();
                        var result = Needs.Utils.Http.ApiHelper.Current.PostData(apiurl, list);
                        var message = Newtonsoft.Json.JsonConvert.DeserializeObject<Underly.JMessage>(result);

                        if (message.code != 200)
                        {
                            throw new Exception(message.data);
                            //调用失败，是否记录到日志中

                        }

                        #endregion
                    }
                    catch (Exception ex)
                    {
                        ex.CcsLog("开票完成调用库房接口");
                    }
                });

            }
        }

        /// <summary>
        ///  给每个发票匹配出库数量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InvoiceContext_Allocate(object sender, StatusChangedEventArgs e)
        {
            List<InvoiceNoticeXmlItem> xmlItems = (List<InvoiceNoticeXmlItem>)e.Object;
            var StorageList = new InvoiceNoticeViewRJ(this.InvoiceNoticeID).GetVItemJoinDeclist4Allocate();
            var mapView = new InvoiceXmlMapView();
            foreach (var item in xmlItems)
            {
                if (item.Spmc.Contains("服务费"))
                {
                    continue;
                }
                var dec = StorageList.Where(t => t.InvoiceNoticeXmlItemID == item.ID).OrderBy(t => t.Quantity).ToList();
                var decIds = dec.Select(t => t.DeclistItemID).ToList();
                var decQty = dec.Sum(t => t.Quantity);
                //如果发票的数量 等于 declist里面的数量
                if (item.Sl == decQty)
                {
                    foreach (var declist in dec)
                    {
                        InvoiceNoticeXmlMap invoiceNoticeXmlMap = new InvoiceNoticeXmlMap();
                        invoiceNoticeXmlMap.ID = ChainsGuid.NewGuidUp();
                        invoiceNoticeXmlMap.InvoiceXmlID = item.ID;
                        invoiceNoticeXmlMap.DecListID = declist.DeclistItemID;
                        invoiceNoticeXmlMap.OutQty = declist.Quantity;

                        invoiceNoticeXmlMap.Enter();
                    }
                }
                else
                {
                    //发票的数量 小于 declist里面的数量  不可能大于
                    var mapResult = mapView.Where(t => decIds.Contains(t.DecListID)).ToList();
                    List<VItemJoinDecList4Allocate> pendingMaps = new List<VItemJoinDecList4Allocate>();
                    // 没有匹配过
                    if (mapResult.Count == 0)
                    {
                        pendingMaps = dec;
                    }
                    else
                    {
                        // 有匹配过 要扣除已匹配的数量                       
                        foreach (var itemdec in dec)
                        {
                            decimal alMappedQty = mapResult.Where(t => t.DecListID == itemdec.DeclistItemID).Sum(t => t.OutQty);
                            if (alMappedQty < itemdec.Quantity)
                            {
                                VItemJoinDecList4Allocate allocateitem = new VItemJoinDecList4Allocate();
                                allocateitem.Quantity = itemdec.Quantity - alMappedQty;
                                allocateitem.DeclistItemID = itemdec.DeclistItemID;
                                pendingMaps.Add(allocateitem);
                            }
                        }
                    }

                    decimal leftmappedQty = item.Sl;
                    foreach (var itemdec in pendingMaps)
                    {
                        if (itemdec.Quantity > leftmappedQty)
                        {
                            InvoiceNoticeXmlMap invoiceNoticeXmlMap = new InvoiceNoticeXmlMap();
                            invoiceNoticeXmlMap.ID = ChainsGuid.NewGuidUp();
                            invoiceNoticeXmlMap.InvoiceXmlID = item.ID;
                            invoiceNoticeXmlMap.DecListID = itemdec.DeclistItemID;
                            invoiceNoticeXmlMap.OutQty = leftmappedQty;
                            invoiceNoticeXmlMap.Enter();
                            break;
                        }
                        else
                        {
                            InvoiceNoticeXmlMap invoiceNoticeXmlMap = new InvoiceNoticeXmlMap();
                            invoiceNoticeXmlMap.ID = ChainsGuid.NewGuidUp();
                            invoiceNoticeXmlMap.InvoiceXmlID = item.ID;
                            invoiceNoticeXmlMap.DecListID = itemdec.DeclistItemID;
                            invoiceNoticeXmlMap.OutQty = itemdec.Quantity;
                            invoiceNoticeXmlMap.Enter();

                            leftmappedQty -= itemdec.Quantity;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取增值税开票的单价
        /// </summary>
        /// <param name="orderIds"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        public List<SZPrice> GetAddTaxPrice(string[] orderIds, string ID)
        {
            using (var reponsitory = new ScCustomsReponsitory())
            {


                var orderlinq = from order in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                                where (orderIds.Contains(order.ID) && order.Status == (int)Status.Normal)
                                select new
                                {
                                    ID = order.ID,
                                    order.CustomsExchangeRate,
                                    MainOrderID = order.MainOrderId
                                };
                var orderView = orderlinq.ToArray();

                var noticeItemlinq = from noticeItem in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems>()
                                     where noticeItem.InvoiceNoticeID == ID
                                     select new
                                     {
                                         ID = noticeItem.ID,
                                         noticeItem.Amount,
                                         noticeItem.Difference,
                                         noticeItem.OrderItemID
                                     };
                var noticeItemView = noticeItemlinq.ToArray();

                var declinq = from dec in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>()
                              where orderIds.Contains(dec.OrderID)
                              select new
                              {
                                  ID = dec.ID,
                                  dec.OrderItemID,
                                  dec.GQty,
                                  dec.OrderID,
                                  dec.DeclTotal,
                              };
                var decView = declinq.ToArray();
                var taxesView = new OrderItemTaxsOrigin().Where(item => item.Type == Enums.CustomsRateType.ImportTax).ToArray();

                var linq = from noticeItem in noticeItemView
                           join dec in decView on noticeItem.OrderItemID equals dec.OrderItemID
                           join order in orderView on dec.OrderID equals order.ID
                           join tax in taxesView on noticeItem.OrderItemID equals tax.OrderItemID
                           select new Models.SZPrice
                           {
                               MainOrderId = order.MainOrderID,
                               OrderItemID = dec.OrderItemID,
                               CustomsExchangeRate = order.CustomsExchangeRate.Value,
                               OrderID = dec.OrderID,
                               DeclTotal = dec.DeclTotal,
                               Qty = dec.GQty,
                               TaxAmount = noticeItem.Amount,
                               diffence = noticeItem.Difference,
                               ReceiptRate = tax.ReceiptRate
                           };
                return linq.ToList();


            };

        }
        /// <summary>
        /// 获取订单费用含税总额（开服务费发票使用）
        /// </summary>
        /// <returns></returns>
        private decimal GetOrdersTotalFee()
        {
            decimal fee = 0M;

            foreach (var order in orders)
            {
                //订单代理费+杂费（含商检费）
                var orderFees = order.Premiums.Sum(item => item.Count * item.UnitPrice * item.Rate);
                var taxPoint = (1 + order.Client.Agreement.InvoiceTaxRate);

                fee = fee + orderFees * taxPoint;
            }
            return fee.ToRound(4);
        }

        private string GetOrdersID()
        {
            string orderIDs = "";
            foreach (var order in orders)
            {
                orderIDs = orderIDs + order.ID + ",";
            }
            return orderIDs.TrimEnd(',');
        }

    }

    /// <summary>
    /// 数据Model（前端上传）
    /// </summary>
    public class InvoiceSubmitModel
    {
        /// <summary>
        /// 通知项ID
        /// </summary>
        public string ID { get; set; }

        public string OrderID { get; set; }

        public decimal Difference { get; set; }

        /// <summary>
        /// 发票代码
        /// </summary>
        public string InvoiceCode { get; set; }

        /// <summary>
        /// 发票号码
        /// </summary>
        public string InvoiceNo { get; set; }

        private string[] _invoiceNosReal;

        public string[] InvoiceNosReal
        {
            get
            {
                if (string.IsNullOrEmpty(this.InvoiceNo))
                {
                    return null;
                }
                List<string> results = new List<string>();
                var invoiceNosByHengGang = this.InvoiceNo.Split(',');
                foreach (var invoiceNoByHG in invoiceNosByHengGang)
                {
                    if (!invoiceNoByHG.Contains("-"))
                    {
                        results.Add(invoiceNoByHG);
                    }
                    else
                    {
                        int begin = int.Parse(invoiceNoByHG.Split('-')[0]);
                        int end = int.Parse(invoiceNoByHG.Split('-')[1]);
                        for (int i = begin; i <= end; i++)
                        {
                            results.Add(Convert.ToString(i));
                        }
                    }
                }

                return results.ToArray();
            }
            set { this._invoiceNosReal = value; }
        }

        /// <summary>
        /// 开票日期
        /// </summary>
        public string InvoiceDate { get; set; }

        private DateTime? _invoiceDateDt;

        public DateTime? InvoiceDateDt
        {
            get
            {
                DateTime dt;
                if (DateTime.TryParse(this.InvoiceDate, out dt))
                {
                    return dt;
                }
                else
                {
                    return null;
                }
            }
            set { this._invoiceDateDt = value; }
        }

        public decimal InvoiceTaxRate { get; set; }

        //public decimal Amount { get; set; }

        public string TaxCode { get; set; }
    }
}
