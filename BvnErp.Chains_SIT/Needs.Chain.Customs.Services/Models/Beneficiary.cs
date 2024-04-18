using Needs.Ccs.Services.Enums;
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
    /// 受益人
    /// </summary>
    public class Beneficiary : Needs.Linq.IUnique, Needs.Linq.IPersist
    {
        string id;
        public string ID
        {
            get
            {
                //根据公司编号和名称确定唯一性
                return this.id ?? string.Concat(this.Company.Name, this.BankAccount).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 公司
        /// </summary>
        public Company Company { get; set; }

        /// <summary>
        /// 账户名称
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// 银行账户
        /// </summary>
        public string BankAccount { get; set; }

        /// <summary>
        /// 银行地址
        /// </summary>
        public string BankAddress { get; set; }

        /// <summary>
        /// 银行名称
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// 银行国际代码
        /// </summary>
        public string SwiftCode { get; set; }

        /// <summary>
        /// 数据状态
        /// </summary>
        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public string Summary { get; set; }

        public Beneficiary()
        {
            this.CreateDate = DateTime.Now;
            this.Status = Enums.Status.Normal;
        }

        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;

        public void Enter()
        {
            this.OnEnter();

            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        virtual public void OnEnter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var companies = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>();
                int count = companies.Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    reponsitory.Insert(null);
                }
                else
                {
                    var company = companies.Where(item => item.ID == this.ID).SingleOrDefault();
                    //reponsitory.Update(null, item => item.ID == this.ID);
                }
            }
        }
    }
}
