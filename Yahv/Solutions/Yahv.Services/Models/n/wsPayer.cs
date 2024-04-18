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
    /// 公有付款人：代仓储客户的付款人
    /// </summary>
    public class wsPayer : IUnique
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

        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        #endregion

        #region 扩展属性
        public string MethordDec
        {
            get
            {
                return this.Methord.GetDescription();
            }
        }
        public string CurrencyDec
        {
            get
            {
                return this.Currency.GetDescription();
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
        public event AbandonHanlder Abandoned;

        #endregion

        #region 持久化

        protected void RealEnter()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                if (!string.IsNullOrWhiteSpace(this.RealEnterpriseName))
                {
                    var enterprise = repository.ReadTable<Layers.Data.Sqls.PvbCrm.Enterprises>().SingleOrDefault(item => item.Name == this.RealEnterpriseName);
                    if (enterprise == null)
                    {
                        string id = this.RealEnterpriseID = PKeySigner.Pick(PKeyType.Enterprise);
                        repository.Insert(new Layers.Data.Sqls.PvbCrm.Enterprises
                        {
                            ID = id,
                            Name = this.RealEnterpriseName,
                            Status = (int)GeneralStatus.Normal
                        });
                    }
                    else
                    {
                        this.RealEnterpriseID = enterprise.ID;
                    }
                }
            }
        }

        virtual public void Enter()
        {
            this.RealEnter();
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                var payers = new Views.wsPayersTopView<PvbCrmReponsitory>().
                    Where(item => item.EnterpriseID == this.EnterpriseID).ToArray();

                var exsitor = payers.FirstOrDefault(item => item.EnterpriseID == this.EnterpriseID
                       && item.Place == this.Place
                       && item.RealEnterpriseID == this.RealEnterpriseID
                       && item.Bank == this.Bank
                       && item.BankAddress == this.BankAddress
                       && item.Account == this.Account
                       && item.SwiftCode == this.SwiftCode
                       && item.Methord == this.Methord
                       && item.Currency == this.Currency);


                if (exsitor != null)
                {
                    repository.Update(new Layers.Data.Sqls.PvbCrm.Payers
                    {
                        Contact = this.Contact,
                        Tel = this.Tel,
                        Mobile = this.Mobile,
                        Email = this.Email,
                        UpdateDate = DateTime.Now,
                    }, item => item.ID == exsitor.ID);
                }
                else
                {
                    this.ID = PKeySigner.Pick(Yahv.Underly.PKeyType.Payer);
                    this.Status = GeneralStatus.Normal;
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
                repository.Update(new Layers.Data.Sqls.PvbCrm.Payers
                {
                    Status = (int)GeneralStatus.Deleted
                }, item => item.ID == this.ID);
                if (this != null && this.Abandoned != null)
                {
                    this.Abandoned(this, new AbandonedEventArgs(this));
                }
            }

        }

        #endregion

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
            if (this.Abandoned != null && e is AbandonedEventArgs)
            {
                this.Abandoned(this, e as AbandonedEventArgs);
            }
        }
    }
}


namespace Yahv.Services.Models.Behands
{
    /// <summary>
    /// 公有付款人：代仓储客户的付款人
    /// </summary>
    public class wsPayer : Yahv.Services.Models.wsPayer
    {
        /// <summary>
        /// 重复事件
        /// </summary>
        public event ErrorHanlder Repeat;

        #region 持久化
        override public void Enter()
        {
            this.RealEnter();
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                var payers = new Views.wsPayersTopView<PvbCrmReponsitory>().
                    Where(item => item.EnterpriseID == this.EnterpriseID).ToArray();

                var exsitor = payers.FirstOrDefault(item => item.EnterpriseID == this.EnterpriseID
                       && item.RealEnterpriseID == this.RealEnterpriseID
                       && item.Bank == this.Bank
                       && item.BankAddress == this.BankAddress
                       && item.Account == this.Account
                       && item.SwiftCode == this.SwiftCode
                       && item.Methord == this.Methord
                       && item.Currency == this.Currency);

                if (exsitor != null)
                {
                    if (this != null && this.Repeat != null)
                    {
                        this.Repeat(this, new ErrorEventArgs());
                    }
                }
                else
                {
                    this.ID = PKeySigner.Pick(Yahv.Underly.PKeyType.Payer);
                    repository.Insert(this.ToLinq());

                    this.Fire(new SuccessEventArgs(this));
                }


            }
        }

        #endregion
    }
}
