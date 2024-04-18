using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Model;
using Needs.Utils.Converters;
using System;
using System.Linq;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 发票收件人
    /// </summary>
    public class ClientInvoiceConsignee : ModelBase<Layer.Data.Sqls.ScCustoms.ClientInvoiceConsignees, ScCustomsReponsitory>, IUnique, IPersist
    {
        #region 属性
        //id
        string id;
        public new string ID
        {
            get
            {
                //根据发票及客户信息确认发票的唯一性
                //与发票信息不同，地址不需要记录变化，只存一条记录
                return this.id ?? string.Concat(this.ClientID).MD5();
            }
            set
            {
                this.id = value;
            }
        }
        //客户ID
        public string ClientID { get; set; }

        //收件人
        public string Name { get; set; }

        //手机
        public string Mobile { get; set; }

        //电话
        public string Tel { get; set; }

        //邮箱
        public string Email { get; set; }

        //地址
        public string Address { get; set; }

        public Admin Admin { get; set; }

        #endregion

        public ClientInvoiceConsignee()
        {
           
        }

        #region 持久化

        public event SuccessHanlder EnterSuccess;

        protected virtual void OnEnter()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        public override void Enter()
        {
            int count = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientInvoiceConsignees>().Count(item => item.ID == this.ID);
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

        #endregion
    }
}