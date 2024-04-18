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
    public class InvoiceAll: UniqueView<Invoice, BvCrmReponsitory>, Needs.Underly.IFkoView<Invoice>
    {
        //人员对象
        IGenericAdmin Admin;

        /// <summary>
        /// 无参构造函数
        /// </summary>
        protected InvoiceAll()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="admin">人员对象</param>
        public InvoiceAll(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="bv">数据库实体</param>
        public InvoiceAll(BvCrmReponsitory bv) : base(bv) { }

        /// <summary>
        /// 获取发票数据集合
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Invoice> GetIQueryable()
        {     
            //收货人视图
            ConsigneeAlls consignees = new ConsigneeAlls(this.Reponsitory);

            return from invoice in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Invoices>()
                   join consignee in consignees on invoice.ConsigneeID equals consignee.ID                 
                   where invoice.Status == (int)ActionStatus.Normal
                   select new Invoice
                   {
                       ID = invoice.ID,
                       InvoiceTypes = (InvoiceType)invoice.Type,
                       ClientID = consignee.ClientID,
                       CompanyCode = invoice.CompanyCode,
                       Address = invoice.Address,
                       BankName = invoice.Bank,
                       Account = invoice.BankAccount,
                       Contacts = consignee.Contact,
                       Consignee = consignee,
                       Phone = invoice.Phone,
                       Status = (ActionStatus)invoice.Status,
                   };
        }


    }
}
