using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 客户收件地址
    /// </summary>
    public class ClientConsignee : IUnique, IPersist
    {
        string id;
        public string ID
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

        /// <summary>
        /// 数据状态
        /// </summary>
        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        public ClientConsignee()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
            this.Status = Enums.Status.Normal;
        }

        public event ErrorHanlder EnterError;
        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        /// <summary>
        /// 删除
        /// </summary>
        public void Abandon()
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

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.ClientConsignees>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);
            }

            this.OnAbandonSuccess();
        }

        virtual protected void OnAbandonSuccess()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }
        public void Enter()
        {
            this.Contact.Enter();

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientConsignees>().Count(item => item.ID == this.ID);
                //默认地址
                if (this.IsDefault)
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.ClientConsignees>(new { IsDefault = false }, item => item.ClientID == this.ClientID);
                }
                if (count == 0)
                {
                    reponsitory.Insert(this.ToLinq());
                }
                else
                {
                   // reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);

                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.ClientConsignees>(new
                    {
                        ID = this.ID,
                        Name = this.Name,
                        ClientID = this.ClientID,
                        ContactID = this.Contact.ID,
                        Address = this.Address,
                        IsDefault = this.IsDefault,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = DateTime.Now,
                        Summary = this.Summary
                    }, item => item.ID == this.ID);
                }
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
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.ClientConsignees>(new { IsDefault = false }, item => item.ClientID == this.ClientID);
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.ClientConsignees>(new { IsDefault = true }, item => item.ID == this.ID);
            }
        }
    }
}