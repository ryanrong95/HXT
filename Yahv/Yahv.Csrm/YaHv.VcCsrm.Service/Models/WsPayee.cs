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
    /// <summary>
    /// 受益人改名：收款人
    /// </summary>
    public class WsPayee : Yahv.Linq.IUnique
    {
        #region 属性
        /// <summary>
        /// 受益人标识号
        /// </summary>
        string id;
        /// <summary>
        /// 唯一码
        /// </summary>
        /// <chenhan>保障全局唯一</chenhan>
        virtual public string ID
        {
            get
            {
                return this.id ?? string.Join("",
                    this.WsSupplierID,
                    this.Bank,
                    this.BankAddress,
                    this.Account,
                    this.SwiftCode,
                    this.Methord,
                    this.Currency).MD5();
            }
            set
            {
                this.id = value;
            }
        }
        /// <summary>
        /// 企业ID
        /// </summary>
        public string WsSupplierID { set; get; }

        /// <summary>
        /// 是否可以带票采购
        /// </summary>
        public InvoiceType InvoiceType { set; get; }

        /// <summary>
        /// 开户银行
        /// </summary>
        public string Bank { get; set; }
        /// <summary>
        /// 开户行地址
        /// </summary>
        public string BankAddress { get; set; }
        /// <summary>
        /// 银行账户
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 银行编码 (国际)
        /// </summary>
        public string SwiftCode { get; set; }
        /// <summary>
        /// 汇款方式
        /// </summary>
        public Methord Methord { get; set; }
        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }
        /// <summary>
        /// 地区
        /// </summary>
        public District District { set; get; }
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
        /// 状态
        /// </summary>
        public GeneralStatus Status { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; internal set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateDate { get; internal set; }


        public string CreatorID { get; set; }

        /// <summary>
        /// 默认受益人
        /// </summary>
        public bool IsDefault { set; get; }
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
        #endregion

        #region 持久化
        public void Enter()
        {
            using (var repository = LinqFactory<PvcCrmReponsitory>.Create())
            {
                //默认地址修改
                if (this.IsDefault)
                {
                    repository.Update<Layers.Data.Sqls.PvcCrm.WsPayees>(new { IsDefault = false }, item => item.WsSupplierID == this.WsSupplierID);
                }
                if (repository.ReadTable<Layers.Data.Sqls.PvcCrm.WsPayees>().Any(item => item.ID == this.ID))
                {
                    repository.Update<Layers.Data.Sqls.PvcCrm.WsPayees>(new
                    {
                        InvoiceType = (int)this.InvoiceType,
                        WsSupplierID = this.WsSupplierID,
                        Bank = this.Bank,
                        BankAddress = this.BankAddress,
                        Account = this.Account,
                        SwiftCode = this.SwiftCode,
                        Methord = (int)this.Methord,
                        Currency = (int)this.Currency,
                        District = (int)this.District,
                        Name = this.Name,
                        Tel = this.Tel,
                        Mobile = this.Mobile,
                        Email = this.Email,
                        Status = (int)this.Status,
                        UpdateDate = this.UpdateDate,
                        IsDefault=this.IsDefault
                    }, item => item.ID == this.ID);
                }
                else
                {
                    repository.Insert(this.ToLinq());
                }
                if (this != null && this.EnterSuccess != null)
                {
                    this.EnterSuccess(this, new SuccessEventArgs(this));
                }
            }
        }
        public void Abandon()
        {
            using (var repository = LinqFactory<PvcCrmReponsitory>.Create())
            {
                if (repository.ReadTable<Layers.Data.Sqls.PvcCrm.WsPayees>().Any(item => item.ID == this.ID))
                {
                    repository.Update<Layers.Data.Sqls.PvcCrm.WsPayees>(new
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
