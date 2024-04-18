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
    /// 公司
    /// </summary>
    public class Company : Needs.Linq.IUnique, Needs.Linq.IPersist
    {
        //string id;
        //public string ID
        //{
        //    get
        //    {
        //        //根据公司编号和名称确定唯一性
        //        return this.id ?? string.Concat(this.Name).MD5();
        //    }
        //    set
        //    {
        //        this.id = value;
        //    }
        //}



         public string ID { get; set; }

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
        /// 检验检疫编码
        /// </summary>
        public string CIQCode { get; set; }

        /// <summary>
        /// 法人
        /// </summary>
        public string Corporate { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public Contact Contact { get; set; }
        public string ContactID { get; set; }


        /// <summary>
        /// 状态
        /// </summary>
        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public string Summary { get; set; }

        public string Address { get; set; }

        public Company()
        {
            this.UpdateDate = this.CreateDate = DateTime.Now;
            this.Status = Enums.Status.Normal;
        }

        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;

        public void Enter()
        {
            this.Contact.Enter();

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
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.Companies
                    {
                        ID = ChainsGuid.NewGuidUp(),
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
                    var company = companies.Where(item => item.ID == this.ID).SingleOrDefault();
                    //var companyType = (CompanyType)company.Type;

                    reponsitory.Update(new Layer.Data.Sqls.ScCustoms.Companies
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
            }
        }
    }
}
