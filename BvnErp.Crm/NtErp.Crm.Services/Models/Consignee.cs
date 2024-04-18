using Needs.Linq;
using Needs.Utils.Converters;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Models
{
    /// <summary>
    /// Party A
    /// Party B
    /// </summary>
    public class Party : Needs.Linq.IUnique
    {        
        string id;
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
        /// 公司  nulll
        /// </summary>
        //public Company Company { get; set; }
        public string CompanyID { get; set; }

        /// <summary>
        /// 联系人  
        /// </summary>
        public Contact Contact { get; set; }

        /// <summary>
        /// 地址[可以是国际]
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 邮编[国际]
        /// </summary>
        public string Zipcode { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public Status Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateDate { get; set; }
    }

    /// <summary>
    /// 收货人 [收信人]
    /// </summary>
    [Needs.Underly.FactoryView(typeof(Views.ConsigneeAlls))]
    public partial class Consignee : Party,  IPersistence, IFulError, IFulSuccess
    {
        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public Consignee()
        {
            this.UpdateDate = this.CreateDate = DateTime.Now;
            this.Status = Status.Normal;
        }

        public event ErrorHanlder AbandonError;
        public event SuccessHanlder EnterSuccess;
        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;


        #region 持久化
        /// <summary>
        /// 保存触发方法
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
        /// 数据保存
        /// </summary>
        virtual protected void OnEnter()
        {
            using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
            {
                //判定数据是否存在
                int count = reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Consignees>().Count(item => item.ID == this.ID);

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
        /// 删除触发方法
        /// </summary>
        public void Abandon()
        {
            if (string.IsNullOrWhiteSpace(this.ID))
            {
                if (this != null && this.AbandonError != null)
                {
                    //删除失败触发事件
                    this.AbandonError(this, new ErrorEventArgs("主键ID不能为空"));
                }
            }

            this.OnAbandon();

            if (this != null && this.AbandonSuccess != null)
            {
                //删除成功触发方法
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        /// <summary>
        /// 执行数据逻辑删除
        /// </summary>
        virtual protected void OnAbandon()
        {
            using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Invoices>().Count(item => item.ConsigneeID == this.ID &&
                item.Status == (int)ActionStatus.Normal);

                if(count > 0)
                {
                    if (this != null && this.AbandonError != null)
                    {
                        //删除失败触发事件
                        this.AbandonError(this, new ErrorEventArgs("开票信息中有该地址信息，不能删除!"));
                    }
                }

                reponsitory.Update<Layer.Data.Sqls.BvCrm.Consignees>(new
                {
                    Status = Status.Delete
                }, item => item.ID == this.ID);
            }
        }
        #endregion
    }


    /// <summary>
    /// 交货人 [发信人]
    /// </summary>
    public class Deliverer : Consignee
    {
    }
}
