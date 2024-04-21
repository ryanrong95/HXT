using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Yahv.Services.Models;
using Yahv.Services.Views;
using Yahv.Underly;
using Yahv.Underly.Erps;
using Yahv.Usually;
using Yahv.Utils.Converters.Contents;
using YaHv.Csrm.Services.Extends;

namespace YaHv.Csrm.Services.Models.Origins
{
    /// <summary>
    /// 供应商
    /// </summary>
    public class Supplier : Yahv.Linq.IUnique
    {
        #region 构造函数
        public Supplier()
        {
            this.SupplierStatus = ApprovalStatus.Waitting;
            this.CreateDate = this.UpdateDate = DateTime.Now;
        }
        #endregion
        #region 属性
        /// <summary>
        /// 唯一码
        /// </summary>
        /// <chenhan>保障全局唯一</chenhan>
        public string ID { set; get; }
        /// <summary>
        /// 供应商类型
        /// </summary>
        public SupplierType Type { set; get; }

        /// <summary>
        /// 大赢家编码
        /// </summary>
        public string DyjCode { set; get; }

        /// <summary>
        /// 纳税人识别号
        /// </summary>
        public string TaxperNumber { set; get; }

        /// <summary>
        /// 供应商性质（保留开发）
        /// </summary>
        public SupplierNature Nature { set; get; }

        /// <summary>
        /// 等级
        /// </summary>
        public SupplierGrade? Grade { set; get; }

        /// <summary>
        /// 所在地区
        /// </summary>
        public AreaType AreaType { set; get; }

        /// <summary>
        /// 是否可以带票采购
        /// </summary>
        public InvoiceType? InvoiceType { set; get; }
        /// <summary>
        /// 是否是原厂供应商
        /// </summary>
        public bool IsFactory { set; get; }
        /// <summary>
        /// 代理公司（内部公司）
        /// </summary>
        public string AgentCompany { set; get; }

        /// <summary>
        /// 客户状态
        /// </summary>
        public ApprovalStatus SupplierStatus { set; get; }
        /// <summary>
        /// 基本信息
        /// </summary>
        public Enterprise Enterprise { set; get; }
        /// <summary>
        /// 账期
        /// </summary>
        public int RepayCycle { set; get; }
        /// <summary>
        /// 额度
        /// </summary>
        public decimal Price { set; get; }
        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime UpdateDate { get; set; }
        public string CreatorID { set; get; }
        /// <summary>
        /// 添加人
        /// </summary>
        public Admin Creator { get; internal set; }
        public string Place { set; get; }
        /// <summary>
        /// 是否货代公司
        /// </summary>
        public bool IsForwarder { set; get; }
        #endregion


        #region 扩展（受益人，联系人，优势品牌，优势型号）
        /// <summary>
        /// 供应商的所有受益人
        /// </summary>
        Views.Rolls.TradeBeneficiariesRoll beneficiaries;
        /// <summary>
        /// 客户受益人信息
        /// </summary>
        public Views.Rolls.TradeBeneficiariesRoll Beneficiaries
        {
            get
            {
                if (this.beneficiaries == null || this.beneficiaries.Disposed)
                {
                    this.beneficiaries = new Views.Rolls.TradeBeneficiariesRoll(this.Enterprise);
                }
                return this.beneficiaries;
            }
        }
        /// <summary>
        /// 供应商的所有发票信息
        /// </summary>
        Views.Rolls.TradingInvoicesRoll invoices;
        public Views.Rolls.TradingInvoicesRoll Invoices
        {
            get
            {
                if (this.invoices == null || this.invoices.Disposed)
                {
                    this.invoices = new Views.Rolls.TradingInvoicesRoll(this.Enterprise);
                }
                return this.invoices;
            }
        }
        /// <summary>
        /// 供应商的联系人信息
        /// </summary>
        Views.Rolls.TradingContactsRoll contacts;
        public Views.Rolls.TradingContactsRoll Contacts
        {
            get
            {
                if (this.contacts == null || this.contacts.Disposed)
                {
                    this.contacts = new Views.Rolls.TradingContactsRoll(this.Enterprise);
                }
                return this.contacts;
            }
        }
        //特色
        Advantage advantage;
        public Advantage Advantage
        {
            get
            {
                if (this.advantage == null)
                {
                    this.advantage = new Views.Rolls.AdvantageRoll(this.Enterprise).FirstOrDefault();
                }
                return advantage;
            }
        }
        Views.Rolls.AutoQuotesRoll autoQuotes;
        public Views.Rolls.AutoQuotesRoll AutoQuotes
        {
            get
            {
                if (this.autoQuotes == null || this.autoQuotes.Disposed)
                {
                    this.autoQuotes = new Views.Rolls.AutoQuotesRoll(this.Enterprise);
                }
                return autoQuotes;
            }
        }

        /// <summary>
        ///供应商的采购人
        /// </summary>
        //Views.Rolls.BusinessAdminsRoll purchasers;
        //public Views.Rolls.BusinessAdminsRoll Purchasers
        //{
        //    get
        //    {
        //        if (this.purchasers == null || this.purchasers.Disposed)
        //        {
        //            this.purchasers = new Views.Rolls.BusinessAdminsRoll(this.Enterprise, MapsType.Supplier);
        //        }
        //        return purchasers;
        //    }
        //}
        #endregion

        #region 事件
        /// <summary>
        /// EnterSuccess
        /// </summary>
        virtual public event SuccessHanlder EnterSuccess;
        /// <summary>
        /// AbandonSuccess
        /// </summary>
        virtual public event SuccessHanlder AbandonSuccess;
        /// <summary>
        /// 状态异常
        /// </summary>
        virtual public event ErrorHanlder StatusUnnormal;
        #endregion

        #region 持久化
        virtual public void Enter()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                this.Enterprise.Enter();
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    //1.Suppliers存在
                    if (repository.GetTable<Layers.Data.Sqls.PvbCrm.Suppliers>().Any(item => item.ID == this.Enterprise.ID))
                    {
                        this.SupplierStatus = new Views.Origins.SuppliersOrigin()[this.Enterprise.ID].SupplierStatus;
                        if (this != null && this.StatusUnnormal != null)
                        {
                            this.StatusUnnormal(this, new ErrorEventArgs());
                        }
                    }
                    //2.Suppliers不存在
                    else
                    {
                        this.ID = this.Enterprise.ID;
                        repository.Insert(this.ToLinq());
                        //this.Admin.Binding(this.ID, MapsType.Supplier);
                    }
                }
                else
                {
                    this.SupplierStatus = ApprovalStatus.Waitting;
                    repository.Update<Layers.Data.Sqls.PvbCrm.Suppliers>(new
                    {
                        Type = (int)this.Type,
                        Nature = (int)this.Nature,
                        Grade = (int?)this.Grade,
                        DyjCode = this.DyjCode,
                        TaxperNumber = this.TaxperNumber,
                        AreaType = (int)this.AreaType,
                        InvoiceType = (int)this.InvoiceType,
                        IsFactory = this.IsFactory,
                        AgentCompany = this.AgentCompany,
                        Status = (int)this.SupplierStatus,
                        RepayCycle = (int?)this.RepayCycle,
                        Currency = (int)this.Currency,
                        Price = this.Price,
                        UpdateDate = this.UpdateDate,
                        IsForwarder = this.IsForwarder
                    }, item => item.ID == this.Enterprise.ID);
                }
                if (this != null && this.EnterSuccess != null)
                {
                    this.EnterSuccess(this, new SuccessEventArgs(this));
                }
            }
        }
        virtual public void Abandon()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvbCrm.Enterprises>(new
                {
                    Status = ApprovalStatus.Deleted
                }, item => item.ID == this.ID);
                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }

        }
        #endregion


    }

    public class TradingSupplier : Supplier
    {
        string mapsid;
        public string MapsID
        {
            get
            {
                return string.Join("", Business.Trading, MapsType.Supplier.ToString(), "_", this.ID + this.CreatorID).MD5();
            }
            set
            {
                this.mapsid = value;
            }
        }
        /// <summary>
        /// 合作公司的ID
        /// </summary>
        public string CompanyID { set; get; }
        /// <summary>
        /// 采购
        /// </summary>
        public IEnumerable<TradingAdmin> Purchasers { set; get; }
        public bool IsDefault { set; get; }
        #region 事件
        /// <summary>
        /// EnterSuccess
        /// </summary>
        override public event SuccessHanlder EnterSuccess;
        /// <summary>
        /// AbandonSuccess
        /// </summary>
        override public event SuccessHanlder AbandonSuccess;
        /// <summary>
        /// 状态异常
        /// </summary>
        override public event ErrorHanlder StatusUnnormal;
        #endregion

        #region 持久化
        override public void Enter()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                this.Enterprise.Enter();
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    //1.Suppliers存在
                    if (repository.GetTable<Layers.Data.Sqls.PvbCrm.Suppliers>().Any(item => item.ID == this.Enterprise.ID))
                    {
                        this.SupplierStatus = new Views.Origins.SuppliersOrigin()[this.Enterprise.ID].SupplierStatus;
                        if (this != null && this.StatusUnnormal != null)
                        {
                            this.StatusUnnormal(this, new ErrorEventArgs());
                        }
                    }
                    //2.Suppliers不存在
                    else
                    {
                        this.ID = this.Enterprise.ID;
                        repository.Insert(this.ToLinq());
                    }
                }
                else
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.Suppliers>(new
                    {
                        Type = (int)this.Type,
                        Nature = (int)this.Nature,
                        Grade = (int?)this.Grade,
                        DyjCode = this.DyjCode,
                        TaxperNumber = this.TaxperNumber,
                        AreaType = (int)this.AreaType,
                        InvoiceType = (int)this.InvoiceType,
                        IsFactory = this.IsFactory,
                        AgentCompany = this.AgentCompany,
                        Status = (int)this.SupplierStatus,
                        RepayCycle = (int?)this.RepayCycle,
                        Currency = (int)this.Currency,
                        Price = this.Price,
                        UpdateDate = this.UpdateDate,
                        Place = this.Place,
                        IsForwarder=this.IsForwarder
                    }, item => item.ID == this.ID);
                }

                var creator = new Views.Rolls.AdminsAllRoll()[this.CreatorID];
                if (creator != null || creator?.RoleID == FixedRole.Purchaser.GetFixedID() || creator?.RoleID == FixedRole.PurchasingManager.GetFixedID())
                {
                    //添加权限关系
                    if (!repository.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Any(item => item.ID == this.MapsID))
                    {
                        repository.Insert(new Layers.Data.Sqls.PvbCrm.MapsBEnter
                        {
                            ID = this.MapsID,
                            Bussiness = (int)Business.Trading,
                            Type = (int)MapsType.Supplier,
                            EnterpriseID = this.ID,
                            SubID = this.CreatorID,
                            CreateDate = this.CreateDate,
                            CtreatorID = this.CreatorID,
                            IsDefault = this.IsDefault
                        });
                    }

                }
                if (this != null && this.EnterSuccess != null)
                {
                    this.EnterSuccess(this, new SuccessEventArgs(this));
                }
            }
        }

        //删除关系
        override public void Abandon()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                repository.Delete<Layers.Data.Sqls.PvbCrm.MapsBEnter>(item => item.ID == this.MapsID);
                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }

        }
        #endregion
    }
}





#region 合作公司&采购  关系的逻辑
//override public void Enter()
//{
//    using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
//    {
//        this.Enterprise.Enter();
//        if (string.IsNullOrWhiteSpace(this.ID))
//        {
//            //1.Suppliers存在
//            if (repository.GetTable<Layers.Data.Sqls.PvbCrm.Suppliers>().Any(item => item.ID == this.Enterprise.ID))
//            {
//                this.SupplierStatus = new Views.Origins.SuppliersOrigin()[this.Enterprise.ID].SupplierStatus;
//                if (this != null && this.StatusUnnormal != null)
//                {
//                    this.StatusUnnormal(this, new ErrorEventArgs());
//                }
//            }
//            //2.Suppliers不存在
//            else
//            {
//                this.ID = this.Enterprise.ID;
//                repository.Insert(this.ToLinq());
//            }
//        }
//        else
//        {
//            repository.Update<Layers.Data.Sqls.PvbCrm.Suppliers>(new
//            {
//                Type = (int)this.Type,
//                Nature = (int)this.Nature,
//                Grade = (int?)this.Grade,
//                DyjCode = this.DyjCode,
//                TaxperNumber = this.TaxperNumber,
//                AreaType = (int)this.AreaType,
//                InvoiceType = (int)this.InvoiceType,
//                IsFactory = this.IsFactory,
//                AgentCompany = this.AgentCompany,
//                Status = (int)this.SupplierStatus,
//                RepayCycle = (int?)this.RepayCycle,
//                Currency = (int)this.Currency,
//                Price = this.Price,
//                UpdateDate = this.UpdateDate
//            }, item => item.ID == this.ID);
//        }
//        if (!string.IsNullOrWhiteSpace(this.CompanyID))
//        {
//            var creator = new Views.Rolls.AdminsAllRoll()[this.CreatorID];
//            if (creator != null || creator?.RoleID == FixedRole.Purchaser.GetFixedID() || creator?.RoleID == FixedRole.PurchasingManager.GetFixedID())
//            {
//                //添加权限关系
//                if (!repository.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Any(item => item.ID == this.MapsID))
//                {
//                    repository.Insert(new Layers.Data.Sqls.PvbCrm.MapsBEnter
//                    {
//                        ID = this.MapsID,
//                        Bussiness = (int)Business.Trading_Purchase,
//                        Type = (int)MapsType.Supplier,
//                        EnterpriseID = this.ID,
//                        SubID = this.CompanyID,
//                        CreateDate = this.CreateDate,
//                        CtreatorID = this.CreatorID,
//                        IsDefault = this.IsDefault
//                    });
//                }
//            }
//        }

//        if (this != null && this.EnterSuccess != null)
//        {
//            this.EnterSuccess(this, new SuccessEventArgs(this));
//        }
//    }
//}
#endregion