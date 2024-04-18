using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Model;
using Needs.Utils.Converters;
using System;
using System.Linq;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 客户供应商地址
    /// </summary>
    public class ClientSupplierAddress : ModelBase<Layer.Data.Sqls.ScCustoms.ClientSupplierAddresses, ScCustomsReponsitory>, IUnique, IPersist
    {
        string id;
        public new string ID
        {
            get
            {
                return this.id ?? string.Concat(this.ClientSupplierID, this.Contact.ID).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        public string ClientSupplierID { get; set; }

        /// <summary>
        /// 是否默认提货地址
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public Contact Contact { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 邮编
        /// </summary>
        public string ZipCode { get; set; }

        public ClientSupplierAddress()
        {
      
        }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        public override void Enter()
        {
            this.Contact.Enter();

            int count = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientSupplierAddresses>().Count(item => item.ID == this.ID);
            //默认地址
            if (this.IsDefault)
            {
                this.Reponsitory.Update<Layer.Data.Sqls.ScCustoms.ClientSupplierAddresses>(new { IsDefault = false }, item => item.ClientSupplierID == this.ClientSupplierID);
            }
            if (count == 0)
            {
                this.Reponsitory.Insert(this.ToLinq());
            }
            else
            {
                this.UpdateDate = DateTime.Now;
                this.Reponsitory.Update<Layer.Data.Sqls.ScCustoms.ClientSupplierAddresses>(this.ToLinq(), item => item.ID == this.ID);
            }

            this.OnEnter();
        }

        protected virtual void OnEnter()
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
                throw new Exception("未将对象设置为对象的实例");
            }

            this.Reponsitory.Update<Layer.Data.Sqls.ScCustoms.ClientSupplierAddresses>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);

            this.OnAbandonSuccess();
        }
    }
}