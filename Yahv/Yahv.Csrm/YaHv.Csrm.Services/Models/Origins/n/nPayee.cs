using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Usually;
using Yahv.Utils.Converters.Contents;
using YaHv.Csrm.Services.Extends;

namespace YaHv.Csrm.Services.Models.Origins
{
    /// <summary>
    /// 收款人
    /// </summary>
    public class nPayee : IUnique
    {
        public nPayee()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
            this.Status = GeneralStatus.Normal;
        }
        #region 属性
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 私有供应商ID
        /// </summary>
        public string nSupplierID { set; get; }
        /// <summary>
        /// 企业ID
        /// </summary>
        public string EnterpriseID { get; set; }
        /// <summary>
        /// 真实ID
        /// </summary>
        public string RealID { set; get; }
        /// <summary>
        /// RealID对应的企业
        /// </summary>
        public Enterprise RealEnterprise { set; get; }

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
        /// 联系人姓名
        /// </summary>
        public string Contact { set; get; }
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
        /// 国家/地区简称
        /// </summary>
        public string Place { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public GeneralStatus Status { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { set; get; }
        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime UpdateDate { set; get; }
        /// <summary>
        /// 录入人
        /// </summary>
        public string Creator { set; get; }
        /// <summary>
        /// 是否默认
        /// </summary>
        public bool IsDefault { set; get; }
        /// <summary>
        /// 唯一性标志
        /// </summary>
        //string uniquesign;

        //public string UniqueSign
        //{
        //    get
        //    {
        //        return this.uniquesign ?? string.Join("",
        //            this.RealID,
        //            this.nSupplierID,
        //            this.Account,
        //            this.Methord,
        //            this.Currency).MD5();
        //    }
        //    set
        //    {
        //        this.uniquesign = value;
        //    }
        //}
        #endregion



        #region 事件
        /// <summary>
        /// EnterSuccess
        /// </summary>
        public event SuccessHanlder EnterSuccess;
        /// <summary>
        /// AbandonSuccess
        /// </summary>
        public event AbandonHanlder AbandonSuccess;
        #endregion

        #region 持久化
        virtual public void Enter()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {

                var payees = new Views.Rolls.nPayeesRoll(this.nSupplierID).ToArray();
                //收款人是否已存在，唯一性判断：供应商ID，支付方式，币种，银行账号
                var exsitor = payees.FirstOrDefault(item => item.EnterpriseID == this.EnterpriseID
                  && item.RealID == this.RealID
                  && item.nSupplierID == this.nSupplierID
                  && item.Account == this.Account
                  && item.Methord == this.Methord
                  && item.Currency == this.Currency);
                if (exsitor != null)
                {
                    if (this.IsDefault)
                    {
                        repository.Update<Layers.Data.Sqls.PvbCrm.nPayees>(new { IsDefault = false }, item => item.nSupplierID == this.nSupplierID);
                    }
                    this.ID = exsitor.ID;
                   // repository.Update(this.ToLinq(), item => item.ID == exsitor.ID);
                    repository.Update<Layers.Data.Sqls.PvbCrm.nPayees>(new
                    {
                        Bank = this.Bank,
                        BankAddress = this.BankAddress,
                        Account = this.Account,
                        SwiftCode = this.SwiftCode,
                        Methord = (int)this.Methord,
                        Currency = (int)this.Currency,
                        Contact = this.Contact,
                        Tel = this.Tel,
                        Mobile = this.Mobile,
                        Email = this.Email,
                        Status = (int)this.Status,
                        UpdateDate = this.UpdateDate,
                        Creator = this.Creator,
                        IsDefault = this.IsDefault,
                        Place = this.Place
                    }, item => item.ID == exsitor.ID);
                }
                else
                {
                    this.ID = PKeySigner.Pick(PKeyType.nPayee);
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
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                repository.Update(new Layers.Data.Sqls.PvbCrm.nPayees
                {
                    Status = (int)GeneralStatus.Deleted
                }, item => item.ID == this.ID);
                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new AbandonedEventArgs(this));
                }
            }

        }
        /// <summary>
        /// 点燃
        /// </summary>
        /// <param name="e">事件参数</param>
        protected void Fire(EventArgs e)
        {
            if (this.EnterSuccess != null && e is SuccessEventArgs)
            {
                this.EnterSuccess(this, e as SuccessEventArgs);
            }
            if (this.AbandonSuccess != null && e is AbandonedEventArgs)
            {
                this.AbandonSuccess(this, e as AbandonedEventArgs);
            }
        }
        #endregion
    }
}
