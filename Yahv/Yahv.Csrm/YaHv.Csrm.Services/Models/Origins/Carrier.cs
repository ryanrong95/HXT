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
    /// <summary>
    /// 承运商
    /// </summary>
    public class Carrier : Yahv.Linq.IUnique
    {
        public Carrier()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
            this.Status = GeneralStatus.Normal;
        }
        #region 属性
        public string ID { set; get; }
        public Enterprise Enterprise { set; get; }
        /// <summary>
        /// 简称
        /// </summary>
        public string Code { set; get; }
        /// <summary>
        /// 企业logo
        /// </summary>
        public string Icon { set; get; }
        /// <summary>
        /// 承运商类型
        /// </summary>
        public CarrierType Type { set; get; }
        /// <summary>
        /// 是否国际快递
        /// </summary>
        public bool IsInternational { set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { set; get; }
        /// <summary>
        /// 编辑时间
        /// </summary>
        public DateTime UpdateDate { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { set; get; }
        public string CreatorID { set; get; }
        /// <summary>
        /// 创建人
        /// </summary>
        public Admin Creator { internal set; get; }
        /// <summary>
        /// 状态
        /// </summary>
        public GeneralStatus Status { set; get; }
        /// <summary>
        /// 司机
        /// </summary>
        Views.Rolls.OwnedDriversRoll drivers;
        public Views.Rolls.OwnedDriversRoll Drivers
        {
            get
            {
                if (this.drivers == null || this.drivers.Disposed)
                {
                    this.drivers = new Views.Rolls.OwnedDriversRoll(this.Enterprise);
                }
                return this.drivers;
            }
        }
        /// <summary>
        /// 运输工具
        /// </summary>
        Views.Rolls.TransportsRoll transports;
        public Views.Rolls.TransportsRoll Transports
        {
            get
            {
                if (this.transports == null || this.transports.Disposed)
                {
                    this.transports = new Views.Rolls.TransportsRoll(this.Enterprise);
                }
                return this.transports;
            }
        }
        /// <summary>
        /// 受益人
        /// </summary>
        Views.Rolls.BeneficiariesRoll beneficiaries;
        public Views.Rolls.BeneficiariesRoll Beneficiaries
        {
            get
            {
                if (this.beneficiaries == null || this.beneficiaries.Disposed)
                {
                    this.beneficiaries = new Views.Rolls.BeneficiariesRoll(this.Enterprise);
                }
                return this.beneficiaries;
            }
        }

        /// <summary>
        /// 运输工具
        /// </summary>
        Views.Rolls.ContactsRoll contacts;
        public Views.Rolls.ContactsRoll Contacts
        {
            get
            {
                if (this.contacts == null || this.beneficiaries.Disposed)
                {
                    this.contacts = new Views.Rolls.ContactsRoll(this.Enterprise);
                }
                return this.contacts;
            }
        }
        #endregion


        #region 事件
        /// <summary>
        /// EnterSuccess
        /// </summary>
        public event SuccessHanlder EnterSuccess;
        /// <summary>
        /// AbandonSuccess
        /// </summary>
        public event SuccessHanlder AbandonSuccess;
        /// <summary>
        /// 已存在
        /// </summary>
        public event ErrorHanlder NameReapt;
        #endregion

        #region 持久化
        public void Enter()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                this.Enterprise.Enter();
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    if (repository.ReadTable<Layers.Data.Sqls.PvbCrm.Carriers>().Any(item => item.ID == this.Enterprise.ID))
                    {
                        if (this != null && this.NameReapt != null)
                        {
                            this.NameReapt(this, new ErrorEventArgs());
                        }
                    }
                    else
                    {
                        repository.Insert(this.ToLinq());
                        if (this != null && this.EnterSuccess != null)
                        {
                            this.EnterSuccess(this, new SuccessEventArgs(this));
                        }
                    }
                }
                else
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.Carriers>(new
                    {
                        Type = (int)this.Type,
                        Icon = this.Icon,
                        Code = this.Code,
                        Status = (int)this.Status,
                        Summary = this.Summary,
                        UpdateDate = this.UpdateDate,
                        IsInternational = this.IsInternational
                    }, item => item.ID == this.ID);
                    if (this != null && this.EnterSuccess != null)
                    {
                        this.EnterSuccess(this, new SuccessEventArgs(this));
                    }
                }
            }
        }
        public void Abandon()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                if (repository.ReadTable<Layers.Data.Sqls.PvbCrm.Carriers>().Any(item => item.ID == this.ID))
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.Carriers>(new
                    {
                        Status = (int)GeneralStatus.Deleted
                    }, item => item.ID == this.ID);
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
