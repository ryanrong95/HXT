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
using YaHv.VcCsrm.Service.Extends;

namespace YaHv.VcCsrm.Service.Models
{
    public class Invoice : Yahv.Linq.IUnique
    {
        public Invoice()
        {
            this.Status = ApprovalStatus.Normal;
            this.CreateDate = this.UpdateDate = DateTime.Now;
        }
        #region 属性
        string id;
        /// <summary>
        /// 唯一码
        /// </summary>
        virtual public string ID
        {
            get
            {
                return this.id ?? string.Join("",
                    this.EnterpriseID,
                    this.Type,
                    this.Bank,
                    this.BankAddress,
                    this.Account,
                    this.TaxperNumber
                    ).MD5();
            }
            set { this.id = value; }
        }
        /// <summary>
        /// 企业ID
        /// </summary>
        public string EnterpriseID { get; set; }
        /// <summary>
        /// 企业电话
        /// </summary>
        public string CompanyTel { set; get; }
        /// <summary>
        /// 发票类型 1 普通发票 2 增值税发票 3 海关发票
        /// </summary>
        public InvoiceType Type { get; set; }
        /// <summary>
        /// 开户银行
        /// </summary>
        public string Bank { get; set; }
        /// <summary>
        /// 开户行地址
        /// </summary>
        public string BankAddress { get; set; }
        /// <summary>
        /// 银行账号
        /// </summary>
        public string Account { set; get; }
        /// <summary>
        /// 纳税人识别号
        /// </summary>
        public string TaxperNumber { get; set; }
        /// <summary>
        /// 联系人姓名
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Tel { get; set; }
        /// <summary>
        /// 联系人手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 联系人邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 收货地区
        /// </summary>
        public District District { set; get; }
        /// <summary>
        /// 省
        /// </summary>
        public string Province { set; get; }
        /// <summary>
        /// 市
        /// </summary>
        public string City { set; get; }
        /// <summary>
        /// 地
        /// </summary>
        public string Land { set; get; }
        /// <summary>
        /// 收货地址
        /// </summary>
        public string Address { set; get; }
        /// <summary>
        /// 邮编
        /// </summary>
        public string Postzip { set; get; }
        /// <summary>
        /// 交付方式
        /// </summary>
        public InvoiceDeliveryType DeliveryType { set; get; }

        /// <summary>
        /// 状态
        /// </summary>
        public ApprovalStatus Status { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; internal set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateDate { get; internal set; }
        public string CreatorID { set; get; }
        /// <summary>
        /// 是否默认
        /// </summary>
        public bool IsDefault { set; get; }
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
        #endregion


        #region 持久化
        virtual public void Enter()
        {
            using (var repository = LinqFactory<PvcCrmReponsitory>.Create())
            {
                if (repository.GetTable<Layers.Data.Sqls.PvcCrm.Invoices>().Any(item => item.ID == this.ID))
                {
                    //修改
                    repository.Update<Layers.Data.Sqls.PvcCrm.Invoices>(new
                    {
                        Bank = this.Bank,
                        BankAddress = this.BankAddress,
                        Type = (int)this.Type,
                        Account = this.Account,
                        TaxperNumber = this.TaxperNumber,
                        District = (int)this.District,
                        Address = this.Address,
                        Postzip = this.Postzip,
                        Name = this.Name,
                        Tel = this.Tel,
                        Mobile = this.Mobile,
                        Email = this.Email,
                        Status = (int)this.Status,
                        UpdateDate = this.UpdateDate,
                        Province = this.Province,
                        City = this.City,
                        Land = this.Land,
                        DeliveryType = (int)this.DeliveryType
                    }, item => item.ID == this.ID);
                }
                else
                {
                    //录入
                    repository.Insert(this.ToLinq());
                }
               
                if (this != null && this.EnterSuccess != null)
                {
                    this.EnterSuccess(this, new SuccessEventArgs(this));
                }
            }
        }

        virtual public void Abandon()
        {
            using (var repository = LinqFactory<PvcCrmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvcCrm.Invoices>(new
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
