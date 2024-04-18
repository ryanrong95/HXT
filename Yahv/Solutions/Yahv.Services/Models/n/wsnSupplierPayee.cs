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
using Yahv.Utils.Serializers;

namespace Yahv.Services.Models
{
    /// <summary>
    /// 供应商的私有收款人
    /// </summary>
    public class wsnSupplierPayee : IUnique
    {
        #region 属性
        public string ID { set; get; }
        /// <summary>
        /// 供应商ID
        /// </summary>
        public string nSupplierID { set; get; }
        /// <summary>
        /// 所属企业ID：客户ID
        /// </summary>
        public string OwnID { set; get; }
        /// <summary>
        /// 所属企业名称：客户名称
        /// </summary>
        public string OwnName { set; get; }
        /// <summary>
        /// 真正的所属ID：供应商ID
        /// </summary>
        public string RealEnterpriseID { set; get; }
        /// <summary>
        /// 真正的所属名称：供应商名称
        /// </summary>
        public string RealEnterpriseName { set; get; }
        /// <summary>
        /// 供应商等级
        /// </summary>
        public SupplierGrade nGrade { set; get; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public Methord Methord { set; get; }
        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { set; get; }
        /// <summary>
        /// 开户行
        /// </summary>
        public string Bank { set; get; }
        /// <summary>
        /// 开户行地址
        /// </summary>
        public string BankAddress { set; get; }
        /// <summary>
        /// 银行账号
        /// </summary>
        public string Account { set; get; }
        /// <summary>
        /// 银行代码
        /// </summary>
        public string SwiftCode { set; get; }
        /// <summary>
        /// 联系人姓名
        /// </summary>
        public string Contact { set; get; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { set; get; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Tel { set; get; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { set; get; }
        /// <summary>
        /// 客户入仓号
        /// </summary>
        public string EnterCode { set; get; }
        /// <summary>
        /// 客户等级
        /// </summary>
        public ClientGrade ClientGrade { set; get; }
        /// <summary>
        /// 是否默认
        /// </summary>
        public bool IsDefault { set; get; }
        /// <summary>
        /// 国家或地区
        /// </summary>
        public string Place { set; get; }
        /// <summary>
        /// 收款人状态
        /// </summary>
        public GeneralStatus Status { set; get; }

        /// <summary>
        /// 录入人
        /// </summary>
        public string Creator { set; get; }
        #endregion

        #region 扩展属性
        public string CurrencyDes
        {
            get
            {
                return this.Currency.GetDescription();
            }
        }

        public string MethordDes
        {
            get
            {
                return this.Methord.GetDescription();
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
        public event ErrorHanlder Repeat;
        #endregion

        #region 持久化

        /// <summary>
        /// 唯一性标志
        /// </summary>
        string uniquesign;

        string UniqueSign
        {
            get
            {
                return this.uniquesign ?? string.Join("",
                    this.OwnID,
                    this.RealEnterpriseID,
                    this.Account,
                    this.Methord,
                    this.Currency).MD5();
            }
            set
            {
                this.uniquesign = value;
            }
        }
        public void Enter()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    //客户供应商的所有收款人，判断是否已存在
                    var payees = new Views.wsnSupplierPayeesTopView<PvbCrmReponsitory>().Where(item => item.nSupplierID == this.nSupplierID).ToArray();
                    //判断收款人是否已存在
                    if (string.IsNullOrWhiteSpace(this.ID) && payees.Any(item => item.UniqueSign == this.UniqueSign && item.Status == GeneralStatus.Normal))
                    {
                        if (this != null && this.Repeat != null)
                        {
                            this.Repeat(this, new ErrorEventArgs());
                        }
                    }
                    else
                    {
                        if (this.IsDefault)
                        {
                            repository.Update<Layers.Data.Sqls.PvbCrm.nPayees>(new { IsDefault = false }, item => item.nSupplierID == this.nSupplierID);
                        }
                        this.ID = PKeySigner.Pick(Yahv.Underly.PKeyType.nPayee);
                        repository.Insert(this.ToLinq());
                        if (this != null && this.EnterSuccess != null)
                        {
                            this.EnterSuccess(this, new SuccessEventArgs(this));
                        }
                    }
                }
                else
                {
                    if (this.IsDefault)
                    {
                        repository.Update<Layers.Data.Sqls.PvbCrm.nPayees>(new { IsDefault = false }, item => item.nSupplierID == this.nSupplierID);
                    }
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
                repository.Update(new Layers.Data.Sqls.PvbCrm.nPayees
                {
                    Status = (int)GeneralStatus.Deleted
                }, item => item.ID == this.ID);
                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }
        }
        /// <summary>
        /// 同步方法
        /// </summary>
        /// <returns>接口返回结果</returns>
        public object Synchro()
        {
            var json = new
            {
                Enterprise = new Enterprise { Name = this.OwnName },
                Place = this.Place,
                SupplierName = this.RealEnterpriseName,
                IsDefault = this.IsDefault,
                Bank = this.Bank,
                BankAddress = this.BankAddress,
                Account = this.Account,
                SwiftCode = this.SwiftCode,
                Methord = this.Methord,
                Currency = this.Currency,
                Name = this.Contact,
                Tel = this.Tel,
                Mobile = this.Mobile,
                Email = this.Email,
            }.Json();
            var response = Commons.HttpPostRaw(Commons.UnifyApiUrl + "/suppliers/banks", json);
            return response;
        }
        /// <summary>
        /// 同步删除
        /// </summary>
        /// <param name="OwnName">OwnName是客户名称</param>
        /// <param name="RealEnterpriseName">供应商的企业名称</param>
        /// <param name="Account">银行账号</param>
        public object AbandonSynchro()
        {
            var response = Commons.CommonHttpRequest(Commons.UnifyApiUrl + "/suppliers/banks?name=" + this.OwnName + "&supplierName=" + this.RealEnterpriseName + "&account=" + this.Account, "DELETE");
            return response;
        }
        #endregion

    }
}
