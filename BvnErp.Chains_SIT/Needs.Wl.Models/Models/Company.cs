using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Model;
using Needs.Utils.Converters;
using System;
using System.Linq;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 公司
    /// </summary>
    public class Company : ModelBase<Layer.Data.Sqls.ScCustoms.Companies, ScCustomsReponsitory>, Needs.Linq.IUnique, Needs.Linq.IPersist
    {
        string id;
        public new string ID
        {
            get
            {
                //根据公司编号和名称确定唯一性
                return this.id ?? string.Concat(this.Name, this.Code).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 统一社会信用编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 海关编码
        /// </summary>
        public string CustomsCode { get; set; }

        /// <summary>
        /// 法人
        /// </summary>
        public string Corporate { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public Contact Contact { get; set; }

        /// <summary>
        /// 公司注册地址
        /// </summary>
        public string Address { get; set; }

        public Company()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
        }

        public event SuccessHanlder EnterSuccess;

        public override void Enter()
        {
            this.Contact.Enter();

            var companies = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>();
            int count = companies.Count(item => item.ID == this.ID);

            if (count == 0)
            {
                this.Reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.Companies
                {
                    ID = this.ID,
                    ContactID = this.Contact.ID,
                    Code = this.Code,
                    Name = this.Name,
                    CustomsCode = this.CustomsCode,
                    Corporate = this.Corporate,
                    Address = this.Address,
                    Status = (int)this.Status,
                    CreateDate = this.CreateDate,
                    UpdateDate = this.UpdateDate,
                    Summary = this.Summary
                });
            }
            else
            {
                this.Reponsitory.Update(new Layer.Data.Sqls.ScCustoms.Companies
                {
                    ID = this.ID,
                    ContactID = this.Contact.ID,
                    Code = this.Code,
                    Name = this.Name,
                    CustomsCode = this.CustomsCode,
                    Corporate = this.Corporate,
                    Address = this.Address,
                    Status = (int)this.Status,
                    CreateDate = this.CreateDate,
                    UpdateDate = this.UpdateDate,
                    Summary = this.Summary
                }, item => item.ID == this.ID);
            }

            this.OnEnter();
        }

        virtual public void OnEnter()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }
    }
}