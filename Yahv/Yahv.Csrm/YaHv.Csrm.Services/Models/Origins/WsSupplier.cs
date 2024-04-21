using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using Yahv.Usually;
using Yahv.Utils.Converters.Contents;
using YaHv.Csrm.Services.Extends;

namespace YaHv.Csrm.Services.Models.Origins
{
    public class WsSupplier : Yahv.Linq.IUnique
    {
        public WsSupplier()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
        }
        #region 属性
        string id;
        virtual public string ID { set; get; }
        /// <summary>
        /// 中文名称
        /// </summary>
        public string ChineseName { set; get; }
        /// <summary>
        /// 英文名称
        /// </summary>
        public string EnglishName { set; get; }
        /// <summary>
        /// 基本信息
        /// </summary>
        public Enterprise Enterprise { set; get; }
        /// <summary>
        /// 等级
        /// </summary>
        public SupplierGrade Grade { set; get; }
        /// <summary>
        /// 状态
        /// </summary>
        public ApprovalStatus WsSupplierStatus { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { set; get; }
        /// <summary>
        /// 添加人ID
        /// </summary>
        public string CreatorID { set; get; }
        /// <summary>
        /// 录入人信息
        /// </summary>
        public Admin Creator { internal set; get; }
        /// <summary>
        /// 国家、地区，（Origin的简称）
        /// </summary>
        public string Place { set; get; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { set; get; }
        public DateTime UpdateDate { set; get; }

        #endregion
        #region 扩展（提货地址、受益人，联系人）


        /// <summary>
        /// 供应商的所有发票信息
        /// </summary>
        //Views.Rolls.OwnedInvoicesRoll invoices;
        //public Views.Rolls.OwnedInvoicesRoll Invoices
        //{
        //    get
        //    {
        //        if (this.invoices == null || this.invoices.Disposed)
        //        {
        //            this.invoices = new Views.Rolls.OwnedInvoicesRoll(this.Enterprise, Yahv.Underly.FromType.WarehouseServicing);
        //        }
        //        return this.invoices;
        //    }
        //}
        ///// <summary>
        ///// 供应商的联系人信息
        ///// </summary>
        //Views.Rolls.OwnedContactsRoll contacts;
        //public Views.Rolls.OwnedContactsRoll Contacts
        //{
        //    get
        //    {
        //        if (this.contacts == null || this.contacts.Disposed)
        //        {
        //            this.contacts = new Views.Rolls.OwnedContactsRoll(this.Enterprise, Yahv.Underly.FromType.WarehouseServicing);
        //        }
        //        return this.contacts;
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
                this.WsSupplierStatus = ApprovalStatus.Normal;
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    //1.WsSuppliers
                    if (repository.GetTable<Layers.Data.Sqls.PvbCrm.WsSuppliers>().Any(item => item.ID == this.Enterprise.ID))
                    {
                        this.WsSupplierStatus = new Views.Origins.WsSuppliersOrigins()[this.Enterprise.ID].WsSupplierStatus;
                        if (this != null && this.StatusUnnormal != null)
                        {
                            this.StatusUnnormal(this, new ErrorEventArgs());
                        }
                    }
                    //2.WsSuppliers
                    else
                    {
                        this.ID = this.Enterprise.ID;
                        repository.Insert(this.ToLinq());
                        /*this.Admin.Binding(this.ID, MapsType.WsSupplier)*/
                        ;
                    }
                }
                else
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.WsSuppliers>(new
                    {
                        Place = this.Place,
                        Grade = (int)this.Grade,
                        Summary = this.Summary,
                        Status = (int)this.WsSupplierStatus,
                        ChineseName = this.ChineseName,
                        EnglishName = this.EnglishName,
                        UpdateDate = this.UpdateDate
                    }, item => item.ID == this.ID);
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
                repository.Update<Layers.Data.Sqls.PvbCrm.WsSuppliers>(new
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

    public class XdtWsSupplier : WsSupplier
    {
        public string MapsID
        {
            get
            {
                return string.Join("", this.WsClient.ID, base.Enterprise.ID).MD5();
            }

            set
            {
                base.ID = value;
            }
        }
        public Enterprise WsClient { set; get; }

        #region 事件
        public override event SuccessHanlder EnterSuccess;
        public override event SuccessHanlder AbandonSuccess;
        public override event ErrorHanlder StatusUnnormal;
        #endregion
        /// <summary>
        /// 客户供应商的所有受益人
        /// </summary>
        Views.Rolls.WsBeneficiariesRoll beneficiaries;
        /// <summary>
        /// 客户供应商受益人信息
        /// </summary>
        public Views.Rolls.WsBeneficiariesRoll Beneficiaries
        {
            get
            {
                if (this.beneficiaries == null || this.beneficiaries.Disposed)
                {
                    this.beneficiaries = new Views.Rolls.WsBeneficiariesRoll(this.WsClient, this.Enterprise);
                }
                return this.beneficiaries;
            }
        }

        /// <summary>
        ///提货地址
        /// </summary>
        Views.Rolls.WsConsignorsRoll consignors;
        public Views.Rolls.WsConsignorsRoll Consignors
        {
            get
            {
                if (this.consignors == null || this.consignors.Disposed)
                {
                    this.consignors = new Views.Rolls.WsConsignorsRoll(this.WsClient, this.Enterprise);
                }
                return this.consignors;
            }
        }
        #region 持久化
        public override void Enter()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                this.Enterprise.Enter();
                this.WsSupplierStatus = ApprovalStatus.Normal;
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    this.ID = this.Enterprise.ID;
                    if (repository.ReadTable<Layers.Data.Sqls.PvbCrm.WsSuppliers>().Any(item => item.ID == this.Enterprise.ID))
                    {
                        //1.WsSuppliers
                        if (repository.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Any(item => item.ID == this.MapsID))
                        {
                            if (this != null && this.StatusUnnormal != null)
                            {
                                this.StatusUnnormal(this, new ErrorEventArgs());
                            }
                        }
                    }
                    else
                    {
                        repository.Insert(this.ToLinq());
                    }
                }
                else
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.WsSuppliers>(new
                    {
                        Grade = (int)this.Grade,
                        Summary = this.Summary,
                        Status = (int)this.WsSupplierStatus,
                        ChineseName = this.ChineseName,
                        EnglishName = this.EnglishName,
                        UpdateDate = this.UpdateDate,
                        Place = this.Place
                    }, item => item.ID == this.ID);
                }

                this.MapsSupplier(this.WsClient.ID, this.CreatorID);
                if (this != null && this.EnterSuccess != null)
                {
                    this.EnterSuccess(this, new SuccessEventArgs(this));
                }
            }
        }
        public override void Abandon()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                if (repository.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Any(item => item.ID == this.MapsID))
                {
                    repository.Delete<Layers.Data.Sqls.PvbCrm.MapsBEnter>(item => item.ID == this.MapsID);
                }
                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }
        }
        #endregion
    }
}
