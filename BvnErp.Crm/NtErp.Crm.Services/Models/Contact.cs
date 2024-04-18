using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Linq;
using NtErp.Crm.Services.Enums;
using Needs.Overall;
using NtErp.Crm.Services.Extends;
using Needs.Utils.Descriptions;
using Needs.Utils.Converters;

namespace NtErp.Crm.Services.Models
{
    [Needs.Underly.FactoryView(typeof(Views.ContactAlls))]
    public partial class Contact : Needs.Underly.Document, IUnique, IPersistence
    {
        #region 属性
        /// <summary>
        /// 主键ID
        /// </summary>
        public string ID
        {
            get
            {
                return this[nameof(ID)] as string;
            }
            set
            {
                this[nameof(ID)] = value;
            }
        }

        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID
        {
            get
            {
                return this[nameof(ClientID)] as string;
            }
            set
            {
                this[nameof(ClientID)] = value;
            }
        }

        /// <summary>
        /// 联系人名称
        /// </summary>
        public string Name
        {
            get
            {
                return this[nameof(Name)] as string;
            }
            set
            {
                this[nameof(Name)] = value;
            }
        }

        /// <summary>
        /// 收货人地址类型
        /// </summary>
        public ConsigneeType Types
        {
            get
            {
                return this[nameof(Types)];
            }
            set
            {
                this[nameof(Types)] = value;
            }
        }

        /// <summary>
        /// 公司ID
        /// </summary>
        public string CompanyID
        {
            get
            {
                return this[nameof(CompanyID)] as string;
            }
            set
            {
                this[nameof(CompanyID)] = value;
            }
        }

        /// <summary>
        /// 联系人位置
        /// </summary>
        public string Position
        {
            get
            {
                return this[nameof(Position)] as string;
            }
            set
            {
                this[nameof(Position)] = value;
            }
        }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email
        {
            get
            {
                return this[nameof(Email)] as string;
            }
            set
            {
                this[nameof(Email)] = value;
            }
        }

        /// <summary>
        /// 电话
        /// </summary>
        public string Tel
        {
            get
            {
                return this[nameof(Tel)] as string;
            }
            set
            {
                this[nameof(Tel)] = value;
            }
        }

        /// <summary>
        /// 手机
        /// </summary>
        public string Mobile
        {
            get
            {
                return this[nameof(Mobile)] as string;
            }
            set
            {
                this[nameof(Mobile)] = value;
            }
        }

        /// <summary>
        /// 状态
        /// </summary>
        public Status Status
        {
            get
            {
                return this[nameof(Status)];
            }
            set
            {
                this[nameof(Status)] = value;
            }
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate
        {
            get
            {
                return this[nameof(CreateDate)];
            }
            set
            {
                this[nameof(CreateDate)] = value;
            }
        }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateDate
        {
            get
            {
                return this[nameof(UpdateDate)];
            }
            set
            {
                this[nameof(UpdateDate)] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Detail
        {
            get
            {
                return this[nameof(Detail)] as string;
            }
            set
            {
                this[nameof(Detail)] = value;
            }
        }

        /// <summary>
        /// 客户
        /// </summary>
        public Client Clients
        {
            get
            {
                return this[nameof(Clients)];
            }
            set
            {
                this[nameof(Clients)] = value;
            }
        }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public Contact()
        {
            this.UpdateDate = this.CreateDate = DateTime.Now;
            this.Status = Status.Normal;
        }

        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;


        #region 持久化
        /// <summary>
        /// 删除触发事件
        /// </summary>
        public void Abandon()
        {
            if (string.IsNullOrWhiteSpace(this.ID))
            {
                if (this != null && this.AbandonError != null)
                {
                    this.AbandonError(this, new ErrorEventArgs("主键ID不能为空！"));
                }
            }

            this.OnAbandon();

            if (this != null && this.AbandonSuccess != null)
            {
                //删除成功触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        /// <summary>
        /// 执行逻辑删除
        /// </summary>
        virtual protected void OnAbandon()
        {
            using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
            {
                //校验联系人是否存在于地址中
                int count = reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Consignees>().Count(item => item.ContactID == this.ID
                && item.Status == (int)Status.Normal);
                if(count > 0)
                {
                    this.AbandonError(this, new ErrorEventArgs("地址簿中有该联系人信息，不能删除！"));
                }
                reponsitory.Update<Layer.Data.Sqls.BvCrm.Contacts>(new
                {
                    Status = Status.Delete
                }, item => item.ID == this.ID);
            }
        }


        /// <summary>
        /// 数据保存触发事件
        /// </summary>
        public void Enter()
        {
            this.OnEnter();
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        protected void OnEnter()
        {
            using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
            {
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    this.ID = PKeySigner.Pick(PKeyType.Contact);
                    reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    this.UpdateDate = DateTime.Now;
                    reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                }

            }
        }

        /// <summary>
        /// 发票信息更新
        /// </summary>
        public void InvoiceEnter()
        {
            this.OnInvoiceEnter();
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        /// <summary>
        /// 数据
        /// </summary>
        protected void OnInvoiceEnter()
        {
            using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
            {
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    this.ID = PKeySigner.Pick(PKeyType.Contact);
                    reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    this.UpdateDate = DateTime.Now;
                    reponsitory.Update(this.ToInoviceLinq(), item => item.ID == this.ID);
                }

            }
        }
        #endregion
    }
}
