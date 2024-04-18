using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Linq;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 联系人
    /// </summary>
    [Serializable]
    public class Contact : IUnique, IPersist
    {
        string id;
        public string ID
        {
            get
            {
                return this.id ?? string.Concat(this.Name, this.Email, this.Mobile).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 联系人名
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 固定电话
        /// </summary>
        public string Tel { get; set; } = string.Empty;

        /// <summary>
        /// 移动电话
        /// </summary>
        public string Mobile { get; set; } = string.Empty;

        /// <summary>
        /// 电子邮箱
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// 传真
        /// </summary>
        public string Fax { get; set; } = string.Empty;

        /// <summary>
        /// QQ
        /// </summary>
        public string QQ { get; set; } = string.Empty;

        /// <summary>
        /// Status
        /// </summary>
        public Enums.Status Status { get; set; }

        /// <summary>
        /// CreateDate
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Summary
        /// </summary>
        public string Summary { get; set; } = string.Empty;

        public event SuccessHanlder AbandonSuccess;
        //public event ErrorHanlder AbandonError;
        public event SuccessHanlder EnterSuccess;
        //public event ErrorHanlder EnterError;

        public Contact()
        {
            this.CreateDate = DateTime.Now;
        }

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Contacts>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.Contacts>(new Layer.Data.Sqls.ScCustoms.Contacts
                    {
                        ID = this.ID,
                        Name = this.Name,
                        Tel = this.Tel,
                        Mobile = this.Mobile,
                        Email = this.Email,
                        Fax = this.Fax,
                        QQ = this.QQ,
                        Status = (int)this.Status,
                        CreateDate = DateTime.Now,
                        Summary = this.Summary
                    });
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.Contacts>(new
                    {
                        Name = this.Name,
                        Tel = this.Tel,
                        Mobile = this.Mobile,
                        Email = this.Email,
                        Fax = this.Fax,
                        QQ = this.QQ,
                        Status = (int)this.Status,
                        Summary = this.Summary
                    }, item => item.ID == this.ID);
                }
            }

            this.OnEnter();
        }

        virtual public void OnEnter()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Abandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Contacts>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);
            }

            this.OnAbandon();
        }

        virtual protected void OnAbandon()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this));
            }
        }

    }
}
