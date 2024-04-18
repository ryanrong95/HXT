using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NtErp.Crm.Services.Extends;
using Needs.Utils.Converters;

namespace NtErp.Crm.Services.Models
{
    public class Charge : IUnique, IPersist
    {
        #region 属性
        string id;
        /// <summary>
        /// 主键ID
        /// </summary>
        public string ID
        {
            get
            {
                return this.id ?? string.Concat(this.Name + DateTime.Now).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID
        {
            get; set;
        }

        /// <summary>
        /// 行动ID
        /// </summary>
        public string ActionID
        {
            get; set;
        }
        /// <summary>
        /// 费用人
        /// </summary>
        public AdminTop Admin
        {
            get; set;
        }
        /// <summary>
        /// 费用人ID
        /// </summary>
        public string AdminID
        {
            get; set;
        }
        /// <summary>
        /// 费用名称
        /// </summary>
        public string Name
        {
            get; set;
        }
        /// <summary>
        /// 费用数量
        /// </summary>
        public int? Count
        {
            get; set;
        }
        /// <summary>
        /// 费用价格
        /// </summary>
        public decimal Price
        {
            get; set;
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate
        {
            get; set;
        }
        /// <summary>
        /// 描述
        /// </summary>
        public string Summary
        {
            get; set;
        }
        /// <summary>
        /// 客户
        /// </summary>
        public Client Clients
        {
            get; set;
        }
        /// <summary>
        /// 行动计划
        /// </summary>
        public Plan Actions
        {
            get; set;
        }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public Charge()
        {
            this.CreateDate = DateTime.Now;
        }

        public event SuccessHanlder EnterSuccess;


        /// <summary>
        /// 持久化触发方法
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
        protected void OnEnter()
        {
            using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
            {
                //判定数据是否存在
                int count = reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Charges>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.BvCrm.Charges
                    {
                        ID = this.ID,
                        ClientID = this.ClientID,
                        ActionID = this.ActionID,
                        AdminID = this.AdminID,
                        Name = this.Name,
                        Count = this.Count,
                        Price = this.Price,
                        CreateDate = this.CreateDate,
                        Summary = this.Summary,
                    });
                }
            }
        }
    }
}
