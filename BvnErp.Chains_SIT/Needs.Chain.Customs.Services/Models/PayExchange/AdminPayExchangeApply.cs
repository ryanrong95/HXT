using Layer.Data.Sqls.ScCustoms;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Views;
using Needs.Linq;
using Needs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 跟单员的付汇申请
    /// TODO:付汇审核需要在单独出来，AuditingPayExchangeApply 待审核的付汇申请,对应的对象的实例化及view加载都要单独出来
    /// 这样就Payer  Operator 就知道如何处理了，
    /// </summary>
    public sealed class AdminPayExchangeApply : PayExchangeApply
    {
        /// <summary>
        /// 付汇申请的付汇总金额
        /// </summary>
        public decimal? TotalAmount
        {
            get
            {
                return this.PayExchangeApplyItems.Sum(item => item.Amount);
            }
        }

        /// <summary>
        /// 操作人(管理端)
        /// </summary>
        private Admin Operator { get; set; }

        public void SetOperator(Admin admin)
        {
            this.Operator = admin;
        }

        #region 事件

        /// <summary>
        /// 当新增或修改成功时发生
        /// </summary>
        public event SuccessHanlder Applyed;

        private void AdminPayExchangeApply_EnterSuccess(object sender, SuccessEventArgs e)
        {
            var apply = (AdminPayExchangeApply)e.Object;
            if (apply.Operator != null)
            {
                apply.Log(apply.Operator, "跟单员[" + apply.Operator.RealName + "]新增了付汇申请，等待审核");
            }
        }

        private void AdminPayExchangeApply_SendNotice(object sender, SuccessEventArgs e)
        {
            var apply = (AdminPayExchangeApply)e.Object;
            NoticeLog noticeLog = new NoticeLog();
            noticeLog.MainID = apply.ID;
            noticeLog.NoticeType = SendNoticeType.PayExchangeAudit;
            noticeLog.OrderID = apply.PayExchangeApplyItems.FirstOrDefault().OrderID;
            noticeLog.SendNotice();
        }

        #endregion

        public AdminPayExchangeApply() : base()
        {
        }

        public AdminPayExchangeApply(IEnumerable<UnPayExchangeOrder> order) : base(order)
        {
            this.Applyed += AdminPayExchangeApply_EnterSuccess;
            this.Applyed += AdminPayExchangeApply_SendNotice;
        }

        /// <summary>
        /// 持久化
        /// </summary>
        public override void Enter()
        {
            base.Enter();
            this.OnEntered();
        }

        public void OnEntered()
        {
            if (this != null && this.Applyed != null)
            {
                //成功后触发事件
                this.Applyed(this, new SuccessEventArgs(this));
            }
        }
    }

    /// <summary>
    /// 待审核的付汇申请
    /// </summary>
    public sealed class UnAuditedPayExchangeApply : PayExchangeApply
    {
        /// <summary>
        /// 付汇申请的总金额
        /// </summary>
        public decimal? TotalAmount
        {
            get
            {
                return this.PayExchangeApplyItems.Sum(item => item.Amount);
            }
        }

        /// <summary>
        /// 付汇委托书
        /// </summary>
        public PayExchangeApplyFile ProxyFile { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        private Admin Operator { get; set; }

        public void SetOperator(Admin admin)
        {
            this.Operator = admin;
        }

        #region 事件

        /// <summary>
        /// 当审核通过时发生
        /// </summary>
        public event PayExchangeApplyAuditedHanlder Audited;

        /// <summary>
        /// 当审核取消时发生
        /// </summary>
        public event PayExchangeApplyCanceledHanlder Canceled;

        /// <summary>
        /// 审核通过时，调用大赢家
        /// </summary>
        public event DyjPayExchangeApplyHanlder DyjAudited;

        private void AdminPayExchangeApply_Audited(object sender, PayExchangeApplyAuditedEventArgs e)
        {
            var apply = (UnAuditedPayExchangeApply)e.PayExchangeApply;
            var admin = apply.Operator;
            if (admin != null)
            {
                apply.Log(admin, "跟单员[" + admin.RealName + "]审核通过了付汇申请，等待经理审批");
            }

            //此处调用一个 Yahv   传付汇应收 Begin

            var ermAdmin = new Needs.Ccs.Services.Views.AdminsTopView2().FirstOrDefault(x => x.OriginID == admin.ID);
            PayExchangeToYahvReceivable toYahv = new PayExchangeToYahvReceivable(apply.ID, ermAdmin);
            toYahv.Execute();

            //此处调用一个 Yahv   传付汇应收 End
        }

        private void AdminPayExchangeApply_Canceled(object sender, PayExchangeApplyCanceledEventArgs e)
        {
            var apply = (UnAuditedPayExchangeApply)e.PayExchangeApply;
            var admin = apply.Operator;
            if (admin != null)
            {
                e.PayExchangeApply.Log(admin, "跟单员[" + admin.RealName + "]审核取消了付汇申请");
            }
        }

        private void UnAuditedPayExchangeApply_DyjAudited(object sender, DyjPayExchangeApplyEventArgs e)
        {
            //付汇申请同步大赢家
            try
            {
                var result = new Finance.DyjFinance.DyjPayExchange(e.DyjPayExchangeApply);
                var data = result.PostToDYJ();
                //付汇接口返回值，保存进付汇申请
                if (!string.IsNullOrEmpty(data))
                {
                    var dyjid = data.Replace(".0", "");
                    using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                    {
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>(new
                        {
                            DyjID = dyjid,
                        }, item => item.ID == this.ID);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.CcsLog("付汇申请调用大赢家错误");
            }
            //
        }

        #endregion

        public UnAuditedPayExchangeApply() : base()
        {
            this.Canceled += AdminPayExchangeApply_Canceled;
            this.Audited += AdminPayExchangeApply_Audited;
            //this.DyjAudited += UnAuditedPayExchangeApply_DyjAudited;
        }

        /// <summary>
        /// 审核通过
        /// </summary>
        public void Audit()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //更新PI文件
                //  var payExchangeApplyFiles = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyFiles>()
                var payExchangeApplyFiles = new PayExchangeApplyFileView()
                      .Where(item => item.PayExchangeApplyID == this.ID && item.FileType == FileType.PIFiles);
                var FileIDs = this.PayExchangeApplyFiles.Select(item => item.ID);
                var dbFileIDs = payExchangeApplyFiles.Select(item => item.ID);
                foreach (var ID in dbFileIDs)
                {
                    if (!FileIDs.Contains(ID))
                    {
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.PayExchangeApplyFiles>(
                            new { Status = Status.Delete }, item => item.ID == ID);
                        new CenterFilesTopView().Modify(new { Status = FileDescriptionStatus.Delete }, ID);
                    }
                }
                //上传付汇PI文件
                foreach (var file in this.PayExchangeApplyFiles)
                {
                    if (string.IsNullOrEmpty(file.ID))
                    {
                        //file.ID = Needs.Overall.PKeySigner.Pick(PKeyType.PayExchangeApplyFile);
                        //file.PayExchangeApplyID = this.ID;
                        //file.AdminID = this.Admin.ID;
                        //file.FileType = FileType.PIFiles;
                        //file.CreateDate = DateTime.Now;
                        //file.Status = Status.Normal;
                        //reponsitory.Insert(file.ToLinq());
                        #region 付汇PI上传中心
                        var dic = new { CustomName = file.FileName, ApplicationID = this.ID, AdminID = file.ErmAdminID };
                        var centerType = Needs.Ccs.Services.Models.ApiModels.Files.FileType.PIFiles;
                        var result = Needs.Ccs.Services.Models.CenterFilesTopView.Upload(FileDirectory.Current.FilePath + @"\" + file.Url, centerType, dic);
                        #endregion
                    }
                }

                ////保存付汇委托书
                //if (this.ProxyFile != null)
                //{
                //    if (string.IsNullOrEmpty(this.ProxyFile.ID))
                //    {
                //        // Layer.Data.Sqls.ScCustoms.PayExchangeApplyFiles applyFile = new Layer.Data.Sqls.ScCustoms.PayExchangeApplyFiles();
                //        //applyFile.ID = Needs.Overall.PKeySigner.Pick(PKeyType.PayExchangeApplyFile);
                //        //applyFile.PayExchangeApplyID = this.ID;
                //        //applyFile.AdminID = this.ProxyFile.AdminID;
                //        //applyFile.Name = ProxyFile.FileName;
                //        //applyFile.FileType = (int)FileType.PayExchange;
                //        //applyFile.FileFormat = ProxyFile.FileFormat;
                //        //applyFile.Url = ProxyFile.Url;
                //        //applyFile.Status = (int)Status.Normal;
                //        //applyFile.CreateDate = DateTime.Now;
                //        //reponsitory.Insert(applyFile);
                //        #region 付汇委托书上传中心
                //        //if (string.IsNullOrEmpty(file.ID))
                //        //{
                //        var dic = new { CustomName = ProxyFile.FileName, ApplicationID = this.ID, AdminID = ProxyFile.ErmAdminID };
                //        var centerType = Needs.Ccs.Services.Models.ApiModels.Files.FileType.PayExchange;
                //        var url = FileDirectory.Current.FilePath + @"\" + ProxyFile.Url;
                //        //本地文件上传到服务器
                //        var result = Needs.Ccs.Services.Models.CenterFilesTopView.Upload(url, centerType, dic);
                //        //}
                //        #endregion
                //    }
                //    else
                //    {
                //        //var payExchangeApplyFile = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyFiles>()
                //        //    .Where(item => item.ID == this.ProxyFile.ID).FirstOrDefault();
                //        var payExchangeApplyFile = new PayExchangeApplyFileView().Where(x => x.ID == this.ProxyFile.ID).OrderByDescending(x => x.CreateDate).FirstOrDefault();
                //        payExchangeApplyFile.FileName = ProxyFile.FileName;
                //        payExchangeApplyFile.FileFormat = ProxyFile?.FileFormat;
                //        if (payExchangeApplyFile.Url != ProxyFile.Url)
                //        {
                //            payExchangeApplyFile.CreateDate = DateTime.Now;
                //        }
                //        payExchangeApplyFile.Url = ProxyFile.Url;
                //        //  reponsitory.Update(payExchangeApplyFile, item => item.ID == this.ProxyFile.ID);
                //        //new CenterFilesTopView().Modify(payExchangeApplyFile, payExchangeApplyFile.ID);
                //        new CenterFilesTopView().Modify(new
                //        {
                //            ID = payExchangeApplyFile.ID,
                //            ApplicationID = payExchangeApplyFile.PayExchangeApplyID,
                //            CustomName = payExchangeApplyFile.FileName,
                //            Type = (int)payExchangeApplyFile.FileType,
                //            Url = payExchangeApplyFile.Url,
                //            CreateDate = payExchangeApplyFile.CreateDate,
                //            Status = (int)payExchangeApplyFile.Status,
                //        }, payExchangeApplyFile.ID);
                //    }
                //}

                //更新付汇申请状态为已审核
                if (this.PayExchangeApplyStatus == PayExchangeApplyStatus.Auditing)
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>(
                            new
                            {
                                ExchangeRate = this.ExchangeRate,
                                IsAdvanceMoney = this.IsAdvanceMoney,
                                PayExchangeApplyStatus = PayExchangeApplyStatus.Audited
                            }, item => item.ID == this.ID);
                }
            }
            this.PayExchangeApplyStatus = Enums.PayExchangeApplyStatus.Audited;
            this.OnAudited();
            //因审核之后才可以收取RMB（收款明细），所以与跟单商量后，不在此处调用大赢家新增付汇接口
            //this.OnDyjAudited();
        }
        public void UploadProxyFile()
        {
            //保存付汇委托书
            if (this.ProxyFile != null)
            {
                if (string.IsNullOrEmpty(this.ProxyFile.ID))
                {
                    #region 付汇委托书上传中心
                    var dic = new { CustomName = ProxyFile.FileName, ApplicationID = this.ID, AdminID = ProxyFile.ErmAdminID };
                    var centerType = Needs.Ccs.Services.Models.ApiModels.Files.FileType.PayExchange;
                    var url = FileDirectory.Current.FilePath + @"\" + ProxyFile.Url;
                    //本地文件上传到服务器
                    var result = Needs.Ccs.Services.Models.CenterFilesTopView.Upload(url, centerType, dic);
                    #endregion
                }
                else
                {
                    var payExchangeApplyFile = new PayExchangeApplyFileView().Where(x => x.ID == this.ProxyFile.ID).OrderByDescending(x => x.CreateDate).FirstOrDefault();
                    payExchangeApplyFile.FileName = ProxyFile.FileName;
                    payExchangeApplyFile.FileFormat = ProxyFile?.FileFormat;
                    if (payExchangeApplyFile.Url != ProxyFile.Url)
                    {
                        payExchangeApplyFile.CreateDate = DateTime.Now;
                    }
                    payExchangeApplyFile.Url = ProxyFile.Url;
                    new CenterFilesTopView().Modify(new
                    {
                        ID = payExchangeApplyFile.ID,
                        ApplicationID = payExchangeApplyFile.PayExchangeApplyID,
                        CustomName = payExchangeApplyFile.FileName,
                        Type = (int)payExchangeApplyFile.FileType,
                        Url = payExchangeApplyFile.Url,
                        CreateDate = payExchangeApplyFile.CreateDate,
                        Status = (int)payExchangeApplyFile.Status,
                    }, payExchangeApplyFile.ID);
                }
            }
        }
        /// <summary>
        /// 审核取消
        /// </summary>
        public void Cancel()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>(
                    new
                    {
                        UpdateDate = DateTime.Now,
                        PayExchangeApplyStatus = Enums.PayExchangeApplyStatus.Cancled
                    }, item => item.ID == this.ID);

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
            this.PayExchangeApplyStatus = Enums.PayExchangeApplyStatus.Cancled;
            this.OnCanceled();
        }

        public void OnCanceled()
        {
            if (this != null && this.Canceled != null)
            {
                //成功后触发事件
                this.Canceled(this, new PayExchangeApplyCanceledEventArgs(this));
            }
        }

        public void OnAudited()
        {
            if (this != null && this.Audited != null)
            {
                //成功后触发事件
                this.Audited(this, new PayExchangeApplyAuditedEventArgs(this));
            }
        }

        public void OnDyjAudited()
        {
            if (this != null && this.DyjAudited != null)
            {
                //成功后触发事件
                this.DyjAudited(this, new DyjPayExchangeApplyEventArgs(this));
            }
        }

    }

    /// <summary>
    /// 待审批的付汇申请
    /// </summary>
    public sealed class UnApprovalPayExchangeApply : PayExchangeApply
    {
        /// <summary>
        /// 付汇申请的总金额
        /// </summary>
        public decimal? TotalAmount
        {
            get
            {
                return this.PayExchangeApplyItems.Sum(item => item.Amount);
            }
        }

        /// <summary>
        /// 审批人
        /// </summary>
        private Admin Operator { get; set; }

        /// <summary>
        /// 指定的付款人
        /// </summary>
        private Admin Payer { get; set; }

        public void SetOperator(Admin admin)
        {
            this.Operator = admin;
        }

        public Admin GetOperator()
        {
            return this.Operator;
        }

        public void SetPayer(Admin admin)
        {
            this.Payer = admin;
        }

        public Admin GetPayer()
        {
            return this.Payer;
        }

        #region 事件

        /// <summary>
        /// 当审批通过时发生
        /// </summary>
        public event PayExchangeApplyApprovaledHanlder Approvaled;

        /// <summary>
        /// 当审批取消时发生
        /// </summary>
        public event PayExchangeApplyApprovalCanceledHanlder ApprovalCanceled;

        /// <summary>
        /// 审批通过，调用大赢家
        /// </summary>
        public event DyjPayExchangeApprovalHanlder DyjApproval;

        private void AdminPayExchangeApply_Approvaled(object sender, PayExchangeApplyApprovaledEventArgs e)
        {
            var apply = (UnApprovalPayExchangeApply)e.PayExchangeApply;

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //生成付款通知
                PaymentNotice notice = new PaymentNotice();
                notice.Admin = apply.Operator;
                notice.Payer = apply.Payer;
                notice.PayExchangeApply = this;
                notice.PayFeeType = FinanceFeeType.Product;
                notice.FeeDesc = "货款";
                notice.PayeeName = apply.SupplierName;
                notice.BankName = apply.BankName;
                notice.BankAccount = apply.BankAccount;
                notice.Amount = (decimal)apply.TotalAmount;
                notice.Currency = apply.Currency;
                notice.ExchangeRate = apply.ExchangeRate;
                notice.PayDate = apply.ExpectPayDate == null ? DateTime.Now : (DateTime)apply.ExpectPayDate;
                notice.PayType = apply.PaymentType;
                notice.Summary = e.Summary;
                notice.Enter();

                //生成付款通知项
                foreach (var item in this.PayExchangeApplyItems)
                {
                    PaymentNoticeItem noticeItem = new PaymentNoticeItem();
                    noticeItem.PaymentNoticeID = notice.ID;
                    noticeItem.OrderID = item.OrderID;
                    noticeItem.PayFeeType = FinanceFeeType.Product;
                    noticeItem.Amount = item.Amount;
                    noticeItem.Currency = apply.Currency;
                    noticeItem.Enter();
                }
            }

            var admin = apply.Operator;
            if (admin != null)
            {
                e.PayExchangeApply.Log(admin, "财务经理[" + admin.ByName + "]审批通过了付汇申请，等待付款");
            }

            //计算是否产生 PayExchangeSwapedNotices
            var payExchangeApplyItems = apply.PayExchangeApplyItems;

            foreach (var item in payExchangeApplyItems)
            {
                GeneratePayExchangeSwapedNotices(apply, item);
            }
        }

        /// <summary>
        /// 计算是否产生 PayExchangeSwapedNotices
        /// </summary>
        /// <param name="payExchangeApply"></param>
        /// <param name="payExchangeApplyItem"></param>
        private void GeneratePayExchangeSwapedNotices(PayExchangeApply payExchangeApply, PayExchangeApplyItem payExchangeApplyItem)
        {
            //计算是否产生 PayExchangeSwapedNotices
            //已经被用了多少, SwapNotice (DecHead)
            //客户申请了多少, PayExchangeApply (Order)

            //A = 报关总金额 - 已经被用了多少, SwapNotice (DecHead) = 1200 - 200（至少含有别人, 别人的 > 0） = 1000
            //B = 客户申请了多少, PayExchangeApply (Order)

            // B = 100,   需补充换汇金额 = 0
            // B = 800,   需补充换汇金额 = 0
            // B = 1100,  需补充换汇金额 = 100,  再来一个 B = 100, 需补充换汇金额 = 100

            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    var existSwapedNotices = (from payExchangeSwapedNotice in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeSwapedNotices>()
                                              join decHeads in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>()
                                                      on payExchangeSwapedNotice.DecHeadID equals decHeads.ID
                                              where payExchangeSwapedNotice.Status == (int)Enums.Status.Normal
                                                 && decHeads.CusDecStatus != "04"
                                                 && decHeads.OrderID == payExchangeApplyItem.OrderID
                                              select new
                                              {
                                                  DecHeadID = payExchangeSwapedNotice.DecHeadID,
                                                  OrderID = decHeads.OrderID,
                                              }).ToList();

                    if (existSwapedNotices != null && existSwapedNotices.Any())
                    {
                        //已有补的，直接插入
                        reponsitory.Insert<Layer.Data.Sqls.ScCustoms.PayExchangeSwapedNotices>(new Layer.Data.Sqls.ScCustoms.PayExchangeSwapedNotices
                        {
                            ID = Guid.NewGuid().ToString("N"),
                            DecHeadID = existSwapedNotices[0].DecHeadID,
                            UnHandleAmount = payExchangeApplyItem.Amount * ConstConfig.TransPremiumInsurance,
                            HandleStatus = (int)Enums.SwapedNoticeHandleStatus.UnHandle,
                            Status = (int)Enums.Status.Normal,
                            CreateTime = DateTime.Now,
                            UpdateTime = DateTime.Now,
                        });
                    }
                    else
                    {
                        //没有补的，计算后插入
                        var NeedAddSwapedApply = (from swapedApply in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapedApplyView>()
                                                  where swapedApply.OrderID == payExchangeApplyItem.OrderID && swapedApply.NeedAdd > 0
                                                  select new
                                                  {
                                                      DecHeadID = swapedApply.DecHeadID,
                                                      OrderID = swapedApply.OrderID,
                                                      NeedAdd = swapedApply.NeedAdd,
                                                  }).FirstOrDefault();

                        if (NeedAddSwapedApply != null && NeedAddSwapedApply.NeedAdd != null)
                        {
                            reponsitory.Insert<Layer.Data.Sqls.ScCustoms.PayExchangeSwapedNotices>(new Layer.Data.Sqls.ScCustoms.PayExchangeSwapedNotices
                            {
                                ID = Guid.NewGuid().ToString("N"),
                                DecHeadID = NeedAddSwapedApply.DecHeadID,
                                UnHandleAmount = Convert.ToDecimal(NeedAddSwapedApply.NeedAdd),
                                HandleStatus = (int)Enums.SwapedNoticeHandleStatus.UnHandle,
                                Status = (int)Enums.Status.Normal,
                                CreateTime = DateTime.Now,
                                UpdateTime = DateTime.Now,
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.CcsLog("计算是否产生 PayExchangeSwapedNotices");
            }
        }

        private void AdminPayExchangeApply_ApprovalCanceled(object sender, PayExchangeApplyApprovalCanceledEventArgs e)
        {
            var apply = (UnApprovalPayExchangeApply)e.PayExchangeApply;
            var admin = apply.Operator;
            if (admin != null)
            {
                e.PayExchangeApply.Log(admin, "财务经理[" + admin.ByName + "]审批退回了付汇申请，退回原因：" + e.Summary);
            }
        }

        private void UnApprovalPayExchangeApply_DyjApproval(object sender, DyjPayExchangeApprovalEventArgs e)
        {

            //付汇申请同步大赢家
            try
            {
                var result = new Finance.DyjFinance.DyjPayExchange(e.DyjPayExchangeApply);
                var data = result.PostToDYJ();
                //付汇接口返回值，保存进付汇申请
                if (!string.IsNullOrEmpty(data))
                {
                    using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                    {
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>(new
                        {
                            DyjID = data,
                        }, item => item.ID == this.ID);
                    }

                    ////付汇审批动作同步
                    //e.DyjPayExchangeApply.DyjID = data;
                    //var approve = new Finance.DyjFinance.DyjPayApprove(e.DyjPayExchangeApply, e.Summary, e.IsPass);
                    //approve.PostToDYJ();
                }
            }
            catch (Exception ex)
            {
                ex.CcsLog("付汇申请调用大赢家错误");
            }


            //审批同步大赢家
            //try
            //{
            //    var result = new Finance.DyjFinance.DyjPayApprove(e.DyjPayExchangeApply,e.Summary,e.IsPass);
            //    result.PostToDYJ();
            //}
            //catch (Exception ex)
            //{
            //    ex.CcsLog("付汇申请调用大赢家错误");
            //}
        }

        #endregion

        public UnApprovalPayExchangeApply() : base()
        {
            this.Approvaled += AdminPayExchangeApply_Approvaled;
            this.ApprovalCanceled += AdminPayExchangeApply_ApprovalCanceled;
            this.DyjApproval += UnApprovalPayExchangeApply_DyjApproval;
        }

        /// <summary>
        /// 审批通过
        /// </summary>
        public void Approval(string summary)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>(new
                {
                    UpdateDate = DateTime.Now,
                    PayExchangeApplyStatus = Enums.PayExchangeApplyStatus.Approvaled
                }, item => item.ID == this.ID);
            }
            this.PayExchangeApplyStatus = Enums.PayExchangeApplyStatus.Approvaled;
            this.OnApprovaled(summary);
            this.OnDyjApproval(summary, true);
        }

        /// <summary>
        /// 审批取消
        /// </summary>
        public void ApprovalCancel(string summary)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>(new
                {
                    UpdateDate = DateTime.Now,
                    PayExchangeApplyStatus = Enums.PayExchangeApplyStatus.Auditing
                }, item => item.ID == this.ID);
            }
            this.PayExchangeApplyStatus = Enums.PayExchangeApplyStatus.Auditing;
            this.OnApprovalCanceled(summary);
            this.OnDyjApproval(summary, false);
        }

        public void OnApprovaled(string summary)
        {
            if (this != null && this.Approvaled != null)
            {
                //成功后触发事件
                this.Approvaled(this, new PayExchangeApplyApprovaledEventArgs(this, summary));
            }
        }

        public void OnApprovalCanceled(string summary)
        {
            if (this != null && this.ApprovalCanceled != null)
            {
                //成功后触发事件
                this.ApprovalCanceled(this, new PayExchangeApplyApprovalCanceledEventArgs(this, summary));
            }
        }

        public void OnDyjApproval(string summary, bool isPass)
        {
            if (this != null && this.DyjApproval != null)
            {
                //成功后触发事件
                this.DyjApproval(this, new DyjPayExchangeApprovalEventArgs(this, summary, isPass));
            }
        }
    }

    /// <summary>
    /// 待完成的付汇申请
    /// </summary>
    public sealed class UnCompletePayExchangeApply : PayExchangeApply
    {
        /// <summary>
        /// 付款人
        /// </summary>
        private Admin Operator { get; set; }

        public void SetOperator(Admin admin)
        {
            this.Operator = admin;
        }

        /// <summary>
        /// 当已付款时发生
        /// </summary>
        public event PayExchangeApplyPaidHanlder Paid;

        private void AdminPayExchangeApply_Paid(object sender, PayExchangeApplyPaidEventArgs e)
        {
            var apply = (UnCompletePayExchangeApply)e.PayExchangeApply;
            var admin = apply.Operator;
            if (admin != null)
            {
                e.PayExchangeApply.Log(admin, "财务[" + admin.RealName + "]完成了付汇申请。");
            }
        }

        public UnCompletePayExchangeApply() : base()
        {
            this.Paid += AdminPayExchangeApply_Paid;
        }

        /// <summary>
        /// 付款完成
        /// </summary>
        public void Pay()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>(new
                {
                    UpdateDate = DateTime.Now,
                    PayExchangeApplyStatus = Enums.PayExchangeApplyStatus.Completed
                }, item => item.ID == this.ID);
            }
            this.PayExchangeApplyStatus = Enums.PayExchangeApplyStatus.Completed;
            this.OnPaid();
        }

        public void OnPaid()
        {
            if (this != null && this.Paid != null)
            {
                //成功后触发事件
                this.Paid(this, new PayExchangeApplyPaidEventArgs(this));
            }
        }
    }
}
