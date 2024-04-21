using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Yahv.Underly;
using Yahv.Underly.Erps;
using Yahv.Usually;
using Yahv.Utils.Converters.Contents;
using YaHv.Csrm.Services.Extends;

namespace YaHv.Csrm.Services.Models.Origins
{

    /// <summary>
    /// 客户
    /// </summary>
    public class Client : Yahv.Linq.IUnique
    {
        #region 构造函数
        public Client()
        {
            this.Vip = VIPLevel.NonVIP;
            this.ClientStatus = ApprovalStatus.Waitting;
            this.CreateDate = this.UpdateDate = DateTime.Now;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 唯一码
        /// </summary>
        /// <chenhan>保障全局唯一</chenhan>
        public string ID { get; set; }
        /// <summary>
        /// 客户性质
        /// </summary>
        public ClientType Nature { set; get; }
        /// <summary>
        /// 国家、地区（Origin的英文简称）
        /// </summary>
        public string Place { set; get; }
        /// <summary>
        /// 等级
        /// </summary>
        public ClientGrade? Grade { set; get; }
        /// <summary>
        /// 客户类型
        /// </summary>
        public AreaType AreaType { set; get; }
        /// <summary>
        /// 大赢家编码
        /// </summary>
        public string DyjCode { set; get; }
        /// <summary>
        /// 纳税人识别号
        /// </summary>
        public string TaxperNumber { set; get; }
        /// <summary>
        /// vip等级1-9，-1仅VIP，0非Vip
        /// </summary>
        public VIPLevel Vip { get; set; }
        /// <summary>
        /// 客户状态
        /// </summary>
        public ApprovalStatus ClientStatus { set; get; }

        /// <summary>
        /// 基本信息
        /// </summary>
        public Enterprise Enterprise { set; get; }
        /// <summary>
        /// 是否默认
        /// </summary>
        public bool IsDefault { set; get; }
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
        /// <summary>
        /// 是否重点客户
        /// </summary>
        public bool Major { set; get; }
        #endregion


        #region 扩展属性(发票，到货地址，联系人,优势品牌，优势型号)
        /// <summary>
        /// /// <summary>
        /// 客户的所有发票信息
        /// </summary>
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
        /// 客户的所有到货地址
        /// </summary>
        Views.Rolls.TradingConsigneesRoll consignees;
        public Views.Rolls.TradingConsigneesRoll Consignees
        {
            get
            {
                if (this.consignees == null || this.consignees.Disposed)
                {
                    this.consignees = new Views.Rolls.TradingConsigneesRoll(this.Enterprise);
                }
                return this.consignees;
            }
        }
        /// <summary>
        /// 客户的所有联系人信息
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
        /// <summary>
        /// 合作的内部公司
        /// </summary>
        //Views.Rolls.CooperatersRoll cooperaters;
        //public Views.Rolls.CooperatersRoll Cooperaters
        //{
        //    get
        //    {
        //        if (this.cooperaters == null || this.cooperaters.Disposed)
        //        {
        //            this.cooperaters = new Views.Rolls.CooperatersRoll(this.Enterprise);
        //        }
        //        return cooperaters;
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
                    //1.Clients存在
                    if (repository.GetTable<Layers.Data.Sqls.PvbCrm.Clients>().Any(item => item.ID == this.Enterprise.ID))
                    {
                        this.ClientStatus = new Views.Origins.ClientsOrigin()[this.Enterprise.ID].ClientStatus;
                        if (this != null && this.StatusUnnormal != null)
                        {
                            this.StatusUnnormal(this, new ErrorEventArgs());
                        }
                    }
                    //2.Clients不存在
                    else
                    {
                        this.ID = this.Enterprise.ID;
                        //repository.Insert(this.ToLinq());
                    }
                }
                else
                {
                    //repository.Update<Layers.Data.Sqls.PvbCrm.Clients>(new
                    //{
                    //    AreaType = (int)this.AreaType,
                    //    Nature = (int)this.Nature,
                    //    Grade = (int)this.Grade,
                    //    DyjCode = this.DyjCode,
                    //    TaxperNumber = this.TaxperNumber,
                    //    Vip = (int?)this.Vip,
                    //    Status = (int)this.ClientStatus,
                    //    UpdateDate = this.UpdateDate
                    //}, item => item.ID == this.Enterprise.ID);
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
                repository.Update<Layers.Data.Sqls.PvbCrm.Clients>(new
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

    public class TradingClient : Client
    {
        string mapsid;
        public string MapsID
        {
            get
            {
                return string.Join("", Business.Trading, MapsType.Client.ToString(), "_", this.ID + this.saleid).MD5();
            }
            set
            {
                this.mapsid = value;
            }
        }
        /// <summary>
        /// 仅添加客户绑定合作关系时使用
        /// </summary>
        public string CompanyID { set; get; }
        /// <summary>
        /// 超级管理员添加客户时不添加关系，仅添加关系时使用；
        /// </summary>
        public string saleid { set; get; }
        /// <summary>
        /// 一个客户的所有销售人员
        /// </summary>
        public IEnumerable<TradingAdmin> Sales { set; get; }
        #region 事件
        /// <summary>
        /// EnterSuccess
        /// </summary>
        override public event SuccessHanlder EnterSuccess;
        /// <summary>
        /// AbandonSuccess
        /// </summary>
        override public event SuccessHanlder AbandonSuccess;
        override public event ErrorHanlder StatusUnnormal;
        #endregion

        override public void Enter()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                this.Enterprise.Enter();
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    //1.Clients存在
                    if (repository.GetTable<Layers.Data.Sqls.PvbCrm.Clients>().Any(item => item.ID == this.Enterprise.ID))
                    {
                        this.ClientStatus = new Views.Origins.ClientsOrigin()[this.Enterprise.ID].ClientStatus;
                        if (this != null && this.StatusUnnormal != null)
                        {
                            this.StatusUnnormal(this, new ErrorEventArgs());
                        }
                    }
                    //2.Clients不存在
                    else
                    {
                        this.ID = this.Enterprise.ID;
                        repository.Insert(this.ToLinq());
                    }
                }
                else
                {
                    this.ClientStatus = ApprovalStatus.Waitting;
                    repository.Update<Layers.Data.Sqls.PvbCrm.Clients>(new
                    {
                        Major = this.Major,
                        Place = this.Place,
                        AreaType = (int)this.AreaType,
                        Nature = (int)this.Nature,
                        Grade = (int)this.Grade,
                        DyjCode = this.DyjCode,
                        TaxperNumber = this.TaxperNumber,
                        Vip = (int?)this.Vip,
                        Status = (int)this.ClientStatus,
                        UpdateDate = this.UpdateDate
                    }, item => item.ID == this.ID);
                }
                //销售或者销售经理添加管理关系（权限），其他角色（超级管理员）不添加关系
                var sale = new Views.Rolls.AdminsAllRoll()[saleid];
                if (sale?.RoleID == FixedRole.Sale.GetFixedID() || sale?.RoleID == FixedRole.SaleManager.GetFixedID())
                {
                    ///客户与管理员关系
                    if (!repository.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Any(item => item.ID == this.MapsID))
                    {
                        repository.Insert(new Layers.Data.Sqls.PvbCrm.MapsBEnter
                        {
                            ID = this.MapsID,
                            Bussiness = (int)Business.Trading,
                            Type = (int)MapsType.Client,
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
    }
}





#region 合作公司&销售  关系的逻辑
//override public void Enter()
//{
//    using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
//    {
//        this.Enterprise.Enter();
//        if (!string.IsNullOrWhiteSpace(this.CompanyID))
//        {
//            ///合作公司是否存在
//            if (repository.GetTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Any(item => item.Bussiness == (int)Business.Trading_Sale && item.Type == (int)MapsType.Client && item.EnterpriseID == this.Enterprise.ID && item.SubID == this.CompanyID))
//            {
//                this.ClientStatus = new Views.Origins.ClientsOrigin()[this.Enterprise.ID].ClientStatus;
//                if (this != null && this.StatusUnnormal != null)
//                {
//                    this.StatusUnnormal(this, new ErrorEventArgs());
//                }
//            }
//        }

//        if (repository.GetTable<Layers.Data.Sqls.PvbCrm.Clients>().Any(item => item.ID == this.Enterprise.ID))
//        {
//            this.ClientStatus = new Views.Origins.ClientsOrigin()[this.Enterprise.ID].ClientStatus;
//            repository.Update<Layers.Data.Sqls.PvbCrm.Clients>(new
//            {
//                // Origin = (int)this.Origin,
//                AreaType = (int)this.AreaType,
//                Nature = (int)this.Nature,
//                Grade = (int?)this.Grade,
//                DyjCode = this.DyjCode,
//                TaxperNumber = this.TaxperNumber,
//                Vip = (int?)this.Vip,
//                Status = (int)this.ClientStatus,
//                UpdateDate = this.UpdateDate
//            }, item => item.ID == this.ID);
//        }
//        else
//        {
//            //2.Clients不存在
//            this.ID = this.Enterprise.ID;
//            repository.Insert(this.ToLinq());
//        }

//        if (!string.IsNullOrWhiteSpace(this.CompanyID))
//        {
//            var sale = new Views.Rolls.AdminsAllRoll()[saleid];
//            if (sale?.RoleID == FixedRole.Sale.GetFixedID() || sale?.RoleID == FixedRole.SaleManager.GetFixedID())
//            {
//                ///客户与管理员关系
//                if (!repository.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Any(item => item.ID == this.MapsID))
//                {
//                    repository.Insert(new Layers.Data.Sqls.PvbCrm.MapsBEnter
//                    {
//                        ID = this.MapsID,
//                        Bussiness = (int)Business.Trading_Sale,
//                        Type = (int)MapsType.Client,
//                        EnterpriseID = this.Enterprise.ID,
//                        SubID = CompanyID,
//                        CreateDate = this.CreateDate,
//                        CtreatorID = this.CreatorID,
//                        IsDefault = this.IsDefault
//                    });
//                }
//                else
//                {
//                    repository.Update<Layers.Data.Sqls.PvbCrm.MapsBEnter>(new
//                    {
//                        SubID = CompanyID,
//                        IsDefault = this.IsDefault
//                    }, item => item.ID == this.MapsID);
//                }
//            }
//        }
//        //销售或者销售经理添加管理关系（权限），其他角色（超级管理员）不添加关系


//        if (this != null && this.EnterSuccess != null)
//        {
//            this.EnterSuccess(this, new SuccessEventArgs(this));
//        }
//    }

//}
#endregion



