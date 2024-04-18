using Needs.Linq;
using Needs.Utils.Converters;
using NtErp.Crm.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NtErp.Crm.Services.Extends;

namespace NtErp.Crm.Services.Models
{
    public partial class Invoice: IUnique, IPersistence
    {
        #region 属性

        string id;
        /// <summary>
        /// ID
        /// </summary>
        public string ID
        {
            get
            {
                return this.id ?? string.Concat(DateTime.Now).MD5();
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
        /// 状态
        /// </summary>
        public InvoiceType InvoiceTypes { get; set; }
        /// <summary>
        /// 公司ID
        /// </summary>
        public string CompanyID { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 公司对象
        /// </summary>
        public Company Companys { get; set; }
        /// <summary>
        /// 税号
        /// </summary>
        public string CompanyCode { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 银行名称
        /// </summary>
        public string BankName { get; set; }
        /// <summary>
        /// 账户
        /// </summary>
        public string Account { get; set; }     
        /// <summary>
        /// 联系人对象
        /// </summary>
        public Contact Contacts { get; set; }
        /// <summary>
        /// 联系人ID
        /// </summary>
        public string ContactID { get; set; }
        /// <summary>
        /// 邮编
        /// </summary>
        public string ZipCode { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public ActionStatus Status { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 更新日期
        /// </summary>
        public DateTime UpdateDate { get; set; }
        /// <summary>
        /// 收货人对象
        /// </summary>
       
        public Consignee Consignee { get; set; }

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public Invoice()
        {
            this.UpdateDate = this.CreateDate = DateTime.Now;
            this.Status = ActionStatus.Normal;
        }

        public event ErrorHanlder AbandonError;
        public event SuccessHanlder EnterSuccess;
        public event SuccessHanlder AbandonSuccess;

        /// <summary>
        /// 数据插入
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
        /// 数据保存到数据库
        /// </summary>
        virtual protected void OnEnter()
        {
            using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Invoices>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
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
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        /// <summary>
        /// 数据逻辑删除
        /// </summary>
        virtual protected void OnAbandon()
        {
            using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.BvCrm.Invoices>(new
                {
                    Status = ActionStatus.Delete
                }, item => item.ID == this.ID);
            }
        }
    }
}
