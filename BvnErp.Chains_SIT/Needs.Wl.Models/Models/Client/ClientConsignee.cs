using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Model;
using Needs.Utils.Converters;
using System;
using System.Linq;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 客户收件地址
    /// </summary>
    public class ClientConsignee : ModelBase<Layer.Data.Sqls.ScCustoms.ClientConsignees, ScCustomsReponsitory>, IUnique, IPersist
    {
        string id;
        public new string ID
        {
            get
            {
                return id ?? string.Concat(this.ClientID, this.Contact.ID, this.Address).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 收件单位名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public Contact Contact { get; set; }

        /// <summary>
        /// 是否默认地址
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        public ClientConsignee()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
        }

        public event ErrorHanlder EnterError;
        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

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

            this.Reponsitory.Update<Layer.Data.Sqls.ScCustoms.ClientConsignees>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);

            this.OnAbandonSuccess();
        }

        virtual protected void OnAbandonSuccess()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }
        public override void Enter()
        {
            this.Contact.Enter();

            int count = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientConsignees>().Count(item => item.ID == this.ID);
            //默认地址
            if (this.IsDefault)
            {
                this.Reponsitory.Update<Layer.Data.Sqls.ScCustoms.ClientConsignees>(new { IsDefault = false }, item => item.ClientID == this.ClientID);
            }

            if (count == 0)
            {
                this.Reponsitory.Insert(this.ToLinq());
            }
            else
            {
                this.Reponsitory.Update<Layer.Data.Sqls.ScCustoms.ClientConsignees>(this.ToLinq(), item => item.ID == this.ID);
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

        /// <summary>
        /// 设为默认地址
        /// </summary>
        public void SetDefault()
        {
            this.Reponsitory.Update<Layer.Data.Sqls.ScCustoms.ClientConsignees>(new { IsDefault = false }, item => item.ClientID == this.ClientID);
            this.Reponsitory.Update<Layer.Data.Sqls.ScCustoms.ClientConsignees>(new { IsDefault = true }, item => item.ID == this.ID);
        }
    }
}