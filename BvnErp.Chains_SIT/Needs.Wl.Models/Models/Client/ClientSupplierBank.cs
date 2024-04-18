using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Model;
using Needs.Utils.Converters;
using System;
using System.Linq;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 客户供应商银行账户
    /// </summary>
    public class ClientSupplierBank : ModelBase<Layer.Data.Sqls.ScCustoms.ClientSupplierAddresses, ScCustomsReponsitory>, IUnique, IPersist
    {
        string id;
        public new string ID
        {
            get
            {
                return id ?? string.Concat(this.ClientSupplierID, this.BankAccount, this.SwiftCode).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 供应商ID
        /// </summary>
        public string ClientSupplierID { get; set; }

        /// <summary>
        /// 银行账户
        /// </summary>
        public string BankAccount { get; set; }

        /// <summary>
        /// 银行地址
        /// </summary>
        public string BankAddress { get; set; }

        /// <summary>
        /// 银行名称
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// 银行国际代码
        /// </summary>
        public string SwiftCode { get; set; }

        public ClientSupplierBank()
        {
            
        }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        public override void Enter()
        {
            int count = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientSupplierBanks>().Count(item => item.ID == this.ID);
            if (count == 0)
            {
                this.Reponsitory.Insert(this.ToLinq());
            }
            else
            {
                this.Reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
            }

            this.OnEnter();
        }

        private void OnEnter()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        virtual protected void OnAbandonSuccess()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public override void Abandon()
        {
            //判定ID不能为空
            if (string.IsNullOrWhiteSpace(this.ID))
            {
                if (this != null && this.AbandonError != null)
                {
                    //失败触发事件
                    this.AbandonError(this, new ErrorEventArgs("主键ID不能为空！"));
                }
            }

            this.Reponsitory.Update<Layer.Data.Sqls.ScCustoms.ClientSupplierBanks>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);

            this.OnAbandonSuccess();
        }
    }
}