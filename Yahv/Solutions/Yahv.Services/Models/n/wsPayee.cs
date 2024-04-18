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

namespace Yahv.Services.Models
{
    /// <summary>
    /// 公有收款人：内部公司的收款人
    /// </summary>
    public class wsPayee : IUnique
    {
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
        /// 企业名称
        /// </summary>
        public string EnterpriseName { set; get; }
        /// <summary>
        /// 真实企业ID
        /// </summary>
        public string RealEnterpriseID { set; get; }
        /// <summary>
        /// 真实企业名称
        /// </summary>
        public string RealEnterpriseName { set; get; }
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
        /// 国家或地区
        /// </summary>
        public string Place { set; get; }
        /// <summary>
        /// 录入人ID
        /// </summary>
        public string CreatorID { set; get; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }
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
        /// <summary>
        /// 唯一性标志
        /// </summary>
        //string uniquesign;

        //string UniqueSign
        //{
        //    get
        //    {
        //        return this.uniquesign ?? string.Join("",
        //            this.RealEnterpriseID,
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
        public void Enter()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                #region RealEnterprise不存在时新增

                if (!string.IsNullOrWhiteSpace(this.RealEnterpriseName))
                {
                    this.RealEnterpriseID = this.EnterpriseName.MD5();
                    if (!repository.ReadTable<Layers.Data.Sqls.PvbCrm.Enterprises>().Any(item => item.ID == this.EnterpriseID))
                    {
                        repository.Insert(new Layers.Data.Sqls.PvbCrm.Enterprises
                        {
                            ID = this.RealEnterpriseID,
                            Name = this.RealEnterpriseName,
                            AdminCode = ""
                        });
                    }
                }
                #endregion

                //查询企业的所有收款人
                var payees = new Views.wsPayeesTopView<PvbCrmReponsitory>().Where(item => item.EnterpriseID == this.EnterpriseID).ToArray();
                var exsitor = payees.FirstOrDefault(item => item.EnterpriseID == this.EnterpriseID
                  && item.RealEnterpriseID == this.RealEnterpriseID
                  && item.Bank == this.Bank
                  && item.BankAddress == this.BankAddress
                  && item.Account == this.Account
                  && item.SwiftCode == this.SwiftCode
                  && item.Methord == this.Methord
                  && item.Currency == this.Currency);
                //判断收款人是否存在
                if (exsitor != null)
                {
                    repository.Update(this.ToLinq(), item => item.ID == this.ID);
                }
                //收款人不存在
                else
                {
                    this.ID = PKeySigner.Pick(Yahv.Underly.PKeyType.Payee);
                    repository.Insert(this.ToLinq());
                    if (this != null && this.EnterSuccess != null)
                    {
                        this.EnterSuccess(this, new SuccessEventArgs(this));
                    }
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
