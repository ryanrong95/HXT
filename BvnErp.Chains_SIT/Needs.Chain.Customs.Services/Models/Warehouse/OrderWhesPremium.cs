using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Linq;
using Needs.Utils;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 订单库房费用
    /// </summary>
    public class OrderWhesPremium : IUnique, IPersist, IFulError, IFulSuccess
    {
        #region 属性

        public string ID { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 客户信息
        /// </summary>
        public string ClientID { get; set; }
        public Client Client { get; set; }

        /// <summary>
        /// 添加人
        /// </summary>
        public string CreaterID { get; set; }
        public Admin Creater { get; set; }

        /// <summary>
        /// 审批人
        /// </summary>
        public string ApproverID { get; set; }
        public Admin Approver { get; set; }

        public WarehouseType WarehouseType { get; set; }

        public WarehousePremiumType WarehousePremiumType { get; set; }

        public WhsePaymentType WhsePaymentType { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 单位名称
        /// </summary>
        public string UnitName { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 汇率
        /// </summary>
        public decimal ExchangeRate { get; set; }

        /// <summary>
        /// 审批费用
        /// </summary>
        public decimal ApprovalPrice { get; set; }

        public WarehousePremiumsStatus WarehousePremiumsStatus { get; set; }

        public Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        #endregion

        //操作人
        protected Admin Operator { get; set; }

        public void SetOperator(Admin admin)
        {
            this.Operator = admin;
        }

        /// <summary>
        /// 费用附件
        /// </summary>
        OrderWhesPremiumFiles files;
        public OrderWhesPremiumFiles Files
        {
            get
            {
                if (files == null)
                {
                    using (var view = new Views.OrderWhesPremiumFileView())
                    {
                        var query = view.Where(item => item.OrderWhesPremiumID == this.ID);
                        this.files = new OrderWhesPremiumFiles(query);
                    }
                }
                return this.files;
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                this.files = new OrderWhesPremiumFiles(value, new Action<OrderWhesPremiumFile>(delegate (OrderWhesPremiumFile item)
                {
                    item.OrderWhesPremiumID = this.ID;
                }));
            }
        }

        #region 事件

        public event ErrorHanlder EnterError;
        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;
        public event OrderWhesPremiumCanceledHanlder Canceled;
        public event OrderWhesPremiumApprovaledHanlder Approvaled;

        private void OrderWhesPremium_EnterSuccess(object sender, SuccessEventArgs e)
        {
            var premium = (OrderWhesPremium)e.Object;
            if (premium == null)
            {
                return;
            }

            string OrderID = premium.OrderID;
            var vendor = new VendorContext(VendorContextInitParam.OrderID, OrderID).Current1;


            var admin = premium?.Operator;
            if (admin != null)
            {
                if (premium.WarehousePremiumsStatus == Enums.WarehousePremiumsStatus.Payed)
                {
                    if (premium.CreateDate == premium.UpdateDate)
                        this.Log(admin, "库房管理员[" + admin.RealName + "]新增了已付款费用");
                    else
                        this.Log(admin, "库房管理员[" + admin.RealName + "]编辑了已付款费用");
                }

                else
                {
                    if (premium.CreateDate == premium.UpdateDate)
                    {
                        this.Log(admin, "库房管理员[" + admin.RealName + "]新增了未付款费用");
                    }
                    else
                    {
                        this.Log(admin, "库房管理员[" + admin.RealName + "]编辑了未付款费用");
                    }
                }
            }
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //保存附件
                var OrderWhesPremiumFiles = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderWhesPremiumFiles>()
                    .Where(item => item.OrderWhesPremiumID == premium.ID && item.FileType == (int)FileType.WarehousFeeFile);
                var FileIDs = premium.Files.Select(item => item.ID);
                var dbFileIDs = OrderWhesPremiumFiles.Select(item => item.ID);
                foreach (var ID in dbFileIDs)
                {
                    if (!FileIDs.Contains(ID))
                    {
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderWhesPremiumFiles>(
                            new { Status = Status.Delete }, item => item.ID == ID);
                    }
                }
                foreach (var file in premium.Files)
                {
                    if (string.IsNullOrEmpty(file.ID))
                    {
                        file.ID = Needs.Overall.PKeySigner.Pick(PKeyType.OrderWhesPremiumFile);
                        file.OrderWhesPremiumID = this.ID;
                        file.AdminID = admin.ID;
                        file.FileType = FileType.WarehousFeeFile;
                        file.CreateDate = DateTime.Now;
                        file.Status = Status.Normal;
                        reponsitory.Insert(file.ToLinq());
                    }
                }
                //新增账户收款
                if (premium.WarehousePremiumsStatus == Enums.WarehousePremiumsStatus.Payed)
                {
                    FinanceReceipt receipt = new FinanceReceipt();
                    receipt.SeqNo = "";
                    receipt.Payer = "客户供应商";
                    receipt.ReceiptType = premium.WhsePaymentType == WhsePaymentType.Cash ? PaymentType.Cash : PaymentType.TransferAccount;
                    receipt.ReceiptDate = premium.CreateDate;
                    receipt.Currency = premium.Currency;
                    var Rate = new Views.RealTimeExchangeRatesView().Where(item => item.Code == premium.Currency).SingleOrDefault().Rate;
                    receipt.Rate = Rate;
                    receipt.Amount = premium.UnitPrice * premium.Count;
                    //var account = new Views.FinanceAccountsView().Where(item => item.ID == ConstConfig.account.ID).FirstOrDefault();
                    var accountCn = vendor.ShortName + "库房现金账户";
                    var account = new Views.FinanceAccountsView().Where(item => item.AccountName.Contains(accountCn)).FirstOrDefault();
                    receipt.Account = account;
                    receipt.Vault = new FinanceVault { ID = account?.FinanceVaultID };
                    receipt.Admin = admin;
                    receipt.Summary = premium.Summary;
                    receipt.FeeType = FinanceFeeType.Incidental;
                    receipt.Enter();
                }
            }
        }

        private void OrderWhesPremium_AbandonSuccess(object sender, SuccessEventArgs e)
        {
            var premium = (OrderWhesPremium)e.Object;
            var admin = premium?.Operator;
            if (admin != null)
            {
                this.Log(admin, "库房管理员[" + admin.RealName + "]删除了费用");
            }
        }

        private void OrderWhesPremium_CancelSuccess(object sender, OrderWhesPremiumCanceledEventArgs e)
        {
            var premium = e.OrderWhesPremium;
            var admin = premium?.Operator;
            if (admin != null)
            {
                this.Log(admin, "跟单员[" + admin.RealName + "]取消了费用");
            }
        }

        private void OrderWhesPremium_ApprovalSuccess(object sender, OrderWhesPremiumApprovaledEventArgs e)
        {
            var whesPremium = e.OrderWhesPremium;
            if (whesPremium.Operator != null)
            {
                whesPremium.Log(whesPremium.Operator, "跟单员[" + whesPremium.Operator.RealName + "]审批了费用");
            }
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                string ermAdminID = null;
                if (whesPremium.Operator != null)
                {
                    var adminModel = new Views.AdminsTopView2(reponsitory).Where(t => t.OriginID == whesPremium.Operator.ID).FirstOrDefault();
                    if (adminModel != null)
                    {
                        ermAdminID = adminModel.ID;
                    }
                }

                //保存附件
                //var OrderWhesPremiumFiles = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderWhesPremiumFiles>()
                //    .Where(item => item.OrderWhesPremiumID == whesPremium.ID && item.FileType == (int)FileType.WarehousFeeFile);
                //var FileIDs = whesPremium.Files.Select(item => item.ID);
                //var dbFileIDs = OrderWhesPremiumFiles.Select(item => item.ID);
                //foreach (var ID in dbFileIDs)
                //{
                //    if (!FileIDs.Contains(ID))
                //    {
                //        reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderWhesPremiumFiles>(
                //            new { Status = Status.Delete }, item => item.ID == ID);
                //    }
                //}
                //foreach (var file in whesPremium.Files)
                //{
                //    if (string.IsNullOrEmpty(file.ID))
                //    {
                //        file.ID = Needs.Overall.PKeySigner.Pick(PKeyType.OrderWhesPremiumFile);
                //        file.OrderWhesPremiumID = whesPremium.ID;
                //        file.AdminID = whesPremium.Operator.ID;
                //        file.FileType = FileType.WarehousFeeFile;
                //        file.CreateDate = DateTime.Now;
                //        file.Status = Status.Normal;
                //        reponsitory.Insert(file.ToLinq());
                //    }
                //}




                //新加的到中心 Begin
                var centerType = Needs.Ccs.Services.Models.ApiModels.Files.FileType.None;
                List<string> toCenterFileNames = new List<string>();
                int fileNum = 0;

                foreach (var file in whesPremium.Files)
                {
                    var oldFile = new Needs.Ccs.Services.Models.CenterFilesTopView().FirstOrDefault(x => x.Url == file.URL && x.PayID == whesPremium.ID);

                    if (oldFile != null)
                    {
                        var entity = new CenterFileDescription();
                        entity.AdminID = ermAdminID;
                        entity.PayID = whesPremium.ID;
                        entity.Type = (int)centerType;
                        entity.Url = oldFile.Url;
                        entity.Status = FileDescriptionStatus.Normal;
                        entity.CreateDate = DateTime.Now;
                        entity.CustomName = file.Name;

                        fileNum++;
                        DateTime liunxStart = new DateTime(1970, 1, 1);
                        var linuxtime = (DateTime.Now - liunxStart).Ticks;
                        string topID = "F" + linuxtime + "-" + fileNum;

                        new CenterFilesTopView().Insert(entity, topID);
                    }
                    else
                    {
                        var url = FileDirectory.Current.FilePath + @"\" + file.URL;

                        toCenterFileNames.Add(url);
                    }
                }

                if (toCenterFileNames != null && toCenterFileNames.Any())
                {
                    CenterFilesTopView.Upload(centerType, new
                    {
                        //ApplicationID = this.ID,
                        AdminID = ermAdminID,
                        PayID = whesPremium.ID,
                    }, toCenterFileNames.ToArray());
                }

                //新加的到中心 End





                //库房费用跟单审核通过，如果是客户支付，则写入订单费用表
                Enums.OrderPremiumType type;
                switch (whesPremium.WarehousePremiumType)
                {
                    case Enums.WarehousePremiumType.EntryFee:
                        type = Enums.OrderPremiumType.EntryFee;
                        break;
                    case Enums.WarehousePremiumType.StorageFee:
                        type = Enums.OrderPremiumType.StorageFee;
                        break;
                    case Enums.WarehousePremiumType.UnNormalFee:
                        type = Enums.OrderPremiumType.UnNormalFee;
                        break;
                    default:
                        type = Enums.OrderPremiumType.EntryFee;
                        break;
                }

                var fee = new OrderPremium(whesPremium, type);
                fee.Admin = whesPremium.Operator;
                //费用附件
                foreach (var file in whesPremium.Files)
                {
                    fee.Files.Add(new OrderFile()
                    {
                        OrderID = fee.OrderID,
                        OrderPremiumID = fee.ID,
                        Admin = fee.Admin,
                        Name = file.Name,
                        FileType = FileType.OrderFeeFile,
                        FileFormat = file.FileFormat,
                        Url = file.URL,
                        FileStatus = OrderFileStatus.Audited
                    });
                }
                fee.Enter();
            }

            //将库房审批后的费用通给 Yahv  Begin
            try
            {
                if (whesPremium.ID.StartsWith("Receb"))
                {
                    var admin = new Needs.Ccs.Services.Views.AdminsTopView2().Where(t => t.OriginID == whesPremium.Operator.ID).FirstOrDefault();

                    Yahv.Payments.PaymentManager.Erp(admin.ErmAdminID)[whesPremium.Client.Company.Name, PurchaserContext.Current.CompanyName][Yahv.Payments.ConductConsts.代报关]
                        .Receivable.For(whesPremium.ID).ReRecord(whesPremium.ApprovalPrice);
                }
            }
            catch (Exception ex)
            {
                ex.CcsLog("库房费用待审批-传实际审批的费用");
            }
            //将库房审批后的费用通给 Yahv  End
        }

        #endregion

        public OrderWhesPremium()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
            this.WarehousePremiumsStatus = Enums.WarehousePremiumsStatus.Auditing;
            this.Status = Enums.Status.Normal;

            this.EnterSuccess += OrderWhesPremium_EnterSuccess;
            this.AbandonSuccess += OrderWhesPremium_AbandonSuccess;
            this.Canceled += OrderWhesPremium_CancelSuccess;
            this.Approvaled += OrderWhesPremium_ApprovalSuccess;
        }

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderWhesPremiums>().Count(item => item.ID == this.ID);

                    if (count == 0)
                    {
                        this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.OrderWhesPremium);
                        reponsitory.Insert(this.ToLinq());
                    }
                    else
                    {
                        this.UpdateDate = DateTime.Now;
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
        /// 去持久化
        /// </summary>
        public void Abandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderWhesPremiums>(
                    new
                    {
                        UpdateDate = DateTime.Now,
                        Status = Enums.Status.Delete
                    }, item => item.ID == this.ID);
            }
            this.OnAbandonSuccess();
        }

        /// <summary>
        /// 取消费用
        /// </summary>
        public void Cancel()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderWhesPremiums>(
                    new
                    {
                        ApproverID = this.Operator?.ID,
                        ApprovalPrice = 0M,
                        PremiumsStatus = Enums.WarehousePremiumsStatus.Audited,
                        UpdateDate = DateTime.Now,
                    },
                    item => item.ID == this.ID);
            }
            this.OnCanceled();
        }

        /// <summary>
        /// 审批费用
        /// </summary>
        public void Approval()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderWhesPremiums>(new
                {
                    ApproverID = this.Operator?.ID,
                    ApprovalPrice = this.ApprovalPrice,
                    Summary = this.Summary,
                    PremiumsStatus = Enums.WarehousePremiumsStatus.Audited,
                    UpdateDate = DateTime.Now,
                }, item => item.ID == this.ID);
            }
            this.OnApprovaled();
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

        virtual public void OnCanceled()
        {
            if (this != null && this.Canceled != null)
            {
                //成功后触发事件
                this.Canceled(this, new OrderWhesPremiumCanceledEventArgs(this));
            }
        }

        virtual public void OnApprovaled()
        {
            if (this != null && this.Approvaled != null)
            {
                //成功后触发事件
                this.Approvaled(this, new OrderWhesPremiumApprovaledEventArgs(this));
            }
        }

    }

    public class AdminOrderWhesPremium : OrderWhesPremium
    {
        public AdminOrderWhesPremium() : base()
        {

        }
    }
}