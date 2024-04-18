using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class ClientInvoiceConsignee : IUnique, IPersist
    {
        #region 属性
        //id
        string id;
        public string ID
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

        //状态
        public Enums.Status Status { get; set; }

        //创建时间
        public DateTime CreateDate { get; set; }

        //更新时间
        public DateTime UpdateDate { get; set; }

        internal Admin Admin;

        public void SetAdmin(Admin admin)
        {
            Admin = admin;
        }

        //备注
        public string Summary { get; set; }
        #endregion
        public ClientInvoiceConsignee()
        {
            this.UpdateDate = this.CreateDate = DateTime.Now;
            this.Status = Enums.Status.Normal;
        }
        #region 持久化
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;

        protected virtual void OnEnter()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientInvoiceConsignees>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                }
            }

            this.OnEnter();
        }
        #endregion

    }
}
