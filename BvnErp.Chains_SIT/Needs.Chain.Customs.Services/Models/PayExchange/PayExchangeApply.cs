using Needs.Ccs.Services.Enums;
using Needs.Linq;
using Needs.Utils;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using Spire.Pdf.Tables;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 付汇申请
    /// </summary>
    public class PayExchangeApply : IUnique, IPersist
    {
        #region 属性

        public string ID { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string SupplierName { get; set; }

        /// <summary>
        /// 供应商英文名称
        /// </summary>
        public string SupplierEnglishName { get; set; }

        /// <summary>
        /// 供应商地址
        /// </summary>
        public string SupplierAddress { get; set; }

        /// <summary>
        /// 银行账户
        /// </summary>
        public string BankAccount { get; set; }

        /// <summary>
        /// 银行地址
        /// </summary>
        public string BankAddress { get; set; }

        /// <summary>
        /// 银行名称
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// 银行国际代码
        /// </summary>
        public string SwiftCode { get; set; }

        /// <summary>
        /// 使用的汇率类型
        /// </summary>
        public Enums.ExchangeRateType ExchangeRateType { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 汇率
        /// </summary>
        public decimal ExchangeRate { get; set; }

        /// <summary>
        /// 付款方式
        /// </summary>
        public Enums.PaymentType PaymentType { get; set; }

        /// <summary>
        /// 期望付款日期
        /// </summary>
        public DateTime? ExpectPayDate { get; set; }

        /// <summary>
        /// 结算截止日期
        /// </summary>
        public DateTime SettlemenDate { get; set; }

        /// <summary>
        /// 其它资料
        /// </summary>
        public string OtherInfo { get; set; }

        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID { get; set; }

        public Client Client { get; set; }

        /// <summary>
        /// 管理员\跟单员
        /// 跟单员提交的付汇申请
        /// </summary>
        public virtual Admin Admin { get; set; }

        /// <summary>
        /// 会员
        /// 客户提交的付汇申请
        /// </summary>
        public virtual User User { get; set; }

        public string FatherID { get; set; }
        /// <summary>
        /// 付汇申请状态
        /// </summary>
        public Enums.PayExchangeApplyStatus PayExchangeApplyStatus { get; set; }

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }
        /// <summary>
        /// 美国付汇账号
        /// </summary>
        public string ABA { get; set; }
        /// <summary>
        /// 欧美付汇账号
        /// </summary>
        public string IBAN { get; set; }

        /// <summary>
        /// 是否垫款
        /// </summary>
        public int? IsAdvanceMoney { get; set; }

        /// <summary>
        /// 大赢家ID
        /// </summary>
        public string DyjID { get; set; }

        /// <summary>
        /// 代付款手续费类型
        /// </summary>
        public int? HandlingFeePayerType { get; set; }

        /// <summary>
        /// 手续费（美元）
        /// </summary>
        public decimal? HandlingFee { get; set; }

        /// <summary>
        /// 美元实时汇率
        /// </summary>
        public decimal? USDRate { get; set; }

        /// <summary>
        /// 付汇申请明细
        /// </summary>
        PayExchangeApplyItems payExchangeApplyItems;
        public PayExchangeApplyItems PayExchangeApplyItems
        {
            get
            {
                if (payExchangeApplyItems == null)
                {
                    using (var view = new Views.PayExchangeApplieItemsView())
                    {
                        var query = view.Where(item => item.PayExchangeApplyID == this.ID);
                        this.PayExchangeApplyItems = new PayExchangeApplyItems(query);
                    }
                }
                return this.payExchangeApplyItems;
            }
            set
            {
                if (value == null)
                {
                    return;
                }
                this.payExchangeApplyItems = new PayExchangeApplyItems(value, new Action<PayExchangeApplyItem>(delegate (PayExchangeApplyItem item)
                {
                    item.PayExchangeApplyID = this.ID;
                }));
            }
        }

        /// <summary>
        /// 付汇申请文件
        /// </summary>
        PayExchangeApplyFiles payExchangeApplyFiles;
        public PayExchangeApplyFiles PayExchangeApplyFiles
        {
            get
            {
                if (payExchangeApplyFiles == null)
                {
                    using (var view = new Views.PayExchangeApplyFileView())
                    {
                        var query = view.Where(item => item.PayExchangeApplyID == this.ID);
                        this.PayExchangeApplyFiles = new PayExchangeApplyFiles(query);
                    }
                }
                return this.payExchangeApplyFiles;
            }
            set
            {
                if (value == null)
                {
                    return;
                }
                this.payExchangeApplyFiles = new PayExchangeApplyFiles(value, new Action<PayExchangeApplyFile>(delegate (PayExchangeApplyFile item)
                {
                    item.PayExchangeApplyID = this.ID;
                }));
            }
        }

        /// <summary>
        /// 付汇申请日志
        /// </summary>
        PayExchangeLogs orderPayExchangeLogs;

        public PayExchangeLogs OrderPayExchangeLogs
        {
            get
            {
                if (orderPayExchangeLogs == null)
                {
                    using (var view = new Views.PayExchangeLogsView())
                    {
                        var query = view.Where(item => item.PayExchangeApplyID == this.ID);
                        this.OrderPayExchangeLogs = new PayExchangeLogs(query);
                    }
                }
                return this.orderPayExchangeLogs;
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                this.orderPayExchangeLogs = new PayExchangeLogs(value, new Action<PayExchangeLog>(delegate (PayExchangeLog item)
                {
                    item.PayExchangeApplyID = this.ID;
                }));
            }
        }

        /// <summary>
        /// 付汇委托书
        /// </summary>
        public PayExchangeAgentProxy PayExchangeAgentProxy
        {
            get
            {
                return new PayExchangeAgentProxy(this);
            }
        }

        private IEnumerable<UnPayExchangeOrder> Orders;

        #endregion

        /// <summary>
        /// 当新增或修改成功时发生
        /// </summary>
        public event SuccessHanlder EnterSuccess;

        #region 构造函数

        public PayExchangeApply()
        {
            this.PayExchangeApplyStatus = PayExchangeApplyStatus.Auditing;
            this.Status = Status.Normal;
            this.CreateDate = this.UpdateDate = DateTime.Now;
            this.SettlemenDate = DateTime.Now.AddDays(90);
        }

        public PayExchangeApply(IEnumerable<UnPayExchangeOrder> orders) : this()
        {
            this.Orders = orders;
            this.payExchangeApplyItems = new PayExchangeApplyItems(orders.Select(item => new PayExchangeApplyItem
            {
                OrderID = item.ID,
                Amount = item.CurrentPaidAmount,
                Status = Status.Normal,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
            }));
        }

        #endregion

        #region 持久化

        public virtual void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //付汇申请
                this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.PayExchangeApply);
                reponsitory.Insert(this.ToLinq());

                //插入文件
                List<string> toCenterFileNames = new List<string>();


                var centerType = Needs.Ccs.Services.Models.ApiModels.Files.FileType.PIFiles;
                string ermAdminID = "";

                int fileNum = 0;

                foreach (var file in PayExchangeApplyFiles)
                {
                    //file.ID = Needs.Overall.PKeySigner.Pick(PKeyType.PayExchangeApplyFile);
                    //file.PayExchangeApplyID = this.ID;
                    //file.AdminID = this.Admin?.ID;
                    //file.UserID = this.User?.ID;
                    //file.Status = Status.Normal;
                    //file.CreateDate = DateTime.Now;

                    //reponsitory.Insert(file.ToLinq());

                    #region 付汇PI上传中心
                    var orderfile = new Needs.Ccs.Services.Models.CenterFilesTopView().FirstOrDefault(x => x.Url == file.Url && x.Type == (int)FileType.OriginalInvoice);

                    if (orderfile != null)
                    {
                        var entity = new CenterFileDescription();
                        entity.AdminID = file.ErmAdminID;
                        entity.ApplicationID = this.ID;
                        entity.Type = (int)centerType;
                        entity.Url = orderfile.Url;
                        entity.Status = FileDescriptionStatus.Normal;
                        entity.CreateDate = DateTime.Now;
                        entity.CustomName = file.FileName;

                        fileNum++;
                        DateTime liunxStart = new DateTime(1970, 1, 1);
                        var linuxtime = (DateTime.Now - liunxStart).Ticks;
                        string topID = "F" + linuxtime + "-" + fileNum;

                        new CenterFilesTopView().Insert(entity, topID);
                    }
                    else
                    {
                        var url = FileDirectory.Current.FilePath + @"\" + file.Url;
                        //var dic = new { CustomName = file.FileName, ApplicationID = this.ID, AdminID = file.ErmAdminID };
                        //var result = Needs.Ccs.Services.Models.CenterFilesTopView.Upload(url, centerType, dic);

                        ermAdminID = file.ErmAdminID;
                        toCenterFileNames.Add(url);
                    }

                    #endregion
                }

                if (toCenterFileNames != null && toCenterFileNames.Any())
                {
                    CenterFilesTopView.Upload(centerType, new
                    {
                        ApplicationID = this.ID,
                        AdminID = ermAdminID,
                    }, toCenterFileNames.ToArray());
                }


                //插入Items
                foreach (var order in PayExchangeApplyItems)
                {
                    Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems orderData = new Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems();
                    orderData.ID = Needs.Overall.PKeySigner.Pick(PKeyType.PayExchangeApplyItem);
                    orderData.PayExchangeApplyID = this.ID;
                    orderData.OrderID = order.OrderID;
                    orderData.Amount = order.Amount;
                    orderData.Status = (int)Status.Normal;
                    orderData.CreateDate = DateTime.Now;
                    orderData.UpdateDate = DateTime.Now;
                    orderData.ApplyStatus = (int)Enums.ApplyItemStatus.Appling;
                    reponsitory.Insert(orderData);

                    var curOrder = this.Orders.Where(item => item.ID == order.OrderID).FirstOrDefault();
                    if ((curOrder.DeclarePrice - curOrder.PaidExchangeAmount) < order.Amount)
                    {
                        throw new Exception("订单号【" + order.OrderID + "】申请付汇金额大于可申请付汇金额！");
                    }
                    //更新订单的已付汇金额
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(
                        new { PaidExchangeAmount = curOrder.PaidExchangeAmount + order.Amount }, item => item.ID == order.OrderID);
                }
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public virtual void Abandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>(
                    new { Status = Enums.Status.Delete }, item => item.ID == this.ID);

                //删除明细
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>(
                    new { Status = Enums.Status.Delete }, t => t.PayExchangeApplyID == this.ID);

                //待审核状态的付汇申请，做取消或删除时需要更新订单已付汇金额
                if (this.PayExchangeApplyStatus == Enums.PayExchangeApplyStatus.Auditing)
                {
                    foreach (var item in PayExchangeApplyItems)
                    {
                        //更新已付汇金额
                        var order = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                            .Where(t => t.ID == item.OrderID).FirstOrDefault();
                        decimal amount = order.PaidExchangeAmount - item.Amount;
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(
                            new { PaidExchangeAmount = amount }, t => t.ID == item.OrderID);
                    }
                }
            }
        }

        #endregion
    }
}