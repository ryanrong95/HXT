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
    public class Payee : IUnique
    {
        public Payee()
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
        /// 国家/地区
        /// </summary>
        public string Place { set; get; }

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
        /// 唯一性标志
        /// </summary>
        //string uniquesign;

        //public string UniqueSign
        //{
        //    get
        //    {
        //        return this.uniquesign ?? string.Join("",
        //            this.RealID,
        //            this.EnterpriseID,
        //            this.Bank,
        //            this.BankAddress,
        //            this.Account,
        //            this.SwiftCode,
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
        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder Repeat;
        #endregion

        #region 持久化
        public void Enter()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    var payees = new Views.Rolls.PayeesRoll(this.EnterpriseID).ToArray();
                    var exsitor = payees.FirstOrDefault(item => item.EnterpriseID == this.EnterpriseID
                  && item.RealID == this.RealID
                  && item.EnterpriseID == this.EnterpriseID
                  && item.Bank == this.Bank
                  && item.BankAddress == this.BankAddress
                  && item.Account == this.Account
                  && item.SwiftCode == this.SwiftCode
                  && item.Methord == this.Methord
                  && item.Currency == this.Currency);
                    //收款人已存在
                    if (exsitor != null)
                    {
                        if (this != null && this.Repeat != null)
                        {
                            this.Repeat(this, new ErrorEventArgs());
                        }
                    }
                    //收款人不存在
                    else
                    {
                        this.ID = PKeySigner.Pick(PKeyType.Payee);
                        repository.Insert(this.ToLinq());
                        if (this != null && this.EnterSuccess != null)
                        {
                            this.EnterSuccess(this, new SuccessEventArgs(this));
                        }
                    }
                }
                else
                {
                    repository.Update(this.ToLinq(), item => item.ID == this.ID);
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
                repository.Update(new Layers.Data.Sqls.PvbCrm.Payees
                {
                    Status = (int)GeneralStatus.Deleted
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
