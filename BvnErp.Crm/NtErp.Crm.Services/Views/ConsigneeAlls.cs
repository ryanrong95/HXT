using Layer.Data.Sqls;
using Needs.Erp.Generic;
using Needs.Linq;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Views
{
    public class ConsigneeAlls : UniqueView<Consignee, BvCrmReponsitory>, Needs.Underly.IFkoView<Consignee>
    {
        //人员对象
        IGenericAdmin Admin;

        /// <summary>
        /// 无参构造函数
        /// </summary>
        public ConsigneeAlls()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="admin">人员对象</param>
        public ConsigneeAlls(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="bv">数据库实体</param>
        public ConsigneeAlls(BvCrmReponsitory bv) : base(bv) { }

        /// <summary>
        /// 获取收货人数据集合
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Consignee> GetIQueryable()
        {  
            //联系人视图
            ContactAlls ContactView = new ContactAlls(this.Reponsitory);
            //品牌视图
            CompanyAlls CompanyVeiw = new CompanyAlls(this.Reponsitory);

            return from consignees in base.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Consignees>()                  
                   join contact in ContactView on consignees.ContactID equals contact.ID                  
                   where consignees.Status != (int)Enums.Status.Delete
                   select new Consignee
                   {
                       ID = consignees.ID,
                       CompanyID = consignees.CompanyID,
                       Contact = contact,
                       ClientID = consignees.ClientID,
                       Address = consignees.Address,
                       Zipcode = consignees.Zipcode,
                       Status = (Enums.Status)consignees.Status,
                       CreateDate = consignees.CreateDate,
                       UpdateDate = consignees.UpdateDate,
                   };
        }
    }
}
