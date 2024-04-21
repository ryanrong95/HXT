using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Yahv.Underly;
using Yahv.Usually;
using Yahv.Utils.Converters.Contents;
using YaHv.Csrm.Services.Extends;

namespace YaHv.Csrm.Services.Models.Origins
{
    public class nSupplier : Yahv.Linq.IUnique
    {
        public nSupplier()
        {
            this.Conduct = Business.WarehouseServicing;
            this.CreateDate = this.UpdateDate = DateTime.Now;
            this.Status = GeneralStatus.Normal;
        }
        #region 属性
        public string ID { set; get; }
        /// <summary>
        /// 所属企业，代仓储中是客户ID
        /// </summary>
        public string EnterpriseID { set; get; }
        public Enterprise Enterprise { internal set; get; }
        /// <summary>
        /// 真实ID,供应商的真实企业ID,可为空
        /// </summary>
        public string RealID { set; get; }
        /// <summary>
        /// 供应商真实的企业
        /// </summary>
        public Enterprise RealEnterprise { set; get; }
        /// <summary>
        /// 来源：内部公司，指芯达通、恒远等
        /// </summary>
        public string FromID { set; get; }
        /// <summary>
        /// 中文名称
        /// </summary>
        public string ChineseName { set; get; }
        string englishname;
        /// <summary>
        /// 英文名称
        /// </summary>
        public string EnglishName
        {
            get { return this.englishname; }
            set
            {
                Regex regex = new Regex(@"\s+", RegexOptions.Singleline);
                this.englishname = regex.Replace(Yahv.Utils.Extends.StringExtend.ToHalfAngle(value), " ").Trim();
            }
        }
        /// <summary>
        /// 中文简称
        /// </summary>
        public string CHNabbreviation { set; get; }
        /// <summary>
        /// 业务
        /// </summary>
        public Business Conduct { set; get; }

        /// <summary>
        /// 等级
        /// </summary>
        public SupplierGrade Grade { set; get; }
        /// <summary>
        /// 状态
        /// </summary>
        public GeneralStatus Status { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { set; get; }
        /// <summary>
        /// 添加人ID
        /// </summary>
        public string Creator { set; get; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { set; get; }
        public DateTime UpdateDate { set; get; }
        #endregion
        #region 扩展（交货地址、受益人）
        /// <summary>
        /// 客户的所有到货地址
        /// </summary>
        Views.Rolls.nConsignorsRoll nconsignors;
        public Views.Rolls.nConsignorsRoll nConsignors
        {
            get
            {
                if (this.nconsignors == null || this.nconsignors.Disposed)
                {
                    this.nconsignors = new Views.Rolls.nConsignorsRoll(this.ID);
                }
                return this.nconsignors;
            }
        }

        /// <summary>
        /// 客户的所有到货地址
        /// </summary>
        Views.Rolls.nPayeesView npayees;
        public Views.Rolls.nPayeesView nPayees
        {
            get
            {
                if (this.npayees == null || this.npayees.Disposed)
                {
                    this.npayees = new Views.Rolls.nPayeesView(this.ID);
                }
                return this.npayees;
            }
        }
        Views.Rolls.nContactsRoll ncontacts;
        /// <summary>
        /// 代仓储供应商的私有联系人
        /// </summary>
        public Views.Rolls.nContactsRoll nContacts
        {
            get
            {
                if (this.ncontacts == null || this.ncontacts.Disposed)
                {
                    this.ncontacts = new Views.Rolls.nContactsRoll(this.ID);
                }
                return this.ncontacts;
            }
        }
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
        virtual public event ErrorHanlder Repeat;
        #endregion

        #region 持久化
        public void Enter()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    //this.RealEnterprise.ID = this.RealEnterprise.Name.MD5();
                    this.RealEnterprise.ID = new Views.Rolls.EnterprisesRoll().FirstOrDefault(item => item.Name == this.RealEnterprise.Name)?.ID;
                    var existSupplier = repository.GetTable<Layers.Data.Sqls.PvbCrm.nSuppliers>()
                        .FirstOrDefault(item => item.EnterpriseID == this.EnterpriseID && item.RealID == this.RealEnterprise.ID && item.Status == (int)GeneralStatus.Normal);
                    //供应商已存在
                    if (existSupplier != null)
                    {
                        this.ID = existSupplier.ID;
                        if (this != null && this.Repeat != null)
                        {
                            this.Repeat(this, new ErrorEventArgs());
                        }
                    }
                    //2.WsSuppliers
                    else
                    {
                        this.RealEnterprise.Enter();
                        this.ID = PKeySigner.Pick(PKeyType.nSupplier);
                        //this.Grade = SupplierGrade.Second;
                        repository.Insert(this.ToLinq());
                    }
                }
                else
                {
                    this.RealEnterprise.Enter();
                    repository.Update<Layers.Data.Sqls.PvbCrm.nSuppliers>(new
                    {
                        Grade = (int)this.Grade,
                        Summary = this.Summary,
                        Status = (int)this.Status,
                        CHNabbreviation = this.CHNabbreviation,
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
        public void Abandon()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvbCrm.nSuppliers>(new
                {
                    Status = GeneralStatus.Deleted
                }, item => item.ID == this.ID);
                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }
        }
        #endregion
    }
}
